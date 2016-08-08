using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using M68K;

namespace Test {
	class Program {
		static void Main(string[] args) {
			CPU Processor = new CPU();
			MemoryBus Bus = new MemoryBus();
			Processor.Memory = Bus;

			Bus.SetDefaultDevice(new RAM(64000));
			Bus.Attach(0, ROM.FromFile("program.bin"));

			/*while (true)
				Processor.Step();*/

			for (int i = 0; i < 32; i++) {
				Processor.Step();
			}

			Console.WriteLine("Done!");
			Console.ReadLine();
		}
	}
}
