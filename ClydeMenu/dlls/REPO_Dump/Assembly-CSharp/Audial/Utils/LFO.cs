using System;
using System.Collections;
using UnityEngine;

namespace Audial.Utils
{
	// Token: 0x020002F4 RID: 756
	public class LFO
	{
		// Token: 0x17000052 RID: 82
		// (get) Token: 0x060017A6 RID: 6054 RVA: 0x000C999B File Offset: 0x000C7B9B
		// (set) Token: 0x060017A7 RID: 6055 RVA: 0x000C99A3 File Offset: 0x000C7BA3
		public float Index
		{
			get
			{
				return this._index;
			}
			set
			{
				this._index = value;
				if (this._index >= (float)this.tableLength - 0.5f)
				{
					this._index -= (float)this.tableLength;
				}
			}
		}

		// Token: 0x060017A8 RID: 6056 RVA: 0x000C99D5 File Offset: 0x000C7BD5
		public IEnumerator Run()
		{
			this.runState = RunState.Running;
			while (this.runState != RunState.Stopped)
			{
				if (this.runState == RunState.Running)
				{
					this.Index += (float)this.tableLength / this.StepSize * Time.deltaTime;
				}
				yield return new WaitForSeconds(0.002f);
			}
			yield break;
		}

		// Token: 0x060017A9 RID: 6057 RVA: 0x000C99E4 File Offset: 0x000C7BE4
		public void Pause()
		{
			this.runState = RunState.Paused;
		}

		// Token: 0x060017AA RID: 6058 RVA: 0x000C99ED File Offset: 0x000C7BED
		public void Resume()
		{
			this.runState = RunState.Running;
		}

		// Token: 0x060017AB RID: 6059 RVA: 0x000C99F6 File Offset: 0x000C7BF6
		public void Stop()
		{
			this.runState = RunState.Stopped;
		}

		// Token: 0x17000053 RID: 83
		// (get) Token: 0x060017AC RID: 6060 RVA: 0x000C99FF File Offset: 0x000C7BFF
		// (set) Token: 0x060017AD RID: 6061 RVA: 0x000C9A07 File Offset: 0x000C7C07
		public float StepSize
		{
			get
			{
				return this._stepSize;
			}
			set
			{
				this._stepSize = value;
			}
		}

		// Token: 0x060017AE RID: 6062 RVA: 0x000C9A10 File Offset: 0x000C7C10
		public void SetRate(float rate)
		{
			this.StepSize = rate;
		}

		// Token: 0x060017AF RID: 6063 RVA: 0x000C9A19 File Offset: 0x000C7C19
		public int GetIndex()
		{
			return Mathf.RoundToInt(this.Index) % LFO.waveTable.Length;
		}

		// Token: 0x060017B0 RID: 6064 RVA: 0x000C9A2E File Offset: 0x000C7C2E
		public float GetValue()
		{
			return LFO.waveTable[this.GetIndex()];
		}

		// Token: 0x060017B1 RID: 6065 RVA: 0x000C9A3C File Offset: 0x000C7C3C
		public void MoveIndex()
		{
			this.Index += (float)this.tableLength / this.StepSize / Settings.SampleRate;
		}

		// Token: 0x060017B2 RID: 6066 RVA: 0x000C9A60 File Offset: 0x000C7C60
		public float[] GetChunkValue(int chunkLength)
		{
			float[] array = new float[chunkLength];
			for (int i = 0; i < chunkLength; i++)
			{
				array[i] = this.GetValue();
			}
			return array;
		}

		// Token: 0x060017B3 RID: 6067 RVA: 0x000C9A8A File Offset: 0x000C7C8A
		public LFO()
		{
			if (LFO.waveTable == null)
			{
				this.CreateWavetable();
			}
		}

		// Token: 0x060017B4 RID: 6068 RVA: 0x000C9AB5 File Offset: 0x000C7CB5
		public LFO(float speed)
		{
			if (LFO.waveTable == null)
			{
				this.CreateWavetable();
			}
			this.StepSize = speed;
		}

		// Token: 0x060017B5 RID: 6069 RVA: 0x000C9AE8 File Offset: 0x000C7CE8
		private void CreateWavetable()
		{
			LFO.waveTable = new float[this.tableLength];
			for (int i = 0; i < this.tableLength; i++)
			{
				LFO.waveTable[i] = 0.5f + Mathf.Sin(6.2831855f * (float)i / (float)this.tableLength) / 2f;
			}
		}

		// Token: 0x040027D8 RID: 10200
		private int tableLength = 128;

		// Token: 0x040027D9 RID: 10201
		public static float[] waveTable;

		// Token: 0x040027DA RID: 10202
		private float _index;

		// Token: 0x040027DB RID: 10203
		private RunState runState;

		// Token: 0x040027DC RID: 10204
		private float _stepSize = 0.3f;
	}
}
