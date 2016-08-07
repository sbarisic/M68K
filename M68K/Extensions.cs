using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace M68K {
	public static partial class Ext {
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool GetBit(this ulong Num, int Bit) {
			return (Num & (0x1u << Bit)) != 0x0u;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool GetBit(this int Num, int Bit) {
			return ((ulong)Num).GetBit(Bit);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool GetBit(this ushort Num, int Bit) {
			return ((ulong)Num).GetBit(Bit);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ulong SetBit(this ulong Num, int Bit, bool Val) {
			if (Val)
				return Num | (0x1u << Bit);
			return Num & ~(0x1u << Bit);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int SetBit(this int Num, int Bit, bool Val) {
			return (int)((ulong)Num).SetBit(Bit, Val);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static byte SetBit(this byte Num, int Bit, bool Val) {
			return (byte)((ulong)Num).SetBit(Bit, Val);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ulong FlipBit(this ulong Num, int Bit) {
			return Num ^ (0x1u << Bit);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static byte LowerByte(this ushort Val) {
			return (byte)(Val & 0xFF);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static byte HigherByte(this ushort Val) {
			return (byte)((Val >> 8) & 0xFF);
		}

		public static string ToBinary<T>(T Num) where T : struct {
			int Bits = Marshal.SizeOf<T>() * 8;
			string StringBits = "";

			for (int i = Bits - 1; i >= 0; i--)
				StringBits += GetBit((ulong)Convert.ChangeType(Num, typeof(ulong)), i) ? 1 : 0;

			return StringBits;
		}

		public static T FromBinary<T>(string Binary) where T : struct {
			ulong Num = 0;

			for (int i = 0; i < Binary.Length; i++) 
				Num = Num.SetBit(i, Binary.Substring(Binary.Length - i - 1, 1) == "1");

			return (T)Convert.ChangeType(Num, typeof(T));
		}
	}
}
