using System;
using System.Collections.Generic;
using System.Text;
using System.Linq.Expressions;
namespace Clover.Core.Reflection
{
    
    
    
    
    public static class FastActivator
    {
        
        private static Dictionary<Type, Func<object>> factoryCache = new Dictionary<Type, Func<object>>();

        
        public static T Create<T>()
        {
            return TypeFactory<T>.Create();
        }

        public static object Create(Type type)
        {
            Func<object> f;
            if (!factoryCache.TryGetValue(type, out f))
            {
                lock (factoryCache)
                {
                    if (!factoryCache.TryGetValue(type, out f))
                    {
                        factoryCache[type] = f = Expression.Lambda<Func<object>>(Expression.New(type), new ParameterExpression[0]).Compile();
                    }
                }
            }
            return f();
        }

        
        private static class TypeFactory<T>
        {
            
            public static readonly Func<T> Create;

            
            static TypeFactory()
            {
                FastActivator.TypeFactory<T>.Create = Expression.Lambda<Func<T>>(Expression.New(typeof(T)), new ParameterExpression[0]).Compile();
            }
        }
    }


}
