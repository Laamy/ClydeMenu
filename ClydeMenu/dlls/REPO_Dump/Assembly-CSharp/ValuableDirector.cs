using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x020002CE RID: 718
public class ValuableDirector : MonoBehaviour
{
	// Token: 0x0600166E RID: 5742 RVA: 0x000C5FD0 File Offset: 0x000C41D0
	private void Awake()
	{
		ValuableDirector.instance = this;
		this.PhotonView = base.GetComponent<PhotonView>();
	}

	// Token: 0x0600166F RID: 5743 RVA: 0x000C5FE4 File Offset: 0x000C41E4
	private void Start()
	{
		if (GameManager.instance.gameMode == 1 && !PhotonNetwork.IsMasterClient)
		{
			base.StartCoroutine(this.SetupClient());
		}
	}

	// Token: 0x06001670 RID: 5744 RVA: 0x000C6007 File Offset: 0x000C4207
	public IEnumerator SetupClient()
	{
		while (this.valuableTargetAmount == -1)
		{
			yield return new WaitForSeconds(0.1f);
		}
		while (this.valuableSpawnAmount < this.valuableTargetAmount)
		{
			yield return new WaitForSeconds(0.1f);
		}
		this.PhotonView.RPC("PlayerReadyRPC", RpcTarget.All, Array.Empty<object>());
		yield break;
	}

	// Token: 0x06001671 RID: 5745 RVA: 0x000C6016 File Offset: 0x000C4216
	public IEnumerator SetupHost()
	{
		float time = SemiFunc.RunGetDifficultyMultiplier();
		if (SemiFunc.RunIsArena())
		{
			time = 0.75f;
		}
		this.totalMaxValue = (float)Mathf.RoundToInt(this.totalMaxValueCurve.Evaluate(time));
		this.tinyMaxAmount = Mathf.RoundToInt(this.tinyMaxAmountCurve.Evaluate(time));
		this.smallMaxAmount = Mathf.RoundToInt(this.smallMaxAmountCurve.Evaluate(time));
		this.mediumMaxAmount = Mathf.RoundToInt(this.mediumMaxAmountCurve.Evaluate(time));
		this.bigMaxAmount = Mathf.RoundToInt(this.bigMaxAmountCurve.Evaluate(time));
		this.wideMaxAmount = Mathf.RoundToInt(this.wideMaxAmountCurve.Evaluate(time));
		this.tallMaxAmount = Mathf.RoundToInt(this.tallMaxAmountCurve.Evaluate(time));
		this.veryTallMaxAmount = Mathf.RoundToInt(this.veryTallMaxAmountCurve.Evaluate(time));
		if (SemiFunc.RunIsArena())
		{
			this.totalMaxAmount /= 2;
			this.tinyMaxAmount /= 3;
			this.smallMaxAmount /= 3;
			this.mediumMaxAmount /= 3;
			this.bigMaxAmount /= 3;
			this.wideMaxAmount /= 2;
			this.tallMaxAmount /= 2;
			this.veryTallMaxAmount /= 2;
		}
		foreach (LevelValuables levelValuables in LevelGenerator.Instance.Level.ValuablePresets)
		{
			this.tinyValuables.AddRange(levelValuables.tiny);
			this.smallValuables.AddRange(levelValuables.small);
			this.mediumValuables.AddRange(levelValuables.medium);
			this.bigValuables.AddRange(levelValuables.big);
			this.wideValuables.AddRange(levelValuables.wide);
			this.tallValuables.AddRange(levelValuables.tall);
			this.veryTallValuables.AddRange(levelValuables.veryTall);
		}
		List<ValuableVolume> list = Enumerable.ToList<ValuableVolume>(Object.FindObjectsOfType<ValuableVolume>(false));
		this.tinyVolumes = list.FindAll((ValuableVolume x) => x.VolumeType == ValuableVolume.Type.Tiny);
		this.tinyVolumes.Shuffle<ValuableVolume>();
		this.smallVolumes = list.FindAll((ValuableVolume x) => x.VolumeType == ValuableVolume.Type.Small);
		this.smallVolumes.Shuffle<ValuableVolume>();
		this.mediumVolumes = list.FindAll((ValuableVolume x) => x.VolumeType == ValuableVolume.Type.Medium);
		this.mediumVolumes.Shuffle<ValuableVolume>();
		this.bigVolumes = list.FindAll((ValuableVolume x) => x.VolumeType == ValuableVolume.Type.Big);
		this.bigVolumes.Shuffle<ValuableVolume>();
		this.wideVolumes = list.FindAll((ValuableVolume x) => x.VolumeType == ValuableVolume.Type.Wide);
		this.wideVolumes.Shuffle<ValuableVolume>();
		this.tallVolumes = list.FindAll((ValuableVolume x) => x.VolumeType == ValuableVolume.Type.Tall);
		this.tallVolumes.Shuffle<ValuableVolume>();
		this.veryTallVolumes = list.FindAll((ValuableVolume x) => x.VolumeType == ValuableVolume.Type.VeryTall);
		this.veryTallVolumes.Shuffle<ValuableVolume>();
		if (this.valuableDebug == ValuableDirector.ValuableDebug.All)
		{
			this.totalMaxAmount = list.Count;
			this.totalMaxValue = 99999f;
			this.tinyMaxAmount = this.tinyVolumes.Count;
			this.smallMaxAmount = this.smallVolumes.Count;
			this.mediumMaxAmount = this.mediumVolumes.Count;
			this.bigMaxAmount = this.bigVolumes.Count;
			this.wideMaxAmount = this.wideVolumes.Count;
			this.tallMaxAmount = this.tallVolumes.Count;
			this.veryTallMaxAmount = this.veryTallVolumes.Count;
		}
		if (this.valuableDebug == ValuableDirector.ValuableDebug.None || LevelGenerator.Instance.Level.ValuablePresets.Count <= 0)
		{
			this.totalMaxAmount = 0;
			this.tinyMaxAmount = 0;
			this.smallMaxAmount = 0;
			this.mediumMaxAmount = 0;
			this.bigMaxAmount = 0;
			this.wideMaxAmount = 0;
			this.tallMaxAmount = 0;
			this.veryTallMaxAmount = 0;
		}
		this.valuableTargetAmount = 0;
		string[] _names = new string[]
		{
			"Tiny",
			"Small",
			"Medium",
			"Big",
			"Wide",
			"Tall",
			"Very Tall"
		};
		int[] _maxAmount = new int[]
		{
			this.tinyMaxAmount,
			this.smallMaxAmount,
			this.mediumMaxAmount,
			this.bigMaxAmount,
			this.wideMaxAmount,
			this.tallMaxAmount,
			this.veryTallMaxAmount
		};
		List<ValuableVolume>[] _volumes = new List<ValuableVolume>[]
		{
			this.tinyVolumes,
			this.smallVolumes,
			this.mediumVolumes,
			this.bigVolumes,
			this.wideVolumes,
			this.tallVolumes,
			this.veryTallVolumes
		};
		string[] _path = new string[]
		{
			this.tinyPath,
			this.smallPath,
			this.mediumPath,
			this.bigPath,
			this.widePath,
			this.tallPath,
			this.veryTallPath
		};
		int[] _chance = new int[]
		{
			this.tinyChance,
			this.smallChance,
			this.mediumChance,
			this.bigChance,
			this.wideChance,
			this.tallChance,
			this.veryTallChance
		};
		List<GameObject>[] _valuables = new List<GameObject>[]
		{
			this.tinyValuables,
			this.smallValuables,
			this.mediumValuables,
			this.bigValuables,
			this.wideValuables,
			this.tallValuables,
			this.veryTallValuables
		};
		int[] _volumeIndex = new int[7];
		int num4;
		for (int _i = 0; _i < this.totalMaxAmount; _i = num4 + 1)
		{
			float num = -1f;
			int num2 = -1;
			for (int i = 0; i < _names.Length; i++)
			{
				if (_volumeIndex[i] < _maxAmount[i] && _volumeIndex[i] < _volumes[i].Count)
				{
					int num3 = Random.Range(0, _chance[i]);
					if ((float)num3 > num)
					{
						num = (float)num3;
						num2 = i;
					}
				}
			}
			if (num2 == -1)
			{
				break;
			}
			ValuableVolume volume = _volumes[num2][_volumeIndex[num2]];
			GameObject valuable = _valuables[num2][Random.Range(0, _valuables[num2].Count)];
			this.Spawn(valuable, volume, _path[num2]);
			_volumeIndex[num2]++;
			yield return null;
			num4 = _i;
		}
		if (this.valuableTargetAmount < this.totalMaxAmount && DebugComputerCheck.instance && (!DebugComputerCheck.instance.enabled || !DebugComputerCheck.instance.LevelDebug || !DebugComputerCheck.instance.ModuleOverrideActive || !DebugComputerCheck.instance.ModuleOverride))
		{
			for (int j = 0; j < _names.Length; j++)
			{
				if (_volumeIndex[j] < _maxAmount[j])
				{
					Debug.LogError("Could not spawn enough ''" + _names[j] + "'' valuables!");
				}
			}
		}
		if (GameManager.instance.gameMode == 1)
		{
			this.PhotonView.RPC("ValuablesTargetSetRPC", RpcTarget.All, new object[]
			{
				this.valuableTargetAmount
			});
		}
		this.valuableSpawnPlayerReady++;
		while (GameManager.instance.gameMode == 1 && this.valuableSpawnPlayerReady < PhotonNetwork.CurrentRoom.PlayerCount)
		{
			yield return new WaitForSeconds(0.1f);
		}
		this.VolumesAndSwitchSetup();
		while (GameManager.instance.gameMode == 1 && this.switchSetupPlayerReady < PhotonNetwork.CurrentRoom.PlayerCount)
		{
			yield return new WaitForSeconds(0.1f);
		}
		this.setupComplete = true;
		yield break;
	}

	// Token: 0x06001672 RID: 5746 RVA: 0x000C6028 File Offset: 0x000C4228
	private void Spawn(GameObject _valuable, ValuableVolume _volume, string _path)
	{
		if (GameManager.instance.gameMode == 0)
		{
			Object.Instantiate<GameObject>(_valuable, _volume.transform.position, _volume.transform.rotation);
		}
		else
		{
			PhotonNetwork.InstantiateRoomObject(this.resourcePath + _path + "/" + _valuable.name, _volume.transform.position, _volume.transform.rotation, 0, null);
		}
		ValuableObject component = _valuable.GetComponent<ValuableObject>();
		component.DollarValueSetLogic();
		this.valuableTargetAmount++;
		this.totalCurrentValue += component.dollarValueCurrent * 0.001f;
		if (this.totalCurrentValue > this.totalMaxValue)
		{
			this.totalMaxAmount = this.valuableTargetAmount;
		}
	}

	// Token: 0x06001673 RID: 5747 RVA: 0x000C60E3 File Offset: 0x000C42E3
	[PunRPC]
	private void ValuablesTargetSetRPC(int _amount, PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.MasterOnlyRPC(_info))
		{
			return;
		}
		this.valuableTargetAmount = _amount;
	}

	// Token: 0x06001674 RID: 5748 RVA: 0x000C60F5 File Offset: 0x000C42F5
	[PunRPC]
	private void PlayerReadyRPC()
	{
		this.valuableSpawnPlayerReady++;
	}

	// Token: 0x06001675 RID: 5749 RVA: 0x000C6108 File Offset: 0x000C4308
	public void VolumesAndSwitchSetup()
	{
		if (GameManager.instance.gameMode == 0)
		{
			this.VolumesAndSwitchSetupRPC(default(PhotonMessageInfo));
			return;
		}
		this.PhotonView.RPC("VolumesAndSwitchSetupRPC", RpcTarget.All, Array.Empty<object>());
	}

	// Token: 0x06001676 RID: 5750 RVA: 0x000C6148 File Offset: 0x000C4348
	[PunRPC]
	private void VolumesAndSwitchSetupRPC(PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.MasterOnlyRPC(_info))
		{
			return;
		}
		ValuableVolume[] array = Object.FindObjectsOfType<ValuableVolume>(true);
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Setup();
		}
		ValuablePropSwitch[] array2 = Object.FindObjectsOfType<ValuablePropSwitch>(true);
		for (int i = 0; i < array2.Length; i++)
		{
			array2[i].Setup();
		}
		if (GameManager.instance.gameMode == 0)
		{
			this.VolumesAndSwitchReadyRPC(default(PhotonMessageInfo));
			return;
		}
		this.PhotonView.RPC("VolumesAndSwitchReadyRPC", RpcTarget.All, Array.Empty<object>());
	}

	// Token: 0x06001677 RID: 5751 RVA: 0x000C61CA File Offset: 0x000C43CA
	[PunRPC]
	private void VolumesAndSwitchReadyRPC(PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!this.switchSetupPlayerReadyList.Contains(_info.Sender))
		{
			this.switchSetupPlayerReadyList.Add(_info.Sender);
			this.switchSetupPlayerReady++;
		}
	}

	// Token: 0x040026B7 RID: 9911
	public static ValuableDirector instance;

	// Token: 0x040026B8 RID: 9912
	private PhotonView PhotonView;

	// Token: 0x040026B9 RID: 9913
	internal ValuableDirector.ValuableDebug valuableDebug;

	// Token: 0x040026BA RID: 9914
	[HideInInspector]
	public bool setupComplete;

	// Token: 0x040026BB RID: 9915
	[HideInInspector]
	public bool valuablesSpawned;

	// Token: 0x040026BC RID: 9916
	internal int valuableSpawnPlayerReady;

	// Token: 0x040026BD RID: 9917
	internal int valuableSpawnAmount;

	// Token: 0x040026BE RID: 9918
	internal int valuableTargetAmount = -1;

	// Token: 0x040026BF RID: 9919
	private List<Player> switchSetupPlayerReadyList = new List<Player>();

	// Token: 0x040026C0 RID: 9920
	internal int switchSetupPlayerReady;

	// Token: 0x040026C1 RID: 9921
	private string resourcePath = "Valuables/";

	// Token: 0x040026C2 RID: 9922
	[Space(20f)]
	public AnimationCurve totalMaxValueCurve;

	// Token: 0x040026C3 RID: 9923
	private float totalMaxValue;

	// Token: 0x040026C4 RID: 9924
	private float totalCurrentValue;

	// Token: 0x040026C5 RID: 9925
	private int totalMaxAmount = 50;

	// Token: 0x040026C6 RID: 9926
	[Space(20f)]
	public AnimationCurve tinyMaxAmountCurve;

	// Token: 0x040026C7 RID: 9927
	public int tinyChance;

	// Token: 0x040026C8 RID: 9928
	private int tinyMaxAmount;

	// Token: 0x040026C9 RID: 9929
	private string tinyPath = "01 Tiny";

	// Token: 0x040026CA RID: 9930
	private List<GameObject> tinyValuables = new List<GameObject>();

	// Token: 0x040026CB RID: 9931
	private List<ValuableVolume> tinyVolumes = new List<ValuableVolume>();

	// Token: 0x040026CC RID: 9932
	[Space]
	public AnimationCurve smallMaxAmountCurve;

	// Token: 0x040026CD RID: 9933
	public int smallChance;

	// Token: 0x040026CE RID: 9934
	private int smallMaxAmount;

	// Token: 0x040026CF RID: 9935
	private string smallPath = "02 Small";

	// Token: 0x040026D0 RID: 9936
	private List<GameObject> smallValuables = new List<GameObject>();

	// Token: 0x040026D1 RID: 9937
	private List<ValuableVolume> smallVolumes = new List<ValuableVolume>();

	// Token: 0x040026D2 RID: 9938
	[Space]
	public AnimationCurve mediumMaxAmountCurve;

	// Token: 0x040026D3 RID: 9939
	public int mediumChance;

	// Token: 0x040026D4 RID: 9940
	private int mediumMaxAmount;

	// Token: 0x040026D5 RID: 9941
	private string mediumPath = "03 Medium";

	// Token: 0x040026D6 RID: 9942
	private List<GameObject> mediumValuables = new List<GameObject>();

	// Token: 0x040026D7 RID: 9943
	private List<ValuableVolume> mediumVolumes = new List<ValuableVolume>();

	// Token: 0x040026D8 RID: 9944
	[Space]
	public AnimationCurve bigMaxAmountCurve;

	// Token: 0x040026D9 RID: 9945
	public int bigChance;

	// Token: 0x040026DA RID: 9946
	private int bigMaxAmount;

	// Token: 0x040026DB RID: 9947
	private string bigPath = "04 Big";

	// Token: 0x040026DC RID: 9948
	private List<GameObject> bigValuables = new List<GameObject>();

	// Token: 0x040026DD RID: 9949
	private List<ValuableVolume> bigVolumes = new List<ValuableVolume>();

	// Token: 0x040026DE RID: 9950
	[Space]
	public AnimationCurve wideMaxAmountCurve;

	// Token: 0x040026DF RID: 9951
	public int wideChance;

	// Token: 0x040026E0 RID: 9952
	private int wideMaxAmount;

	// Token: 0x040026E1 RID: 9953
	private string widePath = "05 Wide";

	// Token: 0x040026E2 RID: 9954
	private List<GameObject> wideValuables = new List<GameObject>();

	// Token: 0x040026E3 RID: 9955
	private List<ValuableVolume> wideVolumes = new List<ValuableVolume>();

	// Token: 0x040026E4 RID: 9956
	[Space]
	public AnimationCurve tallMaxAmountCurve;

	// Token: 0x040026E5 RID: 9957
	public int tallChance;

	// Token: 0x040026E6 RID: 9958
	private int tallMaxAmount;

	// Token: 0x040026E7 RID: 9959
	private string tallPath = "06 Tall";

	// Token: 0x040026E8 RID: 9960
	private List<GameObject> tallValuables = new List<GameObject>();

	// Token: 0x040026E9 RID: 9961
	private List<ValuableVolume> tallVolumes = new List<ValuableVolume>();

	// Token: 0x040026EA RID: 9962
	[Space]
	public AnimationCurve veryTallMaxAmountCurve;

	// Token: 0x040026EB RID: 9963
	public int veryTallChance;

	// Token: 0x040026EC RID: 9964
	private int veryTallMaxAmount;

	// Token: 0x040026ED RID: 9965
	private string veryTallPath = "07 Very Tall";

	// Token: 0x040026EE RID: 9966
	private List<GameObject> veryTallValuables = new List<GameObject>();

	// Token: 0x040026EF RID: 9967
	private List<ValuableVolume> veryTallVolumes = new List<ValuableVolume>();

	// Token: 0x040026F0 RID: 9968
	[Space(20f)]
	public List<ValuableObject> valuableList = new List<ValuableObject>();

	// Token: 0x02000420 RID: 1056
	public enum ValuableDebug
	{
		// Token: 0x04002DF3 RID: 11763
		Normal,
		// Token: 0x04002DF4 RID: 11764
		All,
		// Token: 0x04002DF5 RID: 11765
		None
	}
}
