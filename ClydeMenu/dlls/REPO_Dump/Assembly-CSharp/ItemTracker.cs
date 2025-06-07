using System;
using Photon.Pun;
using TMPro;
using UnityEngine;

// Token: 0x02000187 RID: 391
public class ItemTracker : MonoBehaviour
{
	// Token: 0x06000D5F RID: 3423 RVA: 0x0007507C File Offset: 0x0007327C
	private void Start()
	{
		this.rb = base.GetComponent<Rigidbody>();
		this.physGrabObject = base.GetComponent<PhysGrabObject>();
		this.itemToggle = base.GetComponent<ItemToggle>();
		this.itemBattery = base.GetComponent<ItemBattery>();
		this.photonView = base.GetComponent<PhotonView>();
		this.meshRenderer.material.SetColor("_EmissionColor", Color.black);
		this.nozzleLight.enabled = false;
		this.nozzleLight.intensity = 0f;
	}

	// Token: 0x06000D60 RID: 3424 RVA: 0x000750FC File Offset: 0x000732FC
	private void ValuableTarget()
	{
		if (this.trackerType != ItemTracker.TrackerType.Valuable)
		{
			return;
		}
		Vector3 position = this.nozzleTransform.position;
		this.hasTarget = false;
		float radius = 15f;
		if (!this.currentTarget)
		{
			radius = 30f;
		}
		Collider[] array = Physics.OverlapSphere(base.transform.position, radius);
		float num = float.MaxValue;
		Collider[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			ValuableObject componentInParent = array2[i].gameObject.GetComponentInParent<ValuableObject>();
			if (componentInParent && !componentInParent.discovered)
			{
				PhysGrabObject component = componentInParent.GetComponent<PhysGrabObject>();
				PhysGrabObjectImpactDetector component2 = componentInParent.GetComponent<PhysGrabObjectImpactDetector>();
				float num2 = Vector3.Distance(position, component.midPoint);
				if (num2 < num && !component.grabbed && !component2.inCart)
				{
					num = num2;
					this.currentTarget = component.transform;
					this.currentTargetPhysGrabObject = component;
					this.hasTarget = true;
				}
			}
		}
		if (this.hasTarget)
		{
			this.SetTarget(this.currentTargetPhysGrabObject.photonView.ViewID);
		}
	}

	// Token: 0x06000D61 RID: 3425 RVA: 0x00075204 File Offset: 0x00073404
	private void ExtractionTarget()
	{
		if (this.trackerType != ItemTracker.TrackerType.Extraction)
		{
			return;
		}
		this.hasTarget = false;
		ExtractionPoint extractionPoint = SemiFunc.ExtractionPointGetNearestNotActivated(this.nozzleTransform.position);
		if (extractionPoint)
		{
			this.currentTarget = extractionPoint.transform;
			this.hasTarget = true;
		}
		if (this.hasTarget)
		{
			this.SetTarget(this.currentTarget.GetComponent<PhotonView>().ViewID);
		}
	}

	// Token: 0x06000D62 RID: 3426 RVA: 0x0007526C File Offset: 0x0007346C
	private void FindATarget()
	{
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		if (this.itemBattery.batteryLife <= 0f)
		{
			return;
		}
		this.timer += Time.deltaTime;
		if (this.timer > 2f)
		{
			this.ValuableTarget();
			this.ExtractionTarget();
			this.timer = 0f;
		}
	}

	// Token: 0x06000D63 RID: 3427 RVA: 0x000752CC File Offset: 0x000734CC
	private void AnimateEmissionToBlack()
	{
		if (this.itemToggle.toggleState)
		{
			return;
		}
		Color color = this.meshRenderer.material.GetColor("_EmissionColor");
		if (color != Color.black)
		{
			this.meshRenderer.material.SetColor("_EmissionColor", Color.Lerp(color, Color.black, Time.deltaTime * 20f));
		}
		if (this.nozzleLight.intensity > 0f)
		{
			this.nozzleLight.intensity = Mathf.Lerp(this.nozzleLight.intensity, 0f, Time.deltaTime * 10f);
			return;
		}
		this.nozzleLight.enabled = false;
	}

	// Token: 0x06000D64 RID: 3428 RVA: 0x00075380 File Offset: 0x00073580
	private void PhysGrabOverrides()
	{
		if (this.physGrabObject.grabbed && this.physGrabObject.grabbedLocal)
		{
			PhysGrabber.instance.OverrideGrabDistance(0.8f);
		}
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		if (this.physGrabObject.grabbed)
		{
			Quaternion turnX = Quaternion.Euler(0f, 0f, 0f);
			Quaternion turnY = Quaternion.Euler(0f, 0f, 0f);
			Quaternion identity = Quaternion.identity;
			this.physGrabObject.TurnXYZ(turnX, turnY, identity);
			this.physGrabObject.OverrideTorqueStrengthX(2f, 0.1f);
			if (this.currentTarget && this.itemToggle.toggleState)
			{
				this.physGrabObject.OverrideTorqueStrengthY(0.1f, 0.1f);
			}
			this.physGrabObject.OverrideGrabVerticalPosition(-0.2f);
			return;
		}
		if (this.itemToggle.toggleState)
		{
			this.itemToggle.ToggleItem(false, -1);
		}
	}

	// Token: 0x06000D65 RID: 3429 RVA: 0x0007547C File Offset: 0x0007367C
	private void DisplayLogic()
	{
		if (this.display.gameObject.activeSelf)
		{
			Vector2 textureOffset = this.display.material.GetTextureOffset("_MainTex");
			textureOffset.y += Time.deltaTime * 2f;
			this.display.material.SetTextureOffset("_MainTex", textureOffset);
			if (SemiFunc.IsMasterClientOrSingleplayer())
			{
				this.itemBattery.batteryLife -= Time.deltaTime * 0.5f;
			}
		}
		else if (this.displayText.text != "--")
		{
			this.displayText.text = "--";
		}
		if (this.displayOverrideTimer >= 0f)
		{
			this.displayOverrideTimer -= Time.deltaTime;
			if (this.displayOverrideTimer <= 0f)
			{
				this.displayText.text = "--";
				Color color = this.colorScreenNeutral;
				color.a = 0.2f;
				this.displayText.color = this.colorScreenNeutral;
				this.display.material.color = color;
				this.displayLight.color = this.colorScreenNeutral;
			}
		}
		if (this.trackerType == ItemTracker.TrackerType.Valuable && this.currentTargetPhysGrabObject)
		{
			this.targetPosition = this.currentTargetPhysGrabObject.midPoint;
		}
		if (this.trackerType == ItemTracker.TrackerType.Extraction && this.currentTarget)
		{
			this.targetPosition = this.currentTarget.position;
		}
		if (this.changeDigitTimer <= 0f && this.displayOverrideTimer <= 0f)
		{
			if (this.hasTarget && this.display.gameObject.activeSelf)
			{
				int num = Mathf.RoundToInt(Vector3.Distance(this.nozzleTransform.position, this.targetPosition));
				if (num != this.prevDigit)
				{
					this.changeDigitTimer = 1f;
					this.digitSwap.Play(this.display.transform.position, 1f, 1f, 1f, 1f);
					this.prevDigit = num;
				}
				this.displayText.text = num.ToString();
			}
			else
			{
				this.displayText.text = "--";
			}
		}
		else
		{
			this.changeDigitTimer -= Time.deltaTime;
		}
		if (!SemiFunc.FPSImpulse15())
		{
			return;
		}
		if (this.itemToggle.toggleState)
		{
			if (!this.display.gameObject.activeSelf)
			{
				this.display.gameObject.SetActive(true);
				return;
			}
		}
		else if (this.display.gameObject.activeSelf)
		{
			this.display.gameObject.SetActive(false);
		}
	}

	// Token: 0x06000D66 RID: 3430 RVA: 0x00075734 File Offset: 0x00073934
	private void TargetLogic()
	{
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		if (!this.itemToggle.toggleState)
		{
			this.hasTarget = false;
			this.currentTarget = null;
			this.displayOverrideTimer = 0f;
			return;
		}
		if (this.trackerType == ItemTracker.TrackerType.Valuable)
		{
			if (this.currentTarget && this.currentTarget.GetComponent<ValuableObject>().discovered && this.hasTarget)
			{
				this.CurrentTargetUpdate(true);
				this.currentTarget = null;
				this.hasTarget = false;
			}
			if (!this.currentTarget && this.hasTarget && this.physGrabObject.grabbed)
			{
				this.CurrentTargetUpdate(false);
				this.hasTarget = false;
			}
		}
		if (this.trackerType == ItemTracker.TrackerType.Extraction)
		{
			if (this.currentTarget)
			{
				this.hasTarget = true;
				return;
			}
			this.hasTarget = false;
		}
	}

	// Token: 0x06000D67 RID: 3431 RVA: 0x0007580C File Offset: 0x00073A0C
	private void Update()
	{
		this.PhysGrabOverrides();
		if (this.itemBattery.batteryLifeInt == 0 && this.itemToggle.toggleState)
		{
			if (!this.display.gameObject.activeSelf && this.itemToggle.toggleState)
			{
				this.display.gameObject.SetActive(true);
				this.batteryOutTimer = 0f;
			}
			if (this.batteryOutTimer == 0f)
			{
				this.soundTargetLost.Play(this.display.transform.position, 1f, 1f, 1f, 1f);
			}
			if (this.batteryOutTimer > 2f && this.itemToggle.toggleState)
			{
				this.itemToggle.ToggleItem(false, -1);
				this.display.gameObject.SetActive(false);
				this.batteryOutTimer = 0f;
				return;
			}
			this.DisplayColorOverride("X", Color.red, 2f);
			this.batteryOutTimer += Time.deltaTime;
			return;
		}
		else
		{
			this.batteryOutTimer = 0f;
			this.DisplayLogic();
			this.TargetLogic();
			if (this.displayOverrideTimer > 0f)
			{
				return;
			}
			this.AnimateEmissionToBlack();
			if (!this.itemToggle.toggleState)
			{
				return;
			}
			this.FindATarget();
			this.Blinking();
			return;
		}
	}

	// Token: 0x06000D68 RID: 3432 RVA: 0x00075968 File Offset: 0x00073B68
	private void Blinking()
	{
		Color color = this.meshRenderer.material.GetColor("_EmissionColor");
		Color color2 = this.colorBleepOff;
		if (color != color2)
		{
			Color color3 = Color.Lerp(color, color2, Time.deltaTime * 4f);
			this.meshRenderer.material.SetColor("_EmissionColor", color3);
			this.nozzleLight.color = color3;
		}
		if (this.nozzleLight.intensity < 1f)
		{
			if (!this.nozzleLight.enabled)
			{
				this.nozzleLight.enabled = true;
			}
			this.nozzleLight.intensity = Mathf.Lerp(this.nozzleLight.intensity, 2f, Time.deltaTime * 10f);
		}
		if (this.hasTarget)
		{
			Vector3 position = this.nozzleTransform.position;
			float b = 1.5f;
			float a = 0.2f;
			this.blipTimer += Time.deltaTime;
			float num = 5f;
			float num2 = 0f;
			float time = (Mathf.Clamp(Vector3.Distance(position, this.targetPosition), num2, num) - num2) / (num - num2);
			float num3 = this.animationCurve.Evaluate(time);
			float num4 = Mathf.Lerp(a, b, num3);
			if (this.blipTimer > num4)
			{
				this.blipTimer = 0f;
				this.soundBleep.Pitch = Mathf.Lerp(1f, 2f, 1f - num3);
				this.soundBleep.Play(this.nozzleTransform.position, 1f, 1f, 1f, 1f);
				this.meshRenderer.material.SetColor("_EmissionColor", this.colorBleep);
				this.nozzleLight.color = this.colorBleep;
				this.nozzleLight.enabled = true;
			}
		}
	}

	// Token: 0x06000D69 RID: 3433 RVA: 0x00075B40 File Offset: 0x00073D40
	private void FixedUpdate()
	{
		if (this.itemBattery.batteryLife <= 0f)
		{
			return;
		}
		if (!this.itemToggle.toggleState)
		{
			this.currentTarget = null;
			this.hasTarget = false;
			return;
		}
		if (this.displayOverrideTimer > 0f)
		{
			return;
		}
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		if (this.hasTarget && this.physGrabObject.grabbed)
		{
			SemiFunc.PhysLookAtPositionWithForce(this.rb, base.transform, this.targetPosition, 10f);
			this.rb.AddForceAtPosition(base.transform.forward * 1f, this.nozzleTransform.position, ForceMode.Force);
		}
	}

	// Token: 0x06000D6A RID: 3434 RVA: 0x00075BEF File Offset: 0x00073DEF
	private void SetTarget(int photonViewID)
	{
		if (SemiFunc.IsMultiplayer())
		{
			this.photonView.RPC("SetTargetRPC", RpcTarget.All, new object[]
			{
				photonViewID
			});
		}
	}

	// Token: 0x06000D6B RID: 3435 RVA: 0x00075C18 File Offset: 0x00073E18
	[PunRPC]
	private void SetTargetRPC(int targetViewID)
	{
		PhysGrabObject component = PhotonView.Find(targetViewID).GetComponent<PhysGrabObject>();
		Transform transform = PhotonView.Find(targetViewID).transform;
		this.currentTarget = transform;
		if (component)
		{
			this.currentTargetPhysGrabObject = component;
		}
		this.hasTarget = true;
	}

	// Token: 0x06000D6C RID: 3436 RVA: 0x00075C5C File Offset: 0x00073E5C
	private void DisplayColorOverride(string _text, Color _color, float _time)
	{
		this.displayText.text = _text;
		this.displayText.color = _color;
		this.displayOverrideTimer = _time;
		this.displayLight.color = _color;
		_color.a = 0.2f;
		this.display.material.color = _color;
	}

	// Token: 0x06000D6D RID: 3437 RVA: 0x00075CB1 File Offset: 0x00073EB1
	private void CurrentTargetUpdate(bool _found)
	{
		if (SemiFunc.IsMultiplayer())
		{
			this.photonView.RPC("CurrentTargetUpdateRPC", RpcTarget.All, new object[]
			{
				_found
			});
			return;
		}
		this.CurrentTargetUpdateRPC(_found);
	}

	// Token: 0x06000D6E RID: 3438 RVA: 0x00075CE4 File Offset: 0x00073EE4
	[PunRPC]
	public void CurrentTargetUpdateRPC(bool _found)
	{
		if (_found)
		{
			this.soundTargetFound.Play(this.display.transform.position, 1f, 1f, 1f, 1f);
			this.DisplayColorOverride("FOUND", this.colorTargetFound, 2f);
		}
		else
		{
			this.soundTargetLost.Play(this.display.transform.position, 1f, 1f, 1f, 1f);
			this.DisplayColorOverride("NOT FOUND", Color.red, 2f);
		}
		this.currentTarget = null;
		this.currentTargetPhysGrabObject = null;
		this.hasTarget = false;
	}

	// Token: 0x04001549 RID: 5449
	public ItemTracker.TrackerType trackerType;

	// Token: 0x0400154A RID: 5450
	private float timer;

	// Token: 0x0400154B RID: 5451
	private Transform currentTarget;

	// Token: 0x0400154C RID: 5452
	private PhysGrabObject currentTargetPhysGrabObject;

	// Token: 0x0400154D RID: 5453
	private Rigidbody rb;

	// Token: 0x0400154E RID: 5454
	public Transform nozzleTransform;

	// Token: 0x0400154F RID: 5455
	private PhysGrabObject physGrabObject;

	// Token: 0x04001550 RID: 5456
	public MeshRenderer meshRenderer;

	// Token: 0x04001551 RID: 5457
	public AnimationCurve animationCurve;

	// Token: 0x04001552 RID: 5458
	private float blipTimer;

	// Token: 0x04001553 RID: 5459
	public Sound soundBleep;

	// Token: 0x04001554 RID: 5460
	public Sound digitSwap;

	// Token: 0x04001555 RID: 5461
	public Sound soundTargetFound;

	// Token: 0x04001556 RID: 5462
	public Sound soundTargetLost;

	// Token: 0x04001557 RID: 5463
	private ItemToggle itemToggle;

	// Token: 0x04001558 RID: 5464
	private ItemBattery itemBattery;

	// Token: 0x04001559 RID: 5465
	private PhotonView photonView;

	// Token: 0x0400155A RID: 5466
	private bool currentToggleState;

	// Token: 0x0400155B RID: 5467
	public Light nozzleLight;

	// Token: 0x0400155C RID: 5468
	public MeshRenderer display;

	// Token: 0x0400155D RID: 5469
	public TextMeshPro displayText;

	// Token: 0x0400155E RID: 5470
	private int prevDigit;

	// Token: 0x0400155F RID: 5471
	private float changeDigitTimer;

	// Token: 0x04001560 RID: 5472
	private float displayOverrideTimer;

	// Token: 0x04001561 RID: 5473
	public Light displayLight;

	// Token: 0x04001562 RID: 5474
	private bool hasTarget;

	// Token: 0x04001563 RID: 5475
	public Color colorBleep;

	// Token: 0x04001564 RID: 5476
	public Color colorBleepOff;

	// Token: 0x04001565 RID: 5477
	public Color colorTargetFound;

	// Token: 0x04001566 RID: 5478
	public Color colorScreenNeutral;

	// Token: 0x04001567 RID: 5479
	private Vector3 targetPosition;

	// Token: 0x04001568 RID: 5480
	private float batteryOutTimer;

	// Token: 0x02000395 RID: 917
	public enum TrackerType
	{
		// Token: 0x04002B94 RID: 11156
		Valuable,
		// Token: 0x04002B95 RID: 11157
		Extraction
	}
}
