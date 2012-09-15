using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Mono.Cecil;

namespace DynamicInheritance
{
    public static class ObjectExtensions
    {
        public static Type AddBaseType<T>(this object obj)
        {
            
            var objType = obj.GetType().Name;
            var newAssemblyName = obj.GetAssemblyDefinition().Name.Name;

            var resolver = new DefaultAssemblyResolver();

            var newModule = ModuleDefinition.CreateModule("The Module",new ModuleParameters{Runtime = TargetRuntime.Net_4_0,AssemblyResolver = resolver, Kind = ModuleKind.Dll});
            var baseType = typeof(T).GetImportedTypeReference(newModule);

            var typeToCopy = obj.GetTypeDefinition();
            var methodsToCopy = typeToCopy.Methods;

            var newType = new TypeDefinition(newAssemblyName, objType + "New", typeToCopy.Attributes, baseType);

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
            
            return dynamicType;

        }


        public static TypeDefinition GetTypeDefinition(this object obj)
        {
            return obj.GetAssemblyDefinition().MainModule.Types.First(x => x.Name.Equals(obj.GetType().Name));

        }
        public static AssemblyDefinition GetAssemblyDefinition(this object obj)
        {
            return AssemblyDefinition.ReadAssembly(obj.GetType().Assembly.Location);
        }
        
        public static TypeReference GetImportedTypeReference(this Type type, ModuleDefinition moduleToImportTo)
        {
            var assembly = AssemblyDefinition.ReadAssembly(type.Assembly.Location);

            var typeRef = assembly.MainModule.Types.First(x => x.Name.Equals(type.Name));
            return moduleToImportTo.Import(typeRef);

        }


    }
}