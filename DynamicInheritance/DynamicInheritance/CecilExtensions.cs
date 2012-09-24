using System;
using System.Linq;
using System.Reflection;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;

namespace DynamicInheritance
{
    public static class CecilExtensions
    {
        public static FieldDefinition Copy(this FieldDefinition fieldToCopy, TypeDefinition typeToAddFieldTo)
        {
            var targetModule = typeToAddFieldTo.Module;

            var fieldType = fieldToCopy.FieldType;
            if (fieldType.HasGenericParameters)
            {
                fieldType = fieldType.MakeGenericType(targetModule, ((GenericInstanceType) fieldType).GenericArguments);
            }

            
            var newField = new FieldDefinition(fieldToCopy.Name, fieldToCopy.Attributes, targetModule.Import(fieldType));

            return newField;
        }
        public static MethodDefinition Copy(this MethodDefinition methodToCopy, TypeDefinition typeToAddMethodTo, TypeDefinition baseTypeDefinition, Type baseType)
        {
            var targetModule = typeToAddMethodTo.Module;


            var newMethod = new MethodDefinition(methodToCopy.Name, methodToCopy.Attributes, targetModule.Import(methodToCopy.ReturnType));
            foreach (var variableDefinition in methodToCopy.Body.Variables)
            {
                newMethod.Body.Variables.Add(new VariableDefinition(targetModule.Import(variableDefinition.VariableType)));


            }
            foreach (var parameterDefinition in methodToCopy.Parameters)
            {
                newMethod.Parameters.Add(new ParameterDefinition(targetModule.Import(parameterDefinition.ParameterType)));
            }

            
            foreach (var instruction in methodToCopy.Body.Instructions)
            {
                var constructorInfo = typeof(Instruction).GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new[] { typeof(OpCode), typeof(object) }, null);
                var newInstruction = (Instruction)constructorInfo.Invoke(new[] { instruction.OpCode, instruction.Operand });
                var fieldDefinition = newInstruction.Operand as FieldDefinition;

                if (fieldDefinition != null)
                {
                    targetModule.Import(fieldDefinition.FieldType);


                    newInstruction.Operand = typeToAddMethodTo.Fields.FirstOrDefault(x => x.Name == fieldDefinition.Name);

                    if (newInstruction.Operand == null)
                    {
                        var importedField = targetModule.Import(baseTypeDefinition.Fields.First(x => x.Name == fieldDefinition.Name));
                        newInstruction.Operand = importedField;
                    }
                    
                }

                if (newInstruction.Operand is MethodReference)
                {
                    
                    var method = (MethodReference)newInstruction.Operand;

                    if (methodToCopy.IsConstructor && newInstruction.OpCode == OpCodes.Call)
                    {
                        method = baseTypeDefinition.Methods.First(x => x.IsConstructor && !x.HasParameters);
                        if (baseTypeDefinition.HasGenericParameters)
                            method = method.MakeGeneric(targetModule, baseType.GetGenericArguments());

                    }else if(methodToCopy.IsConstructor && newInstruction.OpCode == OpCodes.Newobj)
                    {
                        
                    }
                    
                    var imported = targetModule.Import(method);
                    
                    newInstruction = Instruction.Create(newInstruction.OpCode, imported);


                }
                if (newInstruction.Operand is TypeReference)
                {
                    targetModule.Import(newInstruction.Operand as TypeReference);
                }
                if (newInstruction.OpCode == OpCodes.Stfld)
                {
                    var field = (FieldReference) newInstruction.Operand;

                    var baseField = baseTypeDefinition.Fields.FirstOrDefault(x => x.Name.Equals(field.Name));

                    if (baseField != null)
                    {
                        var importedBaseField = baseField.Copy(typeToAddMethodTo);
                        newInstruction = Instruction.Create(newInstruction.OpCode, importedBaseField);
                    }
                }
                newMethod.Body.Instructions.Add(newInstruction);
            }


            

            return newMethod;
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



            return typeRef.GetImportedTypeReference(type, moduleToImportTo);

        }

        public static TypeReference GetImportedTypeReference(this TypeReference typeRef, Type type, ModuleDefinition moduleToImportTo)
        {
            
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

        public static TypeReference MakeGenericType(this TypeReference original, ModuleDefinition moduleToImportTo, Collection<TypeReference> arguments)
        {
            var genericType = new GenericInstanceType(original);

            foreach (var param in original.GenericParameters)
            {
                genericType.GenericParameters.Add(param);
            }

            foreach (var arg in arguments)
            {

                genericType.GenericArguments.Add(arg);
            }

            return genericType;
        }

        public static MethodReference MakeGeneric(this MethodReference self, ModuleDefinition module, params Type[] arguments)
        {
            var reference = new MethodReference(self.Name, self.ReturnType)
            {
                DeclaringType = self.DeclaringType.MakeGenericType( module,arguments),
                HasThis = self.HasThis,
                ExplicitThis = self.ExplicitThis,
                CallingConvention = self.CallingConvention,
            };

            foreach (var parameter in self.Parameters)
                reference.Parameters.Add(new ParameterDefinition(parameter.ParameterType));

            foreach (var generic_parameter in self.GenericParameters)
                reference.GenericParameters.Add(new GenericParameter(generic_parameter.Name, reference));

            return reference;
        }
    }
}
