using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M68K {
	public class MemoryBus : MemoryMappedDevice {
		public struct BusSlot {
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

			public int OffsetAddress(int Address) {
				return Address - Start;
			}
		}

		List<BusSlot> MemoryDevices;
		BusSlot DefaultDevice;

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
			DefaultDevice = new BusSlot(0, Device.Size, Device);
		}

		public virtual BusSlot FindDevice(int MemoryLocation) {
			foreach (var Dev in MemoryDevices)
				if (Dev.InRange(MemoryLocation))
					return Dev;
			return DefaultDevice;
		}

		public override byte Read8(int Address) {
			BusSlot Dev = FindDevice(Address);
			return Dev.MemoryDevice.Read8(Dev.OffsetAddress(Address));
		}

		public override void Write8(int Address, byte Value) {
			BusSlot Dev = FindDevice(Address);
			Dev.MemoryDevice.Write8(Dev.OffsetAddress(Address), Value);
		}
	}
}
