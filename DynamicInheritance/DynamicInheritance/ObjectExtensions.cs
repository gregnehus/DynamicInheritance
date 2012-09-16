using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Mono.Cecil;

namespace DynamicInheritance
{
    public static class ObjectExtensions
    {
        public static dynamic GetInstanceWithBaseType<T>(this Type typeToAddBaseTo)
        {
            
            var objType = typeToAddBaseTo.Name;
            var newAssemblyName = typeToAddBaseTo.GetAssemblyDefinition().Name.Name;

            var resolver = new DefaultAssemblyResolver();

            var newModule = ModuleDefinition.CreateModule("The Module",new ModuleParameters{Runtime = TargetRuntime.Net_4_0,AssemblyResolver = resolver, Kind = ModuleKind.Dll});
            
            
            var baseType = typeof(T).GetImportedTypeReference(newModule);
            
            
            var typeToCopy = typeToAddBaseTo.GetTypeDefinition();
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


            if (dynamicType.ContainsGenericParameters)
            {
                dynamicType = dynamicType.MakeGenericType(typeToAddBaseTo.GetGenericArguments());
            }
            var instance = Activator.CreateInstance(dynamicType);
            return instance;

        }


        public static TypeDefinition GetTypeDefinition(this Type obj)
        {
            return obj.GetAssemblyDefinition().MainModule.Types.First(x => x.Name.Equals(obj.Name));

        }
        public static AssemblyDefinition GetAssemblyDefinition(this Type obj)
        {
            return AssemblyDefinition.ReadAssembly(obj.Assembly.Location);
        }
        
        public static TypeReference GetImportedTypeReference(this Type type, ModuleDefinition moduleToImportTo)
        {
            var assembly = AssemblyDefinition.ReadAssembly(type.Assembly.Location);

            var typeRef = assembly.MainModule.Types.First(x => x.Name.Equals(type.Name));


            var ret = moduleToImportTo.Import(typeRef);

            if (type.IsGenericType)
            {
                ret = ret.MakeGenericType(moduleToImportTo, type.GetGenericArguments());
            }
            
            return ret;

        }

        public static TypeReference MakeGenericType(this TypeReference original, ModuleDefinition moduleToImportTo, params Type[] arguments)
        {
            var genericType = new GenericInstanceType(original);

            foreach (var param in original.GenericParameters)
            {
                genericType.GenericParameters.Add(param);
            }

            foreach (var arg in arguments)
            {

                genericType.GenericArguments.Add(moduleToImportTo.Import(arg));
            }

            return genericType;
        }


    }
}