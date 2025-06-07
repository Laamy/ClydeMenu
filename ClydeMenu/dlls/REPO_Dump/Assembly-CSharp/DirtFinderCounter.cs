using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// Token: 0x0200018A RID: 394
public class DirtFinderCounter : MonoBehaviour
{
	// Token: 0x06000D75 RID: 3445 RVA: 0x000760DC File Offset: 0x000742DC
	private void Start()
	{
		this.PlayerAmount = GameDirector.instance.PlayerList.Count;
		this.UpdateNumbers();
		if (GameManager.Multiplayer() && this.Controller && this.Controller.photonView && !this.Controller.photonView.IsMine)
		{
			this.SoundDown.SpatialBlend = 1f;
			this.SoundUp.SpatialBlend = 1f;
		}
	}

	// Token: 0x06000D76 RID: 3446 RVA: 0x0007615C File Offset: 0x0007435C
	private void OnEnable()
	{
		if (this.Controller.PlayerAvatar.upgradeMapPlayerCount > 0)
		{
			this.HatchObject.SetActive(false);
			this.HatchLightObject.SetActive(true);
			this.active = true;
			return;
		}
		this.HatchObject.SetActive(true);
		this.HatchLightObject.SetActive(false);
		this.active = false;
	}

	// Token: 0x06000D77 RID: 3447 RVA: 0x000761BB File Offset: 0x000743BB
	private void OnDisable()
	{
		this.PitchUpdated = 0;
		this.UpdateTimer = 0.8f;
	}

	// Token: 0x06000D78 RID: 3448 RVA: 0x000761D0 File Offset: 0x000743D0
	private void Update()
	{
		if (!this.active)
		{
			return;
		}
		if (this.UpdateTimer <= 0f)
		{
			int num = 0;
			using (List<PlayerAvatar>.Enumerator enumerator = GameDirector.instance.PlayerList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (!enumerator.Current.isDisabled)
					{
						num++;
					}
				}
			}
			if (this.PlayerAmount != num)
			{
				float pitch = Mathf.Clamp(1f + (float)this.PitchUpdated * this.PitchIncrease, 1f, this.PitchMax);
				this.PitchUpdated++;
				if (this.PlayerAmount > num)
				{
					this.SoundDown.Pitch = pitch;
					this.SoundDown.Play(base.transform.position, 1f, 1f, 1f, 1f);
					this.PlayerAmount--;
				}
				else
				{
					this.SoundUp.Pitch = pitch;
					this.SoundUp.Play(base.transform.position, 1f, 1f, 1f, 1f);
					this.PlayerAmount++;
				}
				this.UpdateNumbers();
				this.UpdateTimer = this.UpdateTime - this.UpdateTimerDecrease;
				this.UpdateTimer = Mathf.Clamp(this.UpdateTimer, this.UpdateTimeMin, this.UpdateTime);
				this.UpdateTimerDecrease += this.UpdateTimeDecrease;
			}
			else
			{
				this.UpdateTimerDecrease = 0f;
			}
		}
		else
		{
			this.UpdateTimer -= Time.deltaTime;
		}
		if (this.NumberCurveLerp < 1f)
		{
			this.NumberCurveLerp += Time.deltaTime * this.NumberCurveSpeed;
			this.NumberCurveLerp = Mathf.Clamp01(this.NumberCurveLerp);
		}
		this.NumberText.transform.localPosition = new Vector3(this.NumberText.transform.localPosition.x, this.NumberCurve.Evaluate(this.NumberCurveLerp) * this.NumberCurveAmount, this.NumberText.transform.localPosition.z);
	}

	// Token: 0x06000D79 RID: 3449 RVA: 0x00076414 File Offset: 0x00074614
	private void UpdateNumbers()
	{
		string text = this.NumberText.text;
		this.NumberText.text = this.PlayerAmount.ToString();
		if (text != this.NumberText.text)
		{
			this.NumberCurveLerp = 0f;
		}
	}

	// Token: 0x04001589 RID: 5513
	public MapToolController Controller;

	// Token: 0x0400158A RID: 5514
	[Space]
	public TextMeshPro NumberText;

	// Token: 0x0400158B RID: 5515
	[Space]
	public GameObject HatchObject;

	// Token: 0x0400158C RID: 5516
	public GameObject HatchLightObject;

	// Token: 0x0400158D RID: 5517
	[Space]
	public float UpdateTime;

	// Token: 0x0400158E RID: 5518
	public float UpdateTimeMin;

	// Token: 0x0400158F RID: 5519
	public float UpdateTimeDecrease;

	// Token: 0x04001590 RID: 5520
	private float UpdateTimer;

	// Token: 0x04001591 RID: 5521
	private float UpdateTimerDecrease;

	// Token: 0x04001592 RID: 5522
	[Space]
	public float NumberCurveAmount;

	// Token: 0x04001593 RID: 5523
	public float NumberCurveSpeed;

	// Token: 0x04001594 RID: 5524
	public AnimationCurve NumberCurve;

	// Token: 0x04001595 RID: 5525
	private float NumberCurveLerp = 1f;

	// Token: 0x04001596 RID: 5526
	[Space]
	public Sound SoundDown;

	// Token: 0x04001597 RID: 5527
	public Sound SoundUp;

	// Token: 0x04001598 RID: 5528
	private int PitchUpdated;

	// Token: 0x04001599 RID: 5529
	private int PlayerAmount;

	// Token: 0x0400159A RID: 5530
	[Space]
	public float PitchIncrease = 0.1f;

	// Token: 0x0400159B RID: 5531
	public float PitchMax = 3f;

	// Token: 0x0400159C RID: 5532
	private bool active;
}
