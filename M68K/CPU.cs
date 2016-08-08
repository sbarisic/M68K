using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using OpcodeDef = M68K.OpcodeDefinition32;

namespace M68K {
	public partial class CPU {
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
					OpcodeDef.Create(AnyPref + "101000111100", "00000000????????", Opcode.EORItoCCR, 4),
					OpcodeDef.Create(AnyPref + "101001111100", Opcode.EORItoSR, 4),
					OpcodeDef.Create(AnyPref + "1010????????", Opcode.EORI, 6),
					OpcodeDef.Create(AnyPref + "1100????????", Opcode.CMPI, 6),
					OpcodeDef.Create(AnyPref + "100000??????", "00000000????????", Opcode.BTST, 4),
					OpcodeDef.Create(AnyPref + "100001??????", "00000000????????", Opcode.BCHG, 4),
					OpcodeDef.Create(AnyPref + "100010??????", "00000000????????", Opcode.BCLR, 4),
					OpcodeDef.Create(AnyPref + "100011??????", "00000000????????", Opcode.BSET, 4),
					OpcodeDef.Create(AnyPref + "1110????????", "?????00000000000", Opcode.MOVES, 4),
					OpcodeDef.Create(AnyPref + "1??011111100", "????000???000???", Opcode.CAS2, 6),
					OpcodeDef.Create(AnyPref + "1??011??????", "0000000???000???", Opcode.CAS, 4),
					OpcodeDef.Create(AnyPref + "???100??????", Opcode.BTST),
					OpcodeDef.Create(AnyPref + "???101??????", Opcode.BCHG),
					OpcodeDef.Create(AnyPref + "???110??????", Opcode.BCLR),
					OpcodeDef.Create(AnyPref + "???111??????", Opcode.BSET),
					OpcodeDef.Create(AnyPref + "??????001???", Opcode.MOVEP, 4),
				} },
				// Move byte
				{ Ext.FromBinary<byte>(Prefix + "0001"), new OpcodeDef[] {
					OpcodeDef.Create(AnyPref + "????????????", Opcode.MOVE_BYTE),
					OpcodeDef.Create(AnyPref + "???001??????", Opcode.MOVEA_BYTE),
				} },
				// Move long
				{ Ext.FromBinary<byte>(Prefix + "0010"), new OpcodeDef[] {
					OpcodeDef.Create(AnyPref + "????????????", Opcode.MOVE_LONG),
					OpcodeDef.Create(AnyPref + "???001??????", Opcode.MOVEA_LONG),
				} },
				// Move word
				{ Ext.FromBinary<byte>(Prefix + "0011"), new OpcodeDef[] {
					OpcodeDef.Create(AnyPref + "????????????", Opcode.MOVE_WORD),
					OpcodeDef.Create(AnyPref + "???001??????", Opcode.MOVEA_WORD),
				} },
				// Misc
				{ Ext.FromBinary<byte>(Prefix + "0100"), new OpcodeDef[] {
					OpcodeDef.Create(AnyPref + "000011??????", Opcode.MOVEfromSR),
					OpcodeDef.Create(AnyPref + "001011??????", Opcode.MOVEfromCCR),
					OpcodeDef.Create(AnyPref + "0000????????", Opcode.NEGX),
					OpcodeDef.Create(AnyPref + "0010????????", Opcode.CLR),
					OpcodeDef.Create(AnyPref + "010011??????", Opcode.MOVEtoCCR),
					OpcodeDef.Create(AnyPref + "0100????????", Opcode.NEG),
					OpcodeDef.Create(AnyPref + "0110????????", Opcode.NOT),
					OpcodeDef.Create(AnyPref + "011011??????", Opcode.MOVEtoSR),
					OpcodeDef.Create(AnyPref + "100???000???", Opcode.EXT_EXTB),
					OpcodeDef.Create(AnyPref + "100000001???", Opcode.LINK_LONG, 6),
					OpcodeDef.Create(AnyPref + "100000??????", Opcode.NBCD),
					OpcodeDef.Create(AnyPref + "100001000???", Opcode.SWAP),
					OpcodeDef.Create(AnyPref + "100001001???", Opcode.BKPT),
					OpcodeDef.Create(AnyPref + "100001??????", Opcode.PEA),
					OpcodeDef.Create(AnyPref + "101011111010", Opcode.BGND),
					OpcodeDef.Create(AnyPref + "101011111100", Opcode.ILLEGAL),
					OpcodeDef.Create(AnyPref + "101011??????", Opcode.TAS),
					OpcodeDef.Create(AnyPref + "1010????????", Opcode.TST),
					OpcodeDef.Create(AnyPref + "110000??????", "0???0?0000000???", Opcode.MULU_LONG, 4),
					OpcodeDef.Create(AnyPref + "110000??????", "0???1?0000000???", Opcode.MULS_LONG, 4),
					OpcodeDef.Create(AnyPref + "110000??????", "0???0?0000000???", Opcode.DIVU_DIVUL_LONG, 4),
					OpcodeDef.Create(AnyPref + "110001??????", "0???1?0000000???", Opcode.DIVS_DIVSL_LONG, 4),
					OpcodeDef.Create(AnyPref + "11100100????", Opcode.TRAP),
					OpcodeDef.Create(AnyPref + "111001010???", Opcode.LINK_WORD, 4),
					OpcodeDef.Create(AnyPref + "111001011???", Opcode.UNLK),
					OpcodeDef.Create(AnyPref + "11100110????", Opcode.MOVE_USP),
					OpcodeDef.Create(AnyPref + "111001110000", Opcode.RESET),
					OpcodeDef.Create(AnyPref + "111001110001", Opcode.NOP),
					OpcodeDef.Create(AnyPref + "111001110010", Opcode.STOP, 4),
					OpcodeDef.Create(AnyPref + "111001110011", Opcode.RTE),
					OpcodeDef.Create(AnyPref + "111001110100", Opcode.RTD, 4),
					OpcodeDef.Create(AnyPref + "111001110101", Opcode.RTS),
					OpcodeDef.Create(AnyPref + "111001110110", Opcode.TRAPV),
					OpcodeDef.Create(AnyPref + "111001110111", Opcode.RTR),
					OpcodeDef.Create(AnyPref + "11100111101?", Opcode.MOVEC, 4),
					OpcodeDef.Create(AnyPref + "111010??????", Opcode.JSR),
					OpcodeDef.Create(AnyPref + "111011??????", Opcode.JMP),
					OpcodeDef.Create(AnyPref + "1?001???????", Opcode.MOVEM, 4),
					OpcodeDef.Create(AnyPref + "???111??????", Opcode.LEA),
					OpcodeDef.Create(AnyPref + "?????0??????", Opcode.CHK),
				} },
				// ADDQ/SUBQ/Scc/DBcc/TRAPcc 
				{ Ext.FromBinary<byte>(Prefix + "0101"), new OpcodeDef[] {
					OpcodeDef.Create(AnyPref + "???0????????", Opcode.ADDQ),
					OpcodeDef.Create(AnyPref + "???1????????", Opcode.SUBQ),
					OpcodeDef.Create(AnyPref + "????11001???", Opcode.DBcc, 4),
					OpcodeDef.Create(AnyPref + "????11111???", Opcode.TRAPcc),
					OpcodeDef.Create(AnyPref + "????11??????", Opcode.Scc),
				} },
				// Bcc/BSR/BRA
				{ Ext.FromBinary<byte>(Prefix + "0110"), new OpcodeDef[] {
					OpcodeDef.Create(AnyPref + "0000????????", Opcode.BRA),
					OpcodeDef.Create(AnyPref + "0001????????", Opcode.BSR),
					OpcodeDef.Create(AnyPref + "????????????", Opcode.Bcc),
				} },
				// MOVEQ
				{ Ext.FromBinary<byte>(Prefix + "0111"), new OpcodeDef[] {
					OpcodeDef.Create(AnyPref + "???0????????", Opcode.MOVEQ),
				} },
				// OR/DIV/SBCD
				{ Ext.FromBinary<byte>(Prefix + "1000"), new OpcodeDef[] {
					OpcodeDef.Create(AnyPref + "???011??????", Opcode.DIVU_DIVUL_WORD),
					OpcodeDef.Create(AnyPref + "???10000????", Opcode.SBCD),
					OpcodeDef.Create(AnyPref + "???10100????", Opcode.PACK, 4),
					OpcodeDef.Create(AnyPref + "???11000????", Opcode.UNPK, 4),
					OpcodeDef.Create(AnyPref + "???111??????", Opcode.DIVS_DIVSL_WORD),
					OpcodeDef.Create(AnyPref + "????????????", Opcode.OR),
				} },
				// SUB/SUBX
				{ Ext.FromBinary<byte>(Prefix + "1001"), new OpcodeDef[] {
					OpcodeDef.Create(AnyPref + "???1??00????", Opcode.SUBX),
					OpcodeDef.Create(AnyPref + "????????????", Opcode.SUB_SUBA),
				} },
				// Reserved
				{ Ext.FromBinary<byte>(Prefix + "1010"), new OpcodeDef[] {
					OpcodeDef.Create(Opcode.UNIMPLEMENTED),
				} },
				// CMP/EOR
				{ Ext.FromBinary<byte>(Prefix + "1011"), new OpcodeDef[] {
					OpcodeDef.Create(AnyPref + "???1??001???", Opcode.CMPM),
					OpcodeDef.Create(AnyPref + "????????????", Opcode.CMP_CMPA_EOR),
				} },
				// AND/MUL/ABCD/EXG
				{ Ext.FromBinary<byte>(Prefix + "1100"), new OpcodeDef[] {
					OpcodeDef.Create(AnyPref + "???011??????", Opcode.MULU_WORD),
					OpcodeDef.Create(AnyPref + "???10000????", Opcode.ABCD),
					OpcodeDef.Create(AnyPref + "???111??????", Opcode.MULS_WORD),
					OpcodeDef.Create(AnyPref + "???1????????", Opcode.EXG),
					OpcodeDef.Create(AnyPref + "????????????", Opcode.AND),
				} },
				// ADD/ADDX
				{ Ext.FromBinary<byte>(Prefix + "1101"), new OpcodeDef[] {
					OpcodeDef.Create(AnyPref + "???1??00????", Opcode.ADDX), // bug; sixth bit not known
					OpcodeDef.Create(AnyPref + "????????????", Opcode.ADD_ADDA),
				} },
				// Shift/Rotate/Bit Field
				{ Ext.FromBinary<byte>(Prefix + "1110"), new OpcodeDef[] {
					OpcodeDef.Create(AnyPref + "000?11??????", Opcode.ASL_ASR_MEM_SHIFT),
					OpcodeDef.Create(AnyPref + "001?11??????", Opcode.LSL_LSR_MEM_SHIFT),
					OpcodeDef.Create(AnyPref + "010?11??????", Opcode.ROXL_ROXR_MEM_ROTATE),
					OpcodeDef.Create(AnyPref + "011?11??????", Opcode.ROL_ROR_MEM_ROTATE),
					OpcodeDef.Create(AnyPref + "100011??????", "0000????????????", Opcode.BFTST, 4),
					OpcodeDef.Create(AnyPref + "100111??????", "0???????????????", Opcode.BFEXTU, 4),
					OpcodeDef.Create(AnyPref + "101011??????", "0000????????????", Opcode.BFCHG, 4),
					OpcodeDef.Create(AnyPref + "101111??????", "0???????????????", Opcode.BFEXTS, 4),
					OpcodeDef.Create(AnyPref + "110011??????", "0000????????????", Opcode.BFCLR, 4),
					OpcodeDef.Create(AnyPref + "110111??????", "0???????????????", Opcode.BFFFO, 4),
					OpcodeDef.Create(AnyPref + "111011??????", "0000????????????", Opcode.BFSET, 4),
					OpcodeDef.Create(AnyPref + "111111??????", "0???????????????", Opcode.BFINS, 4),
					OpcodeDef.Create(AnyPref + "???????00???", Opcode.ASL_ASR_REG_SHIFT),
					OpcodeDef.Create(AnyPref + "???????01???", Opcode.LSL_LSR_REG_SHIFT),
					OpcodeDef.Create(AnyPref + "???????10???", Opcode.ROXL_ROXR_REG_ROTATE),
					OpcodeDef.Create(AnyPref + "???????11???", Opcode.ROL_ROR_REG_ROTATE),
				} },
				// Coprocessor Interface/MC68040 and CPU32 Extensions
				{ Ext.FromBinary<byte>(Prefix + "1111"), new OpcodeDef[] {
					OpcodeDef.Create(Opcode.UNIMPLEMENTED),
				} },
			};
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static byte GetLookupMaskForInstruction(uint Instruction) {
			return (byte)(((ushort)((Instruction >> 16) & 0xFFFF)).GetBits(15, 12) & 0xFF);
		}

		static Opcode MatchOpcode(uint Instruction, out int Sizeof) {
			Sizeof = 2;

			OpcodeDef[] Definitions = InstructionSet[GetLookupMaskForInstruction(Instruction)];
			for (int i = 0; i < Definitions.Length; i++)
				if (Definitions[i].Matches(Instruction)) {
					Sizeof = Definitions[i].Higher.Sizeof;
					return Definitions[i].Opcode;
				}

			return Opcode.ILLEGAL_OPCODE;
		}

		static OpcodeDef MatchOpcodeDef(uint Instruction) {
			OpcodeDef[] Definitions = InstructionSet[GetLookupMaskForInstruction(Instruction)];

			for (int i = 0; i < Definitions.Length; i++)
				if (Definitions[i].Matches(Instruction))
					return Definitions[i];

			return null;
		}

		public MemoryMappedDevice Memory;

		public int[] DA;
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
			DA = new int[16];
		}

		public virtual void Step() {
			ushort Word0 = Memory.Read16(PC + sizeof(ushort) * 0);
			ushort Word1 = Memory.Read16(PC + sizeof(ushort) * 1);
			ushort Word2 = Memory.Read16(PC + sizeof(ushort) * 2);
			ushort Word3 = Memory.Read16(PC + sizeof(ushort) * 3);
			ushort Word4 = Memory.Read16(PC + sizeof(ushort) * 4);

			uint Instr = (uint)((Word0 << 16) | Word1);
			int InstrLength;
			Opcode Opcode = MatchOpcode(Instr, out InstrLength);

#if DEBUG
			OpcodeDef OpcodeDef = MatchOpcodeDef(Instr);
			if (OpcodeDef != null) {
				string Bin = Ext.ToBinary(Instr);
				string Pattern = OpcodeDef.Higher.Pattern + OpcodeDef.Lower.Pattern;

				Console.Write("0x{0:X8}\t", Instr);

				for (int i = 0; i < Pattern.Length; i++) {
					if (Pattern[i] == '?')
						Console.ForegroundColor = ConsoleColor.Green;
					else
						Console.ForegroundColor = ConsoleColor.Gray;

					Console.Write(Bin[i]);
				}

				Console.ResetColor();
				Console.WriteLine("\t{0}", Opcode);
			} else
				Console.WriteLine("0x{0:X8}\t{1}\t{2}", Instr, Ext.ToBinary(Instr), Opcode);
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
					PC += 2;
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

			PC += InstrLength;
		}
	}
}