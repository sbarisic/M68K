using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace M68K {
	public class ROM : MemoryMappedDevice {
		public static ROM FromFile(string Path) {
			ROM ROM = new ROM();
			ROM.Memory = File.ReadAllBytes(Path);
			return ROM;
		}

		byte[] Memory;

		public override int Size
		{
			get
			{
				return Memory.Length;
			}
		}

		public ROM() {
		}

		public override byte Read8(int Address) {
			return Memory[Address];
		}
	}
}