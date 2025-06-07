using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020001AB RID: 427
public class PhysObjectParticles : MonoBehaviour
{
	// Token: 0x06000EAA RID: 3754 RVA: 0x00084FBC File Offset: 0x000831BC
	private void Start()
	{
		this.physGrabObject = base.GetComponent<PhysGrabObject>();
	}

	// Token: 0x06000EAB RID: 3755 RVA: 0x00084FCC File Offset: 0x000831CC
	public void DestroyParticles()
	{
		if (!SemiFunc.RunIsTutorial())
		{
			base.StartCoroutine(this.DestroyParticlesAfterTime(4f));
		}
		foreach (Transform transform in this.colliderTransforms)
		{
			Vector3 vector = transform.localScale;
			Vector3 eulerAngles = transform.rotation.eulerAngles;
			Transform colliderTransform = transform;
			int num = (int)(vector.x * 100f * (vector.y * 100f) * (vector.z * 100f) / 1000f);
			num = (int)((float)Mathf.Clamp(num, 10, 150) * this.multiplier);
			float num2 = vector.x * 100f * (vector.y * 100f) * (vector.z * 100f) / 30000f;
			if (transform.GetComponent<SphereCollider>())
			{
				num2 *= 0.55f;
				vector *= 0.4f;
			}
			num2 = Mathf.Clamp(num2, 0.3f, 2f);
			this.SpawnParticles(num, num2 * this.multiplier, vector * this.multiplier, eulerAngles, colliderTransform);
		}
	}

	// Token: 0x06000EAC RID: 3756 RVA: 0x0008511C File Offset: 0x0008331C
	private IEnumerator DestroyParticlesAfterTime(float time)
	{
		yield return new WaitForSeconds(time);
		Object.Destroy(base.gameObject);
		yield break;
	}

	// Token: 0x06000EAD RID: 3757 RVA: 0x00085134 File Offset: 0x00083334
	public void ImpactSmoke(int amount, Vector3 position, float size)
	{
		size = Mathf.Clamp(size, 0.7f, 1.5f);
		Vector3 localPosition = base.transform.InverseTransformPoint(position);
		this.particleSystemSmoke.transform.localPosition = localPosition;
		ParticleSystem.MainModule main = this.particleSystemSmoke.main;
		ParticleSystem.ShapeModule shape = this.particleSystemSmoke.shape;
		float startSizeMultiplier = main.startSizeMultiplier;
		shape.scale = new Vector3(0.2f, 0.2f, 0.2f);
		float max = Mathf.Clamp(size / 4f, 0f, 2f);
		main.startSpeed = new ParticleSystem.MinMaxCurve(0f, max);
		main.startSizeXMultiplier *= size;
		main.startSizeYMultiplier *= size;
		main.startSizeZMultiplier *= size;
		this.particleSystemSmoke.Emit(amount);
		main.startSizeXMultiplier = startSizeMultiplier;
		main.startSizeYMultiplier = startSizeMultiplier;
		main.startSizeZMultiplier = startSizeMultiplier;
	}

	// Token: 0x06000EAE RID: 3758 RVA: 0x00085228 File Offset: 0x00083428
	private void SpawnParticles(int bitCount, float size, Vector3 colliderScale, Vector3 colliderRotation, Transform colliderTransform)
	{
		Vector3.left * 5f;
		this.particleSystemBits.transform.position = colliderTransform.position;
		this.particleSystemBitsSmall.transform.position = colliderTransform.position;
		this.particleSystemSmoke.transform.position = colliderTransform.position;
		ParticleSystem.ShapeModule shape = this.particleSystemBits.shape;
		ParticleSystem.MainModule main = this.particleSystemBits.main;
		main.startSizeXMultiplier *= size;
		main.startSizeYMultiplier *= size;
		main.startSizeZMultiplier *= size;
		shape.scale = colliderScale;
		shape.rotation = colliderRotation;
		main = this.particleSystemBitsSmall.main;
		main.startSizeXMultiplier *= size;
		main.startSizeYMultiplier *= size;
		main.startSizeZMultiplier *= size;
		shape = this.particleSystemBitsSmall.shape;
		shape.scale = colliderScale;
		shape.rotation = colliderRotation;
		main = this.particleSystemSmoke.main;
		shape = this.particleSystemSmoke.shape;
		float startSizeMultiplier = main.startSizeMultiplier;
		main.startSizeXMultiplier *= Mathf.Clamp(size, 0.5f, 1.5f);
		main.startSizeYMultiplier *= Mathf.Clamp(size, 0.5f, 1.5f);
		main.startSizeZMultiplier *= Mathf.Clamp(size, 0.5f, 1.5f);
		float max = Mathf.Clamp(size, 0.5f, 2f);
		main.startSpeed = new ParticleSystem.MinMaxCurve(0f, max);
		shape.scale = colliderScale;
		shape.rotation = colliderRotation;
		this.particleSystemBits.Emit(bitCount);
		this.particleSystemBitsSmall.Emit(bitCount / 3);
		this.particleSystemSmoke.Emit(bitCount);
		this.particlesSpawned = true;
		main = this.particleSystemBits.main;
		main.startSizeXMultiplier /= size;
		main.startSizeYMultiplier /= size;
		main.startSizeZMultiplier /= size;
		main = this.particleSystemBitsSmall.main;
		main.startSizeXMultiplier /= size;
		main.startSizeYMultiplier /= size;
		main.startSizeZMultiplier /= size;
		main = this.particleSystemSmoke.main;
		main.startSizeXMultiplier = startSizeMultiplier;
		main.startSizeYMultiplier = startSizeMultiplier;
		main.startSizeZMultiplier = startSizeMultiplier;
	}

	// Token: 0x06000EAF RID: 3759 RVA: 0x000854A8 File Offset: 0x000836A8
	private void LateUpdate()
	{
		if (!this.particlesSpawned)
		{
			return;
		}
		int maxParticles = this.particleSystemBits.main.maxParticles;
		if (this.particles == null || this.particles.Length < maxParticles)
		{
			this.particles = new ParticleSystem.Particle[maxParticles];
		}
		int num = this.particleSystemBits.GetParticles(this.particles);
		for (int i = 0; i < num; i++)
		{
			float time = (float)i / (float)num;
			Color c = this.gradient.Evaluate(time);
			this.particles[i].startColor = c;
		}
		this.particleSystemBits.SetParticles(this.particles, num);
		maxParticles = this.particleSystemBitsSmall.main.maxParticles;
		if (this.particles == null || this.particles.Length < maxParticles)
		{
			this.particles = new ParticleSystem.Particle[maxParticles];
		}
		num = this.particleSystemBitsSmall.GetParticles(this.particles);
		for (int j = 0; j < num; j++)
		{
			float time2 = (float)j / (float)num;
			Color c2 = this.gradient.Evaluate(time2);
			this.particles[j].startColor = c2;
		}
		this.particleSystemBitsSmall.SetParticles(this.particles, num);
		this.particlesSpawned = false;
	}

	// Token: 0x0400183F RID: 6207
	public Gradient gradient;

	// Token: 0x04001840 RID: 6208
	public ParticleSystem particleSystemBits;

	// Token: 0x04001841 RID: 6209
	public ParticleSystem particleSystemBitsSmall;

	// Token: 0x04001842 RID: 6210
	public ParticleSystem particleSystemSmoke;

	// Token: 0x04001843 RID: 6211
	private ParticleSystem.Particle[] particles;

	// Token: 0x04001844 RID: 6212
	private bool particlesSpawned;

	// Token: 0x04001845 RID: 6213
	private PhysGrabObject physGrabObject;

	// Token: 0x04001846 RID: 6214
	internal float multiplier = 1f;

	// Token: 0x04001847 RID: 6215
	public List<Transform> colliderTransforms = new List<Transform>();
}
