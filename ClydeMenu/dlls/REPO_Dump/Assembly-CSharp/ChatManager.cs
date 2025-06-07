using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020001EB RID: 491
public class ChatManager : MonoBehaviour
{
	// Token: 0x06001093 RID: 4243 RVA: 0x00099055 File Offset: 0x00097255
	private void Awake()
	{
		if (ChatManager.instance == null)
		{
			ChatManager.instance = this;
			Object.DontDestroyOnLoad(base.gameObject);
			return;
		}
		if (ChatManager.instance != this)
		{
			Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x06001094 RID: 4244 RVA: 0x0009908E File Offset: 0x0009728E
	private void SetChatColor(Color color)
	{
		this.chatText.color = color;
	}

	// Token: 0x06001095 RID: 4245 RVA: 0x0009909C File Offset: 0x0009729C
	public void ClearAllChatBatches()
	{
		this.possessBatchQueue.Clear();
		this.currentBatch = null;
	}

	// Token: 0x06001096 RID: 4246 RVA: 0x000990B0 File Offset: 0x000972B0
	public void ForceSendMessage(string _message)
	{
		this.chatMessage = _message;
		this.ForceConfirmChat();
	}

	// Token: 0x06001097 RID: 4247 RVA: 0x000990C0 File Offset: 0x000972C0
	private void CharRemoveEffect()
	{
		ChatUI.instance.SemiUITextFlashColor(Color.red, 0.2f);
		ChatUI.instance.SemiUISpringShakeX(5f, 5f, 0.2f);
		MenuManager.instance.MenuEffectClick(MenuManager.MenuClickEffectType.Dud, null, 2f, 1f, true);
	}

	// Token: 0x06001098 RID: 4248 RVA: 0x00099111 File Offset: 0x00097311
	public void AddLetterToChat(string letter)
	{
		this.prevChatMessage = this.chatMessage;
		this.chatMessage += letter;
		this.chatText.text = this.chatMessage;
	}

	// Token: 0x06001099 RID: 4249 RVA: 0x00099142 File Offset: 0x00097342
	public void ForceConfirmChat()
	{
		this.StateSet(ChatManager.ChatState.Send);
	}

	// Token: 0x0600109A RID: 4250 RVA: 0x0009914B File Offset: 0x0009734B
	private void ChatReset()
	{
		this.chatMessage = "";
	}

	// Token: 0x0600109B RID: 4251 RVA: 0x00099158 File Offset: 0x00097358
	private void PossessChatLovePotion()
	{
		this.playerAvatar.OverridePupilSize(3f, 4, 1f, 1f, 15f, 0.3f, 0.1f);
		this.playerAvatar.playerHealth.EyeMaterialOverride(PlayerHealth.EyeOverrideState.Love, 0.25f, 0);
	}

	// Token: 0x0600109C RID: 4252 RVA: 0x000991A8 File Offset: 0x000973A8
	private void PossessChatCustomLogic()
	{
		switch (this.activePossession)
		{
		case ChatManager.PossessChatID.LovePotion:
			this.PossessChatLovePotion();
			break;
		case ChatManager.PossessChatID.SelfDestruct:
			if (!this.playerAvatar)
			{
				return;
			}
			this.playerAvatar.playerHealth.EyeMaterialOverride(PlayerHealth.EyeOverrideState.Red, 0.25f, 0);
			break;
		case ChatManager.PossessChatID.Betrayal:
			if (!this.playerAvatar)
			{
				return;
			}
			this.playerAvatar.playerHealth.EyeMaterialOverride(PlayerHealth.EyeOverrideState.Red, 0.25f, 0);
			break;
		case ChatManager.PossessChatID.SelfDestructCancel:
			if (!this.playerAvatar)
			{
				return;
			}
			this.playerAvatar.playerHealth.EyeMaterialOverride(PlayerHealth.EyeOverrideState.Green, 0.25f, 0);
			break;
		}
		if (this.isSpeakingTimer > 0f)
		{
			this.isSpeakingTimer -= Time.deltaTime;
		}
		if (this.isSpeakingTimer < 0.2f && this.playerAvatar && this.playerAvatar.voiceChat && this.playerAvatar.voiceChat.ttsVoice && this.playerAvatar.voiceChat.ttsVoice.isSpeaking)
		{
			this.isSpeakingTimer = 0.2f;
		}
		if (this.isSpeakingTimer <= 0f && this.possessBatchQueue.Count == 0 && this.currentBatch == null)
		{
			this.currentPossessChatID = ChatManager.PossessChatID.None;
		}
	}

	// Token: 0x0600109D RID: 4253 RVA: 0x00099304 File Offset: 0x00097504
	public void PossessChatScheduleStart(int messagePrio)
	{
		bool flag = false;
		if (this.currentBatch != null && messagePrio < this.currentBatch.messagePrio)
		{
			this.InterruptCurrentPossessBatch();
			this.ChatReset();
			flag = true;
		}
		if (this.currentBatch == null)
		{
			flag = true;
		}
		if (flag)
		{
			this.isScheduling = true;
			this.scheduledBatch = new ChatManager.PossessMessageBatch(messagePrio);
		}
	}

	// Token: 0x0600109E RID: 4254 RVA: 0x00099357 File Offset: 0x00097557
	public void PossessChatScheduleEnd()
	{
		if (!this.isScheduling)
		{
			return;
		}
		this.isScheduling = false;
		this.EnqueuePossessBatch(this.scheduledBatch);
		this.scheduledBatch = null;
	}

	// Token: 0x0600109F RID: 4255 RVA: 0x0009937C File Offset: 0x0009757C
	public void PossessChat(ChatManager.PossessChatID _possessChatID, string message, float typingSpeed, Color _possessColor, float _messageDelay = 0f, bool sendInTaxmanChat = false, int sendInTaxmanChatEmojiInt = 0, UnityEvent eventExecutionAfterMessageIsDone = null)
	{
		this.isSpeakingTimer = 1f;
		ChatManager.PossessMessage possessMessage = new ChatManager.PossessMessage(_possessChatID, message, typingSpeed, _possessColor, _messageDelay, sendInTaxmanChat, sendInTaxmanChatEmojiInt, eventExecutionAfterMessageIsDone);
		if (this.isScheduling)
		{
			this.scheduledBatch.messages.Add(possessMessage);
		}
	}

	// Token: 0x060010A0 RID: 4256 RVA: 0x000993C0 File Offset: 0x000975C0
	private void EnqueuePossessBatch(ChatManager.PossessMessageBatch batch)
	{
		if (this.currentBatch == null)
		{
			this.StartPossessBatch(batch);
			return;
		}
		if (batch.messagePrio < this.currentBatch.messagePrio)
		{
			this.InterruptCurrentPossessBatch();
			this.StartPossessBatch(batch);
			return;
		}
		if (batch.messagePrio <= this.currentBatch.messagePrio)
		{
			this.possessBatchQueue.Add(batch);
		}
	}

	// Token: 0x060010A1 RID: 4257 RVA: 0x0009941D File Offset: 0x0009761D
	private void StartPossessBatch(ChatManager.PossessMessageBatch batch)
	{
		this.currentBatch = batch;
		this.currentBatch.isProcessing = true;
		this.currentMessageIndex = 0;
		this.StartPossessMessage(this.currentBatch.messages[this.currentMessageIndex]);
	}

	// Token: 0x060010A2 RID: 4258 RVA: 0x00099455 File Offset: 0x00097655
	private void InterruptCurrentPossessBatch()
	{
		this.ChatReset();
		this.currentBatch = null;
		this.possessBatchQueue.Clear();
		this.wasPossessed = false;
		this.wasPossessedPrio = 0;
	}

	// Token: 0x060010A3 RID: 4259 RVA: 0x0009947D File Offset: 0x0009767D
	private void StartPossessMessage(ChatManager.PossessMessage message)
	{
		this.ChatReset();
		this.possessLetterDelay = 0f;
		this.SetChatColor(message.possessColor);
		this.currentPossessMessage = message;
		this.StateSet(ChatManager.ChatState.Possessed);
		this.currentPossessChatID = message.possessChatID;
	}

	// Token: 0x060010A4 RID: 4260 RVA: 0x000994B6 File Offset: 0x000976B6
	private void PossessionReset()
	{
		this.currentPossessChatID = ChatManager.PossessChatID.None;
		this.currentBatch = null;
		this.possessBatchQueue.Clear();
		this.wasPossessed = false;
		this.wasPossessedPrio = 0;
		this.ChatReset();
		this.StateSet(ChatManager.ChatState.Inactive);
	}

	// Token: 0x060010A5 RID: 4261 RVA: 0x000994EC File Offset: 0x000976EC
	private void TypeEffect(Color _color)
	{
		ChatUI.instance.SemiUITextFlashColor(_color, 0.2f);
		ChatUI.instance.SemiUISpringShakeY(2f, 5f, 0.2f);
		MenuManager.instance.MenuEffectClick(MenuManager.MenuClickEffectType.Tick, null, 2f, 0.2f, true);
	}

	// Token: 0x060010A6 RID: 4262 RVA: 0x0009953C File Offset: 0x0009773C
	public void TumbleInterruption()
	{
		if (this.activePossessionTimer > 0f)
		{
			return;
		}
		this.PossessionReset();
		if (!this.playerAvatar || !this.playerAvatar.voiceChatFetched || !this.playerAvatar.voiceChat.ttsVoice.isSpeaking)
		{
			return;
		}
		List<string> list = new List<string>();
		list.Add("Ouch! Ouch! Ouch!");
		list.Add("Ow! Ow! Ow!");
		list.Add("Oof! Oof! Oof!");
		list.Add("Owie! Wowie! Zowie!");
		list.Add("Ouchie! Ouchie! Ouchie!");
		list.Add("error error error");
		list.Add("system error");
		list.Add("fatal error");
		list.Add("error 404");
		list.Add("runtime error");
		list.Add("imma falling");
		list.Add("falling over");
		list.Add("ooooooooh!");
		list.Add("oh nooooo!");
		list.Add("AAAAAAH! AAH!");
		list.Add("AAAAAAAAAAAAAAH!");
		list.Add("AAAAAAAAAAAAAAAAAAAAAAAAAAAH!");
		list.Add("OH! OH! OH!");
		list.Add("AH! AH! AH!");
		List<string> list2 = list;
		int num = Random.Range(0, list2.Count);
		string message = list2[num];
		this.PossessChatScheduleStart(3);
		this.PossessChat(ChatManager.PossessChatID.Ouch, message, 1f, Color.red, 0f, false, 0, null);
		this.PossessChatScheduleEnd();
	}

	// Token: 0x060010A7 RID: 4263 RVA: 0x000996A4 File Offset: 0x000978A4
	private void StateInactive()
	{
		ChatUI.instance.Hide();
		this.chatMessage = "";
		this.chatActive = false;
		if ((!MenuManager.instance.currentMenuPage || (MenuManager.instance.currentMenuPage.menuPageIndex != MenuPageIndex.Escape && MenuManager.instance.currentMenuPage.menuPageIndex != MenuPageIndex.Settings)) && SemiFunc.InputDown(InputKey.Chat))
		{
			TutorialDirector.instance.playerChatted = true;
			MenuManager.instance.MenuEffectClick(MenuManager.MenuClickEffectType.Action, null, 1f, 1f, true);
			this.chatActive = !this.chatActive;
			this.StateSet(ChatManager.ChatState.Active);
			this.chatHistoryIndex = 0;
		}
	}

	// Token: 0x060010A8 RID: 4264 RVA: 0x0009974C File Offset: 0x0009794C
	private void StateActive()
	{
		if (SemiFunc.InputDown(InputKey.Back))
		{
			this.StateSet(ChatManager.ChatState.Inactive);
			ChatUI.instance.SemiUISpringShakeX(10f, 10f, 0.3f);
			ChatUI.instance.SemiUISpringScale(0.05f, 5f, 0.2f);
			MenuManager.instance.MenuEffectClick(MenuManager.MenuClickEffectType.Deny, null, 1f, 1f, true);
			return;
		}
		if (Input.GetKeyDown(KeyCode.UpArrow) && this.chatHistory.Count > 0)
		{
			if (this.chatHistoryIndex > 0)
			{
				this.chatHistoryIndex--;
			}
			else
			{
				this.chatHistoryIndex = this.chatHistory.Count - 1;
			}
			this.chatMessage = this.chatHistory[this.chatHistoryIndex];
			this.chatText.text = this.chatMessage;
			ChatUI.instance.SemiUITextFlashColor(Color.cyan, 0.2f);
			ChatUI.instance.SemiUISpringShakeY(2f, 5f, 0.2f);
			MenuManager.instance.MenuEffectClick(MenuManager.MenuClickEffectType.Tick, null, 1f, 0.2f, true);
		}
		if (Input.GetKeyDown(KeyCode.DownArrow) && this.chatHistory.Count > 0)
		{
			if (this.chatHistoryIndex < this.chatHistory.Count - 1)
			{
				this.chatHistoryIndex++;
			}
			else
			{
				this.chatHistoryIndex = 0;
			}
			this.chatMessage = this.chatHistory[this.chatHistoryIndex];
			this.chatText.text = this.chatMessage;
			ChatUI.instance.SemiUITextFlashColor(Color.cyan, 0.2f);
			ChatUI.instance.SemiUISpringShakeY(2f, 5f, 0.2f);
			MenuManager.instance.MenuEffectClick(MenuManager.MenuClickEffectType.Tick, null, 1f, 0.2f, true);
		}
		SemiFunc.InputDisableMovement();
		if (SemiFunc.InputDown(InputKey.ChatDelete))
		{
			if (this.chatMessage.Length > 0)
			{
				this.chatMessage = this.chatMessage.Remove(this.chatMessage.Length - 1);
				this.chatText.text = this.chatMessage;
				this.CharRemoveEffect();
			}
		}
		else
		{
			if (this.chatMessage == "\b")
			{
				this.chatMessage = "";
			}
			this.prevChatMessage = this.chatMessage;
			string text = this.chatMessage;
			this.chatMessage += Input.inputString;
			this.chatMessage = this.chatMessage.Replace("\n", "");
			if (this.chatMessage.Length > 50)
			{
				ChatUI.instance.SemiUITextFlashColor(Color.red, 0.2f);
				ChatUI.instance.SemiUISpringShakeX(10f, 10f, 0.3f);
				ChatUI.instance.SemiUISpringScale(0.05f, 5f, 0.2f);
				MenuManager.instance.MenuEffectClick(MenuManager.MenuClickEffectType.Deny, null, 1f, 1f, true);
				this.chatMessage = text;
			}
			if (this.prevChatMessage != this.chatMessage)
			{
				bool flag = false;
				if (Input.inputString == "\b")
				{
					this.chatMessage = this.chatMessage.Remove(Mathf.Max(this.chatMessage.Length - 2, 0));
					flag = true;
				}
				else
				{
					this.chatText.text = this.chatMessage;
				}
				this.chatMessage = this.chatMessage.Replace("\r", "");
				this.prevChatMessage = this.chatMessage;
				if (!flag)
				{
					this.TypeEffect(Color.yellow);
				}
				else
				{
					this.CharRemoveEffect();
				}
			}
		}
		if (SemiFunc.InputDown(InputKey.Confirm))
		{
			if (this.chatMessage != "")
			{
				this.StateSet(ChatManager.ChatState.Send);
			}
			else
			{
				this.StateSet(ChatManager.ChatState.Inactive);
			}
		}
		if (Mathf.Sin(Time.time * 10f) > 0f)
		{
			this.chatText.text = this.chatMessage + "|";
		}
		else
		{
			this.chatText.text = this.chatMessage;
		}
		if (SemiFunc.InputDown(InputKey.Back))
		{
			this.StateSet(ChatManager.ChatState.Inactive);
		}
	}

	// Token: 0x060010A9 RID: 4265 RVA: 0x00099B70 File Offset: 0x00097D70
	private void StatePossessed()
	{
		this.chatActive = true;
		this.spamTimer = 0f;
		if (this.currentPossessMessage != null)
		{
			this.SetChatColor(this.currentPossessMessage.possessColor);
		}
		if (this.currentPossessMessage != null)
		{
			bool flag = false;
			if (this.currentPossessMessage.typingSpeed == -1f)
			{
				flag = true;
			}
			if (this.possessLetterDelay <= 0f)
			{
				if (this.currentPossessMessage.message.Length > 0 && !flag)
				{
					string letter = this.currentPossessMessage.message.get_Chars(0).ToString();
					this.currentPossessMessage.message = this.currentPossessMessage.message.Substring(1);
					this.possessLetterDelay = Random.Range(0.005f, 0.05f);
					this.TypeEffect(this.currentPossessMessage.possessColor);
					this.AddLetterToChat(letter);
					return;
				}
				if (this.isSpeakingTimer <= 0f || !this.wasPossessed || this.wasPossessedPrio > this.currentBatch.messagePrio)
				{
					if (this.currentPossessMessage.messageDelay > 0f)
					{
						this.currentPossessMessage.messageDelay -= Time.deltaTime;
						return;
					}
					if (flag)
					{
						this.chatMessage = this.currentPossessMessage.message;
					}
					this.wasPossessed = true;
					if (this.currentBatch != null)
					{
						this.wasPossessedPrio = this.currentBatch.messagePrio;
					}
					this.StateSet(ChatManager.ChatState.Send);
					return;
				}
			}
			else
			{
				this.possessLetterDelay -= Time.deltaTime * this.currentPossessMessage.typingSpeed;
				if (this.currentPossessMessage.typingSpeed == -1f)
				{
					this.possessLetterDelay = 0f;
				}
			}
			return;
		}
		this.currentMessageIndex++;
		if (this.currentBatch != null && this.currentMessageIndex < this.currentBatch.messages.Count)
		{
			this.StartPossessMessage(this.currentBatch.messages[this.currentMessageIndex]);
			return;
		}
		if (this.currentBatch != null && this.currentBatch.messages.Count == this.currentMessageIndex && this.currentBatch.isProcessing)
		{
			this.currentBatch.isProcessing = false;
			this.currentBatch = null;
		}
		if (this.possessBatchQueue.Count > 0)
		{
			this.StartPossessBatch(this.possessBatchQueue[0]);
			this.possessBatchQueue.RemoveAt(0);
			return;
		}
		this.StateSet(ChatManager.ChatState.Inactive);
		this.currentBatch = null;
	}

	// Token: 0x060010AA RID: 4266 RVA: 0x00099DE8 File Offset: 0x00097FE8
	private void SelfDestruct()
	{
		float delay = Random.Range(0.2f, 3f);
		base.StartCoroutine(this.SelfDestructCoroutine(delay));
	}

	// Token: 0x060010AB RID: 4267 RVA: 0x00099E14 File Offset: 0x00098014
	private void BetrayalSelfDestruct()
	{
		float delay = Random.Range(0.2f, 3f);
		base.StartCoroutine(this.SelfDestructCoroutine(delay));
	}

	// Token: 0x060010AC RID: 4268 RVA: 0x00099E40 File Offset: 0x00098040
	public void PossessLeftBehind()
	{
		if (!this.playerAvatar)
		{
			return;
		}
		if (this.playerAvatar.isDisabled)
		{
			return;
		}
		if (this.playerAvatar.RoomVolumeCheck.inTruck)
		{
			return;
		}
		this.betrayalActive = true;
		this.PossessChatScheduleStart(2);
		string message = SemiFunc.MessageGeneratedGetLeftBehind();
		this.PossessChat(ChatManager.PossessChatID.Betrayal, message, 0.5f, Color.red, 0f, true, 2, null);
		this.PossessChat(ChatManager.PossessChatID.Betrayal, "I need to get to the truck in...", 0.4f, Color.red, 0f, true, 2, null);
		this.PossessChat(ChatManager.PossessChatID.Betrayal, "10...", 0.25f, Color.red, 0f, true, 2, null);
		this.PossessChat(ChatManager.PossessChatID.Betrayal, "9...", 0.25f, Color.red, 0.3f, true, 2, null);
		this.PossessChat(ChatManager.PossessChatID.Betrayal, "8...", 0.25f, Color.red, 0.3f, true, 2, null);
		this.PossessChat(ChatManager.PossessChatID.Betrayal, "7...", 0.25f, Color.red, 0.3f, true, 2, null);
		this.PossessChat(ChatManager.PossessChatID.Betrayal, "6...", 0.25f, Color.red, 0.3f, true, 2, null);
		this.PossessChat(ChatManager.PossessChatID.Betrayal, "5...", 0.25f, Color.red, 0.3f, true, 2, null);
		this.PossessChat(ChatManager.PossessChatID.Betrayal, "4...", 0.25f, Color.red, 0.3f, true, 2, null);
		this.PossessChat(ChatManager.PossessChatID.Betrayal, "3...", 0.25f, Color.red, 0.3f, true, 2, null);
		this.PossessChat(ChatManager.PossessChatID.Betrayal, "2...", 0.25f, Color.red, 0.3f, true, 2, null);
		this.PossessChat(ChatManager.PossessChatID.Betrayal, "1...", 0.5f, Color.red, 0.3f, true, 2, null);
		UnityEvent unityEvent = new UnityEvent();
		unityEvent.AddListener(new UnityAction(this.BetrayalSelfDestruct));
		List<string> list = new List<string>();
		list.Add("betrayal");
		list.Add("betrayal detected");
		list.Add("i'm sorry");
		list.Add("I failed");
		list.Add("oh no, not again");
		list.Add("teamwork makes the dream work");
		list.Add("I thought we were friends");
		list.Add("I thought we were a team");
		list.Add("I thought we were in this together");
		list.Add("I thought we were a family");
		List<string> list2 = list;
		string message2 = list2[Random.Range(0, list2.Count)];
		this.PossessChat(ChatManager.PossessChatID.SelfDestruct, message2, 2f, Color.red, 0f, true, 2, unityEvent);
		this.PossessChatScheduleEnd();
	}

	// Token: 0x060010AD RID: 4269 RVA: 0x0009A0B4 File Offset: 0x000982B4
	public void PossessCancelSelfDestruction()
	{
		if (!this.playerAvatar)
		{
			return;
		}
		if (this.playerAvatar.isDisabled)
		{
			return;
		}
		this.PossessChatScheduleEnd();
		this.possessBatchQueue.Clear();
		this.currentBatch = null;
		this.betrayalActive = false;
		this.PossessChatScheduleStart(1);
		this.PossessChat(ChatManager.PossessChatID.SelfDestructCancel, "SELF DESTRUCT SEQUENCE CANCELLED!", 2f, Color.green, 0f, false, 0, null);
		this.PossessChatScheduleEnd();
	}

	// Token: 0x060010AE RID: 4270 RVA: 0x0009A128 File Offset: 0x00098328
	public void PossessSelfDestruction()
	{
		if (!this.playerAvatar)
		{
			return;
		}
		if (this.playerAvatar.isDisabled)
		{
			return;
		}
		this.PossessChatScheduleStart(-1);
		UnityEvent unityEvent = new UnityEvent();
		unityEvent.AddListener(new UnityAction(this.SelfDestruct));
		List<string> list = new List<string>();
		list.Add("i'm out");
		list.Add("Farewell");
		list.Add("Adieu");
		list.Add("sayonara");
		list.Add("Auf Wiedersehen");
		list.Add("adios");
		list.Add("ciao");
		list.Add("Au Revoir");
		list.Add("hasta la vista");
		list.Add("see You Later");
		list.Add("later");
		list.Add("peace OUT");
		list.Add("catch you later");
		list.Add("later gator");
		list.Add("toodles");
		list.Add("bye bye");
		list.Add("bye");
		list.Add("AAAAAAAAAAAAH!");
		list.Add("AAAAAAAAAAAAAAAAAAAAAAAH!");
		list.Add("bye... ... oh?");
		list.Add("this will hurt");
		list.Add("it's over for me");
		list.Add("why me?");
		list.Add("I'm sorry");
		list.Add("i see the light");
		list.Add("sad but necessary");
		list.Add("HEJ DÅ!");
		List<string> list2 = list;
		string message = list2[Random.Range(0, list2.Count)];
		this.PossessChat(ChatManager.PossessChatID.SelfDestruct, message, 2f, Color.red, 0f, true, 2, unityEvent);
		this.PossessChatScheduleEnd();
	}

	// Token: 0x060010AF RID: 4271 RVA: 0x0009A2D2 File Offset: 0x000984D2
	private IEnumerator BetrayalSelfDestructCoroutine(float delay)
	{
		yield return new WaitForSeconds(delay);
		if (this.betrayalActive)
		{
			PlayerAvatar.instance.playerHealth.health = 0;
			PlayerAvatar.instance.playerHealth.Hurt(1, false, -1);
		}
		yield break;
	}

	// Token: 0x060010B0 RID: 4272 RVA: 0x0009A2E8 File Offset: 0x000984E8
	private IEnumerator SelfDestructCoroutine(float delay)
	{
		yield return new WaitForSeconds(delay);
		PlayerAvatar.instance.playerHealth.health = 0;
		PlayerAvatar.instance.playerHealth.Hurt(1, false, -1);
		yield break;
	}

	// Token: 0x060010B1 RID: 4273 RVA: 0x0009A2F7 File Offset: 0x000984F7
	public bool IsPossessed(ChatManager.PossessChatID _possessChatID)
	{
		return this.activePossession == _possessChatID;
	}

	// Token: 0x060010B2 RID: 4274 RVA: 0x0009A304 File Offset: 0x00098504
	private void StateSend()
	{
		bool possessed = false;
		if (this.currentPossessMessage != null && this.currentPossessMessage.sendInTaxmanChat && TruckScreenText.instance)
		{
			TruckScreenText.instance.MessageSendCustom(PlayerController.instance.playerName, this.chatMessage, this.currentPossessMessage.sendInTaxmanChatEmojiInt);
		}
		if (this.currentPossessMessage != null)
		{
			possessed = true;
		}
		this.MessageSend(possessed);
		if (this.currentPossessMessage != null && this.currentPossessMessage.eventExecutionAfterMessageIsDone != null)
		{
			this.currentPossessMessage.eventExecutionAfterMessageIsDone.Invoke();
		}
		this.currentPossessMessage = null;
		this.StateSet(ChatManager.ChatState.Possessed);
	}

	// Token: 0x060010B3 RID: 4275 RVA: 0x0009A39D File Offset: 0x0009859D
	private void StateSet(ChatManager.ChatState state)
	{
		this.chatState = state;
	}

	// Token: 0x060010B4 RID: 4276 RVA: 0x0009A3A8 File Offset: 0x000985A8
	private void ImportantFetches()
	{
		if (!this.chatText)
		{
			this.textMeshFetched = false;
		}
		if (!this.playerAvatar)
		{
			this.localPlayerAvatarFetched = false;
		}
		if (!this.textMeshFetched && ChatUI.instance && ChatUI.instance.chatText)
		{
			this.chatText = ChatUI.instance.chatText;
			this.textMeshFetched = true;
		}
		if (!this.localPlayerAvatarFetched)
		{
			if (SemiFunc.IsMultiplayer())
			{
				List<PlayerAvatar> list = SemiFunc.PlayerGetList();
				if (list.Count <= 0)
				{
					return;
				}
				using (List<PlayerAvatar>.Enumerator enumerator = list.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						PlayerAvatar playerAvatar = enumerator.Current;
						if (playerAvatar.isLocal)
						{
							this.playerAvatar = playerAvatar;
							this.localPlayerAvatarFetched = true;
							break;
						}
					}
					return;
				}
			}
			this.playerAvatar = PlayerAvatar.instance;
			this.localPlayerAvatarFetched = true;
		}
	}

	// Token: 0x060010B5 RID: 4277 RVA: 0x0009A49C File Offset: 0x0009869C
	private void NewLevelResets()
	{
		this.betrayalActive = false;
		this.localPlayerAvatarFetched = false;
		this.textMeshFetched = false;
		this.PossessionReset();
	}

	// Token: 0x060010B6 RID: 4278 RVA: 0x0009A4BC File Offset: 0x000986BC
	private void PossessionActive()
	{
		if (this.activePossessionTimer <= 0f)
		{
			this.activePossession = ChatManager.PossessChatID.None;
		}
		if (this.activePossessionTimer > 0f)
		{
			this.activePossessionTimer -= Time.deltaTime;
		}
		if (this.currentPossessChatID != ChatManager.PossessChatID.None || (this.activePossession != ChatManager.PossessChatID.None && this.isSpeakingTimer > 0f))
		{
			this.activePossessionTimer = 0.5f;
			this.activePossession = this.currentPossessChatID;
		}
	}

	// Token: 0x060010B7 RID: 4279 RVA: 0x0009A530 File Offset: 0x00098730
	private void Update()
	{
		this.PossessionActive();
		if (this.playerAvatar && this.playerAvatar.isDisabled && (this.possessBatchQueue.Count > 0 || this.currentBatch != null))
		{
			this.InterruptCurrentPossessBatch();
		}
		if (!SemiFunc.IsMultiplayer())
		{
			ChatUI.instance.Hide();
			return;
		}
		if (!LevelGenerator.Instance.Generated)
		{
			this.NewLevelResets();
			return;
		}
		this.ImportantFetches();
		this.PossessChatCustomLogic();
		if (!this.textMeshFetched || !this.localPlayerAvatarFetched)
		{
			return;
		}
		switch (this.chatState)
		{
		case ChatManager.ChatState.Inactive:
			this.StateInactive();
			break;
		case ChatManager.ChatState.Active:
			this.StateActive();
			break;
		case ChatManager.ChatState.Possessed:
			this.StatePossessed();
			break;
		case ChatManager.ChatState.Send:
			this.StateSend();
			break;
		}
		this.PossessChatCustomLogic();
		if (!SemiFunc.IsMultiplayer())
		{
			if (this.chatState != ChatManager.ChatState.Inactive)
			{
				this.StateSet(ChatManager.ChatState.Inactive);
			}
			this.chatActive = false;
			return;
		}
		if (this.spamTimer > 0f)
		{
			this.spamTimer -= Time.deltaTime;
		}
		if (SemiFunc.FPSImpulse15() && this.betrayalActive && PlayerController.instance.playerAvatarScript.RoomVolumeCheck.inTruck)
		{
			this.PossessCancelSelfDestruction();
		}
	}

	// Token: 0x060010B8 RID: 4280 RVA: 0x0009A666 File Offset: 0x00098866
	public bool StateIsActive()
	{
		return this.chatState == ChatManager.ChatState.Active;
	}

	// Token: 0x060010B9 RID: 4281 RVA: 0x0009A671 File Offset: 0x00098871
	public bool StateIsPossessed()
	{
		return this.chatState == ChatManager.ChatState.Possessed;
	}

	// Token: 0x060010BA RID: 4282 RVA: 0x0009A67C File Offset: 0x0009887C
	public bool StateIsSend()
	{
		return this.chatState == ChatManager.ChatState.Send;
	}

	// Token: 0x060010BB RID: 4283 RVA: 0x0009A687 File Offset: 0x00098887
	public bool StateIsInactive()
	{
		return this.chatState == ChatManager.ChatState.Inactive;
	}

	// Token: 0x060010BC RID: 4284 RVA: 0x0009A694 File Offset: 0x00098894
	private void MessageSend(bool _possessed = false)
	{
		if (this.chatMessage == "")
		{
			return;
		}
		if (this.spamTimer <= 0f)
		{
			this.playerAvatar.ChatMessageSend(this.chatMessage);
			if (!_possessed)
			{
				this.chatHistory.Add(this.chatMessage);
			}
			if (this.chatHistory.Count > 20)
			{
				this.chatHistory.RemoveAt(0);
			}
			this.chatHistory = Enumerable.ToList<string>(Enumerable.Reverse<string>(Enumerable.Distinct<string>(Enumerable.Reverse<string>(Enumerable.AsEnumerable<string>(this.chatHistory)))));
			this.ChatReset();
			this.chatText.text = this.chatMessage;
			this.chatActive = false;
			this.isSpeakingTimer = 0.2f;
			ChatUI.instance.SemiUITextFlashColor(Color.green, 0.2f);
			ChatUI.instance.SemiUISpringShakeX(10f, 10f, 0.3f);
			ChatUI.instance.SemiUISpringScale(0.05f, 5f, 0.2f);
			MenuManager.instance.MenuEffectClick(MenuManager.MenuClickEffectType.Confirm, null, 1f, 1f, true);
			this.spamTimer = 1f;
		}
	}

	// Token: 0x04001C72 RID: 7282
	public static ChatManager instance;

	// Token: 0x04001C73 RID: 7283
	internal bool chatActive;

	// Token: 0x04001C74 RID: 7284
	internal bool localPlayerAvatarFetched;

	// Token: 0x04001C75 RID: 7285
	internal bool textMeshFetched;

	// Token: 0x04001C76 RID: 7286
	internal PlayerAvatar playerAvatar;

	// Token: 0x04001C77 RID: 7287
	internal string prevChatMessage = "";

	// Token: 0x04001C78 RID: 7288
	internal string chatMessage = "";

	// Token: 0x04001C79 RID: 7289
	public TextMeshProUGUI chatText;

	// Token: 0x04001C7A RID: 7290
	private float spamTimer;

	// Token: 0x04001C7B RID: 7291
	private List<string> chatHistory = new List<string>();

	// Token: 0x04001C7C RID: 7292
	private int chatHistoryIndex;

	// Token: 0x04001C7D RID: 7293
	private float possessLetterDelay;

	// Token: 0x04001C7E RID: 7294
	private bool wasPossessed;

	// Token: 0x04001C7F RID: 7295
	private int wasPossessedPrio;

	// Token: 0x04001C80 RID: 7296
	private bool betrayalActive;

	// Token: 0x04001C81 RID: 7297
	internal ChatManager.PossessChatID activePossession;

	// Token: 0x04001C82 RID: 7298
	internal float activePossessionTimer;

	// Token: 0x04001C83 RID: 7299
	public ChatManager.PossessChatID currentPossessChatID;

	// Token: 0x04001C84 RID: 7300
	private List<ChatManager.PossessMessageBatch> possessBatchQueue = new List<ChatManager.PossessMessageBatch>();

	// Token: 0x04001C85 RID: 7301
	private ChatManager.PossessMessageBatch currentBatch;

	// Token: 0x04001C86 RID: 7302
	private int currentMessageIndex;

	// Token: 0x04001C87 RID: 7303
	private bool isScheduling;

	// Token: 0x04001C88 RID: 7304
	private ChatManager.PossessMessageBatch scheduledBatch;

	// Token: 0x04001C89 RID: 7305
	private float isSpeakingTimer;

	// Token: 0x04001C8A RID: 7306
	private ChatManager.ChatState chatState;

	// Token: 0x04001C8B RID: 7307
	private ChatManager.PossessMessage currentPossessMessage;

	// Token: 0x020003CE RID: 974
	public enum PossessChatID
	{
		// Token: 0x04002C6B RID: 11371
		None,
		// Token: 0x04002C6C RID: 11372
		LovePotion,
		// Token: 0x04002C6D RID: 11373
		Ouch,
		// Token: 0x04002C6E RID: 11374
		SelfDestruct,
		// Token: 0x04002C6F RID: 11375
		Betrayal,
		// Token: 0x04002C70 RID: 11376
		SelfDestructCancel
	}

	// Token: 0x020003CF RID: 975
	public enum ChatState
	{
		// Token: 0x04002C72 RID: 11378
		Inactive,
		// Token: 0x04002C73 RID: 11379
		Active,
		// Token: 0x04002C74 RID: 11380
		Possessed,
		// Token: 0x04002C75 RID: 11381
		Send
	}

	// Token: 0x020003D0 RID: 976
	public class PossessMessage
	{
		// Token: 0x06001A48 RID: 6728 RVA: 0x000D4608 File Offset: 0x000D2808
		public PossessMessage(ChatManager.PossessChatID _possessChatID, string message, float typingSpeed, Color possessColor, float messageDelay, bool sendInTaxmanChat, int sendInTaxmanChatEmojiInt, UnityEvent eventExecutionAfterMessageIsDone)
		{
			this.possessChatID = _possessChatID;
			this.message = message;
			this.typingSpeed = typingSpeed;
			this.possessColor = possessColor;
			this.messageDelay = messageDelay;
			this.sendInTaxmanChat = sendInTaxmanChat;
			this.sendInTaxmanChatEmojiInt = sendInTaxmanChatEmojiInt;
			this.eventExecutionAfterMessageIsDone = eventExecutionAfterMessageIsDone;
		}

		// Token: 0x04002C76 RID: 11382
		public ChatManager.PossessChatID possessChatID;

		// Token: 0x04002C77 RID: 11383
		public string message;

		// Token: 0x04002C78 RID: 11384
		public float typingSpeed;

		// Token: 0x04002C79 RID: 11385
		public Color possessColor;

		// Token: 0x04002C7A RID: 11386
		public float messageDelay;

		// Token: 0x04002C7B RID: 11387
		public bool sendInTaxmanChat;

		// Token: 0x04002C7C RID: 11388
		public int sendInTaxmanChatEmojiInt;

		// Token: 0x04002C7D RID: 11389
		public UnityEvent eventExecutionAfterMessageIsDone;
	}

	// Token: 0x020003D1 RID: 977
	public class PossessMessageBatch
	{
		// Token: 0x06001A49 RID: 6729 RVA: 0x000D4658 File Offset: 0x000D2858
		public PossessMessageBatch(int messagePrio)
		{
			this.messagePrio = messagePrio;
		}

		// Token: 0x04002C7E RID: 11390
		public List<ChatManager.PossessMessage> messages = new List<ChatManager.PossessMessage>();

		// Token: 0x04002C7F RID: 11391
		public int messagePrio;

		// Token: 0x04002C80 RID: 11392
		public bool isProcessing;
	}
}
