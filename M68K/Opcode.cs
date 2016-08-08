using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace M68K {
	public enum Opcode {
		ILLEGAL_OPCODE, UNIMPLEMENTED,
		ORItoCCR, ORItoSR, ORI, ANDItoCCR,
		ANDItoSR, ANDI, SUBI, RTM, CALLM, CMP2,
		CHK2, EORItoCCR, ADDI, EORItoSR, EORI, CMPI,
		BTST, BCHG, BCLR, BSET, MOVES, CAS2,
		CAS, MOVEP, MOVE_BYTE, MOVEA_BYTE, MOVE_LONG, MOVEA_LONG,
		MOVE_WORD, MOVEA_WORD, MOVEfromSR, MOVEfromCCR, NEGX, CLR,
		MOVEtoCCR, NEG, NOT, MOVEtoSR, EXT_EXTB, LINK_LONG,
		NBCD, SWAP, BKPT, PEA, BGND, ILLEGAL,
		TAS, TST, MULU_LONG, MULS_LONG, DIVU_DIVUL_LONG, DIVS_DIVSL_LONG,
		TRAP, LINK_WORD, UNLK, MOVE_USP, RESET, NOP,
		STOP, RTE, RTD, RTS, TRAPV, RTR,
		MOVEC, JSR, JMP, MOVEM, LEA, CHK,
		ADDQ, SUBQ, DBcc, TRAPcc, Scc, BRA,
		BSR, Bcc, MOVEQ, DIVU_DIVUL_WORD, SBCD, PACK,
		UNPK, DIVS_DIVSL_WORD, OR, SUBX, SUB_SUBA, CMPM,
		CMP_CMPA_EOR, MULU_WORD, ABCD, MULS_WORD, EXG, AND,
		ADDX, ADD_ADDA, ASL_ASR_MEM_SHIFT, LSL_LSR_MEM_SHIFT, ROXL_ROXR_MEM_ROTATE, ROL_ROR_MEM_ROTATE,
		BFTST, BFEXTU, BFCHG, BFEXTS, BFCLR, BFFFO,
		BFSET, BFINS, ASL_ASR_REG_SHIFT, LSL_LSR_REG_SHIFT, ROXL_ROXR_REG_ROTATE, ROL_ROR_REG_ROTATE,
	}

	public class OpcodeDefinition16 {
		public static OpcodeDefinition16 CreateAny(Opcode Opcode, int Sizeof = 2) {
			return new OpcodeDefinition16("????????????????", Opcode, Sizeof);
		}

		public Opcode Opcode;
		public byte LowerMask, HigherMask, LowerPattern, HigherPattern;
		public int Sizeof;
		public string Pattern;

		public OpcodeDefinition16(string Pattern, Opcode Opcode, int Sizeof) {
			string LowerPatternString = Pattern.Substring(8);
			string HigherPatternString = Pattern.Substring(0, 8);

			Ext.GenerateMaskPattern(LowerPatternString, out LowerMask, out LowerPattern);
			Ext.GenerateMaskPattern(HigherPatternString, out HigherMask, out HigherPattern);
			this.Opcode = Opcode;
			this.Sizeof = Sizeof;
			this.Pattern = Pattern;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Matches(ushort Instruction) {
			return Instruction.LowerByte().BinaryMatch(LowerMask, LowerPattern)
				&& Instruction.HigherByte().BinaryMatch(HigherMask, HigherPattern);
		}
	}

	public class OpcodeDefinition32 {
		public static OpcodeDefinition32 Create(string Higher, string Lower, Opcode Opcode, int Sizeof = 2) {
			return new OpcodeDefinition32(new OpcodeDefinition16(Higher, Opcode, Sizeof),
				new OpcodeDefinition16(Lower, Opcode, Sizeof));
		}

		public static OpcodeDefinition32 Create(string Higher, Opcode Opcode, int Sizeof = 2) {
			return Create(Higher, "????????????????", Opcode, Sizeof);
		}

		public static OpcodeDefinition32 Create(Opcode Opcode, int Sizeof = 2) {
			return Create("????????????????", Opcode, Sizeof);
		}

		public OpcodeDefinition16 Lower, Higher;
		public Opcode Opcode;

		public OpcodeDefinition32(OpcodeDefinition16 Higher, OpcodeDefinition16 Lower) {
			this.Lower = Lower;
			this.Higher = Higher;
			Opcode = Higher.Opcode;
		}

		public OpcodeDefinition32(OpcodeDefinition16 Higher) : this(Higher, OpcodeDefinition16.CreateAny(Higher.Opcode)) {
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Matches(uint Instruction) {
			return Higher.Matches((ushort)((Instruction >> 16) & 0xFFFF)) && Lower.Matches((ushort)(Instruction & 0xFFFF));
		}
	}
}
