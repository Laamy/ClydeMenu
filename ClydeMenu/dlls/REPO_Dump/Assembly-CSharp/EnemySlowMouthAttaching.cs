using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000076 RID: 118
public class EnemySlowMouthAttaching : MonoBehaviour
{
	// Token: 0x06000473 RID: 1139 RVA: 0x0002C428 File Offset: 0x0002A628
	private void Start()
	{
		this.springQuaternion = new SpringQuaternion();
		this.springQuaternion.damping = 0.5f;
		this.springQuaternion.speed = 20f;
		this.springFloatScale = new SpringFloat();
		this.springFloatScale.damping = 0.5f;
		this.springFloatScale.speed = 20f;
		this.startRotationTopJaw = this.topJaw.localRotation;
		this.startRotationBottomJaw = this.bottomJaw.localRotation;
		this.startPosition = base.transform.position;
		this.particleSystems = new List<ParticleSystem>(this.particleTransform.GetComponentsInChildren<ParticleSystem>());
		this.GoTime();
	}

	// Token: 0x06000474 RID: 1140 RVA: 0x0002C4DC File Offset: 0x0002A6DC
	public void GoTime()
	{
		this.PlayParticles(false);
		if (this.targetPlayerAvatar.isLocal)
		{
			CameraGlitch.Instance.PlayLong();
		}
		this.SetTarget(this.targetPlayerAvatar);
		this.springFloatScale.springVelocity = 50f;
		this.isActive = true;
		this.targetScale = 1f;
		GameDirector.instance.CameraShake.ShakeDistance(3f, 3f, 8f, base.transform.position, 0.1f);
		GameDirector.instance.CameraImpact.ShakeDistance(2f, 3f, 8f, base.transform.position, 0.1f);
		this.soundAttachVO.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x06000475 RID: 1141 RVA: 0x0002C5BC File Offset: 0x0002A7BC
	private void PlayParticles(bool finalPlay)
	{
		foreach (ParticleSystem particleSystem in this.particleSystems)
		{
			particleSystem.Play();
			if (finalPlay)
			{
				particleSystem.transform.parent = null;
				Object.Destroy(particleSystem.gameObject, 4f);
			}
		}
	}

	// Token: 0x06000476 RID: 1142 RVA: 0x0002C630 File Offset: 0x0002A830
	private void SpawnPlayerJaw()
	{
		if (!this.targetPlayerAvatar.isLocal)
		{
			GameObject gameObject = Object.Instantiate<GameObject>(this.topJawPrefab, base.transform.position, base.transform.rotation);
			GameObject gameObject2 = Object.Instantiate<GameObject>(this.bottomJawPrefab, base.transform.position, base.transform.rotation);
			EnemySlowMouthPlayerAvatarAttached component = gameObject.GetComponent<EnemySlowMouthPlayerAvatarAttached>();
			component.jawBot = gameObject2.transform;
			Transform attachPointJawTop = this.targetPlayerAvatar.playerAvatarVisuals.attachPointJawTop;
			Transform attachPointJawBottom = this.targetPlayerAvatar.playerAvatarVisuals.attachPointJawBottom;
			gameObject.transform.parent = attachPointJawTop;
			component.playerTarget = this.targetPlayerAvatar;
			component.enemySlowMouth = this.enemySlowMouth;
			component.semiPuke = gameObject2.GetComponentInChildren<SemiPuke>();
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.rotation = Quaternion.identity;
			gameObject.transform.localRotation = Quaternion.identity;
			gameObject2.transform.parent = attachPointJawBottom;
			gameObject2.transform.localPosition = Vector3.zero;
			gameObject2.transform.rotation = Quaternion.identity;
			gameObject2.transform.localRotation = Quaternion.identity;
			return;
		}
		Transform localCameraTransform = this.targetPlayerAvatar.localCameraTransform;
		GameObject gameObject3 = Object.Instantiate<GameObject>(this.localPlayerJaw, base.transform.position, Quaternion.identity, localCameraTransform);
		gameObject3.transform.localPosition = Vector3.zero;
		gameObject3.transform.localRotation = Quaternion.identity;
		EnemySlowMouthCameraVisuals component2 = gameObject3.GetComponent<EnemySlowMouthCameraVisuals>();
		component2.enemySlowMouth = this.enemySlowMouth;
		component2.playerTarget = this.targetPlayerAvatar;
	}

	// Token: 0x06000477 RID: 1143 RVA: 0x0002C7C8 File Offset: 0x0002A9C8
	private void Update()
	{
		bool flag = !this.targetTransform || !this.targetPlayerAvatar;
		if (!this.isActive)
		{
			return;
		}
		if (flag)
		{
			this.Detach();
			return;
		}
		Quaternion targetRotation = Quaternion.LookRotation(this.targetTransform.position - base.transform.position);
		base.transform.rotation = SemiFunc.SpringQuaternionGet(this.springQuaternion, targetRotation, -1f);
		float d = SemiFunc.SpringFloatGet(this.springFloatScale, this.targetScale, -1f);
		base.transform.localScale = Vector3.one * d;
		float num = Vector3.Distance(base.transform.position, this.targetTransform.position);
		float num2 = num * 2f;
		if (num2 < 4f)
		{
			num2 = 4f;
		}
		if (num2 > 10f)
		{
			num2 = 10f;
		}
		base.transform.position = Vector3.MoveTowards(base.transform.position, this.targetTransform.position, Time.deltaTime * num2);
		if (num < 1f)
		{
			this.targetScale = 2.5f;
		}
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			if (this.enemySlowMouth.currentState != EnemySlowMouth.State.Attack)
			{
				if (!this.targetPlayerAvatar.isDisabled && !this.enemySlowMouth.IsPossessed())
				{
					this.AttachToPlayer();
				}
				else
				{
					this.Detach();
				}
			}
			if (this.targetPlayerAvatar.isLocal)
			{
				GameDirector.instance.CameraShake.Shake(4f, 0.1f);
			}
		}
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		if (!this.targetPlayerAvatar.isDisabled && !this.enemySlowMouth.IsPossessed())
		{
			if (num < 0.1f)
			{
				this.AttachToPlayer();
				this.enemySlowMouth.UpdateState(EnemySlowMouth.State.Attached);
				this.isActive = false;
			}
		}
		else
		{
			this.Detach();
		}
		if (this.targetPlayerAvatar.isLocal)
		{
			CameraAim.Instance.AimTargetSet(base.transform.position, 0.2f, 20f, base.gameObject, 100);
			CameraZoom.Instance.OverrideZoomSet(30f, 0.1f, 8f, 1f, base.gameObject, 50);
		}
		this.tentaclesTransform.localScale = new Vector3(1f + Mathf.Sin(Time.time * 40f) * 0.2f, 1f + Mathf.Sin(Time.time * 60f) * 0.1f, 1f);
		this.tentaclesTransform.localRotation = Quaternion.Euler(0f, 0f, Mathf.Sin(Time.time * 20f) * 10f);
		this.topJaw.localRotation = this.startRotationTopJaw * Quaternion.Euler(Mathf.Sin(Time.time * 60f) * 3f, 0f, 0f);
		this.bottomJaw.localRotation = this.startRotationBottomJaw * Quaternion.Euler(Mathf.Sin(Time.time * 60f) * 10f, 0f, 0f);
		foreach (Transform transform in this.eyeTransforms)
		{
			transform.localScale = new Vector3(1.5f + Mathf.Sin(Time.time * 40f) * 0.5f, 1.5f + Mathf.Sin(Time.time * 60f) * 0.5f, 1.5f);
		}
	}

	// Token: 0x06000478 RID: 1144 RVA: 0x0002CB7C File Offset: 0x0002AD7C
	private void AttachToPlayer()
	{
		if (this.targetPlayerAvatar.isDisabled || this.targetPlayerAvatar.GetComponentInChildren<EnemySlowMouthPlayerAvatarAttached>())
		{
			return;
		}
		this.SpawnPlayerJaw();
		this.Despawn();
	}

	// Token: 0x06000479 RID: 1145 RVA: 0x0002CBAC File Offset: 0x0002ADAC
	private void Detach()
	{
		this.soundAttach.Play(base.transform.position, 1f, 1f, 1f, 1f);
		this.PlayParticles(true);
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			this.enemySlowMouth.detachPosition = base.transform.position;
			this.enemySlowMouth.detachRotation = base.transform.rotation;
			this.enemySlowMouth.UpdateState(EnemySlowMouth.State.Detach);
		}
		Object.Destroy(base.gameObject);
	}

	// Token: 0x0600047A RID: 1146 RVA: 0x0002CC38 File Offset: 0x0002AE38
	private void Despawn()
	{
		this.soundAttach.Play(base.transform.position, 1f, 1f, 1f, 1f);
		this.PlayParticles(true);
		if (this.targetPlayerAvatar.isLocal)
		{
			GameDirector.instance.CameraImpact.Shake(8f, 0.1f);
			GameDirector.instance.CameraShake.Shake(5f, 0.1f);
			CameraGlitch.Instance.PlayLong();
		}
		else
		{
			GameDirector.instance.CameraShake.ShakeDistance(3f, 3f, 8f, base.transform.position, 0.1f);
			GameDirector.instance.CameraImpact.ShakeDistance(2f, 3f, 8f, base.transform.position, 0.1f);
		}
		Object.Destroy(base.gameObject);
	}

	// Token: 0x0600047B RID: 1147 RVA: 0x0002CD29 File Offset: 0x0002AF29
	public void SetTarget(PlayerAvatar _playerAvatar)
	{
		this.targetTransform = SemiFunc.PlayerGetFaceEyeTransform(_playerAvatar);
		this.targetPlayerAvatar = _playerAvatar;
	}

	// Token: 0x04000741 RID: 1857
	internal EnemySlowMouth enemySlowMouth;

	// Token: 0x04000742 RID: 1858
	public Transform tentaclesTransform;

	// Token: 0x04000743 RID: 1859
	public List<Transform> eyeTransforms = new List<Transform>();

	// Token: 0x04000744 RID: 1860
	public Transform topJaw;

	// Token: 0x04000745 RID: 1861
	public Transform bottomJaw;

	// Token: 0x04000746 RID: 1862
	public Transform particleTransform;

	// Token: 0x04000747 RID: 1863
	private List<ParticleSystem> particleSystems = new List<ParticleSystem>();

	// Token: 0x04000748 RID: 1864
	private Quaternion startRotationTopJaw;

	// Token: 0x04000749 RID: 1865
	private Quaternion startRotationBottomJaw;

	// Token: 0x0400074A RID: 1866
	internal Transform targetTransform;

	// Token: 0x0400074B RID: 1867
	public PlayerAvatar targetPlayerAvatar;

	// Token: 0x0400074C RID: 1868
	private SpringQuaternion springQuaternion;

	// Token: 0x0400074D RID: 1869
	private bool isActive;

	// Token: 0x0400074E RID: 1870
	private Vector3 startPosition;

	// Token: 0x0400074F RID: 1871
	private SpringFloat springFloatScale;

	// Token: 0x04000750 RID: 1872
	private float targetScale = 1f;

	// Token: 0x04000751 RID: 1873
	public GameObject topJawPrefab;

	// Token: 0x04000752 RID: 1874
	public GameObject bottomJawPrefab;

	// Token: 0x04000753 RID: 1875
	public GameObject localPlayerJaw;

	// Token: 0x04000754 RID: 1876
	[Space(20f)]
	public Sound soundAttachVO;

	// Token: 0x04000755 RID: 1877
	public Sound soundAttach;
}
