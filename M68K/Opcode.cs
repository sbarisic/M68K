using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace M68K {
	public enum Opcode {
		ILLEGAL_OPCODE,
		UNIMPLEMENTED,

		ABCD, BTST, LEA, ROL, ADD, CALLM,
		LINK_WORD, ROR, ADDA, CAS, LSL, ROXL,
		ADDI, CAS2, LSR, ROXR, ADDQ, CHK,
		MOVE_LONG, RTD, ADDX, CHK2, MOVEA, RTM,
		AND, CLR, MOVEfrmCCR, RTR, ANDI, CMP,
		MOVEtoCCR, RTS, ANDItoCCR, CMPA, MOVEfromSR, SBCD,
		ASL, CMPI, MOVE16, Scc, ASR, CMPM,
		MOVEM, SUB, Bcc, CMP2, MOVEP, SUBA,
		BCHG, DBcc_DBRA, MOVEQ, SUBI, BCLR, DIVS,
		MULS, SUBQ, BFCHG, DIVSL, MULU, SUBX,
		BFCLR, DIVU, NBCD, SWAP, BFEXTS, DIVUL,
		NEG, TAS, BFEXTU, EOR, NEGX, TDIVS,
		BFFFO, EORI, NOP, TDIVU, BFINS, EORItoCCR,
		NOT, TRAP, BFSET, EXG, OR, TRAPcc_Tcc,
		BFTST, EXT, ORI, TRAPV, BKPT, EXTB,
		ORItoCCR, TST, BRA, ILLEGAL, PACK, UNLK,
		BSET, JMP, PEA, UNPK, BSR, JSR,
		ORItoSR,
		ANDItoSR,
		LINK_LONG,
		MOVE_BYTE,
		MOVE_WORD
	}

	public class OpcodeDefinition16 {
		public static OpcodeDefinition16 CreateAny(Opcode Opcode, int Sizeof = 2) {
			return new OpcodeDefinition16("????????????????", Opcode, Sizeof);
		}

		public Opcode Opcode;
		public byte LowerMask, HigherMask, LowerPattern, HigherPattern;
		public int Sizeof;

		public OpcodeDefinition16(string Pattern, Opcode Opcode, int Sizeof) {
			string LowerPatternString = Pattern.Substring(8);
			string HigherPatternString = Pattern.Substring(0, 8);

			Ext.GenerateMaskPattern(LowerPatternString, out LowerMask, out LowerPattern);
			Ext.GenerateMaskPattern(HigherPatternString, out HigherMask, out HigherPattern);
			this.Opcode = Opcode;
			this.Sizeof = Sizeof;
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
