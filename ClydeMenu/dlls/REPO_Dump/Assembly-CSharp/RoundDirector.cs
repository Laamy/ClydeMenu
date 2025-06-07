using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000240 RID: 576
public class RoundDirector : MonoBehaviour
{
	// Token: 0x060012CF RID: 4815 RVA: 0x000A8C9D File Offset: 0x000A6E9D
	private void Awake()
	{
		RoundDirector.instance = this;
		this.physGrabObjects = new List<PhysGrabObject>();
	}

	// Token: 0x060012D0 RID: 4816 RVA: 0x000A8CB0 File Offset: 0x000A6EB0
	private IEnumerator StartRound()
	{
		while (!LevelGenerator.Instance.Generated && this.haulGoalMax == 0)
		{
			yield return new WaitForSeconds(0.5f);
		}
		yield return new WaitForSeconds(0.1f);
		if (GameManager.instance.gameMode == 0 || PhotonNetwork.IsMasterClient)
		{
			float num = (float)this.haulGoalMax * 0.7f;
			num *= this.haulGoalCurve.Evaluate(SemiFunc.RunGetDifficultyMultiplier());
			if (this.debugLowHaul || SemiFunc.RunIsTutorial() || SemiFunc.RunIsRecording())
			{
				num = (float)(1000 * this.extractionPoints);
			}
			this.StartRoundLogic((int)num);
			if (GameManager.instance.gameMode == 1)
			{
				this.photonView.RPC("StartRoundRPC", RpcTarget.All, new object[]
				{
					this.haulGoal
				});
			}
		}
		this.extractionPointsFetched = true;
		yield break;
	}

	// Token: 0x060012D1 RID: 4817 RVA: 0x000A8CBF File Offset: 0x000A6EBF
	private void StartRoundLogic(int value)
	{
		this.currentHaul = 0;
		this.currentHaulMax = 0;
		this.deadPlayers = 0;
		this.haulGoal = value;
	}

	// Token: 0x060012D2 RID: 4818 RVA: 0x000A8CDD File Offset: 0x000A6EDD
	[PunRPC]
	private void StartRoundRPC(int value, PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.MasterOnlyRPC(_info))
		{
			return;
		}
		this.StartRoundLogic(value);
	}

	// Token: 0x060012D3 RID: 4819 RVA: 0x000A8CEF File Offset: 0x000A6EEF
	private void Start()
	{
		this.photonView = base.GetComponent<PhotonView>();
		this.photonView.TransferOwnership(PhotonNetwork.MasterClient);
		this.extractionPointsFetched = false;
		base.StartCoroutine(this.StartRound());
	}

	// Token: 0x060012D4 RID: 4820 RVA: 0x000A8D21 File Offset: 0x000A6F21
	public void PhysGrabObjectAdd(PhysGrabObject _physGrabObject)
	{
		if (this.physGrabObjects.Contains(_physGrabObject))
		{
			return;
		}
		this.physGrabObjects.Add(_physGrabObject);
	}

	// Token: 0x060012D5 RID: 4821 RVA: 0x000A8D3E File Offset: 0x000A6F3E
	public void PhysGrabObjectRemove(PhysGrabObject _physGrabObject)
	{
		if (!this.physGrabObjects.Contains(_physGrabObject))
		{
			return;
		}
		this.physGrabObjects.Remove(_physGrabObject);
	}

	// Token: 0x060012D6 RID: 4822 RVA: 0x000A8D5C File Offset: 0x000A6F5C
	private void Update()
	{
		this.currentHaul = 0;
		this.currentHaulMax = 0;
		if (this.dollarHaulList.Count > 0)
		{
			foreach (GameObject gameObject in this.dollarHaulList)
			{
				if (!gameObject)
				{
					this.dollarHaulList.Remove(gameObject);
				}
				else
				{
					this.currentHaul += (int)gameObject.GetComponent<ValuableObject>().dollarValueCurrent;
					this.currentHaulMax += (int)gameObject.GetComponent<ValuableObject>().dollarValueOriginal;
				}
			}
		}
	}

	// Token: 0x060012D7 RID: 4823 RVA: 0x000A8E10 File Offset: 0x000A7010
	public void RequestExtractionPointActivation(int photonViewID)
	{
		if (SemiFunc.IsMultiplayer())
		{
			this.photonView.RPC("RequestExtractionPointActivationRPC", RpcTarget.MasterClient, new object[]
			{
				photonViewID
			});
			return;
		}
		this.RequestExtractionPointActivationRPC(photonViewID);
	}

	// Token: 0x060012D8 RID: 4824 RVA: 0x000A8E41 File Offset: 0x000A7041
	[PunRPC]
	public void RequestExtractionPointActivationRPC(int photonViewID)
	{
		if (!this.extractionPointActive && SemiFunc.IsMasterClientOrSingleplayer())
		{
			this.extractionPointActive = true;
			this.photonView.RPC("ExtractionPointActivateRPC", RpcTarget.All, new object[]
			{
				photonViewID
			});
		}
	}

	// Token: 0x060012D9 RID: 4825 RVA: 0x000A8E79 File Offset: 0x000A7079
	public void ExtractionPointActivate(int photonViewID)
	{
		if (this.extractionPointActive)
		{
			return;
		}
		if (PhotonNetwork.IsMasterClient)
		{
			this.photonView.RPC("ExtractionPointActivateRPC", RpcTarget.All, new object[]
			{
				photonViewID
			});
		}
	}

	// Token: 0x060012DA RID: 4826 RVA: 0x000A8EAC File Offset: 0x000A70AC
	[PunRPC]
	public void ExtractionPointActivateRPC(int photonViewID, PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.MasterOnlyRPC(_info))
		{
			return;
		}
		this.extractionPointDeductionDone = false;
		this.extractionPointActive = true;
		GameObject gameObject = PhotonView.Find(photonViewID).gameObject;
		this.extractionPointCurrent = gameObject.GetComponent<ExtractionPoint>();
		this.extractionPointCurrent.ButtonPress();
		this.ExtractionPointsLock(gameObject);
	}

	// Token: 0x060012DB RID: 4827 RVA: 0x000A8EFC File Offset: 0x000A70FC
	public void ExtractionPointsLock(GameObject exceptMe)
	{
		foreach (GameObject gameObject in this.extractionPointList)
		{
			if (gameObject != exceptMe)
			{
				gameObject.GetComponent<ExtractionPoint>().isLocked = true;
			}
		}
	}

	// Token: 0x060012DC RID: 4828 RVA: 0x000A8F60 File Offset: 0x000A7160
	public void ExtractionCompleted()
	{
		this.extractionPointsCompleted++;
		if (this.extractionPointsCompleted < this.extractionPoints && SemiFunc.RunIsLevel() && TutorialDirector.instance.TutorialSettingCheck(DataDirector.Setting.TutorialMultipleExtractions, 1))
		{
			TutorialDirector.instance.ActivateTip("Multiple Extractions", 2f, false);
		}
	}

	// Token: 0x060012DD RID: 4829 RVA: 0x000A8FB4 File Offset: 0x000A71B4
	public void ExtractionCompletedAllCheck()
	{
		if (SemiFunc.RunIsShop())
		{
			return;
		}
		if (this.extractionPointsCompleted >= this.extractionPoints - 1)
		{
			this.allExtractionPointsCompleted = true;
			if (SemiFunc.IsMasterClientOrSingleplayer())
			{
				if (SemiFunc.IsMultiplayer())
				{
					this.photonView.RPC("ExtractionCompletedAllRPC", RpcTarget.All, Array.Empty<object>());
					return;
				}
				this.ExtractionCompletedAllRPC(default(PhotonMessageInfo));
			}
		}
	}

	// Token: 0x060012DE RID: 4830 RVA: 0x000A9014 File Offset: 0x000A7214
	public void ExtractionPointsUnlock()
	{
		if (SemiFunc.IsMultiplayer())
		{
			if (SemiFunc.IsMasterClient())
			{
				this.photonView.RPC("ExtractionPointsUnlockRPC", RpcTarget.All, Array.Empty<object>());
				return;
			}
		}
		else
		{
			this.ExtractionPointsUnlockRPC(default(PhotonMessageInfo));
		}
	}

	// Token: 0x060012DF RID: 4831 RVA: 0x000A9058 File Offset: 0x000A7258
	[PunRPC]
	public void ExtractionPointsUnlockRPC(PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.MasterOnlyRPC(_info))
		{
			return;
		}
		foreach (GameObject gameObject in this.extractionPointList)
		{
			gameObject.GetComponent<ExtractionPoint>().isLocked = false;
		}
		this.extractionPointCurrent = null;
	}

	// Token: 0x060012E0 RID: 4832 RVA: 0x000A90C0 File Offset: 0x000A72C0
	public void HaulCheck()
	{
		this.currentHaul = 0;
		List<GameObject> list = new List<GameObject>();
		foreach (GameObject gameObject in this.dollarHaulList)
		{
			if (!gameObject)
			{
				list.Add(gameObject);
			}
			else
			{
				ValuableObject component = gameObject.GetComponent<ValuableObject>();
				if (component)
				{
					component.roomVolumeCheck.CheckSet();
					if (!component.roomVolumeCheck.inExtractionPoint)
					{
						list.Add(gameObject);
					}
					else
					{
						this.currentHaul += (int)component.dollarValueCurrent;
					}
				}
			}
		}
		foreach (GameObject gameObject2 in list)
		{
			this.dollarHaulList.Remove(gameObject2);
		}
	}

	// Token: 0x060012E1 RID: 4833 RVA: 0x000A91B4 File Offset: 0x000A73B4
	[PunRPC]
	private void ExtractionCompletedAllRPC(PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.MasterOnlyRPC(_info))
		{
			return;
		}
		AudioScare.instance.PlayImpact();
		this.lightsTurnOffSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
		GameDirector.instance.CameraShake.Shake(3f, 1f);
		GameDirector.instance.CameraImpact.Shake(5f, 0.1f);
		if (SemiFunc.RunIsLevel() && TutorialDirector.instance.TutorialSettingCheck(DataDirector.Setting.TutorialFinalExtraction, 1))
		{
			TutorialDirector.instance.ActivateTip("Final Extraction", 0.5f, false);
		}
	}

	// Token: 0x060012E2 RID: 4834 RVA: 0x000A925C File Offset: 0x000A745C
	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (!SemiFunc.MasterOnlyRPC(info))
		{
			return;
		}
		if (stream.IsWriting)
		{
			stream.SendNext(this.currentHaul);
			stream.SendNext(this.haulGoal);
			return;
		}
		this.currentHaul = (int)stream.ReceiveNext();
		this.haulGoal = (int)stream.ReceiveNext();
	}

	// Token: 0x0400200E RID: 8206
	private PhotonView photonView;

	// Token: 0x0400200F RID: 8207
	public AnimationCurve haulGoalCurve;

	// Token: 0x04002010 RID: 8208
	internal bool debugLowHaul;

	// Token: 0x04002011 RID: 8209
	internal bool debugInfiniteBattery;

	// Token: 0x04002012 RID: 8210
	internal int currentHaul;

	// Token: 0x04002013 RID: 8211
	internal int totalHaul;

	// Token: 0x04002014 RID: 8212
	internal int currentHaulMax;

	// Token: 0x04002015 RID: 8213
	internal int haulGoal;

	// Token: 0x04002016 RID: 8214
	internal int haulGoalMax;

	// Token: 0x04002017 RID: 8215
	internal int deadPlayers;

	// Token: 0x04002018 RID: 8216
	internal int extractionPoints;

	// Token: 0x04002019 RID: 8217
	internal int extractionPointSurplus;

	// Token: 0x0400201A RID: 8218
	internal int extractionPointsCompleted;

	// Token: 0x0400201B RID: 8219
	internal int extractionHaulGoal;

	// Token: 0x0400201C RID: 8220
	internal bool extractionPointActive;

	// Token: 0x0400201D RID: 8221
	internal List<GameObject> extractionPointList = new List<GameObject>();

	// Token: 0x0400201E RID: 8222
	internal ExtractionPoint extractionPointCurrent;

	// Token: 0x0400201F RID: 8223
	internal bool extractionPointsFetched;

	// Token: 0x04002020 RID: 8224
	[HideInInspector]
	public bool extractionPointDeductionDone;

	// Token: 0x04002021 RID: 8225
	internal bool allExtractionPointsCompleted;

	// Token: 0x04002022 RID: 8226
	public static RoundDirector instance;

	// Token: 0x04002023 RID: 8227
	internal List<PhysGrabObject> physGrabObjects;

	// Token: 0x04002024 RID: 8228
	public List<GameObject> dollarHaulList = new List<GameObject>();

	// Token: 0x04002025 RID: 8229
	[Space]
	public Sound lightsTurnOffSound;
}
