using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace DTimeControl.Parallel_Nightmare
{
    public static class TypeUtility
    {
        public static IEnumerable<Type> AllGenericTypes(Type type)
        {
            return  from x in Assembly.GetAssembly(typeof(Game)).GetTypes()
                    let y = x.BaseType
                    where !x.IsAbstract && !x.IsInterface &&
                    y != null && y.IsGenericType &&
                    y.GetGenericTypeDefinition() == type
                    select x;
        }
    }
}
