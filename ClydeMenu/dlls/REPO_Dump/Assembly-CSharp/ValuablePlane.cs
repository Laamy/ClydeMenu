using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020002B9 RID: 697
public class ValuablePlane : MonoBehaviour
{
	// Token: 0x060015F9 RID: 5625 RVA: 0x000C15E1 File Offset: 0x000BF7E1
	private void Start()
	{
	}

	// Token: 0x060015FA RID: 5626 RVA: 0x000C15E3 File Offset: 0x000BF7E3
	private void Update()
	{
	}

	// Token: 0x04002614 RID: 9748
	[Header("Animation")]
	public Transform planeBody;

	// Token: 0x04002615 RID: 9749
	public Transform pilot;

	// Token: 0x04002616 RID: 9750
	public Transform propeller;

	// Token: 0x04002617 RID: 9751
	public Transform crank;

	// Token: 0x04002618 RID: 9752
	[Header("Sounds")]
	public Sound sfxPlaneStart;

	// Token: 0x04002619 RID: 9753
	public Sound sfxPlaneStop;

	// Token: 0x0400261A RID: 9754
	public Sound sfxPlaneLoop;

	// Token: 0x0400261B RID: 9755
	private bool grabbedPrev;

	// Token: 0x0400261C RID: 9756
	private bool loopPlaying;

	// Token: 0x0400261D RID: 9757
	private bool loopPlayingPrevious;

	// Token: 0x0400261E RID: 9758
	private float loopPitch;

	// Token: 0x0400261F RID: 9759
	[Header("Colliders")]
	public List<Collider> planeColliders;

	// Token: 0x04002620 RID: 9760
	private bool playersNearby;

	// Token: 0x04002621 RID: 9761
	public PhysicMaterial defaultPhysicMaterial;

	// Token: 0x04002622 RID: 9762
	public PhysicMaterial movingPhysicMaterial;

	// Token: 0x04002623 RID: 9763
	public Transform centerTransform;

	// Token: 0x04002624 RID: 9764
	private PhysGrabObjectImpactDetector impactDetector;

	// Token: 0x04002625 RID: 9765
	private Vector3 boxSize = new Vector3(0.16f, 0.1f, 0.42f);

	// Token: 0x04002626 RID: 9766
	private bool activated;

	// Token: 0x04002627 RID: 9767
	private ValuablePlane.State currentState;

	// Token: 0x04002628 RID: 9768
	private bool stateImpulse;

	// Token: 0x04002629 RID: 9769
	private float stateTimer;

	// Token: 0x02000418 RID: 1048
	public enum State
	{
		// Token: 0x04002DD0 RID: 11728
		Inactive,
		// Token: 0x04002DD1 RID: 11729
		Idle,
		// Token: 0x04002DD2 RID: 11730
		MoveForward
	}
}
