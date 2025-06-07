using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200025B RID: 603
public class TrapTV : Trap
{
	// Token: 0x0600135F RID: 4959 RVA: 0x000ACC0C File Offset: 0x000AAE0C
	protected override void Start()
	{
		base.Start();
		this.TVStaticOutro = false;
		this.TVBackground.enabled = false;
		this.TVStatic.enabled = false;
		this.TVStaticIntro = true;
		this.TVLight.enabled = false;
		this.CatObject.SetActive(false);
		this.MouseObject.SetActive(false);
		this.CatMaterial = this.CatObject.GetComponent<Renderer>().material;
		this.MouseMaterial = this.MouseObject.GetComponent<Renderer>().material;
		this.photonView = base.GetComponent<PhotonView>();
		if (GameManager.instance.gameMode == 0)
		{
			this.isLocal = true;
		}
	}

	// Token: 0x06001360 RID: 4960 RVA: 0x000ACCB4 File Offset: 0x000AAEB4
	private IEnumerator AnimationCoroutine()
	{
		for (;;)
		{
			yield return new WaitForSeconds(this.updateInterval);
			float num = this.updateInterval;
			if (!this.trapActive)
			{
				break;
			}
			float y = 0.5f;
			if (this.state == this.stateCatTalk)
			{
				y = 0f;
			}
			if (this.CatMaterial.mainTextureOffset.x < 1f)
			{
				this.CatMaterial.mainTextureOffset = new Vector2(this.CatMaterial.mainTextureOffset.x + 0.33f, y);
			}
			else
			{
				this.CatMaterial.mainTextureOffset = new Vector2(0f, y);
			}
			float y2 = 0.5f;
			if (this.state == this.stateMouseTalk)
			{
				y2 = 0f;
			}
			if (this.CatMaterial.mainTextureOffset.x < 1f)
			{
				this.MouseMaterial.mainTextureOffset = new Vector2(this.MouseMaterial.mainTextureOffset.x + 0.33f, y2);
			}
			else
			{
				this.MouseMaterial.mainTextureOffset = new Vector2(0f, y2);
			}
			if (this.state == this.stateRunning)
			{
				this.Timer += num * this.speedMulti;
				if (this.Timer > this.runTime)
				{
					this.Timer = 0f;
					this.state = this.stateCatTalk;
					this.catTalkSound1.Play(this.physGrabObject.centerPoint, 1f, 1f, 1f, 1f);
				}
			}
			if (this.state == this.stateCatTalk)
			{
				this.Timer += num * this.speedMulti;
				if (this.Timer > this.catTalkTime)
				{
					this.Timer = 0f;
					this.state = this.stateCatTalkPause;
					this.catTalkCounter++;
				}
			}
			if (this.state == this.stateCatTalkPause)
			{
				this.Timer += num * this.speedMulti;
				if (this.Timer > this.catTalkPauseTime)
				{
					this.Timer = 0f;
					if (this.catTalkCounter < this.catTalkCount)
					{
						this.state = this.stateCatTalk;
						this.catTalkSound2.Play(this.physGrabObject.centerPoint, 1f, 1f, 1f, 1f);
					}
					else
					{
						this.catTalkCounter = 0;
						this.state = this.stateMouseTalk;
						this.mouseTalkSound1.Play(this.physGrabObject.centerPoint, 1f, 1f, 1f, 1f);
					}
				}
			}
			if (this.state == this.stateMouseTalk)
			{
				this.Timer += num * this.speedMulti;
				if (this.Timer > this.mouseTalkTime)
				{
					this.Timer = 0f;
					this.state = this.stateMouseTalkPause;
					this.mouseTalkCounter++;
				}
			}
			if (this.state == this.stateMouseTalkPause)
			{
				this.Timer += num * this.speedMulti;
				if (this.Timer > this.mouseTalkPauseTime)
				{
					this.Timer = 0f;
					if (this.mouseTalkCounter < this.mouseTalkCount)
					{
						this.state = this.stateMouseTalk;
						this.mouseTalkSound2.Play(this.physGrabObject.centerPoint, 1f, 1f, 1f, 1f);
					}
					else
					{
						this.mouseTalkCounter = 0;
						this.state = this.stateRunning;
					}
				}
			}
		}
		yield break;
		yield break;
	}

	// Token: 0x06001361 RID: 4961 RVA: 0x000ACCC3 File Offset: 0x000AAEC3
	public void TrapActivate()
	{
		if (!this.trapTriggered)
		{
			this.trapActive = true;
			this.trapTriggered = true;
			this.TVStart = true;
			this.TVTimer.Invoke();
		}
	}

	// Token: 0x06001362 RID: 4962 RVA: 0x000ACCED File Offset: 0x000AAEED
	public void TrapStop()
	{
		this.TVStaticOutro = true;
		this.TVStaticTimer = 0f;
		this.StopSound.Play(this.physGrabObject.centerPoint, 1f, 1f, 1f, 1f);
	}

	// Token: 0x06001363 RID: 4963 RVA: 0x000ACD2C File Offset: 0x000AAF2C
	protected override void Update()
	{
		base.Update();
		this.LoopSound.PlayLoop(this.trapActive, 0.9f, 0.9f, 1f);
		if (this.trapStart)
		{
			this.TrapActivate();
		}
		if (this.trapActive)
		{
			this.enemyInvestigate = true;
			if (this.TVStart)
			{
				this.TVBackground.enabled = true;
				this.TVLight.enabled = true;
				this.TVStart = false;
				this.StartSound.Play(this.physGrabObject.centerPoint, 1f, 1f, 1f, 1f);
				this.CatObject.SetActive(true);
				this.MouseObject.SetActive(true);
				base.StartCoroutine(this.AnimationCoroutine());
			}
			if (this.TVStaticIntro || this.TVStaticOutro)
			{
				float num = this.TVStaticCurve.Evaluate(this.TVStaticTimer / this.TVStaticTime);
				this.TVStaticTimer += 1f * Time.deltaTime * this.speedMulti;
				if (num > 0.5f)
				{
					this.TVStatic.enabled = true;
				}
				else
				{
					this.TVStatic.enabled = false;
				}
				if (this.TVStaticTimer > this.TVStaticTime)
				{
					this.TVStaticIntro = false;
					this.TVStaticTimer = 0f;
					this.TVStatic.enabled = false;
					if (this.TVStaticOutro)
					{
						this.trapActive = false;
						this.TVScreen.SetActive(false);
					}
				}
			}
		}
	}

	// Token: 0x0400210A RID: 8458
	public GameObject TVScreen;

	// Token: 0x0400210B RID: 8459
	public UnityEvent TVTimer;

	// Token: 0x0400210C RID: 8460
	public float runTime = 10f;

	// Token: 0x0400210D RID: 8461
	private float Timer;

	// Token: 0x0400210E RID: 8462
	private float speedMulti = 1f;

	// Token: 0x0400210F RID: 8463
	public MeshRenderer TVBackground;

	// Token: 0x04002110 RID: 8464
	public GameObject CatObject;

	// Token: 0x04002111 RID: 8465
	public GameObject MouseObject;

	// Token: 0x04002112 RID: 8466
	public MeshRenderer TVStatic;

	// Token: 0x04002113 RID: 8467
	public Light TVLight;

	// Token: 0x04002114 RID: 8468
	public AnimationCurve TVStaticCurve;

	// Token: 0x04002115 RID: 8469
	public float TVStaticTime = 0.5f;

	// Token: 0x04002116 RID: 8470
	public float TVStaticTimer;

	// Token: 0x04002117 RID: 8471
	[Space]
	[Header("___________________ Cartoon Cat Talk ___________________")]
	public float catTalkTime = 10f;

	// Token: 0x04002118 RID: 8472
	public float catTalkPauseTime = 2f;

	// Token: 0x04002119 RID: 8473
	public int catTalkCount = 2;

	// Token: 0x0400211A RID: 8474
	private int catTalkCounter;

	// Token: 0x0400211B RID: 8475
	public float catTalkStartScale = 0.2f;

	// Token: 0x0400211C RID: 8476
	public float catTalkEndScale = 0.277f;

	// Token: 0x0400211D RID: 8477
	public Sound catTalkSound1;

	// Token: 0x0400211E RID: 8478
	public Sound catTalkSound2;

	// Token: 0x0400211F RID: 8479
	[Space]
	[Header("___________________ Cartoon Mouse Talk ___________________")]
	public float mouseTalkTime = 10f;

	// Token: 0x04002120 RID: 8480
	public float mouseTalkPauseTime = 2f;

	// Token: 0x04002121 RID: 8481
	public int mouseTalkCount = 2;

	// Token: 0x04002122 RID: 8482
	private int mouseTalkCounter;

	// Token: 0x04002123 RID: 8483
	public float mouseTalkStartScale = 0.12f;

	// Token: 0x04002124 RID: 8484
	public float mouseTalkEndScale = 0.155f;

	// Token: 0x04002125 RID: 8485
	public Sound mouseTalkSound1;

	// Token: 0x04002126 RID: 8486
	public Sound mouseTalkSound2;

	// Token: 0x04002127 RID: 8487
	private int state;

	// Token: 0x04002128 RID: 8488
	private int stateRunning;

	// Token: 0x04002129 RID: 8489
	private int stateCatTalk = 1;

	// Token: 0x0400212A RID: 8490
	private int stateCatTalkPause = 2;

	// Token: 0x0400212B RID: 8491
	private int stateMouseTalk = 3;

	// Token: 0x0400212C RID: 8492
	private int stateMouseTalkPause = 4;

	// Token: 0x0400212D RID: 8493
	private float updateInterval = 0.083333336f;

	// Token: 0x0400212E RID: 8494
	private bool TVStaticIntro = true;

	// Token: 0x0400212F RID: 8495
	public bool TVStaticOutro;

	// Token: 0x04002130 RID: 8496
	[Space]
	[Header("___________________ TV Sounds ___________________")]
	public Sound LoopSound;

	// Token: 0x04002131 RID: 8497
	public Sound StartSound;

	// Token: 0x04002132 RID: 8498
	public Sound StopSound;

	// Token: 0x04002133 RID: 8499
	[HideInInspector]
	public bool TrapDone;

	// Token: 0x04002134 RID: 8500
	private bool TVStart = true;

	// Token: 0x04002135 RID: 8501
	private Material CatMaterial;

	// Token: 0x04002136 RID: 8502
	private Material MouseMaterial;
}
