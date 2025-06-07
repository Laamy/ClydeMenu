using System;
using UnityEngine;

// Token: 0x02000234 RID: 564
public class AnimScaleFlicker : MonoBehaviour
{
	// Token: 0x06001290 RID: 4752 RVA: 0x000A6E2C File Offset: 0x000A502C
	private void Start()
	{
		this.xInit = base.transform.localScale.x;
		this.yInit = base.transform.localScale.y;
		this.zInit = base.transform.localScale.z;
	}

	// Token: 0x06001291 RID: 4753 RVA: 0x000A6E7C File Offset: 0x000A507C
	private void Update()
	{
		float num = Mathf.LerpUnclamped(this.xOld, this.xNew, this.animCurve.Evaluate(this.xLerp));
		this.xLerp += this.xSpeed * this.speedMult * Time.deltaTime;
		if (this.xLerp >= 1f)
		{
			this.xOld = this.xNew;
			this.xNew = Random.Range(this.xAmountMin, this.xAmountMax);
			this.xSpeed = Random.Range(this.xSpeedMin, this.xSpeedMax);
			this.xLerp = 0f;
		}
		float num2 = Mathf.LerpUnclamped(this.yOld, this.yNew, this.animCurve.Evaluate(this.yLerp));
		this.yLerp += this.ySpeed * this.speedMult * Time.deltaTime;
		if (this.yLerp >= 1f)
		{
			this.yOld = this.yNew;
			this.yNew = Random.Range(this.yAmountMin, this.yAmountMax);
			this.ySpeed = Random.Range(this.ySpeedMin, this.ySpeedMax);
			this.yLerp = 0f;
		}
		float num3 = Mathf.LerpUnclamped(this.zOld, this.zNew, this.animCurve.Evaluate(this.zLerp));
		this.zLerp += this.zSpeed * this.speedMult * Time.deltaTime;
		if (this.zLerp >= 1f)
		{
			this.zOld = this.zNew;
			this.zNew = Random.Range(this.zAmountMin, this.zAmountMax);
			this.zSpeed = Random.Range(this.zSpeedMin, this.zSpeedMax);
			this.zLerp = 0f;
		}
		base.transform.localScale = new Vector3(this.xInit + num * this.amountMult, this.yInit + num2 * this.amountMult, this.zInit + num3 * this.amountMult);
	}

	// Token: 0x04001F6E RID: 8046
	public AnimationCurve animCurve;

	// Token: 0x04001F6F RID: 8047
	public float amountMult = 1f;

	// Token: 0x04001F70 RID: 8048
	public float speedMult = 1f;

	// Token: 0x04001F71 RID: 8049
	[Header("Scale X")]
	public float xAmountMin;

	// Token: 0x04001F72 RID: 8050
	public float xAmountMax;

	// Token: 0x04001F73 RID: 8051
	public float xSpeedMin;

	// Token: 0x04001F74 RID: 8052
	public float xSpeedMax;

	// Token: 0x04001F75 RID: 8053
	private float xInit;

	// Token: 0x04001F76 RID: 8054
	private float xOld;

	// Token: 0x04001F77 RID: 8055
	private float xNew;

	// Token: 0x04001F78 RID: 8056
	private float xSpeed;

	// Token: 0x04001F79 RID: 8057
	private float xLerp = 1f;

	// Token: 0x04001F7A RID: 8058
	[Header("Scale Y")]
	public float yAmountMin;

	// Token: 0x04001F7B RID: 8059
	public float yAmountMax;

	// Token: 0x04001F7C RID: 8060
	public float ySpeedMin;

	// Token: 0x04001F7D RID: 8061
	public float ySpeedMax;

	// Token: 0x04001F7E RID: 8062
	private float yInit;

	// Token: 0x04001F7F RID: 8063
	private float yOld;

	// Token: 0x04001F80 RID: 8064
	private float yNew;

	// Token: 0x04001F81 RID: 8065
	private float ySpeed;

	// Token: 0x04001F82 RID: 8066
	private float yLerp = 1f;

	// Token: 0x04001F83 RID: 8067
	[Header("Scale Z")]
	public float zAmountMin;

	// Token: 0x04001F84 RID: 8068
	public float zAmountMax;

	// Token: 0x04001F85 RID: 8069
	public float zSpeedMin;

	// Token: 0x04001F86 RID: 8070
	public float zSpeedMax;

	// Token: 0x04001F87 RID: 8071
	private float zInit;

	// Token: 0x04001F88 RID: 8072
	private float zOld;

	// Token: 0x04001F89 RID: 8073
	private float zNew;

	// Token: 0x04001F8A RID: 8074
	private float zSpeed;

	// Token: 0x04001F8B RID: 8075
	private float zLerp = 1f;
}
