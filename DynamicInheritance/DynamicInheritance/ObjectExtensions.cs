using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
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

            var aName = new AssemblyName(newAssemblyName);
            var ab =
                AppDomain.CurrentDomain.DefineDynamicAssembly(
                    aName,
                    AssemblyBuilderAccess.RunAndSave);

            // For a single-module assembly, the module name is usually 
            // the assembly name plus an extension.
            ModuleBuilder mb =
                ab.DefineDynamicModule(aName.Name, aName.Name + ".dll");

            var tb = mb.DefineType(objType,TypeAttributes.Public);

            tb.SetParent(typeof(T));

            var methodsToCopy = obj.GetType().GetMethods();
            var field = tb.DefineField("_originalObject", obj.GetType(), FieldAttributes.Private);

          
            foreach (var method in methodsToCopy)
            {
                var body = method.GetMethodBody();
                if (body == null)
                    continue;
                
                
                var newMethod = tb.DefineMethod(method.Name, method.Attributes, method.CallingConvention,
                                                method.ReturnType,
                                                method.GetParameters().Select(x => x.ParameterType).ToArray());

                var ilReader = new ILReader();

                var code = ilReader.ReadIL(method);

                

                var bytes = body.GetILAsByteArray();
                var ilg = newMethod.GetILGenerator();
                
                ilg.
                
                newMethod.InitLocals = body.InitLocals;
                foreach (var variable in body.LocalVariables)
                {
                    ilg.DeclareLocal(variable.LocalType);
                }

                newMethod.CreateMethodBody(bytes, bytes.Length);
                
                


            }

            return tb.CreateType();


        }

        public class ILReader
        {
            public class Instruction
            {
                public int StartOffset { get; private set; }
                public OpCode OpCode { get; private set; }
                public long? Argument { get; private set; }
                public Instruction(int startOffset, OpCode opCode, long? argument)
                {
                    StartOffset = startOffset;
                    OpCode = opCode;
                    Argument = argument;
                }
                public override string ToString()
                {
                    return OpCode.ToString() + (Argument == null ? string.Empty : " " + Argument.Value);
                }
            }

            private Dictionary<short, OpCode> _opCodeList;

            public ILReader()
            {
                _opCodeList = typeof(OpCodes).GetFields().Where(f => f.FieldType == typeof(OpCode)).Select(f => (OpCode)f.GetValue(null)).ToDictionary(o => o.Value);
            }

            public IEnumerable<Instruction> ReadIL(MethodBase method)
            {
                MethodBody body = method.GetMethodBody();
                if (body == null)
                    yield break;

                int offset = 0;
                byte[] il = body.GetILAsByteArray();
                while (offset < il.Length)
                {
                    int startOffset = offset;
                    byte opCodeByte = il[offset];
                    short opCodeValue = opCodeByte;
                    offset++;

                    // If it's an extended opcode then grab the second byte. The 0xFE prefix codes aren't marked as prefix operators though.
                    if (opCodeValue == 0xFE || _opCodeList[opCodeValue].OpCodeType == OpCodeType.Prefix)
                    {
                        opCodeValue = (short)((opCodeValue << 8) + il[offset]);
                        offset++;
                    }

                    OpCode code = _opCodeList[opCodeValue];

                    Int64? argument = null;

                    int argumentSize = 4;
                    if (code.OperandType == OperandType.InlineNone)
                        argumentSize = 0;
                    else if (code.OperandType == OperandType.ShortInlineBrTarget || code.OperandType == OperandType.ShortInlineI || code.OperandType == OperandType.ShortInlineVar)
                        argumentSize = 1;
                    else if (code.OperandType == OperandType.InlineVar)
                        argumentSize = 2;
                    else if (code.OperandType == OperandType.InlineI8 || code.OperandType == OperandType.InlineR)
                        argumentSize = 8;
                    else if (code.OperandType == OperandType.InlineSwitch)
                    {
                        long num = il[offset] + (il[offset + 1] << 8) + (il[offset + 2] << 16) + (il[offset + 3] << 24);
                        argumentSize = (int)(4 * num + 4);
                    }

                    // This does not currently handle the 'switch' instruction meaningfully.
                    if (argumentSize > 0)
                    {
                        Int64 arg = 0;
                        for (int i = 0; i < argumentSize; ++i)
                        {
                            Int64 v = il[offset + i];
                            arg += v << (i * 8);
                        }
                        argument = arg;
                        offset += argumentSize;
                    }

                    yield return new Instruction(startOffset, code, argument);
                }
            }
        }
    }
}
