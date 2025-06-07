using System;
using UnityEngine;

// Token: 0x020000BC RID: 188
[RequireComponent(typeof(Transform))]
public class Roach : MonoBehaviour
{
	// Token: 0x060006FF RID: 1791 RVA: 0x00042530 File Offset: 0x00040730
	private void Start()
	{
		this.origin = base.transform.position;
		this.roachSpeedTarget = Random.Range(this.minRoachSpeed, this.maxRoachSpeed);
		this.targetPosition = this.GetOrbitPoint(this.angle);
		base.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
		base.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
		base.transform.rotation = Quaternion.identity;
	}

	// Token: 0x06000700 RID: 1792 RVA: 0x000425C8 File Offset: 0x000407C8
	private void Update()
	{
		this.currentOrbitDistance = Mathf.Lerp(this.minOrbitDistance, this.maxOrbitDistance, (Mathf.Sin(Time.time * this.orbitWiggleFrequency) + 1f) * 0.5f);
		this.currentOrbitSpeed = Mathf.Lerp(this.minOrbitSpeed, this.maxOrbitSpeed, (Mathf.Sin(Time.time * this.speedWiggleFrequency) + 1f) * 0.5f);
		this.angle += Time.deltaTime * this.currentOrbitSpeed;
		Vector3 orbitPoint = this.GetOrbitPoint(this.angle);
		if (Vector3.Distance(base.transform.position, this.targetPosition) < 0.1f)
		{
			Vector3 normalized = (orbitPoint - this.targetPosition).normalized;
			this.targetPosition = orbitPoint + normalized * this.overshootMultiplier;
		}
		this.currentRoachSpeed = Mathf.Lerp(this.currentRoachSpeed, this.roachSpeedTarget, Time.deltaTime * this.roachSpeedFluctuationFrequency);
		if (Mathf.Abs(this.currentRoachSpeed - this.roachSpeedTarget) < 0.1f)
		{
			this.roachSpeedTarget = Random.Range(this.minRoachSpeed, this.maxRoachSpeed);
		}
		Vector3 a = ((this.targetPosition - base.transform.position).normalized * this.currentRoachSpeed - this.velocity) * this.turnMultiplier;
		this.velocity += a * Time.deltaTime;
		base.transform.position += this.velocity * Time.deltaTime;
		if (this.velocity != Vector3.zero)
		{
			base.transform.rotation = Quaternion.LookRotation(this.velocity, Vector3.up);
		}
	}

	// Token: 0x06000701 RID: 1793 RVA: 0x000427AF File Offset: 0x000409AF
	private Vector3 GetOrbitPoint(float angle)
	{
		return this.origin + new Vector3(Mathf.Sin(angle) * this.currentOrbitDistance, 0f, Mathf.Cos(angle) * this.currentOrbitDistance);
	}

	// Token: 0x04000BD0 RID: 3024
	[Space]
	[Header("Orbit Parameters")]
	public float minOrbitDistance = 1f;

	// Token: 0x04000BD1 RID: 3025
	public float maxOrbitDistance = 5f;

	// Token: 0x04000BD2 RID: 3026
	public float minOrbitSpeed = 1f;

	// Token: 0x04000BD3 RID: 3027
	public float maxOrbitSpeed = 5f;

	// Token: 0x04000BD4 RID: 3028
	public float orbitWiggleFrequency = 1f;

	// Token: 0x04000BD5 RID: 3029
	public float speedWiggleFrequency = 1f;

	// Token: 0x04000BD6 RID: 3030
	[Space]
	[Header("Roach Parameters")]
	public float minRoachSpeed = 1f;

	// Token: 0x04000BD7 RID: 3031
	public float maxRoachSpeed = 3f;

	// Token: 0x04000BD8 RID: 3032
	public float roachSpeedFluctuationFrequency = 0.5f;

	// Token: 0x04000BD9 RID: 3033
	public float overshootMultiplier = 1.5f;

	// Token: 0x04000BDA RID: 3034
	public float turnMultiplier = 0.5f;

	// Token: 0x04000BDB RID: 3035
	[Space]
	[Header("Roach Smash")]
	public GameObject roachSmashPrefab;

	// Token: 0x04000BDC RID: 3036
	private Vector3 origin;

	// Token: 0x04000BDD RID: 3037
	private float currentOrbitDistance;

	// Token: 0x04000BDE RID: 3038
	private float currentOrbitSpeed;

	// Token: 0x04000BDF RID: 3039
	private float angle;

	// Token: 0x04000BE0 RID: 3040
	private float roachSpeedTarget;

	// Token: 0x04000BE1 RID: 3041
	private float currentRoachSpeed;

	// Token: 0x04000BE2 RID: 3042
	private Vector3 targetPosition;

	// Token: 0x04000BE3 RID: 3043
	private Vector3 velocity;
}
