using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000148 RID: 328
public class ItemBattery : MonoBehaviour
{
	// Token: 0x06000B2B RID: 2859 RVA: 0x00063988 File Offset: 0x00061B88
	private void Awake()
	{
		this.photonView = base.GetComponent<PhotonView>();
		this.itemAttributes = base.GetComponent<ItemAttributes>();
		if (!this.itemAttributes)
		{
			Debug.LogWarning("ItemBattery.cs: No ItemAttributes found on " + base.gameObject.name);
		}
	}

	// Token: 0x06000B2C RID: 2860 RVA: 0x000639D4 File Offset: 0x00061BD4
	private void Start()
	{
		this.batteryVisualLogic = base.GetComponentInChildren<BatteryVisualLogic>();
		if (this.batteryVisualLogic)
		{
			this.batteryVisualLogic.itemBattery = this;
		}
		this.itemEquippable = base.GetComponent<ItemEquippable>();
		this.mainCamera = Camera.main;
		this.batteryLifeCountBars = this.batteryBars;
		this.batteryLifeCountBarsPrev = this.batteryBars;
		this.batteryLifeInt = this.batteryBars;
		this.batteryLife = 100f;
		this.physGrabObject = base.GetComponentInChildren<PhysGrabObject>();
		if (SemiFunc.RunIsLevel() && TutorialDirector.instance.TutorialSettingCheck(DataDirector.Setting.TutorialChargingStation, 1))
		{
			this.tutorialCheck = true;
		}
	}

	// Token: 0x06000B2D RID: 2861 RVA: 0x00063A75 File Offset: 0x00061C75
	private IEnumerator BatteryInit()
	{
		while (!LevelGenerator.Instance.Generated)
		{
			yield return new WaitForSeconds(0.2f);
		}
		while (base.GetComponent<ItemAttributes>().instanceName == null)
		{
			yield return new WaitForSeconds(0.2f);
		}
		if (SemiFunc.RunIsArena())
		{
			StatsManager.instance.SetBatteryLevel(this.itemAttributes.instanceName, 100);
		}
		this.batteryLife = (float)StatsManager.instance.GetBatteryLevel(this.itemAttributes.instanceName);
		if (this.batteryLife > 0f)
		{
			this.batteryLifeInt = (int)Mathf.Round(this.batteryLife / (float)(100 / this.batteryBars));
			this.batteryColor = this.itemAttributes.colorPreset.GetColorLight();
			this.batteryColorMedium = this.itemAttributes.colorPreset.GetColorMain();
		}
		else
		{
			this.batteryLife = 0f;
			this.batteryLifeInt = 0;
			this.batteryColor = this.itemAttributes.colorPreset.GetColorLight();
			this.batteryColorMedium = this.itemAttributes.colorPreset.GetColorMain();
		}
		this.BatteryFullPercentChange(this.batteryLifeInt, false);
		yield break;
	}

	// Token: 0x06000B2E RID: 2862 RVA: 0x00063A84 File Offset: 0x00061C84
	public void SetBatteryLife(int _batteryLife)
	{
		if (this.batteryLife > 0f)
		{
			this.batteryLife = (float)_batteryLife;
			this.batteryLifeInt = (int)Mathf.Round(this.batteryLife / (float)(100 / this.batteryBars));
		}
		else
		{
			this.batteryLife = 0f;
			this.batteryLifeInt = 0;
		}
		this.batteryColor = this.itemAttributes.colorPreset.GetColorLight();
		this.batteryColorMedium = this.itemAttributes.colorPreset.GetColorMain();
		this.BatteryFullPercentChange(this.batteryLifeInt, false);
	}

	// Token: 0x06000B2F RID: 2863 RVA: 0x00063B10 File Offset: 0x00061D10
	public void OverrideBatteryShow(float time = 0.1f)
	{
		this.showTimer = time;
	}

	// Token: 0x06000B30 RID: 2864 RVA: 0x00063B19 File Offset: 0x00061D19
	public void ChargeBattery(GameObject chargerObject, float chargeAmount)
	{
		if (!this.chargerList.Contains(chargerObject))
		{
			this.chargerList.Add(chargerObject);
			this.chargeRate += chargeAmount;
		}
		this.chargeTimer = 0.1f;
	}

	// Token: 0x06000B31 RID: 2865 RVA: 0x00063B50 File Offset: 0x00061D50
	private void FixedUpdate()
	{
		if (this.showTimer <= 0f)
		{
			this.showBattery = false;
		}
		if (this.showTimer > 0f)
		{
			this.showTimer -= Time.fixedDeltaTime;
			if (!this.showBattery && this.batteryVisualLogic)
			{
				if (!this.batteryVisualLogic.gameObject.activeSelf)
				{
					this.batteryVisualLogic.gameObject.SetActive(true);
				}
				this.batteryVisualLogic.BatteryBarsSet();
			}
			this.showBattery = true;
		}
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		if (this.chargeTimer > 0f && this.batteryLife < 99f)
		{
			this.batteryLife = Mathf.Clamp(this.batteryLife + this.chargeRate * Time.fixedDeltaTime, 0f, 100f);
			if (!this.isCharging)
			{
				this.BatteryChargeToggle(true);
			}
			this.chargeTimer -= Time.fixedDeltaTime;
		}
		else if (this.chargeRate != 0f)
		{
			this.chargeRate = 0f;
			this.chargeTimer = 0f;
			this.chargerList.Clear();
			this.BatteryChargeToggle(false);
		}
		if (this.drainTimer > 0f && this.batteryLife > 0f)
		{
			this.batteryLife = Mathf.Clamp(this.batteryLife - this.drainRate * Time.fixedDeltaTime, 0f, 100f);
			this.drainTimer -= Time.fixedDeltaTime;
			return;
		}
		if (this.drainRate != 0f)
		{
			this.drainRate = 0f;
			this.drainTimer = 0f;
		}
	}

	// Token: 0x06000B32 RID: 2866 RVA: 0x00063CF4 File Offset: 0x00061EF4
	private void Update()
	{
		if ((this.itemAttributes.shopItem && SemiFunc.IsMasterClientOrSingleplayer()) || RoundDirector.instance.debugInfiniteBattery)
		{
			this.batteryLife = 100f;
		}
		if (this.batteryVisualLogic && SemiFunc.RunIsLobby())
		{
			this.OverrideBatteryShow(0.2f);
			if (this.batteryVisualLogic.currentBars <= this.batteryVisualLogic.batteryBars / 2)
			{
				this.batteryVisualLogic.OverrideChargeNeeded(0.2f);
			}
		}
		if (this.isCharging)
		{
			this.batteryVisualLogic.OverrideBatteryCharge(0.2f);
		}
		if (!SemiFunc.RunIsShop() && PhysGrabber.instance && PhysGrabber.instance.grabbed && PhysGrabber.instance.grabbedPhysGrabObject == this.physGrabObject)
		{
			if (BatteryUI.instance.batteryVisualLogic.itemBattery != this)
			{
				BatteryUI.instance.batteryVisualLogic.itemBattery = this;
				BatteryUI.instance.batteryVisualLogic.BatteryBarsSet();
			}
			BatteryUI.instance.Show();
		}
		this.BatteryLookAt();
		this.BatteryChargingVisuals();
		if (SemiFunc.RunIsLobby() && this.batteryLifeInt < this.batteryBars)
		{
			this.OverrideBatteryShow(0.1f);
		}
		if (this.showBattery && this.batteryVisualLogic && !this.batteryVisualLogic.gameObject.activeSelf)
		{
			this.BatteryUpdateBars(this.batteryLifeInt);
		}
		if (this.tutorialCheck && this.batteryLife <= 0f && SemiFunc.FPSImpulse15() && this.physGrabObject.playerGrabbing.Count > 0)
		{
			using (List<PhysGrabber>.Enumerator enumerator = this.physGrabObject.playerGrabbing.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.isLocal && TutorialDirector.instance.TutorialSettingCheck(DataDirector.Setting.TutorialChargingStation, 1))
					{
						TutorialDirector.instance.ActivateTip("Charging Station", 2f, false);
						this.tutorialCheck = false;
					}
				}
			}
		}
		if (this.batteryActive)
		{
			if (SemiFunc.IsMasterClientOrSingleplayer() && this.autoDrain && !this.itemEquippable.isEquipped)
			{
				this.batteryLife -= this.batteryDrainRate * Time.deltaTime;
			}
			if (this.batteryLifeInt < 1)
			{
				if (this.batteryLifeInt == 1)
				{
					this.batteryOutBlinkTimer += Time.deltaTime;
				}
				else
				{
					this.batteryOutBlinkTimer += 5f * Time.deltaTime;
				}
				this.batteryVisualLogic.OverrideBatteryOutWarning(0.2f);
			}
			if (this.batteryVisualLogic && !this.batteryVisualLogic.gameObject.activeSelf && !SemiFunc.RunIsShop())
			{
				this.batteryVisualLogic.gameObject.SetActive(true);
				this.BatteryUpdateBars(this.batteryLifeInt);
			}
		}
		else if (!this.showBattery)
		{
			this.batteryOutBlinkTimer = 0f;
			if (this.batteryVisualLogic && this.batteryVisualLogic.gameObject.activeSelf && !this.isCharging)
			{
				this.batteryVisualLogic.BatteryOutro();
				this.BatteryUpdateBars(this.batteryLifeInt);
			}
		}
		if (GameManager.instance.gameMode == 0 || (GameManager.instance.gameMode == 1 && PhotonNetwork.IsMasterClient))
		{
			this.batteryLifeCountBars = (int)Mathf.Round(this.batteryLife / (float)(100 / this.batteryBars));
			if (this.batteryLifeCountBars != this.batteryLifeCountBarsPrev)
			{
				bool charge = false;
				if (this.batteryLifeCountBarsPrev < this.batteryLifeCountBars)
				{
					charge = true;
				}
				this.BatteryFullPercentChange(this.batteryLifeCountBars, charge);
				this.batteryLifeCountBarsPrev = this.batteryLifeCountBars;
			}
		}
	}

	// Token: 0x06000B33 RID: 2867 RVA: 0x000640A8 File Offset: 0x000622A8
	public void RemoveFullBar(int _bars)
	{
		if (SemiFunc.RunIsShop())
		{
			return;
		}
		if (this.batteryLifeInt > 0)
		{
			this.batteryLifeInt -= _bars;
			if (this.batteryLifeInt <= 0)
			{
				this.batteryLifeInt = 0;
				this.batteryLife = 0f;
			}
			else
			{
				this.batteryLife = (float)(this.batteryLifeInt * (100 / this.batteryBars));
			}
			this.BatteryFullPercentChange(this.batteryLifeInt, false);
		}
	}

	// Token: 0x06000B34 RID: 2868 RVA: 0x00064115 File Offset: 0x00062315
	public void BatteryToggle(bool toggle)
	{
		if (SemiFunc.IsMultiplayer())
		{
			if (SemiFunc.IsMasterClientOrSingleplayer())
			{
				this.photonView.RPC("BatteryToggleRPC", RpcTarget.All, new object[]
				{
					toggle
				});
				return;
			}
		}
		else
		{
			this.BatteryToggleRPC(toggle);
		}
	}

	// Token: 0x06000B35 RID: 2869 RVA: 0x0006414D File Offset: 0x0006234D
	[PunRPC]
	public void BatteryToggleRPC(bool toggle)
	{
		this.batteryActive = toggle;
	}

	// Token: 0x06000B36 RID: 2870 RVA: 0x00064158 File Offset: 0x00062358
	private void BatteryLookAt()
	{
		if (!this.batteryVisualLogic.cameraTurn)
		{
			return;
		}
		if (!this.showBattery && !this.batteryActive && !this.isCharging)
		{
			return;
		}
		this.batteryTransform.LookAt(this.mainCamera.transform);
		float d = Vector3.Distance(this.batteryTransform.position, this.mainCamera.transform.position);
		this.batteryTransform.localScale = Vector3.one * d * 0.8f;
		if (this.batteryTransform.localScale.x > 3f)
		{
			this.batteryTransform.localScale = Vector3.one * 3f;
		}
		this.batteryTransform.Rotate(0f, 180f, 0f);
		this.batteryTransform.position = base.transform.position + Vector3.up * this.upOffset;
	}

	// Token: 0x06000B37 RID: 2871 RVA: 0x0006425C File Offset: 0x0006245C
	private void BatteryChargingVisuals()
	{
		if (this.isCharging)
		{
			if (!this.batteryVisualLogic.gameObject.activeSelf)
			{
				this.batteryVisualLogic.gameObject.SetActive(true);
			}
			this.chargingBlinkTimer += Time.deltaTime;
			if (this.chargingBlinkTimer > 0.5f)
			{
				this.chargingBlink = !this.chargingBlink;
				if (this.chargingBlink)
				{
					this.BatteryUpdateBars(this.batteryLifeInt + 1);
				}
				else
				{
					this.BatteryUpdateBars(this.batteryLifeInt);
				}
				this.chargingBlinkTimer = 0f;
			}
		}
	}

	// Token: 0x06000B38 RID: 2872 RVA: 0x000642F4 File Offset: 0x000624F4
	private void BatteryChargeToggle(bool toggle)
	{
		if (SemiFunc.IsMultiplayer())
		{
			if (SemiFunc.IsMasterClientOrSingleplayer())
			{
				this.photonView.RPC("BatteryChargeStartRPC", RpcTarget.All, new object[]
				{
					toggle
				});
				return;
			}
		}
		else
		{
			this.BatteryChargeStartRPC(toggle);
		}
	}

	// Token: 0x06000B39 RID: 2873 RVA: 0x0006432C File Offset: 0x0006252C
	[PunRPC]
	private void BatteryChargeStartRPC(bool toggle)
	{
		this.isCharging = toggle;
		this.BatteryUpdateBars(this.batteryLifeInt);
	}

	// Token: 0x06000B3A RID: 2874 RVA: 0x00064341 File Offset: 0x00062541
	private void BatteryUpdateBars(int batteryLevel)
	{
		if (this.batteryVisualLogic)
		{
			this.currentBars = this.batteryLifeInt;
			this.batteryVisualLogic.BatteryBarsUpdate(-1, false);
		}
	}

	// Token: 0x06000B3B RID: 2875 RVA: 0x0006436C File Offset: 0x0006256C
	private void BatteryFullPercentChangeLogic(int batteryLevel, bool charge)
	{
		if (this.batteryLifeInt > batteryLevel && batteryLevel == 1 && this.batteryActive)
		{
			AssetManager.instance.batteryLowWarning.Play(base.transform.position, 1f, 1f, 1f, 1f);
		}
		this.batteryLifeInt = batteryLevel;
		if (this.batteryLifeInt != 0)
		{
			this.batteryLife = (float)(this.batteryLifeInt * (100 / this.batteryBars));
		}
		else
		{
			this.batteryLife = 0f;
		}
		SemiFunc.StatSetBattery(this.itemAttributes.instanceName, (int)this.batteryLife);
		this.BatteryUpdateBars(this.batteryLifeInt);
		if (this.batteryActive || charge)
		{
			if (charge)
			{
				AssetManager.instance.batteryChargeSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
				return;
			}
			AssetManager.instance.batteryDrainSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
		}
	}

	// Token: 0x06000B3C RID: 2876 RVA: 0x0006447F File Offset: 0x0006267F
	private void OnDisable()
	{
	}

	// Token: 0x06000B3D RID: 2877 RVA: 0x00064481 File Offset: 0x00062681
	private void OnEnable()
	{
	}

	// Token: 0x06000B3E RID: 2878 RVA: 0x00064484 File Offset: 0x00062684
	private void BatteryFullPercentChange(int batteryLifeInt, bool charge = false)
	{
		if (GameManager.instance.gameMode == 0)
		{
			this.BatteryFullPercentChangeLogic(batteryLifeInt, charge);
			return;
		}
		if (PhotonNetwork.IsMasterClient)
		{
			this.photonView.RPC("BatteryFullPercentChangeRPC", RpcTarget.All, new object[]
			{
				batteryLifeInt,
				charge
			});
		}
	}

	// Token: 0x06000B3F RID: 2879 RVA: 0x000644D6 File Offset: 0x000626D6
	[PunRPC]
	private void BatteryFullPercentChangeRPC(int batteryLifeInt, bool charge)
	{
		this.BatteryFullPercentChangeLogic(batteryLifeInt, charge);
	}

	// Token: 0x06000B40 RID: 2880 RVA: 0x000644E0 File Offset: 0x000626E0
	public void Drain(float amount)
	{
		this.drainRate = amount;
		this.drainTimer = 0.1f;
	}

	// Token: 0x0400120C RID: 4620
	public int batteryBars = 6;

	// Token: 0x0400120D RID: 4621
	public bool isUnchargable;

	// Token: 0x0400120E RID: 4622
	public Transform batteryTransform;

	// Token: 0x0400120F RID: 4623
	private Camera mainCamera;

	// Token: 0x04001210 RID: 4624
	public float upOffset = 0.5f;

	// Token: 0x04001211 RID: 4625
	[HideInInspector]
	public bool batteryActive;

	// Token: 0x04001212 RID: 4626
	[HideInInspector]
	public float batteryLife = 100f;

	// Token: 0x04001213 RID: 4627
	internal int batteryLifeInt = 6;

	// Token: 0x04001214 RID: 4628
	private float batteryOutBlinkTimer;

	// Token: 0x04001215 RID: 4629
	private PhotonView photonView;

	// Token: 0x04001216 RID: 4630
	[HideInInspector]
	public Color batteryColor;

	// Token: 0x04001217 RID: 4631
	internal Color batteryColorMedium;

	// Token: 0x04001218 RID: 4632
	private float chargeTimer;

	// Token: 0x04001219 RID: 4633
	private float chargeRate;

	// Token: 0x0400121A RID: 4634
	private List<GameObject> chargerList = new List<GameObject>();

	// Token: 0x0400121B RID: 4635
	internal bool isCharging;

	// Token: 0x0400121C RID: 4636
	private float chargingBlinkTimer;

	// Token: 0x0400121D RID: 4637
	private bool chargingBlink;

	// Token: 0x0400121E RID: 4638
	private ItemAttributes itemAttributes;

	// Token: 0x0400121F RID: 4639
	private float showTimer;

	// Token: 0x04001220 RID: 4640
	private bool showBattery;

	// Token: 0x04001221 RID: 4641
	public bool autoDrain = true;

	// Token: 0x04001222 RID: 4642
	private ItemEquippable itemEquippable;

	// Token: 0x04001223 RID: 4643
	public bool onlyShowWhenItemToggleIsOn;

	// Token: 0x04001224 RID: 4644
	public float batteryDrainRate = 1f;

	// Token: 0x04001225 RID: 4645
	private float drainRate;

	// Token: 0x04001226 RID: 4646
	private float drainTimer;

	// Token: 0x04001227 RID: 4647
	internal int currentBars = 6;

	// Token: 0x04001228 RID: 4648
	private int batteryLifeCountBars = 6;

	// Token: 0x04001229 RID: 4649
	internal int batteryLifeCountBarsPrev = 6;

	// Token: 0x0400122A RID: 4650
	private BatteryVisualLogic batteryVisualLogic;

	// Token: 0x0400122B RID: 4651
	private bool tutorialCheck;

	// Token: 0x0400122C RID: 4652
	private PhysGrabObject physGrabObject;
}
