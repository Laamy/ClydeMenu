using System;
using UnityEngine;

// Token: 0x020001B5 RID: 437
public class PlayerAvatarMenu : MonoBehaviour
{
	// Token: 0x06000EE7 RID: 3815 RVA: 0x000863B8 File Offset: 0x000845B8
	private void Awake()
	{
		this.startPosition = new Vector3(0f, 0f, -2000f);
		if (PlayerAvatarMenu.instance && PlayerAvatarMenu.instance.startPosition == this.startPosition)
		{
			this.startPosition = new Vector3(0f, 4f, -2000f);
		}
		this.playerVisuals = base.GetComponentInChildren<PlayerAvatarVisuals>();
		if (this.expressionAvatar)
		{
			this.startPosition = new Vector3(0f, 0f, -1000f);
			return;
		}
		PlayerAvatarMenu.instance = this;
	}

	// Token: 0x06000EE8 RID: 3816 RVA: 0x00086454 File Offset: 0x00084654
	private void Start()
	{
		this.rb = base.GetComponent<Rigidbody>();
		if (!this.expressionAvatar)
		{
			this.parentPage = base.GetComponentInParent<MenuPage>();
		}
		this.playerVisuals.expressionAvatar = this.expressionAvatar;
		base.transform.SetParent(null);
		base.transform.localScale = Vector3.one;
		if (this.expressionAvatar)
		{
			this.startPosition = new Vector3(0f, 0f, -1000f);
		}
		base.transform.position = this.startPosition;
		this.cameraAndStuff.SetParent(null);
		this.cameraAndStuff.localScale = Vector3.one;
	}

	// Token: 0x06000EE9 RID: 3817 RVA: 0x00086500 File Offset: 0x00084700
	private void FixedUpdate()
	{
		if (this.expressionAvatar)
		{
			return;
		}
		this.rb.MovePosition(this.startPosition);
		if (this.rotationForce.magnitude > 0.1f)
		{
			this.rb.AddTorque(this.rotationForce * Time.fixedDeltaTime);
			this.rotationForce = Vector3.zero;
			this.rb.angularVelocity = Vector3.ClampMagnitude(this.rb.angularVelocity, 1f);
		}
	}

	// Token: 0x06000EEA RID: 3818 RVA: 0x00086580 File Offset: 0x00084780
	private void Update()
	{
		if (this.expressionAvatar)
		{
			return;
		}
		if (SemiFunc.InputMovementX() > 0.01f || SemiFunc.InputMovementX() < -0.01f)
		{
			float y = -SemiFunc.InputMovementX() * 3000f;
			this.Rotate(new Vector3(0f, y, 0f));
		}
		if (!this.parentPage)
		{
			Object.Destroy(this.cameraAndStuff.gameObject);
			Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x06000EEB RID: 3819 RVA: 0x000865F9 File Offset: 0x000847F9
	public void Rotate(Vector3 _rotation)
	{
		this.rotationForce = _rotation;
	}

	// Token: 0x04001883 RID: 6275
	public static PlayerAvatarMenu instance;

	// Token: 0x04001884 RID: 6276
	public Transform cameraAndStuff;

	// Token: 0x04001885 RID: 6277
	private MenuPage parentPage;

	// Token: 0x04001886 RID: 6278
	public bool expressionAvatar;

	// Token: 0x04001887 RID: 6279
	private Vector3 startPosition;

	// Token: 0x04001888 RID: 6280
	internal Rigidbody rb;

	// Token: 0x04001889 RID: 6281
	private Vector3 rotationForce;

	// Token: 0x0400188A RID: 6282
	internal PlayerAvatarVisuals playerVisuals;
}
