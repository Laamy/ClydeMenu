using System;
using TMPro;
using UnityEngine;

// Token: 0x02000263 RID: 611
public class DebugFPS : MonoBehaviour
{
	// Token: 0x06001399 RID: 5017 RVA: 0x000AEBE7 File Offset: 0x000ACDE7
	private void Awake()
	{
	}

	// Token: 0x0600139A RID: 5018 RVA: 0x000AEBEC File Offset: 0x000ACDEC
	private void Update()
	{
		this.Text.text = "FPS: " + ((int)(1f / Time.unscaledDeltaTime)).ToString();
	}

	// Token: 0x040021BD RID: 8637
	public TextMeshProUGUI Text;
}
