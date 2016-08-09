using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using OpcodeDef = M68K.OpcodeDefinition32;

namespace M68K {
	public partial class CPU {
		int CurrentInstructionLength;
		Queue<int> TrapQueue = new Queue<int>();

		public MemoryMappedDevice Memory;

		public int[] DA = new int[16];
		public int PC;
		public int CCR;

		public bool CCR_C_Carry { get { return CCR.GetBit(0); } set { CCR = CCR.SetBit(0, value); } }
		public bool CCR_V_Overflow { get { return CCR.GetBit(1); } set { CCR = CCR.SetBit(1, value); } }
		public bool CCR_Z_Zero { get { return CCR.GetBit(2); } set { CCR = CCR.SetBit(2, value); } }
		public bool CCR_N_Sign { get { return CCR.GetBit(3); } set { CCR = CCR.SetBit(3, value); } }
		public bool CCR_X_Extend { get { return CCR.GetBit(4); } set { CCR = CCR.SetBit(4, value); } }
		public bool CCR_5 { get { return CCR.GetBit(5); } set { CCR = CCR.SetBit(5, value); } }
		public bool CCR_6 { get { return CCR.GetBit(6); } set { CCR = CCR.SetBit(6, value); } }
		public bool CCR_7 { get { return CCR.GetBit(7); } set { CCR = CCR.SetBit(7, value); } }
		public bool CCR_I0 { get { return CCR.GetBit(8); } set { CCR = CCR.SetBit(8, value); } }
		public bool CCR_I1 { get { return CCR.GetBit(9); } set { CCR = CCR.SetBit(9, value); } }
		public bool CCR_I2 { get { return CCR.GetBit(10); } set { CCR = CCR.SetBit(10, value); } }
		public bool CCR_11 { get { return CCR.GetBit(11); } set { CCR = CCR.SetBit(11, value); } }
		public bool CCR_M { get { return CCR.GetBit(12); } set { CCR = CCR.SetBit(12, value); } }
		public bool CCR_S { get { return CCR.GetBit(13); } set { CCR = CCR.SetBit(13, value); } }
		public bool CCR_T0 { get { return CCR.GetBit(14); } set { CCR = CCR.SetBit(14, value); } }
		public bool CCR_T1 { get { return CCR.GetBit(15); } set { CCR = CCR.SetBit(15, value); } }

		public virtual ulong HandleData(OpSize Size, ulong Addressing, bool Set = false, ulong Value = 0) {
			byte Mode = (byte)((Addressing >> 8) & 0xFF);
			byte Data = (byte)(Addressing & 0xFF);

			// Convert data to register number (Dn or An) for modes 0 .. 6
			int Reg = Data;
			if (Mode >= 1 && Mode <= 6)
				Reg += 8;

			switch (Mode) {
				// 000/001 Dn, An
				case 0:
				case 1:
					if (Set)
						DA[Reg] = (int)Value;
					else
						return (ulong)DA[Reg];
					break;

				// 010
				case 2:
					break;

				// 011
				case 3:
					break;

				// 100
				case 4:
					break;

				// 101
				case 5:
					break;

				// 110
				case 6:
					break;

				// 111
				case 7: {
						switch (Data) {
							// 000
							case 0:
								break;

							// 001
							case 1:
								break;

							// 010
							case 2:
								break;

							// 011
							case 3:
								break;

							// 100 imm, implied
							case 4:
								if (Set)
									throw new InvalidOperationException();

								CurrentInstructionLength += (int)Size;
								return Memory.Read(Size, PC + 2);

							default:
								throw new InvalidOperationException();
						}
						break;
					}

				default:
					throw new InvalidOperationException();
			}

			return 0;
		}

		public virtual void SetData(OpSize Size, ulong Addressing, ulong Value) {
			HandleData(Size, Addressing, true, Value);
		}

		public virtual ulong GetData(OpSize Size, ulong Addressing) {
			return HandleData(Size, Addressing);
		}

		public virtual void Step() {
			if (TrapQueue.Count > 0) {
				int TrapVector = TrapQueue.Dequeue();
				//Console.WriteLine("Trap #{0}, not handled", TrapVector);
			}


			ushort Word0 = Memory.Read16(PC + sizeof(ushort) * 0);
			ushort Word1 = Memory.Read16(PC + sizeof(ushort) * 1);
			ushort Word2 = Memory.Read16(PC + sizeof(ushort) * 2);
			ushort Word3 = Memory.Read16(PC + sizeof(ushort) * 3);
			ushort Word4 = Memory.Read16(PC + sizeof(ushort) * 4);

			uint Instr = (uint)((Word0 << 16) | Word1);
			Opcode Opcode = MatchOpcode(Instr, out CurrentInstructionLength);

#if DEBUG
			Console.ForegroundColor = ConsoleColor.White;
			Console.Write("0x{0:X8}: ", PC);
			Console.ResetColor();

			OpcodeDef OpcodeDef = MatchOpcodeDef(Instr);
			if (OpcodeDef != null) {
				string Bin = Ext.ToBinary(Instr);
				string Pattern = OpcodeDef.Higher.Pattern + OpcodeDef.Lower.Pattern;

				Console.Write("0x{0:X8} ", Instr);

				for (int i = 0; i < Pattern.Length; i++) {
					if (Pattern[i] == '?')
						Console.ForegroundColor = ConsoleColor.Green;
					else
						Console.ForegroundColor = ConsoleColor.Gray;

					Console.Write(Bin[i]);
				}

				Console.ResetColor();
				Console.Write(" {0}", Opcode);
			} else
				Console.Write("0x{0:X8} {1} {2}", Instr, Ext.ToBinary(Instr), Opcode);
			Console.WriteLine();
#endif

			switch (Opcode) {
				case Opcode.ILLEGAL_OPCODE:
					break;
				case Opcode.UNIMPLEMENTED:
					break;
				case Opcode.ORItoCCR:
					break;
				case Opcode.ORItoSR:
					break;
				case Opcode.ORI:
					break;
				case Opcode.ANDItoCCR:
					break;
				case Opcode.ANDItoSR:
					break;
				case Opcode.ANDI:
					break;
				case Opcode.SUBI:
					break;
				case Opcode.RTM:
					break;
				case Opcode.CALLM:
					break;
				case Opcode.CMP2:
					break;
				case Opcode.CHK2:
					break;
				case Opcode.EORItoCCR:
					break;
				case Opcode.ADDI:
					break;
				case Opcode.EORItoSR:
					break;
				case Opcode.EORI:
					break;
				case Opcode.CMPI:
					break;
				case Opcode.BTST:
					break;
				case Opcode.BCHG:
					break;
				case Opcode.BCLR:
					break;
				case Opcode.BSET:
					break;
				case Opcode.MOVES:
					break;
				case Opcode.CAS2:
					break;
				case Opcode.CAS:
					break;
				case Opcode.MOVEP:
					break;
				case Opcode.MOVE_BYTE:
					Move(OpSize.BYTE, Word0);
					break;
				case Opcode.MOVEA_BYTE:
					break;
				case Opcode.MOVE_LONG:
					Move(OpSize.LONG, Word0);
					break;
				case Opcode.MOVEA_LONG:
					break;
				case Opcode.MOVE_WORD:
					Move(OpSize.WORD, Word0);
					break;
				case Opcode.MOVEA_WORD:
					break;
				case Opcode.MOVEfromSR:
					break;
				case Opcode.MOVEfromCCR:
					break;
				case Opcode.NEGX:
					break;
				case Opcode.CLR:
					break;
				case Opcode.MOVEtoCCR:
					break;
				case Opcode.NEG:
					break;
				case Opcode.NOT:
					break;
				case Opcode.MOVEtoSR:
					break;
				case Opcode.EXT_EXTB:
					break;
				case Opcode.LINK_LONG:
					break;
				case Opcode.NBCD:
					break;
				case Opcode.SWAP:
					break;
				case Opcode.BKPT:
					break;
				case Opcode.PEA:
					break;
				case Opcode.BGND:
					break;
				case Opcode.ILLEGAL:
					break;
				case Opcode.TAS:
					break;
				case Opcode.TST:
					break;
				case Opcode.MULU_LONG:
					break;
				case Opcode.MULS_LONG:
					break;
				case Opcode.DIVU_DIVUL_LONG:
					break;
				case Opcode.DIVS_DIVSL_LONG:
					break;
				case Opcode.TRAP:
					Trap(Word0.GetBits(3, 0));
					break;
				case Opcode.LINK_WORD:
					break;
				case Opcode.UNLK:
					break;
				case Opcode.MOVE_USP:
					break;
				case Opcode.RESET:
					break;
				case Opcode.NOP:
					break;
				case Opcode.STOP:
					break;
				case Opcode.RTE:
					break;
				case Opcode.RTD:
					break;
				case Opcode.RTS:
					break;
				case Opcode.TRAPV:
					break;
				case Opcode.RTR:
					break;
				case Opcode.MOVEC:
					break;
				case Opcode.JSR:
					break;
				case Opcode.JMP:
					break;
				case Opcode.MOVEM:
					break;
				case Opcode.LEA:
					break;
				case Opcode.CHK:
					break;
				case Opcode.ADDQ:
					break;
				case Opcode.SUBQ:
					break;
				case Opcode.DBcc:
					break;
				case Opcode.TRAPcc:
					break;
				case Opcode.Scc:
					break;
				case Opcode.BRA:
					break;
				case Opcode.BSR:
					break;
				case Opcode.Bcc:
					break;
				case Opcode.MOVEQ:
					break;
				case Opcode.DIVU_DIVUL_WORD:
					break;
				case Opcode.SBCD:
					break;
				case Opcode.PACK:
					break;
				case Opcode.UNPK:
					break;
				case Opcode.DIVS_DIVSL_WORD:
					break;
				case Opcode.OR:
					break;
				case Opcode.SUBX:
					break;
				case Opcode.SUB_SUBA:
					break;
				case Opcode.CMPM:
					break;
				case Opcode.CMP_CMPA_EOR:
					break;
				case Opcode.MULU_WORD:
					break;
				case Opcode.ABCD:
					break;
				case Opcode.MULS_WORD:
					break;
				case Opcode.EXG:
					break;
				case Opcode.AND:
					break;
				case Opcode.ADDX:
					break;
				case Opcode.ADD_ADDA:
					break;
				case Opcode.ASL_ASR_MEM_SHIFT:
					break;
				case Opcode.LSL_LSR_MEM_SHIFT:
					break;
				case Opcode.ROXL_ROXR_MEM_ROTATE:
					break;
				case Opcode.ROL_ROR_MEM_ROTATE:
					break;
				case Opcode.BFTST:
					break;
				case Opcode.BFEXTU:
					break;
				case Opcode.BFCHG:
					break;
				case Opcode.BFEXTS:
					break;
				case Opcode.BFCLR:
					break;
				case Opcode.BFFFO:
					break;
				case Opcode.BFSET:
					break;
				case Opcode.BFINS:
					break;
				case Opcode.ASL_ASR_REG_SHIFT:
					break;
				case Opcode.LSL_LSR_REG_SHIFT:
					break;
				case Opcode.ROXL_ROXR_REG_ROTATE:
					break;
				case Opcode.ROL_ROR_REG_ROTATE:
					break;
				default:
					break;
			}

			PC += CurrentInstructionLength;
		}
	}
}