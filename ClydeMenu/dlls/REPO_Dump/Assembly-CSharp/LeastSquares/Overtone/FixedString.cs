using System;
using System.Runtime.InteropServices;

namespace LeastSquares.Overtone
{
	// Token: 0x020002D5 RID: 725
	public class FixedString : IDisposable
	{
		// Token: 0x17000009 RID: 9
		// (get) Token: 0x06001696 RID: 5782 RVA: 0x000C6AFF File Offset: 0x000C4CFF
		// (set) Token: 0x06001697 RID: 5783 RVA: 0x000C6B07 File Offset: 0x000C4D07
		public IntPtr Address { get; private set; }

		// Token: 0x06001698 RID: 5784 RVA: 0x000C6B10 File Offset: 0x000C4D10
		public FixedString(string text)
		{
			this.Address = Marshal.StringToHGlobalAnsi(text);
		}

		// Token: 0x06001699 RID: 5785 RVA: 0x000C6B24 File Offset: 0x000C4D24
		public void Dispose()
		{
			if (this.Address == IntPtr.Zero)
			{
				return;
			}
			Marshal.FreeHGlobal(this.Address);
			this.Address = IntPtr.Zero;
		}
	}
}
