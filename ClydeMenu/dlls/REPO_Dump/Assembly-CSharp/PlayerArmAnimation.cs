using System;
using UnityEngine;

// Token: 0x020001B1 RID: 433
public class PlayerArmAnimation : MonoBehaviour
{
	// Token: 0x06000ED9 RID: 3801 RVA: 0x00086054 File Offset: 0x00084254
	private void Start()
	{
		this.Player = PlayerController.instance;
		this.Voice = PlayerVoice.Instance;
		this.Animator = base.GetComponent<Animator>();
		this.Crouching = Animator.StringToHash("Crouching");
		this.Crawling = Animator.StringToHash("Crawling");
	}

	// Token: 0x06000EDA RID: 3802 RVA: 0x000860A4 File Offset: 0x000842A4
	private void Update()
	{
		if (this.Player.Crouching)
		{
			this.Animator.SetBool(this.Crouching, true);
		}
		else
		{
			this.Animator.SetBool(this.Crouching, false);
			this.Animator.SetBool(this.Crawling, false);
		}
		if (this.Player.Crawling)
		{
			this.Animator.SetBool(this.Crawling, true);
			return;
		}
		this.Animator.SetBool(this.Crawling, false);
	}

	// Token: 0x06000EDB RID: 3803 RVA: 0x00086128 File Offset: 0x00084328
	public void PlayCrouchHush()
	{
	}

	// Token: 0x06000EDC RID: 3804 RVA: 0x0008612A File Offset: 0x0008432A
	public void PlayMoveShort()
	{
		this.MoveShort.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x06000EDD RID: 3805 RVA: 0x00086157 File Offset: 0x00084357
	public void PlayMoveLong()
	{
		this.MoveLong.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x0400186A RID: 6250
	private PlayerController Player;

	// Token: 0x0400186B RID: 6251
	private Animator Animator;

	// Token: 0x0400186C RID: 6252
	private int Crouching;

	// Token: 0x0400186D RID: 6253
	private int Crawling;

	// Token: 0x0400186E RID: 6254
	private PlayerVoice Voice;

	// Token: 0x0400186F RID: 6255
	public Sound MoveShort;

	// Token: 0x04001870 RID: 6256
	public Sound MoveLong;
}
