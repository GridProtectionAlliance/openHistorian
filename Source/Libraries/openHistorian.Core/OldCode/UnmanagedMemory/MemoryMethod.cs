//using System.Reflection.Emit;

//namespace openHistorian.UnmanagedMemory
//{
//    internal unsafe class MemoryMethod
//    {
//        /// <summary>
//        /// Internal simple interop signature description.
//        /// </summary>
//        //public class CalliSignature
//        //{
//        //    public CalliSignature(string name, Type returnType, Type[] parameterTypes)
//        //    {
//        //        Name = name;
//        //        ReturnType = returnType;
//        //        ParameterTypes = parameterTypes;
//        //    }

//        //    public string Name;
//        //    public Type ReturnType;
//        //    public Type[] ParameterTypes;
//        //}

//        //private static void CreateMemcpy(TypeBuilder tb)
//        //{
//        //    MethodBuilder methodCopyStruct;

//        //    methodCopyStruct = tb.DefineMethod("memcpy",
//        //                                    MethodAttributes.Public | MethodAttributes.Static | MethodAttributes.HideBySig,
//        //                                    CallingConventions.Standard);

//        //    methodCopyStruct.SetReturnType(typeof(void));
//        //    methodCopyStruct.SetParameters(new Type[] { typeof(void*), typeof(void*), typeof(int) });


//        //    ParameterBuilder pDest = methodCopyStruct.DefineParameter(1, ParameterAttributes.None, "pDest");
//        //    ParameterBuilder pSrc = methodCopyStruct.DefineParameter(2, ParameterAttributes.None, "pSrc");
//        //    ParameterBuilder count = methodCopyStruct.DefineParameter(3, ParameterAttributes.None, "count");

//        //    ILGenerator gen = methodCopyStruct.GetILGenerator();

//        //    gen.Emit(OpCodes.Ldarg_0);
//        //    gen.Emit(OpCodes.Ldarg_1);
//        //    gen.Emit(OpCodes.Ldarg_2);
//        //    if (IntPtr.Size == 8)
//        //        gen.Emit(OpCodes.Unaligned, 1);

//        //    // Memcpy
//        //    gen.Emit(OpCodes.Cpblk);
//        //    // Ret
//        //    gen.Emit(OpCodes.Ret);
//        //}

//        public delegate void MemCpyFunction(void* des, void* src, uint bytes);

//        public static readonly MemCpyFunction MemCpy;

//        static MemoryMethod()
//        {
//            var dynamicMethod = new DynamicMethod
//            (
//                "MemCpy",
//                typeof(void),
//                new[] { typeof(void*), typeof(void*), typeof(uint) },
//                typeof(MemoryMethod)
//            );

//            var ilGenerator = dynamicMethod.GetILGenerator();

//            ilGenerator.Emit(OpCodes.Ldarg_0);
//            ilGenerator.Emit(OpCodes.Ldarg_1);
//            ilGenerator.Emit(OpCodes.Ldarg_2);

//            ilGenerator.Emit(OpCodes.Cpblk);
//            ilGenerator.Emit(OpCodes.Ret);

//            MemCpy = (MemCpyFunction)dynamicMethod.CreateDelegate(typeof(MemCpyFunction));
//        }
//    }
//}

