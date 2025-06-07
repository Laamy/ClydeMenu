using System;
using System.Collections;
using UnityEngine;

// Token: 0x020002CD RID: 717
public class ValuableVolume : MonoBehaviour
{
	// Token: 0x0600166A RID: 5738 RVA: 0x000C5E90 File Offset: 0x000C4090
	private void Start()
	{
		this.Module = base.GetComponentInParent<Module>();
	}

	// Token: 0x0600166B RID: 5739 RVA: 0x000C5EA0 File Offset: 0x000C40A0
	public void Setup()
	{
		ValuablePropSwitch componentInParent = base.GetComponentInParent<ValuablePropSwitch>();
		if (componentInParent && base.transform.parent != componentInParent.ValuableParent.transform)
		{
			Debug.LogError("Valuable Volume: Child of ValuablePropSwitch but not valuable parent...", base.gameObject);
		}
		if (!base.isActiveAndEnabled)
		{
			return;
		}
		bool flag = true;
		if (Debug.isDebugBuild)
		{
			base.StartCoroutine(this.SafetyCheck());
			flag = false;
		}
		foreach (Collider collider in Physics.OverlapSphere(base.transform.position, 2f))
		{
			if (collider.gameObject.CompareTag("Phys Grab Object"))
			{
				ValuableObject componentInParent2 = collider.transform.GetComponentInParent<ValuableObject>();
				if (componentInParent2 && componentInParent2.volumeType == this.VolumeType && Vector3.Distance(componentInParent2.transform.position, base.transform.position) < 0.1f)
				{
					componentInParent2.transform.parent = base.transform.parent;
					break;
				}
			}
		}
		if (flag)
		{
			Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x0600166C RID: 5740 RVA: 0x000C5FB9 File Offset: 0x000C41B9
	private IEnumerator SafetyCheck()
	{
		while (!LevelGenerator.Instance.Generated)
		{
			yield return null;
		}
		Mesh mesh = null;
		switch (this.VolumeType)
		{
		case ValuableVolume.Type.Tiny:
			mesh = AssetManager.instance.valuableMeshTiny;
			break;
		case ValuableVolume.Type.Small:
			mesh = AssetManager.instance.valuableMeshSmall;
			break;
		case ValuableVolume.Type.Medium:
			mesh = AssetManager.instance.valuableMeshMedium;
			break;
		case ValuableVolume.Type.Big:
			mesh = AssetManager.instance.valuableMeshBig;
			break;
		case ValuableVolume.Type.Wide:
			mesh = AssetManager.instance.valuableMeshWide;
			break;
		case ValuableVolume.Type.Tall:
			mesh = AssetManager.instance.valuableMeshTall;
			break;
		case ValuableVolume.Type.VeryTall:
			mesh = AssetManager.instance.valuableMeshVeryTall;
			break;
		}
		Vector3 size = mesh.bounds.size;
		Collider[] array = Physics.OverlapBox(base.transform.position + base.transform.forward * size.z / 2f + base.transform.up * size.y / 2f + Vector3.up * 0.01f, size / 2f, base.transform.rotation, LayerMask.GetMask(new string[]
		{
			"Default"
		}), QueryTriggerInteraction.Ignore);
		if (array.Length != 0)
		{
			string text = "not found";
			Module componentInParent = base.GetComponentInParent<Module>();
			if (componentInParent)
			{
				text = componentInParent.gameObject.name;
			}
			Debug.LogError("Valuable Volume: Overlapping colliders:", base.gameObject);
			Debug.LogError("     Volume Module: " + text, componentInParent.gameObject);
			foreach (Collider collider in array)
			{
				text = "not found";
				componentInParent = collider.gameObject.GetComponentInParent<Module>();
				if (componentInParent)
				{
					text = componentInParent.gameObject.name;
				}
				Debug.LogError("          Collider: " + collider.gameObject.name, collider.gameObject);
				Debug.LogError("          Collider Module: " + text, componentInParent.gameObject);
				Debug.LogError(" ");
			}
		}
		yield break;
	}

	// Token: 0x040026AE RID: 9902
	public ValuableVolume.Type VolumeType;

	// Token: 0x040026AF RID: 9903
	[HideInInspector]
	public Module Module;

	// Token: 0x040026B0 RID: 9904
	private Mesh MeshTiny;

	// Token: 0x040026B1 RID: 9905
	private Mesh MeshSmall;

	// Token: 0x040026B2 RID: 9906
	private Mesh MeshMedium;

	// Token: 0x040026B3 RID: 9907
	private Mesh MeshBig;

	// Token: 0x040026B4 RID: 9908
	private Mesh MeshWide;

	// Token: 0x040026B5 RID: 9909
	private Mesh MeshTall;

	// Token: 0x040026B6 RID: 9910
	private Mesh MeshVeryTall;

	// Token: 0x0200041E RID: 1054
	public enum Type
	{
		// Token: 0x04002DE8 RID: 11752
		Tiny,
		// Token: 0x04002DE9 RID: 11753
		Small,
		// Token: 0x04002DEA RID: 11754
		Medium,
		// Token: 0x04002DEB RID: 11755
		Big,
		// Token: 0x04002DEC RID: 11756
		Wide,
		// Token: 0x04002DED RID: 11757
		Tall,
		// Token: 0x04002DEE RID: 11758
		VeryTall
	}
}
