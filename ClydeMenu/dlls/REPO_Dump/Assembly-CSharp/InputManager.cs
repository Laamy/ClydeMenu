using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

// Token: 0x020001F0 RID: 496
public class InputManager : MonoBehaviour
{
	// Token: 0x060010C7 RID: 4295 RVA: 0x0009A896 File Offset: 0x00098A96
	private void Awake()
	{
		if (InputManager.instance == null)
		{
			InputManager.instance = this;
			Object.DontDestroyOnLoad(base.gameObject);
			this.InitializeInputs();
			this.StoreDefaultBindings();
			return;
		}
		Object.Destroy(base.gameObject);
	}

	// Token: 0x060010C8 RID: 4296 RVA: 0x0009A8D0 File Offset: 0x00098AD0
	private void Start()
	{
		InputSystem.settings.backgroundBehavior = InputSettings.BackgroundBehavior.ResetAndDisableAllDevices;
		this.tagDictionary.Add("[move]", InputKey.Movement);
		this.tagDictionary.Add("[jump]", InputKey.Jump);
		this.tagDictionary.Add("[grab]", InputKey.Grab);
		this.tagDictionary.Add("[grab2]", InputKey.Rotate);
		this.tagDictionary.Add("[sprint]", InputKey.Sprint);
		this.tagDictionary.Add("[crouch]", InputKey.Crouch);
		this.tagDictionary.Add("[map]", InputKey.Map);
		this.tagDictionary.Add("[inventory1]", InputKey.Inventory1);
		this.tagDictionary.Add("[inventory2]", InputKey.Inventory2);
		this.tagDictionary.Add("[inventory3]", InputKey.Inventory3);
		this.tagDictionary.Add("[tumble]", InputKey.Tumble);
		this.tagDictionary.Add("[interact]", InputKey.Interact);
		this.tagDictionary.Add("[push]", InputKey.Push);
		this.tagDictionary.Add("[pull]", InputKey.Pull);
		this.tagDictionary.Add("[chat]", InputKey.Chat);
		this.tagDictionary.Add("[expression1]", InputKey.Expression1);
		this.tagDictionary.Add("[expression2]", InputKey.Expression2);
		this.tagDictionary.Add("[expression3]", InputKey.Expression3);
		this.tagDictionary.Add("[expression4]", InputKey.Expression4);
		this.tagDictionary.Add("[expression5]", InputKey.Expression5);
		this.tagDictionary.Add("[expression6]", InputKey.Expression6);
		ES3.DeleteFile("DefaultKeyBindings.es3");
		if (!ES3.FileExists(new ES3Settings("DefaultKeyBindings.es3", new Enum[]
		{
			ES3.Location.File
		})))
		{
			this.SaveDefaultKeyBindings();
		}
		if (!ES3.FileExists(new ES3Settings("CurrentKeyBindings.es3", new Enum[]
		{
			ES3.Location.File
		})))
		{
			this.SaveCurrentKeyBindings();
		}
		this.LoadKeyBindings("CurrentKeyBindings.es3");
	}

	// Token: 0x060010C9 RID: 4297 RVA: 0x0009AABC File Offset: 0x00098CBC
	private void FixedUpdate()
	{
		float num = Mathf.Min(Time.fixedDeltaTime, 0.05f);
		if (this.disableMovementTimer > 0f)
		{
			this.disableMovementTimer -= num;
		}
		if (this.disableAimingTimer > 0f)
		{
			this.disableAimingTimer -= num;
		}
	}

	// Token: 0x060010CA RID: 4298 RVA: 0x0009AB10 File Offset: 0x00098D10
	private void InitializeInputs()
	{
		this.inputActions = new Dictionary<InputKey, InputAction>();
		this.movementBindingIndices = new Dictionary<MovementDirection, int>();
		this.inputToggle = new Dictionary<InputKey, bool>();
		InputAction inputAction = new InputAction("Movement", InputActionType.Value, null, null, null, null);
		InputActionSetupExtensions.CompositeSyntax compositeSyntax = inputAction.AddCompositeBinding("2DVector", null, null);
		compositeSyntax.With("Up", "<Keyboard>/w", null, null);
		compositeSyntax.With("Down", "<Keyboard>/s", null, null);
		compositeSyntax.With("Left", "<Keyboard>/a", null, null);
		compositeSyntax.With("Right", "<Keyboard>/d", null, null);
		this.inputActions[InputKey.Movement] = inputAction;
		ReadOnlyArray<InputBinding> bindings = inputAction.bindings;
		for (int i = 0; i < bindings.Count; i++)
		{
			InputBinding inputBinding = bindings[i];
			if (inputBinding.isPartOfComposite)
			{
				string text = inputBinding.name.ToLower();
				if (!(text == "up"))
				{
					if (!(text == "down"))
					{
						if (!(text == "left"))
						{
							if (text == "right")
							{
								this.movementBindingIndices[MovementDirection.Right] = i;
							}
						}
						else
						{
							this.movementBindingIndices[MovementDirection.Left] = i;
						}
					}
					else
					{
						this.movementBindingIndices[MovementDirection.Down] = i;
					}
				}
				else
				{
					this.movementBindingIndices[MovementDirection.Up] = i;
				}
			}
		}
		InputAction inputAction2 = new InputAction("Scroll", InputActionType.Value, null, null, null, null);
		inputAction2.AddBinding("<Mouse>/scroll/y", null, null, null);
		this.inputActions[InputKey.Scroll] = inputAction2;
		InputAction inputAction3 = new InputAction("Jump", InputActionType.Value, "<Keyboard>/space", null, null, null);
		this.inputActions[InputKey.Jump] = inputAction3;
		inputAction3 = new InputAction("Use", InputActionType.Value, "<Keyboard>/e", null, null, null);
		this.inputActions[InputKey.Interact] = inputAction3;
		inputAction3 = new InputAction("MouseInput", InputActionType.Value, "<Pointer>/position", null, null, null);
		this.inputActions[InputKey.MouseInput] = inputAction3;
		InputAction inputAction4 = new InputAction("Push", InputActionType.Value, null, null, null, null);
		inputAction4.AddBinding("<Mouse>/scroll/y", null, null, null);
		this.inputActions[InputKey.Push] = inputAction4;
		InputAction inputAction5 = new InputAction("Pull", InputActionType.Value, null, null, null, null);
		inputAction5.AddBinding("<Mouse>/scroll/y", null, null, null);
		this.inputActions[InputKey.Pull] = inputAction5;
		inputAction3 = new InputAction("Menu", InputActionType.Value, "<Keyboard>/escape", null, null, null);
		this.inputActions[InputKey.Menu] = inputAction3;
		inputAction3 = new InputAction("Back", InputActionType.Value, "<Keyboard>/escape", null, null, null);
		this.inputActions[InputKey.Back] = inputAction3;
		inputAction3 = new InputAction("BackEditor", InputActionType.Value, "<Keyboard>/F1", null, null, null);
		this.inputActions[InputKey.BackEditor] = inputAction3;
		inputAction3 = new InputAction("Chat", InputActionType.Value, "<Keyboard>/t", null, null, null);
		this.inputActions[InputKey.Chat] = inputAction3;
		inputAction3 = new InputAction("Map", InputActionType.Value, "<Keyboard>/tab", null, null, null);
		this.inputActions[InputKey.Map] = inputAction3;
		this.inputToggle.Add(InputKey.Map, false);
		inputAction3 = new InputAction("Confirm", InputActionType.Value, "<Keyboard>/enter", null, null, null);
		this.inputActions[InputKey.Confirm] = inputAction3;
		inputAction3 = new InputAction("Grab", InputActionType.Value, "<Mouse>/leftButton", null, null, null);
		this.inputActions[InputKey.Grab] = inputAction3;
		this.inputToggle.Add(InputKey.Grab, false);
		inputAction3 = new InputAction("Rotate", InputActionType.Value, "<Mouse>/rightButton", null, null, null);
		this.inputActions[InputKey.Rotate] = inputAction3;
		inputAction3 = new InputAction("Crouch", InputActionType.Value, "<Keyboard>/ctrl", null, null, null);
		this.inputActions[InputKey.Crouch] = inputAction3;
		this.inputToggle.Add(InputKey.Crouch, false);
		inputAction3 = new InputAction("Chat Delete", InputActionType.Value, "<Keyboard>/backspace", null, null, null);
		this.inputActions[InputKey.ChatDelete] = inputAction3;
		inputAction3 = new InputAction("Tumble", InputActionType.Value, "<Keyboard>/q", null, null, null);
		this.inputActions[InputKey.Tumble] = inputAction3;
		inputAction3 = new InputAction("Sprint", InputActionType.Value, "<Keyboard>/leftShift", null, null, null);
		this.inputActions[InputKey.Sprint] = inputAction3;
		this.inputToggle.Add(InputKey.Sprint, false);
		inputAction3 = new InputAction("MouseDelta", InputActionType.Value, "<Pointer>/delta", null, null, null);
		this.inputActions[InputKey.MouseDelta] = inputAction3;
		inputAction3 = new InputAction("Inventory1", InputActionType.Value, "<Keyboard>/1", null, null, null);
		this.inputActions[InputKey.Inventory1] = inputAction3;
		inputAction3 = new InputAction("Inventory2", InputActionType.Value, "<Keyboard>/2", null, null, null);
		this.inputActions[InputKey.Inventory2] = inputAction3;
		inputAction3 = new InputAction("Inventory3", InputActionType.Value, "<Keyboard>/3", null, null, null);
		this.inputActions[InputKey.Inventory3] = inputAction3;
		inputAction3 = new InputAction("SpectateNext", InputActionType.Value, "<Mouse>/rightButton", null, null, null);
		this.inputActions[InputKey.SpectateNext] = inputAction3;
		inputAction3 = new InputAction("SpectatePrevious", InputActionType.Value, "<Mouse>/leftButton", null, null, null);
		this.inputActions[InputKey.SpectatePrevious] = inputAction3;
		inputAction3 = new InputAction("PushToTalk", InputActionType.Value, "<Keyboard>/v", null, null, null);
		this.inputActions[InputKey.PushToTalk] = inputAction3;
		inputAction3 = new InputAction("ToggleMute", InputActionType.Value, "<Keyboard>/b", null, null, null);
		this.inputActions[InputKey.ToggleMute] = inputAction3;
		this.inputPercentSettings = new Dictionary<InputPercentSetting, int>();
		this.inputPercentSettings[InputPercentSetting.MouseSensitivity] = 50;
		inputAction3 = new InputAction("Expression1", InputActionType.Value, "<Keyboard>/5", null, null, null);
		this.inputActions[InputKey.Expression1] = inputAction3;
		this.inputToggle.Add(InputKey.Expression1, true);
		inputAction3 = new InputAction("Expression2", InputActionType.Value, "<Keyboard>/6", null, null, null);
		this.inputActions[InputKey.Expression2] = inputAction3;
		inputAction3 = new InputAction("Expression3", InputActionType.Value, "<Keyboard>/7", null, null, null);
		this.inputActions[InputKey.Expression3] = inputAction3;
		inputAction3 = new InputAction("Expression4", InputActionType.Value, "<Keyboard>/8", null, null, null);
		this.inputActions[InputKey.Expression4] = inputAction3;
		inputAction3 = new InputAction("Expression5", InputActionType.Value, "<Keyboard>/9", null, null, null);
		this.inputActions[InputKey.Expression5] = inputAction3;
		inputAction3 = new InputAction("Expression6", InputActionType.Value, "<Keyboard>/0", null, null, null);
		this.inputActions[InputKey.Expression6] = inputAction3;
		foreach (InputAction inputAction6 in this.inputActions.Values)
		{
			inputAction6.Enable();
		}
	}

	// Token: 0x060010CB RID: 4299 RVA: 0x0009B1BC File Offset: 0x000993BC
	private void StoreDefaultBindings()
	{
		this.defaultBindingPaths = new Dictionary<InputKey, List<string>>();
		this.defaultInputToggleStates = new Dictionary<InputKey, bool>();
		this.defaultInputPercentSettings = new Dictionary<InputPercentSetting, int>();
		foreach (InputKey inputKey in this.inputActions.Keys)
		{
			InputAction inputAction = this.inputActions[inputKey];
			List<string> list = new List<string>();
			foreach (InputBinding inputBinding in inputAction.bindings)
			{
				list.Add(inputBinding.path);
			}
			this.defaultBindingPaths[inputKey] = list;
		}
		foreach (KeyValuePair<InputKey, bool> keyValuePair in this.inputToggle)
		{
			this.defaultInputToggleStates[keyValuePair.Key] = keyValuePair.Value;
		}
		foreach (KeyValuePair<InputPercentSetting, int> keyValuePair2 in this.inputPercentSettings)
		{
			this.defaultInputPercentSettings[keyValuePair2.Key] = keyValuePair2.Value;
		}
	}

	// Token: 0x060010CC RID: 4300 RVA: 0x0009B348 File Offset: 0x00099548
	public void SaveDefaultKeyBindings()
	{
		KeyBindingSaveData keyBindingSaveData = new KeyBindingSaveData();
		keyBindingSaveData.bindingOverrides = this.defaultBindingPaths;
		keyBindingSaveData.inputToggleStates = this.defaultInputToggleStates;
		keyBindingSaveData.inputPercentSettings = this.defaultInputPercentSettings;
		ES3Settings es3Settings = new ES3Settings(new Enum[]
		{
			ES3.Location.Cache
		});
		es3Settings.path = "DefaultKeyBindings.es3";
		ES3.Save<KeyBindingSaveData>("KeyBindings", keyBindingSaveData, es3Settings);
		ES3.StoreCachedFile(es3Settings);
	}

	// Token: 0x060010CD RID: 4301 RVA: 0x0009B3B4 File Offset: 0x000995B4
	public void SaveCurrentKeyBindings()
	{
		KeyBindingSaveData keyBindingSaveData = new KeyBindingSaveData();
		keyBindingSaveData.bindingOverrides = new Dictionary<InputKey, List<string>>();
		keyBindingSaveData.inputToggleStates = new Dictionary<InputKey, bool>(this.inputToggle);
		keyBindingSaveData.inputPercentSettings = new Dictionary<InputPercentSetting, int>(this.inputPercentSettings);
		foreach (InputKey inputKey in this.inputActions.Keys)
		{
			InputAction inputAction = this.inputActions[inputKey];
			List<string> list = new List<string>();
			foreach (InputBinding inputBinding in inputAction.bindings)
			{
				list.Add(string.IsNullOrEmpty(inputBinding.overridePath) ? inputBinding.path : inputBinding.overridePath);
			}
			keyBindingSaveData.bindingOverrides[inputKey] = list;
		}
		ES3Settings es3Settings = new ES3Settings(new Enum[]
		{
			ES3.Location.Cache
		});
		es3Settings.path = "CurrentKeyBindings.es3";
		ES3.Save<KeyBindingSaveData>("KeyBindings", keyBindingSaveData, es3Settings);
		ES3.StoreCachedFile(es3Settings);
	}

	// Token: 0x060010CE RID: 4302 RVA: 0x0009B4F8 File Offset: 0x000996F8
	public void LoadKeyBindings(string filename)
	{
		try
		{
			ES3Settings settings = new ES3Settings(filename, new Enum[]
			{
				ES3.Location.File
			});
			if (ES3.FileExists(settings))
			{
				KeyBindingSaveData saveData = ES3.Load<KeyBindingSaveData>("KeyBindings", settings);
				this.ApplyLoadedKeyBindings(saveData);
			}
			else
			{
				Debug.LogWarning("Keybindings file not found: " + filename);
			}
		}
		catch (Exception ex)
		{
			Debug.LogError("Failed to load keybindings: " + ex.Message);
		}
	}

	// Token: 0x060010CF RID: 4303 RVA: 0x0009B574 File Offset: 0x00099774
	private void ApplyLoadedKeyBindings(KeyBindingSaveData saveData)
	{
		foreach (InputKey inputKey in saveData.bindingOverrides.Keys)
		{
			InputAction inputAction;
			if (this.inputActions.TryGetValue(inputKey, ref inputAction))
			{
				List<string> list = saveData.bindingOverrides[inputKey];
				inputAction.Disable();
				for (int i = 0; i < list.Count; i++)
				{
					string text = list[i];
					if (!string.IsNullOrEmpty(text) && inputAction.bindings.Count > i)
					{
						inputAction.ApplyBindingOverride(i, text);
					}
				}
				inputAction.Enable();
			}
		}
		if (saveData.inputToggleStates != null)
		{
			foreach (KeyValuePair<InputKey, bool> keyValuePair in saveData.inputToggleStates)
			{
				this.inputToggle[keyValuePair.Key] = keyValuePair.Value;
			}
		}
		if (saveData.inputPercentSettings != null)
		{
			foreach (KeyValuePair<InputPercentSetting, int> keyValuePair2 in saveData.inputPercentSettings)
			{
				this.inputPercentSettings[keyValuePair2.Key] = keyValuePair2.Value;
			}
		}
	}

	// Token: 0x060010D0 RID: 4304 RVA: 0x0009B6F0 File Offset: 0x000998F0
	public void ResetKeyToDefault(InputKey key)
	{
		InputAction inputAction;
		if (this.inputActions.TryGetValue(key, ref inputAction))
		{
			inputAction.Disable();
			List<string> list = this.defaultBindingPaths[key];
			for (int i = 0; i < list.Count; i++)
			{
				inputAction.ApplyBindingOverride(i, list[i]);
			}
			inputAction.Enable();
			if (this.defaultInputToggleStates.ContainsKey(key))
			{
				this.inputToggle[key] = this.defaultInputToggleStates[key];
			}
			if (this.defaultInputPercentSettings.ContainsKey((InputPercentSetting)key))
			{
				this.inputPercentSettings[(InputPercentSetting)key] = this.defaultInputPercentSettings[(InputPercentSetting)key];
				return;
			}
		}
		else
		{
			Debug.LogWarning("InputKey not found: " + key.ToString());
		}
	}

	// Token: 0x060010D1 RID: 4305 RVA: 0x0009B7B4 File Offset: 0x000999B4
	public bool KeyDown(InputKey key)
	{
		if ((key == InputKey.Jump || key == InputKey.Crouch || key == InputKey.Tumble || key == InputKey.Inventory1 || key == InputKey.Inventory2 || key == InputKey.Inventory3 || key == InputKey.Interact || key == InputKey.ToggleMute || key == InputKey.Expression1 || key == InputKey.Expression2 || key == InputKey.Expression3 || key == InputKey.Expression4 || key == InputKey.Expression5 || key == InputKey.Expression6) && this.disableMovementTimer > 0f)
		{
			return false;
		}
		InputAction inputAction;
		if (!this.inputActions.TryGetValue(key, ref inputAction))
		{
			return false;
		}
		if (key == InputKey.Push)
		{
			return inputAction.ReadValue<Vector2>().y > 0f;
		}
		if (key == InputKey.Pull)
		{
			return inputAction.ReadValue<Vector2>().y < 0f;
		}
		return inputAction.WasPressedThisFrame();
	}

	// Token: 0x060010D2 RID: 4306 RVA: 0x0009B85C File Offset: 0x00099A5C
	public bool KeyUp(InputKey key)
	{
		InputAction inputAction;
		return ((key != InputKey.Jump && key != InputKey.Crouch && key != InputKey.Tumble) || this.disableMovementTimer <= 0f) && this.inputActions.TryGetValue(key, ref inputAction) && key != InputKey.Push && key != InputKey.Pull && inputAction.WasReleasedThisFrame();
	}

	// Token: 0x060010D3 RID: 4307 RVA: 0x0009B8A8 File Offset: 0x00099AA8
	public float KeyPullAndPush()
	{
		InputAction inputAction;
		if (this.inputActions.TryGetValue(InputKey.Push, ref inputAction))
		{
			if (inputAction.bindings[0].effectivePath.EndsWith("scroll/y"))
			{
				if (inputAction.ReadValue<float>() > 0f)
				{
					return inputAction.ReadValue<float>();
				}
			}
			else if (inputAction.IsPressed())
			{
				return 1f;
			}
		}
		InputAction inputAction2;
		if (this.inputActions.TryGetValue(InputKey.Pull, ref inputAction2))
		{
			if (inputAction2.bindings[0].effectivePath.EndsWith("scroll/y"))
			{
				if (inputAction2.ReadValue<float>() < 0f)
				{
					return inputAction2.ReadValue<float>();
				}
			}
			else if (inputAction2.IsPressed())
			{
				return -1f;
			}
		}
		return 0f;
	}

	// Token: 0x060010D4 RID: 4308 RVA: 0x0009B964 File Offset: 0x00099B64
	public InputAction GetAction(InputKey key)
	{
		InputAction result;
		if (this.inputActions.TryGetValue(key, ref result))
		{
			return result;
		}
		return null;
	}

	// Token: 0x060010D5 RID: 4309 RVA: 0x0009B984 File Offset: 0x00099B84
	public InputAction GetMovementAction()
	{
		InputAction result;
		if (this.inputActions.TryGetValue(InputKey.Movement, ref result))
		{
			return result;
		}
		return null;
	}

	// Token: 0x060010D6 RID: 4310 RVA: 0x0009B9A4 File Offset: 0x00099BA4
	public int GetMovementBindingIndex(MovementDirection direction)
	{
		int result;
		if (this.movementBindingIndices.TryGetValue(direction, ref result))
		{
			return result;
		}
		return -1;
	}

	// Token: 0x060010D7 RID: 4311 RVA: 0x0009B9C4 File Offset: 0x00099BC4
	public bool KeyHold(InputKey key)
	{
		if ((key == InputKey.Jump || key == InputKey.Crouch || key == InputKey.Tumble || key == InputKey.Expression1 || key == InputKey.Expression2 || key == InputKey.Expression3 || key == InputKey.Expression4 || key == InputKey.Expression5 || key == InputKey.Expression6) && this.disableMovementTimer > 0f)
		{
			return false;
		}
		InputAction inputAction;
		if (!this.inputActions.TryGetValue(key, ref inputAction))
		{
			return false;
		}
		if (key == InputKey.Push)
		{
			return inputAction.ReadValue<Vector2>().y > 0f;
		}
		if (key == InputKey.Pull)
		{
			return inputAction.ReadValue<Vector2>().y < 0f;
		}
		return inputAction.IsPressed();
	}

	// Token: 0x060010D8 RID: 4312 RVA: 0x0009BA54 File Offset: 0x00099C54
	public float GetMovementX()
	{
		if (this.disableMovementTimer > 0f)
		{
			return 0f;
		}
		InputAction inputAction;
		if (this.inputActions.TryGetValue(InputKey.Movement, ref inputAction))
		{
			return inputAction.ReadValue<Vector2>().x;
		}
		return 0f;
	}

	// Token: 0x060010D9 RID: 4313 RVA: 0x0009BA98 File Offset: 0x00099C98
	public float GetScrollY()
	{
		InputAction inputAction;
		if (this.inputActions.TryGetValue(InputKey.Scroll, ref inputAction))
		{
			return inputAction.ReadValue<float>();
		}
		return 0f;
	}

	// Token: 0x060010DA RID: 4314 RVA: 0x0009BAC4 File Offset: 0x00099CC4
	public float GetMovementY()
	{
		if (this.disableMovementTimer > 0f)
		{
			return 0f;
		}
		InputAction inputAction;
		if (this.inputActions.TryGetValue(InputKey.Movement, ref inputAction))
		{
			return inputAction.ReadValue<Vector2>().y;
		}
		return 0f;
	}

	// Token: 0x060010DB RID: 4315 RVA: 0x0009BB08 File Offset: 0x00099D08
	public Vector2 GetMovement()
	{
		if (this.disableMovementTimer > 0f)
		{
			return Vector2.zero;
		}
		InputAction inputAction;
		if (this.inputActions.TryGetValue(InputKey.Movement, ref inputAction))
		{
			return inputAction.ReadValue<Vector2>();
		}
		return Vector2.zero;
	}

	// Token: 0x060010DC RID: 4316 RVA: 0x0009BB44 File Offset: 0x00099D44
	public float GetMouseX()
	{
		if (this.disableAimingTimer > 0f)
		{
			return 0f;
		}
		InputAction inputAction;
		if (this.inputActions.TryGetValue(InputKey.MouseDelta, ref inputAction))
		{
			return inputAction.ReadValue<Vector2>().x * this.mouseSensitivity;
		}
		return 0f;
	}

	// Token: 0x060010DD RID: 4317 RVA: 0x0009BB90 File Offset: 0x00099D90
	public float GetMouseY()
	{
		if (this.disableAimingTimer > 0f)
		{
			return 0f;
		}
		InputAction inputAction;
		if (this.inputActions.TryGetValue(InputKey.MouseDelta, ref inputAction))
		{
			return inputAction.ReadValue<Vector2>().y * this.mouseSensitivity;
		}
		return 0f;
	}

	// Token: 0x060010DE RID: 4318 RVA: 0x0009BBDC File Offset: 0x00099DDC
	public Vector2 GetMousePosition()
	{
		if (this.disableAimingTimer > 0f)
		{
			return Vector2.zero;
		}
		InputAction inputAction;
		if (this.inputActions.TryGetValue(InputKey.MouseInput, ref inputAction))
		{
			return inputAction.ReadValue<Vector2>();
		}
		return Vector2.zero;
	}

	// Token: 0x060010DF RID: 4319 RVA: 0x0009BC18 File Offset: 0x00099E18
	public void Rebind(InputKey key, string newBinding)
	{
		InputAction action;
		if (this.inputActions.TryGetValue(key, ref action))
		{
			action.ApplyBindingOverride(newBinding, null, null);
		}
	}

	// Token: 0x060010E0 RID: 4320 RVA: 0x0009BC40 File Offset: 0x00099E40
	public void RebindMovementKey(MovementDirection direction, string newBinding)
	{
		InputAction action;
		if (this.inputActions.TryGetValue(InputKey.Movement, ref action))
		{
			int bindingIndex;
			if (this.movementBindingIndices.TryGetValue(direction, ref bindingIndex))
			{
				action.ApplyBindingOverride(bindingIndex, newBinding);
				return;
			}
			Debug.LogWarning(string.Format("Binding index for {0} not found.", direction));
		}
	}

	// Token: 0x060010E1 RID: 4321 RVA: 0x0009BC8B File Offset: 0x00099E8B
	public void DisableMovement()
	{
		this.disableMovementTimer = 0.1f;
	}

	// Token: 0x060010E2 RID: 4322 RVA: 0x0009BC98 File Offset: 0x00099E98
	public void DisableAiming()
	{
		this.disableAimingTimer = 0.1f;
	}

	// Token: 0x060010E3 RID: 4323 RVA: 0x0009BCA5 File Offset: 0x00099EA5
	public void InputToggleRebind(InputKey key, bool toggle)
	{
		this.inputToggle[key] = toggle;
	}

	// Token: 0x060010E4 RID: 4324 RVA: 0x0009BCB4 File Offset: 0x00099EB4
	public bool InputToggleGet(InputKey key)
	{
		return this.inputToggle[key];
	}

	// Token: 0x060010E5 RID: 4325 RVA: 0x0009BCC4 File Offset: 0x00099EC4
	public string GetKeyString(InputKey key)
	{
		InputAction action = this.GetAction(key);
		if (action == null)
		{
			return null;
		}
		return action.bindings[0].effectivePath;
	}

	// Token: 0x060010E6 RID: 4326 RVA: 0x0009BCF4 File Offset: 0x00099EF4
	public string GetMovementKeyString(MovementDirection direction)
	{
		InputAction movementAction = this.GetMovementAction();
		int movementBindingIndex = this.GetMovementBindingIndex(direction);
		if (movementAction == null)
		{
			return null;
		}
		return movementAction.bindings[movementBindingIndex].effectivePath;
	}

	// Token: 0x060010E7 RID: 4327 RVA: 0x0009BD2C File Offset: 0x00099F2C
	public string InputDisplayGet(InputKey _inputKey, MenuKeybind.KeyType _keyType, MovementDirection _movementDirection)
	{
		if (_keyType == MenuKeybind.KeyType.InputKey)
		{
			InputAction action = this.GetAction(_inputKey);
			if (action != null)
			{
				int bindingIndex = 0;
				return this.InputDisplayGetString(action, bindingIndex);
			}
		}
		else if (_keyType == MenuKeybind.KeyType.MovementKey)
		{
			InputAction movementAction = this.GetMovementAction();
			int movementBindingIndex = this.GetMovementBindingIndex(_movementDirection);
			if (movementAction != null && movementBindingIndex >= 0)
			{
				return this.InputDisplayGetString(movementAction, movementBindingIndex);
			}
		}
		return "Unassigned";
	}

	// Token: 0x060010E8 RID: 4328 RVA: 0x0009BD7C File Offset: 0x00099F7C
	public string InputDisplayGetString(InputAction action, int bindingIndex)
	{
		InputBinding inputBinding = action.bindings[bindingIndex];
		string text = InputControlPath.ToHumanReadableString(inputBinding.effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice, null);
		bool flag = false;
		if (text.EndsWith("Scroll/Y"))
		{
			text = "Mouse Scroll";
			flag = true;
		}
		if (inputBinding.effectivePath.Contains("Mouse") && !flag)
		{
			text = this.InputDisplayMouseStringGet(inputBinding.effectivePath);
		}
		return text.ToUpper();
	}

	// Token: 0x060010E9 RID: 4329 RVA: 0x0009BDEC File Offset: 0x00099FEC
	private string InputDisplayMouseStringGet(string path)
	{
		if (path.Contains("leftButton"))
		{
			return "Mouse Left";
		}
		if (path.Contains("rightButton"))
		{
			return "Mouse Right";
		}
		if (path.Contains("middleButton"))
		{
			return "Mouse Middle";
		}
		if (path.Contains("press"))
		{
			return "Mouse Press";
		}
		if (path.Contains("backButton"))
		{
			return "Mouse Back";
		}
		if (path.Contains("forwardButton"))
		{
			return "Mouse Forward";
		}
		if (path.Contains("button"))
		{
			int num = path.IndexOf("button");
			string text = path.Substring(num + "button".Length);
			return "Mouse " + text;
		}
		return "Mouse Button";
	}

	// Token: 0x060010EA RID: 4330 RVA: 0x0009BEA8 File Offset: 0x0009A0A8
	public string InputDisplayReplaceTags(string _text)
	{
		foreach (KeyValuePair<string, InputKey> keyValuePair in this.tagDictionary)
		{
			string text;
			if (keyValuePair.Value == InputKey.Movement)
			{
				text = this.InputDisplayGet(keyValuePair.Value, MenuKeybind.KeyType.MovementKey, MovementDirection.Up);
				text += this.InputDisplayGet(keyValuePair.Value, MenuKeybind.KeyType.MovementKey, MovementDirection.Left);
				text += this.InputDisplayGet(keyValuePair.Value, MenuKeybind.KeyType.MovementKey, MovementDirection.Down);
				text += this.InputDisplayGet(keyValuePair.Value, MenuKeybind.KeyType.MovementKey, MovementDirection.Right);
			}
			else
			{
				text = this.InputDisplayGet(keyValuePair.Value, MenuKeybind.KeyType.InputKey, MovementDirection.Up);
			}
			_text = _text.Replace(keyValuePair.Key, "<u><b>" + text + "</b></u>");
		}
		return _text;
	}

	// Token: 0x060010EB RID: 4331 RVA: 0x0009BF90 File Offset: 0x0009A190
	public void ResetInput()
	{
		InputSystem.ResetHaptics();
		InputSystem.ResetDevice(Keyboard.current, false);
		foreach (KeyValuePair<InputKey, InputAction> keyValuePair in this.inputActions)
		{
			keyValuePair.Value.Reset();
		}
	}

	// Token: 0x04001CBB RID: 7355
	public static InputManager instance;

	// Token: 0x04001CBC RID: 7356
	private Dictionary<InputKey, InputAction> inputActions;

	// Token: 0x04001CBD RID: 7357
	private Dictionary<InputKey, bool> inputToggle;

	// Token: 0x04001CBE RID: 7358
	internal Dictionary<InputPercentSetting, int> inputPercentSettings;

	// Token: 0x04001CBF RID: 7359
	private Dictionary<MovementDirection, int> movementBindingIndices;

	// Token: 0x04001CC0 RID: 7360
	private Dictionary<InputKey, List<string>> defaultBindingPaths;

	// Token: 0x04001CC1 RID: 7361
	private Dictionary<InputKey, bool> defaultInputToggleStates;

	// Token: 0x04001CC2 RID: 7362
	private Dictionary<InputPercentSetting, int> defaultInputPercentSettings;

	// Token: 0x04001CC3 RID: 7363
	internal float disableMovementTimer;

	// Token: 0x04001CC4 RID: 7364
	internal float disableAimingTimer;

	// Token: 0x04001CC5 RID: 7365
	internal float mouseSensitivity = 0.1f;

	// Token: 0x04001CC6 RID: 7366
	private Dictionary<string, InputKey> tagDictionary = new Dictionary<string, InputKey>();
}
