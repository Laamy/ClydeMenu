using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000260 RID: 608
public class BatteryVisualLogic : MonoBehaviour
{
	// Token: 0x06001373 RID: 4979 RVA: 0x000AD7FC File Offset: 0x000AB9FC
	private void Awake()
	{
		this.targetPosition = base.transform.localPosition;
		this.targetScale = base.transform.localScale.x;
		this.targetScaleOriginal = this.targetScale;
		this.targetRotationOriginal = this.targetRotation;
		this.targetPositionOriginal = this.targetPosition;
		if (!this.inUI)
		{
			base.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
			base.StartCoroutine(this.VisibleForCamerasAgain());
		}
	}

	// Token: 0x06001374 RID: 4980 RVA: 0x000AD880 File Offset: 0x000ABA80
	private void Start()
	{
		this.springScale = new SpringFloat();
		this.springScale.damping = 0.4f;
		this.springScale.speed = 30f;
		this.springRotation = new SpringFloat();
		this.springRotation.damping = 0.3f;
		this.springRotation.speed = 40f;
		this.springPosition = new SpringVector3();
		this.springPosition.damping = 0.35f;
		this.springPosition.speed = 30f;
		this.springPosition.lastPosition = this.targetPosition;
		this.SetSpacing();
		this.BatteryBarsSet();
		this.batteryBarCharge.gameObject.SetActive(false);
		this.batteryBarDrain.gameObject.SetActive(false);
		this.batteryDrainFullXScale = this.batteryBarDrain.localScale.x;
		this.batteryChargeFullXScale = this.batteryBarCharge.localScale.x;
	}

	// Token: 0x06001375 RID: 4981 RVA: 0x000AD979 File Offset: 0x000ABB79
	public IEnumerator VisibleForCamerasAgain()
	{
		yield return new WaitForSeconds(0.1f);
		base.gameObject.layer = LayerMask.NameToLayer("Default");
		yield break;
	}

	// Token: 0x06001376 RID: 4982 RVA: 0x000AD988 File Offset: 0x000ABB88
	private void SetAllMainColors()
	{
		this.batteryBorderMain.color = this.batteryColorMain;
		this.batteryBackground.color = this.batteryColorBackground;
		this.batteryBorderShadow.color = this.batteryColorShadow;
		this.batteryCharge.color = this.batteryColorCharge;
		this.batteryDrain.color = this.batteryColorDrain;
		foreach (GameObject gameObject in this.bars)
		{
			gameObject.GetComponent<RawImage>().color = this.batteryColorMain;
		}
	}

	// Token: 0x06001377 RID: 4983 RVA: 0x000ADA38 File Offset: 0x000ABC38
	private void BatteryVisualBounce()
	{
		this.springPosition.springVelocity = new Vector3(0f, 500f * base.transform.localScale.x, 0f);
		this.springRotation.springVelocity = 1000f;
	}

	// Token: 0x06001378 RID: 4984 RVA: 0x000ADA85 File Offset: 0x000ABC85
	public void OverrideBatteryOutWarning(float _time)
	{
		this.overrideTimerBatteryOutWarning = _time;
		this.batteryIsOutWarning = true;
	}

	// Token: 0x06001379 RID: 4985 RVA: 0x000ADA95 File Offset: 0x000ABC95
	public void OverrideChargeNeeded(float _time)
	{
		this.overrideTimerChargeNeeded = _time;
		this.batteryChargeNeeded = true;
	}

	// Token: 0x0600137A RID: 4986 RVA: 0x000ADAA8 File Offset: 0x000ABCA8
	public void OverrideBatteryDrain(float _time)
	{
		if (!this.batteryIsDraining)
		{
			this.drainAnimationProgress = 0f;
			if (!this.batteryDrain.gameObject.activeSelf)
			{
				this.batteryDrain.gameObject.SetActive(true);
			}
			this.BatteryVisualBounce();
		}
		this.overrideTimerBatteryDrain = _time;
		this.batteryIsDraining = true;
	}

	// Token: 0x0600137B RID: 4987 RVA: 0x000ADB00 File Offset: 0x000ABD00
	public void OverrideBatteryCharge(float _time)
	{
		if (!this.batteryIsCharging)
		{
			this.chargeAnimationProgress = 0f;
			if (!this.batteryCharge.gameObject.activeSelf)
			{
				this.batteryCharge.gameObject.SetActive(true);
			}
			this.BatteryVisualBounce();
		}
		this.overrideTimerBatteryCharge = _time;
		this.batteryIsCharging = true;
	}

	// Token: 0x0600137C RID: 4988 RVA: 0x000ADB58 File Offset: 0x000ABD58
	private void OverrideBatteryOutWarningTimer()
	{
		if (this.overrideTimerBatteryOutWarning > 0f)
		{
			this.overrideTimerBatteryOutWarning -= Time.deltaTime;
			return;
		}
		if (this.batteryIsOutWarning)
		{
			this.BatteryColorMainReset();
		}
		if (this.batteryOutVisual.activeSelf)
		{
			this.batteryOutVisual.SetActive(false);
		}
		this.batteryIsOutWarning = false;
	}

	// Token: 0x0600137D RID: 4989 RVA: 0x000ADBB4 File Offset: 0x000ABDB4
	private void OverrideChargeNeededTimer()
	{
		if (this.overrideTimerChargeNeeded > 0f)
		{
			this.overrideTimerChargeNeeded -= Time.deltaTime;
			return;
		}
		if (this.batteryChargeNeededVisual.activeSelf)
		{
			this.batteryChargeNeededVisual.SetActive(false);
		}
		this.batteryChargeNeeded = false;
	}

	// Token: 0x0600137E RID: 4990 RVA: 0x000ADC04 File Offset: 0x000ABE04
	private void OverrideBatteryDrainTimer()
	{
		if (this.overrideTimerBatteryDrain > 0f)
		{
			this.overrideTimerBatteryDrain -= Time.deltaTime;
			return;
		}
		if (this.batteryDrain.gameObject.activeSelf)
		{
			this.batteryDrain.gameObject.SetActive(false);
		}
		this.batteryIsDraining = false;
	}

	// Token: 0x0600137F RID: 4991 RVA: 0x000ADC5C File Offset: 0x000ABE5C
	private void OverrideBatteryChargeTimer()
	{
		if (this.overrideTimerBatteryCharge > 0f)
		{
			this.overrideTimerBatteryCharge -= Time.deltaTime;
			return;
		}
		if (this.batteryCharge.gameObject.activeSelf)
		{
			this.batteryCharge.gameObject.SetActive(false);
		}
		this.batteryIsCharging = false;
	}

	// Token: 0x06001380 RID: 4992 RVA: 0x000ADCB3 File Offset: 0x000ABEB3
	private void OverrideTimers()
	{
		this.OverrideBatteryOutWarningTimer();
		this.OverrideBatteryDrainTimer();
		this.OverrideBatteryChargeTimer();
		this.OverrideChargeNeededTimer();
	}

	// Token: 0x06001381 RID: 4993 RVA: 0x000ADCD0 File Offset: 0x000ABED0
	private void BatteryDrainVisuals()
	{
		if (!this.batteryIsDraining)
		{
			return;
		}
		float t = this.drainAnimationProgress;
		this.batteryBarDrain.localScale = new Vector3(Mathf.Lerp(this.batteryDrainFullXScale, 0f, t), this.batteryBarDrain.localScale.y, this.batteryBarDrain.localScale.z);
		if (this.drainAnimationProgress >= 1f)
		{
			this.drainAnimationProgress = 0f;
			this.springPosition.springVelocity = new Vector3(-250f * base.transform.localScale.x, 0f, 0f);
			this.springRotation.springVelocity = -50f;
		}
		this.drainAnimationProgress += Time.deltaTime * 5f;
	}

	// Token: 0x06001382 RID: 4994 RVA: 0x000ADDA0 File Offset: 0x000ABFA0
	private void BatteryChargeVisuals()
	{
		if (!this.batteryIsCharging)
		{
			return;
		}
		float t = this.chargeAnimationProgress;
		this.batteryBarCharge.localScale = new Vector3(Mathf.Lerp(0f, this.batteryChargeFullXScale, t), this.batteryBarCharge.localScale.y, this.batteryBarCharge.localScale.z);
		if (this.chargeAnimationProgress >= 1f)
		{
			this.chargeAnimationProgress = 0f;
			this.springPosition.springVelocity = new Vector3(250f * base.transform.localScale.x, 0f, 0f);
			this.springRotation.springVelocity = 50f;
		}
		this.chargeAnimationProgress += Time.deltaTime * 5f;
	}

	// Token: 0x06001383 RID: 4995 RVA: 0x000ADE70 File Offset: 0x000AC070
	private void BatteryOutWarningVisuals()
	{
		if (!this.batteryIsOutWarning)
		{
			return;
		}
		float t = this.warningAnimationProgress;
		this.batteryBorderMain.color = Color.Lerp(this.batteryColorMain, this.batteryColorWarning, t);
		if (this.warningAnimationProgress > 0.5f && this.batteryOutVisual.activeSelf)
		{
			this.batteryOutVisual.SetActive(false);
		}
		if (this.warningAnimationProgress >= 1f)
		{
			if (!this.batteryOutVisual.activeSelf)
			{
				this.batteryOutVisual.SetActive(true);
			}
			this.warningAnimationProgress = 0f;
		}
		this.warningAnimationProgress += Time.deltaTime * 2f;
	}

	// Token: 0x06001384 RID: 4996 RVA: 0x000ADF1C File Offset: 0x000AC11C
	private void BatteryChargeNeededVisuals()
	{
		if (!this.batteryChargeNeeded)
		{
			return;
		}
		if (this.chargeNeededAnimationProgress > 0.5f && this.batteryChargeNeededVisual.activeSelf)
		{
			this.batteryChargeNeededVisual.SetActive(false);
		}
		if (this.chargeNeededAnimationProgress >= 1f)
		{
			if (!this.batteryChargeNeededVisual.activeSelf)
			{
				this.batteryChargeNeededVisual.SetActive(true);
			}
			this.chargeNeededAnimationProgress = 0f;
		}
		this.chargeNeededAnimationProgress += Time.deltaTime * 0.5f;
	}

	// Token: 0x06001385 RID: 4997 RVA: 0x000ADFA1 File Offset: 0x000AC1A1
	private void BatteryVisuals()
	{
		this.BatteryDrainVisuals();
		this.BatteryChargeVisuals();
		this.BatteryOutWarningVisuals();
		this.BatteryChargeNeededVisuals();
	}

	// Token: 0x06001386 RID: 4998 RVA: 0x000ADFBC File Offset: 0x000AC1BC
	private void Update()
	{
		if (this.itemBatteryPrev != this.itemBattery)
		{
			this.BatteryBarsSet();
			this.itemBatteryPrev = this.itemBattery;
		}
		base.transform.localPosition = SemiFunc.SpringVector3Get(this.springPosition, this.targetPosition, -1f);
		float num = SemiFunc.SpringFloatGet(this.springScale, this.targetScale, -1f);
		base.transform.localScale = new Vector3(num, num, num);
		float z = SemiFunc.SpringFloatGet(this.springRotation, this.targetRotation, -1f);
		base.transform.localRotation = Quaternion.Euler(0f, 0f, z);
		if (this.doOutro && num <= 0f)
		{
			base.gameObject.SetActive(false);
			this.targetScale = this.targetScaleOriginal;
			this.targetRotation = this.targetRotationOriginal;
			this.doOutro = false;
		}
		this.OverrideTimers();
		this.BatteryVisuals();
	}

	// Token: 0x06001387 RID: 4999 RVA: 0x000AE0B4 File Offset: 0x000AC2B4
	private void OnEnable()
	{
		base.transform.localScale = Vector3.zero;
		this.springScale.lastPosition = 0f;
		base.transform.localRotation = Quaternion.Euler(0f, 0f, 45f);
		this.springRotation.lastPosition = -90f;
		this.springPosition.lastPosition = new Vector3(this.targetPosition.x, this.targetPosition.y - 0.5f, this.targetPosition.z);
		base.transform.localPosition = new Vector3(this.targetPosition.x, this.targetPosition.y - 0.5f, this.targetPosition.z);
		this.SetSpacing();
		this.SetBatteryColor();
		this.doOutro = false;
	}

	// Token: 0x06001388 RID: 5000 RVA: 0x000AE191 File Offset: 0x000AC391
	private void SetBatteryColor()
	{
		if (!this.itemBattery)
		{
			return;
		}
		if (this.itemBattery.currentBars > 0)
		{
			this.BatteryColorMainReset();
			return;
		}
		this.BatteryColorMainSet(Color.red);
	}

	// Token: 0x06001389 RID: 5001 RVA: 0x000AE1C4 File Offset: 0x000AC3C4
	public void BatteryBarsSet()
	{
		this.SetAllMainColors();
		int num = 6;
		if (this.itemBattery)
		{
			num = this.itemBattery.batteryBars;
		}
		foreach (GameObject obj in this.bars)
		{
			Object.Destroy(obj);
		}
		this.bars.Clear();
		this.batteryBars = num;
		for (int i = 0; i < num; i++)
		{
			GameObject gameObject = Object.Instantiate<GameObject>(this.batteryBarPrefab, this.batteryBarContainer);
			this.bars.Add(gameObject);
		}
		if (this.itemBattery)
		{
			int num2 = this.itemBattery.currentBars;
			for (int j = 0; j < this.bars.Count; j++)
			{
				if (j >= num2)
				{
					this.bars[j].GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 0f);
				}
			}
			this.SetBatteryColor();
			this.currentBars = num2;
		}
		if (this.itemBattery)
		{
			for (int k = 0; k < this.bars.Count; k++)
			{
				if (k >= this.itemBattery.batteryLifeInt)
				{
					this.bars[k].GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 0f);
				}
				else
				{
					this.bars[k].GetComponent<RawImage>().color = this.batteryColorMain;
				}
			}
		}
		this.SetSpacing();
	}

	// Token: 0x0600138A RID: 5002 RVA: 0x000AE374 File Offset: 0x000AC574
	private void SetSpacing()
	{
		this.batteryBarContainerGroup.spacing = 12f / (float)this.batteryBars;
		if (this.batteryBars <= 8)
		{
			this.batteryBarContainerGroup.spacing = 2f;
		}
		if (this.batteryBars > 8)
		{
			this.batteryBarContainerGroup.spacing = 1f;
		}
	}

	// Token: 0x0600138B RID: 5003 RVA: 0x000AE3CC File Offset: 0x000AC5CC
	public void BatteryBarsUpdate(int _setToBars = -1, bool _forceUpdate = false)
	{
		if (this.itemBattery || _setToBars != -1)
		{
			int num = _setToBars;
			if (this.itemBattery)
			{
				num = this.itemBattery.currentBars;
			}
			if (!this.inUI && !_forceUpdate && BatteryUI.instance.batteryVisualLogic.itemBattery == this.itemBattery)
			{
				BatteryUI.instance.batteryVisualLogic.BatteryBarsUpdate(num, true);
			}
			this.SetBatteryColor();
			this.currentBars = num;
			for (int i = 0; i < this.bars.Count; i++)
			{
				if (i >= num)
				{
					if (this.bars[i].GetComponent<RawImage>().color.a != 0f)
					{
						this.BatteryVisualBounce();
						Object.Instantiate<GameObject>(this.barLossEffectPrefab, this.bars[i].transform).transform.localPosition = new Vector3(0f, 0f, 0f);
					}
					this.bars[i].GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 0f);
				}
				else
				{
					if (this.bars[i].GetComponent<RawImage>().color.a == 0f)
					{
						this.BatteryVisualBounce();
						GameObject gameObject = Object.Instantiate<GameObject>(this.barLossEffectPrefab, this.bars[i].transform);
						gameObject.transform.localPosition = new Vector3(0f, 0f, 0f);
						BatteryBarEffect component = gameObject.GetComponent<BatteryBarEffect>();
						component.barLossEffect = false;
						component.barColor = Color.green;
					}
					this.bars[i].GetComponent<RawImage>().color = this.batteryColorMain;
				}
			}
		}
	}

	// Token: 0x0600138C RID: 5004 RVA: 0x000AE59C File Offset: 0x000AC79C
	public void BatteryColorMainSet(Color _color)
	{
		this.batteryBorderMain.color = _color;
		foreach (GameObject gameObject in this.bars)
		{
			if (gameObject.GetComponent<RawImage>().color.a > 0f)
			{
				gameObject.GetComponent<RawImage>().color = _color;
			}
		}
	}

	// Token: 0x0600138D RID: 5005 RVA: 0x000AE618 File Offset: 0x000AC818
	public void BatteryColorMainReset()
	{
		this.batteryBorderMain.color = this.itemBattery.batteryColorMedium;
		this.batteryColorMain = this.itemBattery.batteryColor;
		foreach (GameObject gameObject in this.bars)
		{
			if (gameObject.GetComponent<RawImage>().color.a > 0f)
			{
				gameObject.GetComponent<RawImage>().color = this.batteryColorMain;
			}
		}
	}

	// Token: 0x0600138E RID: 5006 RVA: 0x000AE6B4 File Offset: 0x000AC8B4
	public void HideCurrentBar(bool _hide, Color _blinkColor)
	{
		if (this.itemBattery)
		{
			int num = this.itemBattery.currentBars;
			if (num > 0)
			{
				this.bars[num - 1].GetComponent<RawImage>().color = (_hide ? new Color(_blinkColor.r, _blinkColor.g, _blinkColor.b, 0f) : _blinkColor);
				return;
			}
			Color color = this.batteryColorMain;
			this.bars[0].GetComponent<RawImage>().color = (_hide ? new Color(_blinkColor.r, _blinkColor.g, _blinkColor.b, 0f) : _blinkColor);
		}
	}

	// Token: 0x0600138F RID: 5007 RVA: 0x000AE75B File Offset: 0x000AC95B
	public void BatteryOutro()
	{
		if (!this.doOutro)
		{
			this.springScale.springVelocity = 0.1f;
			this.springRotation.springVelocity = -1000f;
			this.doOutro = true;
			this.targetScale = 0f;
		}
	}

	// Token: 0x04002181 RID: 8577
	public HorizontalLayoutGroup batteryBarContainerGroup;

	// Token: 0x04002182 RID: 8578
	public GameObject batteryBarPrefab;

	// Token: 0x04002183 RID: 8579
	public int batteryBars = 3;

	// Token: 0x04002184 RID: 8580
	public Transform batteryBarContainer;

	// Token: 0x04002185 RID: 8581
	public bool inUI;

	// Token: 0x04002186 RID: 8582
	public bool cameraTurn = true;

	// Token: 0x04002187 RID: 8583
	public Transform batteryBarCharge;

	// Token: 0x04002188 RID: 8584
	public Transform batteryBarDrain;

	// Token: 0x04002189 RID: 8585
	internal List<GameObject> bars = new List<GameObject>();

	// Token: 0x0400218A RID: 8586
	internal float batteryPercent = 100f;

	// Token: 0x0400218B RID: 8587
	internal int currentBars = 3;

	// Token: 0x0400218C RID: 8588
	public ItemBattery itemBattery;

	// Token: 0x0400218D RID: 8589
	private ItemBattery itemBatteryPrev;

	// Token: 0x0400218E RID: 8590
	public RawImage batteryBorderShadow;

	// Token: 0x0400218F RID: 8591
	public RawImage batteryBorderMain;

	// Token: 0x04002190 RID: 8592
	public RawImage batteryBackground;

	// Token: 0x04002191 RID: 8593
	public RawImage batteryCharge;

	// Token: 0x04002192 RID: 8594
	public RawImage batteryDrain;

	// Token: 0x04002193 RID: 8595
	public GameObject batteryOutVisual;

	// Token: 0x04002194 RID: 8596
	public GameObject batteryChargeNeededVisual;

	// Token: 0x04002195 RID: 8597
	private float batteryDrainFullXScale = 0.93f;

	// Token: 0x04002196 RID: 8598
	private float batteryChargeFullXScale = 1f;

	// Token: 0x04002197 RID: 8599
	private Color batteryColorMain = new Color(1f, 1f, 0f, 1f);

	// Token: 0x04002198 RID: 8600
	private Color batteryColorBackground = new Color(0.1f, 0.1f, 0.1f, 1f);

	// Token: 0x04002199 RID: 8601
	private Color batteryColorShadow = new Color(0.1f, 0.1f, 0.1f, 0.5f);

	// Token: 0x0400219A RID: 8602
	private Color batteryColorWarning = new Color(1f, 0f, 0f, 1f);

	// Token: 0x0400219B RID: 8603
	private Color batteryColorCharge = new Color(0f, 1f, 0f, 1f);

	// Token: 0x0400219C RID: 8604
	private Color batteryColorDrain = new Color(1f, 0.2f, 0f, 1f);

	// Token: 0x0400219D RID: 8605
	private SpringFloat springScale = new SpringFloat();

	// Token: 0x0400219E RID: 8606
	private SpringFloat springRotation = new SpringFloat();

	// Token: 0x0400219F RID: 8607
	private SpringVector3 springPosition = new SpringVector3();

	// Token: 0x040021A0 RID: 8608
	public GameObject barLossEffectPrefab;

	// Token: 0x040021A1 RID: 8609
	private float targetScale = 1f;

	// Token: 0x040021A2 RID: 8610
	private float targetRotation;

	// Token: 0x040021A3 RID: 8611
	private Vector3 targetPosition = Vector3.zero;

	// Token: 0x040021A4 RID: 8612
	private float targetScaleOriginal = 1f;

	// Token: 0x040021A5 RID: 8613
	private float targetRotationOriginal;

	// Token: 0x040021A6 RID: 8614
	private Vector3 targetPositionOriginal = Vector3.zero;

	// Token: 0x040021A7 RID: 8615
	private float overrideTimerBatteryOutWarning;

	// Token: 0x040021A8 RID: 8616
	private bool batteryIsOutWarning;

	// Token: 0x040021A9 RID: 8617
	private float overrideTimerBatteryDrain;

	// Token: 0x040021AA RID: 8618
	private bool batteryIsDraining;

	// Token: 0x040021AB RID: 8619
	private float overrideTimerBatteryCharge;

	// Token: 0x040021AC RID: 8620
	private bool batteryIsCharging;

	// Token: 0x040021AD RID: 8621
	private float overrideTimerChargeNeeded;

	// Token: 0x040021AE RID: 8622
	private bool batteryChargeNeeded;

	// Token: 0x040021AF RID: 8623
	private float chargeAnimationProgress;

	// Token: 0x040021B0 RID: 8624
	private float drainAnimationProgress;

	// Token: 0x040021B1 RID: 8625
	private float warningAnimationProgress;

	// Token: 0x040021B2 RID: 8626
	private float chargeNeededAnimationProgress;

	// Token: 0x040021B3 RID: 8627
	internal bool doOutro;
}
