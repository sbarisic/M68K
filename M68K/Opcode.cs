using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace M68K {
	public enum Opcode {
		ILLEGAL,
		TRAP
	}

	public class OpcodeDefinition {
		public Opcode Opcode;
		public byte LowerMask, HigherMask, LowerPattern, HigherPattern;

		public OpcodeDefinition(string Pattern, Opcode Opcode) {
			string LowerPatternString = Pattern.Substring(8);
			string HigherPatternString = Pattern.Substring(0, 8);

			Ext.GenerateMaskPattern(LowerPatternString, out LowerMask, out LowerPattern);
			Ext.GenerateMaskPattern(HigherPatternString, out HigherMask, out HigherPattern);
			this.Opcode = Opcode;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Matches(ushort Instruction) {
			return Instruction.LowerByte().BinaryMatch(LowerMask, LowerPattern)
				&& Instruction.HigherByte().BinaryMatch(HigherMask, HigherPattern);
		}
	}
}
