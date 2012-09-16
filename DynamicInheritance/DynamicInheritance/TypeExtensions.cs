using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Mono.Cecil;

namespace DynamicInheritance
{
    public static class TypeExtensions
    {
        public static T GetInstanceWithBaseType<T>(this Type typeToAddBaseTo) where T : class
        {
            
            var objType = typeToAddBaseTo.Name;
            var newAssemblyName = typeToAddBaseTo.GetAssemblyDefinition().Name.Name;

            var resolver = new DefaultAssemblyResolver();

            var newModule = ModuleDefinition.CreateModule("DynamicObjects",new ModuleParameters{Runtime = TargetRuntime.Net_4_0,AssemblyResolver = resolver, Kind = ModuleKind.Dll});
            
            
            var baseType = typeof(T).GetImportedTypeReference(newModule);
            
            
            var typeToCopy = typeToAddBaseTo.GetTypeDefinition();
            var methodsToCopy = typeToCopy.Methods;

            var newType = new TypeDefinition(newAssemblyName, objType + "New", typeToCopy.Attributes, baseType);

            foreach(var iface in typeToCopy.Interfaces)
            {
                newType.Interfaces.Add(newModule.Import(iface));
            }


            typeToCopy.GenericParameters.ToList().ForEach(x => newType.GenericParameters.Add(x));
            newModule.Types.Add(newType);

            foreach (var method in methodsToCopy)
            {
                var newMethod = method.Copy(newType);
                newType.Methods.Add(newMethod);
            }


            Type dynamicType;
            using (var stream = new MemoryStream())
            {
                newModule.Write(stream);
                var assembly = Assembly.Load(stream.ToArray());

                dynamicType = assembly.GetType(newType.FullName);
            }


            if (dynamicType.ContainsGenericParameters)
            {
                dynamicType = dynamicType.MakeGenericType(typeToAddBaseTo.GetGenericArguments());
            }
            var instance = Activator.CreateInstance(dynamicType);
            return instance as T;

        }

    }
}