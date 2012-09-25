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

            var baseDefinition = baseType.Resolve();

            foreach(var iface in typeToCopy.Interfaces)
            {
                newType.Interfaces.Add(newModule.Import(iface));
            }


            typeToCopy.GenericParameters.ToList().ForEach(x => newType.GenericParameters.Add(x));
            newModule.Types.Add(newType);

            foreach (var field in typeToCopy.Fields)
            {
                var baseField = baseDefinition.Fields.FirstOrDefault(y => y.Name.Equals(field.Name));
                if (baseField != null)
                {

                    //var newBaseField = baseField.Copy(newType);
                    //newType.Fields.Add(newBaseField);

                    continue;
                }


                var newField = field.Copy(newType);
                newType.Fields.Add(newField);
            }
            foreach (var method in methodsToCopy)
            {
                var newMethod = method.Copy(newType, baseDefinition, typeof(T));
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