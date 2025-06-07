using System;
using System.Collections;
using UnityEngine;

// Token: 0x020001C4 RID: 452
public class PlayerCollisionGrounded : MonoBehaviour
{
	// Token: 0x06000F85 RID: 3973 RVA: 0x0008C5A6 File Offset: 0x0008A7A6
	private void Awake()
	{
		PlayerCollisionGrounded.instance = this;
		this.Collider = base.GetComponent<SphereCollider>();
	}

	// Token: 0x06000F86 RID: 3974 RVA: 0x0008C5BA File Offset: 0x0008A7BA
	private void Start()
	{
		this.ColliderCheckActivate();
	}

	// Token: 0x06000F87 RID: 3975 RVA: 0x0008C5C2 File Offset: 0x0008A7C2
	private void OnEnable()
	{
		this.ColliderCheckActivate();
	}

	// Token: 0x06000F88 RID: 3976 RVA: 0x0008C5CA File Offset: 0x0008A7CA
	private void OnDisable()
	{
		this.colliderCheckActive = false;
		base.StopAllCoroutines();
	}

	// Token: 0x06000F89 RID: 3977 RVA: 0x0008C5D9 File Offset: 0x0008A7D9
	private void ColliderCheckActivate()
	{
		if (!this.colliderCheckActive)
		{
			this.colliderCheckActive = true;
			base.StartCoroutine(this.ColliderCheck());
		}
	}

	// Token: 0x06000F8A RID: 3978 RVA: 0x0008C5F7 File Offset: 0x0008A7F7
	private IEnumerator ColliderCheck()
	{
		for (;;)
		{
			this.GroundedTimer -= 1f * Time.deltaTime;
			this.physRiding = false;
			if (this.CollisionController.GroundedDisableTimer <= 0f)
			{
				Collider[] array = Physics.OverlapSphere(base.transform.position, this.Collider.radius, this.LayerMask, QueryTriggerInteraction.Ignore);
				if (array.Length != 0)
				{
					int num = 0;
					if (LevelGenerator.Instance.Generated)
					{
						foreach (Collider collider in array)
						{
							if (collider.gameObject.CompareTag("Phys Grab Object"))
							{
								PhysGrabObject physGrabObject = collider.gameObject.GetComponent<PhysGrabObject>();
								if (!physGrabObject)
								{
									physGrabObject = collider.gameObject.GetComponentInParent<PhysGrabObject>();
								}
								if (physGrabObject && !physGrabObject.physRidingDisabled)
								{
									if (!PlayerController.instance.JumpGroundedObjects.Contains(physGrabObject))
									{
										PlayerController.instance.JumpGroundedObjects.Add(physGrabObject);
									}
									if (physGrabObject.GetComponent<PlayerTumble>())
									{
										num++;
									}
									else if (physGrabObject.roomVolumeCheck.currentSize.magnitude > 1f)
									{
										this.physRiding = true;
										this.physRidingID = physGrabObject.photonView.ViewID;
										this.physRidingPosition = physGrabObject.photonView.transform.InverseTransformPoint(PlayerController.instance.transform.position);
									}
								}
							}
						}
					}
					if (num != array.Length)
					{
						this.GroundedTimer = 0.1f;
						this.Grounded = true;
					}
				}
			}
			if (this.GroundedTimer < 0f)
			{
				this.Grounded = false;
			}
			yield return null;
		}
		yield break;
	}

	// Token: 0x06000F8B RID: 3979 RVA: 0x0008C606 File Offset: 0x0008A806
	private void Update()
	{
		this.CollisionController.Grounded = this.Grounded;
	}

	// Token: 0x040019CB RID: 6603
	public static PlayerCollisionGrounded instance;

	// Token: 0x040019CC RID: 6604
	public PlayerCollisionController CollisionController;

	// Token: 0x040019CD RID: 6605
	internal bool Grounded;

	// Token: 0x040019CE RID: 6606
	private float GroundedTimer;

	// Token: 0x040019CF RID: 6607
	public LayerMask LayerMask;

	// Token: 0x040019D0 RID: 6608
	private SphereCollider Collider;

	// Token: 0x040019D1 RID: 6609
	[HideInInspector]
	public bool physRiding;

	// Token: 0x040019D2 RID: 6610
	[HideInInspector]
	public int physRidingID;

	// Token: 0x040019D3 RID: 6611
	[HideInInspector]
	public Vector3 physRidingPosition;

	// Token: 0x040019D4 RID: 6612
	private bool colliderCheckActive;
}
