using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M68K {
	public class RAM : MemoryMappedDevice {
		byte[] Data;

		public override int Size
		{
			get
			{
				return Data.Length;
			}
		}

		public RAM(int Size) {
			Data = new byte[Size];
		}

		public override byte Read8(int Address) {
			return Data[Address];
		}

		public override void Write8(int Address, byte Value) {
			Data[Address] = Value;
		}
	}
}