using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x020000FA RID: 250
public class LightManager : MonoBehaviour
{
	// Token: 0x060008CD RID: 2253 RVA: 0x00054FAA File Offset: 0x000531AA
	private void Awake()
	{
		LightManager.instance = this;
	}

	// Token: 0x060008CE RID: 2254 RVA: 0x00054FB2 File Offset: 0x000531B2
	private void Start()
	{
		this.debugActive = SemiFunc.DebugDev();
	}

	// Token: 0x060008CF RID: 2255 RVA: 0x00054FC0 File Offset: 0x000531C0
	private void Update()
	{
		if (GameDirector.instance.currentState != GameDirector.gameState.Main)
		{
			return;
		}
		if (!PlayerAvatar.instance)
		{
			return;
		}
		if (!this.lightCullTarget)
		{
			this.lightCullTarget = PlayerAvatar.instance.transform;
		}
		this.LogicUpdate();
		if (this.debugActive)
		{
			int num = 0;
			foreach (PropLight propLight in this.propLights)
			{
				if (propLight && propLight.gameObject.activeInHierarchy)
				{
					num++;
				}
			}
			this.activeLightsAmount = num;
		}
		if (this.updateInstant)
		{
			this.updateInstantTimer -= Time.deltaTime;
			if (this.updateInstantTimer <= 0f)
			{
				this.updateInstant = false;
			}
		}
		if (RoundDirector.instance.allExtractionPointsCompleted && !this.turnOffLights)
		{
			base.StopAllCoroutines();
			this.turningOffLights = true;
			base.StartCoroutine(this.TurnOffLights());
			this.turningOffEmissions = true;
			base.StartCoroutine(this.TurnOffEmissions());
			this.turnOffLights = true;
		}
	}

	// Token: 0x060008D0 RID: 2256 RVA: 0x000550EC File Offset: 0x000532EC
	private IEnumerator TurnOffLights()
	{
		int _lightsPerFrame = 5;
		int _lightsPerFrameCounter = 0;
		foreach (PropLight propLight in Enumerable.ToList<PropLight>(this.propLights))
		{
			if (propLight && propLight.levelLight)
			{
				propLight.lightComponent.intensity = 0f;
				propLight.originalIntensity = 0f;
				if (propLight.hasHalo)
				{
					propLight.halo.enabled = false;
				}
				propLight.turnedOff = true;
				int num = _lightsPerFrameCounter;
				_lightsPerFrameCounter = num + 1;
				if (_lightsPerFrameCounter >= _lightsPerFrame)
				{
					_lightsPerFrameCounter = 0;
					yield return null;
				}
			}
		}
		List<PropLight>.Enumerator enumerator = default(List<PropLight>.Enumerator);
		this.turningOffLights = false;
		yield break;
		yield break;
	}

	// Token: 0x060008D1 RID: 2257 RVA: 0x000550FB File Offset: 0x000532FB
	private IEnumerator TurnOffEmissions()
	{
		int _emissionsPerFrame = 5;
		int _emissionsPerFrameCounter = 0;
		foreach (PropLightEmission propLightEmission in Enumerable.ToList<PropLightEmission>(this.propEmissions))
		{
			if (propLightEmission && propLightEmission.levelLight)
			{
				propLightEmission.material.SetColor("_EmissionColor", Color.black);
				propLightEmission.originalEmission = Color.black;
				propLightEmission.turnedOff = true;
				int num = _emissionsPerFrameCounter;
				_emissionsPerFrameCounter = num + 1;
				if (_emissionsPerFrameCounter >= _emissionsPerFrame)
				{
					_emissionsPerFrameCounter = 0;
					yield return null;
				}
			}
		}
		List<PropLightEmission>.Enumerator enumerator = default(List<PropLightEmission>.Enumerator);
		this.turningOffEmissions = false;
		yield break;
		yield break;
	}

	// Token: 0x060008D2 RID: 2258 RVA: 0x0005510C File Offset: 0x0005330C
	private void Setup()
	{
		this.setup = true;
		if (this.lightCullTarget)
		{
			this.lastCheckPos = this.lightCullTarget.position;
		}
		foreach (GameObject gameObject in GameObject.FindGameObjectsWithTag("Prop Lights"))
		{
			if (gameObject)
			{
				PropLight component = gameObject.GetComponent<PropLight>();
				if (component)
				{
					this.propLights.Add(component);
				}
				else
				{
					Debug.LogError("PropLight component not found in " + gameObject.name, gameObject);
				}
			}
		}
		foreach (GameObject gameObject2 in GameObject.FindGameObjectsWithTag("Prop Emission"))
		{
			if (gameObject2)
			{
				PropLightEmission component2 = gameObject2.GetComponent<PropLightEmission>();
				if (component2)
				{
					this.propEmissions.Add(component2);
				}
				else
				{
					Debug.LogError("PropLightEmission component not found in " + gameObject2.name, gameObject2);
				}
			}
		}
		foreach (PropLight propLight in this.propLights)
		{
			if (!propLight.turnedOff)
			{
				this.HandleLightActivation(propLight);
			}
		}
		foreach (PropLightEmission propLightEmission in this.propEmissions)
		{
			if (!propLightEmission.turnedOff)
			{
				this.HandleEmissionActivation(propLightEmission);
			}
		}
	}

	// Token: 0x060008D3 RID: 2259 RVA: 0x00055298 File Offset: 0x00053498
	private void LogicUpdate()
	{
		if (!LevelGenerator.Instance.Generated)
		{
			return;
		}
		if (!this.setup)
		{
			this.Setup();
		}
		if (this.logicUpdateTimer > 0f)
		{
			this.logicUpdateTimer -= Time.deltaTime;
			return;
		}
		if (!this.lightCullTarget)
		{
			return;
		}
		bool flag = false;
		if (Mathf.Abs(this.lightCullTarget.eulerAngles.y - this.lastYRotation) >= 20f)
		{
			this.lastYRotation = this.lightCullTarget.eulerAngles.y;
			flag = true;
		}
		if (!this.turningOffLights && !this.turningOffEmissions && (Vector3.Distance(this.lastCheckPos, this.lightCullTarget.position) >= this.checkDistance || flag))
		{
			this.logicUpdateTimer = 0.5f;
			this.UpdateLights();
		}
	}

	// Token: 0x060008D4 RID: 2260 RVA: 0x00055374 File Offset: 0x00053574
	private void UpdateLights()
	{
		this.lastCheckPos = this.lightCullTarget.position;
		List<PropLight> list = new List<PropLight>();
		foreach (PropLight propLight in this.propLights)
		{
			if (propLight)
			{
				this.HandleLightActivation(propLight);
			}
			else
			{
				list.Add(propLight);
			}
		}
		foreach (PropLight propLight2 in list)
		{
			this.propLights.Remove(propLight2);
		}
		List<PropLightEmission> list2 = new List<PropLightEmission>();
		foreach (PropLightEmission propLightEmission in this.propEmissions)
		{
			if (propLightEmission)
			{
				this.HandleEmissionActivation(propLightEmission);
			}
			else
			{
				list2.Add(propLightEmission);
			}
		}
		foreach (PropLightEmission propLightEmission2 in list2)
		{
			this.propEmissions.Remove(propLightEmission2);
		}
	}

	// Token: 0x060008D5 RID: 2261 RVA: 0x000554D8 File Offset: 0x000536D8
	public void RemoveLight(PropLight PropLight)
	{
		if (PropLight && this.propLights.Contains(PropLight))
		{
			this.propLights.Remove(PropLight);
		}
	}

	// Token: 0x060008D6 RID: 2262 RVA: 0x00055500 File Offset: 0x00053700
	private void HandleLightActivation(PropLight propLight)
	{
		if (!this.lightCullTarget)
		{
			return;
		}
		Vector3 position = propLight.transform.position;
		Vector3 position2 = this.lightCullTarget.position;
		bool flag = Vector3.Dot(propLight.transform.position - this.lightCullTarget.position, this.lightCullTarget.forward) <= -0.25f;
		if (SpectateCamera.instance)
		{
			flag = false;
			if (SpectateCamera.instance.CheckState(SpectateCamera.State.Death))
			{
				position.y = 0f;
				position2.y = 0f;
			}
		}
		float num = Vector3.Distance(position, position2);
		float num2 = GraphicsManager.instance.lightDistance * propLight.lightRangeMultiplier;
		if (propLight.gameObject.activeInHierarchy && ((num >= num2 && !flag) || (num >= num2 * 0.8f && flag)))
		{
			base.StartCoroutine(this.FadeLightIntensity(propLight, 0f, Random.Range(this.fadeTimeMin, this.fadeTimeMax), delegate
			{
				propLight.gameObject.SetActive(false);
			}));
			return;
		}
		if (!propLight.gameObject.activeInHierarchy && num < num2)
		{
			propLight.gameObject.SetActive(true);
			propLight.lightComponent.intensity = 0f;
			base.StartCoroutine(this.FadeLightIntensity(propLight, propLight.originalIntensity, Random.Range(this.fadeTimeMin, this.fadeTimeMax), null));
		}
	}

	// Token: 0x060008D7 RID: 2263 RVA: 0x000556A6 File Offset: 0x000538A6
	public void UpdateInstant()
	{
		if (!this.setup)
		{
			return;
		}
		this.updateInstant = true;
		this.updateInstantTimer = 0.1f;
		this.UpdateLights();
	}

	// Token: 0x060008D8 RID: 2264 RVA: 0x000556CC File Offset: 0x000538CC
	private void HandleEmissionActivation(PropLightEmission _propLightEmission)
	{
		if (!this.lightCullTarget)
		{
			return;
		}
		Vector3 position = _propLightEmission.transform.position;
		Vector3 position2 = this.lightCullTarget.position;
		if (SpectateCamera.instance && SpectateCamera.instance.CheckState(SpectateCamera.State.Death))
		{
			position.y = 0f;
			position2.y = 0f;
		}
		if (Vector3.Distance(position, position2) >= GraphicsManager.instance.lightDistance)
		{
			base.StartCoroutine(this.FadeEmissionIntensity(_propLightEmission, Color.black, Random.Range(this.fadeTimeMin, this.fadeTimeMax)));
			return;
		}
		base.StartCoroutine(this.FadeEmissionIntensity(_propLightEmission, _propLightEmission.originalEmission, Random.Range(this.fadeTimeMin, this.fadeTimeMax)));
	}

	// Token: 0x060008D9 RID: 2265 RVA: 0x0005578D File Offset: 0x0005398D
	private IEnumerator FadeLightIntensity(PropLight propLight, float targetIntensity, float duration, Action onComplete = null)
	{
		if (!propLight || !propLight.lightComponent)
		{
			yield break;
		}
		float startTime = Time.time;
		float startIntensity = propLight.lightComponent.intensity;
		while (Time.time - startTime < duration && !this.updateInstant)
		{
			if (!propLight || !propLight.lightComponent)
			{
				yield break;
			}
			float time = (Time.time - startTime) / duration;
			propLight.lightComponent.intensity = Mathf.Lerp(startIntensity, targetIntensity, this.fadeCullCurve.Evaluate(time));
			yield return null;
		}
		if (!propLight || !propLight.lightComponent)
		{
			yield break;
		}
		propLight.lightComponent.intensity = targetIntensity;
		if (Mathf.Approximately(targetIntensity, 0f) && propLight.gameObject.CompareTag("Prop Lights"))
		{
			propLight.gameObject.SetActive(false);
		}
		if (onComplete != null)
		{
			onComplete.Invoke();
		}
		yield break;
	}

	// Token: 0x060008DA RID: 2266 RVA: 0x000557B9 File Offset: 0x000539B9
	private IEnumerator FadeEmissionIntensity(PropLightEmission _propLightEmission, Color targetColor, float duration)
	{
		if (!_propLightEmission)
		{
			yield break;
		}
		float startTime = Time.time;
		Color startColor = _propLightEmission.material.GetColor("_EmissionColor");
		while (Time.time - startTime < duration && !this.updateInstant)
		{
			if (!_propLightEmission)
			{
				yield break;
			}
			float time = (Time.time - startTime) / duration;
			_propLightEmission.material.SetColor("_EmissionColor", Color.Lerp(startColor, targetColor, this.fadeCullCurve.Evaluate(time)));
			yield return null;
		}
		if (!_propLightEmission)
		{
			yield break;
		}
		_propLightEmission.material.SetColor("_EmissionColor", targetColor);
		yield break;
	}

	// Token: 0x0400100F RID: 4111
	[HideInInspector]
	public Transform lightCullTarget;

	// Token: 0x04001010 RID: 4112
	public float checkDistance = 5f;

	// Token: 0x04001011 RID: 4113
	public float fadeTimeMin = 1f;

	// Token: 0x04001012 RID: 4114
	public float fadeTimeMax = 1f;

	// Token: 0x04001013 RID: 4115
	public AnimationCurve fadeCurve;

	// Token: 0x04001014 RID: 4116
	public AnimationCurve fadeCullCurve;

	// Token: 0x04001015 RID: 4117
	public static LightManager instance;

	// Token: 0x04001016 RID: 4118
	internal List<PropLight> propLights = new List<PropLight>();

	// Token: 0x04001017 RID: 4119
	private List<PropLightEmission> propEmissions = new List<PropLightEmission>();

	// Token: 0x04001018 RID: 4120
	private Vector3 lastCheckPos;

	// Token: 0x04001019 RID: 4121
	private float lastYRotation;

	// Token: 0x0400101A RID: 4122
	internal int activeLightsAmount;

	// Token: 0x0400101B RID: 4123
	internal bool updateInstant;

	// Token: 0x0400101C RID: 4124
	internal float updateInstantTimer;

	// Token: 0x0400101D RID: 4125
	[Space]
	[Header("Sounds")]
	public Sound lampFlickerSound;

	// Token: 0x0400101E RID: 4126
	private bool turnOffLights;

	// Token: 0x0400101F RID: 4127
	private bool turningOffLights;

	// Token: 0x04001020 RID: 4128
	private bool turningOffEmissions;

	// Token: 0x04001021 RID: 4129
	private float logicUpdateTimer;

	// Token: 0x04001022 RID: 4130
	private bool setup;

	// Token: 0x04001023 RID: 4131
	private bool debugActive;
}
