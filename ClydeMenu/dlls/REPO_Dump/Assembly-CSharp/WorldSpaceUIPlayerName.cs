using System;
using TMPro;
using UnityEngine;

// Token: 0x0200029A RID: 666
public class WorldSpaceUIPlayerName : WorldSpaceUIChild
{
	// Token: 0x060014DB RID: 5339 RVA: 0x000B8633 File Offset: 0x000B6833
	private void OnDisable()
	{
		this.text.color = new Color(1f, 1f, 1f, 0f);
	}

	// Token: 0x060014DC RID: 5340 RVA: 0x000B865C File Offset: 0x000B685C
	protected override void Update()
	{
		base.Update();
		if (!this.playerAvatar)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		if (this.showTimeTotalResetTimer > 0f)
		{
			this.showTimeTotalResetTimer -= Time.deltaTime;
			if (this.showTimeTotalResetTimer <= 0f)
			{
				this.showTimeTotal = 0f;
			}
		}
		if (SpectateCamera.instance || this.playerAvatar.isDisabled || this.showTimer <= 0f)
		{
			this.text.color = Color.Lerp(this.text.color, new Color(1f, 1f, 1f, 0f), Time.deltaTime * 20f);
		}
		else
		{
			this.showTimer -= Time.deltaTime;
			this.text.color = Color.Lerp(this.text.color, new Color(1f, 1f, 1f, 0.5f), Time.deltaTime * 5f);
		}
		Vector3 position = this.playerAvatar.playerAvatarVisuals.headLookAtTransform.position;
		position.y = this.playerAvatar.playerAvatarVisuals.transform.position.y;
		if (this.playerAvatar == SessionManager.instance.CrownedPlayerGet())
		{
			position.y += 0.02f;
		}
		this.followTarget = Vector3.Lerp(this.followTarget, position, Time.deltaTime * 30f);
		float num = this.playerAvatar.playerAvatarVisuals.headLookAtTransform.position.y - this.playerAvatar.playerAvatarVisuals.transform.position.y + 0.35f;
		if (Mathf.Abs(this.followTargetY - num) > 0.2f)
		{
			this.followTargetY = Mathf.Lerp(this.followTargetY, num, Time.deltaTime * 20f);
		}
		else
		{
			this.followTargetY = Mathf.Lerp(this.followTargetY, num, Time.deltaTime * 3f);
		}
		this.worldPosition = this.followTarget + Vector3.up * this.followTargetY;
		float num2 = Vector3.Distance(this.worldPosition, Camera.main.transform.position);
		this.text.fontSize = 20f - num2;
	}

	// Token: 0x060014DD RID: 5341 RVA: 0x000B88CC File Offset: 0x000B6ACC
	public void Show()
	{
		this.showTimeTotal += 0.25f;
		this.showTimeTotalResetTimer = 0.5f;
		if (this.showTimeTotal >= 1f)
		{
			this.showTimer = 0.5f;
		}
	}

	// Token: 0x040023F2 RID: 9202
	public TextMeshProUGUI text;

	// Token: 0x040023F3 RID: 9203
	internal PlayerAvatar playerAvatar;

	// Token: 0x040023F4 RID: 9204
	private Vector3 followTarget;

	// Token: 0x040023F5 RID: 9205
	private float followTargetY;

	// Token: 0x040023F6 RID: 9206
	private float showTimer;

	// Token: 0x040023F7 RID: 9207
	private float showTimeTotal;

	// Token: 0x040023F8 RID: 9208
	private float showTimeTotalResetTimer;
}
