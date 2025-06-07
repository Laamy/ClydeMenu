using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

// Token: 0x0200028C RID: 652
public class TutorialDirector : MonoBehaviour
{
	// Token: 0x0600145A RID: 5210 RVA: 0x000B3AED File Offset: 0x000B1CED
	private void Awake()
	{
		if (!TutorialDirector.instance)
		{
			TutorialDirector.instance = this;
			Object.DontDestroyOnLoad(base.gameObject);
			return;
		}
		Object.Destroy(base.gameObject);
	}

	// Token: 0x0600145B RID: 5211 RVA: 0x000B3B18 File Offset: 0x000B1D18
	private void Start()
	{
		this.currentPage = -1;
	}

	// Token: 0x0600145C RID: 5212 RVA: 0x000B3B24 File Offset: 0x000B1D24
	private void FixedUpdate()
	{
		if (SemiFunc.RunIsArena())
		{
			return;
		}
		if (SemiFunc.RunIsLobbyMenu())
		{
			return;
		}
		if (SemiFunc.MenuLevel())
		{
			return;
		}
		if (this.tutorialActiveTimer > 0f)
		{
			this.tutorialActiveTimer -= Time.fixedDeltaTime;
			this.tutorialActive = true;
		}
		else
		{
			this.tutorialActive = false;
		}
		this.TipBoolChecks();
	}

	// Token: 0x0600145D RID: 5213 RVA: 0x000B3B80 File Offset: 0x000B1D80
	private void Update()
	{
		if (this.scheduleTipTimer > 0f)
		{
			this.scheduleTipTimer -= Time.deltaTime;
			if (this.scheduleTipTimer <= 0f)
			{
				this.ActivateTip(this.scheduleTipName, 0f, true);
				this.scheduleTipTimer = 0f;
			}
		}
		if (SemiFunc.RunIsArena() || SemiFunc.RunIsLobbyMenu() || SemiFunc.MenuLevel())
		{
			TutorialUI.instance.Hide();
			return;
		}
		if (!this.tutorialActive)
		{
			TutorialUI.instance.Hide();
			this.tutorialCheckActiveTimer -= Time.deltaTime;
			if (this.tutorialCheckActiveTimer < 0f)
			{
				this.tutorialCheckActiveTimer = 0.5f;
				if (LevelGenerator.Instance.Level == RunManager.instance.levelTutorial)
				{
					this.tutorialActive = true;
					this.TutorialActive();
				}
			}
			TutorialUI.instance.Hide();
			this.TipsTick();
			return;
		}
		this.TutorialActive();
		if (this.tutorialActive)
		{
			if (this.currentPage == -1)
			{
				this.NextPage();
			}
			SemiFunc.UIFocusText(this.tutorialPages[this.currentPage].focusText, Color.white, AssetManager.instance.colorYellow, 0.2f);
			if (this.currentPage < 6)
			{
				HealthUI.instance.Hide();
			}
			if (this.currentPage < 14)
			{
				HaulUI.instance.Hide();
				CurrencyUI.instance.Hide();
				GoalUI.instance.Hide();
			}
			if (this.currentPage < 4)
			{
				EnergyUI.instance.Hide();
			}
			if (this.currentPage < 10)
			{
				InventoryUI.instance.Hide();
			}
			if (this.currentPage == 0)
			{
				this.TaskMove();
			}
			if (this.currentPage == 1)
			{
				this.TaskJump();
			}
			if (this.currentPage == 2)
			{
				this.TaskSneak();
			}
			if (this.currentPage == 3)
			{
				this.TaskSneakUnder();
			}
			if (this.currentPage == 4)
			{
				this.TaskSprint();
			}
			if (this.currentPage == 5)
			{
				this.TaskTumble();
			}
			if (this.currentPage == 6)
			{
				this.TaskGrab();
			}
			if (this.currentPage == 7)
			{
				this.TaskPushAndPull();
			}
			if (this.currentPage == 8)
			{
				this.TaskRotate();
			}
			if (this.currentPage == 9)
			{
				this.TaskInteract();
			}
			if (this.currentPage == 10)
			{
				this.TaskInventoryFill();
			}
			if (this.currentPage == 11)
			{
				this.TaskInventoryEmpty();
			}
			if (this.currentPage == 12)
			{
				this.TaskMap();
			}
			if (this.currentPage == 13)
			{
				this.TaskCartMove();
			}
			if (this.currentPage == 14)
			{
				this.TaskCartFill();
			}
			if (this.currentPage == 15)
			{
				this.TaskExtractionPoint();
			}
			if (this.currentPage == 16)
			{
				this.TaskEnterTuck();
			}
			if (this.arrowDelay > 0f)
			{
				this.arrowDelay -= Time.deltaTime;
			}
			if (TutorialUI.instance.progressBarCurrent > 0.98f && this.tutorialProgress > 0.98f)
			{
				this.NextPage();
				this.tutorialProgress = 0f;
				TutorialUI.instance.animationCurveEval = 0f;
				TutorialUI.instance.progressBar.localScale = new Vector3(0f, 1f, 1f);
				TutorialUI.instance.progressBarCurrent = 0f;
			}
		}
	}

	// Token: 0x0600145E RID: 5214 RVA: 0x000B3EB4 File Offset: 0x000B20B4
	public void SetPageID(string pageName)
	{
		for (int i = 0; i < this.tutorialPages.Count; i++)
		{
			if (this.tutorialPages[i].pageName == pageName)
			{
				this.currentPage = i;
				return;
			}
		}
	}

	// Token: 0x0600145F RID: 5215 RVA: 0x000B3EF8 File Offset: 0x000B20F8
	public void NextPage()
	{
		this.currentPage++;
		if (this.currentPage > this.tutorialPages.Count - 1)
		{
			this.currentPage = this.tutorialPages.Count - 1;
		}
		int num = this.currentPage;
		string text = this.tutorialPages[num].text;
		string text2 = this.tutorialPages[num].dummyText;
		text2 = InputManager.instance.InputDisplayReplaceTags(text2);
		VideoClip video = this.tutorialPages[num].video;
		text = InputManager.instance.InputDisplayReplaceTags(text);
		if (num == 0)
		{
			TutorialUI.instance.SetPage(video, text, text2, false);
		}
		else
		{
			TutorialUI.instance.SetPage(video, text, text2, true);
		}
		this.arrowDelay = 4f;
	}

	// Token: 0x06001460 RID: 5216 RVA: 0x000B3FBC File Offset: 0x000B21BC
	private void TipsClear()
	{
		this.potentialTips.Clear();
	}

	// Token: 0x06001461 RID: 5217 RVA: 0x000B3FCC File Offset: 0x000B21CC
	public void TipsStore()
	{
		if (!this.playerJumped && this.TutorialSettingCheck(DataDirector.Setting.TutorialJumping, 3))
		{
			this.potentialTips.Add("Jumping");
		}
		if (!this.playerSprinted && this.TutorialSettingCheck(DataDirector.Setting.TutorialSprinting, 3))
		{
			this.potentialTips.Add("Sprinting");
		}
		if (!this.playerCrouched && this.TutorialSettingCheck(DataDirector.Setting.TutorialSneaking, 3))
		{
			this.potentialTips.Add("Sneaking");
		}
		if (!this.playerCrawled && this.TutorialSettingCheck(DataDirector.Setting.TutorialHiding, 3))
		{
			this.potentialTips.Add("Hiding");
		}
		if (!this.playerTumbled && this.TutorialSettingCheck(DataDirector.Setting.TutorialTumbling, 3))
		{
			this.potentialTips.Add("Tumbling");
		}
		if (!this.playerPushedAndPulled && this.TutorialSettingCheck(DataDirector.Setting.TutorialPushingAndPulling, 3))
		{
			this.potentialTips.Add("Pushing and Pulling");
		}
		if (!this.playerRotated && this.TutorialSettingCheck(DataDirector.Setting.TutorialRotating, 3))
		{
			this.potentialTips.Add("Rotating");
		}
		if (SemiFunc.IsMultiplayer())
		{
			if (this.playerSawHead && !this.playerRevived && this.TutorialSettingCheck(DataDirector.Setting.TutorialReviving, 3))
			{
				this.playerReviveTipDone = true;
				this.potentialTips.Add("Reviving");
			}
			if (!this.playerHealed && this.TutorialSettingCheck(DataDirector.Setting.TutorialHealing, 3))
			{
				bool flag = true;
				bool flag2 = false;
				foreach (PlayerAvatar playerAvatar in SemiFunc.PlayerGetList())
				{
					if (playerAvatar.isLocal)
					{
						if (playerAvatar.playerHealth.health > 50)
						{
							flag2 = true;
						}
					}
					else if (playerAvatar.playerHealth.health < 50)
					{
						flag = false;
					}
				}
				if (flag2 && !flag)
				{
					this.potentialTips.Add("Healing");
				}
			}
			if (!this.playerChatted && this.numberOfRoundsWithoutChatting > 2 && this.TutorialSettingCheck(DataDirector.Setting.TutorialChat, 3))
			{
				this.potentialTips.Add("Chat");
			}
		}
		if (!this.playerUsedCart && this.numberOfRoundsWithoutCart > 2 && this.TutorialSettingCheck(DataDirector.Setting.TutorialCartHandling, 3))
		{
			this.potentialTips.Add("Cart Handling 2");
		}
		if (!this.playerUsedToggle && this.numberOfRoundsWithoutCart > 5 && this.TutorialSettingCheck(DataDirector.Setting.TutorialItemToggling, 3))
		{
			this.potentialTips.Add("Item Toggling");
		}
		if (!this.playerHadItemsAndUsedInventory && this.numberOfRoundsWithoutInventory > 3 && this.TutorialSettingCheck(DataDirector.Setting.TutorialInventoryFill, 3))
		{
			this.potentialTips.Add("Inventory Fill");
		}
		if (!this.playerUsedMap && this.numberOfRoundsWithoutMap > 1 && this.TutorialSettingCheck(DataDirector.Setting.TutorialMap, 3))
		{
			this.potentialTips.Add("Map");
		}
		if (!this.playerUsedChargingStation && this.numberOfRoundsWithoutCharging > 5 && this.TutorialSettingCheck(DataDirector.Setting.TutorialChargingStation, 3))
		{
			this.potentialTips.Add("Charging Station");
		}
		if (!this.playerUsedExpression && this.TutorialSettingCheck(DataDirector.Setting.TutorialExpressions, 3))
		{
			this.potentialTips.Add("Expressions");
		}
	}

	// Token: 0x06001462 RID: 5218 RVA: 0x000B42CC File Offset: 0x000B24CC
	public void TipsShow()
	{
		if (SemiFunc.RunIsArena())
		{
			return;
		}
		if (SemiFunc.RunIsLobbyMenu())
		{
			return;
		}
		if (SemiFunc.MenuLevel())
		{
			return;
		}
		for (int i = 0; i < this.potentialTips.Count; i++)
		{
			for (int j = 0; j < this.shownTips.Count; j++)
			{
				if (this.potentialTips[i] == this.shownTips[j])
				{
					this.potentialTips.RemoveAt(i);
					i--;
					break;
				}
			}
		}
		if (this.potentialTips.Count > 0)
		{
			int num = Random.Range(0, this.potentialTips.Count);
			this.ActivateTip(this.potentialTips[num], 4f, true);
		}
		this.TipsClear();
	}

	// Token: 0x06001463 RID: 5219 RVA: 0x000B438C File Offset: 0x000B258C
	public void ActivateTip(string tipName, float _delay, bool _interrupt)
	{
		if (!GameplayManager.instance.tips)
		{
			return;
		}
		if (!_interrupt && (this.delayBeforeTip > 0f || this.showTipTimer > 0f))
		{
			return;
		}
		this.TutorialSettingSet(tipName);
		this.shownTips.Add(tipName);
		this.SetPageID(tipName);
		this.SetTipPageUI();
		this.delayBeforeTip = _delay;
		this.showTipTimer = 12f;
	}

	// Token: 0x06001464 RID: 5220 RVA: 0x000B43F6 File Offset: 0x000B25F6
	public void ScheduleTip(string tipName, float _timer, bool _interrupt)
	{
		if (!GameplayManager.instance.tips)
		{
			return;
		}
		if (this.scheduleTipTimer <= 0f || _interrupt)
		{
			this.scheduleTipName = tipName;
			this.scheduleTipTimer = _timer;
		}
	}

	// Token: 0x06001465 RID: 5221 RVA: 0x000B4428 File Offset: 0x000B2628
	private void SetTipPageUI()
	{
		if (this.currentPage == -1)
		{
			return;
		}
		int num = this.currentPage;
		string text = this.tutorialPages[num].text;
		VideoClip video = this.tutorialPages[num].video;
		text = InputManager.instance.InputDisplayReplaceTags(text);
		TutorialUI.instance.SetTipPage(video, text);
	}

	// Token: 0x06001466 RID: 5222 RVA: 0x000B4484 File Offset: 0x000B2684
	private void TipBoolChecks()
	{
		if (SemiFunc.RunIsArena())
		{
			return;
		}
		if (SemiFunc.RunIsLobbyMenu())
		{
			return;
		}
		if (SemiFunc.MenuLevel())
		{
			return;
		}
		if (this.tutorialActive)
		{
			return;
		}
		if (!LevelGenerator.Instance.Generated)
		{
			return;
		}
		if (PhysGrabber.instance.isRotating)
		{
			this.playerRotated = true;
		}
		if (Map.Instance.Active)
		{
			this.playerUsedMap = true;
		}
		if (SemiFunc.FPSImpulse1())
		{
			bool flag = Inventory.instance && Inventory.instance.inventorySpots != null && Inventory.instance.InventorySpotsOccupied() > 0;
			if (!SemiFunc.RunIsShop() && ItemManager.instance.purchasedItems.Count > 2 && flag)
			{
				this.playerHadItemsAndUsedInventory = true;
			}
		}
	}

	// Token: 0x06001467 RID: 5223 RVA: 0x000B453C File Offset: 0x000B273C
	public void TipCancel()
	{
		this.showTipTimer = 0f;
		this.tutorialProgress = 0f;
	}

	// Token: 0x06001468 RID: 5224 RVA: 0x000B4554 File Offset: 0x000B2754
	private void TipsTick()
	{
		if (this.delayBeforeTip > 0f)
		{
			this.delayBeforeTip -= Time.deltaTime;
			return;
		}
		if (this.showTipTimer > 0f)
		{
			this.showTipTimer -= Time.deltaTime;
			this.tutorialProgress = 1f - this.showTipTimer / 12f;
			TutorialUI.instance.Show();
		}
	}

	// Token: 0x06001469 RID: 5225 RVA: 0x000B45C2 File Offset: 0x000B27C2
	private void TutorialProgressFill(float amount)
	{
		this.tutorialProgress += amount * Time.deltaTime;
	}

	// Token: 0x0600146A RID: 5226 RVA: 0x000B45D8 File Offset: 0x000B27D8
	public void EndTutorial()
	{
		Object.Destroy(base.gameObject);
	}

	// Token: 0x0600146B RID: 5227 RVA: 0x000B45E5 File Offset: 0x000B27E5
	public bool TutorialSettingCheck(DataDirector.Setting _setting, int _max)
	{
		return DataDirector.instance.SettingValueFetch(_setting) < _max;
	}

	// Token: 0x0600146C RID: 5228 RVA: 0x000B45F8 File Offset: 0x000B27F8
	private void TutorialSettingSet(string _tutorial)
	{
		DataDirector.Setting setting = DataDirector.Setting.TutorialJumping;
		uint num = <PrivateImplementationDetails>.ComputeStringHash(_tutorial);
		if (num <= 1668480752U)
		{
			if (num <= 893207528U)
			{
				if (num <= 578410699U)
				{
					if (num != 183262573U)
					{
						if (num == 578410699U)
						{
							if (_tutorial == "Chat")
							{
								setting = DataDirector.Setting.TutorialChat;
							}
						}
					}
					else if (_tutorial == "Item Toggling")
					{
						setting = DataDirector.Setting.TutorialItemToggling;
					}
				}
				else if (num != 678702281U)
				{
					if (num != 690938701U)
					{
						if (num == 893207528U)
						{
							if (_tutorial == "Expressions")
							{
								setting = DataDirector.Setting.TutorialExpressions;
							}
						}
					}
					else if (_tutorial == "Rotating")
					{
						setting = DataDirector.Setting.TutorialRotating;
					}
				}
				else if (_tutorial == "Shop")
				{
					setting = DataDirector.Setting.TutorialShop;
				}
			}
			else if (num <= 1151856721U)
			{
				if (num != 1057014026U)
				{
					if (num == 1151856721U)
					{
						if (_tutorial == "Map")
						{
							setting = DataDirector.Setting.TutorialMap;
						}
					}
				}
				else if (_tutorial == "Charging Station")
				{
					setting = DataDirector.Setting.TutorialChargingStation;
				}
			}
			else if (num != 1164769509U)
			{
				if (num != 1522540328U)
				{
					if (num == 1668480752U)
					{
						if (_tutorial == "Final Extraction")
						{
							setting = DataDirector.Setting.TutorialFinalExtraction;
						}
					}
				}
				else if (_tutorial == "Only One Extraction")
				{
					setting = DataDirector.Setting.TutorialOnlyOneExtraction;
				}
			}
			else if (_tutorial == "Tumbling")
			{
				setting = DataDirector.Setting.TutorialTumbling;
			}
		}
		else if (num <= 3538838859U)
		{
			if (num <= 2128769280U)
			{
				if (num != 1846982131U)
				{
					if (num == 2128769280U)
					{
						if (_tutorial == "Inventory Fill")
						{
							setting = DataDirector.Setting.TutorialInventoryFill;
						}
					}
				}
				else if (_tutorial == "Jumping")
				{
					setting = DataDirector.Setting.TutorialJumping;
				}
			}
			else if (num != 2213922758U)
			{
				if (num != 3192934482U)
				{
					if (num == 3538838859U)
					{
						if (_tutorial == "Sprinting")
						{
							setting = DataDirector.Setting.TutorialSprinting;
						}
					}
				}
				else if (_tutorial == "Cart Handling 2")
				{
					setting = DataDirector.Setting.TutorialCartHandling;
				}
			}
			else if (_tutorial == "Hiding")
			{
				setting = DataDirector.Setting.TutorialHiding;
			}
		}
		else if (num <= 3907977003U)
		{
			if (num != 3684092241U)
			{
				if (num == 3907977003U)
				{
					if (_tutorial == "Reviving")
					{
						setting = DataDirector.Setting.TutorialReviving;
					}
				}
			}
			else if (_tutorial == "Sneaking")
			{
				setting = DataDirector.Setting.TutorialSneaking;
			}
		}
		else if (num != 4004552029U)
		{
			if (num != 4071788383U)
			{
				if (num == 4290211947U)
				{
					if (_tutorial == "Pushing and Pulling")
					{
						setting = DataDirector.Setting.TutorialPushingAndPulling;
					}
				}
			}
			else if (_tutorial == "Multiple Extractions")
			{
				setting = DataDirector.Setting.TutorialMultipleExtractions;
			}
		}
		else if (_tutorial == "Healing")
		{
			setting = DataDirector.Setting.TutorialHealing;
		}
		int num2 = DataDirector.instance.SettingValueFetch(setting);
		DataDirector.instance.SettingValueSet(setting, num2 + 1);
		DataDirector.instance.SaveSettings();
	}

	// Token: 0x0600146D RID: 5229 RVA: 0x000B495D File Offset: 0x000B2B5D
	public void Reset()
	{
		this.currentPage = -1;
	}

	// Token: 0x0600146E RID: 5230 RVA: 0x000B4968 File Offset: 0x000B2B68
	public void UpdateRoundEnd()
	{
		if (!this.playerUsedCart)
		{
			this.numberOfRoundsWithoutCart++;
		}
		if (!this.playerUsedMap)
		{
			this.numberOfRoundsWithoutMap++;
		}
		if (!this.playerHadItemsAndUsedInventory)
		{
			this.numberOfRoundsWithoutInventory++;
		}
		if (!this.playerUsedToggle)
		{
			this.numberOfRoundsWithoutToggle++;
		}
		if (!this.playerUsedChargingStation)
		{
			this.numberOfRoundsWithoutCharging++;
		}
		if (!this.playerChatted)
		{
			this.numberOfRoundsWithoutChatting++;
		}
		if (!this.playerReviveTipDone)
		{
			this.playerSawHead = false;
			this.playerRevived = false;
		}
	}

	// Token: 0x0600146F RID: 5231 RVA: 0x000B4A0F File Offset: 0x000B2C0F
	public void TutorialActive()
	{
		this.tutorialActiveTimer = 0.2f;
	}

	// Token: 0x06001470 RID: 5232 RVA: 0x000B4A1C File Offset: 0x000B2C1C
	private void TaskMove()
	{
		Vector3 velocity = PlayerController.instance.rb.velocity;
		velocity.y = 0f;
		if (velocity.magnitude > 0.05f)
		{
			this.TutorialProgressFill(0.2f);
		}
	}

	// Token: 0x06001471 RID: 5233 RVA: 0x000B4A5E File Offset: 0x000B2C5E
	private void TaskJump()
	{
		if (!PlayerController.instance)
		{
			return;
		}
		if (PlayerController.instance.rb.velocity.y > 2f)
		{
			this.TutorialProgressFill(0.8f);
		}
	}

	// Token: 0x06001472 RID: 5234 RVA: 0x000B4A94 File Offset: 0x000B2C94
	private void TaskSneak()
	{
		Vector3 velocity = PlayerController.instance.rb.velocity;
		bool crouching = PlayerController.instance.Crouching;
		velocity.y = 0f;
		if (velocity.magnitude > 0.05f && crouching)
		{
			this.TutorialProgressFill(0.2f);
		}
	}

	// Token: 0x06001473 RID: 5235 RVA: 0x000B4AE5 File Offset: 0x000B2CE5
	private void TaskSneakUnder()
	{
		if (PlayerController.instance.Crawling)
		{
			this.TutorialProgressFill(0.2f);
		}
	}

	// Token: 0x06001474 RID: 5236 RVA: 0x000B4B00 File Offset: 0x000B2D00
	private void TaskSprint()
	{
		if (this.arrowDelay <= 0f)
		{
			SemiFunc.UIShowArrow(new Vector3(340f, 90f, 0f), new Vector3(70f, 320f, 0f), 145f);
		}
		bool sprinting = PlayerController.instance.sprinting;
		Vector3 velocity = PlayerController.instance.rb.velocity;
		velocity.y = 0f;
		if (velocity.magnitude > 2f && sprinting)
		{
			this.TutorialProgressFill(0.3f);
		}
	}

	// Token: 0x06001475 RID: 5237 RVA: 0x000B4B90 File Offset: 0x000B2D90
	private void TaskTumble()
	{
		if (!PlayerAvatar.instance.tumble)
		{
			return;
		}
		Vector3 velocity = PlayerAvatar.instance.tumble.rb.velocity;
		bool isTumbling = PlayerAvatar.instance.isTumbling;
		if (velocity.magnitude > 1f && isTumbling)
		{
			this.TutorialProgressFill(0.3f);
		}
		if (isTumbling)
		{
			this.TutorialProgressFill(0.025f);
		}
	}

	// Token: 0x06001476 RID: 5238 RVA: 0x000B4BFA File Offset: 0x000B2DFA
	private void TaskGrab()
	{
		if (PhysGrabber.instance.grabbed)
		{
			this.TutorialProgressFill(0.2f);
		}
	}

	// Token: 0x06001477 RID: 5239 RVA: 0x000B4C13 File Offset: 0x000B2E13
	private void TaskPushAndPull()
	{
		if (PhysGrabber.instance.isPushing || PhysGrabber.instance.isPulling)
		{
			this.TutorialProgressFill(0.6f);
		}
	}

	// Token: 0x06001478 RID: 5240 RVA: 0x000B4C38 File Offset: 0x000B2E38
	private void TaskRotate()
	{
		if (PhysGrabber.instance.isRotating)
		{
			this.TutorialProgressFill(0.2f);
		}
	}

	// Token: 0x06001479 RID: 5241 RVA: 0x000B4C54 File Offset: 0x000B2E54
	private void TaskInteract()
	{
		Transform grabbedObjectTransform = PhysGrabber.instance.grabbedObjectTransform;
		ItemToggle itemToggle = null;
		if (grabbedObjectTransform)
		{
			itemToggle = grabbedObjectTransform.GetComponent<ItemToggle>();
		}
		if (itemToggle && itemToggle.toggleImpulse)
		{
			this.TutorialProgressFill(0.8f);
		}
	}

	// Token: 0x0600147A RID: 5242 RVA: 0x000B4C98 File Offset: 0x000B2E98
	private void TaskInventoryFill()
	{
		if (this.arrowDelay <= 0f)
		{
			SemiFunc.UIShowArrow(new Vector3(340f, 340f, 0f), new Vector3(370f, 20f, 0f), 200f);
		}
		int num = 3;
		int num2 = Inventory.instance.InventorySpotsOccupied();
		this.tutorialProgress = (float)num2 / (float)num;
	}

	// Token: 0x0600147B RID: 5243 RVA: 0x000B4CFC File Offset: 0x000B2EFC
	private void TaskInventoryEmpty()
	{
		if (this.arrowDelay <= 0f)
		{
			SemiFunc.UIShowArrow(new Vector3(340f, 340f, 0f), new Vector3(370f, 20f, 0f), 200f);
		}
		int num = 3;
		int num2 = Inventory.instance.InventorySpotsOccupied();
		this.tutorialProgress = 1f - (float)num2 / (float)num;
	}

	// Token: 0x0600147C RID: 5244 RVA: 0x000B4D66 File Offset: 0x000B2F66
	private void TaskMap()
	{
		if (Map.Instance.Active)
		{
			this.TutorialProgressFill(0.2f);
		}
	}

	// Token: 0x0600147D RID: 5245 RVA: 0x000B4D80 File Offset: 0x000B2F80
	private void TaskCartMove()
	{
		Transform grabbedObjectTransform = PhysGrabber.instance.grabbedObjectTransform;
		PhysGrabCart physGrabCart = null;
		if (grabbedObjectTransform)
		{
			physGrabCart = grabbedObjectTransform.GetComponent<PhysGrabCart>();
		}
		if (!physGrabCart)
		{
			return;
		}
		this.tutorialCart = physGrabCart;
		Vector3 vector = Vector3.zero;
		if (physGrabCart)
		{
			vector = physGrabCart.rb.velocity;
		}
		vector.y = 0f;
		if (physGrabCart && vector.magnitude > 0.5f && physGrabCart.cartBeingPulled)
		{
			this.TutorialProgressFill(0.2f);
		}
	}

	// Token: 0x0600147E RID: 5246 RVA: 0x000B4E08 File Offset: 0x000B3008
	private void TaskCartFill()
	{
		if (this.tutorialCart)
		{
			int num = 3;
			int itemsInCartCount = this.tutorialCart.itemsInCartCount;
			this.tutorialProgress = (float)itemsInCartCount / (float)num;
			return;
		}
		Transform grabbedObjectTransform = PhysGrabber.instance.grabbedObjectTransform;
		PhysGrabCart exists = null;
		if (grabbedObjectTransform)
		{
			exists = grabbedObjectTransform.GetComponent<PhysGrabCart>();
		}
		if (!exists)
		{
			return;
		}
		this.tutorialCart = exists;
	}

	// Token: 0x0600147F RID: 5247 RVA: 0x000B4E68 File Offset: 0x000B3068
	private void TaskExtractionPoint()
	{
		GoalUI.instance.Show();
		if (this.arrowDelay <= 0f)
		{
			SemiFunc.UIShowArrow(new Vector3(340f, 90f, 0f), new Vector3(610f, 330f, 0f), 45f);
		}
		float currentHaul = (float)RoundDirector.instance.currentHaul;
		int haulGoal = RoundDirector.instance.haulGoal;
		float value = currentHaul / (float)haulGoal;
		value = Mathf.Clamp(value, 0f, 0.95f);
		if (RoundDirector.instance.extractionPointCurrent)
		{
			this.tutorialExtractionPoint = RoundDirector.instance.extractionPointCurrent;
		}
		if (this.tutorialExtractionPoint)
		{
			if (this.tutorialExtractionPoint.currentState != ExtractionPoint.State.Extracting && this.tutorialExtractionPoint.currentState != ExtractionPoint.State.Complete)
			{
				this.tutorialProgress = value;
			}
			if (this.tutorialExtractionPoint.currentState == ExtractionPoint.State.Complete && this.tutorialProgress < 0.95f)
			{
				this.tutorialProgress = 0.95f;
			}
			if (this.tutorialExtractionPoint.currentState == ExtractionPoint.State.Complete)
			{
				this.TutorialProgressFill(0.2f);
			}
		}
	}

	// Token: 0x06001480 RID: 5248 RVA: 0x000B4F79 File Offset: 0x000B3179
	private void TaskEnterTuck()
	{
	}

	// Token: 0x040022E4 RID: 8932
	public static TutorialDirector instance;

	// Token: 0x040022E5 RID: 8933
	public List<TutorialDirector.TutorialPage> tutorialPages = new List<TutorialDirector.TutorialPage>();

	// Token: 0x040022E6 RID: 8934
	internal int currentPage = -1;

	// Token: 0x040022E7 RID: 8935
	internal bool tutorialActive;

	// Token: 0x040022E8 RID: 8936
	private float tutorialCheckActiveTimer;

	// Token: 0x040022E9 RID: 8937
	private float tutorialActiveTimer;

	// Token: 0x040022EA RID: 8938
	internal float tutorialProgress;

	// Token: 0x040022EB RID: 8939
	private PhysGrabCart tutorialCart;

	// Token: 0x040022EC RID: 8940
	private ExtractionPoint tutorialExtractionPoint;

	// Token: 0x040022ED RID: 8941
	internal bool deadPlayer;

	// Token: 0x040022EE RID: 8942
	private float arrowDelay;

	// Token: 0x040022EF RID: 8943
	internal bool playerSprinted;

	// Token: 0x040022F0 RID: 8944
	internal bool playerJumped;

	// Token: 0x040022F1 RID: 8945
	internal bool playerSawHead;

	// Token: 0x040022F2 RID: 8946
	internal bool playerRevived;

	// Token: 0x040022F3 RID: 8947
	internal bool playerHealed;

	// Token: 0x040022F4 RID: 8948
	internal bool playerRotated;

	// Token: 0x040022F5 RID: 8949
	internal bool playerTumbled;

	// Token: 0x040022F6 RID: 8950
	internal bool playerCrouched;

	// Token: 0x040022F7 RID: 8951
	internal bool playerCrawled;

	// Token: 0x040022F8 RID: 8952
	internal bool playerUsedCart;

	// Token: 0x040022F9 RID: 8953
	internal bool playerPushedAndPulled;

	// Token: 0x040022FA RID: 8954
	internal bool playerUsedToggle;

	// Token: 0x040022FB RID: 8955
	internal bool playerHadItemsAndUsedInventory;

	// Token: 0x040022FC RID: 8956
	internal bool playerUsedMap;

	// Token: 0x040022FD RID: 8957
	internal bool playerUsedChargingStation;

	// Token: 0x040022FE RID: 8958
	internal bool playerReviveTipDone;

	// Token: 0x040022FF RID: 8959
	internal bool playerChatted;

	// Token: 0x04002300 RID: 8960
	internal bool playerUsedExpression;

	// Token: 0x04002301 RID: 8961
	internal bool playerGotOvercharged;

	// Token: 0x04002302 RID: 8962
	internal int numberOfRoundsWithoutChatting;

	// Token: 0x04002303 RID: 8963
	internal int numberOfRoundsWithoutCharging;

	// Token: 0x04002304 RID: 8964
	internal int numberOfRoundsWithoutMap;

	// Token: 0x04002305 RID: 8965
	internal int numberOfRoundsWithoutInventory;

	// Token: 0x04002306 RID: 8966
	internal int numberOfRoundsWithoutCart;

	// Token: 0x04002307 RID: 8967
	internal int numberOfRoundsWithoutToggle;

	// Token: 0x04002308 RID: 8968
	internal List<string> potentialTips = new List<string>();

	// Token: 0x04002309 RID: 8969
	internal List<string> shownTips = new List<string>();

	// Token: 0x0400230A RID: 8970
	private float showTipTimer;

	// Token: 0x0400230B RID: 8971
	private float delayBeforeTip;

	// Token: 0x0400230C RID: 8972
	private string scheduleTipName;

	// Token: 0x0400230D RID: 8973
	private float scheduleTipTimer;

	// Token: 0x0200040B RID: 1035
	[Serializable]
	public class TutorialPage
	{
		// Token: 0x04002D8C RID: 11660
		public string pageName = "";

		// Token: 0x04002D8D RID: 11661
		[Space(10f)]
		public VideoClip video;

		// Token: 0x04002D8E RID: 11662
		public string text;

		// Token: 0x04002D8F RID: 11663
		public string focusText;

		// Token: 0x04002D90 RID: 11664
		[TextArea(3, 10)]
		public string dummyText;
	}
}
