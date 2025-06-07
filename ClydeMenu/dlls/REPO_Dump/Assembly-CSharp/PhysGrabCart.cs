using System;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;

// Token: 0x020001A7 RID: 423
public class PhysGrabCart : MonoBehaviour
{
	// Token: 0x06000E8C RID: 3724 RVA: 0x00083434 File Offset: 0x00081634
	private void Start()
	{
		this.itemEquippable = base.GetComponent<ItemEquippable>();
		this.originalHaulColor = this.displayText.color;
		this.physGrabObject = base.GetComponent<PhysGrabObject>();
		this.rb = base.GetComponent<Rigidbody>();
		this.rb.mass = 8f;
		this.inCart = base.transform.Find("In Cart");
		this.thrustEffectRenderer = this.thrustEffect.GetComponent<Renderer>();
		this.thrustEffectOriginalScale = this.thrustEffect.localScale;
		foreach (object obj in base.transform)
		{
			Transform transform = (Transform)obj;
			Transform transform2 = transform.Find("Semi Box Collider");
			if (transform.name.Contains("Inside"))
			{
				this.cartInside.Add(transform2.GetComponent<Collider>());
			}
			if (transform2 && (transform2.GetComponent<Collider>().gameObject.layer == LayerMask.NameToLayer("PhysGrabObject") || transform2.GetComponent<Collider>().gameObject.layer == LayerMask.NameToLayer("Default")))
			{
				transform2.GetComponent<Collider>().gameObject.layer = LayerMask.NameToLayer("PhysGrabObjectCart");
			}
			if (transform.name.Contains("Cart Mesh"))
			{
				this.cartMesh = transform.GetComponent<MeshRenderer>();
			}
			if (transform.name.Contains("Cart Wall Collider"))
			{
				transform2.GetComponent<Collider>().material = SemiFunc.PhysicMaterialPhysGrabObject();
			}
			if (transform.name.Contains("Capsule"))
			{
				this.capsuleColliders.Add(transform.GetComponent<Collider>());
			}
		}
		this.photonView = base.GetComponent<PhotonView>();
		this.physGrabObjectGrabArea = base.GetComponent<PhysGrabObjectGrabArea>();
		foreach (MeshRenderer meshRenderer in this.grabMesh)
		{
			this.grabMaterial.Add(meshRenderer.material);
		}
	}

	// Token: 0x06000E8D RID: 3725 RVA: 0x00083650 File Offset: 0x00081850
	private void ObjectsInCart()
	{
		if (SemiFunc.PlayerNearestDistance(base.transform.position) > 12f)
		{
			return;
		}
		if (this.objectInCartCheckTimer > 0f)
		{
			this.objectInCartCheckTimer -= Time.deltaTime;
		}
		else
		{
			Collider[] array = Physics.OverlapBox(this.inCart.position, this.inCart.localScale / 2f, this.inCart.rotation);
			this.itemsInCart.Clear();
			this.haulPrevious = this.haulCurrent;
			this.itemsInCartCount = 0;
			this.haulCurrent = 0;
			foreach (Collider collider in array)
			{
				if (collider.gameObject.layer == LayerMask.NameToLayer("PhysGrabObject"))
				{
					PhysGrabObject componentInParent = collider.GetComponentInParent<PhysGrabObject>();
					if (componentInParent && !this.itemsInCart.Contains(componentInParent))
					{
						this.itemsInCart.Add(componentInParent);
						ValuableObject componentInParent2 = collider.GetComponentInParent<ValuableObject>();
						if (componentInParent2)
						{
							this.haulCurrent += (int)componentInParent2.dollarValueCurrent;
						}
						this.itemsInCartCount++;
					}
				}
			}
			this.objectInCartCheckTimer = 0.5f;
		}
		if (this.haulPrevious != this.haulCurrent)
		{
			this.haulUpdateEffectTimer = 0.3f;
			if (this.haulCurrent > this.haulPrevious)
			{
				this.deductedFromHaul = false;
				this.soundHaulIncrease.Play(this.displayText.transform.position, 1f, 1f, 1f, 1f);
			}
			else
			{
				this.deductedFromHaul = true;
				this.soundHaulDecrease.Play(this.displayText.transform.position, 1f, 1f, 1f, 1f);
			}
			this.haulPrevious = this.haulCurrent;
		}
		if (this.haulUpdateEffectTimer > 0f)
		{
			this.haulUpdateEffectTimer -= Time.deltaTime;
			this.haulUpdateEffectTimer = Mathf.Max(0f, this.haulUpdateEffectTimer);
			Color color = Color.white;
			if (this.deductedFromHaul)
			{
				color = Color.red;
			}
			this.displayText.color = color;
			if (this.thirtyFPSUpdate)
			{
				this.displayText.text = this.GlitchyText();
			}
			this.resetHaulText = false;
			return;
		}
		if (!this.resetHaulText)
		{
			this.displayText.color = this.originalHaulColor;
			this.SetHaulText();
			this.resetHaulText = true;
		}
	}

	// Token: 0x06000E8E RID: 3726 RVA: 0x000838C8 File Offset: 0x00081AC8
	private void SetHaulText()
	{
		string text = "<color=#bd4300>$</color>";
		this.displayText.text = text + SemiFunc.DollarGetString(Mathf.Max(0, this.haulCurrent));
	}

	// Token: 0x06000E8F RID: 3727 RVA: 0x00083900 File Offset: 0x00081B00
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

	// Token: 0x06000E90 RID: 3728 RVA: 0x00083958 File Offset: 0x00081B58
	private string GlitchyText()
	{
		string text = "";
		for (int i = 0; i < 9; i++)
		{
			bool flag = false;
			if (Random.Range(0, 4) == 0 && i <= 5)
			{
				text += "TAX";
				i += 2;
				flag = true;
			}
			if (Random.Range(0, 3) == 0 && !flag)
			{
				text += "$";
				flag = true;
			}
			if (!flag)
			{
				text += Random.Range(0, 10).ToString();
			}
		}
		return text;
	}

	// Token: 0x06000E91 RID: 3729 RVA: 0x000839D0 File Offset: 0x00081BD0
	private void StateMessages()
	{
		if (SemiFunc.RunIsShop())
		{
			return;
		}
		if (this.physGrabObject.grabbedLocal)
		{
			if (this.currentState == PhysGrabCart.State.Handled)
			{
				Color color = new Color(0.2f, 0.8f, 0.1f);
				ItemInfoExtraUI.instance.ItemInfoText("Mode: STRONG", color);
			}
			if (this.currentState == PhysGrabCart.State.Dragged)
			{
				Color color2 = new Color(1f, 0.46f, 0f);
				ItemInfoExtraUI.instance.ItemInfoText("Mode: WEAK", color2);
			}
		}
	}

	// Token: 0x06000E92 RID: 3730 RVA: 0x00083A50 File Offset: 0x00081C50
	private void SmallCartLogic()
	{
		if (!this.isSmallCart)
		{
			return;
		}
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		if (this.itemEquippable.isEquipping)
		{
			if (!this.smallCartHurtCollider.activeSelf)
			{
				this.smallCartHurtCollider.SetActive(true);
			}
		}
		else if (!this.smallCartHurtCollider.activeSelf)
		{
			this.smallCartHurtCollider.SetActive(false);
		}
		if (this.currentState == PhysGrabCart.State.Locked)
		{
			this.CartMassOverride(8f);
			this.physGrabObject.OverrideMaterial(this.physMaterialSticky, 0.1f);
		}
	}

	// Token: 0x06000E93 RID: 3731 RVA: 0x00083AD8 File Offset: 0x00081CD8
	private void Update()
	{
		if (this.itemEquippable && this.itemEquippable.isUnequipping)
		{
			return;
		}
		this.ThirtyFPS();
		this.ObjectsInCart();
		this.StateMessages();
		this.thrustEffectRenderer.material.mainTextureOffset = new Vector2(Time.time * 0.05f, 0f);
		this.thrustEffect.localScale = new Vector3(this.thrustEffectOriginalScale.x + Mathf.Sin(Time.time * 5f) * 0.02f, this.thrustEffectOriginalScale.y + Mathf.Sin(Time.time * 5f) * 0.02f, this.thrustEffectOriginalScale.z);
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		this.AutoTurnOff();
		this.StateLogic();
		if (this.playerInteractionTimer > 0f)
		{
			this.playerInteractionTimer -= Time.deltaTime;
		}
		this.thirtyFPSUpdate = false;
	}

	// Token: 0x06000E94 RID: 3732 RVA: 0x00083BD0 File Offset: 0x00081DD0
	private void FixedUpdate()
	{
		if (this.physGrabObjectGrabArea && this.physGrabObjectGrabArea.listOfAllGrabbers.Count > 0)
		{
			this.CartSteer();
		}
		else
		{
			this.cartBeingPulled = false;
		}
		if (!LevelGenerator.Instance.Generated)
		{
			return;
		}
		if (GameManager.instance.gameMode == 1 && !PhotonNetwork.IsMasterClient)
		{
			return;
		}
		if (this.rb.IsSleeping())
		{
			if (!this.rb.isKinematic && (Mathf.Abs(base.transform.rotation.eulerAngles.x) > 0.05f || Mathf.Abs(base.transform.rotation.eulerAngles.z) > 0.05f))
			{
				Vector3 eulerAngles = base.transform.rotation.eulerAngles;
				eulerAngles.x = 0f;
				eulerAngles.z = 0f;
				this.rb.MoveRotation(Quaternion.Euler(eulerAngles));
				this.rb.angularVelocity = new Vector3(0f, this.rb.angularVelocity.y, 0f);
			}
		}
		else if (!this.rb.isKinematic)
		{
			Vector3 eulerAngles2 = base.transform.rotation.eulerAngles;
			eulerAngles2.x = 0f;
			eulerAngles2.z = 0f;
			this.rb.MoveRotation(Quaternion.Euler(eulerAngles2));
			this.rb.angularVelocity = new Vector3(0f, this.rb.angularVelocity.y, 0f);
		}
		this.actualVelocity = (base.transform.position - this.actualVelocityLastPosition) / Time.fixedDeltaTime;
		this.actualVelocityLastPosition = base.transform.position;
	}

	// Token: 0x06000E95 RID: 3733 RVA: 0x00083DAD File Offset: 0x00081FAD
	private void AutoTurnOff()
	{
		if (this.physGrabObject.playerGrabbing.Count <= 0)
		{
			this.cartActive = false;
		}
	}

	// Token: 0x06000E96 RID: 3734 RVA: 0x00083DC9 File Offset: 0x00081FC9
	private void CartMassOverride(float mass)
	{
		this.physGrabObject.OverrideMass(mass, 0.1f);
	}

	// Token: 0x06000E97 RID: 3735 RVA: 0x00083DDC File Offset: 0x00081FDC
	private void CartSteer()
	{
		List<PhysGrabber> listOfAllGrabbers = this.physGrabObjectGrabArea.listOfAllGrabbers;
		foreach (PhysGrabber physGrabber in listOfAllGrabbers)
		{
			if (physGrabber)
			{
				if (physGrabber.isLocal)
				{
					TutorialDirector.instance.playerUsedCart = true;
				}
				physGrabber.OverrideGrabPoint(this.cartGrabPoint);
			}
		}
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			foreach (PhysGrabber physGrabber2 in listOfAllGrabbers)
			{
				if (physGrabber2 && !this.inCart.GetComponent<BoxCollider>().bounds.Contains(physGrabber2.transform.position))
				{
					float d = 1f;
					float d2 = 1f;
					Rigidbody component = base.GetComponent<Rigidbody>();
					this.CartMassOverride(4f);
					if (physGrabber2 == PhysGrabber.instance)
					{
						SemiFunc.PhysGrabberLocalChangeAlpha(0.1f);
					}
					if (!this.cartActive && physGrabber2.initialPressTimer > 0f)
					{
						this.cartActive = true;
					}
					if (!this.cartActive)
					{
						break;
					}
					if (physGrabber2 != listOfAllGrabbers[0])
					{
						break;
					}
					this.cartBeingPulled = true;
					physGrabber2.physGrabForcesDisabled = true;
					float a = 2f;
					float b = 2.5f;
					if (this.isSmallCart)
					{
						a = 1.5f;
						b = 2f;
					}
					float num = 5f;
					Vector3 lhs = PlayerController.instance.rb.velocity;
					if (!physGrabber2.isLocal)
					{
						lhs = physGrabber2.playerAvatar.rbVelocityRaw;
					}
					bool flag = Vector3.Dot(lhs, base.transform.forward) > 0f;
					if (physGrabber2.playerAvatar.isSprinting)
					{
						num = 7f;
					}
					if (physGrabber2.playerAvatar.isSprinting && flag)
					{
						a = 3f;
						b = 4f;
					}
					float t = Mathf.Clamp(Vector3.Dot(component.velocity, physGrabber2.transform.forward) / num, 0f, 1f);
					float d3 = Mathf.Lerp(a, b, t);
					Vector3 a2 = physGrabber2.transform.rotation * Vector3.back;
					Vector3 a3 = physGrabber2.playerAvatar.transform.position - a2 * d3;
					float num2 = Mathf.Clamp(Vector3.Distance(base.transform.position, a3 / 1f), 0f, 1f);
					Vector3 vector = (a3 - base.transform.position).normalized * 5f * num2;
					vector = Vector3.ClampMagnitude(vector, 5f);
					float y = component.velocity.y;
					component.velocity = Vector3.MoveTowards(component.velocity, vector, num2 * 2f);
					component.velocity = new Vector3(component.velocity.x, y, component.velocity.z) * d;
					component.velocity = Vector3.ClampMagnitude(component.velocity, 5f);
					Quaternion lhs2 = Quaternion.Euler(0f, Quaternion.LookRotation(physGrabber2.transform.position - base.transform.position, Vector3.up).eulerAngles.y + 180f, 0f);
					Quaternion rotation = Quaternion.Euler(0f, component.rotation.eulerAngles.y, 0f);
					float num3;
					Vector3 vector2;
					(lhs2 * Quaternion.Inverse(rotation)).ToAngleAxis(out num3, out vector2);
					if (num3 > 180f)
					{
						num3 -= 360f;
					}
					float num4 = Mathf.Clamp(Mathf.Abs(num3) / 180f, 0.2f, 1f) * 20f;
					num4 = Mathf.Clamp(num4, 0f, 4f);
					Vector3 vector3 = 0.017453292f * num3 * vector2.normalized * num4;
					vector3 = Vector3.ClampMagnitude(vector3, 4f);
					component.angularVelocity = Vector3.MoveTowards(component.angularVelocity, vector3, num4) * d2;
					component.angularVelocity = Vector3.ClampMagnitude(component.angularVelocity, 4f);
				}
			}
		}
	}

	// Token: 0x06000E98 RID: 3736 RVA: 0x00084288 File Offset: 0x00082488
	private void StateLogic()
	{
		if (!LevelGenerator.Instance.Generated)
		{
			return;
		}
		if (this.cartActive != this.cartActivePrevious)
		{
			this.cartActivePrevious = this.cartActive;
		}
		if (this.physGrabObject.playerGrabbing.Count > 0)
		{
			this.draggedTimer += Time.deltaTime;
		}
		else
		{
			this.draggedTimer = 0f;
		}
		if (this.cartActive)
		{
			this.currentState = PhysGrabCart.State.Handled;
		}
		else if (this.draggedTimer > 0.25f)
		{
			this.currentState = PhysGrabCart.State.Dragged;
		}
		else
		{
			this.currentState = PhysGrabCart.State.Locked;
		}
		if (this.currentState != this.previousState)
		{
			this.previousState = this.currentState;
			this.StateSwitch(this.currentState);
		}
	}

	// Token: 0x06000E99 RID: 3737 RVA: 0x00084342 File Offset: 0x00082542
	private void StateSwitch(PhysGrabCart.State _state)
	{
		if (SemiFunc.IsMultiplayer())
		{
			this.photonView.RPC("StateSwitchRPC", RpcTarget.All, new object[]
			{
				_state
			});
			return;
		}
		this.StateSwitchRPC(_state);
	}

	// Token: 0x06000E9A RID: 3738 RVA: 0x00084374 File Offset: 0x00082574
	[PunRPC]
	private void StateSwitchRPC(PhysGrabCart.State _state)
	{
		this.currentState = _state;
		if (this.currentState == PhysGrabCart.State.Locked)
		{
			this.soundLocked.Play(base.transform.position, 1f, 1f, 1f, 1f);
			this.cartMesh.material.SetColor("_EmissionColor", Color.red);
			foreach (Material material in this.grabMaterial)
			{
				Color red = Color.red;
				material.SetColor("_EmissionColor", red);
				material.mainTextureOffset = new Vector2(0f, 0f);
			}
			foreach (Collider collider in this.capsuleColliders)
			{
				collider.material = this.physMaterialNormal;
			}
			using (List<Collider>.Enumerator enumerator2 = this.cartInside.GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					Collider collider2 = enumerator2.Current;
					collider2.material = this.physMaterialNormal;
				}
				return;
			}
		}
		if (this.currentState == PhysGrabCart.State.Dragged)
		{
			this.soundDragged.Play(base.transform.position, 1f, 1f, 1f, 1f);
			Material material2 = this.cartMesh.material;
			Color value = new Color(1f, 0.46f, 0f);
			material2.SetColor("_EmissionColor", value);
			foreach (Material material3 in this.grabMaterial)
			{
				material3.SetColor("_EmissionColor", value);
				material3.mainTextureOffset = new Vector2(0f, 0f);
			}
			foreach (Collider collider3 in this.capsuleColliders)
			{
				collider3.material = this.physMaterialALilSlippery;
			}
			using (List<Collider>.Enumerator enumerator2 = this.cartInside.GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					Collider collider4 = enumerator2.Current;
					collider4.material = this.physMaterialALilSlippery;
				}
				return;
			}
		}
		this.soundHandled.Play(base.transform.position, 1f, 1f, 1f, 1f);
		this.cartMesh.material.SetColor("_EmissionColor", Color.green);
		int num = 0;
		foreach (Material material4 in this.grabMaterial)
		{
			material4.SetColor("_EmissionColor", Color.green);
			if (num == 1)
			{
				material4.mainTextureOffset = new Vector2(0.5f, 0f);
			}
			num++;
		}
		foreach (Collider collider5 in this.capsuleColliders)
		{
			collider5.material = this.physMaterialSlippery;
		}
		foreach (Collider collider6 in this.cartInside)
		{
			collider6.material = SemiFunc.PhysicMaterialPhysGrabObject();
		}
	}

	// Token: 0x040017FE RID: 6142
	public bool isSmallCart;

	// Token: 0x040017FF RID: 6143
	public GameObject smallCartHurtCollider;

	// Token: 0x04001800 RID: 6144
	internal PhysGrabCart.State currentState;

	// Token: 0x04001801 RID: 6145
	internal PhysGrabCart.State previousState = PhysGrabCart.State.Handled;

	// Token: 0x04001802 RID: 6146
	public Transform thrustEffect;

	// Token: 0x04001803 RID: 6147
	private Renderer thrustEffectRenderer;

	// Token: 0x04001804 RID: 6148
	private Vector3 thrustEffectOriginalScale;

	// Token: 0x04001805 RID: 6149
	public TextMeshPro displayText;

	// Token: 0x04001806 RID: 6150
	public Transform handlePoint;

	// Token: 0x04001807 RID: 6151
	private PhysGrabObject physGrabObject;

	// Token: 0x04001808 RID: 6152
	internal Rigidbody rb;

	// Token: 0x04001809 RID: 6153
	public float stabilizationForce = 100f;

	// Token: 0x0400180A RID: 6154
	private Vector3 hitPoint;

	// Token: 0x0400180B RID: 6155
	private PhotonView photonView;

	// Token: 0x0400180C RID: 6156
	internal bool cartActive;

	// Token: 0x0400180D RID: 6157
	private bool cartActivePrevious;

	// Token: 0x0400180E RID: 6158
	public GameObject buttonObject;

	// Token: 0x0400180F RID: 6159
	private List<Collider> capsuleColliders = new List<Collider>();

	// Token: 0x04001810 RID: 6160
	private List<Collider> cartInside = new List<Collider>();

	// Token: 0x04001811 RID: 6161
	public PhysicMaterial physMaterialSlippery;

	// Token: 0x04001812 RID: 6162
	public PhysicMaterial physMaterialSticky;

	// Token: 0x04001813 RID: 6163
	public PhysicMaterial physMaterialALilSlippery;

	// Token: 0x04001814 RID: 6164
	public PhysicMaterial physMaterialNormal;

	// Token: 0x04001815 RID: 6165
	private Vector3 velocityRef;

	// Token: 0x04001816 RID: 6166
	internal bool cartBeingPulled;

	// Token: 0x04001817 RID: 6167
	private float playerInteractionTimer;

	// Token: 0x04001818 RID: 6168
	private PhysGrabObjectGrabArea physGrabObjectGrabArea;

	// Token: 0x04001819 RID: 6169
	private MeshRenderer cartMesh;

	// Token: 0x0400181A RID: 6170
	public MeshRenderer[] grabMesh;

	// Token: 0x0400181B RID: 6171
	private List<Material> grabMaterial = new List<Material>();

	// Token: 0x0400181C RID: 6172
	[Space]
	public PhysGrabInCart physGrabInCart;

	// Token: 0x0400181D RID: 6173
	internal Transform inCart;

	// Token: 0x0400181E RID: 6174
	internal Vector3 actualVelocity;

	// Token: 0x0400181F RID: 6175
	internal Vector3 actualVelocityLastPosition;

	// Token: 0x04001820 RID: 6176
	private Vector3 lastPosition;

	// Token: 0x04001821 RID: 6177
	internal List<PhysGrabObject> itemsInCart = new List<PhysGrabObject>();

	// Token: 0x04001822 RID: 6178
	internal int itemsInCartCount;

	// Token: 0x04001823 RID: 6179
	internal int haulCurrent;

	// Token: 0x04001824 RID: 6180
	private float objectInCartCheckTimer = 0.5f;

	// Token: 0x04001825 RID: 6181
	private int haulPrevious;

	// Token: 0x04001826 RID: 6182
	private float haulUpdateEffectTimer;

	// Token: 0x04001827 RID: 6183
	private bool deductedFromHaul;

	// Token: 0x04001828 RID: 6184
	private bool resetHaulText;

	// Token: 0x04001829 RID: 6185
	private Color originalHaulColor;

	// Token: 0x0400182A RID: 6186
	public Sound soundHaulIncrease;

	// Token: 0x0400182B RID: 6187
	public Sound soundHaulDecrease;

	// Token: 0x0400182C RID: 6188
	[Space]
	public Sound soundLocked;

	// Token: 0x0400182D RID: 6189
	public Sound soundDragged;

	// Token: 0x0400182E RID: 6190
	public Sound soundHandled;

	// Token: 0x0400182F RID: 6191
	private bool thirtyFPSUpdate;

	// Token: 0x04001830 RID: 6192
	private float thirtyFPSUpdateTimer;

	// Token: 0x04001831 RID: 6193
	private float autoTurnOffTimer;

	// Token: 0x04001832 RID: 6194
	private float draggedTimer;

	// Token: 0x04001833 RID: 6195
	public Transform cartGrabPoint;

	// Token: 0x04001834 RID: 6196
	private ItemEquippable itemEquippable;

	// Token: 0x020003AF RID: 943
	public enum State
	{
		// Token: 0x04002C06 RID: 11270
		Locked,
		// Token: 0x04002C07 RID: 11271
		Dragged,
		// Token: 0x04002C08 RID: 11272
		Handled
	}
}
