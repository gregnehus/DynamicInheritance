using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Mono.Cecil;
using Mono.Cecil.Cil;
using FieldAttributes = System.Reflection.FieldAttributes;
using TypeAttributes = System.Reflection.TypeAttributes;

namespace DynamicInheritance
{
    public static class ObjectExtensions
    {
        public static Type AddBaseType<T>(this object obj)
        {
            var objType = obj.GetType().Name;
            var assembly = obj.GetType().Assembly;
            var newAssemblyName = "DynamicAssembly";



            var aName = new AssemblyNameDefinition(newAssemblyName, new Version());
            var assy = AssemblyDefinition.CreateAssembly(aName, objType, ModuleKind.Windows);
            var newType = new TypeDefinition(newAssemblyName, objType, Mono.Cecil.TypeAttributes.Public);


            var newModule = ModuleDefinition.CreateModule(objType,ModuleKind.NetModule);
            
            var originalAssembly = AssemblyDefinition.ReadAssembly(obj.GetType().Assembly.Location);
            var typeD = originalAssembly.MainModule.Types.First(x => x.Name.Equals(objType));
            var methodsToCopy = typeD.Methods;
            
            foreach (var method in methodsToCopy)
            {
                var typedef = typeD.Resolve();

                var newMethod = new MethodDefinition(method.Name, new Mono.Cecil.MethodAttributes(), newType);
                
                var il = newMethod.Body.GetILProcessor();
                foreach (var inst in method.Body.Instructions)
                {
                    if (inst.)
                    il.Create(inst.OpCode, inst.Offset);
                }
                newMethod.CallingConvention = method.CallingConvention;
                newMethod.DeclaringType = method.DeclaringType;
                
                newType.Methods.Add(newMethod);

                //var body = method.GetMethodBody();
                //if (body == null)
                //    continue;


                //var newMethod = tb.DefineMethod(method.Name, method.Attributes, method.CallingConvention,
                //                                method.ReturnType,
                //                                method.GetParameters().Select(x => x.ParameterType).ToArray());







                //newMethod.CreateMethodBody(bytes, bytes.Length);




            }
            newModule.Types.Add(newType);

            assy.Modules.Add(newModule);

            var typeToRet = Type.GetType(newType.FullName + ", " + assy.FullName);
            return typeToRet;




        }


    }
}
