using System;
using UnityEngine;

// Token: 0x02000219 RID: 537
public class MenuElementAnimations : MonoBehaviour
{
	// Token: 0x060011E7 RID: 4583 RVA: 0x000A2A98 File Offset: 0x000A0C98
	private void Start()
	{
		this.rectTransform = base.GetComponent<RectTransform>();
		if (this.forceMiddlePivot)
		{
			this.rectTransform.pivot = new Vector2(0.5f, 0.5f);
		}
		this.initialPosition = this.rectTransform.anchoredPosition;
		this.initialScale = this.rectTransform.localScale.x;
		this.initialRotation = this.rectTransform.localEulerAngles.z;
		this.springFloatPosX = new SpringFloat();
		this.springFloatPosY = new SpringFloat();
		this.springFloatScale = new SpringFloat();
		this.springFloatRotation = new SpringFloat();
		this.springFloatPosX.lastPosition = this.initialPosition.x;
		this.springFloatPosY.lastPosition = this.initialPosition.y;
		this.springFloatScale.lastPosition = this.initialScale;
		this.springFloatRotation.lastPosition = this.initialRotation;
	}

	// Token: 0x060011E8 RID: 4584 RVA: 0x000A2B8C File Offset: 0x000A0D8C
	private void Update()
	{
		float x = SemiFunc.SpringFloatGet(this.springFloatPosX, this.initialPosition.x, -1f);
		float y = SemiFunc.SpringFloatGet(this.springFloatPosY, this.initialPosition.y, -1f);
		float num = SemiFunc.SpringFloatGet(this.springFloatScale, this.initialScale, -1f);
		float z = SemiFunc.SpringFloatGet(this.springFloatRotation, this.initialRotation, -1f);
		this.rectTransform.anchoredPosition = new Vector2(x, y);
		this.rectTransform.localScale = new Vector3(num, num, 1f);
		this.rectTransform.localEulerAngles = new Vector3(0f, 0f, z);
	}

	// Token: 0x060011E9 RID: 4585 RVA: 0x000A2C43 File Offset: 0x000A0E43
	public void UIAniNewInitialPosition(Vector2 newPos)
	{
		this.initialPosition = newPos;
	}

	// Token: 0x060011EA RID: 4586 RVA: 0x000A2C4C File Offset: 0x000A0E4C
	public void UIAniNudgeX(float nudgeForce = 10f, float dampen = 0.2f, float springStrengthMultiplier = 1f)
	{
		this.springFloatPosX.damping = dampen;
		this.springFloatPosX.springVelocity = nudgeForce * 100f;
		this.springFloatPosX.speed = nudgeForce * 5f * springStrengthMultiplier;
	}

	// Token: 0x060011EB RID: 4587 RVA: 0x000A2C80 File Offset: 0x000A0E80
	public void UIAniNudgeY(float nudgeForce = 10f, float dampen = 0.2f, float springStrengthMultiplier = 1f)
	{
		this.springFloatPosY.damping = dampen;
		this.springFloatPosY.springVelocity = nudgeForce * 100f;
		this.springFloatPosY.speed = nudgeForce * 5f * springStrengthMultiplier;
	}

	// Token: 0x060011EC RID: 4588 RVA: 0x000A2CB4 File Offset: 0x000A0EB4
	public void UIAniScale(float scaleForce = 2f, float dampen = 0.2f, float springStrengthMultiplier = 1f)
	{
		this.springFloatScale.damping = dampen;
		this.springFloatScale.springVelocity = scaleForce * 1f;
		this.springFloatScale.speed = scaleForce * 15f * springStrengthMultiplier;
	}

	// Token: 0x060011ED RID: 4589 RVA: 0x000A2CE8 File Offset: 0x000A0EE8
	public void UIAniRotate(float rotateForce = 2f, float dampen = 0.2f, float springStrengthMultiplier = 1f)
	{
		this.springFloatRotation.damping = dampen;
		this.springFloatRotation.springVelocity = rotateForce * 100f;
		this.springFloatRotation.speed = rotateForce * 15f * springStrengthMultiplier;
	}

	// Token: 0x04001E4D RID: 7757
	private SpringFloat springFloatScale;

	// Token: 0x04001E4E RID: 7758
	private SpringFloat springFloatPosX;

	// Token: 0x04001E4F RID: 7759
	private SpringFloat springFloatPosY;

	// Token: 0x04001E50 RID: 7760
	private SpringFloat springFloatRotation;

	// Token: 0x04001E51 RID: 7761
	private RectTransform rectTransform;

	// Token: 0x04001E52 RID: 7762
	private Vector2 initialPosition;

	// Token: 0x04001E53 RID: 7763
	private float initialScale;

	// Token: 0x04001E54 RID: 7764
	private float initialRotation;

	// Token: 0x04001E55 RID: 7765
	public bool forceMiddlePivot = true;
}
