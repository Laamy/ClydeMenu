using System;
using UnityEngine;

// Token: 0x0200007B RID: 123
public class SpringTentacle : MonoBehaviour
{
	// Token: 0x060004A4 RID: 1188 RVA: 0x0002E449 File Offset: 0x0002C649
	private void Start()
	{
		this.offsetX = Random.Range(0f, 100f);
		this.offsetY = Random.Range(0f, 100f);
	}

	// Token: 0x060004A5 RID: 1189 RVA: 0x0002E478 File Offset: 0x0002C678
	private void Update()
	{
		this.springStartTarget.transform.localRotation = Quaternion.Euler(Mathf.Sin(Time.time * 5f + this.offsetX) * 5f, Mathf.Sin(Time.time * 5f + this.offsetY) * 10f, 0f);
		this.springStartSource.rotation = SemiFunc.SpringQuaternionGet(this.springStart, this.springStartTarget.transform.rotation, -1f);
		this.springMidSource.rotation = SemiFunc.SpringQuaternionGet(this.springMid, this.springMidTarget.transform.rotation, -1f);
		this.springEndSource.rotation = SemiFunc.SpringQuaternionGet(this.springEnd, this.springEndTarget.transform.rotation, -1f);
	}

	// Token: 0x04000794 RID: 1940
	public SpringQuaternion springStart;

	// Token: 0x04000795 RID: 1941
	public Transform springStartTarget;

	// Token: 0x04000796 RID: 1942
	public Transform springStartSource;

	// Token: 0x04000797 RID: 1943
	[Space]
	public SpringQuaternion springMid;

	// Token: 0x04000798 RID: 1944
	public Transform springMidTarget;

	// Token: 0x04000799 RID: 1945
	public Transform springMidSource;

	// Token: 0x0400079A RID: 1946
	[Space]
	public SpringQuaternion springEnd;

	// Token: 0x0400079B RID: 1947
	public Transform springEndTarget;

	// Token: 0x0400079C RID: 1948
	public Transform springEndSource;

	// Token: 0x0400079D RID: 1949
	private float offsetX;

	// Token: 0x0400079E RID: 1950
	private float offsetY;
}
