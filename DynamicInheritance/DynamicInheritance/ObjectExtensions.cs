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
            var newAssemblyName = "DynamicAssembly";

            var resolver = new DefaultAssemblyResolver();

            var aName = new AssemblyNameDefinition(newAssemblyName, new Version());
            var assy = AssemblyDefinition.CreateAssembly(aName, objType, ModuleKind.Dll);
            

            var corLib = resolver.Resolve("mscorlib");

            var newModule = ModuleDefinition.CreateModule("The Module",new ModuleParameters{Runtime = TargetRuntime.Net_4_0,AssemblyResolver = resolver, Kind = ModuleKind.Dll});

            newModule.Import(corLib.MainModule.GetType("System.Object"));
            newModule.Import(corLib.MainModule.GetType("System.String"));


            var typeToCopy = obj.GetTypeDefinition();
            var methodsToCopy = typeToCopy.Methods;


            var newType = new TypeDefinition(newAssemblyName, objType+"New", Mono.Cecil.TypeAttributes.Public | Mono.Cecil.TypeAttributes.Class,newModule.TypeSystem.Object);
            
            newModule.Types.Add(newType);
            assy.Modules.Add(newModule);
            foreach (var method in methodsToCopy)
            {
                
                var newMethod = method.Copy(newType, assy);

                newType.Methods.Add(newMethod);


            }


            Type dynamicType;
            newModule.Write("C:\\test.dll");
            using (var stream = new MemoryStream())
            {
                newModule.Write(stream);
                Assembly ass = Assembly.Load(stream.ToArray());


                dynamicType = ass.GetType(newType.FullName);


          
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
        


    }
}