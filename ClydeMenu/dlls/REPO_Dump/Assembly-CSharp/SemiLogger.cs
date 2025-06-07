using System;
using UnityEngine;

// Token: 0x02000132 RID: 306
public class SemiLogger : MonoBehaviour
{
	// Token: 0x06000AA6 RID: 2726 RVA: 0x0005EA28 File Offset: 0x0005CC28
	[HideInCallstack]
	public static void Log(SemiFunc.User _user, object _message, GameObject _obj = null, Color? color = null)
	{
		if (SemiFunc.DebugUser(_user))
		{
			string text = ColorUtility.ToHtmlStringRGB(color ?? Color.Lerp(Color.gray, Color.white, 0.4f));
			Debug.Log(string.Format("<color=#{0}>{1}</color>", text, _message), _obj);
		}
	}

	// Token: 0x06000AA7 RID: 2727 RVA: 0x0005EA7D File Offset: 0x0005CC7D
	[HideInCallstack]
	public static void LogAxel(object _message, GameObject _obj = null, Color? color = null)
	{
		SemiLogger.Log(SemiFunc.User.Axel, _message, _obj, color);
	}

	// Token: 0x06000AA8 RID: 2728 RVA: 0x0005EA88 File Offset: 0x0005CC88
	[HideInCallstack]
	public static void LogJannek(object _message, GameObject _obj = null, Color? color = null)
	{
		SemiLogger.Log(SemiFunc.User.Jannek, _message, _obj, color);
	}

	// Token: 0x06000AA9 RID: 2729 RVA: 0x0005EA93 File Offset: 0x0005CC93
	[HideInCallstack]
	public static void LogRobin(object _message, GameObject _obj = null, Color? color = null)
	{
		SemiLogger.Log(SemiFunc.User.Robin, _message, _obj, color);
	}

	// Token: 0x06000AAA RID: 2730 RVA: 0x0005EA9E File Offset: 0x0005CC9E
	[HideInCallstack]
	public static void LogRuben(object _message, GameObject _obj = null, Color? color = null)
	{
		SemiLogger.Log(SemiFunc.User.Ruben, _message, _obj, color);
	}

	// Token: 0x06000AAB RID: 2731 RVA: 0x0005EAA9 File Offset: 0x0005CCA9
	[HideInCallstack]
	public static void LogWalter(object _message, GameObject _obj = null, Color? color = null)
	{
		SemiLogger.Log(SemiFunc.User.Walter, _message, _obj, color);
	}

	// Token: 0x06000AAC RID: 2732 RVA: 0x0005EAB4 File Offset: 0x0005CCB4
	[HideInCallstack]
	public static void LogMonika(object _message, GameObject _obj = null, Color? color = null)
	{
		SemiLogger.Log(SemiFunc.User.Monika, _message, _obj, color);
	}
}
