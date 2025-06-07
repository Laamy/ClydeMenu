using System;
using System.Collections;
using UnityEngine;

// Token: 0x0200018E RID: 398
public class DirtFinderMapDoor : MonoBehaviour
{
	// Token: 0x06000D85 RID: 3461 RVA: 0x000767B7 File Offset: 0x000749B7
	public void Start()
	{
		this.Hinge = base.GetComponent<PhysGrabHinge>();
		base.StartCoroutine(this.Logic());
	}

	// Token: 0x06000D86 RID: 3462 RVA: 0x000767D2 File Offset: 0x000749D2
	private IEnumerator Logic()
	{
		while (!LevelGenerator.Instance.Generated)
		{
			yield return new WaitForSeconds(0.1f);
		}
		this.MapObject = Map.Instance.AddDoor(this, this.DoorPrefab);
		while (!this.Hinge.broken)
		{
			yield return new WaitForSeconds(1f);
		}
		Object.Destroy(this.MapObject);
		yield break;
	}

	// Token: 0x040015B7 RID: 5559
	public Transform Target;

	// Token: 0x040015B8 RID: 5560
	public GameObject DoorPrefab;

	// Token: 0x040015B9 RID: 5561
	public PhysGrabHinge Hinge;

	// Token: 0x040015BA RID: 5562
	private GameObject MapObject;
}
