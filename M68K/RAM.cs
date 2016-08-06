using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M68K {
	public class RAM : MemoryMappedDevice {
		byte[] Data;

		public RAM(int Size) {
			Data = new byte[Size];
		}

		public override int Size
		{
			get
			{
				return Data.Length;
			}
		}
	}
}
