using System.Reflection.Emit;

class Calc
{
    public Calc()
    {
        // 簡単な計算であればDynamicMethodでOK
        // 変数等を使う場合は別途用意する必要あり
        DynamicMethod dm = new("Sum41", typeof(int), new[] { typeof(int), typeof(int) });   ;
        var il = dm.GetILGenerator();
        il.Emit(OpCodes.Ldarg_0); // 引数0
        il.Emit(OpCodes.Ldarg_1); // 引数0,引数1
        il.Emit(OpCodes.Add);  // 引数0 + 引数1
        il.Emit(OpCodes.Ldc_I4,41); // 引数0 + 引数1,定数41        
        il.Emit(OpCodes.Add); // 引数0 + 引数1 + 定数41
        il.Emit(OpCodes.Ret);
        var sum = dm.CreateDelegate(typeof(Func<int,int,int>)) as Func<int,int,int>;
        Console.WriteLine(sum(10,20));
    }
}

