using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

// Token: 0x02000153 RID: 339
public class ItemGrenade : MonoBehaviour
{
	// Token: 0x06000B98 RID: 2968 RVA: 0x00066D34 File Offset: 0x00064F34
	private void Start()
	{
		this.itemEquippable = base.GetComponent<ItemEquippable>();
		this.itemToggle = base.GetComponent<ItemToggle>();
		this.itemAttributes = base.GetComponent<ItemAttributes>();
		this.photonView = base.GetComponent<PhotonView>();
		this.physGrabObjectImpactDetector = base.GetComponent<PhysGrabObjectImpactDetector>();
		this.splinterTransform = base.transform.Find("Splinter");
		GameObject gameObject = base.transform.Find("Mesh").gameObject;
		this.grenadeEmissionMaterial = gameObject.GetComponent<Renderer>().material;
		this.grenadeStartPosition = base.transform.position;
		this.grenadeStartRotation = base.transform.rotation;
		this.physGrabObject = base.GetComponent<PhysGrabObject>();
		this.rb = base.GetComponent<Rigidbody>();
		this.throwLineTrail = this.throwLine.GetComponent<TrailRenderer>();
	}

	// Token: 0x06000B99 RID: 2969 RVA: 0x00066E08 File Offset: 0x00065008
	private void FixedUpdate()
	{
		if (this.itemEquippable.isEquipped || this.itemEquippable.wasEquippedTimer > 0f)
		{
			this.prevPosition = this.rb.position;
			return;
		}
		Vector3 vector = (this.rb.position - this.prevPosition) / Time.fixedDeltaTime;
		Vector3 normalized = (this.rb.position - this.prevPosition).normalized;
		this.prevPosition = this.rb.position;
		if (!this.physGrabObject.grabbed && vector.magnitude > 2f)
		{
			this.throwLineTimer = 0.2f;
		}
		if (this.throwLineTimer > 0f)
		{
			this.throwLineTrail.emitting = true;
			this.throwLineTimer -= Time.fixedDeltaTime;
			return;
		}
		this.throwLineTrail.emitting = false;
	}

	// Token: 0x06000B9A RID: 2970 RVA: 0x00066EF8 File Offset: 0x000650F8
	private void Update()
	{
		this.soundTick.PlayLoop(this.isActive, 2f, 2f, 1f);
		if (this.itemEquippable.isEquipped)
		{
			if (this.isActive)
			{
				this.isActive = false;
				this.grenadeTimer = 0f;
				this.splinterAnimationProgress = 0f;
				this.itemToggle.ToggleItem(false, -1);
				this.splinterTransform.localEulerAngles = new Vector3(0f, 0f, 0f);
				this.grenadeEmissionMaterial.SetColor("_EmissionColor", Color.black);
			}
			return;
		}
		if (this.isActive)
		{
			if (this.splinterAnimationProgress < 1f)
			{
				this.splinterAnimationProgress += 5f * Time.deltaTime;
				float num = this.splinterAnimationCurve.Evaluate(this.splinterAnimationProgress);
				this.splinterTransform.localEulerAngles = new Vector3(num * 90f, 0f, 0f);
			}
			float value = Mathf.PingPong(Time.time * 8f, 1f);
			Color value2 = this.blinkColor * Mathf.LinearToGammaSpace(value);
			this.grenadeEmissionMaterial.SetColor("_EmissionColor", value2);
		}
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		if (this.itemToggle.toggleState && !this.isActive)
		{
			this.isActive = true;
			this.TickStart();
		}
		if (this.isActive)
		{
			this.grenadeTimer += Time.deltaTime;
			if (this.grenadeTimer >= this.tickTime)
			{
				this.grenadeTimer = 0f;
				this.TickEnd();
			}
		}
	}

	// Token: 0x06000B9B RID: 2971 RVA: 0x00067098 File Offset: 0x00065298
	private void GrenadeReset()
	{
		this.isActive = false;
		this.grenadeTimer = 0f;
		this.throwLine.SetActive(false);
		this.splinterAnimationProgress = 0f;
		this.itemToggle.ToggleItem(false, -1);
		this.splinterTransform.localEulerAngles = new Vector3(0f, 0f, 0f);
		this.grenadeEmissionMaterial.SetColor("_EmissionColor", Color.black);
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			Rigidbody component = base.GetComponent<Rigidbody>();
			component.velocity = Vector3.zero;
			component.angularVelocity = Vector3.zero;
		}
	}

	// Token: 0x06000B9C RID: 2972 RVA: 0x00067134 File Offset: 0x00065334
	private void TickStart()
	{
		if (SemiFunc.IsMasterClient())
		{
			this.photonView.RPC("TickStartRPC", RpcTarget.All, Array.Empty<object>());
			return;
		}
		this.TickStartRPC(default(PhotonMessageInfo));
	}

	// Token: 0x06000B9D RID: 2973 RVA: 0x00067170 File Offset: 0x00065370
	private void TickEnd()
	{
		if (SemiFunc.IsMasterClient())
		{
			this.photonView.RPC("TickEndRPC", RpcTarget.All, Array.Empty<object>());
			return;
		}
		this.TickEndRPC(default(PhotonMessageInfo));
	}

	// Token: 0x06000B9E RID: 2974 RVA: 0x000671AA File Offset: 0x000653AA
	[PunRPC]
	private void TickStartRPC(PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.MasterOnlyRPC(_info))
		{
			return;
		}
		this.soundSplinter.Play(base.transform.position, 1f, 1f, 1f, 1f);
		this.isActive = true;
	}

	// Token: 0x06000B9F RID: 2975 RVA: 0x000671E8 File Offset: 0x000653E8
	[PunRPC]
	private void TickEndRPC(PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.MasterOnlyRPC(_info))
		{
			return;
		}
		if (this.itemEquippable.isEquipped)
		{
			return;
		}
		this.onDetonate.Invoke();
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			if (!SemiFunc.RunIsShop() || this.isSpawnedGrenade)
			{
				if (!this.isSpawnedGrenade)
				{
					StatsManager.instance.ItemRemove(this.itemAttributes.instanceName);
				}
				this.physGrabObjectImpactDetector.DestroyObject(true);
			}
			else
			{
				this.physGrabObject.Teleport(this.grenadeStartPosition, this.grenadeStartRotation);
			}
		}
		if (SemiFunc.RunIsShop() && !this.isSpawnedGrenade)
		{
			this.GrenadeReset();
		}
	}

	// Token: 0x040012C7 RID: 4807
	public Color blinkColor;

	// Token: 0x040012C8 RID: 4808
	public UnityEvent onDetonate;

	// Token: 0x040012C9 RID: 4809
	private ItemToggle itemToggle;

	// Token: 0x040012CA RID: 4810
	private ItemAttributes itemAttributes;

	// Token: 0x040012CB RID: 4811
	internal bool isActive;

	// Token: 0x040012CC RID: 4812
	private float grenadeTimer;

	// Token: 0x040012CD RID: 4813
	public float tickTime = 3f;

	// Token: 0x040012CE RID: 4814
	private PhotonView photonView;

	// Token: 0x040012CF RID: 4815
	private PhysGrabObjectImpactDetector physGrabObjectImpactDetector;

	// Token: 0x040012D0 RID: 4816
	public Sound soundSplinter;

	// Token: 0x040012D1 RID: 4817
	public Sound soundTick;

	// Token: 0x040012D2 RID: 4818
	private float splinterAnimationProgress;

	// Token: 0x040012D3 RID: 4819
	public AnimationCurve splinterAnimationCurve;

	// Token: 0x040012D4 RID: 4820
	private Transform splinterTransform;

	// Token: 0x040012D5 RID: 4821
	private Material grenadeEmissionMaterial;

	// Token: 0x040012D6 RID: 4822
	private ItemEquippable itemEquippable;

	// Token: 0x040012D7 RID: 4823
	private Vector3 grenadeStartPosition;

	// Token: 0x040012D8 RID: 4824
	private Quaternion grenadeStartRotation;

	// Token: 0x040012D9 RID: 4825
	private PhysGrabObject physGrabObject;

	// Token: 0x040012DA RID: 4826
	private Vector3 prevPosition;

	// Token: 0x040012DB RID: 4827
	[FormerlySerializedAs("isThiefGrenade")]
	[HideInInspector]
	public bool isSpawnedGrenade;

	// Token: 0x040012DC RID: 4828
	public GameObject throwLine;

	// Token: 0x040012DD RID: 4829
	private Rigidbody rb;

	// Token: 0x040012DE RID: 4830
	private float throwLineTimer;

	// Token: 0x040012DF RID: 4831
	private TrailRenderer throwLineTrail;
}
