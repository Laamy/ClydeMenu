using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x0200019E RID: 414
public class NotValuableObject : MonoBehaviour
{
	// Token: 0x06000DC7 RID: 3527 RVA: 0x00078438 File Offset: 0x00076638
	private void Start()
	{
		this.physGrabObject = base.GetComponent<PhysGrabObject>();
		this.navMeshObstacle = base.GetComponent<NavMeshObstacle>();
		if (this.navMeshObstacle)
		{
			Debug.LogError(base.gameObject.name + " has a NavMeshObstacle component. Please remove it.", base.gameObject);
		}
		base.StartCoroutine(this.EnableRigidbody());
		this.rb = base.GetComponent<Rigidbody>();
		if (this.rb)
		{
			this.rb.mass = this.physAttributePreset.mass;
		}
		if (this.physGrabObject)
		{
			this.physGrabObject.massOriginal = this.physAttributePreset.mass;
		}
		if (this.hasHealth)
		{
			this.healthCurrent = this.healthMax;
		}
	}

	// Token: 0x06000DC8 RID: 3528 RVA: 0x00078500 File Offset: 0x00076700
	public void Impact(PhysGrabObjectImpactDetector.ImpactState _impactState)
	{
		switch (_impactState)
		{
		case PhysGrabObjectImpactDetector.ImpactState.Light:
			this.healthCurrent -= this.healthLossOnBreakLight;
			break;
		case PhysGrabObjectImpactDetector.ImpactState.Medium:
			this.healthCurrent -= this.healthLossOnBreakMedium;
			break;
		case PhysGrabObjectImpactDetector.ImpactState.Heavy:
			this.healthCurrent -= this.healthLossOnBreakHeavy;
			break;
		}
		if (this.healthCurrent <= 0)
		{
			this.physGrabObject.impactDetector.DestroyObject(true);
		}
	}

	// Token: 0x06000DC9 RID: 3529 RVA: 0x0007857A File Offset: 0x0007677A
	private IEnumerator EnableRigidbody()
	{
		while (!LevelGenerator.Instance.Generated)
		{
			yield return new WaitForSeconds(0.1f);
		}
		PhysGrabObject component = base.GetComponent<PhysGrabObject>();
		if (!component)
		{
			yield return new WaitForSeconds(0.5f);
			yield return new WaitForFixedUpdate();
		}
		else
		{
			component.spawned = true;
		}
		yield break;
	}

	// Token: 0x04001639 RID: 5689
	public PhysAttribute physAttributePreset;

	// Token: 0x0400163A RID: 5690
	public PhysAudio audioPreset;

	// Token: 0x0400163B RID: 5691
	public Durability durabilityPreset;

	// Token: 0x0400163C RID: 5692
	public Gradient particleColors;

	// Token: 0x0400163D RID: 5693
	[Range(0.5f, 3f)]
	public float audioPresetPitch = 1f;

	// Token: 0x0400163E RID: 5694
	private NavMeshObstacle navMeshObstacle;

	// Token: 0x0400163F RID: 5695
	private PhysGrabObject physGrabObject;

	// Token: 0x04001640 RID: 5696
	private Rigidbody rb;

	// Token: 0x04001641 RID: 5697
	[Space]
	public bool hasHealth;

	// Token: 0x04001642 RID: 5698
	private int healthCurrent;

	// Token: 0x04001643 RID: 5699
	public int healthMax;

	// Token: 0x04001644 RID: 5700
	public int healthLossOnBreakLight;

	// Token: 0x04001645 RID: 5701
	public int healthLossOnBreakMedium;

	// Token: 0x04001646 RID: 5702
	public int healthLossOnBreakHeavy;
}
