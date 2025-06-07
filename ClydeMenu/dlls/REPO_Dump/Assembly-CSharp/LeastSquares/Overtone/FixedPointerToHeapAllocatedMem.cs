using System;
using System.Runtime.InteropServices;

namespace LeastSquares.Overtone
{
	// Token: 0x020002D4 RID: 724
	public class FixedPointerToHeapAllocatedMem
	{
		// Token: 0x17000007 RID: 7
		// (get) Token: 0x0600168F RID: 5775 RVA: 0x000C6A8C File Offset: 0x000C4C8C
		// (set) Token: 0x06001690 RID: 5776 RVA: 0x000C6A94 File Offset: 0x000C4C94
		public IntPtr Address { get; private set; }

		// Token: 0x06001691 RID: 5777 RVA: 0x000C6A9D File Offset: 0x000C4C9D
		public void Free()
		{
			this._handle.Free();
			this.Address = IntPtr.Zero;
		}

		// Token: 0x06001692 RID: 5778 RVA: 0x000C6AB5 File Offset: 0x000C4CB5
		public static FixedPointerToHeapAllocatedMem Create<T>(T Object, uint SizeInBytes)
		{
			FixedPointerToHeapAllocatedMem fixedPointerToHeapAllocatedMem = new FixedPointerToHeapAllocatedMem();
			fixedPointerToHeapAllocatedMem._handle = GCHandle.Alloc(Object, 3);
			fixedPointerToHeapAllocatedMem.SizeInBytes = SizeInBytes;
			fixedPointerToHeapAllocatedMem.Address = fixedPointerToHeapAllocatedMem._handle.AddrOfPinnedObject();
			return fixedPointerToHeapAllocatedMem;
		}

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x06001693 RID: 5779 RVA: 0x000C6AE6 File Offset: 0x000C4CE6
		// (set) Token: 0x06001694 RID: 5780 RVA: 0x000C6AEE File Offset: 0x000C4CEE
		public uint SizeInBytes { get; private set; }

		// Token: 0x04002715 RID: 10005
		private GCHandle _handle;
	}
}
