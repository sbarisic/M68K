using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using OpcodeDef = M68K.OpcodeDefinition32;

namespace M68K {
	public class CPU {
		static Dictionary<byte, OpcodeDef[]> InstructionSet;

		static CPU() {
			const string Prefix = "0000";
			const string AnyPref = "????";

			InstructionSet = new Dictionary<byte, OpcodeDef[]>() {
				// Bit manipulation/MOVEP/Immediate
				{ Ext.FromBinary<byte>(Prefix + "0000"), new OpcodeDef[] {
					OpcodeDef.Create(AnyPref + "000000111100", "00000000????????", Opcode.ORItoCCR, 4),
					OpcodeDef.Create(AnyPref + "000001111100", Opcode.ORItoSR, 4),
					OpcodeDef.Create(AnyPref + "0000????????", Opcode.ORI, 6),
					OpcodeDef.Create(AnyPref + "001000111100", "00000000????????", Opcode.ANDItoCCR, 4),
					OpcodeDef.Create(AnyPref + "001001111100", Opcode.ANDItoSR, 4),
					OpcodeDef.Create(AnyPref + "0000????????", Opcode.ANDI, 6),
					OpcodeDef.Create(AnyPref + "0100????????", Opcode.SUBI, 6),
					OpcodeDef.Create(AnyPref + "01101100????", Opcode.RTM),
					OpcodeDef.Create(AnyPref + "011011??????", "00000000????????", Opcode.CALLM, 4),
					OpcodeDef.Create(AnyPref + "0110????????", Opcode.ADDI, 6),
					OpcodeDef.Create(AnyPref + "0??011??????", "????000000000000", Opcode.CMP2, 4),
					OpcodeDef.Create(AnyPref + "0??011??????", "????100000000000", Opcode.CHK2, 4),

					//OpcodeDefinition.Create12bit("11100100????", Opcode.ILLEGAL),
				} },
				// Move byte
				{ Ext.FromBinary<byte>(Prefix + "0001"), new OpcodeDef[] {
					OpcodeDef.Create(AnyPref + "????????????", Opcode.MOVE_BYTE),
				} },
				// Move long
				{ Ext.FromBinary<byte>(Prefix + "0010"), new OpcodeDef[] {
					OpcodeDef.Create(AnyPref + "????????????", Opcode.MOVE_LONG),
				} },
				// Move word
				{ Ext.FromBinary<byte>(Prefix + "0011"), new OpcodeDef[] {
					OpcodeDef.Create(AnyPref + "????????????", Opcode.MOVE_WORD),
				} },
				// Misc
				{ Ext.FromBinary<byte>(Prefix + "0100"), new OpcodeDef[] {
					OpcodeDef.Create(AnyPref + "11100100????", Opcode.TRAP),
					OpcodeDef.Create(AnyPref + "100000001???", Opcode.LINK_LONG, 6),
					OpcodeDef.Create(AnyPref + "111001010???", Opcode.LINK_WORD, 4),
					OpcodeDef.Create(AnyPref + "111011??????", Opcode.JMP),
					OpcodeDef.Create(AnyPref + "111010??????", Opcode.JSR),
					OpcodeDef.Create(AnyPref + "111001110001", Opcode.NOP),
					OpcodeDef.Create(AnyPref + "111001011???", Opcode.UNLK),
					OpcodeDef.Create(AnyPref + "111001110101", Opcode.RTS),
				} },
				// ADDQ/SUBQ/Scc/DBcc/TRAPcc 
				{ Ext.FromBinary<byte>(Prefix + "0101"), new OpcodeDef[] {

				} },
				// Bcc/BSR/BRA
				{ Ext.FromBinary<byte>(Prefix + "0110"), new OpcodeDef[] {

				} },
				// MOVEQ
				{ Ext.FromBinary<byte>(Prefix + "0111"), new OpcodeDef[] {

				} },
				// OR/DIV/SBCD
				{ Ext.FromBinary<byte>(Prefix + "1000"), new OpcodeDef[] {

				} },
				// SUB/SUBX
				{ Ext.FromBinary<byte>(Prefix + "1001"), new OpcodeDef[] {

				} },
				// Reserved
				{ Ext.FromBinary<byte>(Prefix + "1010"), new OpcodeDef[] {

				} },
				// CMP/EOR
				{ Ext.FromBinary<byte>(Prefix + "1011"), new OpcodeDef[] {

				} },
				// AND/MUL/ABCD/EXG
				{ Ext.FromBinary<byte>(Prefix + "1100"), new OpcodeDef[] {

				} },
				// ADD/ADDX
				{ Ext.FromBinary<byte>(Prefix + "1101"), new OpcodeDef[] {

				} },
				// Shift/Rotate/Bit Field
				{ Ext.FromBinary<byte>(Prefix + "1110"), new OpcodeDef[] {

				} },
				// Coprocessor Interface/MC68040 and CPU32 Extensions
				{ Ext.FromBinary<byte>(Prefix + "1111"), new OpcodeDef[] {

				} },
			};
		}

		static Opcode MatchOpcode(uint Instruction, out int Sizeof) {
			Sizeof = 2;

			byte LookupMask = (byte)(((ushort)((Instruction >> 16) & 0xFFFF)).GetBits(15, 12) & 0xFF);
			OpcodeDef[] Definitions = InstructionSet[LookupMask];
			for (int i = 0; i < Definitions.Length; i++)
				if (Definitions[i].Matches(Instruction)) {
					Sizeof = Definitions[i].Higher.Sizeof;
					return Definitions[i].Opcode;
				}

			return Opcode.ILLEGAL_OPCODE;
		}

		public MemoryMappedDevice Memory;

		public int[] D;
		public int[] A;
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

		public CPU() {
			D = new int[8];
			A = new int[8];
		}

		public virtual void Trap(int Num) {
			Console.WriteLine("Trap #{0}", Num);
		}

		public virtual void Step() {
			ushort InstrHigher = Memory.Read16(PC);
			ushort InstrLower = Memory.Read16(PC + sizeof(ushort));
			uint Instr = (uint)((InstrHigher << 16) | InstrLower);
			int InstrLength;

			Opcode Opcode = MatchOpcode(Instr, out InstrLength);
			Console.WriteLine("0x{0:X8}\t{1}\t{2}", Instr, Ext.ToBinary(Instr), Opcode);

			switch (Opcode) {
				case Opcode.ABCD:
					break;
				case Opcode.BTST:
					break;
				case Opcode.LEA:
					break;
				case Opcode.ROL:
					break;
				case Opcode.ADD:
					break;
				case Opcode.CALLM:
					break;
				case Opcode.LINK_WORD:
					break;
				case Opcode.ROR:
					break;
				case Opcode.ADDA:
					break;
				case Opcode.CAS:
					break;
				case Opcode.LSL:
					break;
				case Opcode.ROXL:
					break;
				case Opcode.ADDI:
					break;
				case Opcode.CAS2:
					break;
				case Opcode.LSR:
					break;
				case Opcode.ROXR:
					break;
				case Opcode.ADDQ:
					break;
				case Opcode.CHK:
					break;
				case Opcode.MOVE_LONG:
					break;
				case Opcode.RTD:
					break;
				case Opcode.ADDX:
					break;
				case Opcode.CHK2:
					break;
				case Opcode.MOVEA:
					break;
				case Opcode.RTM:
					break;
				case Opcode.AND:
					break;
				case Opcode.CLR:
					break;
				case Opcode.MOVEfrmCCR:
					break;
				case Opcode.RTR:
					break;
				case Opcode.ANDI:
					break;
				case Opcode.CMP:
					break;
				case Opcode.MOVEtoCCR:
					break;
				case Opcode.RTS:
					break;
				case Opcode.ANDItoCCR:
					break;
				case Opcode.CMPA:
					break;
				case Opcode.MOVEfromSR:
					break;
				case Opcode.SBCD:
					break;
				case Opcode.ASL:
					break;
				case Opcode.CMPI:
					break;
				case Opcode.MOVE16:
					break;
				case Opcode.Scc:
					break;
				case Opcode.ASR:
					break;
				case Opcode.CMPM:
					break;
				case Opcode.MOVEM:
					break;
				case Opcode.SUB:
					break;
				case Opcode.Bcc:
					break;
				case Opcode.CMP2:
					break;
				case Opcode.MOVEP:
					break;
				case Opcode.SUBA:
					break;
				case Opcode.BCHG:
					break;
				case Opcode.DBcc_DBRA:
					break;
				case Opcode.MOVEQ:
					break;
				case Opcode.SUBI:
					break;
				case Opcode.BCLR:
					break;
				case Opcode.DIVS:
					break;
				case Opcode.MULS:
					break;
				case Opcode.SUBQ:
					break;
				case Opcode.BFCHG:
					break;
				case Opcode.DIVSL:
					break;
				case Opcode.MULU:
					break;
				case Opcode.SUBX:
					break;
				case Opcode.BFCLR:
					break;
				case Opcode.DIVU:
					break;
				case Opcode.NBCD:
					break;
				case Opcode.SWAP:
					break;
				case Opcode.BFEXTS:
					break;
				case Opcode.DIVUL:
					break;
				case Opcode.NEG:
					break;
				case Opcode.TAS:
					break;
				case Opcode.BFEXTU:
					break;
				case Opcode.EOR:
					break;
				case Opcode.NEGX:
					break;
				case Opcode.TDIVS:
					break;
				case Opcode.BFFFO:
					break;
				case Opcode.EORI:
					break;
				case Opcode.NOP:
					break;
				case Opcode.TDIVU:
					break;
				case Opcode.BFINS:
					break;
				case Opcode.EORItoCCR:
					break;
				case Opcode.NOT:
					break;
				case Opcode.TRAP:
					Trap((int)((ulong)InstrHigher).GetBits(3, 0));
					break;
				case Opcode.BFSET:
					break;
				case Opcode.EXG:
					break;
				case Opcode.OR:
					break;
				case Opcode.TRAPcc_Tcc:
					break;
				case Opcode.BFTST:
					break;
				case Opcode.EXT:
					break;
				case Opcode.ORI:
					break;
				case Opcode.TRAPV:
					break;
				case Opcode.BKPT:
					break;
				case Opcode.EXTB:
					break;
				case Opcode.ORItoCCR:
					break;
				case Opcode.TST:
					break;
				case Opcode.BRA:
					break;
				case Opcode.ILLEGAL:
					break;
				case Opcode.PACK:
					break;
				case Opcode.UNLK:
					break;
				case Opcode.BSET:
					break;
				case Opcode.JMP:
					break;
				case Opcode.PEA:
					break;
				case Opcode.UNPK:
					break;
				case Opcode.BSR:
					break;
				case Opcode.JSR:
					PC += 2;
					break;
				case Opcode.ORItoSR:
					break;
				case Opcode.ANDItoSR:
					break;
				case Opcode.LINK_LONG:
					break;
				case Opcode.MOVE_BYTE:
					break;
				case Opcode.MOVE_WORD:
					break;
				default:
					break;
			}

			PC += InstrLength;
		}
	}
}