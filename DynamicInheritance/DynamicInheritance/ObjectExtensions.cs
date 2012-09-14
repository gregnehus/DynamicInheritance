﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Mono.Cecil;
using Mono.Cecil.Cil;
using FieldAttributes = System.Reflection.FieldAttributes;
using OpCode = Mono.Cecil.Cil.OpCode;
using TypeAttributes = System.Reflection.TypeAttributes;

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


            var originalAssembly = AssemblyDefinition.ReadAssembly(obj.GetType().Assembly.Location);
            var typeD = originalAssembly.MainModule.Types.First(x => x.Name.Equals(objType));
            var methodsToCopy = typeD.Methods;


            var newType = new TypeDefinition(newAssemblyName, objType+"New", Mono.Cecil.TypeAttributes.Public | Mono.Cecil.TypeAttributes.Class,newModule.TypeSystem.Object);
            
            newModule.Types.Add(newType);
            assy.Modules.Add(newModule);
            foreach (var method in methodsToCopy)
            {
                
                var newMethod = ObjectExtensions.CopyMethod(method,newType, assy);

                newType.Methods.Add(newMethod);


            }


            Type dynamicType;

            using (var stream = new MemoryStream())
            {
                newModule.Write(stream);
                Assembly ass = Assembly.Load(stream.ToArray());


                dynamicType = ass.GetType(newType.FullName);


          
            }

                
            
            
            return dynamicType;




        }



        private static MethodDefinition CopyMethod(MethodDefinition templateMethod, TypeDefinition targetType, AssemblyDefinition assemblyDefinition)
        {
            var targetModule = targetType.Module;

            var newMethod = new MethodDefinition(templateMethod.Name, templateMethod.Attributes,targetModule.Import(templateMethod.ReturnType));
            foreach (var variableDefinition in templateMethod.Body.Variables)
            {
                newMethod.Body.Variables.Add(new VariableDefinition(targetModule.Import(variableDefinition.VariableType)));


            }
            foreach (var parameterDefinition in templateMethod.Parameters)
            {
                newMethod.Parameters.Add(new ParameterDefinition(targetModule.Import(parameterDefinition.ParameterType)));
            }
            foreach (var instruction in templateMethod.Body.Instructions)
            {
                var constructorInfo = typeof(Instruction).GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new[] { typeof(OpCode), typeof(object) },null);
                var newInstruction = (Instruction)constructorInfo.Invoke(new[] { instruction.OpCode ,instruction.Operand});
                var fieldDefinition = newInstruction.Operand as FieldDefinition;

                if (fieldDefinition != null)
                {
                    targetModule.Import(fieldDefinition.FieldType);
                    newInstruction.Operand = targetType.Fields.First(x=> x.Name == fieldDefinition.Name);
                }

                if (newInstruction.Operand is MethodReference)
                {
                    var method = (MethodReference) newInstruction.Operand;

                    assemblyDefinition.Modules.Add(method.Module);
                    var imported = targetModule.Import(method);
                    var refff = targetModule.Import(method.ReturnType).Resolve();



                    newInstruction = Instruction.Create(newInstruction.OpCode, imported);

                    //targetModule.Import((MethodReference) newInstruction.Operand).Resolve();

                    //newInstruction.OpCode = newInstruction.OpCode;
                    //newInstruction.Operand = newInstruction.Operand;


                    ////Try really hard to import type
                    //var methodReference = (MethodReference)newInstruction.Operand;

                    //targetModule.Import(methodReference.MethodReturnType.ReturnType);
                    //targetModule.Import(methodReference.DeclaringType);
                    //targetModule.Import(methodReference);
                }
                if (newInstruction.Operand is TypeReference)
                {
                    targetModule.Import(newInstruction.Operand as TypeReference);
                }
                newMethod.Body.Instructions.Add(newInstruction);
            }
            
            return newMethod;
        }


    }
}