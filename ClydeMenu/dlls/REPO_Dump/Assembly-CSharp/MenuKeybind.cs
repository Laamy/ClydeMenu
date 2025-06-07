using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

// Token: 0x020001FC RID: 508
public class MenuKeybind : MonoBehaviour
{
	// Token: 0x06001123 RID: 4387 RVA: 0x0009CEA7 File Offset: 0x0009B0A7
	private void Start()
	{
		this.menuBigButton = base.GetComponent<MenuBigButton>();
		this.parentPage = base.GetComponentInParent<MenuPage>();
		this.settingElement = base.GetComponent<MenuSettingElement>();
		base.StartCoroutine(this.LateStart());
	}

	// Token: 0x06001124 RID: 4388 RVA: 0x0009CEDA File Offset: 0x0009B0DA
	private IEnumerator LateStart()
	{
		yield return null;
		this.UpdateBindingDisplay();
		yield break;
	}

	// Token: 0x06001125 RID: 4389 RVA: 0x0009CEEC File Offset: 0x0009B0EC
	private void UpdateBindingDisplay()
	{
		string text = InputManager.instance.InputDisplayGet(this.inputKey, this.keyType, this.movementDirection);
		this.menuBigButton.buttonName = text;
		this.menuBigButton.menuButton.buttonText.text = text;
		if (MenuPageLobby.instance)
		{
			MenuPageLobby.instance.UpdateChatPrompt();
		}
	}

	// Token: 0x06001126 RID: 4390 RVA: 0x0009CF50 File Offset: 0x0009B150
	public void OnClick()
	{
		if (this.parentPage.SettingElementActiveCheckFree(this.settingElement.settingElementID))
		{
			this.menuBigButton.state = MenuBigButton.State.Edit;
			this.parentPage.SettingElementActiveSet(this.settingElement.settingElementID);
			this.StartRebinding();
		}
	}

	// Token: 0x06001127 RID: 4391 RVA: 0x0009CFA0 File Offset: 0x0009B1A0
	private void StartRebinding()
	{
		if (this.keyType == MenuKeybind.KeyType.InputKey)
		{
			InputAction action = InputManager.instance.GetAction(this.inputKey);
			if (action != null)
			{
				int bindingIndex = 0;
				action.Disable();
				this.rebindingOperation = action.PerformInteractiveRebinding(bindingIndex).WithExpectedControlType("Axis").OnComplete(delegate(InputActionRebindingExtensions.RebindingOperation operation)
				{
					action.Enable();
					this.rebindingOperation.Dispose();
					this.menuBigButton.state = MenuBigButton.State.Main;
					this.UpdateBindingDisplay();
					MenuManager.instance.MenuEffectClick(MenuManager.MenuClickEffectType.Confirm, this.parentPage, 0.2f, -1f, false);
				}).Start();
				return;
			}
		}
		else if (this.keyType == MenuKeybind.KeyType.MovementKey)
		{
			InputAction action = InputManager.instance.GetMovementAction();
			int movementBindingIndex = InputManager.instance.GetMovementBindingIndex(this.movementDirection);
			if (action != null && movementBindingIndex >= 0)
			{
				action.Disable();
				this.rebindingOperation = action.PerformInteractiveRebinding(movementBindingIndex).OnComplete(delegate(InputActionRebindingExtensions.RebindingOperation operation)
				{
					action.Enable();
					this.rebindingOperation.Dispose();
					this.menuBigButton.state = MenuBigButton.State.Main;
					this.UpdateBindingDisplay();
					MenuManager.instance.MenuEffectClick(MenuManager.MenuClickEffectType.Confirm, this.parentPage, 0.2f, -1f, false);
				}).Start();
			}
		}
	}

	// Token: 0x04001D05 RID: 7429
	public MenuKeybind.KeyType keyType;

	// Token: 0x04001D06 RID: 7430
	public InputKey inputKey;

	// Token: 0x04001D07 RID: 7431
	public MovementDirection movementDirection;

	// Token: 0x04001D08 RID: 7432
	private MenuBigButton menuBigButton;

	// Token: 0x04001D09 RID: 7433
	private MenuPage parentPage;

	// Token: 0x04001D0A RID: 7434
	private MenuSettingElement settingElement;

	// Token: 0x04001D0B RID: 7435
	private float actionValue;

	// Token: 0x04001D0C RID: 7436
	private InputActionRebindingExtensions.RebindingOperation rebindingOperation;

	// Token: 0x020003D6 RID: 982
	public enum KeyType
	{
		// Token: 0x04002C90 RID: 11408
		InputKey,
		// Token: 0x04002C91 RID: 11409
		MovementKey
	}
}
