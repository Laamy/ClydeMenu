using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000165 RID: 357
public class ItemMine : MonoBehaviour
{
	// Token: 0x06000C29 RID: 3113 RVA: 0x0006B5F0 File Offset: 0x000697F0
	private void Start()
	{
		this.triggerSpringQuaternion = new SpringQuaternion();
		this.triggerSpringQuaternion.damping = 0.2f;
		this.triggerSpringQuaternion.speed = 10f;
		this.itemAttributes = base.GetComponent<ItemAttributes>();
		this.physGrabObject = base.GetComponent<PhysGrabObject>();
		this.photonView = base.GetComponent<PhotonView>();
		this.lightArmed.color = this.emissionColor;
		this.meshRenderer.material.SetColor("_EmissionColor", this.emissionColor);
		this.initialLightIntensity = this.lightArmed.intensity;
		this.impactDetector = base.GetComponent<PhysGrabObjectImpactDetector>();
		this.itemMineTrigger = base.GetComponentInChildren<ItemMineTrigger>();
		this.particleScriptExplosion = base.GetComponent<ParticleScriptExplosion>();
		this.startPosition = base.transform.position;
		this.itemEquippable = base.GetComponent<ItemEquippable>();
		this.startRotation = base.transform.rotation;
		this.itemToggle = base.GetComponent<ItemToggle>();
	}

	// Token: 0x06000C2A RID: 3114 RVA: 0x0006B6E8 File Offset: 0x000698E8
	private void StateDisarmed()
	{
		if (this.stateStart)
		{
			this.soundDisarmedBeep.Play(base.transform.position, 1f, 1f, 1f, 1f);
			this.stateStart = false;
			this.lightArmed.intensity = this.initialLightIntensity * 3f;
			this.meshRenderer.material.SetColor("_EmissionColor", Color.green);
			this.lightArmed.color = Color.green;
			this.beepTimer = 1f;
		}
		if (this.firstLight)
		{
			this.meshRenderer.material.SetColor("_EmissionColor", this.emissionColor);
			this.lightArmed.color = this.emissionColor;
			this.firstLight = false;
		}
		else if (!this.firstLightDone)
		{
			this.meshRenderer.material.SetColor("_EmissionColor", Color.green);
			this.lightArmed.color = Color.green;
			this.firstLightDone = true;
		}
		if (this.lightArmed.intensity > 0f && this.beepTimer > 0f)
		{
			float t = 1f - this.beepTimer;
			this.lightArmed.intensity = Mathf.Lerp(this.lightArmed.intensity, 0f, t);
			Color value = Color.Lerp(this.meshRenderer.material.GetColor("_EmissionColor"), Color.black, t);
			this.meshRenderer.material.SetColor("_EmissionColor", value);
			this.beepTimer -= Time.deltaTime * 0.1f;
		}
		if (SemiFunc.IsMasterClientOrSingleplayer() && this.itemToggle.toggleState)
		{
			this.StateSet(ItemMine.States.Arming);
		}
	}

	// Token: 0x06000C2B RID: 3115 RVA: 0x0006B8AC File Offset: 0x00069AAC
	private void StateArming()
	{
		if (this.stateStart)
		{
			this.stateStart = false;
			this.beepTimer = 1f;
			this.lightArmed.color = this.emissionColor;
			Color color = new Color(1f, 0.5f, 0f);
			this.soundArmingBeep.Play(base.transform.position, 1f, 1f, 1f, 1f);
			this.ColorSet(color);
		}
		this.beepTimer -= Time.deltaTime * 4f;
		if (this.beepTimer <= 0f)
		{
			this.soundArmingBeep.Play(base.transform.position, 1f, 1f, 1f, 1f);
			Color color2 = new Color(1f, 0.5f, 0f);
			this.ColorSet(color2);
			this.beepTimer = 1f;
		}
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			if (!this.physGrabObject.grabbed)
			{
				this.stateTimer += Time.deltaTime;
			}
			else
			{
				this.stateTimer += Time.deltaTime * 0.25f;
			}
			if (this.physGrabObject.grabbed || this.physGrabObject.rb.velocity.magnitude > 1f)
			{
				this.stateTimer = 0f;
			}
			if (this.stateTimer >= this.armingTime)
			{
				this.StateSet(ItemMine.States.Armed);
			}
		}
		if (SemiFunc.IsMasterClientOrSingleplayer() && !this.itemToggle.toggleState)
		{
			this.StateSet(ItemMine.States.Disarming);
		}
	}

	// Token: 0x06000C2C RID: 3116 RVA: 0x0006BA50 File Offset: 0x00069C50
	private void StateArmed()
	{
		if (this.stateStart)
		{
			this.soundArmedBeep.Play(base.transform.position, 1f, 1f, 1f, 1f);
			this.ColorSet(this.emissionColor);
			this.lightArmed.intensity = this.initialLightIntensity * 8f;
			this.stateStart = false;
			this.secondArmedTimer = 2f;
		}
		this.lightArmed.intensity = Mathf.Lerp(this.lightArmed.intensity, this.initialLightIntensity, Time.deltaTime * 4f);
		if (this.secondArmedTimer > 0f)
		{
			this.secondArmedTimer -= Time.deltaTime;
		}
		if (SemiFunc.IsMasterClientOrSingleplayer() && this.secondArmedTimer <= 0f && (this.triggeredByForces && this.physGrabObject.rb.velocity.magnitude > 0.5f))
		{
			this.StateSet(ItemMine.States.Triggering);
		}
		if (SemiFunc.IsMasterClientOrSingleplayer() && !this.itemToggle.toggleState)
		{
			this.StateSet(ItemMine.States.Disarming);
		}
	}

	// Token: 0x06000C2D RID: 3117 RVA: 0x0006BB72 File Offset: 0x00069D72
	private void ColorSet(Color _color)
	{
		this.lightArmed.intensity = this.initialLightIntensity;
		this.lightArmed.color = _color;
		this.meshRenderer.material.SetColor("_EmissionColor", _color);
	}

	// Token: 0x06000C2E RID: 3118 RVA: 0x0006BBA8 File Offset: 0x00069DA8
	private void StateDisarming()
	{
		if (this.stateStart)
		{
			this.stateStart = false;
			this.beepTimer = 1f;
			this.soundDisarmingBeep.Play(base.transform.position, 1f, 1f, 1f, 1f);
			this.ColorSet(this.emissionColor);
			this.beepTimer = 1f;
		}
		this.beepTimer -= Time.deltaTime * 4f;
		if (this.beepTimer <= 0f)
		{
			this.soundDisarmingBeep.Play(base.transform.position, 1f, 1f, 1f, 1f);
			this.ColorSet(Color.green);
			this.beepTimer = 1f;
		}
		this.stateTimer += Time.deltaTime;
		if (this.stateTimer > 0.1f)
		{
			this.StateSet(ItemMine.States.Disarmed);
		}
	}

	// Token: 0x06000C2F RID: 3119 RVA: 0x0006BCA0 File Offset: 0x00069EA0
	private void StateTriggering()
	{
		if (this.stateStart)
		{
			this.stateStart = false;
			this.beepTimer = 1f;
		}
		this.beepTimer -= Time.deltaTime * 4f;
		if (this.beepTimer < 0f)
		{
			this.soundTriggereringBeep.Play(base.transform.position, 1f, 1f, 1f, 1f);
			this.ColorSet(this.emissionColor);
			this.beepTimer = 1f;
		}
		this.stateTimer += Time.deltaTime;
		if (this.stateTimer > this.triggeringTime)
		{
			this.StateSet(ItemMine.States.Triggered);
		}
		if (SemiFunc.IsMasterClientOrSingleplayer() && !this.itemToggle.toggleState)
		{
			this.StateSet(ItemMine.States.Disarming);
		}
	}

	// Token: 0x06000C30 RID: 3120 RVA: 0x0006BD70 File Offset: 0x00069F70
	private void StateTriggered()
	{
		if (this.stateStart)
		{
			this.stateStart = false;
			this.beepTimer = 1f;
			if (!this.destroyAfterTimer)
			{
				this.DestroyMine();
			}
			Color color = new Color(0.5f, 0.9f, 1f);
			this.ColorSet(color);
		}
		this.stateTimer += Time.deltaTime;
		if (this.destroyAfterTimer && this.stateTimer > this.destroyTimer)
		{
			this.DestroyMine();
		}
	}

	// Token: 0x06000C31 RID: 3121 RVA: 0x0006BDF0 File Offset: 0x00069FF0
	public void DestroyMine()
	{
		if (!SemiFunc.RunIsShop())
		{
			if (!this.mineDestroyed)
			{
				StatsManager.instance.ItemRemove(this.itemAttributes.instanceName);
				this.impactDetector.DestroyObject(true);
				this.mineDestroyed = true;
				return;
			}
		}
		else
		{
			this.ResetMine();
			this.physGrabObject.Teleport(this.startPosition, this.startRotation);
		}
	}

	// Token: 0x06000C32 RID: 3122 RVA: 0x0006BE54 File Offset: 0x0006A054
	private void ResetMine()
	{
		this.hasBeenGrabbed = false;
		this.StateSet(ItemMine.States.Disarmed);
		this.itemToggle.ToggleItem(false, -1);
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			this.stateTimer = 0f;
			Rigidbody component = base.GetComponent<Rigidbody>();
			if (!component.isKinematic)
			{
				component.velocity = Vector3.zero;
				component.angularVelocity = Vector3.zero;
			}
		}
	}

	// Token: 0x06000C33 RID: 3123 RVA: 0x0006BEB4 File Offset: 0x0006A0B4
	private void AnimateLight()
	{
		if (this.lightArmed.intensity > 0f && this.beepTimer > 0f)
		{
			float t = 1f - this.beepTimer;
			this.lightArmed.intensity = Mathf.Lerp(this.lightArmed.intensity, 0f, t);
			Color value = Color.Lerp(this.meshRenderer.material.GetColor("_EmissionColor"), Color.black, t);
			this.meshRenderer.material.SetColor("_EmissionColor", value);
		}
	}

	// Token: 0x06000C34 RID: 3124 RVA: 0x0006BF48 File Offset: 0x0006A148
	private void Update()
	{
		this.TriggerRotation();
		this.TriggerLineVisuals();
		this.TriggerScaleFixer();
		this.AnimateLight();
		if (this.physGrabObject.grabbedLocal && !SemiFunc.RunIsShop())
		{
			PhysGrabber.instance.OverrideGrabDistance(1f);
		}
		if (this.physGrabObject.grabbed)
		{
			this.hasBeenGrabbed = true;
		}
		if (this.itemEquippable.isEquipped && SemiFunc.IsMasterClientOrSingleplayer() && this.hasBeenGrabbed)
		{
			this.StateSet(ItemMine.States.Disarmed);
		}
		if (!SemiFunc.RunIsShop())
		{
			if (SemiFunc.IsMasterClientOrSingleplayer() && this.wasGrabbed && !this.physGrabObject.grabbed)
			{
				Rigidbody component = base.GetComponent<Rigidbody>();
				if (!component.isKinematic)
				{
					component.velocity *= 0.15f;
				}
			}
			this.wasGrabbed = this.physGrabObject.grabbed;
		}
		switch (this.state)
		{
		case ItemMine.States.Disarmed:
			this.StateDisarmed();
			return;
		case ItemMine.States.Arming:
			this.StateArming();
			return;
		case ItemMine.States.Armed:
			this.StateArmed();
			return;
		case ItemMine.States.Disarming:
			this.StateDisarming();
			return;
		case ItemMine.States.Triggering:
			this.StateTriggering();
			return;
		case ItemMine.States.Triggered:
			this.StateTriggered();
			return;
		default:
			return;
		}
	}

	// Token: 0x06000C35 RID: 3125 RVA: 0x0006C06F File Offset: 0x0006A26F
	[PunRPC]
	public void TriggeredRPC(PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.MasterOnlyRPC(_info))
		{
			return;
		}
		this.onTriggered.Invoke();
	}

	// Token: 0x06000C36 RID: 3126 RVA: 0x0006C088 File Offset: 0x0006A288
	private void StateSet(ItemMine.States newState)
	{
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		if (newState == this.state)
		{
			return;
		}
		if (newState == ItemMine.States.Triggered)
		{
			if (SemiFunc.IsMultiplayer())
			{
				this.photonView.RPC("TriggeredRPC", RpcTarget.All, Array.Empty<object>());
			}
			else
			{
				this.TriggeredRPC(default(PhotonMessageInfo));
			}
		}
		if (SemiFunc.IsMultiplayer())
		{
			this.photonView.RPC("StateSetRPC", RpcTarget.All, new object[]
			{
				(int)newState
			});
			return;
		}
		this.StateSetRPC((int)newState, default(PhotonMessageInfo));
	}

	// Token: 0x06000C37 RID: 3127 RVA: 0x0006C111 File Offset: 0x0006A311
	[PunRPC]
	public void StateSetRPC(int newState, PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.MasterOnlyRPC(_info))
		{
			return;
		}
		this.state = (ItemMine.States)newState;
		this.stateStart = true;
		this.stateTimer = 0f;
		this.beepTimer = 0f;
	}

	// Token: 0x06000C38 RID: 3128 RVA: 0x0006C140 File Offset: 0x0006A340
	private void TriggerScaleFixer()
	{
		if (this.state != ItemMine.States.Armed)
		{
			return;
		}
		bool flag = false;
		if (SemiFunc.FPSImpulse30())
		{
			if (Vector3.Distance(this.prevPos, base.transform.position) > 0.01f)
			{
				flag = true;
				this.prevPos = base.transform.position;
			}
			if (Quaternion.Angle(this.prevRot, base.transform.rotation) > 0.01f)
			{
				flag = true;
				this.prevRot = base.transform.rotation;
			}
		}
		if ((!flag && SemiFunc.FPSImpulse1()) || (flag && SemiFunc.FPSImpulse30()))
		{
			RaycastHit raycastHit;
			if (Physics.Raycast(this.triggerTransform.position, this.triggerTransform.forward, out raycastHit, 1f, LayerMask.GetMask(new string[]
			{
				"Default"
			})))
			{
				this.targetLineLength = raycastHit.distance * 0.8f;
			}
			else
			{
				this.targetLineLength = 1f;
			}
		}
		this.triggerTransform.localScale = Mathf.Lerp(this.triggerTransform.localScale.z, this.targetLineLength, Time.deltaTime * 8f) * Vector3.one;
	}

	// Token: 0x06000C39 RID: 3129 RVA: 0x0006C268 File Offset: 0x0006A468
	private void TriggerRotation()
	{
		this.upsideDown = true;
		if (Vector3.Dot(base.transform.up, Vector3.up) < 0f)
		{
			this.upsideDown = false;
		}
		if (this.upsideDown)
		{
			this.triggerTargetRotation = Quaternion.Euler(-90f, 0f, 0f);
		}
		else
		{
			this.triggerTargetRotation = Quaternion.Euler(90f, 0f, 0f);
		}
		this.triggerTransform.localRotation = SemiFunc.SpringQuaternionGet(this.triggerSpringQuaternion, this.triggerTargetRotation, -1f);
	}

	// Token: 0x06000C3A RID: 3130 RVA: 0x0006C300 File Offset: 0x0006A500
	private void TriggerLineVisuals()
	{
		if (this.state == ItemMine.States.Armed)
		{
			this.triggerLine.material.SetTextureOffset("_MainTex", new Vector2(-Time.time * 2f, 0f));
			if (!this.triggerLine.enabled)
			{
				this.triggerLine.enabled = true;
				this.lineParticles.Play();
			}
			this.triggerLine.widthMultiplier = Mathf.Lerp(this.triggerLine.widthMultiplier, 1f, Time.deltaTime * 4f);
			return;
		}
		if (this.triggerLine.enabled)
		{
			this.triggerLine.widthMultiplier = Mathf.Lerp(this.triggerLine.widthMultiplier, 0f, Time.deltaTime * 8f);
			if (this.triggerLine.widthMultiplier < 0.01f)
			{
				this.triggerLine.enabled = false;
				this.lineParticles.Stop();
			}
		}
	}

	// Token: 0x06000C3B RID: 3131 RVA: 0x0006C3F2 File Offset: 0x0006A5F2
	public void SetTriggered()
	{
		if (this.state == ItemMine.States.Armed)
		{
			this.StateSet(ItemMine.States.Triggering);
		}
	}

	// Token: 0x040013AF RID: 5039
	public ItemMine.MineType mineType;

	// Token: 0x040013B0 RID: 5040
	public Color emissionColor;

	// Token: 0x040013B1 RID: 5041
	public UnityEvent onTriggered;

	// Token: 0x040013B2 RID: 5042
	public float armingTime;

	// Token: 0x040013B3 RID: 5043
	public float triggeringTime;

	// Token: 0x040013B4 RID: 5044
	private SpringQuaternion triggerSpringQuaternion;

	// Token: 0x040013B5 RID: 5045
	private Quaternion triggerTargetRotation;

	// Token: 0x040013B6 RID: 5046
	private bool upsideDown;

	// Token: 0x040013B7 RID: 5047
	public Transform triggerTransform;

	// Token: 0x040013B8 RID: 5048
	public LineRenderer triggerLine;

	// Token: 0x040013B9 RID: 5049
	public ParticleSystem lineParticles;

	// Token: 0x040013BA RID: 5050
	private float beepTimer;

	// Token: 0x040013BB RID: 5051
	private float checkTimer;

	// Token: 0x040013BC RID: 5052
	private ItemMineTrigger itemMineTrigger;

	// Token: 0x040013BD RID: 5053
	private ItemEquippable itemEquippable;

	// Token: 0x040013BE RID: 5054
	private ItemAttributes itemAttributes;

	// Token: 0x040013BF RID: 5055
	private ItemToggle itemToggle;

	// Token: 0x040013C0 RID: 5056
	[Space(20f)]
	private PhotonView photonView;

	// Token: 0x040013C1 RID: 5057
	private PhysGrabObject physGrabObject;

	// Token: 0x040013C2 RID: 5058
	public MeshRenderer meshRenderer;

	// Token: 0x040013C3 RID: 5059
	public Light lightArmed;

	// Token: 0x040013C4 RID: 5060
	[Space(20f)]
	public Sound soundArmingBeep;

	// Token: 0x040013C5 RID: 5061
	public Sound soundArmedBeep;

	// Token: 0x040013C6 RID: 5062
	public Sound soundDisarmingBeep;

	// Token: 0x040013C7 RID: 5063
	public Sound soundDisarmedBeep;

	// Token: 0x040013C8 RID: 5064
	public Sound soundTriggereringBeep;

	// Token: 0x040013C9 RID: 5065
	private float initialLightIntensity;

	// Token: 0x040013CA RID: 5066
	private ParticleScriptExplosion particleScriptExplosion;

	// Token: 0x040013CB RID: 5067
	private bool hasBeenGrabbed;

	// Token: 0x040013CC RID: 5068
	private Vector3 startPosition;

	// Token: 0x040013CD RID: 5069
	private Quaternion startRotation;

	// Token: 0x040013CE RID: 5070
	internal Vector3 triggeredPosition;

	// Token: 0x040013CF RID: 5071
	internal Transform triggeredTransform;

	// Token: 0x040013D0 RID: 5072
	internal PlayerAvatar triggeredPlayerAvatar;

	// Token: 0x040013D1 RID: 5073
	internal PlayerTumble triggeredPlayerTumble;

	// Token: 0x040013D2 RID: 5074
	internal PhysGrabObject triggeredPhysGrabObject;

	// Token: 0x040013D3 RID: 5075
	public bool triggeredByRigidBodies = true;

	// Token: 0x040013D4 RID: 5076
	public bool triggeredByEnemies = true;

	// Token: 0x040013D5 RID: 5077
	public bool triggeredByPlayers = true;

	// Token: 0x040013D6 RID: 5078
	public bool triggeredByForces = true;

	// Token: 0x040013D7 RID: 5079
	public bool destroyAfterTimer;

	// Token: 0x040013D8 RID: 5080
	public float destroyTimer = 10f;

	// Token: 0x040013D9 RID: 5081
	internal bool wasTriggeredByEnemy;

	// Token: 0x040013DA RID: 5082
	internal bool wasTriggeredByPlayer;

	// Token: 0x040013DB RID: 5083
	internal bool wasTriggeredByForce;

	// Token: 0x040013DC RID: 5084
	internal bool wasTriggeredByRigidBody;

	// Token: 0x040013DD RID: 5085
	internal bool firstLight = true;

	// Token: 0x040013DE RID: 5086
	private bool firstLightDone;

	// Token: 0x040013DF RID: 5087
	private float secondArmedTimer;

	// Token: 0x040013E0 RID: 5088
	private bool wasGrabbed;

	// Token: 0x040013E1 RID: 5089
	private float targetLineLength = 1f;

	// Token: 0x040013E2 RID: 5090
	private Vector3 prevPos = Vector3.zero;

	// Token: 0x040013E3 RID: 5091
	private Quaternion prevRot = Quaternion.identity;

	// Token: 0x040013E4 RID: 5092
	internal ItemMine.States state;

	// Token: 0x040013E5 RID: 5093
	private bool stateStart = true;

	// Token: 0x040013E6 RID: 5094
	private float stateTimer;

	// Token: 0x040013E7 RID: 5095
	private PhysGrabObjectImpactDetector impactDetector;

	// Token: 0x040013E8 RID: 5096
	private bool mineDestroyed;

	// Token: 0x02000389 RID: 905
	public enum MineType
	{
		// Token: 0x04002B69 RID: 11113
		None,
		// Token: 0x04002B6A RID: 11114
		Explosive,
		// Token: 0x04002B6B RID: 11115
		Shockwave,
		// Token: 0x04002B6C RID: 11116
		Stun
	}

	// Token: 0x0200038A RID: 906
	public enum States
	{
		// Token: 0x04002B6E RID: 11118
		Disarmed,
		// Token: 0x04002B6F RID: 11119
		Arming,
		// Token: 0x04002B70 RID: 11120
		Armed,
		// Token: 0x04002B71 RID: 11121
		Disarming,
		// Token: 0x04002B72 RID: 11122
		Triggering,
		// Token: 0x04002B73 RID: 11123
		Triggered
	}
}
