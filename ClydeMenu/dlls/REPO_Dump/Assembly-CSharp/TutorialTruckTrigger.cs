using System;
using UnityEngine;

// Token: 0x0200028D RID: 653
public class TutorialTruckTrigger : MonoBehaviour
{
	// Token: 0x06001482 RID: 5250 RVA: 0x000B4FAB File Offset: 0x000B31AB
	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			this.triggered = true;
		}
	}

	// Token: 0x06001483 RID: 5251 RVA: 0x000B4FC4 File Offset: 0x000B31C4
	private void Update()
	{
		if (this.triggered && base.GetComponent<Collider>().enabled)
		{
			this.lockLookTimer = 0.5f;
			base.GetComponent<Collider>().enabled = false;
			CameraGlitch.Instance.PlayLong();
		}
		if (this.lockLookTimer > 0f)
		{
			this.lockLookTimer -= Time.deltaTime;
			CameraAim.Instance.AimTargetSet(this.lookTarget.position + Vector3.down, 0.1f, 5f, base.gameObject, 90);
		}
		if (this.triggered)
		{
			if (this.messageDelay > 0f)
			{
				this.messageDelay -= Time.deltaTime;
				return;
			}
			if (!this.messageSent)
			{
				TruckScreenText component = this.lookTarget.GetComponent<TruckScreenText>();
				if (!component.isTyping && component.delayTimer <= 0f)
				{
					component.GotoPage(1);
				}
				this.messageSent = true;
			}
		}
	}

	// Token: 0x0400230E RID: 8974
	private float lockLookTimer;

	// Token: 0x0400230F RID: 8975
	public Transform lookTarget;

	// Token: 0x04002310 RID: 8976
	private float messageDelay = 1.5f;

	// Token: 0x04002311 RID: 8977
	private bool messageSent;

	// Token: 0x04002312 RID: 8978
	private bool triggered;
}
