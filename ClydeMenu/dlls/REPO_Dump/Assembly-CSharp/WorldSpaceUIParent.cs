using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000299 RID: 665
public class WorldSpaceUIParent : MonoBehaviour
{
	// Token: 0x060014D4 RID: 5332 RVA: 0x000B8379 File Offset: 0x000B6579
	private void Awake()
	{
		WorldSpaceUIParent.instance = this;
		this.canvasGroup = base.GetComponent<CanvasGroup>();
	}

	// Token: 0x060014D5 RID: 5333 RVA: 0x000B8390 File Offset: 0x000B6590
	private void Update()
	{
		float b = 1f;
		if (this.hideTimer > 0f)
		{
			b = 0f;
			this.hideTimer -= Time.deltaTime;
		}
		this.hideAlpha = Mathf.Lerp(this.hideAlpha, b, Time.deltaTime * 20f);
		this.canvasGroup.alpha = this.hideAlpha;
	}

	// Token: 0x060014D6 RID: 5334 RVA: 0x000B83F8 File Offset: 0x000B65F8
	public void ValueLostCreate(Vector3 _worldPosition, int _value)
	{
		if (PlayerController.instance.isActiveAndEnabled && Vector3.Distance(_worldPosition, PlayerController.instance.transform.position) > 10f)
		{
			return;
		}
		foreach (WorldSpaceUIValueLost worldSpaceUIValueLost in this.valueLostList)
		{
			if (Vector3.Distance(worldSpaceUIValueLost.worldPosition, _worldPosition) < 1f && worldSpaceUIValueLost.timer > 0f)
			{
				worldSpaceUIValueLost.timer = 0f;
				_value += worldSpaceUIValueLost.value;
			}
		}
		WorldSpaceUIValueLost component = Object.Instantiate<GameObject>(this.valueLostPrefab, base.transform.position, base.transform.rotation, base.transform).GetComponent<WorldSpaceUIValueLost>();
		component.worldPosition = _worldPosition;
		component.value = _value;
		this.valueLostList.Add(component);
	}

	// Token: 0x060014D7 RID: 5335 RVA: 0x000B84EC File Offset: 0x000B66EC
	public void Hide()
	{
		this.hideTimer = 0.1f;
	}

	// Token: 0x060014D8 RID: 5336 RVA: 0x000B84FC File Offset: 0x000B66FC
	public void TTS(PlayerAvatar _player, string _text, float _time)
	{
		if (GameDirector.instance.currentState != GameDirector.gameState.Main)
		{
			return;
		}
		if (_player.isDisabled || _player.isLocal)
		{
			return;
		}
		WorldSpaceUITTS component = Object.Instantiate<GameObject>(this.TTSPrefab, base.transform.position, base.transform.rotation, base.transform).GetComponent<WorldSpaceUITTS>();
		component.text.text = _text;
		component.playerAvatar = _player;
		component.followTransform = _player.playerAvatarVisuals.TTSTransform;
		component.worldPosition = component.followTransform.position;
		component.followPosition = component.worldPosition;
		component.wordTime = _time;
		component.ttsVoice = _player.voiceChat.ttsVoice;
	}

	// Token: 0x060014D9 RID: 5337 RVA: 0x000B85AC File Offset: 0x000B67AC
	public void PlayerName(PlayerAvatar _player)
	{
		if (_player.isLocal || SemiFunc.MenuLevel())
		{
			return;
		}
		WorldSpaceUIPlayerName component = Object.Instantiate<GameObject>(this.playerNamePrefab, base.transform.position, base.transform.rotation, base.transform).GetComponent<WorldSpaceUIPlayerName>();
		component.playerAvatar = _player;
		component.text.text = _player.playerName;
		_player.worldSpaceUIPlayerName = component;
	}

	// Token: 0x040023EA RID: 9194
	public static WorldSpaceUIParent instance;

	// Token: 0x040023EB RID: 9195
	internal CanvasGroup canvasGroup;

	// Token: 0x040023EC RID: 9196
	[Space]
	public GameObject valueLostPrefab;

	// Token: 0x040023ED RID: 9197
	public GameObject TTSPrefab;

	// Token: 0x040023EE RID: 9198
	public GameObject playerNamePrefab;

	// Token: 0x040023EF RID: 9199
	internal List<WorldSpaceUIValueLost> valueLostList = new List<WorldSpaceUIValueLost>();

	// Token: 0x040023F0 RID: 9200
	private float hideTimer;

	// Token: 0x040023F1 RID: 9201
	internal float hideAlpha = 1f;
}
