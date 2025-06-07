using System;
using System.Collections;
using UnityEngine;

// Token: 0x020001B0 RID: 432
public class ArmIntroController : MonoBehaviour
{
	// Token: 0x06000ED0 RID: 3792 RVA: 0x00085ED9 File Offset: 0x000840D9
	public void Start()
	{
		this.Animator.enabled = false;
		base.transform.parent = this.CameraTransform;
		this.Hide.SetActive(false);
		base.StartCoroutine(this.StartIntro());
	}

	// Token: 0x06000ED1 RID: 3793 RVA: 0x00085F11 File Offset: 0x00084111
	public void Update()
	{
		PlayerController.instance.CrouchDisable(0.1f);
	}

	// Token: 0x06000ED2 RID: 3794 RVA: 0x00085F22 File Offset: 0x00084122
	private IEnumerator StartIntro()
	{
		while (GameDirector.instance.currentState != GameDirector.gameState.Main)
		{
			yield return null;
		}
		if (this.DebugDisable)
		{
			Object.Destroy(base.gameObject);
		}
		else
		{
			yield return new WaitForSeconds(this.WaitTimer);
			this.Animator.enabled = true;
			this.Hide.SetActive(true);
		}
		yield break;
	}

	// Token: 0x06000ED3 RID: 3795 RVA: 0x00085F31 File Offset: 0x00084131
	public void AnimationDone()
	{
		Object.Destroy(base.gameObject);
	}

	// Token: 0x06000ED4 RID: 3796 RVA: 0x00085F40 File Offset: 0x00084140
	public void PlayGlovePull()
	{
		this.GlovePull.Play(base.transform.position, 1f, 1f, 1f, 1f);
		GameDirector.instance.CameraImpact.Shake(0.25f, 1f);
	}

	// Token: 0x06000ED5 RID: 3797 RVA: 0x00085F94 File Offset: 0x00084194
	public void PlayGloveSnap()
	{
		this.GloveSnap.Play(base.transform.position, 1f, 1f, 1f, 1f);
		GameDirector.instance.CameraImpact.Shake(1f, 0.1f);
	}

	// Token: 0x06000ED6 RID: 3798 RVA: 0x00085FE5 File Offset: 0x000841E5
	public void PlayMoveShort()
	{
		this.MoveShort.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x06000ED7 RID: 3799 RVA: 0x00086012 File Offset: 0x00084212
	public void PlayMoveLong()
	{
		this.MoveLong.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x04001861 RID: 6241
	public bool DebugDisable;

	// Token: 0x04001862 RID: 6242
	[Space]
	public Animator Animator;

	// Token: 0x04001863 RID: 6243
	public Transform CameraTransform;

	// Token: 0x04001864 RID: 6244
	public GameObject Hide;

	// Token: 0x04001865 RID: 6245
	[Space]
	public float WaitTimer = 0.25f;

	// Token: 0x04001866 RID: 6246
	[Space]
	public Sound MoveShort;

	// Token: 0x04001867 RID: 6247
	public Sound MoveLong;

	// Token: 0x04001868 RID: 6248
	public Sound GlovePull;

	// Token: 0x04001869 RID: 6249
	public Sound GloveSnap;
}
