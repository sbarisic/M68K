using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace M68K {
	public static partial class Ext {
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool BinaryMatch(this byte Value, byte Mask, byte Pattern) {
			return (byte)(Value & Mask) == Pattern;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool BinaryMatch(this ushort Value, ushort Mask, ushort Pattern) {
			return (ushort)(Value & Mask) == Pattern;
		}
		
		public static void GenerateMaskPattern(string PatternString, out byte Mask, out byte Pattern) {
			Mask = 0xFF;
			Pattern = 0;

			for (int i = 0; i < PatternString.Length; i++) {
				if (PatternString[PatternString.Length - i - 1] == '?')
					Mask = Mask.SetBit(i, false);
				Pattern = Pattern.SetBit(i, PatternString[PatternString.Length - i - 1] == '1');
			}
		}
	}
}
