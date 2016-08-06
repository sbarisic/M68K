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
			
			Console.WriteLine("Done!");
			Console.ReadLine();
		}

		static void PrintBits(int I) {
			for (int i = 0; i < 32; i++)
				Console.Write(I.GetBit(i) ? 1 : 0);
			Console.WriteLine();
		}
	}
}
