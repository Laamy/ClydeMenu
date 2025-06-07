using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200016E RID: 366
public class ItemOrb : MonoBehaviour
{
	// Token: 0x06000C5B RID: 3163 RVA: 0x0006D6CC File Offset: 0x0006B8CC
	private void Start()
	{
		this.customTargetingCondition = base.GetComponent<ITargetingCondition>();
		this.itemBattery = base.GetComponent<ItemBattery>();
		this.itemEquippable = base.GetComponent<ItemEquippable>();
		this.itemAttributes = base.GetComponent<ItemAttributes>();
		this.emojiIcon = this.itemAttributes.emojiIcon;
		this.colorPresets = this.itemAttributes.colorPreset;
		this.orbColor = this.colorPresets.GetColorMain();
		this.orbColorLight = this.colorPresets.GetColorLight();
		this.batteryColor = this.orbColorLight;
		this.itemBattery.batteryColor = this.batteryColor;
		this.batteryDrainRate = this.batteryDrainPreset.batteryDrainRate;
		this.itemBattery.batteryDrainRate = this.batteryDrainRate;
		this.itemEquippable.itemEmoji = this.emojiIcon.ToString();
		ItemLight component = base.GetComponent<ItemLight>();
		if (component)
		{
			component.itemLight.color = this.orbColor;
		}
		Transform transform = base.transform.Find("Item Orb Mesh/Top/Piece1/Orb Icon");
		if (transform)
		{
			transform.GetComponent<Renderer>().material.SetTexture("_EmissionMap", this.orbIcon);
			transform.GetComponent<Renderer>().material.SetColor("_EmissionColor", this.orbColor);
		}
		Transform transform2 = null;
		foreach (object obj in base.transform)
		{
			Transform transform3 = (Transform)obj;
			if (transform3.name == "Item Orb Mesh")
			{
				transform2 = transform3;
			}
		}
		if (transform2 == null)
		{
			Debug.LogWarning("Item Orb Mesh not found in" + base.gameObject.name);
		}
		foreach (object obj2 in transform2)
		{
			Transform transform4 = (Transform)obj2;
			foreach (object obj3 in transform4)
			{
				Transform transform5 = (Transform)obj3;
				if (transform5.name.Contains("Piece"))
				{
					this.spherePieces.Add(transform5);
					transform5.GetComponent<Renderer>().material.SetColor("_EmissionColor", this.orbColor);
				}
			}
			if (transform4.name.Contains("Core"))
			{
				this.sphereCore = transform4;
				transform4.GetComponent<Renderer>().material.SetColor("_EmissionColor", this.orbColorLight);
			}
		}
		this.sphereEffectTransform = base.transform.Find("sphere effect");
		Material material = base.transform.Find("sphere effect/AreaEffect/effect").GetComponent<Renderer>().material;
		Material material2 = base.transform.Find("sphere effect/AreaEffect/outline_inside").GetComponent<Renderer>().material;
		Material material3 = base.transform.Find("sphere effect/AreaEffect/outline").GetComponent<Renderer>().material;
		Color color = this.orbColorLight;
		color = new Color(color.r, color.g, color.b, 0.5f);
		Color value = new Color(this.orbColor.r, this.orbColor.g, this.orbColor.b, 0.1f);
		if (material)
		{
			material.SetColor("_Color", value);
		}
		if (material2)
		{
			material2.SetColor("_EdgeColor", color);
		}
		if (material3)
		{
			material3.SetColor("_EdgeColor", color);
		}
		this.itemToggle = base.GetComponent<ItemToggle>();
		this.physGrabObject = base.GetComponent<PhysGrabObject>();
		this.sphereEffectTransform.transform.localScale = new Vector3(0f, 0f, 0f);
		this.sphereEffectTransform.gameObject.SetActive(false);
		this.orbRadiusOriginal = this.orbRadius;
		this.physGrabObject.clientNonKinematic = true;
	}

	// Token: 0x06000C5C RID: 3164 RVA: 0x0006DB08 File Offset: 0x0006BD08
	private void Update()
	{
		if (!SemiFunc.RunIsLevel() && !SemiFunc.RunIsLobby() && !SemiFunc.RunIsShop() && !SemiFunc.RunIsArena() && !SemiFunc.RunIsTutorial())
		{
			return;
		}
		this.soundOrbLoop.PlayLoop(this.itemActive, 0.5f, 0.5f, 1f);
		if (!this.itemActive)
		{
			this.onNoBatteryTimer = 0f;
		}
		if (this.orbType == ItemOrb.OrbType.Constant)
		{
			this.OrbConstantLogic();
		}
		if (this.orbType == ItemOrb.OrbType.Pulse)
		{
			this.OrbPulseLogic();
		}
		bool flag = this.itemActive;
		this.itemActive = this.itemToggle.toggleState;
		this.orbRadius = this.orbRadiusOriginal * this.orbRadiusMultiplier;
		if (flag != this.itemActive)
		{
			this.SphereAnimatePiecesBack();
			this.itemBattery.batteryActive = this.itemActive;
			if (this.itemActive)
			{
				this.soundOrbBoot.Play(base.transform.position, 1f, 1f, 1f, 1f);
			}
			else
			{
				this.soundOrbShutdown.Play(base.transform.position, 1f, 1f, 1f, 1f);
			}
		}
		if (this.itemActive)
		{
			this.sphereEffectTransform.gameObject.SetActive(true);
			if (this.itemBattery.batteryLife > 0f)
			{
				this.OrbAnimateAppear();
			}
			this.SphereAnimatePieces();
			if (this.itemBattery.batteryLife <= 0f)
			{
				this.onNoBatteryTimer += Time.deltaTime;
				if (this.onNoBatteryTimer >= 1.5f)
				{
					this.itemToggle.ToggleItem(false, -1);
					this.onNoBatteryTimer = 0f;
				}
			}
		}
		else
		{
			this.OrbAnimateDisappear();
		}
		if (this.sphereEffectTransform.gameObject.activeSelf)
		{
			this.sphereEffectTransform.rotation = Quaternion.identity;
		}
	}

	// Token: 0x06000C5D RID: 3165 RVA: 0x0006DCDC File Offset: 0x0006BEDC
	private void SphereAnimatePieces()
	{
		int num = 0;
		foreach (Transform transform in this.spherePieces)
		{
			float num2 = Mathf.Sin(Time.time * 50f + (float)num) * 0.1f;
			transform.localScale = new Vector3(1f + num2, 1f + num2, 1f + num2);
			num++;
		}
		float num3 = Mathf.Sin(Time.time * 30f) * 0.2f;
		this.sphereCore.localScale = new Vector3(1f + num3, 1f + num3, 1f + num3);
	}

	// Token: 0x06000C5E RID: 3166 RVA: 0x0006DDA4 File Offset: 0x0006BFA4
	private void SphereAnimatePiecesBack()
	{
		foreach (Transform transform in this.spherePieces)
		{
			transform.localScale = new Vector3(1f, 1f, 1f);
		}
	}

	// Token: 0x06000C5F RID: 3167 RVA: 0x0006DE08 File Offset: 0x0006C008
	private void OrbConstantLogic()
	{
		if (this.itemBattery.batteryLifeInt == 0)
		{
			this.objectAffected.Clear();
			this.localPlayerAffected = false;
			return;
		}
		if (!this.itemActive)
		{
			return;
		}
		this.sphereCheckTimer += Time.deltaTime;
		if (this.sphereCheckTimer > 0.1f)
		{
			this.objectAffected.Clear();
			this.sphereCheckTimer = 0f;
			if (this.itemBattery.batteryLife <= 0f)
			{
				return;
			}
			if (this.targetEnemies || this.targetNonValuables || this.targetValuables)
			{
				this.objectAffected = SemiFunc.PhysGrabObjectGetAllWithinRange(this.orbRadius, base.transform.position, false, default(LayerMask), null);
				if (!this.targetEnemies || !this.targetNonValuables || !this.targetValuables)
				{
					List<PhysGrabObject> list = new List<PhysGrabObject>();
					foreach (PhysGrabObject physGrabObject in this.objectAffected)
					{
						bool flag = this.customTargetingCondition != null && this.customTargetingCondition.CustomTargetingCondition(physGrabObject.gameObject);
						if (this.customTargetingCondition == null)
						{
							flag = true;
						}
						if (this.targetEnemies && physGrabObject.isEnemy && flag)
						{
							list.Add(physGrabObject);
						}
						if (this.targetNonValuables && physGrabObject.isNonValuable && flag)
						{
							list.Add(physGrabObject);
						}
						if (this.targetValuables && physGrabObject.isValuable && flag)
						{
							list.Add(physGrabObject);
						}
					}
					this.objectAffected.Clear();
					this.objectAffected = list;
				}
			}
			if (this.targetPlayers)
			{
				this.localPlayerAffected = SemiFunc.LocalPlayerOverlapCheck(this.orbRadius, base.transform.position, false);
			}
		}
	}

	// Token: 0x06000C60 RID: 3168 RVA: 0x0006DFF4 File Offset: 0x0006C1F4
	private void OrbPulseLogic()
	{
	}

	// Token: 0x06000C61 RID: 3169 RVA: 0x0006DFF8 File Offset: 0x0006C1F8
	private void OrbAnimateAppear()
	{
		float num = Mathf.Lerp(0f, this.orbRadius, this.sphereEffectScaleLerp);
		this.sphereEffectTransform.localScale = new Vector3(num, num, num);
		if (this.sphereEffectScaleLerp < 1f)
		{
			this.sphereEffectScaleLerp += 10f * Time.deltaTime;
			return;
		}
		this.sphereEffectScaleLerp = 1f;
	}

	// Token: 0x06000C62 RID: 3170 RVA: 0x0006E060 File Offset: 0x0006C260
	private void OrbAnimateDisappear()
	{
		if (this.sphereEffectTransform.gameObject.activeSelf)
		{
			float num = Mathf.Lerp(0f, this.orbRadius, this.sphereEffectScaleLerp);
			this.sphereEffectTransform.localScale = new Vector3(num, num, num);
			if (this.sphereEffectScaleLerp > 0f)
			{
				this.sphereEffectScaleLerp -= 10f * Time.deltaTime;
				return;
			}
			this.sphereEffectScaleLerp = 0f;
			this.sphereEffectTransform.gameObject.SetActive(false);
		}
	}

	// Token: 0x04001412 RID: 5138
	[HideInInspector]
	public SemiFunc.emojiIcon emojiIcon;

	// Token: 0x04001413 RID: 5139
	public Texture orbIcon;

	// Token: 0x04001414 RID: 5140
	private Material orbEffect;

	// Token: 0x04001415 RID: 5141
	public float orbRadius = 1f;

	// Token: 0x04001416 RID: 5142
	private float orbRadiusOriginal = 1f;

	// Token: 0x04001417 RID: 5143
	private float orbRadiusMultiplier = 1f;

	// Token: 0x04001418 RID: 5144
	private Transform orbTransform;

	// Token: 0x04001419 RID: 5145
	private Transform orbInnerTransform;

	// Token: 0x0400141A RID: 5146
	private ItemToggle itemToggle;

	// Token: 0x0400141B RID: 5147
	[HideInInspector]
	public float batteryDrainRate = 0.1f;

	// Token: 0x0400141C RID: 5148
	[HideInInspector]
	public bool itemActive;

	// Token: 0x0400141D RID: 5149
	private Transform sphereEffectTransform;

	// Token: 0x0400141E RID: 5150
	private float sphereEffectScaleLerp;

	// Token: 0x0400141F RID: 5151
	private PhysGrabObject physGrabObject;

	// Token: 0x04001420 RID: 5152
	internal List<PhysGrabObject> objectAffected = new List<PhysGrabObject>();

	// Token: 0x04001421 RID: 5153
	internal bool localPlayerAffected;

	// Token: 0x04001422 RID: 5154
	private Transform sphereCheckTransform;

	// Token: 0x04001423 RID: 5155
	private float sphereCheckTimer;

	// Token: 0x04001424 RID: 5156
	private List<Transform> spherePieces = new List<Transform>();

	// Token: 0x04001425 RID: 5157
	private Transform sphereCore;

	// Token: 0x04001426 RID: 5158
	[HideInInspector]
	public ColorPresets colorPresets;

	// Token: 0x04001427 RID: 5159
	public BatteryDrainPresets batteryDrainPreset;

	// Token: 0x04001428 RID: 5160
	[HideInInspector]
	public Color orbColor;

	// Token: 0x04001429 RID: 5161
	private Color orbColorLight;

	// Token: 0x0400142A RID: 5162
	[HideInInspector]
	public Color batteryColor;

	// Token: 0x0400142B RID: 5163
	private ItemBattery itemBattery;

	// Token: 0x0400142C RID: 5164
	private float onNoBatteryTimer;

	// Token: 0x0400142D RID: 5165
	private ItemEquippable itemEquippable;

	// Token: 0x0400142E RID: 5166
	private ItemAttributes itemAttributes;

	// Token: 0x0400142F RID: 5167
	public Sound soundOrbBoot;

	// Token: 0x04001430 RID: 5168
	public Sound soundOrbShutdown;

	// Token: 0x04001431 RID: 5169
	public Sound soundOrbLoop;

	// Token: 0x04001432 RID: 5170
	public ItemOrb.OrbType orbType;

	// Token: 0x04001433 RID: 5171
	public bool targetValuables = true;

	// Token: 0x04001434 RID: 5172
	public bool targetPlayers = true;

	// Token: 0x04001435 RID: 5173
	public bool targetEnemies = true;

	// Token: 0x04001436 RID: 5174
	public bool targetNonValuables = true;

	// Token: 0x04001437 RID: 5175
	private ITargetingCondition customTargetingCondition;

	// Token: 0x0200038B RID: 907
	public enum OrbType
	{
		// Token: 0x04002B75 RID: 11125
		Constant,
		// Token: 0x04002B76 RID: 11126
		Pulse
	}
}
