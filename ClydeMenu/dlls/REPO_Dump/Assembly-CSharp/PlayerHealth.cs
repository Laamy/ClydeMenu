using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

// Token: 0x020001CD RID: 461
public class PlayerHealth : MonoBehaviour
{
	// Token: 0x06000FDD RID: 4061 RVA: 0x00091CD0 File Offset: 0x0008FED0
	private void Awake()
	{
		this.photonView = base.GetComponent<PhotonView>();
		if (!this.isMenuAvatar)
		{
			this.playerAvatar = base.GetComponent<PlayerAvatar>();
		}
		else
		{
			this.playerAvatar = PlayerAvatar.instance;
		}
		if (!this.isMenuAvatar && !SemiFunc.RunIsLobbyMenu() && (!GameManager.Multiplayer() || this.photonView.IsMine))
		{
			base.StartCoroutine(this.Fetch());
		}
		this.materialEffectCurve = AssetManager.instance.animationCurveImpact;
		this.renderers = new List<MeshRenderer>();
		this.renderers.AddRange(this.meshParent.GetComponentsInChildren<MeshRenderer>(true));
		foreach (MeshRenderer meshRenderer in this.renderers)
		{
			Material material = null;
			foreach (Material material2 in this.sharedMaterials)
			{
				if (meshRenderer.sharedMaterial.name == material2.name)
				{
					material = material2;
					meshRenderer.sharedMaterial = this.instancedMaterials[this.sharedMaterials.IndexOf(material2)];
				}
			}
			if (!material)
			{
				string name = meshRenderer.sharedMaterial.name;
				material = meshRenderer.sharedMaterial;
				this.sharedMaterials.Add(material);
				this.instancedMaterials.Add(meshRenderer.material);
				if (name == "Player Avatar - Body")
				{
					this.bodyMaterial = meshRenderer.sharedMaterial;
				}
				if (name == "Player Avatar - Health")
				{
					this.healthMaterial = meshRenderer.sharedMaterial;
				}
				if (name == "Player Avatar - Eye")
				{
					this.eyeMaterial = meshRenderer.sharedMaterial;
				}
				if (name == "Player Avatar - Pupil")
				{
					this.pupilMaterial = meshRenderer.sharedMaterial;
				}
			}
		}
		this.materialHurtColor = Shader.PropertyToID("_ColorOverlay");
		this.materialHurtAmount = Shader.PropertyToID("_ColorOverlayAmount");
		this.healthMaterialAmount = Shader.PropertyToID("_OffsetX");
	}

	// Token: 0x06000FDE RID: 4062 RVA: 0x00091F10 File Offset: 0x00090110
	private void Start()
	{
		foreach (DebugComputerCheck debugComputerCheck in Object.FindObjectsOfType<DebugComputerCheck>())
		{
			if (debugComputerCheck.Active && debugComputerCheck.PlayerDebug && debugComputerCheck.GodMode)
			{
				this.godMode = true;
			}
		}
	}

	// Token: 0x06000FDF RID: 4063 RVA: 0x00091F54 File Offset: 0x00090154
	private IEnumerator Fetch()
	{
		while (!LevelGenerator.Instance.Generated)
		{
			yield return new WaitForSeconds(0.1f);
		}
		int num = StatsManager.instance.GetPlayerHealth(SemiFunc.PlayerGetSteamID(this.playerAvatar));
		if (num <= 0)
		{
			num = 1;
		}
		this.health = num;
		this.maxHealth = 100 + StatsManager.instance.GetPlayerMaxHealth(SemiFunc.PlayerGetSteamID(this.playerAvatar));
		this.health = Mathf.Clamp(this.health, 0, this.maxHealth);
		if (SemiFunc.RunIsArena())
		{
			this.health = this.maxHealth;
		}
		StatsManager.instance.SetPlayerHealth(SemiFunc.PlayerGetSteamID(this.playerAvatar), this.health, false);
		if (GameManager.Multiplayer())
		{
			this.photonView.RPC("UpdateHealthRPC", RpcTarget.Others, new object[]
			{
				this.health,
				this.maxHealth,
				true
			});
		}
		this.healthSet = true;
		yield break;
	}

	// Token: 0x06000FE0 RID: 4064 RVA: 0x00091F64 File Offset: 0x00090164
	private void Update()
	{
		if (this.playerAvatar.isLocal)
		{
			if (this.overrideEyeMaterialTimer > 0f)
			{
				this.overrideEyeMaterialTimer -= Time.deltaTime;
				if (!this.overrideEyeActive)
				{
					this.overrideEyeActive = true;
				}
			}
			else if (this.overrideEyeActive)
			{
				this.overrideEyeActive = false;
				this.overrideEyePriority = -999;
			}
			if (SemiFunc.IsMultiplayer() && (this.overrideEyeActive != this.overrideEyeActivePrevious || this.overrideEyeState != this.overrideEyeStatePrevious))
			{
				this.overrideEyeActivePrevious = this.overrideEyeActive;
				this.overrideEyeStatePrevious = this.overrideEyeState;
				this.photonView.RPC("EyeMaterialOverrideRPC", RpcTarget.Others, new object[]
				{
					this.overrideEyeState,
					this.overrideEyeActive
				});
			}
		}
		if (this.overrideEyeActive)
		{
			this.overrideEyeMaterialLerp += 3f * Time.deltaTime;
		}
		else
		{
			this.overrideEyeMaterialLerp -= 3f * Time.deltaTime;
		}
		this.overrideEyeMaterialLerp = Mathf.Clamp01(this.overrideEyeMaterialLerp);
		if (this.overrideEyeMaterialLerp != this.overrideEyeMaterialLerpPrevious)
		{
			this.overrideEyeMaterialLerpPrevious = this.overrideEyeMaterialLerp;
			float num = AssetManager.instance.animationCurveEaseInOut.Evaluate(this.overrideEyeMaterialLerp);
			this.eyeMaterial.SetFloat(this.materialHurtAmount, num);
			this.eyeMaterial.SetColor(this.materialHurtColor, this.overrideEyeMaterialColor);
			this.pupilMaterial.SetFloat(this.materialHurtAmount, num);
			this.pupilMaterial.SetColor(this.materialHurtColor, this.overridePupilMaterialColor);
			if (this.overrideEyeMaterialLerp <= 0f)
			{
				this.eyeLight.gameObject.SetActive(false);
			}
			else if (!this.eyeLight.gameObject.activeSelf)
			{
				this.eyeLight.gameObject.SetActive(true);
			}
			this.eyeLight.color = this.overrideEyeLightColor;
			this.eyeLight.intensity = this.overrideEyeLightIntensity * num;
		}
		if (this.materialEffect)
		{
			this.materialEffectLerp += 2.5f * Time.deltaTime;
			this.materialEffectLerp = Mathf.Clamp01(this.materialEffectLerp);
			if (this.playerAvatar.deadSet && !this.playerAvatar.isDisabled)
			{
				this.materialEffectLerp = Mathf.Clamp(this.materialEffectLerp, 0f, 0.1f);
			}
			foreach (Material material in this.instancedMaterials)
			{
				if (material != this.eyeMaterial && material != this.pupilMaterial)
				{
					material.SetFloat(this.materialHurtAmount, this.materialEffectCurve.Evaluate(this.materialEffectLerp));
				}
			}
			if (this.hurtFreeze && this.materialEffectLerp > 0.2f)
			{
				this.hurtFreeze = false;
			}
			if (this.materialEffectLerp >= 1f)
			{
				this.materialEffect = false;
				foreach (Material material2 in this.instancedMaterials)
				{
					if (material2 != this.eyeMaterial && material2 != this.pupilMaterial)
					{
						material2.SetFloat(this.materialHurtAmount, 0f);
					}
				}
			}
			this.hurtFreezeTimer = 0f;
			if (!this.overrideEyeActive)
			{
				this.eyeMaterial.SetFloat(this.materialHurtAmount, this.materialEffectCurve.Evaluate(this.materialEffectLerp));
				this.eyeMaterial.SetColor(this.materialHurtColor, Color.white);
				this.pupilMaterial.SetFloat(this.materialHurtAmount, this.materialEffectCurve.Evaluate(this.materialEffectLerp));
				this.pupilMaterial.SetColor(this.materialHurtColor, Color.black);
			}
		}
		else if (this.hurtFreeze)
		{
			this.hurtFreezeTimer -= Time.deltaTime;
			if (this.hurtFreezeTimer <= 0f)
			{
				this.hurtFreeze = false;
			}
		}
		if (this.isMenuAvatar)
		{
			this.health = this.playerAvatar.playerHealth.health;
		}
		if ((this.isMenuAvatar || (GameManager.Multiplayer() && !this.playerAvatar.isLocal)) && this.healthPrevious != this.health)
		{
			float num2 = (float)this.health / (float)this.maxHealth;
			float num3 = Mathf.Lerp(0.98f, 0f, num2);
			if (num3 <= 0f)
			{
				num3 = -0.5f;
			}
			this.healthMaterial.SetFloat(this.healthMaterialAmount, num3);
			int nameID = Shader.PropertyToID("_AlbedoColor");
			int nameID2 = Shader.PropertyToID("_EmissionColor");
			Color value = this.healthMaterialColor.Evaluate(num2);
			this.healthMaterial.SetColor(nameID, value);
			value.a = this.healthMaterial.GetColor(nameID2).a;
			this.healthMaterial.SetColor(nameID2, value);
			this.healthPrevious = this.health;
		}
		if (this.invincibleTimer > 0f)
		{
			this.invincibleTimer -= Time.deltaTime;
		}
	}

	// Token: 0x06000FE1 RID: 4065 RVA: 0x000924C0 File Offset: 0x000906C0
	public void HurtFreezeOverride(float _time)
	{
		this.hurtFreeze = true;
		this.hurtFreezeTimer = _time;
	}

	// Token: 0x06000FE2 RID: 4066 RVA: 0x000924D0 File Offset: 0x000906D0
	public void Death()
	{
		this.health = 0;
		StatsManager.instance.SetPlayerHealth(SemiFunc.PlayerGetSteamID(this.playerAvatar), this.health, false);
		if (GameManager.Multiplayer())
		{
			this.photonView.RPC("UpdateHealthRPC", RpcTarget.Others, new object[]
			{
				this.health,
				this.maxHealth,
				true
			});
		}
	}

	// Token: 0x06000FE3 RID: 4067 RVA: 0x00092543 File Offset: 0x00090743
	public void MaterialEffectOverride(PlayerHealth.Effect _effect)
	{
		if (!GameManager.Multiplayer())
		{
			this.MaterialEffectOverrideRPC((int)_effect);
			return;
		}
		if (PhotonNetwork.IsMasterClient)
		{
			this.photonView.RPC("MaterialEffectOverrideRPC", RpcTarget.All, new object[]
			{
				(int)_effect
			});
		}
	}

	// Token: 0x06000FE4 RID: 4068 RVA: 0x0009257C File Offset: 0x0009077C
	[PunRPC]
	public void MaterialEffectOverrideRPC(int _effect)
	{
		this.materialEffect = true;
		this.materialEffectLerp = 0f;
		Color white = Color.white;
		if (_effect == 0)
		{
			white = new Color(1f, 0.94f, 0f);
			if (!this.playerAvatar.isLocal)
			{
				this.upgradeOther.Play(base.transform.position, 1f, 1f, 1f, 1f);
			}
		}
		foreach (Material material in this.instancedMaterials)
		{
			if (material != this.eyeMaterial && material != this.pupilMaterial)
			{
				material.SetColor(this.materialHurtColor, white);
			}
		}
	}

	// Token: 0x06000FE5 RID: 4069 RVA: 0x0009265C File Offset: 0x0009085C
	public void Hurt(int damage, bool savingGrace, int enemyIndex = -1)
	{
		if (this.invincibleTimer > 0f)
		{
			return;
		}
		if (damage <= 0)
		{
			return;
		}
		if (GameManager.Multiplayer() && !this.photonView.IsMine)
		{
			return;
		}
		if (this.playerAvatar.deadSet || this.godMode)
		{
			return;
		}
		if (savingGrace && damage <= 25 && this.health > 5 && this.health <= 20)
		{
			this.health -= damage;
			if (this.health <= 0)
			{
				this.health = Random.Range(1, 5);
			}
		}
		else
		{
			this.health -= damage;
		}
		if (this.health <= 0)
		{
			this.playerAvatar.PlayerDeath(enemyIndex);
			this.health = 0;
			return;
		}
		if ((float)damage >= 25f)
		{
			CameraGlitch.Instance.PlayLongHurt();
		}
		else
		{
			CameraGlitch.Instance.PlayShortHurt();
		}
		StatsManager.instance.SetPlayerHealth(SemiFunc.PlayerGetSteamID(this.playerAvatar), this.health, false);
		if (GameManager.Multiplayer())
		{
			this.photonView.RPC("UpdateHealthRPC", RpcTarget.Others, new object[]
			{
				this.health,
				this.maxHealth,
				true
			});
		}
	}

	// Token: 0x06000FE6 RID: 4070 RVA: 0x00092794 File Offset: 0x00090994
	public void HurtOther(int damage, Vector3 hurtPosition, bool savingGrace, int enemyIndex = -1)
	{
		if (!GameManager.Multiplayer())
		{
			this.Hurt(damage, savingGrace, enemyIndex);
			return;
		}
		this.photonView.RPC("HurtOtherRPC", RpcTarget.All, new object[]
		{
			damage,
			hurtPosition,
			savingGrace,
			enemyIndex
		});
	}

	// Token: 0x06000FE7 RID: 4071 RVA: 0x000927F0 File Offset: 0x000909F0
	[PunRPC]
	public void HurtOtherRPC(int damage, Vector3 hurtPosition, bool savingGrace, int enemyIndex, PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.MasterAndOwnerOnlyRPC(_info, this.photonView))
		{
			return;
		}
		if (this.photonView.IsMine && (hurtPosition == Vector3.zero || Vector3.Distance(this.playerAvatar.transform.position, hurtPosition) < 2f))
		{
			this.Hurt(damage, savingGrace, enemyIndex);
		}
	}

	// Token: 0x06000FE8 RID: 4072 RVA: 0x00092850 File Offset: 0x00090A50
	public void Heal(int healAmount, bool effect = true)
	{
		if (healAmount <= 0 || this.health == this.maxHealth)
		{
			return;
		}
		if (GameManager.Multiplayer() && !this.photonView.IsMine)
		{
			return;
		}
		if (this.playerAvatar.isDisabled || this.godMode)
		{
			return;
		}
		if (effect && this.health != 0)
		{
			if ((float)healAmount >= 25f)
			{
				CameraGlitch.Instance.PlayLongHeal();
			}
			else
			{
				CameraGlitch.Instance.PlayShortHeal();
			}
		}
		this.health += healAmount;
		this.health = Mathf.Clamp(this.health, 0, this.maxHealth);
		StatsManager.instance.SetPlayerHealth(SemiFunc.PlayerGetSteamID(this.playerAvatar), this.health, false);
		if (GameManager.Multiplayer())
		{
			this.photonView.RPC("UpdateHealthRPC", RpcTarget.Others, new object[]
			{
				this.health,
				this.maxHealth,
				effect
			});
		}
	}

	// Token: 0x06000FE9 RID: 4073 RVA: 0x0009294A File Offset: 0x00090B4A
	public void HealOther(int healAmount, bool effect)
	{
		if (!GameManager.Multiplayer())
		{
			this.Heal(healAmount, effect);
			return;
		}
		this.photonView.RPC("HealOtherRPC", RpcTarget.All, new object[]
		{
			healAmount,
			effect
		});
	}

	// Token: 0x06000FEA RID: 4074 RVA: 0x00092985 File Offset: 0x00090B85
	[PunRPC]
	public void HealOtherRPC(int healAmount, bool effect)
	{
		if (this.photonView.IsMine)
		{
			this.Heal(healAmount, effect);
		}
	}

	// Token: 0x06000FEB RID: 4075 RVA: 0x0009299C File Offset: 0x00090B9C
	[PunRPC]
	public void UpdateHealthRPC(int healthNew, int healthMax, bool effect, PhotonMessageInfo _info = default(PhotonMessageInfo))
	{
		if (!SemiFunc.MasterAndOwnerOnlyRPC(_info, this.photonView))
		{
			return;
		}
		this.maxHealth = healthMax;
		if (!this.healthSet)
		{
			this.health = healthNew;
			this.healthSet = true;
		}
		else
		{
			if (effect)
			{
				this.materialEffect = true;
				if (!this.playerAvatar.deadSet)
				{
					this.materialEffectLerp = 0f;
				}
				if (healthNew < this.health || healthNew == 0)
				{
					this.hurtOther.Play(base.transform.position, 1f, 1f, 1f, 1f);
					this.hurtFreeze = true;
					using (List<Material>.Enumerator enumerator = this.instancedMaterials.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							Material material = enumerator.Current;
							if (material != this.eyeMaterial && material != this.pupilMaterial)
							{
								material.SetColor(this.materialHurtColor, Color.red);
							}
						}
						goto IL_12D;
					}
				}
				if (this.health != 0)
				{
					this.healOther.Play(base.transform.position, 1f, 1f, 1f, 1f);
				}
				this.SetMaterialGreen();
			}
			IL_12D:
			this.health = healthNew;
		}
		StatsManager.instance.SetPlayerHealth(SemiFunc.PlayerGetSteamID(this.playerAvatar), this.health, false);
	}

	// Token: 0x06000FEC RID: 4076 RVA: 0x00092B0C File Offset: 0x00090D0C
	public void SetMaterialGreen()
	{
		foreach (Material material in this.instancedMaterials)
		{
			if (material != this.eyeMaterial && material != this.pupilMaterial)
			{
				material.SetColor(this.materialHurtColor, new Color(0f, 1f, 0.25f));
			}
		}
	}

	// Token: 0x06000FED RID: 4077 RVA: 0x00092B94 File Offset: 0x00090D94
	private void EyeMaterialSetup()
	{
		if (this.overrideEyeMaterialLerp >= 1f)
		{
			this.overrideEyeMaterialLerp = 0.8f;
		}
		if (this.overrideEyeState == PlayerHealth.EyeOverrideState.Red)
		{
			this.overrideEyeMaterialColor = Color.red;
			this.overridePupilMaterialColor = Color.white;
			this.overrideEyeLightColor = Color.red;
			this.overrideEyeLightIntensity = 5f;
			return;
		}
		if (this.overrideEyeState == PlayerHealth.EyeOverrideState.Green)
		{
			this.overrideEyeMaterialColor = Color.green;
			this.overridePupilMaterialColor = Color.white;
			this.overrideEyeLightColor = Color.green;
			this.overrideEyeLightIntensity = 5f;
			return;
		}
		if (this.overrideEyeState == PlayerHealth.EyeOverrideState.Love)
		{
			this.overrideEyeMaterialColor = new Color(1f, 0f, 0.5f);
			this.overridePupilMaterialColor = new Color(0.2f, 0f, 0.2f);
			this.overrideEyeLightColor = new Color(0.4f, 0f, 0f);
			this.overrideEyeLightIntensity = 1f;
			return;
		}
		if (this.overrideEyeState == PlayerHealth.EyeOverrideState.CeilingEye)
		{
			this.overrideEyeMaterialColor = new Color(1f, 0.4f, 0f);
			this.overridePupilMaterialColor = new Color(1f, 1f, 0f);
			this.overrideEyeLightColor = new Color(1f, 0.4f, 0f);
			this.overrideEyeLightIntensity = 1f;
			return;
		}
		if (this.overrideEyeState == PlayerHealth.EyeOverrideState.Inverted)
		{
			this.overrideEyeMaterialColor = new Color(0f, 0f, 0f);
			this.overridePupilMaterialColor = new Color(1f, 1f, 1f);
			this.overrideEyeLightColor = new Color(0f, 0f, 0f);
			this.overrideEyeLightIntensity = 1f;
		}
	}

	// Token: 0x06000FEE RID: 4078 RVA: 0x00092D4D File Offset: 0x00090F4D
	public void EyeMaterialOverride(PlayerHealth.EyeOverrideState _state, float _time, int _priority)
	{
		if (_priority < this.overrideEyePriority)
		{
			return;
		}
		this.overrideEyePriority = _priority;
		if (this.overrideEyeState != _state)
		{
			this.overrideEyeState = _state;
			this.EyeMaterialSetup();
		}
		this.overrideEyeMaterialTimer = _time;
	}

	// Token: 0x06000FEF RID: 4079 RVA: 0x00092D7D File Offset: 0x00090F7D
	[PunRPC]
	public void EyeMaterialOverrideRPC(PlayerHealth.EyeOverrideState _state, bool _active)
	{
		this.overrideEyeActive = _active;
		if (this.overrideEyeState != _state)
		{
			this.overrideEyeState = _state;
			this.EyeMaterialSetup();
		}
	}

	// Token: 0x06000FF0 RID: 4080 RVA: 0x00092D9C File Offset: 0x00090F9C
	public void InvincibleSet(float _time)
	{
		this.invincibleTimer = _time;
	}

	// Token: 0x04001AF9 RID: 6905
	public bool isMenuAvatar;

	// Token: 0x04001AFA RID: 6906
	private PlayerExpression playerExpression;

	// Token: 0x04001AFB RID: 6907
	private PlayerAvatar playerAvatar;

	// Token: 0x04001AFC RID: 6908
	internal PhotonView photonView;

	// Token: 0x04001AFD RID: 6909
	internal bool hurtFreeze;

	// Token: 0x04001AFE RID: 6910
	private float hurtFreezeTimer;

	// Token: 0x04001AFF RID: 6911
	private bool healthSet;

	// Token: 0x04001B00 RID: 6912
	internal int health = 100;

	// Token: 0x04001B01 RID: 6913
	private int healthPrevious;

	// Token: 0x04001B02 RID: 6914
	internal int maxHealth = 100;

	// Token: 0x04001B03 RID: 6915
	private bool godMode;

	// Token: 0x04001B04 RID: 6916
	public Transform meshParent;

	// Token: 0x04001B05 RID: 6917
	public Light eyeLight;

	// Token: 0x04001B06 RID: 6918
	private List<MeshRenderer> renderers;

	// Token: 0x04001B07 RID: 6919
	private List<Material> sharedMaterials = new List<Material>();

	// Token: 0x04001B08 RID: 6920
	internal List<Material> instancedMaterials = new List<Material>();

	// Token: 0x04001B09 RID: 6921
	private int materialHurtAmount;

	// Token: 0x04001B0A RID: 6922
	private int materialHurtColor;

	// Token: 0x04001B0B RID: 6923
	internal Material bodyMaterial;

	// Token: 0x04001B0C RID: 6924
	internal Material eyeMaterial;

	// Token: 0x04001B0D RID: 6925
	internal Material pupilMaterial;

	// Token: 0x04001B0E RID: 6926
	private Material healthMaterial;

	// Token: 0x04001B0F RID: 6927
	private int healthMaterialAmount;

	// Token: 0x04001B10 RID: 6928
	public Gradient healthMaterialColor;

	// Token: 0x04001B11 RID: 6929
	private bool materialEffect;

	// Token: 0x04001B12 RID: 6930
	private Color materialEffectColor;

	// Token: 0x04001B13 RID: 6931
	private AnimationCurve materialEffectCurve;

	// Token: 0x04001B14 RID: 6932
	private float materialEffectLerp;

	// Token: 0x04001B15 RID: 6933
	public Sound hurtOther;

	// Token: 0x04001B16 RID: 6934
	public Sound healOther;

	// Token: 0x04001B17 RID: 6935
	public Sound upgradeOther;

	// Token: 0x04001B18 RID: 6936
	private float overrideEyeMaterialLerp;

	// Token: 0x04001B19 RID: 6937
	private float overrideEyeMaterialLerpPrevious;

	// Token: 0x04001B1A RID: 6938
	private float overrideEyeMaterialTimer;

	// Token: 0x04001B1B RID: 6939
	private Color overrideEyeMaterialColor;

	// Token: 0x04001B1C RID: 6940
	private Color overridePupilMaterialColor;

	// Token: 0x04001B1D RID: 6941
	private Color overrideEyeLightColor;

	// Token: 0x04001B1E RID: 6942
	private float overrideEyeLightIntensity;

	// Token: 0x04001B1F RID: 6943
	private bool overrideEyeActive;

	// Token: 0x04001B20 RID: 6944
	private bool overrideEyeActivePrevious;

	// Token: 0x04001B21 RID: 6945
	private int overrideEyePriority = -999;

	// Token: 0x04001B22 RID: 6946
	private PlayerHealth.EyeOverrideState overrideEyeState;

	// Token: 0x04001B23 RID: 6947
	private PlayerHealth.EyeOverrideState overrideEyeStatePrevious;

	// Token: 0x04001B24 RID: 6948
	private float invincibleTimer;

	// Token: 0x020003BE RID: 958
	public enum Effect
	{
		// Token: 0x04002C38 RID: 11320
		Upgrade
	}

	// Token: 0x020003BF RID: 959
	public enum EyeOverrideState
	{
		// Token: 0x04002C3A RID: 11322
		None,
		// Token: 0x04002C3B RID: 11323
		Red,
		// Token: 0x04002C3C RID: 11324
		Green,
		// Token: 0x04002C3D RID: 11325
		Love,
		// Token: 0x04002C3E RID: 11326
		CeilingEye,
		// Token: 0x04002C3F RID: 11327
		Inverted
	}
}
