namespace Clover.Core.Reflection
{
    #region 引用的程序集.
    using System;
    using System.IO;
    using System.Threading;
    using System.Reflection;
    using Clover.I18n;
    using System.Reflection.Emit;
    using System.Collections.Generic;
    using System.Linq;
    #endregion

    
    
    
    public class ReflectorHelper
    {
        #region 反射使用的标记
        
        
        
        const BindingFlags setFlags = BindingFlags.Public |
                                      BindingFlags.SetField|
                                      BindingFlags.SetProperty|
                                      BindingFlags.GetProperty|
                                      BindingFlags.Instance |
                                      BindingFlags.Static |
                                      BindingFlags.GetField ;


        
        
        
        const BindingFlags getFlags = BindingFlags.Public |
                                          BindingFlags.NonPublic |
                                          BindingFlags.SetProperty |
                                          BindingFlags.Instance |
                                          BindingFlags.Static |
                                          BindingFlags.SetField;

        
        
        
        const BindingFlags methodFlags = BindingFlags.Public | BindingFlags.NonPublic |                                         
                                          BindingFlags.InvokeMethod |
                                          BindingFlags.Instance |
                                          BindingFlags.Static;
        #endregion

        
        
        
        
        
        
        public static void SetObjPropertyValue(object target, string propertyName,params object[] newValue)
        {
            Type type = target.GetType();
            if(type != null)
            {
                type.InvokeMember(propertyName, setFlags, null, target, newValue);
            }
        }


        
        
        
        
        
        
        
        public static Dictionary<string,string> CompareObjProperty(object source, object target, string[] propnames)
        {
            Type type = source.GetType();
            Type ttype = target.GetType();
            var rst = new Dictionary<string, string>();
            bool specNames = propnames!=null && propnames.Length > 0;
            if (type != null)
            {
                var props = type.GetProperties();
                foreach (var p in props)
                {
                    if (specNames && !propnames.Contains(p.Name))
                        continue;

                    var s = p.GetValue(source, null);
                    var t = p.GetValue(target, null);
                    if (!(s == null && t == null) && ((s == null && t != null) || (s != null && t == null) || !s.Equals(t)))
                    {
                        rst.Add(p.Name, string.Format("{0} NotEq {1}", s, t));
                    }
                    else
                    {
                        rst.Add(p.Name, string.Format("{0} Eq {1}", s, t));
                    }
                }
            }
            return rst;
        }


        
        
        
        
        
        
        
        public static object GetObjPropertyValue(object target, string propertyName)
        {
            Type type = target.GetType();
            if (type != null)
            {
                return type.InvokeMember(propertyName, getFlags, null, target, null);
            }
            else
                return null;
        }


        
        
        
        
        
        
        public delegate object GenericInvoker(object target, params object[] arguments);


        
        
        
        
        
        
        
        
        
        private static void FindMethod(Type type, string methodName, Type[] typeArguments, Type[] parameterTypes, 
            out MethodInfo methodInfo,
            out ParameterInfo[] parameters)
        {

            methodInfo = null;
            parameters = null;

            if (null == parameterTypes)
            {
                methodInfo = type.GetMethod(methodName, methodFlags);
                
                if (typeArguments != null)
                
                    methodInfo = methodInfo.MakeGenericMethod(typeArguments);
                
                
                parameters = methodInfo.GetParameters();
            }
            else
            {
                
                
                
                MethodInfo[] methods = type.GetMethods(methodFlags);
                foreach (MethodInfo method in methods)
                {
                    if (method.Name == methodName) 
                    {
                        
                        MethodInfo genericMethod = method.MakeGenericMethod(typeArguments);
                        parameters = genericMethod.GetParameters();

                        
                        if (parameters.Length == parameterTypes.Length)
                        {
                            for (int i = 0; i < parameters.Length; i++)
                            {
                                if (parameters[i].ParameterType != parameterTypes[i])
                                {
                                    continue; 
                                }
                            }

                            
                            methodInfo = genericMethod;
                            break;
                        }
                    }
                }

                if (null == methodInfo)
                {
                    throw new InvalidOperationException(
                        Strings.T("Can't Find the method '{0}'",methodName)
                     );
                }
            }
        }

        
        
        
        
        
        
        
        public static GenericInvoker MethodInvokerMethod(object obj, string methodName, Type[] parameterTypes)
        {
            return GenericMethodInvokerMethod(obj.GetType(), methodName, null, parameterTypes);
        }


           
        
        
        
        
        
        
        public static GenericInvoker MethodInvokerMethod(Type type, string methodName, Type[] parameterTypes)
        {
            return GenericMethodInvokerMethod(type, methodName, null,parameterTypes);
        }

        
        
        
        
        
        
        
        
        public static GenericInvoker GenericMethodInvokerMethod(Type type, string methodName, Type[] typeArguments, Type[] parameterTypes)
        {
            MethodInfo methodInfo;
            ParameterInfo[] parameters;

            
            FindMethod(type, methodName, typeArguments, parameterTypes, out methodInfo, out parameters);

            string name = string.Format("__MethodInvoker_{0}_ON_{1}", methodInfo.Name, methodInfo.DeclaringType.Name);

            
            DynamicMethod dynamicMethod = new DynamicMethod(name, typeof(object), new Type[] { typeof(object), typeof(object[]) },
              methodInfo.DeclaringType);

            
            ILGenerator generator = dynamicMethod.GetILGenerator();

            
            generator.DeclareLocal(typeof(object));

            
            generator.Emit(OpCodes.Ldarg_0);

            
            generator.Emit(OpCodes.Castclass, methodInfo.DeclaringType);

            for (int i = 0; i < parameters.Length; i++)
            {
                
                generator.Emit(OpCodes.Ldarg_1);

                
                generator.Emit(OpCodes.Ldc_I4, i);
                generator.Emit(OpCodes.Ldelem_Ref);

                
                Type parameterType = parameters[i].ParameterType;
                if (parameterType.IsClass)
                {
                    generator.Emit(OpCodes.Castclass, parameterType);
                }
                else
                {
                    generator.Emit(OpCodes.Unbox_Any, parameterType);
                }
            }

            
            generator.EmitCall(OpCodes.Callvirt, methodInfo, null);

            
            if (methodInfo.ReturnType == typeof(void))
            {
                
                generator.Emit(OpCodes.Ldnull);
            }
            else
            {
                
                if (methodInfo.ReturnType.IsValueType)
                {
                    generator.Emit(OpCodes.Box, methodInfo.ReturnType);
                }
            }

            
            generator.Emit(OpCodes.Stloc_0);

            
            generator.Emit(OpCodes.Ldloc_0);
            generator.Emit(OpCodes.Ret);

            
            return (GenericInvoker)dynamicMethod.CreateDelegate(typeof(GenericInvoker));
        }

        
        
        
        
        
        
        
        public static GenericInvoker GenericMethodInvokerMethod(Type type, string methodName, Type[] typeArguments)
        {
            return GenericMethodInvokerMethod(type, methodName, typeArguments, null);
        }
    }
}
