using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200025D RID: 605
public class TVCartoonController : MonoBehaviour
{
	// Token: 0x06001368 RID: 4968 RVA: 0x000AD004 File Offset: 0x000AB204
	private void Start()
	{
		this.TVStaticOutro = false;
		this.TVActivated = false;
		this.TVBackground.enabled = false;
		this.TVStatic.enabled = false;
		this.TVStaticIntro = true;
		this.TVActiveTime = Random.Range(this.TVActiveTimeMin, this.TVActiveTimeMax);
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

	// Token: 0x06001369 RID: 4969 RVA: 0x000AD0C4 File Offset: 0x000AB2C4
	private IEnumerator AnimationCoroutine()
	{
		for (;;)
		{
			yield return new WaitForSeconds(this.updateInterval);
			float num = this.updateInterval;
			if (this.TrapDone)
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
					this.catTalkSound1.Play(base.transform.position, 1f, 1f, 1f, 1f);
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
						this.catTalkSound2.Play(base.transform.position, 1f, 1f, 1f, 1f);
					}
					else
					{
						this.catTalkCounter = 0;
						this.state = this.stateMouseTalk;
						this.mouseTalkSound1.Play(base.transform.position, 1f, 1f, 1f, 1f);
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
						this.mouseTalkSound2.Play(base.transform.position, 1f, 1f, 1f, 1f);
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

	// Token: 0x0600136A RID: 4970 RVA: 0x000AD0D4 File Offset: 0x000AB2D4
	private void Update()
	{
		this.LoopSound.PlayLoop(this.TVActivated, 0.9f, 0.9f, 1f);
		if (this.TVActivated)
		{
			if (this.isLocal)
			{
				this.TVActiveTime -= Time.deltaTime;
			}
			if (this.TVActiveTime <= this.TVStaticTime)
			{
				bool tvstaticOutro = this.TVStaticOutro;
			}
			if (this.TVStart)
			{
				this.TVBackground.enabled = true;
				this.TVLight.enabled = true;
				this.TVStart = false;
				this.StartSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
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
				}
			}
			float tvactiveTime = this.TVActiveTime;
		}
	}

	// Token: 0x0400213A RID: 8506
	public GameObject TVScreen;

	// Token: 0x0400213B RID: 8507
	public float runTime = 10f;

	// Token: 0x0400213C RID: 8508
	public float TVActiveTimeMin = 20f;

	// Token: 0x0400213D RID: 8509
	public float TVActiveTimeMax = 35f;

	// Token: 0x0400213E RID: 8510
	private float TVActiveTime;

	// Token: 0x0400213F RID: 8511
	private float Timer;

	// Token: 0x04002140 RID: 8512
	private float speedMulti = 1f;

	// Token: 0x04002141 RID: 8513
	public MeshRenderer TVBackground;

	// Token: 0x04002142 RID: 8514
	public GameObject CatObject;

	// Token: 0x04002143 RID: 8515
	public GameObject MouseObject;

	// Token: 0x04002144 RID: 8516
	public MeshRenderer TVStatic;

	// Token: 0x04002145 RID: 8517
	public Light TVLight;

	// Token: 0x04002146 RID: 8518
	public AnimationCurve TVStaticCurve;

	// Token: 0x04002147 RID: 8519
	public float TVStaticTime = 0.5f;

	// Token: 0x04002148 RID: 8520
	public float TVStaticTimer;

	// Token: 0x04002149 RID: 8521
	public TrapTV trapTV;

	// Token: 0x0400214A RID: 8522
	[Space]
	[Header("___________________ Cartoon Cat Talk ___________________")]
	public float catTalkTime = 10f;

	// Token: 0x0400214B RID: 8523
	public float catTalkPauseTime = 2f;

	// Token: 0x0400214C RID: 8524
	public int catTalkCount = 2;

	// Token: 0x0400214D RID: 8525
	private int catTalkCounter;

	// Token: 0x0400214E RID: 8526
	public float catTalkStartScale = 0.2f;

	// Token: 0x0400214F RID: 8527
	public float catTalkEndScale = 0.277f;

	// Token: 0x04002150 RID: 8528
	public Sound catTalkSound1;

	// Token: 0x04002151 RID: 8529
	public Sound catTalkSound2;

	// Token: 0x04002152 RID: 8530
	[Space]
	[Header("___________________ Cartoon Mouse Talk ___________________")]
	public float mouseTalkTime = 10f;

	// Token: 0x04002153 RID: 8531
	public float mouseTalkPauseTime = 2f;

	// Token: 0x04002154 RID: 8532
	public int mouseTalkCount = 2;

	// Token: 0x04002155 RID: 8533
	private int mouseTalkCounter;

	// Token: 0x04002156 RID: 8534
	public float mouseTalkStartScale = 0.12f;

	// Token: 0x04002157 RID: 8535
	public float mouseTalkEndScale = 0.155f;

	// Token: 0x04002158 RID: 8536
	public Sound mouseTalkSound1;

	// Token: 0x04002159 RID: 8537
	public Sound mouseTalkSound2;

	// Token: 0x0400215A RID: 8538
	private int state;

	// Token: 0x0400215B RID: 8539
	private int stateRunning;

	// Token: 0x0400215C RID: 8540
	private int stateCatTalk = 1;

	// Token: 0x0400215D RID: 8541
	private int stateCatTalkPause = 2;

	// Token: 0x0400215E RID: 8542
	private int stateMouseTalk = 3;

	// Token: 0x0400215F RID: 8543
	private int stateMouseTalkPause = 4;

	// Token: 0x04002160 RID: 8544
	private float updateInterval = 0.083333336f;

	// Token: 0x04002161 RID: 8545
	private bool TVStaticIntro = true;

	// Token: 0x04002162 RID: 8546
	public bool TVStaticOutro;

	// Token: 0x04002163 RID: 8547
	public bool isLocal;

	// Token: 0x04002164 RID: 8548
	[Space]
	[Header("___________________ TV Sounds ___________________")]
	public Sound LoopSound;

	// Token: 0x04002165 RID: 8549
	public Sound StartSound;

	// Token: 0x04002166 RID: 8550
	public Sound StopSound;

	// Token: 0x04002167 RID: 8551
	[HideInInspector]
	public bool TVActivated;

	// Token: 0x04002168 RID: 8552
	[HideInInspector]
	public bool TrapDone;

	// Token: 0x04002169 RID: 8553
	private bool TVStart = true;

	// Token: 0x0400216A RID: 8554
	private Material CatMaterial;

	// Token: 0x0400216B RID: 8555
	private Material MouseMaterial;

	// Token: 0x0400216C RID: 8556
	private PhotonView photonView;
}
