using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x0200018B RID: 395
public class MapBacktrack : MonoBehaviour
{
	// Token: 0x06000D7B RID: 3451 RVA: 0x00076480 File Offset: 0x00074680
	private void Start()
	{
		this.path = new NavMeshPath();
		for (int i = 0; i < this.amount; i++)
		{
			GameObject gameObject = Object.Instantiate<GameObject>(this.pointPrefab, base.transform);
			this.points.Add(gameObject.GetComponent<MapBacktrackPoint>());
			gameObject.transform.name = string.Format("Point {0}", i);
		}
		base.StartCoroutine(this.Backtrack());
	}

	// Token: 0x06000D7C RID: 3452 RVA: 0x000764F4 File Offset: 0x000746F4
	private IEnumerator Backtrack()
	{
		while (!LevelGenerator.Instance.Generated)
		{
			yield return new WaitForSeconds(0.1f);
		}
		yield return new WaitForSeconds(0.5f);
		foreach (LevelPoint levelPoint in LevelGenerator.Instance.LevelPathPoints)
		{
			if (levelPoint.Room.Truck)
			{
				this.truckDestination = levelPoint.transform.position;
				break;
			}
		}
		for (;;)
		{
			Vector3 lastNavmeshPosition = PlayerController.instance.playerAvatarScript.LastNavmeshPosition;
			Vector3 vector = lastNavmeshPosition;
			if (RoundDirector.instance.allExtractionPointsCompleted)
			{
				vector = this.truckDestination;
			}
			else if (RoundDirector.instance.extractionPointCurrent)
			{
				vector = RoundDirector.instance.extractionPointCurrent.transform.position;
			}
			bool flag = false;
			if (Map.Instance.Active)
			{
				MapLayer layerParent = Map.Instance.GetLayerParent(lastNavmeshPosition.y + 1f);
				MapLayer layerParent2 = Map.Instance.GetLayerParent(vector.y + 1f);
				if (layerParent.layer == layerParent2.layer)
				{
					flag = true;
				}
			}
			if (!Map.Instance.Active || (flag && Vector3.Distance(lastNavmeshPosition, vector) < 10f))
			{
				yield return new WaitForSeconds(0.25f);
			}
			else
			{
				NavMesh.CalculatePath(lastNavmeshPosition, vector, -1, this.path);
				this.currentPoint = 0;
				this.currentPointPosition = lastNavmeshPosition;
				this.currentPointCorner = 0;
				while (this.currentPoint < this.points.Count)
				{
					bool flag2 = false;
					float num = this.spacing;
					while (!flag2 && this.currentPointCorner < this.path.corners.Length)
					{
						float num2 = Vector3.Distance(this.currentPointPosition, this.path.corners[this.currentPointCorner]);
						if (num2 < num)
						{
							this.currentPointPosition = this.path.corners[this.currentPointCorner];
							num -= num2;
							this.currentPointCorner++;
						}
						else
						{
							this.currentPointPosition = Vector3.Lerp(this.currentPointPosition, this.path.corners[this.currentPointCorner], num / num2);
							if (Map.Instance.GetLayerParent(this.currentPointPosition.y + 1f).layer == Map.Instance.PlayerLayer)
							{
								this.points[this.currentPoint].Show(true);
							}
							else
							{
								this.points[this.currentPoint].Show(false);
							}
							Vector3 a = new Vector3(this.currentPointPosition.x, 0f, this.currentPointPosition.z);
							this.points[this.currentPoint].transform.position = a * Map.Instance.Scale + Map.Instance.OverLayerParent.position;
							this.currentPoint++;
							flag2 = true;
						}
					}
					if (this.currentPointCorner >= this.path.corners.Length)
					{
						this.currentPoint = this.points.Count;
					}
					yield return new WaitForSeconds(this.pointWait);
				}
				foreach (MapBacktrackPoint _point in this.points)
				{
					while (_point.animating)
					{
						yield return new WaitForSeconds(0.05f);
					}
					_point = null;
				}
				List<MapBacktrackPoint>.Enumerator enumerator2 = default(List<MapBacktrackPoint>.Enumerator);
				yield return new WaitForSeconds(this.pointWait);
			}
		}
		yield break;
		yield break;
	}

	// Token: 0x0400159D RID: 5533
	public GameObject pointPrefab;

	// Token: 0x0400159E RID: 5534
	private List<MapBacktrackPoint> points = new List<MapBacktrackPoint>();

	// Token: 0x0400159F RID: 5535
	[Space]
	public int amount;

	// Token: 0x040015A0 RID: 5536
	public float spacing;

	// Token: 0x040015A1 RID: 5537
	public float pointWait;

	// Token: 0x040015A2 RID: 5538
	public float resetWait;

	// Token: 0x040015A3 RID: 5539
	private int currentPoint;

	// Token: 0x040015A4 RID: 5540
	private Vector3 currentPointPosition;

	// Token: 0x040015A5 RID: 5541
	private int currentPointCorner;

	// Token: 0x040015A6 RID: 5542
	private Vector3 truckDestination;

	// Token: 0x040015A7 RID: 5543
	private NavMeshPath path;
}
