using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200014B RID: 331
public class ItemCartCannonMain : MonoBehaviour
{
	// Token: 0x06000B4C RID: 2892 RVA: 0x000647B4 File Offset: 0x000629B4
	private void Start()
	{
		this.battery = base.GetComponent<ItemBattery>();
		this.physGrabObject = base.GetComponent<PhysGrabObject>();
		this.itemToggle = base.GetComponent<ItemToggle>();
		this.impactDetector = base.GetComponent<PhysGrabObjectImpactDetector>();
		this.rb = base.GetComponent<Rigidbody>();
		this.physGrabObjectGrabArea = base.GetComponent<PhysGrabObjectGrabArea>();
		this.photonView = base.GetComponent<PhotonView>();
		foreach (MeshRenderer meshRenderer in this.grabMeshRenderers)
		{
			if (meshRenderer != null)
			{
				Material material = meshRenderer.material;
				if (material != null)
				{
					this.grabMaterials.Add(material);
				}
			}
		}
	}

	// Token: 0x06000B4D RID: 2893 RVA: 0x0006487C File Offset: 0x00062A7C
	private void Update()
	{
		this.impulseShoot = false;
		this.StateMachine();
		this.MovementSounds();
		if (this.stateTimer <= this.stateTimerMax)
		{
			this.stateTimer += Time.deltaTime;
		}
	}

	// Token: 0x06000B4E RID: 2894 RVA: 0x000648B1 File Offset: 0x00062AB1
	private void FixedUpdate()
	{
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		this.isFixedUpdate = true;
		this.StateMachine();
		this.isFixedUpdate = false;
	}

	// Token: 0x06000B4F RID: 2895 RVA: 0x000648D0 File Offset: 0x00062AD0
	private void StateSet(int state)
	{
		if (state == (int)this.stateCurrent)
		{
			return;
		}
		if (SemiFunc.IsMultiplayer())
		{
			if (SemiFunc.IsMasterClient())
			{
				this.photonView.RPC("StateSetRPC", RpcTarget.All, new object[]
				{
					state
				});
				return;
			}
		}
		else
		{
			this.StateSetRPC(state, default(PhotonMessageInfo));
		}
	}

	// Token: 0x06000B50 RID: 2896 RVA: 0x00064926 File Offset: 0x00062B26
	[PunRPC]
	private void StateSetRPC(int state, PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.MasterOnlyRPC(_info))
		{
			return;
		}
		this.stateStart = true;
		this.statePrev = this.stateCurrent;
		this.stateCurrent = (ItemCartCannonMain.state)state;
		this.stateTimer = 0f;
	}

	// Token: 0x06000B51 RID: 2897 RVA: 0x00064958 File Offset: 0x00062B58
	private void StateInactive()
	{
		if (this.stateStart && !this.isFixedUpdate)
		{
			this.mainMesh.material.SetColor("_EmissionColor", Color.red);
			this.mainLight.color = Color.red;
			this.cartLogoScreen.material.SetColor("_EmissionColor", Color.red);
			this.soundShutdown.Play(base.transform.position, 1f, 1f, 1f, 1f);
			int num = 0;
			foreach (Material material in this.grabMaterials)
			{
				material.SetColor("_EmissionColor", Color.red);
				if (num == 1)
				{
					material.mainTextureOffset = new Vector2(0f, 0f);
				}
				num++;
			}
			this.cartGrabLight.color = Color.red;
			this.stateStart = false;
		}
		if (!this.isFixedUpdate)
		{
			if (this.itemToggle.toggleState)
			{
				this.prevToggleState = false;
				this.itemToggle.toggleState = false;
			}
			if (SemiFunc.FPSImpulse5())
			{
				bool flag = true;
				if (!this.impactDetector.inCart)
				{
					flag = false;
				}
				if (!this.impactDetector.currentCart)
				{
					flag = false;
				}
				if (this.battery.batteryLifeInt <= 0)
				{
					flag = false;
				}
				if (flag)
				{
					this.StateSet(1);
				}
			}
		}
		bool flag2 = this.isFixedUpdate;
	}

	// Token: 0x06000B52 RID: 2898 RVA: 0x00064AE4 File Offset: 0x00062CE4
	private void StateActive()
	{
		if (this.stateStart && !this.isFixedUpdate)
		{
			this.mainMesh.material.SetColor("_EmissionColor", this.mainOnColor);
			this.mainLight.color = this.mainOnColor;
			this.cartLogoScreen.material.SetColor("_EmissionColor", Color.green);
			this.stateStart = false;
			if (this.statePrev == ItemCartCannonMain.state.inactive)
			{
				this.soundBootUp.Play(base.transform.position, 1f, 1f, 1f, 1f);
			}
		}
		if (!this.isFixedUpdate)
		{
			if (SemiFunc.IsMasterClientOrSingleplayer())
			{
				if (!this.impactDetector.currentCart || !this.impactDetector.inCart || this.battery.batteryLifeInt <= 0)
				{
					this.StateSet(0);
					return;
				}
				if (this.itemToggle.toggleState != this.prevToggleState && this.battery.batteryLifeInt > 0)
				{
					this.prevToggleState = this.itemToggle.toggleState;
					this.StateSet(2);
					this.itemToggle.toggleImpulse = false;
				}
			}
			this.CorrectorAndLightLogic();
		}
		if (this.isFixedUpdate)
		{
			this.GrabLogic();
		}
	}

	// Token: 0x06000B53 RID: 2899 RVA: 0x00064C28 File Offset: 0x00062E28
	private void StateBuildup()
	{
		if (this.stateStart && !this.isFixedUpdate)
		{
			this.stateStart = false;
			this.stateTimerMax = this.shootBuildUpTime;
		}
		if (!this.isFixedUpdate)
		{
			this.CorrectorAndLightLogic();
		}
		if (this.isFixedUpdate)
		{
			this.GrabLogic();
		}
		if (this.stateTimer >= this.stateTimerMax)
		{
			this.StateSet(3);
		}
	}

	// Token: 0x06000B54 RID: 2900 RVA: 0x00064C8C File Offset: 0x00062E8C
	private void StateShooting()
	{
		if (this.stateStart && !this.isFixedUpdate)
		{
			this.stateStart = false;
			this.stateTimerMax = this.shootTime;
			this.singleShotNextFrame = false;
			EnemyDirector.instance.SetInvestigate(base.transform.position, this.investigationRange, false);
			this.impulseShoot = true;
		}
		if (!this.isFixedUpdate)
		{
			if (this.singleShot)
			{
				if (this.singleShotNextFrame)
				{
					this.StateSet(4);
					this.singleShotNextFrame = false;
					return;
				}
				this.singleShotNextFrame = true;
			}
			this.impulseShoot = true;
			this.CorrectorAndLightLogic();
			if (!this.impactDetector.inCart)
			{
				this.stateTimer = this.stateTimerMax;
			}
		}
		if (this.isFixedUpdate)
		{
			this.GrabLogic();
		}
		if (this.stateTimer >= this.stateTimerMax)
		{
			this.StateSet(4);
		}
	}

	// Token: 0x06000B55 RID: 2901 RVA: 0x00064D60 File Offset: 0x00062F60
	private void StateGoingBack()
	{
		if (this.stateStart && !this.isFixedUpdate)
		{
			this.stateStart = false;
			this.stateTimerMax = this.goBackFromShootTime;
		}
		if (!this.isFixedUpdate)
		{
			this.CorrectorAndLightLogic();
		}
		if (this.isFixedUpdate)
		{
			this.GrabLogic();
		}
		if (this.stateTimer >= this.stateTimerMax)
		{
			this.StateSet(1);
		}
	}

	// Token: 0x06000B56 RID: 2902 RVA: 0x00064DC4 File Offset: 0x00062FC4
	private void StateMachine()
	{
		switch (this.stateCurrent)
		{
		case ItemCartCannonMain.state.inactive:
			this.StateInactive();
			return;
		case ItemCartCannonMain.state.active:
			this.StateActive();
			return;
		case ItemCartCannonMain.state.buildup:
			this.StateBuildup();
			return;
		case ItemCartCannonMain.state.shooting:
			this.StateShooting();
			return;
		case ItemCartCannonMain.state.goingBack:
			this.StateGoingBack();
			return;
		default:
			return;
		}
	}

	// Token: 0x06000B57 RID: 2903 RVA: 0x00064E18 File Offset: 0x00063018
	private void GrabLogic()
	{
		if (!this.currentCorrector)
		{
			return;
		}
		if (!this.impactDetector.currentCart)
		{
			return;
		}
		float y = this.rotationTargetY.eulerAngles.y;
		float y2 = this.impactDetector.currentCart.transform.rotation.eulerAngles.y;
		float num = Mathf.DeltaAngle(y2, y);
		Quaternion quaternion = Quaternion.Euler(0f, y2 - num, 0f);
		bool flag = false;
		List<PhysGrabber> listOfAllGrabbers = this.physGrabObjectGrabArea.listOfAllGrabbers;
		bool flag2 = false;
		using (List<PhysGrabber>.Enumerator enumerator = this.physGrabObject.playerGrabbing.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.isRotating)
				{
					flag2 = true;
					break;
				}
			}
		}
		if (this.physGrabObject.playerGrabbing.Count > 0 && listOfAllGrabbers.Count > 0)
		{
			foreach (PhysGrabber physGrabber in listOfAllGrabbers)
			{
				physGrabber.OverrideGrabPoint(this.cannonGrabPoint.transform);
			}
			if (this.currentCorrector)
			{
				Quaternion localCameraRotation = this.physGrabObject.playerGrabbing[0].playerAvatar.localCameraRotation;
				this.currentCorrector.transform.rotation = this.physGrabObject.playerGrabbing[0].playerAvatar.localCameraRotation;
				this.currentCorrector.transform.rotation = Quaternion.Euler(0f, this.currentCorrector.transform.rotation.eulerAngles.y, 0f);
				Vector3 vector = this.physGrabObject.playerGrabbing[0].playerAvatar.localCameraPosition + localCameraRotation * Vector3.forward * 0.5f;
				Vector3 position = new Vector3(vector.x, base.transform.position.y, vector.z);
				this.currentCorrector.transform.position = position;
			}
			flag = true;
			if (!flag2)
			{
				this.physGrabObject.OverrideTorqueStrength(0f, 0.1f);
			}
			this.physGrabObject.OverrideGrabStrength(0f, 0.1f);
			this.physGrabObject.OverrideMass(2f, 0.1f);
			Vector3 force = Vector3.down * 2f;
			this.rb.AddForce(force, ForceMode.Force);
		}
		if (!flag2)
		{
			quaternion = this.currentCorrector.transform.rotation;
			float num2 = 0.5f;
			if (flag)
			{
				num2 = 0.8f;
			}
			float num3 = Quaternion.Angle(quaternion, base.transform.rotation);
			num2 *= num3;
			this.physGrabObject.OverrideAngularDrag(5f, 0.1f);
			num2 = Mathf.Min(num2, 12f);
			Vector3 torque = SemiFunc.PhysFollowRotation(base.transform, quaternion, this.rb, num2 * 0.1f);
			this.rb.AddTorque(torque, ForceMode.Impulse);
		}
		Vector3 position2 = base.transform.position;
		Vector3 position3 = this.currentCorrector.transform.position;
		if (flag)
		{
			Vector3 a = SemiFunc.PhysFollowPosition(position2, position3, this.rb.velocity, 5f);
			this.rb.AddForce(a * 0.1f, ForceMode.Impulse);
		}
	}

	// Token: 0x06000B58 RID: 2904 RVA: 0x000651B4 File Offset: 0x000633B4
	private void MovementSounds()
	{
		float num = Quaternion.Angle(this.prevRotation, base.transform.rotation);
		this.prevRotation = base.transform.rotation;
		bool flag = num > 0.1f && this.physGrabObjectGrabArea.listOfAllGrabbers.Count > 0 && this.stateCurrent > ItemCartCannonMain.state.inactive;
		float b = Mathf.Max(num * 0.4f, 0.5f);
		this.smoothPitch = Mathf.Lerp(this.smoothPitch, b, Time.deltaTime * 5f);
		this.soundAimLoop.PlayLoop(flag, 1f, 0.5f, this.smoothPitch);
		if (SemiFunc.FPSImpulse5() && (num > 2f && this.quickTurnSoundCooldown <= 0f && flag))
		{
			this.soundQuickTurn.Play(base.transform.position, 1f, 1f, 1f, 1f);
			this.quickTurnSoundCooldown = 1f;
		}
		if (this.quickTurnSoundCooldown > 0f)
		{
			this.quickTurnSoundCooldown -= Time.deltaTime;
		}
	}

	// Token: 0x06000B59 RID: 2905 RVA: 0x000652D8 File Offset: 0x000634D8
	private void CorrectorAndLightLogic()
	{
		if (SemiFunc.IsMasterClientOrSingleplayer() && this.impactDetector.currentCart)
		{
			if (!this.currentCorrector || (this.currentCorrector && this.currentCorrector.transform.parent != this.impactDetector.currentCart.transform))
			{
				if (this.currentCorrector)
				{
					Object.Destroy(this.currentCorrector);
				}
				GameObject original = new GameObject("GreatCorrector");
				this.currentCorrector = Object.Instantiate<GameObject>(original, this.impactDetector.currentCart.transform);
				this.currentCorrector.transform.localPosition = Vector3.zero;
				this.currentCorrector.transform.localRotation = Quaternion.identity;
				this.currentCorrector.transform.localScale = Vector3.one;
				this.currentCorrector.transform.SetParent(this.impactDetector.currentCart.transform);
			}
			if (this.physGrabObject.playerGrabbing.Count > 0)
			{
				this.rotationTargetY = this.physGrabObject.playerGrabbing[0].playerAvatar.localCameraRotation;
			}
		}
		if (SemiFunc.FPSImpulse15())
		{
			if (this.physGrabObjectGrabArea.listOfAllGrabbers.Count > 0)
			{
				if (!this.handleGrabbed)
				{
					this.soundGrabStart.Play(base.transform.position, 1f, 1f, 1f, 1f);
					this.handleGrabbed = true;
				}
				int num = 0;
				foreach (Material material in this.grabMaterials)
				{
					material.SetColor("_EmissionColor", Color.green);
					if (num == 1)
					{
						material.mainTextureOffset = new Vector2(0.5f, 0f);
					}
					num++;
				}
				this.cartGrabLight.color = Color.green;
				return;
			}
			if (this.handleGrabbed)
			{
				this.soundGrabEnd.Play(base.transform.position, 1f, 1f, 1f, 1f);
				this.handleGrabbed = false;
			}
			int num2 = 0;
			foreach (Material material2 in this.grabMaterials)
			{
				material2.SetColor("_EmissionColor", Color.red);
				if (num2 == 1)
				{
					material2.mainTextureOffset = new Vector2(0f, 0f);
				}
				num2++;
			}
			this.cartGrabLight.color = Color.red;
		}
	}

	// Token: 0x0400123C RID: 4668
	public float shootBuildUpTime = 0.5f;

	// Token: 0x0400123D RID: 4669
	public float shootTime = 0.5f;

	// Token: 0x0400123E RID: 4670
	public float goBackFromShootTime = 0.5f;

	// Token: 0x0400123F RID: 4671
	public bool singleShot;

	// Token: 0x04001240 RID: 4672
	public Color mainOnColor = Color.green;

	// Token: 0x04001241 RID: 4673
	public float investigationRange = 35f;

	// Token: 0x04001242 RID: 4674
	private ItemBattery battery;

	// Token: 0x04001243 RID: 4675
	private PhysGrabObject physGrabObject;

	// Token: 0x04001244 RID: 4676
	internal bool impulseShoot;

	// Token: 0x04001245 RID: 4677
	internal bool impulseShooting;

	// Token: 0x04001246 RID: 4678
	public Transform muzzle;

	// Token: 0x04001247 RID: 4679
	private ItemToggle itemToggle;

	// Token: 0x04001248 RID: 4680
	private bool prevToggleState;

	// Token: 0x04001249 RID: 4681
	private Rigidbody rb;

	// Token: 0x0400124A RID: 4682
	private PhysGrabObjectImpactDetector impactDetector;

	// Token: 0x0400124B RID: 4683
	internal bool isActive;

	// Token: 0x0400124C RID: 4684
	internal Quaternion rotationTargetY;

	// Token: 0x0400124D RID: 4685
	public GameObject currentCorrector;

	// Token: 0x0400124E RID: 4686
	public GameObject cannonGrabPoint;

	// Token: 0x0400124F RID: 4687
	private PhysGrabObjectGrabArea physGrabObjectGrabArea;

	// Token: 0x04001250 RID: 4688
	public List<MeshRenderer> grabMeshRenderers = new List<MeshRenderer>();

	// Token: 0x04001251 RID: 4689
	private List<Material> grabMaterials = new List<Material>();

	// Token: 0x04001252 RID: 4690
	public MeshRenderer cartLogoScreen;

	// Token: 0x04001253 RID: 4691
	public Light cartGrabLight;

	// Token: 0x04001254 RID: 4692
	private PhotonView photonView;

	// Token: 0x04001255 RID: 4693
	public MeshRenderer mainMesh;

	// Token: 0x04001256 RID: 4694
	public Light mainLight;

	// Token: 0x04001257 RID: 4695
	public Sound soundBootUp;

	// Token: 0x04001258 RID: 4696
	public Sound soundShutdown;

	// Token: 0x04001259 RID: 4697
	public Sound soundAimLoop;

	// Token: 0x0400125A RID: 4698
	public Sound soundQuickTurn;

	// Token: 0x0400125B RID: 4699
	private Quaternion prevRotation;

	// Token: 0x0400125C RID: 4700
	private float quickTurnSoundCooldown;

	// Token: 0x0400125D RID: 4701
	private float smoothPitch;

	// Token: 0x0400125E RID: 4702
	internal float stateTimer;

	// Token: 0x0400125F RID: 4703
	internal float stateTimerMax = 0.5f;

	// Token: 0x04001260 RID: 4704
	internal bool stateStart = true;

	// Token: 0x04001261 RID: 4705
	internal ItemCartCannonMain.state stateCurrent;

	// Token: 0x04001262 RID: 4706
	internal ItemCartCannonMain.state statePrev;

	// Token: 0x04001263 RID: 4707
	private bool isFixedUpdate;

	// Token: 0x04001264 RID: 4708
	private bool singleShotNextFrame;

	// Token: 0x04001265 RID: 4709
	public Sound soundGrabStart;

	// Token: 0x04001266 RID: 4710
	public Sound soundGrabEnd;

	// Token: 0x04001267 RID: 4711
	private bool handleGrabbed;

	// Token: 0x0200037D RID: 893
	public enum state
	{
		// Token: 0x04002B32 RID: 11058
		inactive,
		// Token: 0x04002B33 RID: 11059
		active,
		// Token: 0x04002B34 RID: 11060
		buildup,
		// Token: 0x04002B35 RID: 11061
		shooting,
		// Token: 0x04002B36 RID: 11062
		goingBack
	}
}
