using System;
using System.Linq;
using System.Reflection;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace DynamicInheritance
{
    public static class CecilExtensions
    {
        public static MethodDefinition Copy(this MethodDefinition methodToCopy, TypeDefinition typeToAddMethodTo)
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
                    newInstruction.Operand = typeToAddMethodTo.Fields.First(x => x.Name == fieldDefinition.Name);
                }

                if (newInstruction.Operand is MethodReference)
                {
                    var method = (MethodReference)newInstruction.Operand;

                    var imported = targetModule.Import(method);
                    var refff = targetModule.Import(method.ReturnType).Resolve();



                    newInstruction = Instruction.Create(newInstruction.OpCode, imported);


                }
                if (newInstruction.Operand is TypeReference)
                {
                    targetModule.Import(newInstruction.Operand as TypeReference);
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
