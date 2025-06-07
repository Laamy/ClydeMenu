using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020000CB RID: 203
public class ToolController : MonoBehaviour
{
	// Token: 0x06000738 RID: 1848 RVA: 0x00044EB1 File Offset: 0x000430B1
	private void Awake()
	{
		ToolController.instance = this;
	}

	// Token: 0x06000739 RID: 1849 RVA: 0x00044EBC File Offset: 0x000430BC
	private void Start()
	{
		this.MainCamera = Camera.main;
		this.Mask = LayerMask.GetMask(new string[]
		{
			"Interaction"
		});
		this.VisibilityMask = LayerMask.GetMask(new string[]
		{
			"Default"
		});
	}

	// Token: 0x0600073A RID: 1850 RVA: 0x00044F10 File Offset: 0x00043110
	private void Update()
	{
		this.UpdateInput();
		this.InteractionCheck();
		this.UpdateDirtFinder();
		this.UpdateActive();
		this.UpdateInteract();
		this.ToolFollow.position = Vector3.Lerp(this.ToolFollow.position, this.FollowTargetTransform.position, 20f * Time.deltaTime);
		this.ToolFollow.rotation = Quaternion.Lerp(this.ToolFollow.rotation, this.FollowTargetTransform.rotation, 20f * Time.deltaTime);
	}

	// Token: 0x0600073B RID: 1851 RVA: 0x00044F9D File Offset: 0x0004319D
	public void Disable(float time)
	{
		this.DisableTimer = time;
	}

	// Token: 0x0600073C RID: 1852 RVA: 0x00044FA8 File Offset: 0x000431A8
	private void UpdateInput()
	{
		if (GameDirector.instance.currentState != GameDirector.gameState.Main)
		{
			return;
		}
		if (PlayerAvatar.instance.isDisabled)
		{
			return;
		}
		if (SemiFunc.InputDown(InputKey.Interact) || this.InteractInputDelayed)
		{
			if (this.ActiveInteractionType != Interaction.InteractionType.None && this.CurrentInteractionType != Interaction.InteractionType.None && this.CurrentInteractionType != this.ActiveInteractionType)
			{
				this.InteractInputDelayed = true;
				this.InteractInput = false;
			}
			else
			{
				this.InteractInput = true;
			}
		}
		else
		{
			this.InteractInput = false;
		}
		if (PlayerController.instance.CanInteract && (Input.GetButton("Dirt Finder") || Input.GetAxis("Dirt Finder") == 1f || GameDirector.instance.LevelCompleted))
		{
			this.DirtFinderInput = true;
		}
		else
		{
			this.DirtFinderInput = false;
		}
		if (this.DisableTimer > 0f)
		{
			this.DisableTimer -= 1f * Time.deltaTime;
			this.ActiveTimer = 0f;
			this.InteractInputDelayed = false;
			this.InteractInput = false;
			this.DirtFinderInput = false;
		}
	}

	// Token: 0x0600073D RID: 1853 RVA: 0x000450A8 File Offset: 0x000432A8
	private void InteractionCheck()
	{
		if (this.InteractionCheckTimer <= 0f)
		{
			this.InteractionCheckTimer = this.InteractionCheckTime;
			this.CurrentInteractionType = Interaction.InteractionType.None;
			if (PlayerController.instance.CanInteract)
			{
				RaycastHit[] array = Physics.BoxCastAll(this.MainCamera.transform.position, new Vector3(0.01f, 0.01f, 0.01f), this.MainCamera.transform.forward, this.MainCamera.transform.rotation, this.InteractionRange, this.Mask);
				if (array.Length != 0)
				{
					RaycastHit raycastHit;
					bool flag = Physics.Raycast(this.MainCamera.transform.position, this.MainCamera.transform.forward, out raycastHit, this.InteractionRange, this.VisibilityMask);
					bool flag2 = false;
					Interaction hitPicked = null;
					float num = 360f;
					RaycastHit[] array2 = array;
					for (int i = 0; i < array2.Length; i++)
					{
						RaycastHit raycastHit2 = array2[i];
						if (!flag || raycastHit2.distance <= raycastHit.distance)
						{
							Interaction interaction = raycastHit2.transform.GetComponent<Interaction>();
							if (!(interaction == null))
							{
								float range = this.Tools.Find((ToolController.Tool x) => x.InteractionType == interaction.Type).Range;
								if (raycastHit2.distance <= range)
								{
									float num2 = Quaternion.Angle(Quaternion.LookRotation(raycastHit2.transform.position - this.MainCamera.transform.position), this.MainCamera.transform.rotation);
									if (num2 < num)
									{
										num = num2;
										hitPicked = interaction;
										flag2 = true;
									}
								}
							}
						}
					}
					if (flag2)
					{
						this.CurrentInteraction = hitPicked;
						this.CurrentInteractionType = hitPicked.Type;
						if (this.CurrentInteractionType == this.ActiveInteractionType)
						{
							this.ActiveInteraction = this.CurrentInteraction;
						}
						this.CurrentSprite = this.Tools.Find((ToolController.Tool x) => x.InteractionType == this.CurrentInteractionType).Icon;
						this.CurrentRange = this.Tools.Find((ToolController.Tool x) => x.InteractionType == hitPicked.Type).Range;
						return;
					}
				}
			}
		}
		else
		{
			this.InteractionCheckTimer -= 1f * Time.deltaTime;
		}
	}

	// Token: 0x0600073E RID: 1854 RVA: 0x00045320 File Offset: 0x00043520
	private void UpdateDirtFinder()
	{
		if (GameDirector.instance.LevelCompletedDone || (this.DirtFinderInput && this.ForceActiveTimer <= 0f))
		{
			this.CurrentInteractionType = Interaction.InteractionType.DirtFinder;
			this.CurrentSprite = this.Tools.Find((ToolController.Tool x) => x.InteractionType == this.CurrentInteractionType).Icon;
			if (this.ActiveInteractionType != Interaction.InteractionType.DirtFinder && this.ActiveInteractionType != Interaction.InteractionType.None)
			{
				this.DeactivateTool();
			}
			else if (!this.Active && this.CurrentObject == null)
			{
				this.ActivateTool();
			}
			if (this.ActiveInteractionType == Interaction.InteractionType.DirtFinder)
			{
				this.ActiveTimer = this.ActiveTime;
			}
		}
	}

	// Token: 0x0600073F RID: 1855 RVA: 0x000453C4 File Offset: 0x000435C4
	private void UpdateActive()
	{
		if (this.ForceActiveTimer > 0f)
		{
			this.ForceActiveTimer -= 1f * Time.deltaTime;
			this.ForceActiveTimer = Mathf.Max(this.ForceActiveTimer, 0f);
		}
		if (GameDirector.instance.currentState == GameDirector.gameState.Main)
		{
			if (this.ActiveInteractionType != Interaction.InteractionType.DirtFinder && (this.CurrentInteractionType != Interaction.InteractionType.None || this.ActiveInteractionType != Interaction.InteractionType.None) && (this.InteractInput || this.ForceActiveTimer > 0f))
			{
				this.ActiveTimer = this.ActiveTime;
				if (!this.Active && this.CurrentObject == null)
				{
					this.ActivateTool();
				}
			}
			if (this.Active && this.ActiveTimer <= 0f)
			{
				this.DeactivateTool();
			}
			if (this.Active)
			{
				if (this.CurrentInteraction == null || this.ActiveInteractionType == Interaction.InteractionType.DirtFinder)
				{
					this.ActiveTimer -= 1f * Time.deltaTime;
				}
				else if (!this.Interact)
				{
					if (this.RangeCheckTimer <= 0f)
					{
						this.RangeCheck = false;
						Vector3 direction = this.CurrentInteraction.transform.position - this.MainCamera.transform.position;
						RaycastHit[] array = Physics.BoxCastAll(this.MainCamera.transform.position, new Vector3(0.01f, 0.01f, 0.01f), direction, Quaternion.identity, this.InteractionRange, this.Mask);
						if (array.Length != 0)
						{
							foreach (RaycastHit raycastHit in array)
							{
								if (raycastHit.transform.GetComponent<Interaction>() == this.ActiveInteraction && raycastHit.distance <= this.CurrentRange)
								{
									this.RangeCheck = true;
									break;
								}
							}
						}
						this.RangeCheckTimer = 0.2f;
					}
					else
					{
						this.RangeCheckTimer -= 1f * Time.deltaTime;
					}
					if (!this.RangeCheck)
					{
						this.ActiveTimer -= 1f * Time.deltaTime;
					}
				}
			}
		}
		if (this.ActiveInteractionType != Interaction.InteractionType.None)
		{
			PlayerController.instance.CrouchDisable(0.5f);
		}
	}

	// Token: 0x06000740 RID: 1856 RVA: 0x00045600 File Offset: 0x00043800
	private void UpdateInteract()
	{
		if (this.ActiveInteractionType == Interaction.InteractionType.DirtFinder)
		{
			this.Interact = false;
			return;
		}
		if (this.CurrentInteractionType == this.ActiveInteractionType && (this.InteractInput || this.DebugAlwaysInteract))
		{
			this.InteractTimer = 0.25f;
			this.InteractInputDelayed = false;
		}
		if (this.InteractTimer > 0f)
		{
			this.Interact = true;
			this.InteractTimer -= 1f * Time.deltaTime;
			if (this.InteractTimer <= 0f)
			{
				this.Interact = false;
			}
		}
	}

	// Token: 0x06000741 RID: 1857 RVA: 0x00045690 File Offset: 0x00043890
	private void ActivateTool()
	{
		this.ActiveInteractionType = this.CurrentInteractionType;
		this.ActiveInteraction = this.CurrentInteraction;
		this.Active = true;
		foreach (ToolController.Tool tool in this.Tools)
		{
			if (tool.InteractionType == this.CurrentInteractionType)
			{
				this.ToolOffset.transform.localPosition = tool.OffsetPosition;
				this.ToolOffset.transform.localRotation = Quaternion.Euler(tool.OffsetRotation);
				if (tool.HeadBob)
				{
					this.ToolHeadbob.Activate();
				}
				else
				{
					this.ToolHeadbob.Deactivate();
				}
				this.CurrentHidePosition = tool.HidePosition;
				this.CurrentHideRotation = tool.HideRotation;
				this.CurrentHideSpeed = tool.HideSpeed;
				break;
			}
		}
		this.ToolHide.Show();
	}

	// Token: 0x06000742 RID: 1858 RVA: 0x00045794 File Offset: 0x00043994
	public void ShowTool()
	{
		foreach (ToolController.Tool tool in this.Tools)
		{
			if (tool.InteractionType == this.ActiveInteractionType)
			{
				this.CurrentObject = Object.Instantiate<GameObject>(tool.Object, tool.ObjectParent.transform);
				break;
			}
		}
	}

	// Token: 0x06000743 RID: 1859 RVA: 0x0004580C File Offset: 0x00043A0C
	private void DeactivateTool()
	{
		this.Active = false;
		this.ActiveInteractionType = Interaction.InteractionType.None;
		this.ActiveInteraction = null;
		this.ToolHide.Hide();
	}

	// Token: 0x06000744 RID: 1860 RVA: 0x0004582E File Offset: 0x00043A2E
	public void HideTool()
	{
		this.ToolOffset.transform.localPosition = Vector3.zero;
		this.ToolOffset.transform.localRotation = Quaternion.identity;
		Object.Destroy(this.CurrentObject);
	}

	// Token: 0x04000C9F RID: 3231
	public bool DebugAlwaysInteract;

	// Token: 0x04000CA0 RID: 3232
	[HideInInspector]
	public static ToolController instance;

	// Token: 0x04000CA1 RID: 3233
	[HideInInspector]
	public bool Active;

	// Token: 0x04000CA2 RID: 3234
	private float ActiveTime = 0.25f;

	// Token: 0x04000CA3 RID: 3235
	private float ActiveTimer;

	// Token: 0x04000CA4 RID: 3236
	[HideInInspector]
	public bool Interact;

	// Token: 0x04000CA5 RID: 3237
	private float InteractTimer;

	// Token: 0x04000CA6 RID: 3238
	[Space]
	public float InteractionRange = 4f;

	// Token: 0x04000CA7 RID: 3239
	public float InteractionCheckTime = 0.1f;

	// Token: 0x04000CA8 RID: 3240
	private float InteractionCheckTimer;

	// Token: 0x04000CA9 RID: 3241
	private float RangeCheckTimer;

	// Token: 0x04000CAA RID: 3242
	private bool RangeCheck = true;

	// Token: 0x04000CAB RID: 3243
	[HideInInspector]
	public float ForceActiveTimer;

	// Token: 0x04000CAC RID: 3244
	[HideInInspector]
	public Interaction.InteractionType ActiveInteractionType;

	// Token: 0x04000CAD RID: 3245
	[HideInInspector]
	public Interaction.InteractionType CurrentInteractionType;

	// Token: 0x04000CAE RID: 3246
	[HideInInspector]
	public Interaction ActiveInteraction;

	// Token: 0x04000CAF RID: 3247
	[HideInInspector]
	public Interaction CurrentInteraction;

	// Token: 0x04000CB0 RID: 3248
	[HideInInspector]
	public Vector3 CurrentHidePosition;

	// Token: 0x04000CB1 RID: 3249
	[HideInInspector]
	public Vector3 CurrentHideRotation;

	// Token: 0x04000CB2 RID: 3250
	[HideInInspector]
	public float CurrentHideSpeed;

	// Token: 0x04000CB3 RID: 3251
	[HideInInspector]
	public Sprite CurrentSprite;

	// Token: 0x04000CB4 RID: 3252
	private float CurrentRange;

	// Token: 0x04000CB5 RID: 3253
	private GameObject CurrentObject;

	// Token: 0x04000CB6 RID: 3254
	[HideInInspector]
	public Interaction.InteractionType PreviousInteractionType;

	// Token: 0x04000CB7 RID: 3255
	[Space]
	public ToolFollowPush ToolFollowPush;

	// Token: 0x04000CB8 RID: 3256
	public ToolHide ToolHide;

	// Token: 0x04000CB9 RID: 3257
	public Transform ToolFollow;

	// Token: 0x04000CBA RID: 3258
	public Transform ToolOffset;

	// Token: 0x04000CBB RID: 3259
	public ToolFollow ToolHeadbob;

	// Token: 0x04000CBC RID: 3260
	public Transform ToolTargetParent;

	// Token: 0x04000CBD RID: 3261
	public Transform FollowTargetTransform;

	// Token: 0x04000CBE RID: 3262
	private LayerMask Mask;

	// Token: 0x04000CBF RID: 3263
	private LayerMask VisibilityMask;

	// Token: 0x04000CC0 RID: 3264
	public List<ToolController.Tool> Tools;

	// Token: 0x04000CC1 RID: 3265
	private Camera MainCamera;

	// Token: 0x04000CC2 RID: 3266
	private bool InteractInput;

	// Token: 0x04000CC3 RID: 3267
	private bool InteractInputDelayed;

	// Token: 0x04000CC4 RID: 3268
	private bool DirtFinderInput;

	// Token: 0x04000CC5 RID: 3269
	private float DisableTimer;

	// Token: 0x04000CC6 RID: 3270
	public PlayerAvatar playerAvatarScript;

	// Token: 0x02000337 RID: 823
	[Serializable]
	public class Tool
	{
		// Token: 0x040029B1 RID: 10673
		public string Name;

		// Token: 0x040029B2 RID: 10674
		public Interaction.InteractionType InteractionType;

		// Token: 0x040029B3 RID: 10675
		[Space]
		public GameObject Object;

		// Token: 0x040029B4 RID: 10676
		public GameObject ObjectParent;

		// Token: 0x040029B5 RID: 10677
		public GameObject playerAvatarPrefab;

		// Token: 0x040029B6 RID: 10678
		[Space]
		public Vector3 HidePosition;

		// Token: 0x040029B7 RID: 10679
		public Vector3 HideRotation;

		// Token: 0x040029B8 RID: 10680
		public float HideSpeed = 2f;

		// Token: 0x040029B9 RID: 10681
		[Space]
		public Vector3 OffsetPosition;

		// Token: 0x040029BA RID: 10682
		public Vector3 OffsetRotation;

		// Token: 0x040029BB RID: 10683
		[Space]
		public Sprite Icon;

		// Token: 0x040029BC RID: 10684
		public bool HeadBob = true;

		// Token: 0x040029BD RID: 10685
		public float Range = 1f;
	}
}
