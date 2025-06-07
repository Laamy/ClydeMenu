using System;
using UnityEngine;

// Token: 0x020002BF RID: 703
public class UraniumScript : MonoBehaviour
{
	// Token: 0x0600161A RID: 5658 RVA: 0x000C1EE8 File Offset: 0x000C00E8
	private void Start()
	{
		this.physGrabObject = base.GetComponent<PhysGrabObject>();
		this.uraniumCloudParticles = base.GetComponentInChildren<ParticleSystem>();
	}

	// Token: 0x0600161B RID: 5659 RVA: 0x000C1F04 File Offset: 0x000C0104
	private void Update()
	{
		this.uraniumSoundLoop.PlayLoop(this.loopPlaying, 5f, 5f, 1f);
		if (this.physGrabObject.grabbed)
		{
			this.loopPlaying = true;
		}
		else
		{
			this.loopPlaying = false;
		}
		if (this.physGrabObject.grabbedLocal && this.uraniumIntensityOption == UraniumScript.UraniumIntensity.Big)
		{
			PostProcessing.Instance.VignetteOverride(new Color(0f, 0.6f, 0f), 1f, 0.5f, 0.1f, 2f, 0.1f, base.gameObject);
			return;
		}
		if (this.physGrabObject.grabbedLocal && this.uraniumIntensityOption == UraniumScript.UraniumIntensity.Small)
		{
			PostProcessing.Instance.VignetteOverride(new Color(0f, 0.6f, 0f), 0.5f, 0.5f, 0.33f, 2f, 0.1f, base.gameObject);
		}
	}

	// Token: 0x0600161C RID: 5660 RVA: 0x000C1FF4 File Offset: 0x000C01F4
	public void Cloud()
	{
		Object obj = Object.Instantiate<GameObject>(this.geigerLoopObject, base.transform.position, Quaternion.identity);
		this.uraniumCloudSound.Play(this.physGrabObject.centerPoint, 1f, 1f, 1f, 1f);
		this.uraniumCloudParticles.transform.parent = null;
		this.hurtCollider.transform.parent = null;
		this.uraniumCloudParticles.Play();
		this.hurtCollider.gameObject.SetActive(true);
		Object.Destroy(obj, 7f);
	}

	// Token: 0x0400264A RID: 9802
	public Sound uraniumSoundLoop;

	// Token: 0x0400264B RID: 9803
	public Sound uraniumCloudSound;

	// Token: 0x0400264C RID: 9804
	public GameObject geigerLoopObject;

	// Token: 0x0400264D RID: 9805
	private PhysGrabObject physGrabObject;

	// Token: 0x0400264E RID: 9806
	private ParticleSystem uraniumCloudParticles;

	// Token: 0x0400264F RID: 9807
	public HurtCollider hurtCollider;

	// Token: 0x04002650 RID: 9808
	private bool loopPlaying;

	// Token: 0x04002651 RID: 9809
	public UraniumScript.UraniumIntensity uraniumIntensityOption;

	// Token: 0x0200041A RID: 1050
	public enum UraniumIntensity
	{
		// Token: 0x04002DD7 RID: 11735
		Big,
		// Token: 0x04002DD8 RID: 11736
		Small,
		// Token: 0x04002DD9 RID: 11737
		None
	}
}
