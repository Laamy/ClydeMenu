using System;
using UnityEngine;

// Token: 0x020000B5 RID: 181
public class CanvasList : MonoBehaviour
{
	// Token: 0x060006E9 RID: 1769 RVA: 0x000420E1 File Offset: 0x000402E1
	private void Awake()
	{
		CanvasList.PopulateAvailableTextures();
	}

	// Token: 0x060006EA RID: 1770 RVA: 0x000420E8 File Offset: 0x000402E8
	public static void PopulateAvailableTextures()
	{
		CanvasAssigner.AvailableTextures.Clear();
		Texture2D[] array = Resources.LoadAll<Texture2D>("Canvas");
		if (array.Length == 0)
		{
			Debug.LogWarning("No textures were loaded from the Resources/Canvas folder.");
		}
		foreach (Texture2D texture2D in array)
		{
			if (texture2D != null)
			{
				CanvasAssigner.AvailableTextures.Add(texture2D);
			}
			else
			{
				Debug.LogWarning("A texture was found but is not a Texture2D or could not be cast to Texture2D.");
			}
		}
	}
}
