using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M68K {
	public abstract class MemoryMappedDevice {
		public abstract int Size { get; }

		public virtual ulong Read(OpSize Size, int Address) {
			switch (Size) {
				case OpSize._8:
					return Read8(Address);
				case OpSize._16:
					return Read16(Address);
				case OpSize._32:
					return Read32(Address);
				default:
					throw new NotImplementedException();
			}
		}

		public virtual void Write(OpSize Size, int Address, ulong Value) {
			switch (Size) {
				case OpSize._8:
					Write8(Address, (byte)Value);
					break;
				case OpSize._16:
					Write16(Address, (ushort)Value);
					break;
				case OpSize._32:
					Write32(Address, (uint)Value);
					break;
				default:
					throw new NotImplementedException();
			}
		}

		public virtual byte Read8(int Address) {
			throw new NotImplementedException();
		}

		public virtual void Write8(int Address, byte Value) {
			throw new NotImplementedException();
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

		public virtual uint Read32(int Address) {
			uint A = Read16(Address);
			uint B = Read16(Address + 2);
			return (A << 16) | B;
		}

		public virtual void Write32(int Address, uint Value) {
			Write16(Address, (ushort)((Value >> 16) & 0xFFFF));
			Write16(Address + 2, (ushort)(Value & 0xFFFF));
		}
	}
}
