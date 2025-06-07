using System;
using UnityEngine;

// Token: 0x02000243 RID: 579
public class GizmoCapsule : MonoBehaviour
{
	// Token: 0x060012ED RID: 4845 RVA: 0x000A9678 File Offset: 0x000A7878
	private void OnDrawGizmos()
	{
		if (this.capsuleMesh == null)
		{
			this.capsuleMesh = this.CreateCapsuleMesh();
		}
		this.gizmoColor.a = 0.4f * this.gizmoTransparency;
		Gizmos.color = this.gizmoColor;
		Vector3 localScale = base.transform.localScale;
		Vector3 position = base.transform.position;
		Quaternion rotation = base.transform.rotation;
		this.gizmoColor.a = 0.2f * this.gizmoTransparency;
		Gizmos.color = this.gizmoColor;
		Gizmos.DrawMesh(this.capsuleMesh, position, rotation, localScale);
		float num = Mathf.Min(localScale.x, localScale.z) * 0.5f;
		float num2 = localScale.y - num * 2f;
		this.gizmoColor.a = 0.4f * this.gizmoTransparency;
		Gizmos.color = this.gizmoColor;
		Gizmos.DrawWireSphere(position + rotation * Vector3.up * (num2 + num), num);
		Gizmos.DrawWireSphere(position + rotation * Vector3.down * (num2 + num), num);
		Gizmos.DrawLine(position + rotation * Vector3.up * (num2 + num) + rotation * Vector3.right * num, position + rotation * Vector3.down * (num2 + num) + rotation * Vector3.right * num);
		Gizmos.DrawLine(position + rotation * Vector3.up * (num2 + num) + rotation * Vector3.left * num, position + rotation * Vector3.down * (num2 + num) + rotation * Vector3.left * num);
		Gizmos.DrawLine(position + rotation * Vector3.up * (num2 + num) + rotation * Vector3.forward * num, position + rotation * Vector3.down * (num2 + num) + rotation * Vector3.forward * num);
		Gizmos.DrawLine(position + rotation * Vector3.up * (num2 + num) + rotation * Vector3.back * num, position + rotation * Vector3.down * (num2 + num) + rotation * Vector3.back * num);
	}

	// Token: 0x060012EE RID: 4846 RVA: 0x000A9934 File Offset: 0x000A7B34
	private Mesh CreateCapsuleMesh()
	{
		GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Capsule);
		Mesh sharedMesh = gameObject.GetComponent<MeshFilter>().sharedMesh;
		Object.DestroyImmediate(gameObject);
		return sharedMesh;
	}

	// Token: 0x0400202D RID: 8237
	public Color gizmoColor = Color.yellow;

	// Token: 0x0400202E RID: 8238
	private Mesh capsuleMesh;

	// Token: 0x0400202F RID: 8239
	[Range(0.2f, 1f)]
	public float gizmoTransparency = 1f;
}
