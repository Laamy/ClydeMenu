using System;
using System.Threading;
using UnityEngine;

namespace LeastSquares.Overtone
{
	// Token: 0x020002D9 RID: 729
	public class TTSVoiceNative
	{
		// Token: 0x1700000C RID: 12
		// (get) Token: 0x060016AE RID: 5806 RVA: 0x000C6D67 File Offset: 0x000C4F67
		// (set) Token: 0x060016AF RID: 5807 RVA: 0x000C6D6F File Offset: 0x000C4F6F
		public IntPtr Pointer { get; set; }

		// Token: 0x1700000D RID: 13
		// (get) Token: 0x060016B0 RID: 5808 RVA: 0x000C6D78 File Offset: 0x000C4F78
		// (set) Token: 0x060016B1 RID: 5809 RVA: 0x000C6D80 File Offset: 0x000C4F80
		public FixedPointerToHeapAllocatedMem ConfigPointer { get; set; }

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x060016B2 RID: 5810 RVA: 0x000C6D89 File Offset: 0x000C4F89
		// (set) Token: 0x060016B3 RID: 5811 RVA: 0x000C6D91 File Offset: 0x000C4F91
		public FixedPointerToHeapAllocatedMem ModelPointer { get; set; }

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x060016B4 RID: 5812 RVA: 0x000C6D9A File Offset: 0x000C4F9A
		// (set) Token: 0x060016B5 RID: 5813 RVA: 0x000C6DA2 File Offset: 0x000C4FA2
		public bool Disposed { get; private set; }

		// Token: 0x060016B6 RID: 5814 RVA: 0x000C6DAC File Offset: 0x000C4FAC
		public static TTSVoiceNative LoadVoiceFromResources(string voiceName)
		{
			TextAsset textAsset = Resources.Load<TextAsset>(voiceName ?? "");
			TextAsset textAsset2 = Resources.Load<TextAsset>(voiceName + ".config");
			if (textAsset == null)
			{
				Debug.LogError("Failed to find voice model " + voiceName + ".bytes in Resources");
				return null;
			}
			if (textAsset2 == null)
			{
				Debug.LogError("Failed to find voice model " + voiceName + ".config.json in Resources");
				return null;
			}
			byte[] bytes = textAsset2.bytes;
			byte[] bytes2 = textAsset.bytes;
			FixedPointerToHeapAllocatedMem fixedPointerToHeapAllocatedMem = FixedPointerToHeapAllocatedMem.Create<byte[]>(bytes, (uint)bytes.Length);
			FixedPointerToHeapAllocatedMem fixedPointerToHeapAllocatedMem2 = FixedPointerToHeapAllocatedMem.Create<byte[]>(bytes2, (uint)bytes2.Length);
			IntPtr intPtr = TTSNative.OvertoneLoadVoice(fixedPointerToHeapAllocatedMem.Address, fixedPointerToHeapAllocatedMem.SizeInBytes, fixedPointerToHeapAllocatedMem2.Address, fixedPointerToHeapAllocatedMem2.SizeInBytes);
			TTSNative.OvertoneSetSpeakerId(intPtr, 0L);
			return new TTSVoiceNative
			{
				Pointer = intPtr,
				ConfigPointer = fixedPointerToHeapAllocatedMem,
				ModelPointer = fixedPointerToHeapAllocatedMem2
			};
		}

		// Token: 0x060016B7 RID: 5815 RVA: 0x000C6E81 File Offset: 0x000C5081
		public void SetSpeakerId(int speakerId)
		{
			TTSNative.OvertoneSetSpeakerId(this.Pointer, (long)speakerId);
		}

		// Token: 0x060016B8 RID: 5816 RVA: 0x000C6E90 File Offset: 0x000C5090
		public void AcquireReaderLock()
		{
			this._lock.AcquireReaderLock(8000);
		}

		// Token: 0x060016B9 RID: 5817 RVA: 0x000C6EA2 File Offset: 0x000C50A2
		public void ReleaseReaderLock()
		{
			this._lock.ReleaseReaderLock();
		}

		// Token: 0x060016BA RID: 5818 RVA: 0x000C6EB0 File Offset: 0x000C50B0
		public void Dispose()
		{
			this._lock.AcquireWriterLock(8000);
			this.Disposed = true;
			this.ConfigPointer.Free();
			this.ModelPointer.Free();
			TTSNative.OvertoneFreeVoice(this.Pointer);
			this._lock.ReleaseWriterLock();
		}

		// Token: 0x04002721 RID: 10017
		public const int Timeout = 8000;

		// Token: 0x04002726 RID: 10022
		private readonly ReaderWriterLock _lock = new ReaderWriterLock();
	}
}
