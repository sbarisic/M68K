using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace M68K {
	public enum OpSize {
		BYTE,
		WORD,
		LONG,

		_8 = BYTE,
		_16 = WORD,
		_32 = LONG,
	}

	public partial class CPU {
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static void Decode_DestEAddr(ulong Value) {
			Decode_EAddr(Value.GetBits(2, 0), Value.GetBits(5, 3));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static void Decode_SrcEAddr(ulong Value) {
			Decode_EAddr(Value.GetBits(5, 3), Value.GetBits(2, 0));
		}


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static void Decode_EAddr(ulong Value) {
			Decode_SrcEAddr(Value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static void Decode_EAddr(ulong Mode, ulong Reg) {

		}

		public virtual void Move(OpSize Size, ulong Instruction) {

		}

		public virtual void Trap(int Num) {
			Console.WriteLine("Trap #{0}", Num);
		}
	}
}
