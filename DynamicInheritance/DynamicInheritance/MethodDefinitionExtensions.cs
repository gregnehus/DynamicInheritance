using System.Linq;
using System.Reflection;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace DynamicInheritance
{
    public static class MethodDefinitionExtensions
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
    }
}
