using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M68K {
	public static class Extensions {
		public static bool GetBit(this int Num, int Bit) {
			return (Num & (1 << Bit)) != 0;
		}

		public static int SetBit(this int Num, int Bit, bool Val) {
			if (Val)
				return Num | (1 << Bit);
			return Num & ~(1 << Bit);
		}

		public static int FlipBit(this int Num, int Bit) {
			return Num ^ (1 << Bit);
		}
	}
}
