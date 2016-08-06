using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M68K {
	public class CPU {
		public MemoryMappedDevice Memory;

		public int[] D;
		public int[] A;
		public int PC;
		public int CCR;

		public bool CCR_C_Carry { get { return CCR.GetBit(0); } set { CCR = CCR.SetBit(0, value); } }
		public bool CCR_V_Overflow { get { return CCR.GetBit(1); } set { CCR = CCR.SetBit(1, value); } }
		public bool CCR_Z_Zero { get { return CCR.GetBit(2); } set { CCR = CCR.SetBit(2, value); } }
		public bool CCR_N_Sign { get { return CCR.GetBit(3); } set { CCR = CCR.SetBit(3, value); } }
		public bool CCR_X_Extend { get { return CCR.GetBit(4); } set { CCR = CCR.SetBit(4, value); } }
		public bool CCR_5 { get { return CCR.GetBit(5); } set { CCR = CCR.SetBit(5, value); } }
		public bool CCR_6 { get { return CCR.GetBit(6); } set { CCR = CCR.SetBit(6, value); } }
		public bool CCR_7 { get { return CCR.GetBit(7); } set { CCR = CCR.SetBit(7, value); } }
		public bool CCR_I0 { get { return CCR.GetBit(8); } set { CCR = CCR.SetBit(8, value); } }
		public bool CCR_I1 { get { return CCR.GetBit(9); } set { CCR = CCR.SetBit(9, value); } }
		public bool CCR_I2 { get { return CCR.GetBit(10); } set { CCR = CCR.SetBit(10, value); } }
		public bool CCR_11 { get { return CCR.GetBit(11); } set { CCR = CCR.SetBit(11, value); } }
		public bool CCR_M { get { return CCR.GetBit(12); } set { CCR = CCR.SetBit(12, value); } }
		public bool CCR_S { get { return CCR.GetBit(13); } set { CCR = CCR.SetBit(13, value); } }
		public bool CCR_T0 { get { return CCR.GetBit(14); } set { CCR = CCR.SetBit(14, value); } }
		public bool CCR_T1 { get { return CCR.GetBit(15); } set { CCR = CCR.SetBit(15, value); } }

		public CPU() {
			D = new int[8];
			A = new int[8];
		}

		public virtual void Interrupt(int Num) {
		}

		public virtual void Step() {
		}
	}
}