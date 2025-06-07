using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000239 RID: 569
public class AssetManager : MonoBehaviour
{
	// Token: 0x0600129C RID: 4764 RVA: 0x000A73E9 File Offset: 0x000A55E9
	private void Awake()
	{
		if (AssetManager.instance == null)
		{
			AssetManager.instance = this;
		}
		else
		{
			Object.Destroy(base.gameObject);
		}
		this.mainCamera = Camera.main;
	}

	// Token: 0x0600129D RID: 4765 RVA: 0x000A7416 File Offset: 0x000A5616
	public void PhysImpactEffect(Vector3 _position)
	{
		this.physImpactEffectSound.Play(_position, 1f, 1f, 1f, 1f);
		Object.Instantiate<GameObject>(this.physImpactEffect, _position, Quaternion.identity);
	}

	// Token: 0x04001F8F RID: 8079
	public PhysicMaterial physicMaterialStickyExtreme;

	// Token: 0x04001F90 RID: 8080
	public PhysicMaterial physicMaterialSlipperyExtreme;

	// Token: 0x04001F91 RID: 8081
	public PhysicMaterial physicMaterialSlippery;

	// Token: 0x04001F92 RID: 8082
	public PhysicMaterial physicMaterialDefault;

	// Token: 0x04001F93 RID: 8083
	public PhysicMaterial physicMaterialPlayerMove;

	// Token: 0x04001F94 RID: 8084
	public PhysicMaterial physicMaterialPlayerIdle;

	// Token: 0x04001F95 RID: 8085
	public PhysicMaterial physicMaterialPhysGrabObject;

	// Token: 0x04001F96 RID: 8086
	public PhysicMaterial physicMaterialSlipperyPlus;

	// Token: 0x04001F97 RID: 8087
	public AnimationCurve animationCurveImpact;

	// Token: 0x04001F98 RID: 8088
	public AnimationCurve animationCurveWooshAway;

	// Token: 0x04001F99 RID: 8089
	public AnimationCurve animationCurveWooshIn;

	// Token: 0x04001F9A RID: 8090
	public AnimationCurve animationCurveInOut;

	// Token: 0x04001F9B RID: 8091
	public AnimationCurve animationCurveClickInOut;

	// Token: 0x04001F9C RID: 8092
	public AnimationCurve animationCurveEaseInOut;

	// Token: 0x04001F9D RID: 8093
	public Sound soundEquip;

	// Token: 0x04001F9E RID: 8094
	public Sound soundUnequip;

	// Token: 0x04001F9F RID: 8095
	public Sound soundDeviceTurnOn;

	// Token: 0x04001FA0 RID: 8096
	public Sound soundDeviceTurnOff;

	// Token: 0x04001FA1 RID: 8097
	public Sound batteryChargeSound;

	// Token: 0x04001FA2 RID: 8098
	public Sound batteryDrainSound;

	// Token: 0x04001FA3 RID: 8099
	public Sound batteryLowBeep;

	// Token: 0x04001FA4 RID: 8100
	public Sound batteryLowWarning;

	// Token: 0x04001FA5 RID: 8101
	public List<Color> playerColors;

	// Token: 0x04001FA6 RID: 8102
	public GameObject enemyValuableSmall;

	// Token: 0x04001FA7 RID: 8103
	public GameObject enemyValuableMedium;

	// Token: 0x04001FA8 RID: 8104
	public GameObject enemyValuableBig;

	// Token: 0x04001FA9 RID: 8105
	public GameObject surplusValuableSmall;

	// Token: 0x04001FAA RID: 8106
	public GameObject surplusValuableMedium;

	// Token: 0x04001FAB RID: 8107
	public GameObject surplusValuableBig;

	// Token: 0x04001FAC RID: 8108
	[Space]
	public Mesh valuableMeshTiny;

	// Token: 0x04001FAD RID: 8109
	public Mesh valuableMeshSmall;

	// Token: 0x04001FAE RID: 8110
	public Mesh valuableMeshMedium;

	// Token: 0x04001FAF RID: 8111
	public Mesh valuableMeshBig;

	// Token: 0x04001FB0 RID: 8112
	public Mesh valuableMeshWide;

	// Token: 0x04001FB1 RID: 8113
	public Mesh valuableMeshTall;

	// Token: 0x04001FB2 RID: 8114
	public Mesh valuableMeshVeryTall;

	// Token: 0x04001FB3 RID: 8115
	public GameObject prefabTeleportEffect;

	// Token: 0x04001FB4 RID: 8116
	public GameObject debugEnemyInvestigate;

	// Token: 0x04001FB5 RID: 8117
	public GameObject debugLevelPointError;

	// Token: 0x04001FB6 RID: 8118
	public GameObject debugCube;

	// Token: 0x04001FB7 RID: 8119
	public GameObject physImpactEffect;

	// Token: 0x04001FB8 RID: 8120
	public Sound physImpactEffectSound;

	// Token: 0x04001FB9 RID: 8121
	internal Color colorYellow = new Color(1f, 0.55f, 0f);

	// Token: 0x04001FBA RID: 8122
	internal Camera mainCamera;

	// Token: 0x04001FBB RID: 8123
	public static AssetManager instance;
}
