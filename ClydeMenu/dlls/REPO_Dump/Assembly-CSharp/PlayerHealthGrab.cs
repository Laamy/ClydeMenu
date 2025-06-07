using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x020001CE RID: 462
public class PlayerHealthGrab : MonoBehaviour
{
	// Token: 0x06000FF2 RID: 4082 RVA: 0x00092DDE File Offset: 0x00090FDE
	private void Start()
	{
		this.physCollider = base.GetComponent<Collider>();
		this.staticGrabObject = base.GetComponent<StaticGrabObject>();
		if (this.playerAvatar.isLocal)
		{
			this.staticGrabObject.enabled = false;
		}
	}

	// Token: 0x06000FF3 RID: 4083 RVA: 0x00092E14 File Offset: 0x00091014
	private void Update()
	{
		if (this.playerAvatar.isTumbling || SemiFunc.RunIsShop() || SemiFunc.RunIsArena())
		{
			if (this.hideLerp < 1f)
			{
				this.hideLerp += Time.deltaTime * 5f;
				this.hideLerp = Mathf.Clamp(this.hideLerp, 0f, 1f);
				this.hideTransform.localScale = new Vector3(1f, this.hideCurve.Evaluate(this.hideLerp), 1f);
				if (this.hideLerp >= 1f)
				{
					this.hideTransform.gameObject.SetActive(false);
				}
			}
		}
		else if (this.hideLerp > 0f)
		{
			if (!this.hideTransform.gameObject.activeSelf)
			{
				this.hideTransform.gameObject.SetActive(true);
			}
			this.hideLerp -= Time.deltaTime * 2f;
			this.hideLerp = Mathf.Clamp(this.hideLerp, 0f, 1f);
			this.hideTransform.localScale = new Vector3(1f, this.hideCurve.Evaluate(this.hideLerp), 1f);
		}
		bool flag = true;
		if (this.playerAvatar.isDisabled || this.hideLerp > 0f)
		{
			flag = false;
		}
		if (this.colliderActive != flag)
		{
			this.colliderActive = flag;
			this.physCollider.enabled = this.colliderActive;
		}
		base.transform.position = this.followTransform.position;
		base.transform.rotation = this.followTransform.rotation;
		if (this.colliderActive && (!GameManager.Multiplayer() || PhotonNetwork.IsMasterClient))
		{
			if (this.staticGrabObject.playerGrabbing.Count > 0)
			{
				this.grabbingTimer += Time.deltaTime;
				foreach (PhysGrabber physGrabber in this.staticGrabObject.playerGrabbing)
				{
					if (this.grabbingTimer >= 1f)
					{
						PlayerAvatar playerAvatar = physGrabber.playerAvatar;
						if (this.playerAvatar.playerHealth.health != this.playerAvatar.playerHealth.maxHealth && playerAvatar.playerHealth.health > 10)
						{
							this.playerAvatar.playerHealth.HealOther(10, true);
							playerAvatar.playerHealth.HurtOther(10, Vector3.zero, false, -1);
							playerAvatar.HealedOther();
						}
					}
				}
				if (this.grabbingTimer >= 1f)
				{
					this.grabbingTimer = 0f;
					return;
				}
			}
			else
			{
				this.grabbingTimer = 0f;
			}
		}
	}

	// Token: 0x04001B25 RID: 6949
	public Transform followTransform;

	// Token: 0x04001B26 RID: 6950
	public Transform hideTransform;

	// Token: 0x04001B27 RID: 6951
	public PlayerAvatar playerAvatar;

	// Token: 0x04001B28 RID: 6952
	internal StaticGrabObject staticGrabObject;

	// Token: 0x04001B29 RID: 6953
	private Collider physCollider;

	// Token: 0x04001B2A RID: 6954
	private bool colliderActive = true;

	// Token: 0x04001B2B RID: 6955
	private float grabbingTimer;

	// Token: 0x04001B2C RID: 6956
	[Space]
	public AnimationCurve hideCurve;

	// Token: 0x04001B2D RID: 6957
	private float hideLerp;
}
