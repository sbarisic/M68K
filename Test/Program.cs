using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using M68K;

namespace Test {
	class Program {
		static CPU Processor;


		static void Main(string[] args) {
			Console.Title = string.Format("M68K, {0} opcodes", Enum.GetNames(typeof(Opcode)).Length);
			Processor = new CPU();

			MemoryBus Bus = new MemoryBus();
			Bus.SetDefaultDevice(new RAM(64000));
			Bus.Attach(0, ROM.FromFile("program.bin"));
			Processor.Memory = Bus;
			
			while (true) {
				Console.ForegroundColor = ConsoleColor.White;
				Console.Write("(M68K)>> ");
				Console.ResetColor();
				string In = Console.ReadLine();
				string RawIn = In;

				if (!string.IsNullOrWhiteSpace(In)) {
					while (In.Contains("  "))
						In = In.Replace("  ", " ");
					Exec(RawIn, In.ToLower().Split(' '));
				}
			}
		}

		static void Exec(string Cmd, string[] CmdSplit) {
			if (CmdSplit.Length == 1 && CmdSplit[0] == "q")
				Environment.Exit(0);
			else if (CmdSplit.Length == 2 && CmdSplit[0] == "step") {
				int Cnt = int.Parse(CmdSplit[1]);
				for (int i = 0; i < Cnt; i++)
					Processor.Step();
			} else if (CmdSplit.Length == 2 && CmdSplit[0] == "regs") {
				const int ValSpacing = 0;
				const string NumberFormat = "0x{0:X8} ";

				if (CmdSplit[1] == "all" || CmdSplit[1].Contains("d")) {
					Console.Write(" ");
					for (int i = 0; i < 8; i++) {
						Console.ForegroundColor = ConsoleColor.Green;
						Console.Write("D{0}", i);
						Console.ResetColor();
						Console.Write(" {1}", i, FmtToLen(NumberFormat, ValSpacing, Processor.DA[i]));
					}
					Console.WriteLine();
				}

				if (CmdSplit[1] == "all" || CmdSplit[1].Contains("a")) {
					Console.Write(" ");
					for (int i = 0; i < 8; i++) {
						Console.ForegroundColor = ConsoleColor.Green;
						Console.Write("A{0}", i);
						Console.ResetColor();
						Console.Write(" {1}", i, FmtToLen(NumberFormat, ValSpacing, Processor.DA[i + 8]));
					}
					Console.WriteLine();
				}

				if (CmdSplit[1] == "all" || CmdSplit[1].Contains("ccr")) {
					Console.ForegroundColor = ConsoleColor.Green;
					Console.Write("CCR ");
					Console.ResetColor();
					Console.WriteLine("0x{0:X8} 0b{1}", Processor.CCR, Ext.ToBinary(Processor.CCR));
				}

				if (CmdSplit[1] == "all" || CmdSplit[1].Contains("pc")) {
					Console.ForegroundColor = ConsoleColor.Green;
					Console.Write(" PC ");
					Console.ResetColor();
					Console.WriteLine("0x{0:X8}", Processor.PC);
				}
			} else
				Console.WriteLine("Unknown command '{0}'", Cmd);
			Console.WriteLine();
		}

		static string FmtToLen(string Fmt, int Len, params object[] Args) {
			string Str = string.Format(Fmt, Args);
			if (Str.Length < Len)
				Str += new string(' ', Len - Str.Length);
			return Str;
		}
	}
}
