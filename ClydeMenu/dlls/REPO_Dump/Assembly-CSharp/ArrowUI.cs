using System;
using UnityEngine;

// Token: 0x02000270 RID: 624
public class ArrowUI : MonoBehaviour
{
	// Token: 0x060013C9 RID: 5065 RVA: 0x000AF80D File Offset: 0x000ADA0D
	private void Awake()
	{
		ArrowUI.instance = this;
		this.arrowMesh.enabled = false;
		this.mainCamera = Camera.main;
	}

	// Token: 0x060013CA RID: 5066 RVA: 0x000AF82C File Offset: 0x000ADA2C
	public void ArrowShow(Vector3 startPos, Vector3 endPos, float rotation)
	{
		if (this.endPosition != endPos)
		{
			this.arrowCurveMoveEval = 0f;
			base.transform.localPosition = startPos;
		}
		startPos.z = 0f;
		endPos.z = 0f;
		this.targetWorldPos = false;
		this.startPosition = startPos;
		this.endPosition = endPos;
		this.endRotation = rotation;
		this.showArrowTimer = 0.2f;
	}

	// Token: 0x060013CB RID: 5067 RVA: 0x000AF8A0 File Offset: 0x000ADAA0
	public void ArrowShowWorldPos(Vector3 startPos, Vector3 endPos, float rotation)
	{
		if (this.endPosition != endPos)
		{
			this.arrowCurveMoveEval = 0f;
			base.transform.position = startPos;
		}
		this.targetWorldPos = true;
		this.startPosition = startPos;
		this.endPosition = endPos;
		this.endRotation = rotation;
		this.showArrowTimer = 0.2f;
	}

	// Token: 0x060013CC RID: 5068 RVA: 0x000AF8FC File Offset: 0x000ADAFC
	private void Update()
	{
		if (this.targetWorldPos)
		{
			this.endPosition = this.mainCamera.WorldToScreenPoint(this.endPosition).normalized;
			this.endPosition.z = 0f;
		}
		this.bopEval += Time.deltaTime;
		this.bopEval = Mathf.Clamp01(this.bopEval);
		float num = this.arrowCurveBop.Evaluate(this.bopEval);
		this.arrowMesh.transform.localPosition = new Vector3(-51f + -30f * num, 0f, 0f);
		if (this.bopEval >= 1f)
		{
			this.bopEval = 0f;
		}
		if (this.showArrowTimer > 0f)
		{
			this.arrowMesh.enabled = true;
			this.endShow = false;
			this.showArrowTimer -= Time.deltaTime;
			this.arrowCurveMoveEval += Time.deltaTime;
			this.arrowCurveMoveEval = Mathf.Clamp01(this.arrowCurveMoveEval);
			float t = this.arrowCurveMove.Evaluate(this.arrowCurveMoveEval);
			base.transform.localPosition = Vector3.LerpUnclamped(this.startPosition, this.endPosition, t);
			base.transform.localRotation = Quaternion.Euler(0f, 0f, Mathf.LerpUnclamped(90f, this.endRotation, t));
			float num2 = this.arrowCurveMove.Evaluate(this.arrowCurveMoveEval * 2f);
			base.transform.localScale = new Vector3(num2, num2, num2);
			return;
		}
		if (!this.endShow)
		{
			this.arrowCurveMoveEval = 0f;
			this.endShow = true;
		}
		if (this.arrowCurveMoveEval >= 1f)
		{
			this.arrowMesh.enabled = false;
			this.arrowCurveMoveEval = 1f;
			return;
		}
		this.arrowCurveMoveEval += Time.deltaTime * 4f;
		float num3 = this.arrowCurveMove.Evaluate(this.arrowCurveMoveEval);
		base.transform.localScale = new Vector3(1f - num3, 1f - num3, 1f - num3);
		this.startPosition = Vector3.zero;
		this.endPosition = Vector3.one;
	}

	// Token: 0x040021F9 RID: 8697
	public static ArrowUI instance;

	// Token: 0x040021FA RID: 8698
	public AnimationCurve arrowCurveMove;

	// Token: 0x040021FB RID: 8699
	private float arrowCurveMoveEval;

	// Token: 0x040021FC RID: 8700
	public AnimationCurve arrowCurveBop;

	// Token: 0x040021FD RID: 8701
	private float showArrowTimer;

	// Token: 0x040021FE RID: 8702
	private Vector3 startPosition;

	// Token: 0x040021FF RID: 8703
	private Vector3 endPosition;

	// Token: 0x04002200 RID: 8704
	private float endRotation;

	// Token: 0x04002201 RID: 8705
	private bool endShow;

	// Token: 0x04002202 RID: 8706
	public MeshRenderer arrowMesh;

	// Token: 0x04002203 RID: 8707
	private float bopEval;

	// Token: 0x04002204 RID: 8708
	private bool targetWorldPos;

	// Token: 0x04002205 RID: 8709
	private Camera mainCamera;
}
