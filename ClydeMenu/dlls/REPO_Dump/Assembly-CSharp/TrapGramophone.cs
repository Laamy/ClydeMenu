using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200024E RID: 590
public class TrapGramophone : Trap
{
	// Token: 0x06001322 RID: 4898 RVA: 0x000AAD38 File Offset: 0x000A8F38
	protected override void Start()
	{
		base.Start();
		this.initialGramophoneRotation = this.Gramophone.transform.localRotation;
		this.randomRange = base.GetComponent<SyncedEventRandom>();
	}

	// Token: 0x06001323 RID: 4899 RVA: 0x000AAD64 File Offset: 0x000A8F64
	protected override void Update()
	{
		base.Update();
		this.GramophoneMusic.PlayLoop(this.MusicPlaying, 0.5f, 0.5f, 1f);
		if (this.trapStart)
		{
			this.TrapActivate();
		}
		if (this.MusicStart && !this.MusicStartPointFetched)
		{
			this.randomRange.RandomRangeFloat(0f, this.GramophoneMusic.Source.clip.length);
			if (this.randomRange.resultReceivedRandomRangeFloat)
			{
				this.MusicStartPointFetched = true;
				this.GramophoneMusic.StartTimeOverride = this.randomRange.resultRandomRangeFloat;
				this.MusicPlaying = true;
			}
		}
		if (this.trapActive)
		{
			this.enemyInvestigate = true;
			this.GramophoneRecord.transform.Rotate(0f, 20f * Time.deltaTime, 0f);
			this.GramophoneCrank.transform.Rotate(0f, 0f, 20f * Time.deltaTime);
			float num = 1f;
			if (this.StartSequenceProgress == 0f && !this.StartSequence)
			{
				this.StartSequence = true;
				this.GramophoneStart.Play(this.physGrabObject.centerPoint, 1f, 1f, 1f, 1f);
				this.GramophoneMusic.LoopPitch = 0.2f;
			}
			if (this.StartSequence)
			{
				num += this.GramophoneStartCurve.Evaluate(this.StartSequenceProgress) * this.GramophoneStartIntensity;
				this.GramophoneMusic.LoopPitch = 1f - this.GramophoneStartCurve.Evaluate(this.StartSequenceProgress);
				this.StartSequenceProgress += Time.deltaTime / this.GramophoneStartDuration;
				if (this.StartSequenceProgress >= 1f)
				{
					this.StartSequence = false;
				}
			}
			if (this.endSequence)
			{
				num += this.GramophoneEndCurve.Evaluate(this.EndSequenceProgress) * this.GramophoneEndIntensity;
				this.EndSequenceProgress += Time.deltaTime / this.GramophoneEndDuration;
				this.GramophoneMusic.LoopPitch -= 0.5f * Time.deltaTime;
				if (this.EndSequenceProgress >= 1f)
				{
					this.EndSequenceDone();
				}
			}
			float num2 = 1f * num;
			float num3 = 40f;
			float num4 = num2 * Mathf.Sin(Time.time * num3);
			float z = num2 * Mathf.Sin(Time.time * num3 + 1.5707964f);
			this.Gramophone.transform.localRotation = this.initialGramophoneRotation * Quaternion.Euler(num4, 0f, z);
			this.Gramophone.transform.localPosition = new Vector3(this.Gramophone.transform.localPosition.x, this.Gramophone.transform.localPosition.y - num4 * 0.005f * Time.deltaTime, this.Gramophone.transform.localPosition.z);
		}
	}

	// Token: 0x06001324 RID: 4900 RVA: 0x000AB05B File Offset: 0x000A925B
	private void EndSequenceDone()
	{
		this.trapActive = false;
		this.endSequence = false;
		this.MusicPlaying = false;
	}

	// Token: 0x06001325 RID: 4901 RVA: 0x000AB072 File Offset: 0x000A9272
	public void TrapStop()
	{
		this.endSequence = true;
		this.GramophoneEnd.Play(this.physGrabObject.centerPoint, 1f, 1f, 1f, 1f);
	}

	// Token: 0x06001326 RID: 4902 RVA: 0x000AB0A6 File Offset: 0x000A92A6
	public void TrapActivate()
	{
		if (!this.trapTriggered)
		{
			this.MusicStart = true;
			this.gramophoneTimer.Invoke();
			this.trapActive = true;
			this.trapTriggered = true;
		}
	}

	// Token: 0x0400207F RID: 8319
	public UnityEvent gramophoneTimer;

	// Token: 0x04002080 RID: 8320
	[Space]
	[Header("Gramophone Components")]
	public GameObject Gramophone;

	// Token: 0x04002081 RID: 8321
	public GameObject GramophoneRecord;

	// Token: 0x04002082 RID: 8322
	public GameObject GramophoneCrank;

	// Token: 0x04002083 RID: 8323
	[Space]
	[Header("Sounds")]
	public Sound GramophoneStart;

	// Token: 0x04002084 RID: 8324
	public Sound GramophoneEnd;

	// Token: 0x04002085 RID: 8325
	public Sound GramophoneMusic;

	// Token: 0x04002086 RID: 8326
	[Space]
	[Header("Gramophone Animation")]
	public AnimationCurve GramophoneStartCurve;

	// Token: 0x04002087 RID: 8327
	public float GramophoneStartIntensity;

	// Token: 0x04002088 RID: 8328
	public float GramophoneStartDuration;

	// Token: 0x04002089 RID: 8329
	[Space]
	public AnimationCurve GramophoneEndCurve;

	// Token: 0x0400208A RID: 8330
	public float GramophoneEndIntensity;

	// Token: 0x0400208B RID: 8331
	public float GramophoneEndDuration;

	// Token: 0x0400208C RID: 8332
	private bool StartSequence;

	// Token: 0x0400208D RID: 8333
	private bool endSequence;

	// Token: 0x0400208E RID: 8334
	private float StartSequenceProgress;

	// Token: 0x0400208F RID: 8335
	private float EndSequenceProgress;

	// Token: 0x04002090 RID: 8336
	private bool MusicPlaying;

	// Token: 0x04002091 RID: 8337
	private bool MusicStartPointFetched;

	// Token: 0x04002092 RID: 8338
	private bool MusicStart;

	// Token: 0x04002093 RID: 8339
	private SyncedEventRandom randomRange;

	// Token: 0x04002094 RID: 8340
	private Quaternion initialGramophoneRotation;
}
