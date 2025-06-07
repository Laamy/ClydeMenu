using System;
using System.Collections;
using UnityEngine;

// Token: 0x020000BB RID: 187
public class RandomRotateAndScale : MonoBehaviour
{
	// Token: 0x060006F8 RID: 1784 RVA: 0x0004233C File Offset: 0x0004053C
	private void Start()
	{
		this.RotateObjectAndChildren();
		base.StartCoroutine(this.ScaleAnimation(this.spawnScaleCurve, this.spawnAnimationLength, delegate
		{
			base.StartCoroutine(this.WaitAndDespawn(this.durationBeforeDespawn));
		}));
		base.transform.position += Vector3.up * 0.02f;
		float maxDistance = 0.1f;
		RaycastHit raycastHit;
		if (Physics.Raycast(new Ray(base.transform.position, -Vector3.up), out raycastHit, maxDistance))
		{
			base.transform.position = raycastHit.point + Vector3.up * 0.0001f;
		}
	}

	// Token: 0x060006F9 RID: 1785 RVA: 0x000423EC File Offset: 0x000405EC
	private void RotateObjectAndChildren()
	{
		Vector3 localEulerAngles = base.transform.localEulerAngles;
		base.transform.localRotation = Quaternion.Euler(localEulerAngles.x + 90f, localEulerAngles.y, (float)Random.Range(0, 360));
		foreach (object obj in base.transform)
		{
			Transform transform = (Transform)obj;
			Vector3 localEulerAngles2 = transform.localEulerAngles;
			transform.localRotation = Quaternion.Euler(localEulerAngles2.x, localEulerAngles2.y, (float)Random.Range(0, 360));
		}
	}

	// Token: 0x060006FA RID: 1786 RVA: 0x000424A0 File Offset: 0x000406A0
	private IEnumerator ScaleAnimation(AnimationCurve curve, float animationLength, Action onComplete)
	{
		float elapsedTime = 0f;
		while (elapsedTime < animationLength)
		{
			elapsedTime += Time.deltaTime;
			float time = elapsedTime / animationLength;
			float num = curve.Evaluate(time) * this.scaleMultiplier;
			base.transform.localScale = new Vector3(num, num, num);
			yield return null;
		}
		if (onComplete != null)
		{
			onComplete.Invoke();
		}
		yield break;
	}

	// Token: 0x060006FB RID: 1787 RVA: 0x000424C4 File Offset: 0x000406C4
	private IEnumerator WaitAndDespawn(float duration)
	{
		yield return new WaitForSeconds(duration);
		base.StartCoroutine(this.ScaleAnimation(this.despawnScaleCurve, this.despawnAnimationLength, delegate
		{
			Object.Destroy(base.gameObject);
		}));
		yield break;
	}

	// Token: 0x04000BCA RID: 3018
	[Space]
	[Header("Spawn")]
	public AnimationCurve spawnScaleCurve;

	// Token: 0x04000BCB RID: 3019
	public float spawnAnimationLength = 1f;

	// Token: 0x04000BCC RID: 3020
	[Space]
	[Header("Time before despawn")]
	public float durationBeforeDespawn = 5f;

	// Token: 0x04000BCD RID: 3021
	[Space]
	[Header("Despawn")]
	public AnimationCurve despawnScaleCurve;

	// Token: 0x04000BCE RID: 3022
	public float despawnAnimationLength = 1f;

	// Token: 0x04000BCF RID: 3023
	[Space]
	[Header("Scale")]
	public float scaleMultiplier = 1f;
}
