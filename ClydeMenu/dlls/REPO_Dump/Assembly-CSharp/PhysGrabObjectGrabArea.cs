using System;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020001AA RID: 426
public class PhysGrabObjectGrabArea : MonoBehaviour
{
	// Token: 0x06000EA2 RID: 3746 RVA: 0x000849F4 File Offset: 0x00082BF4
	private void Start()
	{
		this.physGrabObject = base.GetComponentInParent<PhysGrabObject>();
		this.staticGrabObject = base.GetComponentInParent<StaticGrabObject>();
		this.photonView = base.GetComponentInParent<PhotonView>();
		foreach (PhysGrabObjectGrabArea.GrabArea grabArea in this.grabAreas)
		{
			if (grabArea.grabAreaTransform)
			{
				if (grabArea.grabAreaTransform.childCount == 0)
				{
					Collider component = grabArea.grabAreaTransform.GetComponent<Collider>();
					if (component != null)
					{
						grabArea.grabAreaColliders.Add(component);
					}
					else
					{
						Debug.LogWarning("Grab area '" + grabArea.grabAreaTransform.name + "' is missing a Collider component.");
					}
				}
				else
				{
					Collider[] componentsInChildren = grabArea.grabAreaTransform.GetComponentsInChildren<Collider>();
					if (componentsInChildren.Length != 0)
					{
						grabArea.grabAreaColliders.AddRange(componentsInChildren);
					}
					else
					{
						Debug.LogWarning("Grab area '" + grabArea.grabAreaTransform.name + "' has children but no colliders.");
					}
				}
			}
			else
			{
				Debug.LogWarning("Grab area in '" + base.gameObject.name + "' has a missing Transform. Please assign it.");
			}
		}
	}

	// Token: 0x06000EA3 RID: 3747 RVA: 0x00084B2C File Offset: 0x00082D2C
	public PlayerAvatar GetLatestGrabber()
	{
		if (this.listOfAllGrabbers.Count > 0)
		{
			return this.listOfAllGrabbers[this.listOfAllGrabbers.Count - 1].playerAvatar;
		}
		return null;
	}

	// Token: 0x06000EA4 RID: 3748 RVA: 0x00084B5C File Offset: 0x00082D5C
	private void Update()
	{
		if (GameManager.instance.gameMode == 1 && !PhotonNetwork.IsMasterClient)
		{
			return;
		}
		foreach (PhysGrabber physGrabber in Enumerable.ToList<PhysGrabber>(this.physGrabObject ? this.physGrabObject.playerGrabbing : this.staticGrabObject.playerGrabbing))
		{
			if (physGrabber.initialPressTimer > 0f)
			{
				Vector3 position = physGrabber.physGrabPoint.position;
				foreach (PhysGrabObjectGrabArea.GrabArea grabArea in this.grabAreas)
				{
					if (grabArea.grabAreaColliders.Count != 0)
					{
						bool flag = false;
						using (List<Collider>.Enumerator enumerator3 = grabArea.grabAreaColliders.GetEnumerator())
						{
							while (enumerator3.MoveNext())
							{
								if (enumerator3.Current.ClosestPoint(position) == position)
								{
									flag = true;
									break;
								}
							}
						}
						if (flag)
						{
							if (!grabArea.listOfGrabbers.Contains(physGrabber))
							{
								grabArea.listOfGrabbers.Add(physGrabber);
								if (!this.listOfAllGrabbers.Contains(physGrabber))
								{
									this.listOfAllGrabbers.Add(physGrabber);
									this.UpdateList(true, physGrabber);
								}
								UnityEvent grabAreaEventOnStart = grabArea.grabAreaEventOnStart;
								if (grabAreaEventOnStart != null)
								{
									grabAreaEventOnStart.Invoke();
								}
							}
							else
							{
								UnityEvent grabAreaEventOnHolding = grabArea.grabAreaEventOnHolding;
								if (grabAreaEventOnHolding != null)
								{
									grabAreaEventOnHolding.Invoke();
								}
							}
							grabArea.grabAreaActive = true;
							break;
						}
					}
				}
			}
		}
		foreach (PhysGrabObjectGrabArea.GrabArea grabArea2 in this.grabAreas)
		{
			for (int i = grabArea2.listOfGrabbers.Count - 1; i >= 0; i--)
			{
				PhysGrabber physGrabber2 = grabArea2.listOfGrabbers[i];
				if (!physGrabber2.grabbed)
				{
					this.UpdateList(false, physGrabber2);
					this.listOfAllGrabbers.Remove(physGrabber2);
					grabArea2.listOfGrabbers.RemoveAt(i);
				}
			}
		}
		foreach (PhysGrabObjectGrabArea.GrabArea grabArea3 in this.grabAreas)
		{
			if (grabArea3.listOfGrabbers.Count == 0 && grabArea3.grabAreaActive)
			{
				UnityEvent grabAreaEventOnRelease = grabArea3.grabAreaEventOnRelease;
				if (grabAreaEventOnRelease != null)
				{
					grabAreaEventOnRelease.Invoke();
				}
				grabArea3.grabAreaActive = false;
			}
		}
	}

	// Token: 0x06000EA5 RID: 3749 RVA: 0x00084E5C File Offset: 0x0008305C
	[PunRPC]
	public void AddToGrabbersList(int grabberId)
	{
		PhysGrabber physGrabber = this.FindGrabberById(grabberId);
		if (physGrabber != null && !this.listOfAllGrabbers.Contains(physGrabber))
		{
			this.listOfAllGrabbers.Add(physGrabber);
		}
	}

	// Token: 0x06000EA6 RID: 3750 RVA: 0x00084E94 File Offset: 0x00083094
	[PunRPC]
	public void RemoveFromGrabbersList(int grabberId)
	{
		PhysGrabber physGrabber = this.FindGrabberById(grabberId);
		if (physGrabber != null)
		{
			this.listOfAllGrabbers.Remove(physGrabber);
		}
	}

	// Token: 0x06000EA7 RID: 3751 RVA: 0x00084EC0 File Offset: 0x000830C0
	private PhysGrabber FindGrabberById(int id)
	{
		foreach (PlayerAvatar playerAvatar in SemiFunc.PlayerGetList())
		{
			PhysGrabber componentInChildren = playerAvatar.GetComponentInChildren<PhysGrabber>();
			if (componentInChildren != null && componentInChildren.photonView.ViewID == id)
			{
				return componentInChildren;
			}
		}
		return null;
	}

	// Token: 0x06000EA8 RID: 3752 RVA: 0x00084F30 File Offset: 0x00083130
	private void UpdateList(bool add, PhysGrabber grabber)
	{
		if (!SemiFunc.IsMultiplayer() || grabber == null)
		{
			return;
		}
		int viewID = grabber.photonView.ViewID;
		if (add)
		{
			this.photonView.RPC("AddToGrabbersList", RpcTarget.Others, new object[]
			{
				viewID
			});
			return;
		}
		this.photonView.RPC("RemoveFromGrabbersList", RpcTarget.Others, new object[]
		{
			viewID
		});
	}

	// Token: 0x0400183A RID: 6202
	private PhysGrabObject physGrabObject;

	// Token: 0x0400183B RID: 6203
	private StaticGrabObject staticGrabObject;

	// Token: 0x0400183C RID: 6204
	private PhotonView photonView;

	// Token: 0x0400183D RID: 6205
	[HideInInspector]
	public List<PhysGrabber> listOfAllGrabbers = new List<PhysGrabber>();

	// Token: 0x0400183E RID: 6206
	public List<PhysGrabObjectGrabArea.GrabArea> grabAreas = new List<PhysGrabObjectGrabArea.GrabArea>();

	// Token: 0x020003B1 RID: 945
	[Serializable]
	public class GrabArea
	{
		// Token: 0x04002C0B RID: 11275
		public Transform grabAreaTransform;

		// Token: 0x04002C0C RID: 11276
		[Space(20f)]
		public UnityEvent grabAreaEventOnStart = new UnityEvent();

		// Token: 0x04002C0D RID: 11277
		public UnityEvent grabAreaEventOnRelease = new UnityEvent();

		// Token: 0x04002C0E RID: 11278
		public UnityEvent grabAreaEventOnHolding = new UnityEvent();

		// Token: 0x04002C0F RID: 11279
		[HideInInspector]
		public bool grabAreaActive;

		// Token: 0x04002C10 RID: 11280
		[HideInInspector]
		public List<PhysGrabber> listOfGrabbers = new List<PhysGrabber>();

		// Token: 0x04002C11 RID: 11281
		[HideInInspector]
		public List<Collider> grabAreaColliders = new List<Collider>();
	}
}
