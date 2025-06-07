using System;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x02000253 RID: 595
public class MusicalValuableLogic : MonoBehaviour
{
	// Token: 0x06001337 RID: 4919 RVA: 0x000ABCF7 File Offset: 0x000A9EF7
	private void Start()
	{
		this.photonView = base.GetComponent<PhotonView>();
		this.grabArea = base.GetComponent<PhysGrabObjectGrabArea>();
		this.physGrabObject = base.GetComponent<PhysGrabObject>();
		this.numberOfKeys = this.musicKeys.Count * this.numberOfOctaves;
	}

	// Token: 0x06001338 RID: 4920 RVA: 0x000ABD38 File Offset: 0x000A9F38
	private void RemovePhysGrabberFromDictionary(PhysGrabber _physGrabber)
	{
		foreach (KeyValuePair<AudioSource, PhysGrabber> keyValuePair in Enumerable.ToList<KeyValuePair<AudioSource, PhysGrabber>>(this.currentlyPlayedKeys))
		{
			AudioSource key = keyValuePair.Key;
			if (keyValuePair.Value == _physGrabber)
			{
				key.priority = 50;
				this.currentlyPlayedKeys.Remove(key);
				break;
			}
		}
	}

	// Token: 0x06001339 RID: 4921 RVA: 0x000ABDB8 File Offset: 0x000A9FB8
	private void UpdateGrabbedByLocalPlayerGrabRelease()
	{
		if (!SemiFunc.IsMultiplayer())
		{
			this.RemovePhysGrabberFromDictionary(PhysGrabber.instance);
			return;
		}
		this.photonView.RPC("UpdateGrabbedByThisPhysGrabberGrabReleaseRPC", RpcTarget.All, new object[]
		{
			PhysGrabber.instance.photonView.ViewID
		});
	}

	// Token: 0x0600133A RID: 4922 RVA: 0x000ABE08 File Offset: 0x000AA008
	[PunRPC]
	public void UpdateGrabbedByThisPhysGrabberGrabReleaseRPC(int physGrabberPhotonViewID)
	{
		PhysGrabber component = PhotonView.Find(physGrabberPhotonViewID).GetComponent<PhysGrabber>();
		this.RemovePhysGrabberFromDictionary(component);
	}

	// Token: 0x0600133B RID: 4923 RVA: 0x000ABE28 File Offset: 0x000AA028
	private void PitchShiftLogic()
	{
		if (PhysGrabber.instance.grabbed && PhysGrabber.instance.grabbedPhysGrabObject == this.physGrabObject)
		{
			this.grabbedByLocalPlayer = true;
		}
		else
		{
			if (this.grabbedByLocalPlayer)
			{
				this.UpdateGrabbedByLocalPlayerGrabRelease();
			}
			this.grabbedByLocalPlayer = false;
		}
		foreach (KeyValuePair<AudioSource, PhysGrabber> keyValuePair in Enumerable.ToList<KeyValuePair<AudioSource, PhysGrabber>>(this.currentlyPlayedKeys))
		{
			AudioSource key = keyValuePair.Key;
			PhysGrabber value = keyValuePair.Value;
			if (!key || value == null)
			{
				this.currentlyPlayedKeys.Remove(key);
			}
			else
			{
				AudioSource audioSource = key;
				Vector3 physGrabPointPullerPosition = value.physGrabPointPullerPosition;
				Vector3 position = value.physGrabPoint.position;
				float forceMax = value.forceMax;
				float b = Mathf.Clamp(1f + (Vector3.ClampMagnitude(physGrabPointPullerPosition - position, forceMax) * 10f).magnitude / forceMax, 1f, 1f + this.pitchShiftAmount);
				audioSource.pitch = Mathf.Lerp(audioSource.pitch, b, Time.deltaTime * 10f);
				audioSource.priority = 20;
			}
		}
	}

	// Token: 0x0600133C RID: 4924 RVA: 0x000ABF7C File Offset: 0x000AA17C
	private void Update()
	{
		if (this.hasPitchShift)
		{
			this.PitchShiftLogic();
		}
	}

	// Token: 0x0600133D RID: 4925 RVA: 0x000ABF8C File Offset: 0x000AA18C
	public void MusicKeyPressed()
	{
		int num = this.numberOfKeys;
		PlayerAvatar latestGrabber = this.grabArea.GetLatestGrabber();
		Vector3 position = latestGrabber.physGrabber.physGrabPoint.position;
		Vector3 position2 = this.musicKeysStart.position;
		Vector3 position3 = this.musicKeysEnd.position;
		Vector3 normalized = (position3 - position2).normalized;
		float num2 = Vector3.Dot(position - position2, normalized);
		float num3 = Vector3.Distance(position2, position3);
		int num4;
		if (num2 <= 0f)
		{
			num4 = 0;
		}
		else if (num2 >= num3)
		{
			num4 = num - 1;
		}
		else
		{
			float num5 = num3 / (float)num;
			num4 = (int)(num2 / num5);
		}
		if (SemiFunc.IsMultiplayer())
		{
			this.photonView.RPC("MusicKeyPressedRPC", RpcTarget.All, new object[]
			{
				num4,
				latestGrabber.physGrabber.photonView.ViewID
			});
			return;
		}
		this.MusicKeyPressedRPC(num4, -1);
	}

	// Token: 0x0600133E RID: 4926 RVA: 0x000AC072 File Offset: 0x000AA272
	[PunRPC]
	public void MusicKeyPressedRPC(int keyIndex, int grabberID = -1)
	{
		this.PlayKey(keyIndex, grabberID);
		SemiFunc.EnemyInvestigate(this.physGrabObject.midPoint, 25f, false);
	}

	// Token: 0x0600133F RID: 4927 RVA: 0x000AC094 File Offset: 0x000AA294
	private void PlayKey(int key, int grabberID = -1)
	{
		float num = 0.05f;
		int num2 = 0;
		int num3 = this.numberOfKeys / this.musicKeys.Count;
		for (int i = 0; i < this.numberOfKeys; i++)
		{
			int num4 = i % this.musicKeys.Count;
			if (key >= num2 && key < num2 + num3)
			{
				PhysGrabber physGrabber;
				if (grabberID != -1)
				{
					physGrabber = PhotonView.Find(grabberID).GetComponent<PhysGrabber>();
				}
				else
				{
					physGrabber = PhysGrabber.instance;
				}
				int num5 = 0;
				int num6 = this.numberOfKeys - 1;
				float num7 = Mathf.Clamp(1f - (float)(key - num5) / (float)(num6 - num5), 0f, 1f) * this.lowKeyAmpAmount;
				this.musicKeys[num4].Volume = this.volume * (1f + num7);
				this.musicKeys[num4].Pitch = 1f + (float)(key - num2) * num;
				AudioSource audioSource = this.musicKeys[num4].Play(this.physGrabObject.midPoint, 1f, 1f, 1f, 1f);
				audioSource.priority = 20;
				if (this.hasPitchShift && physGrabber)
				{
					this.currentlyPlayedKeys.Add(audioSource, physGrabber);
				}
			}
			num2 += num3;
		}
	}

	// Token: 0x040020C3 RID: 8387
	private PhotonView photonView;

	// Token: 0x040020C4 RID: 8388
	[FormerlySerializedAs("pianoKeysStart")]
	public Transform musicKeysStart;

	// Token: 0x040020C5 RID: 8389
	[FormerlySerializedAs("pianoKeysEnd")]
	public Transform musicKeysEnd;

	// Token: 0x040020C6 RID: 8390
	[Range(0f, 1f)]
	public float volume = 0.25f;

	// Token: 0x040020C7 RID: 8391
	[Range(0f, 3f)]
	public float lowKeyAmpAmount;

	// Token: 0x040020C8 RID: 8392
	[FormerlySerializedAs("pitchShift")]
	public bool hasPitchShift;

	// Token: 0x040020C9 RID: 8393
	public float pitchShiftAmount = 1f;

	// Token: 0x040020CA RID: 8394
	public int numberOfOctaves = 6;

	// Token: 0x040020CB RID: 8395
	public List<Sound> musicKeys;

	// Token: 0x040020CC RID: 8396
	private PhysGrabObject physGrabObject;

	// Token: 0x040020CD RID: 8397
	private PhysGrabObjectGrabArea grabArea;

	// Token: 0x040020CE RID: 8398
	private int numberOfKeys = 108;

	// Token: 0x040020CF RID: 8399
	private Dictionary<AudioSource, PhysGrabber> currentlyPlayedKeys = new Dictionary<AudioSource, PhysGrabber>();

	// Token: 0x040020D0 RID: 8400
	private bool grabbedByLocalPlayer;
}
