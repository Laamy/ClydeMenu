using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x0200028E RID: 654
public class TutorialDoor : MonoBehaviour
{
	// Token: 0x06001485 RID: 5253 RVA: 0x000B50C9 File Offset: 0x000B32C9
	private void Start()
	{
		this.doorEndYPos = 7.42f;
		this.doorLight.intensity = 0f;
	}

	// Token: 0x06001486 RID: 5254 RVA: 0x000B50E8 File Offset: 0x000B32E8
	private void ThirtyFPS()
	{
		if (this.thirtyFPSUpdateTimer > 0f)
		{
			this.thirtyFPSUpdateTimer -= Time.deltaTime;
			this.thirtyFPSUpdateTimer = Mathf.Max(0f, this.thirtyFPSUpdateTimer);
			return;
		}
		this.thirtyFPSUpdate = true;
		this.thirtyFPSUpdateTimer = 0.033333335f;
	}

	// Token: 0x06001487 RID: 5255 RVA: 0x000B5140 File Offset: 0x000B3340
	private void Update()
	{
		this.StateMachine();
		if (this.fillBarProgress != this.fillBarProgressPrev)
		{
			if (this.fillBarProgress > this.fillBarProgressPrev)
			{
				this.soundGoUp.Pitch = 1f + this.fillBarProgress / 11f;
				this.soundGoUp.Play(this.animationTransform.position, 1f, 1f, 1f, 1f);
				this.particlesBleep1.Play();
				this.particlesBleep2.Play();
			}
			else
			{
				this.soundGoDown.Pitch = 1f + this.fillBarProgress / 11f;
				this.soundGoDown.Play(this.animationTransform.position, 1f, 1f, 1f, 1f);
			}
			this.fillBarProgressPrev = this.fillBarProgress;
		}
	}

	// Token: 0x06001488 RID: 5256 RVA: 0x000B5228 File Offset: 0x000B3428
	private void StateMachine()
	{
		this.ThirtyFPS();
		switch (this.currentState)
		{
		case 0:
			this.StateClosed();
			break;
		case 1:
			this.StateSuccess();
			break;
		case 2:
			this.StateUnlock();
			break;
		case 3:
			this.StateOpening();
			break;
		case 4:
			this.StateOpen();
			break;
		}
		this.EmojiScreenGlitchLogic();
		this.thirtyFPSUpdate = false;
		this.stateTimer += Time.deltaTime;
		if (this.stateTimer > 1000000f)
		{
			this.stateTimer = 0f;
		}
	}

	// Token: 0x06001489 RID: 5257 RVA: 0x000B52BB File Offset: 0x000B34BB
	private void StateSet(int _state)
	{
		this.prevState = this.currentState;
		this.stateTimer = 0f;
		this.currentState = _state;
		this.stateStart = true;
		this.animationDone = false;
		this.animationProgress = 0f;
		this.animationImpactDone = false;
	}

	// Token: 0x0600148A RID: 5258 RVA: 0x000B52FC File Offset: 0x000B34FC
	private void EffectEmoji()
	{
		GameDirector.instance.CameraShake.ShakeDistance(3f, 3f, 8f, this.animationTransform.position, 0.1f);
		this.soundSuccess.Play(this.animationTransform.position, 1f, 1f, 1f, 1f);
		this.lightParticle.transform.localPosition = new Vector3(-0.82f, 3.3f, 0f);
		this.lightParticle.Play();
		this.doorLight.color = new Color(1f, 0.5f, 0f, 1f);
		this.doorLight.range = 10f;
		this.doorLight.intensity = 4f;
	}

	// Token: 0x0600148B RID: 5259 RVA: 0x000B53D8 File Offset: 0x000B35D8
	private void EffectScreenRotateStart()
	{
		GameDirector.instance.CameraShake.ShakeDistance(3f, 3f, 8f, this.animationTransform.position, 0.1f);
		this.soundUnlock.Play(this.animationTransform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x0600148C RID: 5260 RVA: 0x000B5440 File Offset: 0x000B3640
	private void EffectScreenRotateEnd()
	{
		GameDirector.instance.CameraShake.ShakeDistance(5f, 3f, 8f, this.animationTransform.position, 0.1f);
		this.soundUnlockEnd.Play(this.animationTransform.position, 1f, 1f, 1f, 1f);
		this.particlesUnlock.Play();
		this.lightParticle.transform.localPosition = new Vector3(-0.82f, 3.3f, 0f);
		this.lightParticle.Play();
	}

	// Token: 0x0600148D RID: 5261 RVA: 0x000B54E0 File Offset: 0x000B36E0
	private void EffectLatchStart()
	{
		GameDirector.instance.CameraShake.ShakeDistance(3f, 3f, 8f, this.animationTransform.position, 0.1f);
		this.soundLatches.Play(this.animationTransform.position, 1f, 1f, 1f, 1f);
		this.lightParticle.transform.localPosition = new Vector3(-0.82f, 3.3f, 4.57f);
		this.lightParticle.Play();
		this.lightParticle2.transform.localPosition = new Vector3(-0.82f, 3.3f, -4.57f);
		this.lightParticle2.Play();
		this.latchLamp1.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", new Color(0f, 1f, 0f, 1f));
		this.latchLamp2.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", new Color(0f, 1f, 0f, 1f));
	}

	// Token: 0x0600148E RID: 5262 RVA: 0x000B560C File Offset: 0x000B380C
	private void EffectLatchEnd()
	{
		GameDirector.instance.CameraShake.ShakeDistance(5f, 3f, 8f, base.transform.position, 0.1f);
		this.soundLatchesEnd.Play(base.transform.position, 1f, 1f, 1f, 1f);
		this.particlesLatch1.Play();
		this.particlesLatch2.Play();
		this.lightParticle.transform.localPosition = new Vector3(-0.82f, 3.3f, 3.3f);
		this.lightParticle.Play();
		this.lightParticle2.transform.localPosition = new Vector3(-0.82f, 3.3f, -3.3f);
		this.lightParticle2.Play();
	}

	// Token: 0x0600148F RID: 5263 RVA: 0x000B56E8 File Offset: 0x000B38E8
	private void EffectDoorOpenStart()
	{
		GameDirector.instance.CameraShake.ShakeDistance(8f, 3f, 8f, this.animationTransform.position, 0.1f);
		this.soundDoorOpen.Play(this.animationTransform.position, 1f, 1f, 1f, 1f);
		this.particlesDoorSmoke.Play();
	}

	// Token: 0x06001490 RID: 5264 RVA: 0x000B5759 File Offset: 0x000B3959
	private void EffectDoorMove()
	{
		this.soundDoorMove.Play(this.animationTransform.position, 1f, 1f, 1f, 1f);
		this.particlesOpen.gameObject.SetActive(true);
	}

	// Token: 0x06001491 RID: 5265 RVA: 0x000B5798 File Offset: 0x000B3998
	private void EffectDoorOpenEnd()
	{
		GameDirector.instance.CameraShake.ShakeDistance(8f, 3f, 8f, this.animationTransform.position, 0.1f);
		this.soundSlamCeiling.Play(this.animationTransform.position, 1f, 1f, 1f, 1f);
		this.particlesCeiling.Play();
		Object.Destroy(this.doorLight);
	}

	// Token: 0x06001492 RID: 5266 RVA: 0x000B5814 File Offset: 0x000B3A14
	private void StateClosed()
	{
		if (TutorialDirector.instance.currentPage > this.tutorialPage)
		{
			this.StateSet(1);
		}
		float num = TutorialUI.instance.progressBarCurrent * 130f;
		this.fillBarProgress = (float)Mathf.FloorToInt(num / 11f);
		for (int i = 0; i < this.fillBars.Count; i++)
		{
			this.fillBars[i].localScale = new Vector3(1f, 1f, Mathf.Clamp01(this.fillBarProgress / 11f));
		}
	}

	// Token: 0x06001493 RID: 5267 RVA: 0x000B58A5 File Offset: 0x000B3AA5
	private void EmojiSet(string emoji)
	{
		this.doorText.text = "<size=100>|</size>" + emoji + "<size=100>|</size>";
	}

	// Token: 0x06001494 RID: 5268 RVA: 0x000B58C4 File Offset: 0x000B3AC4
	private void EmojiScreenGlitch(Color color)
	{
		if (this.emojiScreenGlitchTimer <= 0f)
		{
			this.soundEmojiGlitch.Play(this.doorText.transform.position, 1f, 1f, 1f, 1f);
		}
		this.emojiScreenGlitchTimer = 0.2f;
		this.emojiScreenGlitch.SetActive(true);
		this.doorText.enabled = false;
		this.emojiScreenGlitch.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", color);
	}

	// Token: 0x06001495 RID: 5269 RVA: 0x000B594C File Offset: 0x000B3B4C
	private void EmojiScreenGlitchLogic()
	{
		if (this.emojiDelay > 0f)
		{
			return;
		}
		this.currentEmoji = this.doorText.text;
		if (this.prevEmoji != this.currentEmoji)
		{
			this.prevEmoji = this.currentEmoji;
			this.EmojiScreenGlitch(Color.yellow);
		}
		if (this.emojiScreenGlitchTimer <= 0f)
		{
			return;
		}
		Vector2 textureOffset = this.emojiScreenGlitch.GetComponent<MeshRenderer>().material.GetTextureOffset("_MainTex");
		textureOffset.y += Time.deltaTime * 15f;
		this.emojiScreenGlitch.GetComponent<MeshRenderer>().material.SetTextureOffset("_MainTex", textureOffset);
		this.emojiScreenGlitchTimer -= Time.deltaTime;
		if (this.thirtyFPSUpdate)
		{
			float num = Random.Range(0.1f, 1f);
			this.emojiScreenGlitch.GetComponent<MeshRenderer>().material.SetTextureScale("_MainTex", new Vector2(num, num));
		}
		if (this.emojiScreenGlitchTimer <= 0f)
		{
			this.emojiScreenGlitch.SetActive(false);
			this.doorText.enabled = true;
		}
	}

	// Token: 0x06001496 RID: 5270 RVA: 0x000B5A6C File Offset: 0x000B3C6C
	private void StateSuccess()
	{
		if (this.stateStart)
		{
			this.EmojiSet("<sprite name=creepycrying>");
			this.EffectEmoji();
			this.grossupTransform.SetActive(true);
			this.stateStart = false;
			for (int i = 0; i < this.fillBars.Count; i++)
			{
				this.fillBars[i].localScale = new Vector3(1f, 1f, 1f);
			}
		}
		if (!this.animationDone)
		{
			if (this.animationProgress == 0f)
			{
				this.EffectScreenRotateStart();
			}
			this.animationProgress += 2f * Time.deltaTime;
			float t = this.animationCurve.Evaluate(this.animationProgress);
			this.screenTransform.localRotation = Quaternion.Euler(Mathf.LerpUnclamped(0f, 45f, t), 0f, 0f);
			if (this.animationProgress >= 0.53f && !this.animationImpactDone)
			{
				this.EffectScreenRotateEnd();
				this.animationImpactDone = true;
			}
			if (this.animationProgress >= 1f)
			{
				this.animationDone = true;
			}
		}
		if (this.stateTimer > 1f)
		{
			this.StateSet(2);
		}
	}

	// Token: 0x06001497 RID: 5271 RVA: 0x000B5B9C File Offset: 0x000B3D9C
	private void StateUnlock()
	{
		if (this.stateStart)
		{
			this.EffectLatchStart();
			this.stateStart = false;
		}
		this.animationProgress += 1.5f * Time.deltaTime;
		float t = this.animationCurve.Evaluate(this.animationProgress);
		if (this.animationProgress > 0.53f && !this.animationImpactDone)
		{
			this.animationImpactDone = true;
			this.EffectLatchEnd();
		}
		this.latchTransform.localScale = new Vector3(this.latchTransform.localScale.x, this.latchTransform.localScale.y, Mathf.LerpUnclamped(1f, 0.8f, t));
		if (this.animationProgress >= 1f)
		{
			this.animationDone = true;
			this.StateSet(3);
		}
	}

	// Token: 0x06001498 RID: 5272 RVA: 0x000B5C68 File Offset: 0x000B3E68
	private void StateOpening()
	{
		if (this.stateStart)
		{
			this.EffectDoorOpenStart();
			this.stateStart = false;
		}
		if (this.animationProgress > 0.53f && !this.moveDone)
		{
			this.moveDone = true;
			this.EffectDoorMove();
		}
		this.animationProgress += Time.deltaTime;
		float t = this.animationCurveDoor.Evaluate(this.animationProgress);
		this.animationTransform.position = new Vector3(this.animationTransform.position.x, Mathf.LerpUnclamped(0f, this.doorEndYPos, t), this.animationTransform.position.z);
		if (this.animationProgress > 0.53f && !this.animationImpactDone)
		{
			this.animationImpactDone = true;
			this.EffectDoorOpenEnd();
		}
		if (this.animationProgress >= 1f)
		{
			this.animationDone = true;
			this.StateSet(4);
		}
	}

	// Token: 0x06001499 RID: 5273 RVA: 0x000B5D4E File Offset: 0x000B3F4E
	private void StateOpen()
	{
	}

	// Token: 0x04002313 RID: 8979
	public int tutorialPage;

	// Token: 0x04002314 RID: 8980
	public AnimationCurve animationCurve;

	// Token: 0x04002315 RID: 8981
	public AnimationCurve animationCurveDoor;

	// Token: 0x04002316 RID: 8982
	private float doorEndYPos;

	// Token: 0x04002317 RID: 8983
	private float animationProgress;

	// Token: 0x04002318 RID: 8984
	private bool animationDone;

	// Token: 0x04002319 RID: 8985
	private int prevState;

	// Token: 0x0400231A RID: 8986
	private int currentState;

	// Token: 0x0400231B RID: 8987
	private float stateTimer;

	// Token: 0x0400231C RID: 8988
	private bool stateStart;

	// Token: 0x0400231D RID: 8989
	public Transform latchTransform;

	// Token: 0x0400231E RID: 8990
	public Transform screenTransform;

	// Token: 0x0400231F RID: 8991
	private bool animationImpactDone;

	// Token: 0x04002320 RID: 8992
	public GameObject emojiScreenGlitch;

	// Token: 0x04002321 RID: 8993
	public TextMeshPro doorText;

	// Token: 0x04002322 RID: 8994
	private string prevEmoji;

	// Token: 0x04002323 RID: 8995
	private string currentEmoji;

	// Token: 0x04002324 RID: 8996
	private float emojiScreenGlitchTimer;

	// Token: 0x04002325 RID: 8997
	private float emojiDelay;

	// Token: 0x04002326 RID: 8998
	private bool thirtyFPSUpdate;

	// Token: 0x04002327 RID: 8999
	public Sound soundEmojiGlitch;

	// Token: 0x04002328 RID: 9000
	private float thirtyFPSUpdateTimer;

	// Token: 0x04002329 RID: 9001
	public List<Transform> fillBars = new List<Transform>();

	// Token: 0x0400232A RID: 9002
	private float fillBarProgress;

	// Token: 0x0400232B RID: 9003
	private float fillBarProgressPrev = -1f;

	// Token: 0x0400232C RID: 9004
	private bool moveDone;

	// Token: 0x0400232D RID: 9005
	public Transform animationTransform;

	// Token: 0x0400232E RID: 9006
	[FormerlySerializedAs("light")]
	public Light doorLight;

	// Token: 0x0400232F RID: 9007
	public Transform latchLamp1;

	// Token: 0x04002330 RID: 9008
	public Transform latchLamp2;

	// Token: 0x04002331 RID: 9009
	public GameObject grossupTransform;

	// Token: 0x04002332 RID: 9010
	public ParticleSystem particlesCeiling;

	// Token: 0x04002333 RID: 9011
	public Transform particlesOpen;

	// Token: 0x04002334 RID: 9012
	public ParticleSystem particlesUnlock;

	// Token: 0x04002335 RID: 9013
	public ParticleSystem particlesLatch1;

	// Token: 0x04002336 RID: 9014
	public ParticleSystem particlesLatch2;

	// Token: 0x04002337 RID: 9015
	public ParticleSystem particlesDoorSmoke;

	// Token: 0x04002338 RID: 9016
	public ParticleSystem particlesBleep1;

	// Token: 0x04002339 RID: 9017
	public ParticleSystem particlesBleep2;

	// Token: 0x0400233A RID: 9018
	public ParticleSystem lightParticle;

	// Token: 0x0400233B RID: 9019
	public ParticleSystem lightParticle2;

	// Token: 0x0400233C RID: 9020
	public Sound soundGoUp;

	// Token: 0x0400233D RID: 9021
	public Sound soundGoDown;

	// Token: 0x0400233E RID: 9022
	public Sound soundSuccess;

	// Token: 0x0400233F RID: 9023
	public Sound soundUnlock;

	// Token: 0x04002340 RID: 9024
	public Sound soundUnlockEnd;

	// Token: 0x04002341 RID: 9025
	public Sound soundLatches;

	// Token: 0x04002342 RID: 9026
	public Sound soundLatchesEnd;

	// Token: 0x04002343 RID: 9027
	public Sound soundDoorOpen;

	// Token: 0x04002344 RID: 9028
	public Sound soundDoorMove;

	// Token: 0x04002345 RID: 9029
	public Sound soundSlamCeiling;

	// Token: 0x0200040C RID: 1036
	private enum DoorState
	{
		// Token: 0x04002D92 RID: 11666
		Closed,
		// Token: 0x04002D93 RID: 11667
		Success,
		// Token: 0x04002D94 RID: 11668
		Unlock,
		// Token: 0x04002D95 RID: 11669
		Opening,
		// Token: 0x04002D96 RID: 11670
		Open
	}
}
