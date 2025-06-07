using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020000B3 RID: 179
[RequireComponent(typeof(Renderer))]
public class CanvasAssigner : MonoBehaviour
{
	// Token: 0x060006DB RID: 1755 RVA: 0x00041B92 File Offset: 0x0003FD92
	private void Awake()
	{
		this.AssignRandomTexture();
	}

	// Token: 0x060006DC RID: 1756 RVA: 0x00041B9C File Offset: 0x0003FD9C
	private void AssignRandomTexture()
	{
		if (CanvasAssigner.AvailableTextures.Count == 0)
		{
			CanvasList.PopulateAvailableTextures();
		}
		int num = Random.Range(0, CanvasAssigner.AvailableTextures.Count);
		Texture mainTexture = CanvasAssigner.AvailableTextures[num];
		CanvasAssigner.AvailableTextures.RemoveAt(num);
		Renderer component = base.GetComponent<Renderer>();
		if (component && component.material)
		{
			component.material.mainTexture = mainTexture;
		}
	}

	// Token: 0x04000B9D RID: 2973
	public static List<Texture> AvailableTextures = new List<Texture>();
}
