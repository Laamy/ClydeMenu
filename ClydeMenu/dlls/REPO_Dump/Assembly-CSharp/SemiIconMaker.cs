using System;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x0200024B RID: 587
public class SemiIconMaker : MonoBehaviour
{
	// Token: 0x06001309 RID: 4873 RVA: 0x000AA4A0 File Offset: 0x000A86A0
	public Sprite CreateIconFromRenderTexture()
	{
		if (!this.renderTexture)
		{
			Debug.LogError("RenderTexture is null");
			return null;
		}
		if (!ItemManager.instance.firstIcon)
		{
			Light[] componentsInChildren = base.GetComponentsInChildren<Light>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].enabled = false;
			}
		}
		else
		{
			ItemManager.instance.firstIcon = false;
		}
		Transform transform = base.GetComponentInParent<ItemAttributes>().transform;
		Vector3 position = transform.position;
		transform.position = new Vector3(-1000f, -1000f, -1000f);
		Texture2D texture2D = new Texture2D(this.renderTexture.width, this.renderTexture.height);
		RenderTexture active = RenderTexture.active;
		RenderTexture.active = this.renderTexture;
		RenderSettings.fog = false;
		Color ambientLight = RenderSettings.ambientLight;
		RenderSettings.ambientLight = Color.white;
		this.iconCamera.Render();
		RenderSettings.fog = true;
		RenderSettings.ambientLight = ambientLight;
		texture2D.ReadPixels(new Rect(0f, 0f, (float)this.renderTexture.width, (float)this.renderTexture.height), 0, 0);
		texture2D.Apply();
		RenderTexture.active = active;
		transform.position = position;
		Sprite result = Sprite.Create(texture2D, new Rect(0f, 0f, (float)texture2D.width, (float)texture2D.height), new Vector2(0.5f, 0.5f));
		base.gameObject.SetActive(false);
		return result;
	}

	// Token: 0x04002073 RID: 8307
	[FormerlySerializedAs("camera")]
	public Camera iconCamera;

	// Token: 0x04002074 RID: 8308
	public RenderTexture renderTexture;

	// Token: 0x04002075 RID: 8309
	public bool iconCameraPlacementDone;
}
