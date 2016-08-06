using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M68K {
	public class MemoryBus : MemoryMappedDevice {
		struct BusSlot {
			public int Start;
			public int End;
			public MemoryMappedDevice MemoryDevice;

			public BusSlot(int Start, int End, MemoryMappedDevice Mem) {
				this.Start = Start;
				this.End = End;
				this.MemoryDevice = Mem;
			}

			public bool InRange(int Address) {
				return Address >= Start && Address < End;
			}
		}

		List<BusSlot> MemoryDevices;
		MemoryMappedDevice DefaultDevice;

		public MemoryBus() {
			MemoryDevices = new List<BusSlot>();
		}

		public override int Size
		{
			get
			{
				return -1;
			}
		}

		public virtual void Attach(int Start, int End, MemoryMappedDevice Mem) {
			MemoryDevices.Add(new BusSlot(Start, End, Mem));
		}

		public virtual void Attach(int Start, MemoryMappedDevice Mem) {
			Attach(Start, Start + Mem.Size, Mem);
		}

		public virtual void SetDefaultDevice(MemoryMappedDevice Device) {
			DefaultDevice = Device;
		}

		public virtual MemoryMappedDevice FindDevice(int MemoryLocation) {
			foreach (var Dev in MemoryDevices)
				if (Dev.InRange(MemoryLocation))
					return Dev.MemoryDevice;
			return DefaultDevice;
		}
	}
}
