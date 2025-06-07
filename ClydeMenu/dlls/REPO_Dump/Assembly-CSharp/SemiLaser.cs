using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x02000042 RID: 66
public class SemiLaser : MonoBehaviour
{
	// Token: 0x06000186 RID: 390 RVA: 0x0000F994 File Offset: 0x0000DB94
	private void Start()
	{
		this.enableLaser.SetActive(true);
		this.lineRenderers = Enumerable.ToList<LineRenderer>(base.GetComponentsInChildren<LineRenderer>());
		this.startPosition = base.transform.position;
		this.endPosition = base.transform.position + base.transform.forward * 10f;
		this.pointLights = Enumerable.ToList<Light>(base.GetComponentsInChildren<Light>());
		this.pointLights.RemoveAll((Light light) => light.type != LightType.Point);
		this.pointLights.RemoveAll((Light light) => light.shadows > LightShadows.None);
		this.hitMeshRenderers = Enumerable.ToList<MeshRenderer>(this.hitTransform.GetComponentsInChildren<MeshRenderer>());
		this.hitTransform.localScale = Vector3.one * 0.1f;
		this.hitTransform.gameObject.SetActive(false);
		this.hitParticles = Enumerable.ToList<ParticleSystem>(this.hitParticlesTransform.GetComponentsInChildren<ParticleSystem>());
		this.shootParticles = Enumerable.ToList<ParticleSystem>(this.shootTransform.GetComponentsInChildren<ParticleSystem>());
		this.hurtCollider = base.GetComponentInChildren<HurtCollider>();
		this.hitLight = this.hitTransform.GetComponentInChildren<Light>();
		this.hitLightOriginalIntensity = this.hitLight.intensity;
		this.graceParticles = Enumerable.ToList<ParticleSystem>(this.graceTransform.GetComponentsInChildren<ParticleSystem>());
		this.graceTransform.localScale = Vector3.one * this.beamThickness;
		this.shootTransform.localScale = Vector3.one * this.beamThickness;
		this.hitTransform.localScale = Vector3.one * this.beamHitSize * this.beamThickness;
		this.enableLaser.SetActive(false);
		this.beamThicknessOriginal = this.beamThickness;
		this.originalHitLightRange = this.hitLight.range;
		this.audioSourceTransform = this.audioSource.transform;
		this.audioSourceHitTransform = this.audioSourceHit.transform;
	}

	// Token: 0x06000187 RID: 391 RVA: 0x0000FBBC File Offset: 0x0000DDBC
	public void LaserActive(Vector3 _startPosition, Vector3 _endPosition, bool _isHitting)
	{
		if (!this.enableLaser.activeSelf)
		{
			this.enableLaser.SetActive(true);
		}
		this.startPosition = _startPosition;
		this.endPosition = _endPosition;
		if (_isHitting)
		{
			this.isHittingTimer = 0.1f;
		}
		this.isActiveTimer = 0.1f;
	}

	// Token: 0x06000188 RID: 392 RVA: 0x0000FC0C File Offset: 0x0000DE0C
	private void ActiveTimer()
	{
		if (this.isActiveTimer <= 0f && this.isActivePrev)
		{
			this.HitParticles(false);
			this.ShootParticles(false);
			this.isActivePrev = this.isActive;
			this.isActive = false;
		}
		if (this.isActiveTimer > 0f)
		{
			if (!this.isActivePrev)
			{
				this.ShootParticles(true);
				this.isActivePrev = this.isActive;
				this.isActive = true;
			}
			this.isActiveTimer -= Time.fixedDeltaTime;
		}
		if (this.isHittingTimer <= 0f)
		{
			this.isHitting = false;
		}
		if (this.isHittingTimer > 0f)
		{
			this.isHitting = true;
			this.isHittingTimer -= Time.fixedDeltaTime;
		}
	}

	// Token: 0x06000189 RID: 393 RVA: 0x0000FCCA File Offset: 0x0000DECA
	private void FixedUpdate()
	{
		this.ActiveTimer();
	}

	// Token: 0x0600018A RID: 394 RVA: 0x0000FCD4 File Offset: 0x0000DED4
	private void LaserActiveIntroOutro()
	{
		if (!this.isActive)
		{
			this.beamThickness = Mathf.Lerp(this.beamThickness, 0f, Time.deltaTime * 10f);
			if (this.hurtCollider.gameObject.activeSelf)
			{
				this.hurtCollider.gameObject.SetActive(false);
			}
			if (this.beamThickness < this.beamThicknessOriginal * 0.01f)
			{
				this.enableLaser.SetActive(false);
				this.beamThickness = 0f;
			}
			if (!this.laserEnd)
			{
				this.soundLaserEnd.Play(this.audioSourceTransform.position, 1f, 1f, 1f, 1f);
				this.soundLaserEndGlobal.Play(this.audioSourceTransform.position, 1f, 1f, 1f, 1f);
				this.laserEnd = true;
			}
			this.laserStart = false;
			return;
		}
		this.laserEnd = false;
		if (!this.laserStart)
		{
			this.soundLaserStart.Play(this.audioSourceTransform.position, 1f, 1f, 1f, 1f);
			this.soundLaserStartGlobal.Play(this.audioSourceTransform.position, 1f, 1f, 1f, 1f);
			this.laserStart = true;
		}
		this.beamThickness = Mathf.Lerp(this.beamThickness, this.beamThicknessOriginal, Time.deltaTime * 10f);
		if (this.beamThickness > this.beamThicknessOriginal * 0.95f && !this.hurtCollider.gameObject.activeSelf)
		{
			this.hurtCollider.gameObject.SetActive(true);
		}
	}

	// Token: 0x0600018B RID: 395 RVA: 0x0000FE90 File Offset: 0x0000E090
	private void LaserPositioning()
	{
		base.transform.position = this.startPosition;
		this.hitTransform.LookAt(this.startPosition);
		this.shootTransform.LookAt(this.endPosition);
		this.graceTransform.LookAt(this.endPosition);
		this.shootTransform.position = this.startPosition;
		this.hitTransform.position = this.endPosition + this.hitTransform.forward * 0.3f;
		this.audioSourceHitTransform.position = this.hitTransform.position;
		this.hitParticlesTransform.position = this.hitTransform.position;
		this.hitParticlesTransform.LookAt(this.startPosition);
		this.hurtCollider.transform.localScale = new Vector3(this.hurtColliderBeamThickness, this.hurtColliderBeamThickness, Vector3.Distance(this.startPosition, this.endPosition));
		this.hurtCollider.transform.localPosition = new Vector3(-this.hurtCollider.transform.localScale.x / 2f, -this.hurtCollider.transform.localScale.y / 2f, 0f);
		this.hurtColliderRotation.transform.LookAt(this.endPosition);
		this.laserSpotLight.transform.LookAt(this.endPosition);
		this.laserSpotLight.range = Vector3.Distance(this.startPosition, this.endPosition) * 1.5f;
		this.laserSpotLight.intensity = this.laserSpotLightOriginalIntensity * this.beamThickness;
	}

	// Token: 0x0600018C RID: 396 RVA: 0x00010044 File Offset: 0x0000E244
	private void Update()
	{
		this.soundLaserLoop.PlayLoop(this.enableLaser.activeSelf, 2f, 2f, 1f);
		this.soundLaserHitLoop.PlayLoop(this.isHitting && this.enableLaser.activeSelf, 2f, 2f, 1f);
		this.LaserEffectIsHitting();
		this.LaserActiveIntroOutro();
		if (!this.enableLaser.activeSelf)
		{
			return;
		}
		this.LaserPositioning();
		this.AudioSourcePositioning();
		this.LaserEffectGrace();
		this.LaserEffectLine();
	}

	// Token: 0x0600018D RID: 397 RVA: 0x000100D8 File Offset: 0x0000E2D8
	private void LaserEffectGrace()
	{
		if (this.graceSoundTimer > 0f)
		{
			this.graceSoundTimer -= Time.deltaTime;
		}
		if (SemiFunc.FPSImpulse15())
		{
			foreach (RaycastHit raycastHit in Physics.SphereCastAll(this.startPosition, this.hurtColliderBeamThickness, this.shootTransform.forward, Vector3.Distance(this.startPosition, this.endPosition), SemiFunc.LayerMaskGetVisionObstruct()))
			{
				foreach (ParticleSystem particleSystem in this.graceParticles)
				{
					if (raycastHit.point != Vector3.zero && this.isActive)
					{
						particleSystem.transform.position = raycastHit.point;
						particleSystem.Emit(3);
						float num = Vector3.Distance(this.graceSoundPosition, raycastHit.point);
						if (this.graceSoundTimer <= 0f || num > 1f)
						{
							this.soundLaserGrace.Play(raycastHit.point, 1f, 1f, 1f, 1f);
							this.graceSoundPosition = raycastHit.point;
							this.graceSoundTimer = 0.15f;
						}
					}
				}
			}
		}
	}

	// Token: 0x0600018E RID: 398 RVA: 0x00010250 File Offset: 0x0000E450
	private void AudioSourcePositioning()
	{
		Transform transform = AudioListenerFollow.instance.transform;
		Vector3 vector = this.endPosition - this.startPosition;
		float num = Vector3.Dot(transform.position - this.startPosition, vector) / vector.sqrMagnitude;
		num = Mathf.Clamp01(num);
		Vector3 position = this.startPosition + vector * num;
		this.audioSourceTransform.position = position;
	}

	// Token: 0x0600018F RID: 399 RVA: 0x000102C0 File Offset: 0x0000E4C0
	private void LaserEffectLine()
	{
		float d = 0.035f * this.wobbleAmount + (this.beamThicknessOriginal - this.beamThickness) * 0.02f;
		int num = Mathf.CeilToInt(Vector3.Distance(this.startPosition, this.endPosition) * 2f);
		foreach (LineRenderer lineRenderer in this.lineRenderers)
		{
			Vector3[] array = new Vector3[num];
			for (int i = 0; i < num; i++)
			{
				float t = (float)i / (float)num;
				array[i] = Vector3.Lerp(this.startPosition, this.endPosition, t);
				array[i] += Vector3.right * Mathf.Sin(Time.time * 60f + (float)i) * d;
				array[i] += Vector3.up * Mathf.Cos(Time.time * 60f + (float)i) * d;
			}
			lineRenderer.material.mainTextureOffset = new Vector2(-Time.time * 30f, 0f);
			lineRenderer.widthMultiplier = (Mathf.PingPong(Time.time * 60f, 0.4f) + 0.8f) * this.beamThickness;
			lineRenderer.positionCount = num;
			lineRenderer.SetPositions(array);
			if (this.isHitting)
			{
				lineRenderer.endWidth = 0.4f * this.beamThickness;
			}
			else
			{
				lineRenderer.endWidth = 0f;
			}
		}
		float num2 = 4f;
		float b = 4f;
		int count = this.pointLights.Count;
		float num3 = Vector3.Distance(this.startPosition, this.endPosition);
		Vector3 normalized = (this.endPosition - this.startPosition).normalized;
		int num4 = Mathf.Min(count, Mathf.CeilToInt(num3 / 2f));
		for (int j = 0; j < count; j++)
		{
			if (j < num4)
			{
				int num5 = num4 - 1;
				if (num5 <= 0)
				{
					num5 = 1;
				}
				float t2 = (float)j / (float)num5;
				Vector3 vector = Vector3.Lerp(this.startPosition, this.endPosition, t2);
				this.pointLights[j].transform.position = Vector3.Lerp(this.pointLights[j].transform.position, vector, Time.deltaTime * 20f);
				if (!this.pointLights[j].enabled)
				{
					this.pointLights[j].transform.position = vector;
					this.pointLights[j].enabled = true;
					this.pointLights[j].range = 0f;
				}
				this.pointLights[j].range = Mathf.Lerp(this.pointLights[j].range, b, Time.deltaTime * 10f) * this.beamThickness;
				this.pointLights[j].intensity = Mathf.PingPong(Time.time * 20f, 2f) + num2;
			}
			else
			{
				this.pointLights[j].range = Mathf.Lerp(this.pointLights[j].range, 0f, Time.deltaTime * 8f);
				if (this.pointLights[j].range < 0.05f)
				{
					this.pointLights[j].enabled = false;
				}
			}
		}
	}

	// Token: 0x06000190 RID: 400 RVA: 0x000106AC File Offset: 0x0000E8AC
	private void HitParticles(bool _play)
	{
		foreach (ParticleSystem particleSystem in this.hitParticles)
		{
			if (_play)
			{
				particleSystem.Play();
			}
			else
			{
				particleSystem.Stop();
			}
		}
	}

	// Token: 0x06000191 RID: 401 RVA: 0x0001070C File Offset: 0x0000E90C
	private void ShootParticles(bool _play)
	{
		foreach (ParticleSystem particleSystem in this.shootParticles)
		{
			if (_play)
			{
				particleSystem.Play();
			}
			else
			{
				particleSystem.Stop();
			}
		}
	}

	// Token: 0x06000192 RID: 402 RVA: 0x0001076C File Offset: 0x0000E96C
	private void LaserEffectIsHitting()
	{
		if (this.isHitting)
		{
			if (!this.hitTransform.gameObject.activeSelf)
			{
				this.HitParticles(true);
				this.hitTransform.gameObject.SetActive(true);
				GameDirector.instance.CameraShake.ShakeDistance(8f, 3f, 8f, this.hitTransform.position, 0.1f);
				GameDirector.instance.CameraImpact.ShakeDistance(12f, 3f, 8f, this.hitTransform.position, 0.1f);
				this.hitTransform.localScale = Vector3.zero;
				this.hitLight.intensity = 0f;
				this.soundLaserHitStart.Play(this.hitTransform.position, 1f, 1f, 1f, 1f);
				this.hitEnd = false;
			}
			GameDirector.instance.CameraShake.ShakeDistance(8f, 0f, 6f, this.hitTransform.position, 0.1f);
			this.hitTransform.localScale = Vector3.Lerp(this.hitTransform.localScale, Vector3.one, Time.deltaTime * 40f) * this.beamHitSize * this.beamThickness;
			this.hitLight.intensity = Mathf.Lerp(this.hitLight.intensity, this.hitLightOriginalIntensity, Time.deltaTime * 40f) * this.beamThickness;
			this.hitLight.intensity += Mathf.Sin(Time.time * 40f) * 0.5f * this.beamThickness;
			this.hitLight.range = this.originalHitLightRange * this.beamThickness;
			int num = 0;
			float num2 = 0.15f;
			float num3 = 60f;
			using (List<MeshRenderer>.Enumerator enumerator = this.hitMeshRenderers.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					MeshRenderer meshRenderer = enumerator.Current;
					float num4;
					if (num == 0)
					{
						num4 = 0.85f;
					}
					else
					{
						num4 = 1.55f;
					}
					Vector3 vector = new Vector3(Mathf.Sin(Time.time * num3) * num2 + 1f, Mathf.Cos(Time.time * num3) * num2 + 1f, Mathf.Sin(Time.time * num3) * num2 + 1f);
					meshRenderer.transform.localScale = new Vector3(vector.x * num4, vector.y * num4, vector.z * num4);
					meshRenderer.material.mainTextureOffset = new Vector2(Time.time * 10f, 0f);
					meshRenderer.material.mainTextureScale = new Vector2(Mathf.Sin(Time.time * 20f) * 0.4f + 1f, Mathf.Cos(Time.time * 10f) * 0.4f + 1f);
					num++;
				}
				return;
			}
		}
		if (!this.hitEnd)
		{
			this.soundLaserHitEnd.Play(this.hitTransform.position, 1f, 1f, 1f, 1f);
			this.hitEnd = true;
		}
		this.hitTransform.localScale = Vector3.Lerp(this.hitTransform.localScale, Vector3.zero, Time.deltaTime * 40f) * this.beamHitSize * this.beamThickness;
		this.hitLight.intensity = Mathf.Lerp(this.hitLight.intensity, 0f, Time.deltaTime * 40f) / this.beamThickness;
		if (this.hitTransform.localScale.x < 0.01f)
		{
			this.hitTransform.gameObject.SetActive(false);
		}
	}

	// Token: 0x04000342 RID: 834
	public float hurtColliderBeamThickness = 1f;

	// Token: 0x04000343 RID: 835
	public float beamThickness = 1f;

	// Token: 0x04000344 RID: 836
	public float beamHitSize = 1f;

	// Token: 0x04000345 RID: 837
	public float wobbleAmount = 1f;

	// Token: 0x04000346 RID: 838
	public GameObject enableLaser;

	// Token: 0x04000347 RID: 839
	public Transform hitTransform;

	// Token: 0x04000348 RID: 840
	public Transform shootTransform;

	// Token: 0x04000349 RID: 841
	public Transform graceTransform;

	// Token: 0x0400034A RID: 842
	public Light laserSpotLight;

	// Token: 0x0400034B RID: 843
	public Transform hurtColliderRotation;

	// Token: 0x0400034C RID: 844
	[FormerlySerializedAs("HitParticlesTransform")]
	public Transform hitParticlesTransform;

	// Token: 0x0400034D RID: 845
	public AudioSource audioSource;

	// Token: 0x0400034E RID: 846
	public AudioSource audioSourceHit;

	// Token: 0x0400034F RID: 847
	public Sound soundLaserStart;

	// Token: 0x04000350 RID: 848
	public Sound soundLaserStartGlobal;

	// Token: 0x04000351 RID: 849
	public Sound soundLaserEnd;

	// Token: 0x04000352 RID: 850
	public Sound soundLaserEndGlobal;

	// Token: 0x04000353 RID: 851
	public Sound soundLaserLoop;

	// Token: 0x04000354 RID: 852
	public Sound soundLaserHitStart;

	// Token: 0x04000355 RID: 853
	public Sound soundLaserHitEnd;

	// Token: 0x04000356 RID: 854
	public Sound soundLaserHitLoop;

	// Token: 0x04000357 RID: 855
	public Sound soundLaserGrace;

	// Token: 0x04000358 RID: 856
	internal SemiLaser.LaserState state;

	// Token: 0x04000359 RID: 857
	private List<LineRenderer> lineRenderers = new List<LineRenderer>();

	// Token: 0x0400035A RID: 858
	private List<Light> pointLights = new List<Light>();

	// Token: 0x0400035B RID: 859
	private Vector3 startPosition;

	// Token: 0x0400035C RID: 860
	private Vector3 endPosition;

	// Token: 0x0400035D RID: 861
	private List<MeshRenderer> hitMeshRenderers = new List<MeshRenderer>();

	// Token: 0x0400035E RID: 862
	private bool isHitting;

	// Token: 0x0400035F RID: 863
	private List<ParticleSystem> hitParticles = new List<ParticleSystem>();

	// Token: 0x04000360 RID: 864
	private List<ParticleSystem> shootParticles = new List<ParticleSystem>();

	// Token: 0x04000361 RID: 865
	private List<ParticleSystem> graceParticles = new List<ParticleSystem>();

	// Token: 0x04000362 RID: 866
	internal HurtCollider hurtCollider;

	// Token: 0x04000363 RID: 867
	private Light hitLight;

	// Token: 0x04000364 RID: 868
	private float hitLightOriginalIntensity;

	// Token: 0x04000365 RID: 869
	private bool isActivePrev;

	// Token: 0x04000366 RID: 870
	private bool isActive;

	// Token: 0x04000367 RID: 871
	private float isActiveTimer;

	// Token: 0x04000368 RID: 872
	private float beamThicknessOriginal;

	// Token: 0x04000369 RID: 873
	private float originalHitLightRange;

	// Token: 0x0400036A RID: 874
	private float laserSpotLightOriginalIntensity;

	// Token: 0x0400036B RID: 875
	private bool hitEnd;

	// Token: 0x0400036C RID: 876
	private bool laserEnd;

	// Token: 0x0400036D RID: 877
	private bool laserStart;

	// Token: 0x0400036E RID: 878
	private Transform audioSourceTransform;

	// Token: 0x0400036F RID: 879
	private Transform audioSourceHitTransform;

	// Token: 0x04000370 RID: 880
	private float graceSoundTimer;

	// Token: 0x04000371 RID: 881
	private Vector3 graceSoundPosition;

	// Token: 0x04000372 RID: 882
	private float isHittingTimer;

	// Token: 0x02000308 RID: 776
	public enum LaserState
	{
		// Token: 0x0400283F RID: 10303
		Intro,
		// Token: 0x04002840 RID: 10304
		Active,
		// Token: 0x04002841 RID: 10305
		Outro
	}
}
