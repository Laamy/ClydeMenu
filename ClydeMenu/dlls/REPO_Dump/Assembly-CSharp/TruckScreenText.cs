using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x020000F3 RID: 243
public class TruckScreenText : MonoBehaviour
{
	// Token: 0x06000871 RID: 2161 RVA: 0x000519AA File Offset: 0x0004FBAA
	private void Awake()
	{
		TruckScreenText.instance = this;
	}

	// Token: 0x06000872 RID: 2162 RVA: 0x000519B4 File Offset: 0x0004FBB4
	private void Start()
	{
		this.truckScreenLocked = base.GetComponentInChildren<TuckScreenLocked>();
		this.staticGrabObject = base.GetComponent<StaticGrabObject>();
		foreach (TruckScreenText.TextPages textPages in this.pages)
		{
			foreach (TruckScreenText.TextLine textLine in textPages.textLines)
			{
				if (textLine.typingSpeed)
				{
					textLine.letterRevealDelay = textLine.typingSpeed.GetDelay();
				}
				else
				{
					textLine.letterRevealDelay = this.textRevealNormalSetting.GetDelay();
				}
				if (textLine.messageDelayTime)
				{
					textLine.delayAfter = textLine.messageDelayTime.GetDelay();
				}
				else
				{
					textLine.delayAfter = this.nextMessageDelayNormalSetting.GetDelay();
				}
			}
		}
		this.screenActive = true;
		if (this.textMesh == null)
		{
			Debug.LogError("TextMeshProUGUI component is not assigned.");
		}
		this.photonView = base.GetComponent<PhotonView>();
		this.currentNickname = this.nicknameTaxman;
		this.chatMessageString = SemiFunc.EmojiText(this.chatMessageString);
	}

	// Token: 0x06000873 RID: 2163 RVA: 0x00051B04 File Offset: 0x0004FD04
	private void LobbyScreenStartLogic()
	{
		if (this.lobbyStarted)
		{
			return;
		}
		if (SemiFunc.IsMasterClientOrSingleplayer() && this.screenType == TruckScreenText.ScreenType.TruckLobbyScreen && RunManager.instance.levelFailed)
		{
			if (RunManager.instance.runLives == 2)
			{
				this.GotoPage(1);
			}
			if (RunManager.instance.runLives == 1)
			{
				this.GotoPage(2);
			}
			if (RunManager.instance.runLives == 0)
			{
				this.GotoPage(3);
			}
		}
		this.lobbyStarted = true;
	}

	// Token: 0x06000874 RID: 2164 RVA: 0x00051B78 File Offset: 0x0004FD78
	public void TutorialFinish()
	{
		GameDirector.instance.OutroStart();
		NetworkManager.instance.leavePhotonRoom = true;
	}

	// Token: 0x06000875 RID: 2165 RVA: 0x00051B8F File Offset: 0x0004FD8F
	public void ArrowPointAtGoal()
	{
		this.arrowPointAtGoalTimer = 4f;
	}

	// Token: 0x06000876 RID: 2166 RVA: 0x00051B9C File Offset: 0x0004FD9C
	private void ArrowPointAtGoalLogic()
	{
		if (this.arrowPointAtGoalTimer > 0f)
		{
			if (PlayerAvatar.instance.RoomVolumeCheck.inTruck)
			{
				SemiFunc.UIShowArrow(new Vector3(340f, 90f, 0f), new Vector3(610f, 330f, 0f), 45f);
			}
			this.arrowPointAtGoalTimer -= Time.deltaTime;
		}
	}

	// Token: 0x06000877 RID: 2167 RVA: 0x00051C0C File Offset: 0x0004FE0C
	private void ChatMessageIdleStringTick()
	{
		if (this.chatActive)
		{
			return;
		}
		if (this.chatMessageIdleStringTimer < 1f)
		{
			this.chatMessageIdleStringTimer += Time.deltaTime;
			return;
		}
		if (this.chatMessageIdleStringCurrent == this.chatMessageIdleString1)
		{
			this.chatMessageIdleStringCurrent = this.chatMessageIdleString2;
		}
		else
		{
			this.chatMessageIdleStringCurrent = this.chatMessageIdleString1;
		}
		this.chatMessage.text = this.chatMessageIdleStringCurrent;
		this.chatMessageIdleStringTimer = 0f;
	}

	// Token: 0x06000878 RID: 2168 RVA: 0x00051C8B File Offset: 0x0004FE8B
	private void PlayerSelfDestruction()
	{
		if (SemiFunc.FPSImpulse5() && this.selfDestructingPlayers && SemiFunc.PlayersAllInTruck())
		{
			this.ChatMessageResultSuccess();
			this.PlayerChatBoxStateUpdate(TruckScreenText.PlayerChatBoxState.LockedStartingTruck);
			this.GotoPage(2);
			this.selfDestructingPlayers = false;
		}
	}

	// Token: 0x06000879 RID: 2169 RVA: 0x00051CC0 File Offset: 0x0004FEC0
	private void Update()
	{
		this.PlayerChatBoxStateMachine();
		this.PlayerSelfDestruction();
		this.ChatMessageResultTick();
		this.ChatMessageIdleStringTick();
		this.HoldChat();
		float b = this.chatMessageTimer / this.chatMessageTimerMax;
		this.chatMessageLoadingBar.localScale = new Vector3(Mathf.Lerp(this.chatMessageLoadingBar.localScale.x, b, Time.deltaTime * 10f), this.chatMessageLoadingBar.localScale.y, this.chatMessageLoadingBar.localScale.z);
		if (this.chatActive)
		{
			this.chatActiveTimer = 0.2f;
		}
		else
		{
			this.chatActiveTimer -= Time.deltaTime;
			if (this.chatActiveTimer <= 0f)
			{
				this.chatMessageTimer = 0f;
			}
		}
		if (SemiFunc.IsMasterClientOrSingleplayer() && !this.started && GameDirector.instance.currentState == GameDirector.gameState.Main)
		{
			if (this.startWaitTimer < 1f)
			{
				this.startWaitTimer += Time.deltaTime;
			}
			else
			{
				this.InitializeTextTyping();
				this.LobbyScreenStartLogic();
			}
		}
		if (!this.started)
		{
			return;
		}
		this.UpdateBackgroundColor();
		if (this.isTyping)
		{
			this.TypingUpdate();
		}
		else
		{
			this.DelayUpdate();
		}
		this.CheckTextMeshLines();
		this.ArrowPointAtGoalLogic();
	}

	// Token: 0x0600087A RID: 2170 RVA: 0x00051E04 File Offset: 0x00050004
	private void InitializeTextTypingLogic()
	{
		this.textMesh.text = "";
		this.currentPageIndex = 0;
		this.currentLineIndex = 0;
		this.currentCharIndex = 0;
		this.typingTimer = 0f;
		foreach (TruckScreenText.TextPages textPages in this.pages)
		{
			foreach (TruckScreenText.TextLine textLine in textPages.textLines)
			{
				if (textLine.textLines.Count > 0)
				{
					textLine.text = textLine.textLines[0];
				}
				else
				{
					textLine.text = "Missing line!!";
				}
				textLine.text = SemiFunc.EmojiText(textLine.text);
				textLine.textOriginal = textLine.text;
			}
		}
		this.started = true;
		this.NextLine(this.currentLineIndex);
	}

	// Token: 0x0600087B RID: 2171 RVA: 0x00051F1C File Offset: 0x0005011C
	private void InitializeTextTyping()
	{
		if (GameManager.instance.gameMode == 0)
		{
			this.InitializeTextTypingLogic();
			return;
		}
		if (PhotonNetwork.IsMasterClient)
		{
			this.photonView.RPC("InitializeTextTypingRPC", RpcTarget.All, Array.Empty<object>());
		}
	}

	// Token: 0x0600087C RID: 2172 RVA: 0x00051F4E File Offset: 0x0005014E
	[PunRPC]
	public void InitializeTextTypingRPC(PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.MasterOnlyRPC(_info))
		{
			return;
		}
		this.InitializeTextTypingLogic();
	}

	// Token: 0x0600087D RID: 2173 RVA: 0x00051F60 File Offset: 0x00050160
	private void CheckTextMeshLines()
	{
		if (this.textMesh.textInfo.lineCount > 12)
		{
			int num = this.textMesh.text.IndexOf('\n');
			if (num != -1)
			{
				this.textMesh.text = this.textMesh.text.Substring(num + 1);
			}
		}
	}

	// Token: 0x0600087E RID: 2174 RVA: 0x00051FB8 File Offset: 0x000501B8
	private void ChatMessageResultSuccess()
	{
		this.chatMessageLoadingBar.localScale = new Vector3(0f, this.chatMessageLoadingBar.localScale.y, this.chatMessageLoadingBar.localScale.z);
		this.chatMessageResultBar.gameObject.SetActive(true);
		this.chatMessageResultSuccess.Play(this.chatMessageResultBar.position, 1f, 1f, 1f, 1f);
		this.chatMessageResultBarLight.color = new Color(0f, 1f, 0f, 1f);
		this.chatMessageResultBar.GetComponent<RawImage>().color = new Color(0f, 1f, 0f, 1f);
		this.chatMessageResultBarTimer = 0.2f;
	}

	// Token: 0x0600087F RID: 2175 RVA: 0x00052090 File Offset: 0x00050290
	private void ChatMessageResultFail()
	{
		this.chatMessageLoadingBar.localScale = new Vector3(0f, this.chatMessageLoadingBar.localScale.y, this.chatMessageLoadingBar.localScale.z);
		this.chatMessageResultBar.gameObject.SetActive(true);
		this.chatMessageResultFail.Play(this.chatMessageResultBar.position, 1f, 1f, 1f, 1f);
		this.chatMessageResultBarLight.color = new Color(1f, 0f, 0f, 1f);
		this.chatMessageResultBar.GetComponent<RawImage>().color = new Color(1f, 0f, 0f, 1f);
		this.chatMessageResultBarTimer = 0.2f;
	}

	// Token: 0x06000880 RID: 2176 RVA: 0x00052168 File Offset: 0x00050368
	private void ChatMessageResultTick()
	{
		if (this.chatMessageResultBarTimer > 0f)
		{
			this.chatMessageResultBarTimer -= Time.deltaTime;
			return;
		}
		if (this.chatMessageResultBarTimer != -123f)
		{
			this.chatMessageResultBar.gameObject.SetActive(false);
			this.chatMessageResultBarTimer = -123f;
		}
	}

	// Token: 0x06000881 RID: 2177 RVA: 0x000521C0 File Offset: 0x000503C0
	public void ChatMessageLevel()
	{
		if (RoundDirector.instance.extractionPointsCompleted != RoundDirector.instance.extractionPoints)
		{
			this.ChatMessageResultFail();
			this.GotoPage(1);
			return;
		}
		if (SemiFunc.PlayersAllInTruck())
		{
			this.ChatMessageResultSuccess();
			this.PlayerChatBoxStateUpdate(TruckScreenText.PlayerChatBoxState.LockedStartingTruck);
			this.GotoPage(2);
			return;
		}
		this.ChatMessageResultFail();
		this.PlayerChatBoxStateUpdate(TruckScreenText.PlayerChatBoxState.LockedDestroySlackers);
		this.GotoPage(3);
	}

	// Token: 0x06000882 RID: 2178 RVA: 0x00052221 File Offset: 0x00050421
	public void ChatMessageLobby()
	{
		this.ChatMessageResultSuccess();
		this.PlayerChatBoxStateUpdate(TruckScreenText.PlayerChatBoxState.LockedStartingTruck);
		this.GotoPage(4);
	}

	// Token: 0x06000883 RID: 2179 RVA: 0x00052237 File Offset: 0x00050437
	public void ChatMessageTutorial()
	{
		this.PlayerChatBoxStateUpdate(TruckScreenText.PlayerChatBoxState.LockedStartingTruck);
		this.ChatMessageResultSuccess();
		this.GotoPage(2);
	}

	// Token: 0x06000884 RID: 2180 RVA: 0x0005224D File Offset: 0x0005044D
	public void ChatMessageShop()
	{
		if (SemiFunc.PlayersAllInTruck())
		{
			this.ChatMessageResultSuccess();
			this.PlayerChatBoxStateUpdate(TruckScreenText.PlayerChatBoxState.LockedStartingTruck);
			this.GotoPage(2);
			return;
		}
		this.ChatMessageResultFail();
		this.PlayerChatBoxStateUpdate(TruckScreenText.PlayerChatBoxState.LockedDestroySlackers);
		this.GotoPage(4);
	}

	// Token: 0x06000885 RID: 2181 RVA: 0x0005227F File Offset: 0x0005047F
	private void OnValidate()
	{
		if (SemiFunc.OnValidateCheck())
		{
			return;
		}
		this.textMesh.text = SemiFunc.EmojiText(this.testingText);
	}

	// Token: 0x06000886 RID: 2182 RVA: 0x0005229F File Offset: 0x0005049F
	private void ReplaceStringWithVariable(string variableString, string variableValueString, TruckScreenText.TextLine currentLine)
	{
		currentLine.text = currentLine.text.Remove(this.currentCharIndex, variableString.Length - 1);
		currentLine.text = currentLine.text.Insert(this.currentCharIndex + 1, variableValueString);
	}

	// Token: 0x06000887 RID: 2183 RVA: 0x000522DC File Offset: 0x000504DC
	private void TypingUpdate()
	{
		if (!this.screenActive)
		{
			return;
		}
		if (this.currentPageIndex < this.pages.Count)
		{
			TruckScreenText.TextPages textPages = this.pages[this.currentPageIndex];
			if (this.currentLineIndex < textPages.textLines.Count)
			{
				TruckScreenText.TextLine textLine = textPages.textLines[this.currentLineIndex];
				if (this.currentCharIndex < textLine.text.Length)
				{
					if (this.currentCharIndex == 0 && this.typingTimer == 0f)
					{
						if (this.currentLineIndex != 0)
						{
							this.textMesh.text = this.textMesh.text;
						}
						this.textMesh.text = this.textMesh.text + this.currentNickname;
						this.NewLineSoundEffect();
					}
					this.typingTimer += Time.deltaTime;
					if (this.typingTimer >= textLine.letterRevealDelay)
					{
						string emojiString = "";
						bool flag = false;
						int num = this.currentCharIndex;
						if (textLine.text.get_Chars(num) == '<')
						{
							while (textLine.text.get_Chars(num) != '>')
							{
								emojiString += textLine.text.get_Chars(num).ToString();
								num++;
							}
							flag = true;
						}
						if (flag)
						{
							TextMeshProUGUI textMeshProUGUI = this.textMesh;
							textMeshProUGUI.text += emojiString;
							this.currentCharIndex = num;
						}
						string text = "";
						int num2 = 0;
						bool flag2 = false;
						if (textLine.text.get_Chars(this.currentCharIndex) == '[')
						{
							flag2 = true;
							while (textLine.text.get_Chars(this.currentCharIndex) != ']')
							{
								text += textLine.text.get_Chars(this.currentCharIndex).ToString();
								this.currentCharIndex++;
								num2++;
							}
							text += textLine.text.get_Chars(this.currentCharIndex).ToString();
							this.currentCharIndex++;
							num2++;
							this.currentCharIndex -= num2;
							if (text == "[haul]")
							{
								string text2 = RoundDirector.instance.totalHaul.ToString();
								text2 = this.FormatDollarValueStrings(text2);
								text2 = "<color=#003300>$</color>" + text2;
								this.ReplaceStringWithVariable(text, text2, textLine);
							}
							if (text == "[goal]")
							{
								int haulGoal = RoundDirector.instance.haulGoal;
								string text3 = haulGoal.ToString();
								text3 = this.FormatDollarValueStrings(text3);
								this.ReplaceStringWithVariable(text, text3, textLine);
							}
							if (text == "[goalmax]")
							{
								string text4 = RoundDirector.instance.haulGoalMax.ToString();
								text4 = this.FormatDollarValueStrings(text4);
								this.ReplaceStringWithVariable(text, text4, textLine);
							}
							if (text == "[hitroad]")
							{
								if (this.currentNickname == this.nicknameTaxman)
								{
									this.chatMessageString = this.chatMessageString.Replace("?", "!");
								}
								this.ReplaceStringWithVariable(text, this.chatMessageString, textLine);
								this.currentNickname = this.nicknameTaxman;
							}
							if (text == "[allplayerintruck]")
							{
								List<string> list = new List<string>();
								foreach (PlayerAvatar playerAvatar in GameDirector.instance.PlayerList)
								{
									if (!playerAvatar.isDisabled && !playerAvatar.RoomVolumeCheck.inTruck)
									{
										list.Add(playerAvatar.playerName);
									}
								}
								string text5 = "";
								for (int i = 0; i < list.Count; i++)
								{
									if (i == 0)
									{
										text5 += list[i];
									}
									else if (i == list.Count - 1)
									{
										text5 = text5 + " & " + list[i];
									}
									else
									{
										text5 = text5 + ", " + list[i];
									}
								}
								text5 += "...<sprite name=fedup>";
								text5 += "\n";
								text5 += SemiFunc.EmojiText("{pointright}{truck}{pointleft}");
								this.ReplaceStringWithVariable(text, text5, textLine);
							}
							if (text == "[betrayplayers]")
							{
								List<string> list2 = new List<string>();
								foreach (PlayerAvatar playerAvatar2 in GameDirector.instance.PlayerList)
								{
									if (!playerAvatar2.isDisabled && !playerAvatar2.RoomVolumeCheck.inTruck)
									{
										list2.Add(playerAvatar2.playerName);
									}
								}
								string text6 = "";
								for (int j = 0; j < list2.Count; j++)
								{
									if (j == 0)
									{
										text6 += list2[j];
									}
									else if (j == list2.Count - 1)
									{
										text6 = text6 + " & " + list2[j];
									}
									else
									{
										text6 = text6 + ", " + list2[j];
									}
								}
								text6 += "... <sprite name=fedup>";
								text6 += "\n";
								text6 += SemiFunc.EmojiText("{pointright}{truck}{pointleft}");
								this.ReplaceStringWithVariable(text, text6, textLine);
							}
						}
						if (!flag2)
						{
							TextMeshProUGUI textMeshProUGUI2 = this.textMesh;
							textMeshProUGUI2.text += textLine.text.get_Chars(this.currentCharIndex).ToString();
						}
						if (flag)
						{
							emojiString += textLine.text.get_Chars(this.currentCharIndex).ToString();
						}
						if (textLine.text.get_Chars(this.currentCharIndex) != ' ')
						{
							if (!flag)
							{
								this.TypeSoundEffect();
							}
							else if (Enumerable.Any<TruckScreenText.CustomEmojiSounds>(this.customEmojiSounds, (TruckScreenText.CustomEmojiSounds x) => x.emojiString == emojiString))
							{
								this.customEmojiSounds.Find((TruckScreenText.CustomEmojiSounds x) => x.emojiString == emojiString).emojiSound.Play(this.textMesh.transform.position, 1f, 1f, 1f, 1f);
							}
							else
							{
								this.emojiSound.Play(this.textMesh.transform.position, 1f, 1f, 1f, 1f);
							}
						}
						this.currentCharIndex++;
						this.typingTimer = 0f;
						if (this.currentCharIndex >= textLine.text.Length)
						{
							this.textMesh.text = this.textMesh.text;
							this.isTyping = false;
							this.delayTimer = 0f;
						}
					}
				}
			}
		}
	}

	// Token: 0x06000888 RID: 2184 RVA: 0x000529F0 File Offset: 0x00050BF0
	private void UpdateBackgroundColor()
	{
		if (this.backgroundColorChangeTimer < this.backgroundColorChangeDuration)
		{
			this.backgroundColorChangeTimer += Time.deltaTime;
			return;
		}
		if (this.screenActive)
		{
			this.background.color = this.mainBackgroundColor;
			return;
		}
		this.background.color = this.offBackgroundColor;
	}

	// Token: 0x06000889 RID: 2185 RVA: 0x00052A4C File Offset: 0x00050C4C
	private void DelayUpdate()
	{
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		if (this.currentLineIndex >= this.pages[this.currentPageIndex].textLines.Count)
		{
			return;
		}
		TruckScreenText.TextLine textLine = this.pages[this.currentPageIndex].textLines[this.currentLineIndex];
		this.delayTimer += Time.deltaTime;
		if (this.delayTimer >= textLine.delayAfter)
		{
			UnityEvent onLineEnd = this.pages[this.currentPageIndex].textLines[this.currentLineIndex].onLineEnd;
			if (onLineEnd != null)
			{
				onLineEnd.Invoke();
			}
			this.currentLineIndex++;
			if ((this.currentLineIndex >= this.pages[this.currentPageIndex].textLines.Count && this.pages[this.currentPageIndex].goToNextPageAutomatically) || this.nextPageOverride != -1)
			{
				if (this.nextPageOverride != -1)
				{
					this.GotoPage(this.nextPageOverride);
					this.nextPageOverride = -1;
				}
				else
				{
					this.GotoPage(this.currentPageIndex + 1);
				}
				this.currentLineIndex = 0;
				if (this.currentPageIndex >= this.pages.Count)
				{
					this.currentPageIndex = this.pages.Count;
				}
			}
			if (this.currentLineIndex < this.pages[this.currentPageIndex].textLines.Count)
			{
				this.NextLine(this.currentLineIndex);
			}
		}
	}

	// Token: 0x0600088A RID: 2186 RVA: 0x00052BD2 File Offset: 0x00050DD2
	private void RestartTyping()
	{
		this.InitializeTextTyping();
	}

	// Token: 0x0600088B RID: 2187 RVA: 0x00052BDC File Offset: 0x00050DDC
	private void TypeSoundEffect()
	{
		if (Enumerable.Count<AudioClip>(this.pages[this.currentPageIndex].textLines[this.currentLineIndex].customTypingSoundEffect.Sounds) > 0)
		{
			this.pages[this.currentPageIndex].textLines[this.currentLineIndex].customTypingSoundEffect.Play(this.textMesh.transform.position, 1f, 1f, 1f, 1f);
			return;
		}
		this.typingSound.Play(this.textMesh.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x0600088C RID: 2188 RVA: 0x00052CA0 File Offset: 0x00050EA0
	private void NewLineSoundEffect()
	{
		if (Enumerable.Count<AudioClip>(this.pages[this.currentPageIndex].textLines[this.currentLineIndex].customMessageSoundEffect.Sounds) > 0)
		{
			this.pages[this.currentPageIndex].textLines[this.currentLineIndex].customMessageSoundEffect.Play(this.textMesh.transform.position, 1f, 1f, 1f, 1f);
			return;
		}
		this.newLineSound.Play(this.textMesh.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x0600088D RID: 2189 RVA: 0x00052D61 File Offset: 0x00050F61
	public void StartChat()
	{
		if (!this.screenActive)
		{
			return;
		}
		if (GameManager.instance.gameMode == 0)
		{
			this.chatActive = true;
			return;
		}
		this.photonView.RPC("StartChatRPC", RpcTarget.All, Array.Empty<object>());
	}

	// Token: 0x0600088E RID: 2190 RVA: 0x00052D96 File Offset: 0x00050F96
	[PunRPC]
	public void StartChatRPC(PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.MasterOnlyRPC(_info))
		{
			return;
		}
		this.chatActive = true;
	}

	// Token: 0x0600088F RID: 2191 RVA: 0x00052DA8 File Offset: 0x00050FA8
	public void HoldChat()
	{
		this.chargeChatLoop.LoopPitch = 1f + this.chatMessageTimer / this.chatMessageTimerMax;
		this.chargeChatLoop.PlayLoop(this.chatActive, 0.9f, 0.9f, 1f);
		if (!this.screenActive && this.chatActiveTimer <= 0f)
		{
			return;
		}
		if (this.chatDeactivatedTimer > 0f && this.chatActiveTimer <= 0f)
		{
			this.chatDeactivatedTimer -= Time.deltaTime;
			return;
		}
		if (!this.chatActive && this.chatActiveTimer <= 0f)
		{
			return;
		}
		if (this.playerChatBoxState != TruckScreenText.PlayerChatBoxState.Idle)
		{
			this.ForceReleaseChat();
			this.staticGrabCollider.SetActive(false);
			this.chatActive = false;
			return;
		}
		this.chatMessageTimer += 1.5f * Time.deltaTime;
		if (this.chatMessageTimer >= this.chatMessageTimerMax)
		{
			this.chatMessageTimer = 0f;
			this.chatActiveTimer = 0f;
			this.chatActive = false;
			if (this.staticGrabObject.playerGrabbing.Count > 0)
			{
				PhysGrabber physGrabber = this.staticGrabObject.playerGrabbing[0];
				if (physGrabber)
				{
					string playerName = physGrabber.playerAvatar.playerName;
					if (GameManager.instance.gameMode == 0)
					{
						this.ChatMessageSend(playerName);
						return;
					}
					if (PhotonNetwork.IsMasterClient)
					{
						this.photonView.RPC("ChatMessageSendRPC", RpcTarget.All, new object[]
						{
							playerName
						});
					}
				}
			}
			return;
		}
		int num = (int)(this.chatMessageTimer / this.chatMessageTimerMax * (float)this.chatMessageString.Length);
		num = Mathf.Min(num, this.chatMessageString.Length);
		bool flag = false;
		string emojiString = "";
		while (this.chatCharacterIndex < num)
		{
			if (this.chatMessageString.get_Chars(this.chatCharacterIndex) == '<')
			{
				flag = true;
				int num2 = this.chatMessageString.IndexOf('>', this.chatCharacterIndex);
				if (num2 != -1)
				{
					num = Mathf.Min(num + (num2 - this.chatCharacterIndex), this.chatMessageString.Length);
					this.chatCharacterIndex = num2 + 1;
				}
				else
				{
					this.chatCharacterIndex++;
					emojiString += this.chatMessageString.get_Chars(this.chatCharacterIndex).ToString();
				}
			}
			else
			{
				this.chatCharacterIndex++;
			}
			if (!flag)
			{
				this.TypeSoundEffect();
			}
			else if (Enumerable.Any<TruckScreenText.CustomEmojiSounds>(this.customEmojiSounds, (TruckScreenText.CustomEmojiSounds x) => x.emojiString == emojiString))
			{
				this.customEmojiSounds.Find((TruckScreenText.CustomEmojiSounds x) => x.emojiString == emojiString).emojiSound.Play(this.textMesh.transform.position, 1f, 1f, 1f, 1f);
			}
			else
			{
				this.emojiSound.Play(this.textMesh.transform.position, 1f, 1f, 1f, 1f);
			}
			this.chatMessage.text = this.chatMessageString.Substring(0, this.chatCharacterIndex);
		}
	}

	// Token: 0x06000890 RID: 2192 RVA: 0x000530D8 File Offset: 0x000512D8
	private void ForceReleaseChat()
	{
		if (this.staticGrabObject.playerGrabbing.Count > 0)
		{
			List<PhysGrabber> list = new List<PhysGrabber>();
			list.AddRange(this.staticGrabObject.playerGrabbing);
			foreach (PhysGrabber physGrabber in list)
			{
				if (!SemiFunc.IsMultiplayer())
				{
					physGrabber.ReleaseObject(0.1f);
				}
				else
				{
					physGrabber.photonView.RPC("ReleaseObjectRPC", RpcTarget.All, new object[]
					{
						false,
						0.1f
					});
				}
			}
		}
	}

	// Token: 0x06000891 RID: 2193 RVA: 0x0005318C File Offset: 0x0005138C
	private void NextLineLogic(int _lineIndex, int index)
	{
		UnityEvent onLineStart = this.pages[this.currentPageIndex].textLines[this.currentLineIndex].onLineStart;
		if (onLineStart != null)
		{
			onLineStart.Invoke();
		}
		this.pages[this.currentPageIndex].textLines[this.currentLineIndex].text = SemiFunc.EmojiText(this.pages[this.currentPageIndex].textLines[this.currentLineIndex].textLines[index]);
		this.currentCharIndex = 0;
		this.currentLineIndex = _lineIndex;
		this.isTyping = true;
		this.typingTimer = 0f;
		this.delayTimer = 0f;
	}

	// Token: 0x06000892 RID: 2194 RVA: 0x0005324C File Offset: 0x0005144C
	private void NextLine(int _currentLineIndex)
	{
		if (this.pages[this.currentPageIndex].textLines.Count == 0)
		{
			return;
		}
		if (GameManager.instance.gameMode == 0)
		{
			int maxExclusive = Enumerable.Count<string>(this.pages[this.currentPageIndex].textLines[this.currentLineIndex].textLines);
			int index = Random.Range(0, maxExclusive);
			this.NextLineLogic(_currentLineIndex, index);
			return;
		}
		if (PhotonNetwork.IsMasterClient)
		{
			int maxExclusive2 = Enumerable.Count<string>(this.pages[this.currentPageIndex].textLines[this.currentLineIndex].textLines);
			int num = Random.Range(0, maxExclusive2);
			this.photonView.RPC("NextLineRPC", RpcTarget.All, new object[]
			{
				num,
				_currentLineIndex
			});
		}
	}

	// Token: 0x06000893 RID: 2195 RVA: 0x00053324 File Offset: 0x00051524
	[PunRPC]
	public void NextLineRPC(int index, int _currentLineIndex, PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.MasterOnlyRPC(_info))
		{
			return;
		}
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			this.currentLineIndex = _currentLineIndex;
		}
		this.NextLineLogic(_currentLineIndex, index);
	}

	// Token: 0x06000894 RID: 2196 RVA: 0x00053348 File Offset: 0x00051548
	private void ForceCompleteChatMessage()
	{
		int length = this.pages[this.currentPageIndex].textLines[this.currentLineIndex].text.Length;
		if (this.currentCharIndex <= length)
		{
			for (int i = this.currentCharIndex; i < length; i++)
			{
				this.TypingUpdate();
			}
			this.currentCharIndex = length;
			this.isTyping = false;
			this.typingTimer = 0f;
		}
		this.TypeSoundEffect();
	}

	// Token: 0x06000895 RID: 2197 RVA: 0x000533C0 File Offset: 0x000515C0
	private void ChatMessageSend(string playerName)
	{
		string text = ColorUtility.ToHtmlStringRGB(SemiFunc.PlayerGetFromName(playerName).playerAvatarVisuals.color);
		this.currentNickname = string.Concat(new string[]
		{
			"\n\n<color=#",
			text,
			"><b>",
			playerName,
			":</b></color>\n"
		});
		this.onChatMessage.Invoke();
		this.chatDeactivatedTimer = 3f;
		this.chatMessage.text = "";
	}

	// Token: 0x06000896 RID: 2198 RVA: 0x0005343C File Offset: 0x0005163C
	public void SelfDestructPlayersOutsideTruck()
	{
		if (!SemiFunc.IsMultiplayer())
		{
			this.SelfDestructPlayersOutsideTruckRPC(default(PhotonMessageInfo));
			return;
		}
		if (PhotonNetwork.IsMasterClient)
		{
			this.photonView.RPC("SelfDestructPlayersOutsideTruckRPC", RpcTarget.All, Array.Empty<object>());
		}
	}

	// Token: 0x06000897 RID: 2199 RVA: 0x0005347D File Offset: 0x0005167D
	[PunRPC]
	public void SelfDestructPlayersOutsideTruckRPC(PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.MasterOnlyRPC(_info))
		{
			return;
		}
		this.selfDestructingPlayers = true;
		ChatManager.instance.PossessLeftBehind();
	}

	// Token: 0x06000898 RID: 2200 RVA: 0x00053499 File Offset: 0x00051699
	[PunRPC]
	public void ChatMessageSendRPC(string playerName, PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.MasterOnlyRPC(_info))
		{
			return;
		}
		this.ChatMessageSend(playerName);
	}

	// Token: 0x06000899 RID: 2201 RVA: 0x000534AB File Offset: 0x000516AB
	public void GotoNextLevel()
	{
		this.EngineStartSound();
		RunManager.instance.ChangeLevel(true, false, RunManager.ChangeLevelType.Normal);
	}

	// Token: 0x0600089A RID: 2202 RVA: 0x000534C0 File Offset: 0x000516C0
	public void ShopCompleted()
	{
		base.StartCoroutine(this.ShopGotoNextLevel());
	}

	// Token: 0x0600089B RID: 2203 RVA: 0x000534D0 File Offset: 0x000516D0
	private void EngineStartSound()
	{
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		if (SemiFunc.IsMultiplayer())
		{
			this.photonView.RPC("EngineStartRPC", RpcTarget.All, Array.Empty<object>());
			return;
		}
		this.EngineStartRPC(default(PhotonMessageInfo));
	}

	// Token: 0x0600089C RID: 2204 RVA: 0x00053512 File Offset: 0x00051712
	[PunRPC]
	public void EngineStartRPC(PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.MasterOnlyRPC(_info))
		{
			return;
		}
		this.engineSuccessSound.Play(this.textMesh.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x0600089D RID: 2205 RVA: 0x0005354D File Offset: 0x0005174D
	public IEnumerator ShopGotoNextLevel()
	{
		yield return new WaitForSeconds(0.5f);
		RunManager.instance.ChangeLevel(true, false, RunManager.ChangeLevelType.Normal);
		yield break;
	}

	// Token: 0x0600089E RID: 2206 RVA: 0x00053558 File Offset: 0x00051758
	private void PageTransitionEffect()
	{
		this.background.color = this.transitionBackgroundColor;
		this.backgroundColorChangeTimer = 0f;
		this.backgroundColorChangeDuration = 0.5f;
		this.newPageSound.Play(this.textMesh.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x0600089F RID: 2207 RVA: 0x000535BC File Offset: 0x000517BC
	public void GotoPage(int pageIndex)
	{
		if (GameManager.instance.gameMode == 0)
		{
			this.GotoPageLogic(pageIndex);
			return;
		}
		if (PhotonNetwork.IsMasterClient)
		{
			this.photonView.RPC("GotoPageRPC", RpcTarget.All, new object[]
			{
				pageIndex
			});
		}
	}

	// Token: 0x060008A0 RID: 2208 RVA: 0x000535FC File Offset: 0x000517FC
	[PunRPC]
	public void MessageSendCustomRPC(string playerName, string message, PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (this.isTyping)
		{
			return;
		}
		bool flag = false;
		bool flag2 = false;
		if (GameManager.Multiplayer() && _info.Sender != PhotonNetwork.MasterClient)
		{
			flag = true;
			foreach (PlayerAvatar playerAvatar in SemiFunc.PlayerGetList())
			{
				if (playerAvatar.photonView.Owner == _info.Sender)
				{
					playerName = playerAvatar.playerName;
				}
			}
		}
		if (playerName != "" || flag)
		{
			foreach (PlayerAvatar playerAvatar2 in SemiFunc.PlayerGetList())
			{
				if (playerAvatar2.playerName == playerName)
				{
					string text = ColorUtility.ToHtmlStringRGB(playerAvatar2.playerAvatarVisuals.color);
					this.currentNickname = string.Concat(new string[]
					{
						"\n\n<color=#",
						text,
						"><b>",
						playerName,
						":</b></color>\n"
					});
					flag2 = true;
					break;
				}
			}
			if (!flag2)
			{
				return;
			}
		}
		else
		{
			this.currentNickname = this.nicknameTaxman;
		}
		TextMeshProUGUI textMeshProUGUI = this.textMesh;
		textMeshProUGUI.text = textMeshProUGUI.text + this.currentNickname + SemiFunc.EmojiText(message);
		this.newLineSound.Play(this.textMesh.transform.position, 1f, 1f, 1f, 1f);
		this.currentNickname = this.nicknameTaxman;
	}

	// Token: 0x060008A1 RID: 2209 RVA: 0x00053798 File Offset: 0x00051998
	public void MessageSendCustom(string playerName, string message, int emojis)
	{
		if (playerName != "")
		{
			List<string> list = new List<string>();
			list.Add("{:)}");
			list.Add("{:D}");
			list.Add("{:P}");
			list.Add("{eyes}");
			list.Add("{:o}");
			list.Add("{heart}");
			List<string> list2 = list;
			List<string> list3 = new List<string>();
			list3.Add("{:(}");
			list3.Add("{:'(}");
			list3.Add("{heartbreak}");
			list3.Add("{fedup}");
			List<string> list4 = list3;
			string text = "";
			if (emojis != 0)
			{
				List<string> list5 = list2;
				if (emojis == 1)
				{
					list5 = list2;
				}
				if (emojis == 2)
				{
					list5 = list4;
				}
				text += list5[Random.Range(0, list5.Count)];
				if (Random.Range(0, 2) == 1)
				{
					text += list5[Random.Range(0, list5.Count)];
				}
				if (Random.Range(0, 5) == 1)
				{
					text += list5[Random.Range(0, list5.Count)];
				}
				if (Random.Range(0, 30) == 1)
				{
					text = list5[Random.Range(0, list5.Count)];
					text += text;
					text += text;
				}
			}
			message += text;
		}
		if (SemiFunc.IsMultiplayer())
		{
			this.photonView.RPC("MessageSendCustomRPC", RpcTarget.All, new object[]
			{
				playerName,
				message
			});
			return;
		}
		this.MessageSendCustomRPC(playerName, message, default(PhotonMessageInfo));
	}

	// Token: 0x060008A2 RID: 2210 RVA: 0x00053918 File Offset: 0x00051B18
	private void GotoPageLogic(int pageIndex)
	{
		this.currentPageIndex = pageIndex;
		this.currentLineIndex = 0;
		this.currentCharIndex = 0;
		this.typingTimer = 0f;
		this.isTyping = true;
		foreach (TruckScreenText.TextPages textPages in this.pages)
		{
			foreach (TruckScreenText.TextLine textLine in textPages.textLines)
			{
				textLine.text = textLine.textOriginal;
			}
		}
		this.NextLine(this.currentLineIndex);
	}

	// Token: 0x060008A3 RID: 2211 RVA: 0x000539DC File Offset: 0x00051BDC
	[PunRPC]
	public void GotoPageRPC(int pageIndex)
	{
		this.GotoPageLogic(pageIndex);
	}

	// Token: 0x060008A4 RID: 2212 RVA: 0x000539E8 File Offset: 0x00051BE8
	public void ReleaseChat()
	{
		if (GameManager.instance.gameMode == 0)
		{
			this.ReleaseChatRPC(default(PhotonMessageInfo));
			return;
		}
		this.photonView.RPC("ReleaseChatRPC", RpcTarget.All, Array.Empty<object>());
	}

	// Token: 0x060008A5 RID: 2213 RVA: 0x00053A27 File Offset: 0x00051C27
	private string FormatDollarValueStrings(string valueString)
	{
		valueString = SemiFunc.DollarGetString(int.Parse(valueString));
		return valueString;
	}

	// Token: 0x060008A6 RID: 2214 RVA: 0x00053A37 File Offset: 0x00051C37
	[PunRPC]
	public void ReleaseChatRPC(PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.MasterOnlyRPC(_info))
		{
			return;
		}
		this.chatActive = false;
		this.chatMessageTimer = 0f;
		this.chatCharacterIndex = 0;
		this.chatMessage.text = "";
	}

	// Token: 0x060008A7 RID: 2215 RVA: 0x00053A6B File Offset: 0x00051C6B
	private void GotoPageAfterPageIsDone(int pageIndex)
	{
		this.nextPageOverride = pageIndex;
	}

	// Token: 0x060008A8 RID: 2216 RVA: 0x00053A74 File Offset: 0x00051C74
	private void PlayerChatBoxStateUpdate(TruckScreenText.PlayerChatBoxState _state)
	{
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			if (SemiFunc.IsMultiplayer())
			{
				this.photonView.RPC("PlayerChatBoxStateUpdateRPC", RpcTarget.All, new object[]
				{
					_state
				});
				return;
			}
			this.PlayerChatBoxStateUpdateRPC(_state);
		}
	}

	// Token: 0x060008A9 RID: 2217 RVA: 0x00053AAC File Offset: 0x00051CAC
	public void PlayerChatBoxStateUpdateToLockedDestroySlackers()
	{
		this.PlayerChatBoxStateUpdate(TruckScreenText.PlayerChatBoxState.LockedDestroySlackers);
	}

	// Token: 0x060008AA RID: 2218 RVA: 0x00053AB5 File Offset: 0x00051CB5
	public void PlayerChatBoxStateUpdateToLockedStartingTruck()
	{
		this.PlayerChatBoxStateUpdate(TruckScreenText.PlayerChatBoxState.LockedStartingTruck);
	}

	// Token: 0x060008AB RID: 2219 RVA: 0x00053ABE File Offset: 0x00051CBE
	[PunRPC]
	private void PlayerChatBoxStateUpdateRPC(TruckScreenText.PlayerChatBoxState _state)
	{
		this.playerChatBoxState = _state;
		this.playerChatBoxStateStart = true;
	}

	// Token: 0x060008AC RID: 2220 RVA: 0x00053AD0 File Offset: 0x00051CD0
	private void PlayerChatBoxStateIdle()
	{
		if (this.playerChatBoxStateStart)
		{
			this.truckScreenLocked.LockChatToggle(false, "", default(Color), default(Color));
			this.staticGrabCollider.SetActive(true);
			this.playerChatBoxStateStart = false;
		}
	}

	// Token: 0x060008AD RID: 2221 RVA: 0x00053B1C File Offset: 0x00051D1C
	private void PlayerChatBoxStateLockedStartingTruck()
	{
		if (this.playerChatBoxStateStart)
		{
			this.ForceReleaseChat();
			Color darkColor = new Color(0.8f, 0.2f, 0.1f, 1f);
			Color lightColor = new Color(1f, 0.8f, 0f, 1f);
			string lockedText = "STARTING ENGINE";
			if (SemiFunc.RunIsLobby())
			{
				lockedText = "HITTING THE ROAD";
			}
			this.truckScreenLocked.LockChatToggle(true, lockedText, lightColor, darkColor);
			this.playerChatBoxStateStart = false;
		}
		if (!SemiFunc.RunIsLobby())
		{
			if (this.engineSoundTimer > 0f)
			{
				this.engineSoundTimer -= Time.deltaTime;
				return;
			}
			this.engineSoundTimer = Random.Range(2f, 4f);
			this.engineRevSound.Play(this.truckScreenLocked.transform.position, 1f, 1f, 1f, 1f);
		}
	}

	// Token: 0x060008AE RID: 2222 RVA: 0x00053C04 File Offset: 0x00051E04
	private void PlayerChatBoxStateLockedDestroySlackers()
	{
		if (this.playerChatBoxStateStart)
		{
			this.ForceReleaseChat();
			Color darkColor = new Color(0.4f, 0f, 0.3f, 1f);
			Color lightColor = new Color(1f, 0f, 0f, 1f);
			this.truckScreenLocked.LockChatToggle(true, "DESTROYING SLACKERS", lightColor, darkColor);
			this.playerChatBoxStateStart = false;
		}
	}

	// Token: 0x060008AF RID: 2223 RVA: 0x00053C70 File Offset: 0x00051E70
	private void PlayerChatBoxStateMachine()
	{
		switch (this.playerChatBoxState)
		{
		case TruckScreenText.PlayerChatBoxState.Idle:
			this.PlayerChatBoxStateIdle();
			return;
		case TruckScreenText.PlayerChatBoxState.Typing:
			break;
		case TruckScreenText.PlayerChatBoxState.LockedDestroySlackers:
			this.PlayerChatBoxStateLockedDestroySlackers();
			break;
		case TruckScreenText.PlayerChatBoxState.LockedStartingTruck:
			this.PlayerChatBoxStateLockedStartingTruck();
			return;
		default:
			return;
		}
	}

	// Token: 0x04000F8F RID: 3983
	public ScreenTextRevealDelaySettings textRevealNormalSetting;

	// Token: 0x04000F90 RID: 3984
	public ScreenNextMessageDelaySettings nextMessageDelayNormalSetting;

	// Token: 0x04000F91 RID: 3985
	public GameObject staticGrabCollider;

	// Token: 0x04000F92 RID: 3986
	public static TruckScreenText instance;

	// Token: 0x04000F93 RID: 3987
	private StaticGrabObject staticGrabObject;

	// Token: 0x04000F94 RID: 3988
	public TruckScreenText.ScreenType screenType;

	// Token: 0x04000F95 RID: 3989
	public string chatMessageString;

	// Token: 0x04000F96 RID: 3990
	public TextMeshProUGUI chatMessage;

	// Token: 0x04000F97 RID: 3991
	public UnityEvent onChatMessage;

	// Token: 0x04000F98 RID: 3992
	private float chatMessageTimer;

	// Token: 0x04000F99 RID: 3993
	private float chatMessageTimerMax = 2f;

	// Token: 0x04000F9A RID: 3994
	private bool chatActive;

	// Token: 0x04000F9B RID: 3995
	private PhotonView photonView;

	// Token: 0x04000F9C RID: 3996
	private int chatCharacterIndex;

	// Token: 0x04000F9D RID: 3997
	private float chatDeactivatedTimer;

	// Token: 0x04000F9E RID: 3998
	private string nicknameTaxman = "\n\n<color=#4d0000><b>TAXMAN:</b></color>\n";

	// Token: 0x04000F9F RID: 3999
	private string currentNickname = "";

	// Token: 0x04000FA0 RID: 4000
	private bool screenActive;

	// Token: 0x04000FA1 RID: 4001
	private int nextPageOverride = -1;

	// Token: 0x04000FA2 RID: 4002
	public Transform chatMessageLoadingBar;

	// Token: 0x04000FA3 RID: 4003
	public Transform chatMessageResultBar;

	// Token: 0x04000FA4 RID: 4004
	public Light chatMessageResultBarLight;

	// Token: 0x04000FA5 RID: 4005
	internal float chatMessageResultBarTimer;

	// Token: 0x04000FA6 RID: 4006
	private float chatActiveTimer;

	// Token: 0x04000FA7 RID: 4007
	private bool selfDestructingPlayers;

	// Token: 0x04000FA8 RID: 4008
	private TuckScreenLocked truckScreenLocked;

	// Token: 0x04000FA9 RID: 4009
	private string chatMessageIdleString1 = "<sprite name=message>";

	// Token: 0x04000FAA RID: 4010
	private string chatMessageIdleString2 = "<sprite name=message_arrow>";

	// Token: 0x04000FAB RID: 4011
	private string chatMessageIdleStringCurrent = "<sprite name=message>";

	// Token: 0x04000FAC RID: 4012
	private float chatMessageIdleStringTimer;

	// Token: 0x04000FAD RID: 4013
	internal TruckScreenText.PlayerChatBoxState playerChatBoxState;

	// Token: 0x04000FAE RID: 4014
	private bool playerChatBoxStateStart;

	// Token: 0x04000FAF RID: 4015
	public List<TruckScreenText.CustomEmojiSounds> customEmojiSounds = new List<TruckScreenText.CustomEmojiSounds>();

	// Token: 0x04000FB0 RID: 4016
	public Sound typingSound;

	// Token: 0x04000FB1 RID: 4017
	public Sound emojiSound;

	// Token: 0x04000FB2 RID: 4018
	public Sound newLineSound;

	// Token: 0x04000FB3 RID: 4019
	public Sound newPageSound;

	// Token: 0x04000FB4 RID: 4020
	public Sound chargeChatLoop;

	// Token: 0x04000FB5 RID: 4021
	public Sound chatMessageResultSuccess;

	// Token: 0x04000FB6 RID: 4022
	public Sound chatMessageResultFail;

	// Token: 0x04000FB7 RID: 4023
	public RawImage background;

	// Token: 0x04000FB8 RID: 4024
	public Color mainBackgroundColor = new Color(0.6f, 0.6f, 0.6f, 1f);

	// Token: 0x04000FB9 RID: 4025
	public Color offBackgroundColor = new Color(0f, 0f, 0f, 1f);

	// Token: 0x04000FBA RID: 4026
	public Color evilBackgroundColor = new Color(0.5f, 0f, 0f, 1f);

	// Token: 0x04000FBB RID: 4027
	public Color transitionBackgroundColor = new Color(0.5f, 0.5f, 0f, 1f);

	// Token: 0x04000FBC RID: 4028
	private float arrowPointAtGoalTimer;

	// Token: 0x04000FBD RID: 4029
	private float engineSoundTimer;

	// Token: 0x04000FBE RID: 4030
	public Transform engineSoundTransform;

	// Token: 0x04000FBF RID: 4031
	public Sound engineRevSound;

	// Token: 0x04000FC0 RID: 4032
	public Sound engineSuccessSound;

	// Token: 0x04000FC1 RID: 4033
	public TextMeshProUGUI textMesh;

	// Token: 0x04000FC2 RID: 4034
	public string testingText;

	// Token: 0x04000FC3 RID: 4035
	public List<TruckScreenText.TextPages> pages = new List<TruckScreenText.TextPages>();

	// Token: 0x04000FC4 RID: 4036
	private int currentPageIndex;

	// Token: 0x04000FC5 RID: 4037
	private int currentLineIndex;

	// Token: 0x04000FC6 RID: 4038
	private int currentCharIndex;

	// Token: 0x04000FC7 RID: 4039
	private float typingTimer;

	// Token: 0x04000FC8 RID: 4040
	internal float delayTimer;

	// Token: 0x04000FC9 RID: 4041
	internal bool isTyping;

	// Token: 0x04000FCA RID: 4042
	private float backgroundColorChangeTimer;

	// Token: 0x04000FCB RID: 4043
	private float backgroundColorChangeDuration = 0.5f;

	// Token: 0x04000FCC RID: 4044
	private float startWaitTimer;

	// Token: 0x04000FCD RID: 4045
	private bool started;

	// Token: 0x04000FCE RID: 4046
	private bool lobbyStarted;

	// Token: 0x02000357 RID: 855
	public enum ScreenType
	{
		// Token: 0x04002A51 RID: 10833
		TruckScreen,
		// Token: 0x04002A52 RID: 10834
		TruckLobbyScreen,
		// Token: 0x04002A53 RID: 10835
		TruckShopScreen
	}

	// Token: 0x02000358 RID: 856
	public enum TruckScreenPage
	{
		// Token: 0x04002A55 RID: 10837
		Start,
		// Token: 0x04002A56 RID: 10838
		EndNotEnough,
		// Token: 0x04002A57 RID: 10839
		EndEnough,
		// Token: 0x04002A58 RID: 10840
		AllPlayersInTruck
	}

	// Token: 0x02000359 RID: 857
	public enum LobbyScreenPage
	{
		// Token: 0x04002A5A RID: 10842
		Start,
		// Token: 0x04002A5B RID: 10843
		FailFirst,
		// Token: 0x04002A5C RID: 10844
		FailSecond,
		// Token: 0x04002A5D RID: 10845
		FailThird,
		// Token: 0x04002A5E RID: 10846
		HitRoad
	}

	// Token: 0x0200035A RID: 858
	public enum ShopScreenPage
	{
		// Token: 0x04002A60 RID: 10848
		Start,
		// Token: 0x04002A61 RID: 10849
		NotEnough,
		// Token: 0x04002A62 RID: 10850
		Enough,
		// Token: 0x04002A63 RID: 10851
		Stealing,
		// Token: 0x04002A64 RID: 10852
		AllPlayersInTruck
	}

	// Token: 0x0200035B RID: 859
	public enum PlayerChatBoxState
	{
		// Token: 0x04002A66 RID: 10854
		Idle,
		// Token: 0x04002A67 RID: 10855
		Typing,
		// Token: 0x04002A68 RID: 10856
		LockedDestroySlackers,
		// Token: 0x04002A69 RID: 10857
		LockedStartingTruck
	}

	// Token: 0x0200035C RID: 860
	[Serializable]
	public class CustomEmojiSounds
	{
		// Token: 0x04002A6A RID: 10858
		public string emojiString;

		// Token: 0x04002A6B RID: 10859
		public Sound emojiSound;
	}

	// Token: 0x0200035D RID: 861
	[Serializable]
	public class TextLine
	{
		// Token: 0x04002A6C RID: 10860
		public string messageName;

		// Token: 0x04002A6D RID: 10861
		[Multiline]
		public List<string> textLines = new List<string>();

		// Token: 0x04002A6E RID: 10862
		public bool customSoundEffects;

		// Token: 0x04002A6F RID: 10863
		public bool customEvents;

		// Token: 0x04002A70 RID: 10864
		public bool customRevealSettings;

		// Token: 0x04002A71 RID: 10865
		public Sound customMessageSoundEffect;

		// Token: 0x04002A72 RID: 10866
		public Sound customTypingSoundEffect;

		// Token: 0x04002A73 RID: 10867
		public UnityEvent onLineStart;

		// Token: 0x04002A74 RID: 10868
		public UnityEvent onLineEnd;

		// Token: 0x04002A75 RID: 10869
		public ScreenTextRevealDelaySettings typingSpeed;

		// Token: 0x04002A76 RID: 10870
		[HideInInspector]
		public float letterRevealDelay;

		// Token: 0x04002A77 RID: 10871
		public ScreenNextMessageDelaySettings messageDelayTime;

		// Token: 0x04002A78 RID: 10872
		[HideInInspector]
		public float delayAfter;

		// Token: 0x04002A79 RID: 10873
		[HideInInspector]
		public string text;

		// Token: 0x04002A7A RID: 10874
		[HideInInspector]
		public string textOriginal;
	}

	// Token: 0x0200035E RID: 862
	[Serializable]
	public class TextPages
	{
		// Token: 0x04002A7B RID: 10875
		public string pageName;

		// Token: 0x04002A7C RID: 10876
		public List<TruckScreenText.TextLine> textLines = new List<TruckScreenText.TextLine>();

		// Token: 0x04002A7D RID: 10877
		public bool goToNextPageAutomatically;
	}
}
