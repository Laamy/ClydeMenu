using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x020002CF RID: 719
public class ValuableObject : MonoBehaviour
{
	// Token: 0x06001679 RID: 5753 RVA: 0x000C632A File Offset: 0x000C452A
	private void Awake()
	{
		this.photonView = base.GetComponent<PhotonView>();
	}

	// Token: 0x0600167A RID: 5754 RVA: 0x000C6338 File Offset: 0x000C4538
	private void Start()
	{
		this.physGrabObject = base.GetComponent<PhysGrabObject>();
		this.roomVolumeCheck = base.GetComponent<RoomVolumeCheck>();
		this.navMeshObstacle = base.GetComponent<NavMeshObstacle>();
		if (this.navMeshObstacle)
		{
			Debug.LogError(base.gameObject.name + " has a NavMeshObstacle component. Please remove it.");
		}
		base.StartCoroutine(this.DollarValueSet());
		this.rigidBodyMass = this.physAttributePreset.mass;
		this.rb = base.GetComponent<Rigidbody>();
		if (this.rb)
		{
			this.rb.mass = this.rigidBodyMass;
		}
		this.physGrabObject.massOriginal = this.rigidBodyMass;
		if (!LevelGenerator.Instance.Generated)
		{
			ValuableDirector.instance.valuableSpawnAmount++;
			ValuableDirector.instance.valuableList.Add(this);
		}
		if (this.volumeType <= ValuableVolume.Type.Small)
		{
			this.physGrabObject.clientNonKinematic = true;
		}
	}

	// Token: 0x0600167B RID: 5755 RVA: 0x000C642C File Offset: 0x000C462C
	private void AddToDollarHaulList()
	{
		if (GameManager.instance.gameMode == 1)
		{
			this.photonView.RPC("AddToDollarHaulListRPC", RpcTarget.All, Array.Empty<object>());
			return;
		}
		if (base.GetComponent<ValuableObject>() && !RoundDirector.instance.dollarHaulList.Contains(base.gameObject))
		{
			RoundDirector.instance.dollarHaulList.Add(base.gameObject);
		}
	}

	// Token: 0x0600167C RID: 5756 RVA: 0x000C6498 File Offset: 0x000C4698
	[PunRPC]
	public void AddToDollarHaulListRPC(PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.MasterOnlyRPC(_info))
		{
			return;
		}
		if (base.GetComponent<ValuableObject>() && !RoundDirector.instance.dollarHaulList.Contains(base.gameObject))
		{
			RoundDirector.instance.dollarHaulList.Add(base.gameObject);
		}
	}

	// Token: 0x0600167D RID: 5757 RVA: 0x000C64E8 File Offset: 0x000C46E8
	private void RemoveFromDollarHaulList()
	{
		if (GameManager.instance.gameMode == 1)
		{
			this.photonView.RPC("RemoveFromDollarHaulListRPC", RpcTarget.All, Array.Empty<object>());
			return;
		}
		if (base.GetComponent<ValuableObject>() && RoundDirector.instance.dollarHaulList.Contains(base.gameObject))
		{
			RoundDirector.instance.dollarHaulList.Remove(base.gameObject);
		}
	}

	// Token: 0x0600167E RID: 5758 RVA: 0x000C6554 File Offset: 0x000C4754
	[PunRPC]
	public void RemoveFromDollarHaulListRPC(PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.MasterOnlyRPC(_info))
		{
			return;
		}
		if (base.GetComponent<ValuableObject>() && RoundDirector.instance.dollarHaulList.Contains(base.gameObject))
		{
			RoundDirector.instance.dollarHaulList.Remove(base.gameObject);
		}
	}

	// Token: 0x0600167F RID: 5759 RVA: 0x000C65A4 File Offset: 0x000C47A4
	private void Update()
	{
		if (GameManager.instance.gameMode == 0 || PhotonNetwork.IsMasterClient)
		{
			if (this.inStartRoomCheckTimer > 0f)
			{
				this.inStartRoomCheckTimer -= Time.deltaTime;
			}
			else
			{
				bool flag = false;
				using (List<RoomVolume>.Enumerator enumerator = this.roomVolumeCheck.CurrentRooms.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.Current.Extraction)
						{
							if (!this.inStartRoom)
							{
								this.AddToDollarHaulList();
								this.inStartRoom = true;
							}
							flag = true;
						}
					}
				}
				if (!flag && this.inStartRoom)
				{
					this.RemoveFromDollarHaulList();
					this.inStartRoom = false;
				}
				this.inStartRoomCheckTimer = 0.5f;
			}
		}
		this.DiscoverReminderLogic();
	}

	// Token: 0x06001680 RID: 5760 RVA: 0x000C6674 File Offset: 0x000C4874
	private IEnumerator DollarValueSet()
	{
		yield return new WaitForSeconds(0.05f);
		while (LevelGenerator.Instance.State <= LevelGenerator.LevelState.Valuable)
		{
			yield return new WaitForSeconds(0.05f);
		}
		if (!SemiFunc.IsMultiplayer())
		{
			this.DollarValueSetLogic();
		}
		else if (SemiFunc.IsMasterClient())
		{
			this.DollarValueSetLogic();
			this.photonView.RPC("DollarValueSetRPC", RpcTarget.Others, new object[]
			{
				this.dollarValueCurrent
			});
		}
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			RoundDirector.instance.haulGoalMax += (int)this.dollarValueCurrent;
		}
		yield break;
	}

	// Token: 0x06001681 RID: 5761 RVA: 0x000C6684 File Offset: 0x000C4884
	public void DollarValueSetLogic()
	{
		if (this.dollarValueSet)
		{
			return;
		}
		if (this.dollarValueOverride != 0)
		{
			this.dollarValueOriginal = (float)this.dollarValueOverride;
			this.dollarValueCurrent = (float)this.dollarValueOverride;
		}
		else
		{
			this.dollarValueOriginal = Mathf.Round(Random.Range(this.valuePreset.valueMin, this.valuePreset.valueMax));
			this.dollarValueOriginal = Mathf.Round(this.dollarValueOriginal / 100f) * 100f;
			this.dollarValueCurrent = this.dollarValueOriginal;
		}
		this.dollarValueSet = true;
	}

	// Token: 0x06001682 RID: 5762 RVA: 0x000C6714 File Offset: 0x000C4914
	private void DiscoverReminderLogic()
	{
		if (this.discovered && !this.discoveredReminder)
		{
			if (this.discoveredReminderTimer > 0f)
			{
				this.discoveredReminderTimer -= Time.deltaTime;
				return;
			}
			this.discoveredReminderTimer = Random.Range(2f, 5f);
			if (!this.physGrabObject.impactDetector.inCart && PlayerController.instance.isActiveAndEnabled && Vector3.Distance(base.transform.position, PlayerController.instance.transform.position) > 20f)
			{
				this.discoveredReminder = true;
			}
		}
	}

	// Token: 0x06001683 RID: 5763 RVA: 0x000C67B7 File Offset: 0x000C49B7
	public void Discover(ValuableDiscoverGraphic.State _state)
	{
		if (!this.discovered)
		{
			if (!GameManager.Multiplayer())
			{
				this.DiscoverRPC();
			}
			else
			{
				this.photonView.RPC("DiscoverRPC", RpcTarget.All, Array.Empty<object>());
			}
		}
		ValuableDiscover.instance.New(this.physGrabObject, _state);
	}

	// Token: 0x06001684 RID: 5764 RVA: 0x000C67F7 File Offset: 0x000C49F7
	[PunRPC]
	private void DiscoverRPC()
	{
		this.discovered = true;
		Map.Instance.AddValuable(this);
	}

	// Token: 0x06001685 RID: 5765 RVA: 0x000C680B File Offset: 0x000C4A0B
	[PunRPC]
	public void DollarValueSetRPC(float value, PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.MasterOnlyRPC(_info))
		{
			return;
		}
		this.dollarValueOriginal = value;
		this.dollarValueCurrent = value;
		this.dollarValueSet = true;
	}

	// Token: 0x040026F1 RID: 9969
	[Space(40f)]
	[Header("Presets")]
	public Durability durabilityPreset;

	// Token: 0x040026F2 RID: 9970
	public Value valuePreset;

	// Token: 0x040026F3 RID: 9971
	public PhysAttribute physAttributePreset;

	// Token: 0x040026F4 RID: 9972
	public PhysAudio audioPreset;

	// Token: 0x040026F5 RID: 9973
	[Range(0.5f, 3f)]
	public float audioPresetPitch = 1f;

	// Token: 0x040026F6 RID: 9974
	public Gradient particleColors;

	// Token: 0x040026F7 RID: 9975
	[Space(70f)]
	public ValuableVolume.Type volumeType;

	// Token: 0x040026F8 RID: 9976
	public bool debugVolume = true;

	// Token: 0x040026F9 RID: 9977
	private Mesh meshTiny;

	// Token: 0x040026FA RID: 9978
	private Mesh meshSmall;

	// Token: 0x040026FB RID: 9979
	private Mesh meshMedium;

	// Token: 0x040026FC RID: 9980
	private Mesh meshBig;

	// Token: 0x040026FD RID: 9981
	private Mesh meshWide;

	// Token: 0x040026FE RID: 9982
	private Mesh meshTall;

	// Token: 0x040026FF RID: 9983
	private Mesh meshVeryTall;

	// Token: 0x04002700 RID: 9984
	[Space(20f)]
	internal float dollarValueOriginal = 100f;

	// Token: 0x04002701 RID: 9985
	internal float dollarValueCurrent = 100f;

	// Token: 0x04002702 RID: 9986
	internal bool dollarValueSet;

	// Token: 0x04002703 RID: 9987
	internal int dollarValueOverride;

	// Token: 0x04002704 RID: 9988
	private float rigidBodyMass;

	// Token: 0x04002705 RID: 9989
	private Rigidbody rb;

	// Token: 0x04002706 RID: 9990
	private PhotonView photonView;

	// Token: 0x04002707 RID: 9991
	private NavMeshObstacle navMeshObstacle;

	// Token: 0x04002708 RID: 9992
	private bool inStartRoom;

	// Token: 0x04002709 RID: 9993
	private float inStartRoomCheckTimer;

	// Token: 0x0400270A RID: 9994
	internal RoomVolumeCheck roomVolumeCheck;

	// Token: 0x0400270B RID: 9995
	internal PhysGrabObject physGrabObject;

	// Token: 0x0400270C RID: 9996
	internal bool discovered;

	// Token: 0x0400270D RID: 9997
	internal bool discoveredReminder;

	// Token: 0x0400270E RID: 9998
	private float discoveredReminderTimer;
}
