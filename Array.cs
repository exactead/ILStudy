using System.Reflection;
using System.Reflection.Emit;

class Array
{
    /*
    string ArrayTest()
    {
        var strings = new[]{"月","火","水","木","金","土","日"};
        return string.Join(", ", strings);
    }
    */

    public Array()
    {
        // .NET8に期待
        // 以下は動きません

        string[] array = new[] { "月", "火", "水", "木", "金", "土", "日" };
        string moduleName = "ArrayTestIl";
        string methodName = "ArrayMake";
        // RunAndSave...
        var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName($"{Guid.NewGuid()}"), AssemblyBuilderAccess.Run);
        var moduleBuilder = assemblyBuilder.DefineDynamicModule(moduleName);
        var typeBuilder = moduleBuilder.DefineType("Foo", TypeAttributes.Public);
        var dm = typeBuilder.DefineMethod(methodName, MethodAttributes.Public, typeof(string), null);

        // Method
        var joinMethod = typeof(string).GetMethod("Join", new[] { typeof(string), typeof(IEnumerable<string>) });

        var il = dm.GetILGenerator();
        var strings = il.DeclareLocal(typeof(string[]));
        var local = il.DeclareLocal(typeof(string));

        // Array Create
        il.Emit(OpCodes.Ldc_I4, array.Length);
        il.Emit(OpCodes.Newarr, typeof(string));
        for (int i = 0; i < array.Length; i++)
        {
            il.Emit(OpCodes.Dup);
            il.Emit(OpCodes.Ldc_I4, i);
            il.Emit(OpCodes.Ldstr, array[i]);
            il.Emit(OpCodes.Ldloc, strings);
        }

        // Join
        il.Emit(OpCodes.Stloc, strings);
        il.Emit(OpCodes.Ldstr, ",");
        il.Emit(OpCodes.Ldloc, strings);

        il.EmitCall(OpCodes.Call, joinMethod, null);
        il.Emit(OpCodes.Stloc, local);
        il.Emit(OpCodes.Ldloc, local);
        il.Emit(OpCodes.Ret);

        Type type = typeBuilder.CreateType();
        var instance = Activator.CreateInstance(type);
        // NET45-48
        //assemblyBuilder.Save($"{moduleName}.dll");   
        var result = type.GetMethod(methodName).Invoke(instance, null);
        Console.WriteLine(result);
    }
}