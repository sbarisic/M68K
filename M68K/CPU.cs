using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using OpcodeDef = M68K.OpcodeDefinition32;

namespace M68K {
	public partial class CPU {
		public const int D0 = 0, D1 = 1, D2 = 2, D3 = 3, D4 = 4, D5 = 5, D6 = 6, D7 = 7,
			A0 = 8, A1 = 9, A2 = 10, A3 = 11, A4 = 12, A5 = 13, A6 = 14, A7 = 15,
			SP = 15;

		int InstrLen;
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
					if (Set) {
						switch (Size) {
							case OpSize._8:
								DA[Reg] = (DA[Reg] & ~0xFF) + (int)(Value & 0xFF);
								break;
							case OpSize._16:
								DA[Reg] = (DA[Reg] & ~0xFFFF) + (int)(Value & 0xFFFF);
								break;
							case OpSize._32:
								DA[Reg] = (int)(DA[Reg] & ~0xFFFFFFFF) + (int)(Value & 0xFFFFFFFF);
								break;
								;
						}
					} else
						return (ulong)DA[Reg];
					break;

				// 010 (An)
				case 2:
					if (Set)
						Memory.Write(Size, DA[Reg], Value);
					else
						return Memory.Read(Size, DA[Reg]);
					break;

				// 011 (An)+
				case 3: {
						ulong Ret = 0;

						if (Set)
							Memory.Write(Size, DA[Reg], Value);
						else
							Ret = Memory.Read(Size, DA[Reg]);

						int Inc = (int)Size;
						if (Inc < 2 && Reg == 15) // If not even and register A7
							Inc = 2;

						DA[Reg] += Inc; // TOOD: implement after instruction executes
						return Ret;
					}

				// 100 -(An)
				case 4: {
						int Inc = (int)Size;
						if (Inc < 2 && Reg == 15) // If not even and register A7
							Inc = 2;

						DA[Reg] -= Inc; // TODO: implement before instruction executes

						if (Set)
							Memory.Write(Size, DA[Reg], Value);
						else
							return Memory.Read(Size, DA[Reg]);
						break;
					}

				// 101 (d16, An)
				case 5:
					if (Set) {
						Memory.Write(Size, DA[Reg] + (short)Memory.Read16(PC + InstrLen), Value);
						InstrLen += 2;
					} else {
						short Offset = (short)Memory.Read16(PC + InstrLen);
						ulong Ret = Memory.Read(Size, DA[Reg] + Offset);
						InstrLen += 2;
						return Ret;
					}
					break;

				// 110
				case 6:
					throw new NotImplementedException();

				// 111
				case 7: {
						switch (Data) {
							// 000 addr16
							case 0: {
									ulong Addr = Memory.Read16(PC + InstrLen);
									InstrLen += 2;

									if (Set)
										Memory.Write(Size, (int)Addr, Value);
									return Addr;
								}

							// 001 addr32
							case 1: {
									ulong Addr = Memory.Read32(PC + InstrLen);
									InstrLen += 4;

									if (Set)
										Memory.Write(Size, (int)Addr, Value);
									return Addr;
								}

							// 010 d16(PC)
							case 2: {
									if (Set)
										throw new InvalidOperationException();

									short Addr = (short)Memory.Read16(PC + InstrLen);
									InstrLen += 2;

									return (ulong)(PC + Addr + 2);
								}

							// 011
							case 3:
								throw new NotImplementedException();

							// 100 imm, implied
							case 4:
								if (Set)
									throw new InvalidOperationException();

								if (Size == OpSize.BYTE)
									Size = OpSize.WORD;

								ulong Val = Memory.Read(Size, PC + InstrLen);
								InstrLen += (int)Size;
								return Val;

							default:
								throw new InvalidOperationException();
						}
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
				Console.WriteLine("Trap #{0}, not handled", TrapVector);
			}

			bool IncPC = true;

			ushort Word0 = Memory.Read16(PC + sizeof(ushort) * 0);
			ushort Word1 = Memory.Read16(PC + sizeof(ushort) * 1);
			ushort Word2 = Memory.Read16(PC + sizeof(ushort) * 2);
			ushort Word3 = Memory.Read16(PC + sizeof(ushort) * 3);
			ushort Word4 = Memory.Read16(PC + sizeof(ushort) * 4);

			uint Instr = (uint)((Word0 << 16) | Word1);
			ulong Instr2 = ((ulong)Word0 << 64) | ((ulong)Word1 << 48) | ((ulong)Word2 << 32) | ((ulong)Word3 << 16) | Word4;

			Opcode Opcode = MatchOpcode(Instr);
			InstrLen = 2;

#if DEBUG
			/*Console.Write("                       ");
			for (int i = 16 - 1; i >= 0; i--)
				Console.Write("{0:X}", i);
			Console.WriteLine();*/

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
					throw new InvalidOperationException();
				case Opcode.UNIMPLEMENTED:
					throw new NotImplementedException();
					
				case Opcode.ADDI: {
						ushort DstEAddr = Decode_EAddr(((ulong)Word0).GetBits(5, 0));
						OpSize Size = Decode_Size(((ulong)Word0).GetBits(7, 6));

						ulong Src = GetData(Size, MODE_IMMEDIATE);
						ulong Dest = GetData(Size, DstEAddr);
						SetData(Size, DstEAddr, Src + Dest);

						break;
					}
					
				case Opcode.MOVE_BYTE: {
						Move(OpSize.BYTE, Word0);
						break;
					}

				case Opcode.MOVE_WORD: {
						Move(OpSize.WORD, Word0);
						break;
					}

				case Opcode.MOVE_LONG: {
						Move(OpSize.LONG, Word0);
						break;
					}

				case Opcode.PEA: {
						ushort EAddr = Decode_EAddr(((ulong)Word0).GetBits(5, 0));
						uint Val = (uint)GetData(OpSize.LONG, EAddr);

						DA[SP] -= 4;
						Memory.Write32(DA[SP], Val);

						break;
					}

				case Opcode.TRAP: {
						Trap(Word0.GetBits(3, 0));
						break;
					}

				case Opcode.UNLK: {
						ulong Register = Word0.GetBits(2, 0);

						DA[SP] = (int)GetData(OpSize.LONG, MODE_REGISTER_A | Register);
						SetData(OpSize.LONG, MODE_REGISTER_A | Register, Memory.Read32(DA[SP]));
						DA[SP] += 4;

						break;
					}

				case Opcode.LINK_WORD: {
						ulong Register = Word0.GetBits(2, 0);
						int Immediate = (int)GetData(OpSize.WORD, MODE_IMMEDIATE);

						DA[SP] -= 4;
						Memory.Write32(DA[SP], (uint)GetData(OpSize.LONG, MODE_REGISTER_A | Register));
						SetData(OpSize.LONG, MODE_REGISTER_A | Register, (ulong)DA[SP]);
						DA[SP] = DA[SP] + Immediate;

						break;
					}
					
				case Opcode.NOP: {
						break;
					}

				case Opcode.JMP:
				case Opcode.JSR: {
						ushort EAddr = Decode_EAddr(((ulong)Word0).GetBits(5, 0));
						ulong Addr = GetData(OpSize.LONG, EAddr);

						if (Opcode == Opcode.JSR) {
							DA[SP] -= 4;
							Memory.Write32(DA[SP], (uint)(PC + InstrLen));
						}
						PC = (int)Addr;

						IncPC = false;
						break;
					}

				case Opcode.RTS: {
						PC = (int)Memory.Read32(DA[SP]);
						DA[SP] += 4;

						IncPC = false;
						break;
					}
				
				case Opcode.MULU_WORD:
				case Opcode.MULU_LONG:
				case Opcode.MULS_WORD:
				case Opcode.MULS_LONG: {
						OpSize Size = (Opcode == Opcode.MULS_WORD || Opcode == Opcode.MULU_WORD) ? OpSize.WORD : OpSize.LONG;

						ulong SrcEAddr = Decode_EAddr(Word0.GetBits(5, 0));
						ulong DstEAddr = Size == OpSize.LONG ?
							(MODE_REGISTER_D | Word1.GetBits(14, 12)) : (MODE_REGISTER_D | Word0.GetBits(11, 9));

						if (Size == OpSize.LONG)
							InstrLen += 2;

						ulong Src = GetData(Size, SrcEAddr);
						ulong Dst = GetData(Size, DstEAddr);
						ulong Mul = (Opcode == Opcode.MULS_WORD || Opcode == Opcode.MULS_LONG) ?
							(ulong)((long)Src * (long)Dst) : Src * Dst;

						SetData(Size, DstEAddr, Mul & 0xFFFFFFFF);
						if (Size == OpSize.LONG && Word1.GetBit(10)) {
							ulong RegDh = MODE_REGISTER_D | Word1.GetBits(2, 0);
							ulong Dh = GetData(Size, RegDh);
							SetData(Size, RegDh, (Mul >> 32) & 0xFFFFFFFF);
						}

						break;
					}

				case Opcode.ADDQ: {
						ushort EAddr = Decode_EAddr(Word0.GetBits(5, 0));
						OpSize Size = Decode_Size(Word0.GetBits(7, 6));
						ulong Data = Word0.GetBits(11, 9);
						if (Data == 0)
							Data = 8;

						ulong Dest = GetData(Size, EAddr);
						SetData(Size, EAddr, Dest + Data);

						break;
					}

				case Opcode.ADD_ADDA: {
						ushort EAddr = Decode_EAddr(Word0.GetBits(5, 0));
						ushort OpMode = Word0.GetBits(7, 6);
						OpSize Size = Decode_Size(OpMode);
						ulong Register = Word0.GetBits(11, 9);
						bool IsSource = !Word0.GetBit(8);

						// ADDA part
						ulong MODE_REGISTER = MODE_REGISTER_D;
						if (OpMode == 0x3 || OpMode == 0x7) {
							MODE_REGISTER = MODE_REGISTER_A;
							IsSource = true;

							if (OpMode == 0x3)
								Size = OpSize.WORD;
							else
								Size = OpSize.LONG;
						}

						ulong Src, Dst;
						ulong SetEAddr = MODE_REGISTER | Register;


						if (IsSource) {
							Src = GetData(Size, EAddr);
							Dst = GetData(Size, MODE_REGISTER | Register);
						} else {
							Src = GetData(Size, MODE_REGISTER_D | Register);
							Dst = GetData(Size, EAddr);
							SetEAddr = EAddr;
						}

						SetData(Size, SetEAddr, Src + Dst);
						break;
					}

				default:
					throw new NotImplementedException();
			}

			if (IncPC)
				PC += InstrLen;
		}
	}
}