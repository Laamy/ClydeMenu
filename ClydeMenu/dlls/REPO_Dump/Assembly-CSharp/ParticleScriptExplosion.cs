using System;
using UnityEngine;

// Token: 0x020001E6 RID: 486
public class ParticleScriptExplosion : MonoBehaviour
{
	// Token: 0x06001080 RID: 4224 RVA: 0x00098978 File Offset: 0x00096B78
	private void Start()
	{
		this.explosionPrefab = Resources.Load<GameObject>("Effects/Part Prefab Explosion");
	}

	// Token: 0x06001081 RID: 4225 RVA: 0x0009898C File Offset: 0x00096B8C
	public void PlayExplosionSoundSmall(Vector3 _position)
	{
		this.explosionPreset.explosionSoundSmall.Play(_position, 1f, 1f, 1f, 1f);
		this.explosionPreset.explosionSoundSmallGlobal.Play(_position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x06001082 RID: 4226 RVA: 0x000989E8 File Offset: 0x00096BE8
	public void PlayExplosionSoundMedium(Vector3 _position)
	{
		this.explosionPreset.explosionSoundMedium.Play(_position, 1f, 1f, 1f, 1f);
		this.explosionPreset.explosionSoundMediumGlobal.Play(_position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x06001083 RID: 4227 RVA: 0x00098A44 File Offset: 0x00096C44
	public void PlayExplosionSoundBig(Vector3 _position)
	{
		this.explosionPreset.explosionSoundBig.Play(_position, 1f, 1f, 1f, 1f);
		this.explosionPreset.explosionSoundBigGlobal.Play(_position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x06001084 RID: 4228 RVA: 0x00098AA0 File Offset: 0x00096CA0
	public ParticlePrefabExplosion Spawn(Vector3 position, float size, int damage, int enemyDamage, float forceMulti = 1f, bool onlyParticleEffect = false, bool disableSound = false, float shakeMultiplier = 1f)
	{
		if (size < 0.25f)
		{
			if (!disableSound)
			{
				this.explosionPreset.explosionSoundSmall.Play(position, 1f, 1f, 1f, 1f);
				this.explosionPreset.explosionSoundSmallGlobal.Play(position, 1f, 1f, 1f, 1f);
			}
			if (shakeMultiplier != 0f)
			{
				GameDirector.instance.CameraImpact.ShakeDistance(3f * shakeMultiplier, 3f, 6f, base.transform.position, 0.2f);
				GameDirector.instance.CameraShake.ShakeDistance(3f * shakeMultiplier, 3f, 6f, base.transform.position, 0.5f);
			}
		}
		else if (size < 0.5f)
		{
			if (!disableSound)
			{
				this.explosionPreset.explosionSoundMedium.Play(position, 1f, 1f, 1f, 1f);
				this.explosionPreset.explosionSoundMediumGlobal.Play(position, 1f, 1f, 1f, 1f);
			}
			if (shakeMultiplier != 0f)
			{
				GameDirector.instance.CameraImpact.ShakeDistance(5f * shakeMultiplier, 4f, 8f, base.transform.position, 0.2f);
				GameDirector.instance.CameraShake.ShakeDistance(5f * shakeMultiplier, 4f, 8f, base.transform.position, 0.5f);
			}
		}
		else
		{
			if (!disableSound)
			{
				this.explosionPreset.explosionSoundBig.Play(position, 1f, 1f, 1f, 1f);
				this.explosionPreset.explosionSoundBigGlobal.Play(position, 1f, 1f, 1f, 1f);
			}
			if (shakeMultiplier != 0f)
			{
				GameDirector.instance.CameraImpact.ShakeDistance(10f * shakeMultiplier, 6f, 12f, base.transform.position, 0.2f);
				GameDirector.instance.CameraShake.ShakeDistance(5f * shakeMultiplier, 6f, 12f, base.transform.position, 0.5f);
			}
		}
		ParticlePrefabExplosion component = Object.Instantiate<GameObject>(this.explosionPrefab, position, Quaternion.identity).GetComponent<ParticlePrefabExplosion>();
		component.forceMultiplier = this.explosionPreset.explosionForceMultiplier * forceMulti;
		component.explosionSize = size;
		component.explosionDamage = damage;
		component.explosionDamageEnemy = enemyDamage;
		component.lightColorOverTime = this.explosionPreset.lightColor;
		component.particleFire.colorOverLifetime.color = this.explosionPreset.explosionColors;
		component.particleSmoke.colorOverLifetime.color = this.explosionPreset.smokeColors;
		component.particleFire.Play();
		component.particleSmoke.Play();
		component.light.enabled = true;
		return component;
	}

	// Token: 0x04001C56 RID: 7254
	public ExplosionPreset explosionPreset;

	// Token: 0x04001C57 RID: 7255
	private GameObject explosionPrefab;
}
