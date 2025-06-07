using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000217 RID: 535
public class MenuButtonPopUp : MonoBehaviour
{
	// Token: 0x04001E40 RID: 7744
	public bool richText = true;

	// Token: 0x04001E41 RID: 7745
	[Space]
	public UnityEvent option1Event;

	// Token: 0x04001E42 RID: 7746
	public UnityEvent option2Event;

	// Token: 0x04001E43 RID: 7747
	public string headerText = "Oh really?";

	// Token: 0x04001E44 RID: 7748
	public Color headerColor = new Color(1f, 0.55f, 0f);

	// Token: 0x04001E45 RID: 7749
	[TextArea(3, 10)]
	public string bodyText = "Is that really so?";

	// Token: 0x04001E46 RID: 7750
	public string option1Text = "Yes!";

	// Token: 0x04001E47 RID: 7751
	public string option2Text = "No";
}
