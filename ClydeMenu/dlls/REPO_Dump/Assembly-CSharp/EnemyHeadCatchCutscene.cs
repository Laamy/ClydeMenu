using System;
using UnityEngine;

// Token: 0x02000054 RID: 84
public class EnemyHeadCatchCutscene : MonoBehaviour
{
	// Token: 0x060002EE RID: 750 RVA: 0x0001D77C File Offset: 0x0001B97C
	public void PlayBiteBegin()
	{
		this.BiteBegin.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x060002EF RID: 751 RVA: 0x0001D7AC File Offset: 0x0001B9AC
	public void PlayBiteFirst()
	{
		this.BiteFirst.Play(base.transform.position, 1f, 1f, 1f, 1f);
		this.CameraGlitchShort01.Play(base.transform.position, 1f, 1f, 1f, 1f);
		this.GlassBreak01.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x060002F0 RID: 752 RVA: 0x0001D83C File Offset: 0x0001BA3C
	public void PlayBiteLast()
	{
		this.BiteLast.Play(base.transform.position, 1f, 1f, 1f, 1f);
		this.CameraGlitchShort02.Play(base.transform.position, 1f, 1f, 1f, 1f);
		this.GlassBreak02.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x060002F1 RID: 753 RVA: 0x0001D8CA File Offset: 0x0001BACA
	public void PlayMusic01()
	{
		this.Music01.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x060002F2 RID: 754 RVA: 0x0001D8F7 File Offset: 0x0001BAF7
	public void PlayMusic02()
	{
		this.Music02.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x060002F3 RID: 755 RVA: 0x0001D924 File Offset: 0x0001BB24
	public void PlayCameraGlitchLong01()
	{
		this.CameraGlitchLong01.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x060002F4 RID: 756 RVA: 0x0001D951 File Offset: 0x0001BB51
	public void PlayCameraGlitchLong02()
	{
		this.CameraGlitchLong02.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x060002F5 RID: 757 RVA: 0x0001D97E File Offset: 0x0001BB7E
	public void PlayGlassTension()
	{
		this.GlassTension.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x0400050A RID: 1290
	public Sound BiteBegin;

	// Token: 0x0400050B RID: 1291
	public Sound BiteFirst;

	// Token: 0x0400050C RID: 1292
	public Sound BiteLast;

	// Token: 0x0400050D RID: 1293
	[Space]
	public Sound Music01;

	// Token: 0x0400050E RID: 1294
	public Sound Music02;

	// Token: 0x0400050F RID: 1295
	[Space]
	public Sound CameraGlitchLong01;

	// Token: 0x04000510 RID: 1296
	public Sound CameraGlitchLong02;

	// Token: 0x04000511 RID: 1297
	[Space]
	public Sound CameraGlitchShort01;

	// Token: 0x04000512 RID: 1298
	public Sound CameraGlitchShort02;

	// Token: 0x04000513 RID: 1299
	[Space]
	public Sound GlassBreak01;

	// Token: 0x04000514 RID: 1300
	public Sound GlassBreak02;

	// Token: 0x04000515 RID: 1301
	public Sound GlassTension;
}
