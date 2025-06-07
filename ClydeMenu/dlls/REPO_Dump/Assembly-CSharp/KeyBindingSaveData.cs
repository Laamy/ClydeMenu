using System;
using System.Collections.Generic;

// Token: 0x020001F1 RID: 497
[Serializable]
public class KeyBindingSaveData
{
	// Token: 0x04001CC7 RID: 7367
	public Dictionary<InputKey, List<string>> bindingOverrides;

	// Token: 0x04001CC8 RID: 7368
	public Dictionary<InputKey, bool> inputToggleStates;

	// Token: 0x04001CC9 RID: 7369
	public Dictionary<InputPercentSetting, int> inputPercentSettings;
}
