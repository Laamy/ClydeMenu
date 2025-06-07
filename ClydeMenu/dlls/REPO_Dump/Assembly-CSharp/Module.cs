using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x020000E1 RID: 225
public class Module : MonoBehaviour
{
	// Token: 0x06000812 RID: 2066 RVA: 0x0004F40A File Offset: 0x0004D60A
	private void Awake()
	{
		this.photonView = base.GetComponent<PhotonView>();
		this.photonView.ObservedComponents.Clear();
	}

	// Token: 0x06000813 RID: 2067 RVA: 0x0004F428 File Offset: 0x0004D628
	private void Start()
	{
		if (base.GetComponent<StartRoom>())
		{
			this.StartRoom = true;
			return;
		}
		base.transform.parent = LevelGenerator.Instance.LevelParent.transform;
	}

	// Token: 0x06000814 RID: 2068 RVA: 0x0004F45C File Offset: 0x0004D65C
	private void ResetChecklist()
	{
		this.wallsInside = false;
		this.wallsMap = false;
		this.levelPointsEntrance = false;
		this.levelPointsWaypoints = false;
		this.levelPointsRoomVolume = false;
		this.levelPointsNavmesh = false;
		this.levelPointsConnected = false;
		this.lightsMax = false;
		this.lightsPrefab = false;
		this.roomVolumeDoors = false;
		this.roomVolumeHeight = false;
		this.roomVolumeSpace = false;
		this.navmeshConnected = false;
		this.navmeshPitfalls = false;
		this.valuablesAllTypes = false;
		this.valuablesMaxed = false;
		this.valuablesSwitch = false;
		this.valuablesSwitchNavmesh = false;
		this.valuablesTest = false;
		this.ModulePropSwitchSetup = false;
		this.ModulePropSwitchNavmesh = false;
	}

	// Token: 0x06000815 RID: 2069 RVA: 0x0004F4FC File Offset: 0x0004D6FC
	private void SetAllChecklist()
	{
		this.wallsInside = true;
		this.wallsMap = true;
		this.levelPointsEntrance = true;
		this.levelPointsWaypoints = true;
		this.levelPointsRoomVolume = true;
		this.levelPointsNavmesh = true;
		this.levelPointsConnected = true;
		this.lightsMax = true;
		this.lightsPrefab = true;
		this.roomVolumeDoors = true;
		this.roomVolumeHeight = true;
		this.roomVolumeSpace = true;
		this.navmeshConnected = true;
		this.navmeshPitfalls = true;
		this.valuablesAllTypes = true;
		this.valuablesMaxed = true;
		this.valuablesSwitch = true;
		this.valuablesSwitchNavmesh = true;
		this.valuablesTest = true;
		this.ModulePropSwitchSetup = true;
		this.ModulePropSwitchNavmesh = true;
	}

	// Token: 0x06000816 RID: 2070 RVA: 0x0004F59C File Offset: 0x0004D79C
	public void ModuleConnectionSet(bool _top, bool _bottom, bool _right, bool _left, bool _first)
	{
		if (SemiFunc.IsMultiplayer())
		{
			this.photonView.RPC("ModuleConnectionSetRPC", RpcTarget.All, new object[]
			{
				_top,
				_bottom,
				_right,
				_left,
				_first
			});
			return;
		}
		this.ModuleConnectionSetRPC(_top, _bottom, _right, _left, _first, default(PhotonMessageInfo));
	}

	// Token: 0x06000817 RID: 2071 RVA: 0x0004F610 File Offset: 0x0004D810
	[PunRPC]
	private void ModuleConnectionSetRPC(bool _top, bool _bottom, bool _right, bool _left, bool _first, PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.MasterOnlyRPC(_info))
		{
			return;
		}
		this.ConnectingTop = _top;
		this.ConnectingBottom = _bottom;
		this.ConnectingRight = _right;
		this.ConnectingLeft = _left;
		this.First = _first;
		this.SetupDone = true;
		foreach (ModulePropSwitch modulePropSwitch in base.GetComponentsInChildren<ModulePropSwitch>())
		{
			modulePropSwitch.Module = this;
			modulePropSwitch.Setup();
		}
		LevelGenerator.Instance.ModulesSpawned++;
		if (!this.wallsInside || !this.wallsMap || !this.levelPointsEntrance || !this.levelPointsWaypoints || !this.levelPointsRoomVolume || !this.levelPointsNavmesh || !this.levelPointsConnected || !this.lightsMax || !this.lightsPrefab || !this.roomVolumeDoors || !this.roomVolumeHeight || !this.roomVolumeSpace || !this.navmeshConnected || !this.navmeshPitfalls || !this.valuablesAllTypes || !this.valuablesMaxed || !this.valuablesSwitch || !this.valuablesSwitchNavmesh || !this.valuablesTest || !this.ModulePropSwitchSetup || !this.ModulePropSwitchNavmesh)
		{
			Debug.LogWarning("Module not checked off: " + base.name, base.gameObject);
		}
	}

	// Token: 0x04000EDC RID: 3804
	[Space]
	public SemiFunc.User moduleOwner;

	// Token: 0x04000EDD RID: 3805
	[Space(20f)]
	private PhotonView photonView;

	// Token: 0x04000EDE RID: 3806
	private Color colorPositive = Color.green;

	// Token: 0x04000EDF RID: 3807
	private Color colorNegative = new Color(1f, 0.74f, 0.61f);

	// Token: 0x04000EE0 RID: 3808
	public bool wallsInside;

	// Token: 0x04000EE1 RID: 3809
	[Space]
	public bool wallsMap;

	// Token: 0x04000EE2 RID: 3810
	public bool levelPointsEntrance;

	// Token: 0x04000EE3 RID: 3811
	[Space]
	public bool levelPointsWaypoints;

	// Token: 0x04000EE4 RID: 3812
	[Space]
	public bool levelPointsRoomVolume;

	// Token: 0x04000EE5 RID: 3813
	[Space]
	public bool levelPointsNavmesh;

	// Token: 0x04000EE6 RID: 3814
	[Space]
	public bool levelPointsConnected;

	// Token: 0x04000EE7 RID: 3815
	public bool lightsMax;

	// Token: 0x04000EE8 RID: 3816
	[Space]
	public bool lightsPrefab;

	// Token: 0x04000EE9 RID: 3817
	public bool roomVolumeDoors;

	// Token: 0x04000EEA RID: 3818
	[Space]
	public bool roomVolumeHeight;

	// Token: 0x04000EEB RID: 3819
	[Space]
	public bool roomVolumeSpace;

	// Token: 0x04000EEC RID: 3820
	public bool navmeshConnected;

	// Token: 0x04000EED RID: 3821
	[Space]
	public bool navmeshPitfalls;

	// Token: 0x04000EEE RID: 3822
	public bool valuablesAllTypes;

	// Token: 0x04000EEF RID: 3823
	[Space]
	public bool valuablesMaxed;

	// Token: 0x04000EF0 RID: 3824
	[Space]
	public bool valuablesSwitch;

	// Token: 0x04000EF1 RID: 3825
	[Space]
	public bool valuablesSwitchNavmesh;

	// Token: 0x04000EF2 RID: 3826
	[Space]
	public bool valuablesTest;

	// Token: 0x04000EF3 RID: 3827
	public bool ModulePropSwitchSetup;

	// Token: 0x04000EF4 RID: 3828
	[Space]
	public bool ModulePropSwitchNavmesh;

	// Token: 0x04000EF5 RID: 3829
	internal bool ConnectingTop;

	// Token: 0x04000EF6 RID: 3830
	internal bool ConnectingRight;

	// Token: 0x04000EF7 RID: 3831
	internal bool ConnectingBottom;

	// Token: 0x04000EF8 RID: 3832
	internal bool ConnectingLeft;

	// Token: 0x04000EF9 RID: 3833
	[Space]
	internal bool SetupDone;

	// Token: 0x04000EFA RID: 3834
	internal bool First;

	// Token: 0x04000EFB RID: 3835
	[Space]
	internal int GridX;

	// Token: 0x04000EFC RID: 3836
	internal int GridY;

	// Token: 0x04000EFD RID: 3837
	public bool Explored;

	// Token: 0x04000EFE RID: 3838
	internal bool StartRoom;

	// Token: 0x0200034D RID: 845
	public enum Type
	{
		// Token: 0x04002A31 RID: 10801
		Normal,
		// Token: 0x04002A32 RID: 10802
		Passage,
		// Token: 0x04002A33 RID: 10803
		DeadEnd,
		// Token: 0x04002A34 RID: 10804
		Extraction
	}
}
