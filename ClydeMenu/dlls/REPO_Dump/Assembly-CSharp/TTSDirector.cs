using System;
using LeastSquares.Overtone;
using UnityEngine;

// Token: 0x02000135 RID: 309
public class TTSDirector : MonoBehaviour
{
	// Token: 0x06000AB4 RID: 2740 RVA: 0x0005EB36 File Offset: 0x0005CD36
	private void Start()
	{
		TTSDirector.instance = this;
		this.engine = base.GetComponent<TTSEngine>();
	}

	// Token: 0x0400114E RID: 4430
	public static TTSDirector instance;

	// Token: 0x0400114F RID: 4431
	internal TTSEngine engine;
}
