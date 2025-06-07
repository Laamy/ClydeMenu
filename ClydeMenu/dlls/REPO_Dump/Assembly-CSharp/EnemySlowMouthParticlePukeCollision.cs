using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000078 RID: 120
public class EnemySlowMouthParticlePukeCollision : MonoBehaviour
{
	// Token: 0x06000488 RID: 1160 RVA: 0x0002D836 File Offset: 0x0002BA36
	private void Start()
	{
		this.pukeParticles = base.GetComponent<ParticleSystem>();
		this.parentTransform = base.transform.parent;
		this.startPosition = base.transform.localPosition;
	}

	// Token: 0x06000489 RID: 1161 RVA: 0x0002D868 File Offset: 0x0002BA68
	private void Update()
	{
		if (this.pukeParticles.isPlaying)
		{
			if (!this.pukeLight.enabled)
			{
				this.pukeLight.enabled = true;
				this.pukeLight.intensity = 0f;
			}
			this.pukeLight.intensity = Mathf.Lerp(this.pukeLight.intensity, 0.6f, Time.deltaTime * 5f);
			this.pukeLight.intensity += Mathf.Sin(Time.time * 40f) * 0.07f;
		}
		else if (this.pukeLight.enabled)
		{
			this.pukeLight.intensity = Mathf.Lerp(this.pukeLight.intensity, 0f, Time.deltaTime * 1f);
			this.pukeLight.intensity += Mathf.Sin(Time.time * 30f) * 0.01f;
			if (this.pukeLight.intensity < 0.01f)
			{
				this.pukeLight.enabled = false;
			}
		}
		if (this.hurtColliderTimer > 0f)
		{
			this.hurtColliderTimer -= Time.deltaTime;
			if (this.hurtColliderTimer <= 0f)
			{
				this.hurtCollider.SetActive(false);
			}
		}
		if (SemiFunc.FPSImpulse15())
		{
			base.transform.localPosition = this.startPosition;
			float num = Vector3.Distance(this.parentTransform.position, base.transform.position);
			RaycastHit raycastHit;
			if (Physics.Raycast(this.parentTransform.position, num * this.parentTransform.forward, out raycastHit, num, SemiFunc.LayerMaskGetVisionObstruct()))
			{
				Vector3 point = raycastHit.point;
				if (Vector3.Distance(this.parentTransform.position, point) < num)
				{
					Vector3 vector = this.parentTransform.InverseTransformPoint(point);
					base.transform.localPosition = new Vector3(0f, 0f, vector.z);
				}
			}
		}
	}

	// Token: 0x0600048A RID: 1162 RVA: 0x0002DA68 File Offset: 0x0002BC68
	private void ActivateHurtCollider(Vector3 _direction, Vector3 _position)
	{
		this.pukeSmokeParticles.transform.position = _position;
		this.pukeSmokeParticles.transform.rotation = Quaternion.LookRotation(_direction);
		this.pukeSmokeParticles.Emit(1);
		this.hurtCollider.SetActive(true);
		this.pukeBubbleParticles.transform.position = _position;
		this.pukeBubbleParticles.transform.rotation = Quaternion.LookRotation(_direction);
		this.pukeBubbleParticles.Emit(2);
		this.pukeSplashParticles.transform.position = _position;
		this.pukeSplashParticles.Emit(3);
		this.hurtCollider.transform.rotation = Quaternion.LookRotation(_direction);
		this.hurtCollider.transform.position = _position;
		this.hurtColliderTimer = 0.2f;
		_direction.y += 180f;
		this.pukeSplashParticles.transform.rotation = Quaternion.LookRotation(_direction);
	}

	// Token: 0x0600048B RID: 1163 RVA: 0x0002DB5C File Offset: 0x0002BD5C
	private void OnParticleCollision(GameObject other)
	{
		List<ParticleCollisionEvent> list = new List<ParticleCollisionEvent>();
		int collisionEvents = this.pukeParticles.GetCollisionEvents(other, list);
		for (int i = 0; i < collisionEvents; i++)
		{
			ParticleCollisionEvent particleCollisionEvent = list[i];
			Vector3 intersection = particleCollisionEvent.intersection;
			Vector3 velocity = particleCollisionEvent.velocity;
			Vector3 normalized = velocity.normalized;
			if (velocity.magnitude > 3f)
			{
				this.ActivateHurtCollider(normalized, intersection);
			}
		}
	}

	// Token: 0x04000771 RID: 1905
	public Light pukeLight;

	// Token: 0x04000772 RID: 1906
	private ParticleSystem pukeParticles;

	// Token: 0x04000773 RID: 1907
	public GameObject hurtCollider;

	// Token: 0x04000774 RID: 1908
	private float hurtColliderTimer;

	// Token: 0x04000775 RID: 1909
	private Transform parentTransform;

	// Token: 0x04000776 RID: 1910
	private Vector3 startPosition;

	// Token: 0x04000777 RID: 1911
	public ParticleSystem pukeBubbleParticles;

	// Token: 0x04000778 RID: 1912
	public ParticleSystem pukeSplashParticles;

	// Token: 0x04000779 RID: 1913
	public ParticleSystem pukeSmokeParticles;
}
