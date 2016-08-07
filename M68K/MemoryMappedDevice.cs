using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M68K {
	public abstract class MemoryMappedDevice {
		public abstract int Size { get; }

		public virtual byte Read8(int Address) {
			throw new InvalidOperationException();
		}

		public virtual void Write8(int Address, byte Value) {
			throw new InvalidOperationException();
		}

		public virtual ushort Read16(int Address) {
			ushort A = Read8(Address);
			ushort B = Read8(Address + 1);
			return (ushort)((A << 8) | B);
		}

		public virtual void Write16(int Address, ushort Value) {
			Write8(Address, (byte)((Value >> 8) & 0xFF));
			Write8(Address + 1, (byte)(Value & 0xFF));
		}
	}
}
