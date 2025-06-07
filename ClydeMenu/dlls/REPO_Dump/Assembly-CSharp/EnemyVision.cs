using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020000A7 RID: 167
public class EnemyVision : MonoBehaviour
{
	// Token: 0x060006A9 RID: 1705 RVA: 0x00040812 File Offset: 0x0003EA12
	private void Awake()
	{
		this.Enemy = base.GetComponent<Enemy>();
		base.StartCoroutine(this.Vision());
		this.VisionLogicActive = true;
	}

	// Token: 0x060006AA RID: 1706 RVA: 0x00040834 File Offset: 0x0003EA34
	private void OnDisable()
	{
		base.StopAllCoroutines();
		this.VisionLogicActive = false;
	}

	// Token: 0x060006AB RID: 1707 RVA: 0x00040843 File Offset: 0x0003EA43
	private void OnEnable()
	{
		if (!this.VisionLogicActive)
		{
			base.StartCoroutine(this.Vision());
			this.VisionLogicActive = true;
		}
	}

	// Token: 0x060006AC RID: 1708 RVA: 0x00040861 File Offset: 0x0003EA61
	public void PlayerAdded(int photonID)
	{
		this.VisionsTriggered.TryAdd(photonID, 0);
		this.VisionTriggered.TryAdd(photonID, false);
	}

	// Token: 0x060006AD RID: 1709 RVA: 0x0004087F File Offset: 0x0003EA7F
	public void PlayerRemoved(int photonID)
	{
		this.VisionsTriggered.Remove(photonID);
		this.VisionTriggered.Remove(photonID);
	}

	// Token: 0x060006AE RID: 1710 RVA: 0x0004089B File Offset: 0x0003EA9B
	private IEnumerator Vision()
	{
		this.VisionLogicActive = true;
		if (GameManager.Multiplayer() && !PhotonNetwork.IsMasterClient)
		{
			yield break;
		}
		while (this.VisionsTriggered.Count == 0)
		{
			yield return new WaitForSeconds(this.VisionCheckTime);
		}
		for (;;)
		{
			if (this.DisableTimer <= 0f && !EnemyDirector.instance.debugNoVision)
			{
				if (!this.Enemy.HasStateChaseBegin || this.Enemy.CurrentState != EnemyState.ChaseBegin)
				{
					this.HasVision = false;
					bool[] array = new bool[GameDirector.instance.PlayerList.Count];
					if (this.PhysObjectVision)
					{
						float radius = this.PhysObjectVisionRadius;
						if (this.PhysObjectVisionRadiusOverride > 0f)
						{
							radius = this.PhysObjectVisionRadiusOverride;
						}
						foreach (Collider collider in Physics.OverlapSphere(this.VisionTransform.position, radius, SemiFunc.LayerMaskGetPhysGrabObject()))
						{
							if (collider.CompareTag("Phys Grab Object"))
							{
								PhysGrabObject componentInParent = collider.GetComponentInParent<PhysGrabObject>();
								if (componentInParent && componentInParent.playerGrabbing.Count > 0)
								{
									Vector3 direction = componentInParent.centerPoint - this.VisionTransform.position;
									RaycastHit[] array3 = Physics.RaycastAll(this.VisionTransform.position, direction, direction.magnitude, this.Enemy.VisionMask);
									bool flag = true;
									if (array3.Length != 0)
									{
										RaycastHit[] array4 = array3;
										int j = 0;
										while (j < array4.Length)
										{
											RaycastHit raycastHit = array4[j];
											if (!raycastHit.transform.CompareTag("Phys Grab Object") && !raycastHit.transform.CompareTag("Enemy"))
											{
												goto IL_2B7;
											}
											PhysGrabObject componentInParent2 = raycastHit.transform.GetComponentInParent<PhysGrabObject>();
											if (!componentInParent2 || (!(componentInParent2 == componentInParent) && (!this.Enemy.HasRigidbody || !(raycastHit.transform.GetComponentInParent<EnemyRigidbody>() == this.Enemy.Rigidbody))))
											{
												goto IL_2B7;
											}
											IL_2BA:
											j++;
											continue;
											IL_2B7:
											flag = false;
											goto IL_2BA;
										}
									}
									if (flag && Vector3.Dot(this.VisionTransform.forward, direction.normalized) >= this.PhysObjectVisionDot)
									{
										int num = 0;
										using (List<PlayerAvatar>.Enumerator enumerator = GameDirector.instance.PlayerList.GetEnumerator())
										{
											while (enumerator.MoveNext())
											{
												if (enumerator.Current == componentInParent.playerGrabbing[0].playerAvatar)
												{
													array[num] = true;
												}
												num++;
											}
										}
									}
								}
							}
						}
					}
					int num2 = 0;
					foreach (PlayerAvatar playerAvatar in GameDirector.instance.PlayerList)
					{
						bool flag2 = false;
						if (!playerAvatar.isDisabled)
						{
							int viewID = playerAvatar.photonView.ViewID;
							if (playerAvatar.enemyVisionFreezeTimer > 0f)
							{
								if (this.VisionTriggered[viewID])
								{
									this.VisionTrigger(viewID, playerAvatar, false, false);
								}
								num2++;
							}
							else
							{
								this.VisionTriggered[viewID] = false;
								float num3 = Vector3.Distance(this.VisionTransform.position, playerAvatar.transform.position);
								if (array[num2] || num3 <= this.VisionDistance)
								{
									bool flag3 = playerAvatar.isCrawling;
									bool flag4 = playerAvatar.isCrouching;
									if (playerAvatar.isTumbling)
									{
										flag4 = true;
										flag3 = false;
									}
									if (this.StandOverrideTimer > 0f)
									{
										flag4 = false;
										flag3 = false;
									}
									Transform transform = null;
									Transform transform2 = null;
									Vector3 direction2 = playerAvatar.PlayerVisionTarget.VisionTransform.transform.position - this.VisionTransform.position;
									Collider[] array5 = Physics.OverlapSphere(this.VisionTransform.position, 0.01f, this.Enemy.VisionMask);
									if (array5.Length != 0)
									{
										foreach (Collider collider2 in array5)
										{
											if (!collider2.transform.CompareTag("Enemy"))
											{
												if (collider2.transform.CompareTag("Player"))
												{
													transform = collider2.transform;
												}
												if (collider2.transform.GetComponentInParent<PlayerTumble>())
												{
													transform = collider2.transform;
												}
												if (!collider2.transform.GetComponentInParent<EnemyRigidbody>())
												{
													transform2 = collider2.transform;
												}
											}
										}
									}
									if (!transform2)
									{
										RaycastHit[] array6 = Physics.RaycastAll(this.VisionTransform.position, direction2, this.VisionDistance, this.Enemy.VisionMask);
										float num4 = 1000f;
										foreach (RaycastHit raycastHit2 in array6)
										{
											if (!raycastHit2.transform.CompareTag("Enemy"))
											{
												if (raycastHit2.transform.CompareTag("Player"))
												{
													transform = raycastHit2.transform;
												}
												if (!raycastHit2.transform.GetComponentInParent<EnemyRigidbody>())
												{
													if (raycastHit2.transform.GetComponentInParent<PlayerTumble>())
													{
														transform = raycastHit2.transform;
													}
													float num5 = Vector3.Distance(this.VisionTransform.position, raycastHit2.point);
													if (num5 < num4)
													{
														num4 = num5;
														transform2 = raycastHit2.transform;
													}
												}
											}
										}
									}
									if (array[num2] || (transform && transform == transform2))
									{
										float num6 = Vector3.Dot(this.VisionTransform.forward, direction2.normalized);
										bool flag5 = false;
										if (flag4)
										{
											if (num3 <= this.VisionDistanceCloseCrouch)
											{
												flag5 = true;
											}
										}
										else if (num3 <= this.VisionDistanceClose)
										{
											flag5 = true;
										}
										if (flag5)
										{
											this.VisionsTriggered[viewID] = this.VisionsToTrigger;
										}
										bool flag6 = false;
										if (flag3 && this.Enemy.CurrentState != EnemyState.LookUnder)
										{
											if (num6 >= this.VisionDotCrawl)
											{
												flag6 = true;
											}
										}
										else if (flag4 && this.Enemy.CurrentState != EnemyState.LookUnder)
										{
											if (num6 >= this.VisionDotCrouch)
											{
												flag6 = true;
											}
										}
										else if (num6 >= this.VisionDotStanding)
										{
											flag6 = true;
										}
										if (array[num2] || flag6 || flag5)
										{
											flag2 = true;
											bool flag7 = false;
											if (flag3 && this.Enemy.CurrentState != EnemyState.LookUnder)
											{
												if (this.VisionsTriggered[viewID] >= this.VisionsToTriggerCrawl)
												{
													flag7 = true;
												}
											}
											else if (flag4 && this.Enemy.CurrentState != EnemyState.LookUnder)
											{
												if (this.VisionsTriggered[viewID] >= this.VisionsToTriggerCrouch)
												{
													flag7 = true;
												}
											}
											else if (this.VisionsTriggered[viewID] >= this.VisionsToTrigger)
											{
												flag7 = true;
											}
											bool culled = false;
											if (this.Enemy.HasOnScreen)
											{
												if (GameManager.instance.gameMode == 0)
												{
													if (this.Enemy.OnScreen.CulledLocal)
													{
														culled = true;
													}
												}
												else if (this.Enemy.OnScreen.CulledPlayer[playerAvatar.photonView.ViewID])
												{
													culled = true;
												}
											}
											if (flag7 || flag5)
											{
												this.VisionTrigger(viewID, playerAvatar, culled, flag5);
											}
										}
									}
									if (flag2)
									{
										Dictionary<int, int> visionsTriggered = this.VisionsTriggered;
										int i = viewID;
										int j = visionsTriggered[i];
										visionsTriggered[i] = j + 1;
									}
									else
									{
										this.VisionsTriggered[viewID] = 0;
									}
									num2++;
								}
							}
						}
					}
				}
				if (this.StandOverrideTimer > 0f)
				{
					this.StandOverrideTimer -= this.VisionCheckTime;
				}
				yield return new WaitForSeconds(this.VisionCheckTime);
			}
			else
			{
				this.DisableTimer -= Time.deltaTime;
				foreach (PlayerAvatar playerAvatar2 in GameDirector.instance.PlayerList)
				{
					int viewID2 = playerAvatar2.photonView.ViewID;
					this.VisionTriggered[viewID2] = false;
					this.VisionsTriggered[viewID2] = 0;
				}
				yield return null;
			}
		}
		yield break;
	}

	// Token: 0x060006AF RID: 1711 RVA: 0x000408AC File Offset: 0x0003EAAC
	public void VisionTrigger(int playerID, PlayerAvatar player, bool culled, bool playerNear)
	{
		this.VisionTriggered[playerID] = true;
		this.VisionsTriggered[playerID] = Mathf.Max(this.VisionsTriggered[playerID], this.VisionsToTrigger);
		this.onVisionTriggeredID = playerID;
		this.onVisionTriggeredPlayer = player;
		this.onVisionTriggeredCulled = culled;
		this.onVisionTriggeredNear = playerNear;
		this.onVisionTriggered.Invoke();
	}

	// Token: 0x060006B0 RID: 1712 RVA: 0x00040911 File Offset: 0x0003EB11
	public void DisableVision(float time)
	{
		this.DisableTimer = time;
	}

	// Token: 0x060006B1 RID: 1713 RVA: 0x0004091A File Offset: 0x0003EB1A
	public void StandOverride(float time)
	{
		this.StandOverrideTimer = time;
	}

	// Token: 0x04000B28 RID: 2856
	private Enemy Enemy;

	// Token: 0x04000B29 RID: 2857
	internal bool HasVision;

	// Token: 0x04000B2A RID: 2858
	internal float DisableTimer;

	// Token: 0x04000B2B RID: 2859
	internal float StandOverrideTimer;

	// Token: 0x04000B2C RID: 2860
	private float VisionTimer;

	// Token: 0x04000B2D RID: 2861
	private float VisionCheckTime = 0.25f;

	// Token: 0x04000B2E RID: 2862
	public Transform VisionTransform;

	// Token: 0x04000B2F RID: 2863
	[Header("Base")]
	public float VisionDistance = 10f;

	// Token: 0x04000B30 RID: 2864
	public Dictionary<int, int> VisionsTriggered = new Dictionary<int, int>();

	// Token: 0x04000B31 RID: 2865
	public Dictionary<int, bool> VisionTriggered = new Dictionary<int, bool>();

	// Token: 0x04000B32 RID: 2866
	[Header("Close")]
	public float VisionDistanceClose = 3.5f;

	// Token: 0x04000B33 RID: 2867
	public float VisionDistanceCloseCrouch = 2f;

	// Token: 0x04000B34 RID: 2868
	[Header("Dot")]
	public float VisionDotStanding = 0.4f;

	// Token: 0x04000B35 RID: 2869
	public float VisionDotCrouch = 0.6f;

	// Token: 0x04000B36 RID: 2870
	public float VisionDotCrawl = 0.9f;

	// Token: 0x04000B37 RID: 2871
	[Header("Phys Object Vision")]
	public bool PhysObjectVision = true;

	// Token: 0x04000B38 RID: 2872
	private float PhysObjectVisionRadius = 10f;

	// Token: 0x04000B39 RID: 2873
	public float PhysObjectVisionRadiusOverride = -1f;

	// Token: 0x04000B3A RID: 2874
	public float PhysObjectVisionDot = 0.4f;

	// Token: 0x04000B3B RID: 2875
	[Header("Triggers")]
	public int VisionsToTrigger = 4;

	// Token: 0x04000B3C RID: 2876
	public int VisionsToTriggerCrouch = 10;

	// Token: 0x04000B3D RID: 2877
	public int VisionsToTriggerCrawl = 20;

	// Token: 0x04000B3E RID: 2878
	[Header("Events")]
	public UnityEvent onVisionTriggered;

	// Token: 0x04000B3F RID: 2879
	internal int onVisionTriggeredID;

	// Token: 0x04000B40 RID: 2880
	internal PlayerAvatar onVisionTriggeredPlayer;

	// Token: 0x04000B41 RID: 2881
	internal bool onVisionTriggeredCulled;

	// Token: 0x04000B42 RID: 2882
	internal bool onVisionTriggeredNear;

	// Token: 0x04000B43 RID: 2883
	internal float onVisionTriggeredDistance;

	// Token: 0x04000B44 RID: 2884
	private bool VisionLogicActive;
}
