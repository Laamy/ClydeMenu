using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020000EA RID: 234
public class RoomVolumeCheck : MonoBehaviour
{
	// Token: 0x0600082F RID: 2095 RVA: 0x0004FBB9 File Offset: 0x0004DDB9
	private void Awake()
	{
		if (base.GetComponentInParent<PlayerAvatar>())
		{
			this.player = true;
		}
		this.Mask = LayerMask.GetMask(new string[]
		{
			"RoomVolume"
		});
		this.CheckStart();
	}

	// Token: 0x06000830 RID: 2096 RVA: 0x0004FBF3 File Offset: 0x0004DDF3
	private void OnEnable()
	{
		this.CheckStart();
	}

	// Token: 0x06000831 RID: 2097 RVA: 0x0004FBFB File Offset: 0x0004DDFB
	private void OnDisable()
	{
		this.checkActive = false;
		base.StopAllCoroutines();
	}

	// Token: 0x06000832 RID: 2098 RVA: 0x0004FC0C File Offset: 0x0004DE0C
	public void CheckSet()
	{
		this.inTruck = false;
		this.inExtractionPoint = false;
		this.CurrentRooms.Clear();
		Vector3 localScale = this.currentSize;
		if (localScale == Vector3.zero)
		{
			localScale = base.transform.localScale;
		}
		foreach (Collider collider in Physics.OverlapBox(base.transform.position + base.transform.rotation * this.CheckPosition, localScale / 2f, base.transform.rotation, this.Mask))
		{
			RoomVolume roomVolume = collider.transform.GetComponent<RoomVolume>();
			if (!roomVolume)
			{
				roomVolume = collider.transform.GetComponentInParent<RoomVolume>();
			}
			if (!this.CurrentRooms.Contains(roomVolume))
			{
				this.CurrentRooms.Add(roomVolume);
			}
			if (roomVolume.Truck)
			{
				this.inTruck = true;
			}
			if (roomVolume.Extraction)
			{
				this.inExtractionPoint = true;
			}
		}
		if (this.player && this.CurrentRooms.Count > 0)
		{
			bool flag = true;
			MapModule mapModule = this.CurrentRooms[0].MapModule;
			foreach (RoomVolume roomVolume2 in this.CurrentRooms)
			{
				if (mapModule != roomVolume2.MapModule)
				{
					flag = false;
					break;
				}
			}
			if (flag)
			{
				this.CurrentRooms[0].SetExplored();
			}
		}
	}

	// Token: 0x06000833 RID: 2099 RVA: 0x0004FDA8 File Offset: 0x0004DFA8
	private IEnumerator Check()
	{
		while (!LevelGenerator.Instance.Generated)
		{
			yield return new WaitForSeconds(0.1f);
		}
		yield return new WaitForSeconds(0.5f);
		for (;;)
		{
			if (this.PauseCheckTimer > 0f)
			{
				this.PauseCheckTimer -= 0.5f;
				yield return new WaitForSeconds(0.5f);
			}
			else
			{
				this.CheckSet();
				if (!this.Continuous)
				{
					break;
				}
				if (this.player)
				{
					yield return new WaitForSeconds(0.1f);
				}
				else
				{
					yield return new WaitForSeconds(0.5f);
				}
			}
		}
		yield break;
	}

	// Token: 0x06000834 RID: 2100 RVA: 0x0004FDB7 File Offset: 0x0004DFB7
	private void CheckStart()
	{
		if (!this.checkActive)
		{
			this.checkActive = true;
			base.StartCoroutine(this.Check());
		}
	}

	// Token: 0x04000F1C RID: 3868
	public List<RoomVolume> CurrentRooms;

	// Token: 0x04000F1D RID: 3869
	public bool Continuous = true;

	// Token: 0x04000F1E RID: 3870
	internal float PauseCheckTimer;

	// Token: 0x04000F1F RID: 3871
	private LayerMask Mask;

	// Token: 0x04000F20 RID: 3872
	[Space]
	public bool DebugCheckPosition;

	// Token: 0x04000F21 RID: 3873
	public Vector3 CheckPosition = Vector3.one;

	// Token: 0x04000F22 RID: 3874
	public Vector3 currentSize = Vector3.one;

	// Token: 0x04000F23 RID: 3875
	internal bool inTruck;

	// Token: 0x04000F24 RID: 3876
	internal bool inExtractionPoint;

	// Token: 0x04000F25 RID: 3877
	private bool player;

	// Token: 0x04000F26 RID: 3878
	private bool checkActive;
}
