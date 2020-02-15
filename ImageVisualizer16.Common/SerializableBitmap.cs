using System;
using System.Drawing;
using System.Reflection.Emit;
using System.Runtime.Serialization;

namespace ImageVisualizer
{
    [Serializable]
    public class SerializableBitmap : ISerializable
    {
        private Bitmap image;

        public SerializableBitmap(Bitmap image)
        {
            this.image = image;
        }

        private SerializableBitmap(SerializationInfo info, StreamingContext context)
        {
            var ci = image.GetType().GetConstructor(new Type[] { typeof(SerializationInfo), typeof(StreamingContext) });
            if (ci != null)
            {
                ci.Invoke(new object[] { info, context });
            }
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            var mi = image.GetType().GetMethod(nameof(GetObjectData), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (mi != null)
            {
                InvokeNotOverride(mi, image, new object[] { info, context });
            }
        }

        public static object InvokeNotOverride(/*this*/ System.Reflection.MethodInfo methodInfo, object targetObject, params object[] arguments)
        {
            var parameters = methodInfo.GetParameters();

            if (parameters.Length == 0)
            {
                if (arguments != null && arguments.Length != 0)
                    throw new Exception("Arguments cont doesn't match");
            }
            else
            {
                if (parameters.Length != arguments.Length)
                    throw new Exception("Arguments cont doesn't match");
            }

            Type returnType = null;
            if (methodInfo.ReturnType != typeof(void))
            {
                returnType = methodInfo.ReturnType;
            }

            var type = targetObject.GetType();
            var dynamicMethod = new DynamicMethod("", returnType, new Type[] { type, typeof(object) }, type);

            var iLGenerator = dynamicMethod.GetILGenerator();
            iLGenerator.Emit(System.Reflection.Emit.OpCodes.Ldarg_0); // this

            for (int i = 0; i < parameters.Length; i++)
            {
                var parameter = parameters[i];

                iLGenerator.Emit(System.Reflection.Emit.OpCodes.Ldarg_1); // load array argument

                // get element at index
                iLGenerator.Emit(System.Reflection.Emit.OpCodes.Ldc_I4_S, i); // specify index
                iLGenerator.Emit(System.Reflection.Emit.OpCodes.Ldelem_Ref); // get element

                var parameterType = parameter.ParameterType;
                if (parameterType.IsPrimitive)
                {
                    iLGenerator.Emit(System.Reflection.Emit.OpCodes.Unbox_Any, parameterType);
                }
                else if (parameterType == typeof(object))
                {
                    // do nothing
                }
                else
                {
                    iLGenerator.Emit(System.Reflection.Emit.OpCodes.Castclass, parameterType);
                }
            }

            iLGenerator.Emit(System.Reflection.Emit.OpCodes.Call, methodInfo);
            iLGenerator.Emit(System.Reflection.Emit.OpCodes.Ret);

            return dynamicMethod.Invoke(null, new object[] { targetObject, arguments });
        }

        /*public static object InvokeNotOverride(*//*this*//* System.Reflection.ConstructorInfo constructorInfo, object targetObject, params object[] arguments)
        {
            var parameters = constructorInfo.GetParameters();

            if (parameters.Length == 0)
            {
                if (arguments != null && arguments.Length != 0)
                    throw new Exception("Arguments cont doesn't match");
            }
            else
            {
                if (parameters.Length != arguments.Length)
                    throw new Exception("Arguments cont doesn't match");
            }

            var baseType = typeof(MyBaseType);
            var baseConstructor = baseType.GetConstructor(BindingFlags.Public | BindingFlags.FlattenHierarchy | BindingFlags.Instance, null, new Type[0], null);

            // Create a Type Builder that generates a type directly into the current AppDomain.
            var appDomain = AppDomain.CurrentDomain;
            var assemblyName = new AssemblyName(&quot; MyDynamicAssembly & quot;);
            var assemblyBuilder = appDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndSave);
            var moduleBuilder = assemblyBuilder.DefineDynamicModule(assemblyName.Name);

            var typeBuilder = moduleBuilder.DefineType(&quot; MyDynamicType & quot;, TypeAttributes.Class | TypeAttributes.Public, baseType);

            // Create a parameterless (default) constructor.
            var constructor = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, null);

            var ilGenerator = constructor.GetILGenerator();

            // Generate constructor code
            ilGenerator.Emit(OpCodes.Ldarg_0);                // push &quot;this&quot; onto stack.
            ilGenerator.Emit(OpCodes.Call, baseConstructor);  // call base constructor

            ilGenerator.Emit(OpCodes.Nop);                    // C# compiler add 2 NOPS, so
            ilGenerator.Emit(OpCodes.Nop);                    // we'll add them, too.

            ilGenerator.Emit(OpCodes.Ret);                    // Return
        }*/
    }
}
