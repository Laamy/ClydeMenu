using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020002BC RID: 700
public class ToyMonkeyTrap : Trap
{
	// Token: 0x0600160E RID: 5646 RVA: 0x000C1A3C File Offset: 0x000BFC3C
	protected override void Start()
	{
		base.Start();
		this.rb = base.GetComponent<Rigidbody>();
	}

	// Token: 0x0600160F RID: 5647 RVA: 0x000C1A50 File Offset: 0x000BFC50
	protected override void Update()
	{
		base.Update();
		if (this.trapStart)
		{
			this.ToyMonkeyTrapActivated();
		}
		this.mechanicalLoop.PlayLoop(this.trapPlaying, 0.8f, 0.8f, 1f);
		if (this.trapActive)
		{
			this.enemyInvestigate = true;
			this.trapPlaying = true;
			if (this.armRotationLerp < 1f)
			{
				this.armRotationLerp += Time.deltaTime * 3f;
			}
			if (this.armRotationLerp >= 1f)
			{
				this.armRotationLerp = 0f;
				Vector3 a = Vector3.Slerp(Vector3.up, base.transform.right, 0.25f);
				this.cymbal.Play(this.physGrabObject.centerPoint, 1f, 1f, 1f, 1f);
				Vector3 normalized = Random.insideUnitSphere.normalized;
				this.rb.AddForce(a * 1.3f, ForceMode.Impulse);
				this.rb.AddTorque(base.transform.up * Random.Range(-0.25f, 0.25f), ForceMode.Impulse);
			}
			float num = Mathf.Lerp(-15f, 40f, this.armAnimationCurve.Evaluate(this.armRotationLerp));
			this.leftArm.localEulerAngles = new Vector3(0f, num, 0f);
			this.rightArm.localEulerAngles = new Vector3(0f, -num, 0f);
			if (this.headRotationXLerp < 1f)
			{
				this.headRotationXLerp += Time.deltaTime * this.headRotationSpeed;
			}
			if (this.headRotationXLerp >= 1f)
			{
				this.headRotationXLerp = 0f;
			}
			float x = Mathf.Lerp(-15f, 15f, this.headAnimationCurve.Evaluate(this.headRotationXLerp));
			if (this.headRotationZLerp < 1f)
			{
				this.headRotationZLerp += Time.deltaTime * this.headRotationSpeed;
			}
			if (this.headRotationZLerp >= 1f)
			{
				this.headRotationZLerp = 0f;
			}
			float z = Mathf.Lerp(-15f, 15f, this.headAnimationCurve.Evaluate(this.headRotationZLerp));
			this.head.localEulerAngles = new Vector3(x, 0f, z);
		}
	}

	// Token: 0x06001610 RID: 5648 RVA: 0x000C1CB0 File Offset: 0x000BFEB0
	private void FixedUpdate()
	{
		if (this.trapActive && this.isLocal)
		{
			Vector3 normalized = Random.insideUnitSphere.normalized;
			if (this.physGrabObject.playerGrabbing.Count == 0)
			{
				if (this.spinLerp < 1f)
				{
					this.spinLerp += Time.deltaTime;
				}
				float d = Mathf.Lerp(0f, 0.5f, this.spinLerp);
				this.rb.AddTorque(base.transform.right * d + normalized * 0.05f, ForceMode.Force);
				return;
			}
			this.spinLerp = 0f;
			this.rb.AddTorque(normalized * 0.5f, ForceMode.Force);
		}
	}

	// Token: 0x06001611 RID: 5649 RVA: 0x000C1D78 File Offset: 0x000BFF78
	public void ToyMonkeyTrapStop()
	{
		this.trapActive = false;
		this.trapPlaying = false;
	}

	// Token: 0x06001612 RID: 5650 RVA: 0x000C1D88 File Offset: 0x000BFF88
	public void ToyMonkeyTrapActivated()
	{
		if (!this.trapTriggered)
		{
			this.toyMonkeyTimer.Invoke();
			this.trapTriggered = true;
			this.trapActive = true;
		}
	}

	// Token: 0x04002637 RID: 9783
	public UnityEvent toyMonkeyTimer;

	// Token: 0x04002638 RID: 9784
	[Space]
	[Header("Components")]
	public Transform head;

	// Token: 0x04002639 RID: 9785
	public Transform leftArm;

	// Token: 0x0400263A RID: 9786
	public Transform rightArm;

	// Token: 0x0400263B RID: 9787
	[Space]
	[Header("Sounds")]
	public Sound cymbal;

	// Token: 0x0400263C RID: 9788
	public Sound mechanicalLoop;

	// Token: 0x0400263D RID: 9789
	[Space]
	[Header("Animation")]
	public AnimationCurve armAnimationCurve;

	// Token: 0x0400263E RID: 9790
	public AnimationCurve headAnimationCurve;

	// Token: 0x0400263F RID: 9791
	private float armRotationLerp = 0.5f;

	// Token: 0x04002640 RID: 9792
	private float headRotationXLerp;

	// Token: 0x04002641 RID: 9793
	private float headRotationZLerp = 0.25f;

	// Token: 0x04002642 RID: 9794
	private float headRotationSpeed = 4f;

	// Token: 0x04002643 RID: 9795
	private float spinLerp;

	// Token: 0x04002644 RID: 9796
	[Space]
	private Rigidbody rb;

	// Token: 0x04002645 RID: 9797
	private bool trapPlaying;
}
