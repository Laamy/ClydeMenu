using System;
using System.Collections;
using UnityEngine;

// Token: 0x020000F5 RID: 245
public class TruckDoor : MonoBehaviour
{
	// Token: 0x060008B4 RID: 2228 RVA: 0x00054088 File Offset: 0x00052288
	private void Start()
	{
		this.playerInTruckCheckTimer = 2f;
		this.startYPosition = base.transform.position.y;
		base.StartCoroutine(this.DelayedStart());
	}

	// Token: 0x060008B5 RID: 2229 RVA: 0x000540B8 File Offset: 0x000522B8
	private IEnumerator DelayedStart()
	{
		while (!SemiFunc.LevelGenDone())
		{
			yield return new WaitForSeconds(0.3f);
		}
		while (!this.extractionPointNearest)
		{
			yield return new WaitForSeconds(0.1f);
			this.extractionPointNearest = SemiFunc.ExtractionPointGetNearest(base.transform.position);
		}
		this.timeToCheck = true;
		yield break;
	}

	// Token: 0x060008B6 RID: 2230 RVA: 0x000540C8 File Offset: 0x000522C8
	private void Update()
	{
		if (this.timeToCheck)
		{
			if (this.playerInTruckCheckTimer > 0f)
			{
				this.playerInTruckCheckTimer -= Time.deltaTime;
			}
			else
			{
				this.playerInTruckCheckTimer = 0.5f;
				if (!this.introActivationDone && !SemiFunc.PlayersAllInTruck())
				{
					this.introActivationDone = true;
					if (!TutorialDirector.instance.tutorialActive)
					{
						this.extractionPointNearest.ActivateTheFirstExtractionPointAutomaticallyWhenAPlayerLeaveTruck();
					}
				}
			}
		}
		if (this.doorDelay > 0f && SemiFunc.LevelGenDone())
		{
			this.doorDelay -= Time.deltaTime;
		}
		if (this.doorDelay <= 0f && this.doorEval < 1f)
		{
			if (!this.doorOpen)
			{
				this.doorOpen = true;
				if (SemiFunc.RunIsShop())
				{
					SemiFunc.UIFocusText("Buy stuff in the shop", Color.white, AssetManager.instance.colorYellow, 3f);
				}
				GameDirector.instance.CameraImpact.ShakeDistance(3f, 3f, 8f, base.transform.position, 0.1f);
				this.doorLoopStart.Play(base.transform.position, 1f, 1f, 1f, 1f);
			}
			float num = this.doorCurve.Evaluate(this.doorEval);
			this.doorEval += 1.5f * Time.deltaTime;
			base.transform.position = new Vector3(base.transform.position.x, this.startYPosition + 2.5f * num, base.transform.position.z);
		}
		if (this.doorEval >= 1f && !this.fullyOpen)
		{
			this.fullyOpen = true;
			GameDirector.instance.CameraImpact.ShakeDistance(5f, 3f, 8f, base.transform.position, 0.1f);
			this.doorLoopEnd.Play(base.transform.position, 1f, 1f, 1f, 1f);
			this.doorSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
		}
	}

	// Token: 0x04000FD8 RID: 4056
	public Sound doorLoopStart;

	// Token: 0x04000FD9 RID: 4057
	public Sound doorLoopEnd;

	// Token: 0x04000FDA RID: 4058
	public Sound doorSound;

	// Token: 0x04000FDB RID: 4059
	private float startYPosition;

	// Token: 0x04000FDC RID: 4060
	private bool fullyOpen;

	// Token: 0x04000FDD RID: 4061
	private float doorEval;

	// Token: 0x04000FDE RID: 4062
	public AnimationCurve doorCurve;

	// Token: 0x04000FDF RID: 4063
	public Transform doorMesh;

	// Token: 0x04000FE0 RID: 4064
	private float doorDelay = 2f;

	// Token: 0x04000FE1 RID: 4065
	private bool doorOpen;

	// Token: 0x04000FE2 RID: 4066
	private ExtractionPoint extractionPointNearest;

	// Token: 0x04000FE3 RID: 4067
	private float playerInTruckCheckTimer;

	// Token: 0x04000FE4 RID: 4068
	private bool timeToCheck;

	// Token: 0x04000FE5 RID: 4069
	private bool introActivationDone;
}
