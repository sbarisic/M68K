using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace M68K {
	public static class Ext {
		public static bool GetBit(this ulong Num, int Bit) {
			return (Num & (0x1u << Bit)) != 0x0u;
		}

		public static bool GetBit(this int Num, int Bit) {
			return ((ulong)Num).GetBit(Bit);
		}

		public static bool GetBit(this ushort Num, int Bit) {
			return ((ulong)Num).GetBit(Bit);
		}

		public static ulong SetBit(this ulong Num, int Bit, bool Val) {
			if (Val)
				return Num | (0x1u << Bit);
			return Num & ~(0x1u << Bit);
		}

		public static int SetBit(this int Num, int Bit, bool Val) {
			return (int)((ulong)Num).SetBit(Bit, Val);
		}

		public static ulong FlipBit(this ulong Num, int Bit) {
			return Num ^ (0x1u << Bit);
		}

		public static string ToBinary<T>(T Num) where T : struct {
			int Bits = Marshal.SizeOf<T>() * 8;
			string StringBits = "";

			for (int i = Bits - 1; i >= 0; i--)
				StringBits += GetBit((ulong)Convert.ChangeType(Num, typeof(ulong)), i) ? 1 : 0;

			return StringBits;
		}
	}
}
