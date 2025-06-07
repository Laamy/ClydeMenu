using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020000D8 RID: 216
public class ArenaPlatform : MonoBehaviour
{
	// Token: 0x06000794 RID: 1940 RVA: 0x00048389 File Offset: 0x00046589
	private void Start()
	{
		this.lights = new List<ArenaLight>();
		this.lights.AddRange(base.GetComponentsInChildren<ArenaLight>());
		this.meshRenderer.material.SetColor("_EmissionColor", Color.black);
	}

	// Token: 0x06000795 RID: 1941 RVA: 0x000483C1 File Offset: 0x000465C1
	private void StateIdle()
	{
		if (this.stateStart)
		{
			this.stateStart = false;
		}
	}

	// Token: 0x06000796 RID: 1942 RVA: 0x000483D4 File Offset: 0x000465D4
	private void StateWarning()
	{
		if (this.stateStart)
		{
			this.stateStart = false;
			this.lights.ForEach(delegate(ArenaLight light)
			{
				light.TurnOnArenaWarningLight();
			});
			this.meshRenderer.material.SetColor("_EmissionColor", Color.red);
		}
		Color b = new Color(0.3f, 0f, 0f);
		this.meshRenderer.material.SetColor("_EmissionColor", Color.Lerp(this.meshRenderer.material.GetColor("_EmissionColor"), b, Time.deltaTime * 2f));
	}

	// Token: 0x06000797 RID: 1943 RVA: 0x00048488 File Offset: 0x00046688
	private void StateGoDown()
	{
		if (this.stateStart)
		{
			DirtFinderMapFloor[] array = this.map;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].MapObject.Hide();
			}
			this.stateStart = false;
		}
		if (base.transform.position.y > -60f)
		{
			base.transform.position = new Vector3(base.transform.position.x, base.transform.position.y - 30f * Time.deltaTime, base.transform.position.z);
			return;
		}
		this.StateSet(ArenaPlatform.States.End);
	}

	// Token: 0x06000798 RID: 1944 RVA: 0x00048531 File Offset: 0x00046731
	private void StateEnd()
	{
		if (this.stateStart)
		{
			this.stateStart = false;
		}
		Object.Destroy(base.gameObject);
	}

	// Token: 0x06000799 RID: 1945 RVA: 0x00048550 File Offset: 0x00046750
	private void StateMachine()
	{
		switch (this.currentState)
		{
		case ArenaPlatform.States.Idle:
			this.StateIdle();
			return;
		case ArenaPlatform.States.Warning:
			this.StateWarning();
			return;
		case ArenaPlatform.States.GoDown:
			this.StateGoDown();
			return;
		case ArenaPlatform.States.End:
			this.StateEnd();
			return;
		default:
			return;
		}
	}

	// Token: 0x0600079A RID: 1946 RVA: 0x00048596 File Offset: 0x00046796
	private void Update()
	{
		this.StateMachine();
		if (this.stateTimer > 0f)
		{
			this.stateTimer -= Time.deltaTime;
		}
	}

	// Token: 0x0600079B RID: 1947 RVA: 0x000485C0 File Offset: 0x000467C0
	public void PulsateLights()
	{
		this.lights.ForEach(delegate(ArenaLight light)
		{
			light.PulsateLight();
		});
		this.meshRenderer.material.SetColor("_EmissionColor", Color.red);
	}

	// Token: 0x0600079C RID: 1948 RVA: 0x00048611 File Offset: 0x00046811
	public void StateSet(ArenaPlatform.States state)
	{
		this.currentState = state;
		this.stateStart = true;
	}

	// Token: 0x04000D65 RID: 3429
	private List<ArenaLight> lights;

	// Token: 0x04000D66 RID: 3430
	internal ArenaPlatform.States currentState;

	// Token: 0x04000D67 RID: 3431
	private bool stateStart;

	// Token: 0x04000D68 RID: 3432
	private float stateTimer;

	// Token: 0x04000D69 RID: 3433
	public MeshRenderer meshRenderer;

	// Token: 0x04000D6A RID: 3434
	[Space]
	public DirtFinderMapFloor[] map;

	// Token: 0x0200033B RID: 827
	public enum States
	{
		// Token: 0x040029CB RID: 10699
		Idle,
		// Token: 0x040029CC RID: 10700
		Warning,
		// Token: 0x040029CD RID: 10701
		GoDown,
		// Token: 0x040029CE RID: 10702
		End
	}
}
