using System;
using UnityEngine;

// Token: 0x020000DB RID: 219
public class RecordingDirector : MonoBehaviour
{
	// Token: 0x060007DB RID: 2011 RVA: 0x0004CE08 File Offset: 0x0004B008
	private void Start()
	{
		if (RecordingDirector.instance != null && RecordingDirector.instance != this)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		RecordingDirector.instance = this;
		Object.DontDestroyOnLoad(base.gameObject);
	}

	// Token: 0x060007DC RID: 2012 RVA: 0x0004CE44 File Offset: 0x0004B044
	private void Update()
	{
		if (!PlayerAvatar.instance.isDisabled)
		{
			this.playerLight.transform.position = PlayerAvatar.instance.PlayerVisionTarget.VisionTransform.position;
		}
		if (Input.GetKey(KeyCode.Keypad4))
		{
			float num;
			float num2;
			float num3;
			Color.RGBToHSV(this.playerLight.color, out num, out num2, out num3);
			num = (num + Time.deltaTime * 0.2f) % 1f;
			this.playerLight.color = Color.HSVToRGB(num, 1f, 1f);
		}
		if (Input.GetKey(KeyCode.Keypad6))
		{
			float num4;
			float num5;
			float num6;
			Color.RGBToHSV(this.playerLight.color, out num4, out num5, out num6);
			num4 = (num4 - Time.deltaTime * 0.2f) % 1f;
			this.playerLight.color = Color.HSVToRGB(num4, 1f, 1f);
		}
		if (Input.GetKey(KeyCode.Keypad8))
		{
			this.playerLight.range += Time.deltaTime * 30f;
		}
		if (Input.GetKey(KeyCode.Keypad2))
		{
			this.playerLight.range -= Time.deltaTime * 30f;
		}
		if (Input.GetKey(KeyCode.Keypad7))
		{
			this.playerLight.intensity += Time.deltaTime * 2f;
		}
		if (Input.GetKey(KeyCode.Keypad9))
		{
			this.playerLight.intensity -= Time.deltaTime * 2f;
		}
		if (Input.GetKey(KeyCode.Keypad0))
		{
			this.playerLight.intensity = 1f;
			this.playerLight.range = 10f;
			this.playerLight.color = Color.white;
		}
		if (!MenuPageEsc.instance && !ChatManager.instance.chatActive)
		{
			this.hideUI = true;
		}
		else
		{
			this.hideUI = false;
		}
		if (this.hideUI)
		{
			RenderTextureMain.instance.OverlayDisable();
		}
		FlashlightController.Instance.hideFlashlight = true;
		GameplayManager.instance.OverrideCameraAnimation(0f, 0.2f);
		if (SemiFunc.RunIsLobbyMenu() || SemiFunc.MenuLevel())
		{
			GameDirector.instance.CommandRecordingDirectorToggle();
		}
	}

	// Token: 0x04000E1D RID: 3613
	internal bool hideUI;

	// Token: 0x04000E1E RID: 3614
	public static RecordingDirector instance;

	// Token: 0x04000E1F RID: 3615
	public Light playerLight;

	// Token: 0x04000E20 RID: 3616
	private float lightHue;
}
