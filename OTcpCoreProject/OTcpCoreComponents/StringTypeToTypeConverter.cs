using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace OTcpCoreComponents
{
    public class StringTypeToTypeConverter
    {
        public static Type Convert(string typeName)
        {
            try
            {
                Type t = Type.GetType(typeName);
                return t;
            }
            catch (Exception e)
            {
                throw e;
            }
            
        }
        public static string GetStringTypeName<T>(T obj)
        {
            try
            {
                string fullName = typeof(T).FullName;
                return fullName;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public static Type GetTypeFrom(string valueType)
        {
            var type = Type.GetType(valueType);
            if (type != null)
                return type;

            try
            {
                var assemblies = AppDomain.CurrentDomain.GetAssemblies();

                //To speed things up, we check first in the already loaded assemblies.
                foreach (var assembly in assemblies)
                {
                    type = assembly.GetType(valueType);
                    if (type != null)
                        break;
                }
                if (type != null)
                    return type;

                var loadedAssemblies = assemblies.ToList();

                foreach (var loadedAssembly in assemblies)
                {
                    foreach (AssemblyName referencedAssemblyName in loadedAssembly.GetReferencedAssemblies())
                    {
                        var found = loadedAssemblies.All(x => x.GetName() != referencedAssemblyName);

                        if (!found)
                        {
                            try
                            {
                                var referencedAssembly = Assembly.Load(referencedAssemblyName);
                                type = referencedAssembly.GetType(valueType);
                                if (type != null)
                                    break;
                                loadedAssemblies.Add(referencedAssembly);
                            }
                            catch
                            {
                                //We will ignore this, because the Type might still be in one of the other Assemblies.
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                //throw my custom exception    
            }

            if (type == null)
            {
                //throw my custom exception.
            }

            return type;
        }
    }
}
