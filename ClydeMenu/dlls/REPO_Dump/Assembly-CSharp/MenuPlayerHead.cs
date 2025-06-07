using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000200 RID: 512
public class MenuPlayerHead : MonoBehaviour
{
	// Token: 0x06001167 RID: 4455 RVA: 0x0009E608 File Offset: 0x0009C808
	private void Start()
	{
		this.playerListed = base.GetComponentInParent<MenuPlayerListed>();
		this.allRawImagesInChildren.AddRange(base.GetComponentsInChildren<RawImage>());
		this.playerListedTransform = this.playerListed.GetComponent<RectTransform>();
		MenuManager.instance.PlayerHeadAdd(this);
		this.rectTransform = base.GetComponent<RectTransform>();
		this.startedTalkingAtTime = Time.time;
		this.muteIconTransform.localScale = Vector3.zero;
		this.muteScalePrevious = this.muteIconTransform.localScale;
	}

	// Token: 0x06001168 RID: 4456 RVA: 0x0009E688 File Offset: 0x0009C888
	public void SetColor(Color color)
	{
		foreach (RawImage rawImage in this.allRawImagesInChildren)
		{
			rawImage.color = color;
		}
		this.muteIcon.color = Color.Lerp(color, Color.white, 0.5f);
	}

	// Token: 0x06001169 RID: 4457 RVA: 0x0009E6F4 File Offset: 0x0009C8F4
	private void HeadRight()
	{
		this.headRight.gameObject.SetActive(true);
		this.headLeft.gameObject.SetActive(false);
		this.eyesTransform = this.headRight.Find("Eyes").GetComponent<RectTransform>();
		this.left = false;
		this.right = true;
		this.headTransform = this.headRight.GetComponent<RectTransform>();
	}

	// Token: 0x0600116A RID: 4458 RVA: 0x0009E760 File Offset: 0x0009C960
	private void HeadLeft()
	{
		this.headRight.gameObject.SetActive(false);
		this.headLeft.gameObject.SetActive(true);
		this.eyesTransform = this.headLeft.Find("Eyes").GetComponent<RectTransform>();
		this.left = true;
		this.right = false;
		this.headTransform = this.headLeft.GetComponent<RectTransform>();
	}

	// Token: 0x0600116B RID: 4459 RVA: 0x0009E7CC File Offset: 0x0009C9CC
	private void Update()
	{
		if (SemiFunc.IsMultiplayer())
		{
			if (this.playerAvatar)
			{
				this.isTalkingPrev = this.isTalking;
				if (this.playerAvatar.voiceChatFetched)
				{
					this.isTalking = this.playerAvatar.voiceChat.isTalking;
				}
			}
			else
			{
				this.playerAvatar = this.playerListed.playerAvatar;
			}
		}
		if (MenuManager.instance.currentMenuPageIndex == MenuPageIndex.Lobby)
		{
			this.myFocusPoint.localPosition = MenuCursor.instance.transform.localPosition - this.rectTransform.parent.parent.localPosition;
			this.myFocusPoint.localPosition = new Vector3(this.myFocusPoint.localPosition.x + 18f, this.myFocusPoint.localPosition.y + 15f, 0f);
		}
		if (this.playerAvatar.voiceChatFetched && this.muteShow != this.playerAvatar.voiceChat.toggleMute)
		{
			this.muteShow = this.playerAvatar.voiceChat.toggleMute;
			this.muteAnimationLerp = 0f;
			this.muteScalePrevious = this.muteIconTransform.localScale;
		}
		if (this.muteShow)
		{
			if (this.muteAnimationLerp < 1f)
			{
				this.muteAnimationLerp += Time.deltaTime * 5f;
				this.muteIconTransform.localScale = Vector3.LerpUnclamped(this.muteScalePrevious, Vector3.one, this.muteIntroCurve.Evaluate(this.muteAnimationLerp));
			}
		}
		else if (this.muteAnimationLerp < 1f)
		{
			this.muteAnimationLerp += Time.deltaTime * 10f;
			this.muteIconTransform.localScale = Vector3.LerpUnclamped(this.muteScalePrevious, Vector3.zero, this.muteOutroCurve.Evaluate(this.muteAnimationLerp));
		}
		if (!this.isWinnerHead && this.headTransform)
		{
			this.focusPoint.localPosition = this.playerListedTransform.localPosition + this.rectTransform.localPosition + this.headTransform.localPosition * this.rectTransform.localScale.x;
			float length = 12.5f;
			if (this.left)
			{
				length = -12.5f;
			}
			this.focusPoint.localPosition += new Vector3(MenuPlayerHead.LengthDirX(length, this.headTransform.localEulerAngles.z), MenuPlayerHead.LengthDirY(length, this.headTransform.localEulerAngles.z), 0f);
			length = 6f;
			this.focusPoint.localPosition += new Vector3(MenuPlayerHead.LengthDirX(length, this.headTransform.localEulerAngles.z + 90f), MenuPlayerHead.LengthDirY(length, this.headTransform.localEulerAngles.z + 90f), 0f);
		}
		if (this.isTalking != this.isTalkingPrev)
		{
			if (this.isTalking)
			{
				this.startedTalkingAtTime = Time.time;
			}
			this.isTalkingPrev = this.isTalking;
		}
		if (!this.isWinnerHead)
		{
			this.listSpot = this.playerListed.listSpot;
			if (this.listSpot != this.listSpotPrev)
			{
				if (this.listSpot % 2 == 0)
				{
					this.HeadRight();
				}
				else
				{
					this.HeadLeft();
				}
				this.listSpotPrev = this.listSpot;
			}
		}
		if (!this.isWinnerHead)
		{
			MenuPlayerHead menuPlayerHead = null;
			float num = 0f;
			foreach (MenuPlayerHead menuPlayerHead2 in MenuManager.instance.playerHeads)
			{
				if (!(menuPlayerHead2 == this) && menuPlayerHead2.isTalking)
				{
					float num2 = menuPlayerHead2.startedTalkingAtTime;
					if (num2 > num)
					{
						num = num2;
						menuPlayerHead = menuPlayerHead2;
					}
				}
			}
			if (menuPlayerHead)
			{
				float d = 10f;
				Vector3 vector = menuPlayerHead.focusPoint.localPosition - this.focusPoint.localPosition;
				vector.z = 0f;
				Vector3 b = new Vector3(50f, 25f, 0f) + vector.normalized * d;
				if (this.left)
				{
					b = new Vector3(-50f, 25f, 0f) + vector.normalized * d;
				}
				this.eyesTransform.localPosition = Vector3.Lerp(this.eyesTransform.localPosition, b, Time.deltaTime * 10f);
			}
			else
			{
				float d2 = 10f;
				Vector3 vector2 = this.myFocusPoint.localPosition - this.focusPoint.localPosition;
				vector2.z = 0f;
				Vector3 b2 = new Vector3(50f, 25f, 0f) + vector2.normalized * d2;
				if (this.left)
				{
					b2 = new Vector3(-50f, 25f, 0f) + vector2.normalized * d2;
				}
				this.eyesTransform.localPosition = Vector3.Lerp(this.eyesTransform.localPosition, b2, Time.deltaTime * 10f);
			}
		}
		if (this.playerAvatar && this.playerAvatar.voiceChatFetched)
		{
			if (this.left)
			{
				float clipLoudness = this.playerAvatar.voiceChat.clipLoudness;
				this.headLeft.localEulerAngles = new Vector3(0f, 0f, -clipLoudness * 200f);
			}
			if (this.right)
			{
				float clipLoudness2 = this.playerAvatar.voiceChat.clipLoudness;
				this.headRight.localEulerAngles = new Vector3(0f, 0f, clipLoudness2 * 200f);
			}
		}
	}

	// Token: 0x0600116C RID: 4460 RVA: 0x0009EDDC File Offset: 0x0009CFDC
	public void SetPlayer(PlayerAvatar player)
	{
		this.playerAvatar = player;
		if (this.allRawImagesInChildren.Count == 0)
		{
			this.allRawImagesInChildren.AddRange(base.GetComponentsInChildren<RawImage>());
		}
		if (this.playerAvatar && this.playerAvatar.playerAvatarVisuals)
		{
			this.SetColor(this.playerAvatar.playerAvatarVisuals.color);
		}
	}

	// Token: 0x0600116D RID: 4461 RVA: 0x0009EE43 File Offset: 0x0009D043
	private void OnDestroy()
	{
		MenuManager.instance.PlayerHeadRemove(this);
		Object.Destroy(this.focusPoint.gameObject);
		Object.Destroy(this.myFocusPoint.gameObject);
	}

	// Token: 0x0600116E RID: 4462 RVA: 0x0009EE70 File Offset: 0x0009D070
	public static float LengthDirX(float length, float direction)
	{
		return length * Mathf.Cos(direction * 0.017453292f);
	}

	// Token: 0x0600116F RID: 4463 RVA: 0x0009EE80 File Offset: 0x0009D080
	public static float LengthDirY(float length, float direction)
	{
		return length * Mathf.Sin(direction * 0.017453292f);
	}

	// Token: 0x04001D67 RID: 7527
	internal PlayerAvatar playerAvatar;

	// Token: 0x04001D68 RID: 7528
	public Transform headRight;

	// Token: 0x04001D69 RID: 7529
	public Transform headLeft;

	// Token: 0x04001D6A RID: 7530
	private RectTransform eyesTransform;

	// Token: 0x04001D6B RID: 7531
	private MenuPlayerListed playerListed;

	// Token: 0x04001D6C RID: 7532
	private int listSpotPrev = -1;

	// Token: 0x04001D6D RID: 7533
	private int listSpot;

	// Token: 0x04001D6E RID: 7534
	private bool left;

	// Token: 0x04001D6F RID: 7535
	private bool right = true;

	// Token: 0x04001D70 RID: 7536
	private List<RawImage> allRawImagesInChildren = new List<RawImage>();

	// Token: 0x04001D71 RID: 7537
	private Vector3 eyesStartPosOriginal;

	// Token: 0x04001D72 RID: 7538
	private Vector3 eyesStartPos;

	// Token: 0x04001D73 RID: 7539
	private bool isTalkingPrev;

	// Token: 0x04001D74 RID: 7540
	internal bool isTalking;

	// Token: 0x04001D75 RID: 7541
	public RectTransform focusPoint;

	// Token: 0x04001D76 RID: 7542
	public RectTransform myFocusPoint;

	// Token: 0x04001D77 RID: 7543
	private RectTransform playerListedTransform;

	// Token: 0x04001D78 RID: 7544
	private RectTransform rectTransform;

	// Token: 0x04001D79 RID: 7545
	internal RectTransform headTransform;

	// Token: 0x04001D7A RID: 7546
	internal float startedTalkingAtTime;

	// Token: 0x04001D7B RID: 7547
	public bool isWinnerHead;

	// Token: 0x04001D7C RID: 7548
	[Space]
	public RectTransform muteIconTransform;

	// Token: 0x04001D7D RID: 7549
	public Image muteIcon;

	// Token: 0x04001D7E RID: 7550
	public AnimationCurve muteIntroCurve;

	// Token: 0x04001D7F RID: 7551
	public AnimationCurve muteOutroCurve;

	// Token: 0x04001D80 RID: 7552
	private float muteAnimationLerp;

	// Token: 0x04001D81 RID: 7553
	private bool muteShow;

	// Token: 0x04001D82 RID: 7554
	private Vector3 muteScalePrevious;
}
