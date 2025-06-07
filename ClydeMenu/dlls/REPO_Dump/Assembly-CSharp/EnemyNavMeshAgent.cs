using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x020000A1 RID: 161
[RequireComponent(typeof(NavMeshAgent))]
public class EnemyNavMeshAgent : MonoBehaviour
{
	// Token: 0x06000664 RID: 1636 RVA: 0x0003E280 File Offset: 0x0003C480
	private void Awake()
	{
		this.Agent = base.GetComponent<NavMeshAgent>();
		if (!this.updateRotation)
		{
			this.Agent.updateRotation = false;
		}
		if (GameManager.instance.gameMode == 0 || PhotonNetwork.IsMasterClient)
		{
			this.Agent.enabled = true;
		}
		else
		{
			this.Agent.enabled = false;
		}
		this.DefaultSpeed = this.Agent.speed;
		this.DefaultAcceleration = this.Agent.acceleration;
	}

	// Token: 0x06000665 RID: 1637 RVA: 0x0003E2FC File Offset: 0x0003C4FC
	private void Update()
	{
		this.AgentVelocity = this.Agent.velocity;
		if (this.SetPathTimer > 0f)
		{
			this.SetPathTimer -= Time.deltaTime;
		}
		if (this.DisableTimer > 0f)
		{
			this.Agent.enabled = false;
			this.DisableTimer -= Time.deltaTime;
			return;
		}
		if (!this.Agent.enabled)
		{
			this.Agent.enabled = true;
		}
		if (this.StopTimer > 0f)
		{
			if (this.Agent.enabled)
			{
				this.Agent.isStopped = true;
			}
			this.StopTimer -= Time.deltaTime;
		}
		else if (this.Agent.enabled && this.Agent.isStopped)
		{
			this.Agent.isStopped = false;
		}
		if (this.OverrideTimer > 0f)
		{
			this.OverrideTimer -= Time.deltaTime;
			if (this.OverrideTimer <= 0f)
			{
				this.Agent.speed = this.DefaultSpeed;
				this.Agent.acceleration = this.DefaultAcceleration;
			}
		}
	}

	// Token: 0x06000666 RID: 1638 RVA: 0x0003E42C File Offset: 0x0003C62C
	public void OverrideAgent(float speed, float acceleration, float time)
	{
		this.Agent.speed = speed;
		this.Agent.acceleration = acceleration;
		this.OverrideTimer = time;
	}

	// Token: 0x06000667 RID: 1639 RVA: 0x0003E44D File Offset: 0x0003C64D
	public void UpdateAgent(float speed, float acceleration)
	{
		this.Agent.speed = speed;
		this.Agent.acceleration = acceleration;
	}

	// Token: 0x06000668 RID: 1640 RVA: 0x0003E468 File Offset: 0x0003C668
	public void AgentMove(Vector3 position)
	{
		Vector3 velocity = this.Agent.velocity;
		Vector3 destination = this.Agent.destination;
		if (!this.OnNavmesh(position))
		{
			return;
		}
		this.Warp(position);
		this.SetDestination(destination);
		this.Agent.velocity = velocity;
	}

	// Token: 0x06000669 RID: 1641 RVA: 0x0003E4B4 File Offset: 0x0003C6B4
	private bool OnNavmesh(Vector3 position)
	{
		NavMeshHit navMeshHit;
		return NavMesh.SamplePosition(position, out navMeshHit, 5f, -1);
	}

	// Token: 0x0600066A RID: 1642 RVA: 0x0003E4D0 File Offset: 0x0003C6D0
	public void Warp(Vector3 position)
	{
		if (Vector3.Distance(base.transform.position, position) < 1f)
		{
			return;
		}
		if (this.DisableTimer > 0f)
		{
			this.Agent.enabled = true;
		}
		if (!this.OnNavmesh(position))
		{
			return;
		}
		this.Agent.Warp(position);
		if (this.DisableTimer > 0f)
		{
			this.Agent.enabled = false;
		}
	}

	// Token: 0x0600066B RID: 1643 RVA: 0x0003E53F File Offset: 0x0003C73F
	public void ResetPath()
	{
		if (!this.Agent.enabled)
		{
			return;
		}
		if (!this.HasPath())
		{
			return;
		}
		this.Agent.ResetPath();
	}

	// Token: 0x0600066C RID: 1644 RVA: 0x0003E563 File Offset: 0x0003C763
	public bool CanReach(Vector3 _target, float _range)
	{
		return !this.Agent.enabled || !this.Agent.hasPath || Vector3.Distance(this.GetPoint(), _target) <= _range;
	}

	// Token: 0x0600066D RID: 1645 RVA: 0x0003E595 File Offset: 0x0003C795
	public void SetDestination(Vector3 position)
	{
		if (!this.Agent.enabled)
		{
			return;
		}
		if (!this.Agent.hasPath)
		{
			this.SetPathTimer = 0.1f;
		}
		this.Agent.SetDestination(position);
	}

	// Token: 0x0600066E RID: 1646 RVA: 0x0003E5CA File Offset: 0x0003C7CA
	public void Stop(float time)
	{
		if (!this.Agent.enabled)
		{
			return;
		}
		this.StopTimer = time;
		if (this.StopTimer == 0f)
		{
			this.Agent.isStopped = false;
			return;
		}
		this.Agent.isStopped = true;
	}

	// Token: 0x0600066F RID: 1647 RVA: 0x0003E607 File Offset: 0x0003C807
	public bool IsStopped()
	{
		return this.StopTimer > 0f;
	}

	// Token: 0x06000670 RID: 1648 RVA: 0x0003E619 File Offset: 0x0003C819
	public void Disable(float time)
	{
		this.Agent.enabled = false;
		this.DisableTimer = time;
	}

	// Token: 0x06000671 RID: 1649 RVA: 0x0003E62E File Offset: 0x0003C82E
	public void Enable()
	{
		if (this.DisableTimer > 0f)
		{
			this.Agent.enabled = true;
			this.DisableTimer = 0f;
		}
	}

	// Token: 0x06000672 RID: 1650 RVA: 0x0003E654 File Offset: 0x0003C854
	public bool IsDisabled()
	{
		return this.DisableTimer > 0f;
	}

	// Token: 0x06000673 RID: 1651 RVA: 0x0003E668 File Offset: 0x0003C868
	public Vector3 GetPoint()
	{
		if (this.Agent.hasPath)
		{
			return this.Agent.path.corners[this.Agent.path.corners.Length - 1];
		}
		return new Vector3(-1000f, 1000f, 1000f);
	}

	// Token: 0x06000674 RID: 1652 RVA: 0x0003E6C0 File Offset: 0x0003C8C0
	public Vector3 GetDestination()
	{
		if (this.Agent.hasPath)
		{
			return this.Agent.destination;
		}
		return base.transform.position;
	}

	// Token: 0x06000675 RID: 1653 RVA: 0x0003E6E6 File Offset: 0x0003C8E6
	public bool HasPath()
	{
		return this.SetPathTimer > 0f || this.Agent.hasPath;
	}

	// Token: 0x06000676 RID: 1654 RVA: 0x0003E708 File Offset: 0x0003C908
	public NavMeshPath CalculatePath(Vector3 position)
	{
		NavMeshPath navMeshPath = new NavMeshPath();
		if (!this.Agent.enabled)
		{
			return navMeshPath;
		}
		this.Agent.CalculatePath(position, navMeshPath);
		return navMeshPath;
	}

	// Token: 0x04000A98 RID: 2712
	internal NavMeshAgent Agent;

	// Token: 0x04000A99 RID: 2713
	internal Vector3 AgentVelocity;

	// Token: 0x04000A9A RID: 2714
	public bool updateRotation;

	// Token: 0x04000A9B RID: 2715
	private float StopTimer;

	// Token: 0x04000A9C RID: 2716
	private float DisableTimer;

	// Token: 0x04000A9D RID: 2717
	internal float DefaultSpeed;

	// Token: 0x04000A9E RID: 2718
	internal float DefaultAcceleration;

	// Token: 0x04000A9F RID: 2719
	private float OverrideTimer;

	// Token: 0x04000AA0 RID: 2720
	private float SetPathTimer;
}
