using System;
using System.Collections;
using UnityEngine;

// Token: 0x020000E9 RID: 233
public class RoomVolume : MonoBehaviour
{
	// Token: 0x0600082B RID: 2091 RVA: 0x0004FB20 File Offset: 0x0004DD20
	private void Start()
	{
		this.Module = base.GetComponentInParent<Module>();
		RoomVolume[] componentsInParent = base.GetComponentsInParent<RoomVolume>();
		for (int i = 0; i < componentsInParent.Length; i++)
		{
			if (componentsInParent[i] != this)
			{
				Object.Destroy(this);
				return;
			}
		}
		base.StartCoroutine(this.Setup());
	}

	// Token: 0x0600082C RID: 2092 RVA: 0x0004FB6D File Offset: 0x0004DD6D
	private IEnumerator Setup()
	{
		yield return new WaitForSeconds(0.1f);
		foreach (BoxCollider boxCollider in base.GetComponentsInChildren<BoxCollider>())
		{
			Vector3 halfExtents = boxCollider.size * 0.5f;
			halfExtents.x *= Mathf.Abs(boxCollider.transform.lossyScale.x);
			halfExtents.y *= Mathf.Abs(boxCollider.transform.lossyScale.y);
			halfExtents.z *= Mathf.Abs(boxCollider.transform.lossyScale.z);
			Collider[] array = Physics.OverlapBox(boxCollider.transform.TransformPoint(boxCollider.center), halfExtents, boxCollider.transform.rotation, LayerMask.GetMask(new string[]
			{
				"Other"
			}), QueryTriggerInteraction.Collide);
			for (int j = 0; j < array.Length; j++)
			{
				LevelPoint component = array[j].transform.GetComponent<LevelPoint>();
				if (component)
				{
					component.Room = this;
				}
			}
		}
		foreach (MeshCollider meshCollider in base.GetComponentsInChildren<MeshCollider>())
		{
			Collider[] array = Physics.OverlapBox(meshCollider.bounds.center, meshCollider.bounds.size, meshCollider.transform.rotation, LayerMask.GetMask(new string[]
			{
				"Other"
			}), QueryTriggerInteraction.Collide);
			for (int j = 0; j < array.Length; j++)
			{
				LevelPoint component2 = array[j].transform.GetComponent<LevelPoint>();
				if (component2)
				{
					component2.Room = this;
				}
			}
		}
		if (!this.Extraction && !this.Truck && !this.Module.StartRoom && !SemiFunc.RunIsShop())
		{
			foreach (BoxCollider boxCollider2 in base.GetComponentsInChildren<BoxCollider>())
			{
				Vector3 scale = boxCollider2.size * 0.5f;
				scale.x *= Mathf.Abs(boxCollider2.transform.lossyScale.x);
				scale.y *= Mathf.Abs(boxCollider2.transform.lossyScale.y);
				scale.z *= Mathf.Abs(boxCollider2.transform.lossyScale.z);
				Vector3 position = boxCollider2.transform.TransformPoint(boxCollider2.center);
				Vector3 euler = new Vector3(0f, boxCollider2.transform.rotation.eulerAngles.y, 0f);
				this.MapModule = Map.Instance.AddRoomVolume(base.gameObject, position, Quaternion.Euler(euler), scale, this.Module, null);
			}
			foreach (MeshCollider meshCollider2 in base.GetComponentsInChildren<MeshCollider>())
			{
				Vector3 scale2 = meshCollider2.transform.lossyScale * 0.5f;
				Vector3 position2 = meshCollider2.transform.position;
				Quaternion rotation = meshCollider2.transform.rotation;
				this.MapModule = Map.Instance.AddRoomVolume(base.gameObject, position2, rotation, scale2, this.Module, meshCollider2.sharedMesh);
			}
		}
		yield break;
	}

	// Token: 0x0600082D RID: 2093 RVA: 0x0004FB7C File Offset: 0x0004DD7C
	public void SetExplored()
	{
		if (this.Explored)
		{
			return;
		}
		this.Explored = true;
		if (this.MapModule)
		{
			this.MapModule.Hide();
		}
	}

	// Token: 0x04000F14 RID: 3860
	public bool Truck;

	// Token: 0x04000F15 RID: 3861
	public bool Extraction;

	// Token: 0x04000F16 RID: 3862
	public Color Color = Color.blue;

	// Token: 0x04000F17 RID: 3863
	[Space]
	public ReverbPreset ReverbPreset;

	// Token: 0x04000F18 RID: 3864
	public RoomAmbience RoomAmbience;

	// Token: 0x04000F19 RID: 3865
	public Module Module;

	// Token: 0x04000F1A RID: 3866
	public MapModule MapModule;

	// Token: 0x04000F1B RID: 3867
	private bool Explored;
}
