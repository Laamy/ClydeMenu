using System;
using UnityEngine;

// Token: 0x02000170 RID: 368
[CreateAssetMenu(fileName = "Color Preset", menuName = "Semi Presets/Color Preset")]
public class ColorPresets : ScriptableObject
{
	// Token: 0x06000C66 RID: 3174 RVA: 0x0006E178 File Offset: 0x0006C378
	public Color GetColorMain()
	{
		return this.colorMain;
	}

	// Token: 0x06000C67 RID: 3175 RVA: 0x0006E180 File Offset: 0x0006C380
	public Color GetColorLight()
	{
		return this.colorLight;
	}

	// Token: 0x06000C68 RID: 3176 RVA: 0x0006E188 File Offset: 0x0006C388
	public Color GetColorDark()
	{
		return this.colorDark;
	}

	// Token: 0x04001439 RID: 5177
	public Color colorMain;

	// Token: 0x0400143A RID: 5178
	public Color colorLight;

	// Token: 0x0400143B RID: 5179
	public Color colorDark;
}
