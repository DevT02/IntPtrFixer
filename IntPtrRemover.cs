using dnlib.DotNet;
using dnlib.DotNet.Emit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntptrPoint
{
    class IntPtrRemover
    {
        public static int amount = 0;
        public static void Clean()
        {
            foreach (TypeDef types in Program.module.GetTypes())
            {
                foreach (MethodDef method in types.Methods)
                {
                    if (!method.HasBody) continue;
                    IList<Instruction> instr = method.Body.Instructions;
                    for (int i = 0; i < instr.Count; i++)
                    {
                        if (method.Body.Instructions[i].OpCode == OpCodes.Call && (instr[i].Operand.ToString().ToLower().Contains("intptr::subtract") || instr[i].Operand.ToString().ToLower().Contains("intptr::add")) && instr[i - 1].OpCode.ToString().Contains("ldc.i4") && instr [i - 2].Operand.ToString().ToLower().Contains("intptr::op_explicit") && instr[i - 3].OpCode.ToString().Contains("ldc.i4"))
                        {
                            /// CODE: IntPtr.Add((IntPtr)1, 259)
                            /// In Dnspy:
                            // 15  0037    ldc.i4  1
                            // 16  003C    call    native int[mscorlib] System.IntPtr::op_Explicit(int32)
                            // 17  0041    ldc.i4  0x103 
                            // 18  0046    call native int[mscorlib] System.IntPtr::Add(native int, int32)

                            /// NOTE: 0x103 = 259
                            // op_Explicit is (IntPtr)
                            // IntPtr::Add is IntPtr.Add
                            // Compiling this in an online compiler will result in 260. IntPtr mutations don't prioritize pointers. In other words, this statement is literally just 1 + 259 = 260.  
                            // Stil not convinced? https://offline.is-ne.at/vayAi8   https://offline.is-ne.at/P2kHoN
                            // Same goes for IntPtr::Subtract. (except the adding part ofc)                   

                            int y = instr[i - 1].GetLdcI4Value(), x = instr[i - 3].GetLdcI4Value(), result;
                            if (instr[i].Operand.ToString().ToLower().Contains("intptr::subtract"))
                            {
                                result = x - y;
                                instr[i].OpCode = OpCodes.Nop;
                                instr[i - 1].OpCode = OpCodes.Nop;
                                instr[i - 2].OpCode = OpCodes.Nop;                              
                                instr[i - 3].Operand = result;
                                amount++;
                            }
                            else if(instr[i].Operand.ToString().ToLower().Contains("intptr::add"))
                            {
                                result = x + y;
                                instr[i].OpCode = OpCodes.Nop;
                                instr[i - 1].OpCode = OpCodes.Nop;
                                instr[i - 2].OpCode = OpCodes.Nop;
                                instr[i - 3].Operand = result;
                                amount++;
                            }
       
                        }

                    }
                }
            }
        }
    }
}
