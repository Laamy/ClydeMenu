using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x02000195 RID: 405
public class Map : MonoBehaviour
{
	// Token: 0x06000D9B RID: 3483 RVA: 0x000768ED File Offset: 0x00074AED
	private void Awake()
	{
		Map.Instance = this;
	}

	// Token: 0x06000D9C RID: 3484 RVA: 0x000768F5 File Offset: 0x00074AF5
	private void Start()
	{
		this.playerTransformSource = PlayerController.instance.transform;
		this.ActiveSet(false);
	}

	// Token: 0x06000D9D RID: 3485 RVA: 0x00076910 File Offset: 0x00074B10
	private void Update()
	{
		if (this.Active != this.ActivePrevious)
		{
			if (!this.Active)
			{
				foreach (MapLayer mapLayer in this.Layers)
				{
					mapLayer.transform.position = mapLayer.positionStart;
				}
			}
			this.ActivePrevious = this.Active;
		}
		if (this.Active)
		{
			foreach (MapLayer mapLayer2 in this.Layers)
			{
				if (mapLayer2.layer == this.PlayerLayer)
				{
					mapLayer2.transform.localPosition = new Vector3(mapLayer2.transform.localPosition.x, 0f, mapLayer2.transform.localPosition.z);
				}
				else if (mapLayer2.layer == this.PlayerLayer - 1)
				{
					mapLayer2.transform.localPosition = new Vector3(mapLayer2.transform.localPosition.x, this.GetLayerPosition(2).y, mapLayer2.transform.localPosition.z);
				}
				else if (mapLayer2.layer == this.PlayerLayer + 1)
				{
					mapLayer2.transform.localPosition = new Vector3(mapLayer2.transform.localPosition.x, this.GetLayerPosition(3).y, mapLayer2.transform.localPosition.z);
				}
				else
				{
					mapLayer2.transform.localPosition = new Vector3(mapLayer2.transform.localPosition.x, -5f, mapLayer2.transform.localPosition.z);
				}
			}
		}
	}

	// Token: 0x06000D9E RID: 3486 RVA: 0x00076B10 File Offset: 0x00074D10
	public void ActiveSet(bool active)
	{
		this.Active = active;
		if (this.ActiveParent != null)
		{
			this.ActiveParent.SetActive(active);
		}
	}

	// Token: 0x06000D9F RID: 3487 RVA: 0x00076B33 File Offset: 0x00074D33
	public void EnemyPositionSet(Transform transformTarget, Transform transformSource)
	{
	}

	// Token: 0x06000DA0 RID: 3488 RVA: 0x00076B35 File Offset: 0x00074D35
	public void AddEnemy(Enemy enemy)
	{
	}

	// Token: 0x06000DA1 RID: 3489 RVA: 0x00076B38 File Offset: 0x00074D38
	public void CustomPositionSet(Transform transformTarget, Transform transformSource)
	{
		transformTarget.position = transformSource.transform.position * this.Scale + this.OverLayerParent.position;
		transformTarget.localPosition = new Vector3(transformTarget.localPosition.x, 0f, transformTarget.localPosition.z);
		transformTarget.localRotation = Quaternion.Euler(0f, transformSource.rotation.eulerAngles.y, 0f);
	}

	// Token: 0x06000DA2 RID: 3490 RVA: 0x00076BC0 File Offset: 0x00074DC0
	public void AddCustom(MapCustom mapCustom, Sprite sprite, Color color)
	{
		GameObject gameObject = Object.Instantiate<GameObject>(this.CustomObject, this.OverLayerParent.transform);
		gameObject.gameObject.name = mapCustom.gameObject.name;
		this.CustomPositionSet(gameObject.transform, mapCustom.transform);
		MapCustomEntity component = gameObject.GetComponent<MapCustomEntity>();
		component.Parent = mapCustom.transform;
		component.mapCustom = mapCustom;
		component.spriteRenderer.sprite = sprite;
		component.spriteRenderer.color = color;
		component.StartCoroutine(component.Logic());
		mapCustom.mapCustomEntity = component;
	}

	// Token: 0x06000DA3 RID: 3491 RVA: 0x00076C54 File Offset: 0x00074E54
	public void AddFloor(DirtFinderMapFloor floor)
	{
		GameObject gameObject = null;
		MapLayer layerParent = this.GetLayerParent(floor.transform.position.y);
		if (floor.Type == DirtFinderMapFloor.FloorType.Floor_1x1)
		{
			gameObject = Object.Instantiate<GameObject>(this.FloorObject1x1, layerParent.transform);
		}
		else if (floor.Type == DirtFinderMapFloor.FloorType.Floor_1x1_Diagonal)
		{
			gameObject = Object.Instantiate<GameObject>(this.FloorObject1x1Diagonal, layerParent.transform);
		}
		else if (floor.Type == DirtFinderMapFloor.FloorType.Floor_1x05)
		{
			gameObject = Object.Instantiate<GameObject>(this.FloorObject1x05, layerParent.transform);
		}
		else if (floor.Type == DirtFinderMapFloor.FloorType.Floor_1x05_Diagonal)
		{
			gameObject = Object.Instantiate<GameObject>(this.FloorObject1x05Diagonal, layerParent.transform);
		}
		else if (floor.Type == DirtFinderMapFloor.FloorType.Floor_1x05_Curve)
		{
			gameObject = Object.Instantiate<GameObject>(this.FloorObject1x05Curve, layerParent.transform);
		}
		else if (floor.Type == DirtFinderMapFloor.FloorType.Floor_1x05_Curve_Inverted)
		{
			gameObject = Object.Instantiate<GameObject>(this.FloorObject1x05CurveInverted, layerParent.transform);
		}
		else if (floor.Type == DirtFinderMapFloor.FloorType.Floor_1x025)
		{
			gameObject = Object.Instantiate<GameObject>(this.FloorObject1x025, layerParent.transform);
		}
		else if (floor.Type == DirtFinderMapFloor.FloorType.Floor_1x025_Diagonal)
		{
			gameObject = Object.Instantiate<GameObject>(this.FloorObject1x025Diagonal, layerParent.transform);
		}
		else if (floor.Type == DirtFinderMapFloor.FloorType.Truck_Floor)
		{
			gameObject = Object.Instantiate<GameObject>(this.FloorTruck, layerParent.transform);
		}
		else if (floor.Type == DirtFinderMapFloor.FloorType.Truck_Wall)
		{
			gameObject = Object.Instantiate<GameObject>(this.WallTruck, layerParent.transform);
		}
		else if (floor.Type == DirtFinderMapFloor.FloorType.Used_Floor)
		{
			gameObject = Object.Instantiate<GameObject>(this.FloorUsed, layerParent.transform);
		}
		else if (floor.Type == DirtFinderMapFloor.FloorType.Used_Wall)
		{
			gameObject = Object.Instantiate<GameObject>(this.WallUsed, layerParent.transform);
		}
		else if (floor.Type == DirtFinderMapFloor.FloorType.Inactive_Floor)
		{
			gameObject = Object.Instantiate<GameObject>(this.FloorInactive, layerParent.transform);
		}
		else if (floor.Type == DirtFinderMapFloor.FloorType.Inactive_Wall)
		{
			gameObject = Object.Instantiate<GameObject>(this.WallInactive, layerParent.transform);
		}
		else if (floor.Type == DirtFinderMapFloor.FloorType.Floor_1x1_Curve)
		{
			gameObject = Object.Instantiate<GameObject>(this.FloorObject1x1Curve, layerParent.transform);
		}
		else if (floor.Type == DirtFinderMapFloor.FloorType.Floor_1x1_Curve_Inverted)
		{
			gameObject = Object.Instantiate<GameObject>(this.FloorObject1x1CurveInverted, layerParent.transform);
		}
		gameObject.gameObject.name = floor.gameObject.name;
		gameObject.transform.localScale = floor.transform.localScale;
		gameObject.transform.position = floor.transform.position * this.Scale + layerParent.transform.position + this.GetLayerPosition(layerParent.layer);
		gameObject.transform.rotation = floor.transform.rotation;
		this.MapObjectSetup(floor.gameObject, gameObject);
	}

	// Token: 0x06000DA4 RID: 3492 RVA: 0x00076F04 File Offset: 0x00075104
	public void AddWall(DirtFinderMapWall wall)
	{
		MapLayer layerParent = this.GetLayerParent(wall.transform.position.y);
		GameObject gameObject;
		if (wall.Type == DirtFinderMapWall.WallType.Door_1x1)
		{
			gameObject = Object.Instantiate<GameObject>(this.Door1x1Object, layerParent.transform);
		}
		else if (wall.Type == DirtFinderMapWall.WallType.Door_1x2)
		{
			gameObject = Object.Instantiate<GameObject>(this.Door1x2Object, layerParent.transform);
		}
		else if (wall.Type == DirtFinderMapWall.WallType.Door_Blocked)
		{
			gameObject = Object.Instantiate<GameObject>(this.DoorBlockedObject, layerParent.transform);
		}
		else if (wall.Type == DirtFinderMapWall.WallType.Door_Blocked_Wizard)
		{
			gameObject = Object.Instantiate<GameObject>(this.DoorBlockedWizardObject, layerParent.transform);
		}
		else if (wall.Type == DirtFinderMapWall.WallType.Door_Blocked_Arctic)
		{
			gameObject = Object.Instantiate<GameObject>(this.DoorBlockedArcticObject, layerParent.transform);
		}
		else if (wall.Type == DirtFinderMapWall.WallType.Stairs)
		{
			gameObject = Object.Instantiate<GameObject>(this.StairsObject, layerParent.transform);
		}
		else if (wall.Type == DirtFinderMapWall.WallType.Door_1x05)
		{
			gameObject = Object.Instantiate<GameObject>(this.Door1x05Object, layerParent.transform);
		}
		else if (wall.Type == DirtFinderMapWall.WallType.Door_1x1_Diagonal)
		{
			gameObject = Object.Instantiate<GameObject>(this.Door1x1DiagonalObject, layerParent.transform);
		}
		else if (wall.Type == DirtFinderMapWall.WallType.Door_1x05_Diagonal)
		{
			gameObject = Object.Instantiate<GameObject>(this.Door1x05DiagonalObject, layerParent.transform);
		}
		else if (wall.Type == DirtFinderMapWall.WallType.Wall_1x05)
		{
			gameObject = Object.Instantiate<GameObject>(this.Wall1x05Object, layerParent.transform);
		}
		else if (wall.Type == DirtFinderMapWall.WallType.Wall_1x025)
		{
			gameObject = Object.Instantiate<GameObject>(this.Wall1x025Object, layerParent.transform);
		}
		else if (wall.Type == DirtFinderMapWall.WallType.Wall_1x05_Diagonal)
		{
			gameObject = Object.Instantiate<GameObject>(this.Wall1x05DiagonalObject, layerParent.transform);
		}
		else if (wall.Type == DirtFinderMapWall.WallType.Wall_1x025_Diagonal)
		{
			gameObject = Object.Instantiate<GameObject>(this.Wall1x025DiagonalObject, layerParent.transform);
		}
		else if (wall.Type == DirtFinderMapWall.WallType.Wall_1x1_Diagonal)
		{
			gameObject = Object.Instantiate<GameObject>(this.Wall1x1DiagonalObject, layerParent.transform);
		}
		else if (wall.Type == DirtFinderMapWall.WallType.Door_1x1_Wizard)
		{
			gameObject = Object.Instantiate<GameObject>(this.Door1x1WizardObject, layerParent.transform);
		}
		else if (wall.Type == DirtFinderMapWall.WallType.Door_1x1_Arctic)
		{
			gameObject = Object.Instantiate<GameObject>(this.Door1x1ArcticObject, layerParent.transform);
		}
		else if (wall.Type == DirtFinderMapWall.WallType.Wall_1x1_Curve)
		{
			gameObject = Object.Instantiate<GameObject>(this.Wall1x1CurveObject, layerParent.transform);
		}
		else if (wall.Type == DirtFinderMapWall.WallType.Wall_1x05_Curve)
		{
			gameObject = Object.Instantiate<GameObject>(this.Wall1x05CurveObject, layerParent.transform);
		}
		else
		{
			gameObject = Object.Instantiate<GameObject>(this.Wall1x1Object, layerParent.transform);
		}
		gameObject.gameObject.name = wall.gameObject.name;
		gameObject.transform.position = wall.transform.position * this.Scale + layerParent.transform.position + this.GetLayerPosition(layerParent.layer);
		gameObject.transform.rotation = wall.transform.rotation;
		gameObject.transform.localScale = wall.transform.localScale;
		this.MapObjectSetup(wall.gameObject, gameObject);
	}

	// Token: 0x06000DA5 RID: 3493 RVA: 0x0007720C File Offset: 0x0007540C
	public MapModule AddRoomVolume(GameObject _parent, Vector3 _position, Quaternion _rotation, Vector3 _scale, Module _module, Mesh _mesh = null)
	{
		MapLayer component = this.OverLayerParent.GetComponent<MapLayer>();
		GameObject gameObject = Object.Instantiate<GameObject>(this.RoomVolume, component.transform);
		gameObject.gameObject.name = "Room Volume";
		gameObject.transform.position = _position * this.Scale + component.transform.position + this.GetLayerPosition(component.layer);
		gameObject.transform.localPosition = new Vector3(gameObject.transform.localPosition.x, 0f, gameObject.transform.localPosition.z);
		gameObject.transform.rotation = _rotation;
		gameObject.transform.localScale = _scale;
		gameObject.transform.localScale = new Vector3(gameObject.transform.localScale.x, 0.1f, gameObject.transform.localScale.z);
		if (_mesh)
		{
			gameObject.transform.localScale = new Vector3(gameObject.transform.localScale.x, 0.025f, gameObject.transform.localScale.z);
			gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y - 0.01f, gameObject.transform.position.z);
			gameObject.GetComponentInChildren<MeshFilter>().mesh = _mesh;
		}
		GameObject gameObject2 = Object.Instantiate<GameObject>(this.RoomVolumeOutline, component.transform);
		gameObject2.transform.position = gameObject.transform.position;
		gameObject2.transform.rotation = gameObject.transform.rotation;
		if (!_mesh)
		{
			gameObject2.transform.localScale = new Vector3(gameObject.transform.localScale.x + 0.25f, gameObject.transform.localScale.y, gameObject.transform.localScale.z + 0.25f);
		}
		else
		{
			gameObject2.transform.localScale = new Vector3(gameObject.transform.localScale.x, gameObject.transform.localScale.y, gameObject.transform.localScale.z);
		}
		if (_mesh)
		{
			MeshFilter componentInChildren = gameObject2.GetComponentInChildren<MeshFilter>();
			foreach (Map.RoomVolumeOutlineCustom roomVolumeOutlineCustom in this.RoomVolumeOutlineCustoms)
			{
				if (roomVolumeOutlineCustom.mesh == _mesh)
				{
					componentInChildren.mesh = roomVolumeOutlineCustom.meshOutline;
					break;
				}
			}
		}
		foreach (MapModule mapModule in this.MapModules)
		{
			if (mapModule.module == _module)
			{
				gameObject.transform.SetParent(mapModule.transform);
				gameObject2.transform.SetParent(mapModule.transform);
				return mapModule;
			}
		}
		GameObject gameObject3 = Object.Instantiate<GameObject>(this.ModulePrefab, component.transform);
		MapModule component2 = gameObject3.GetComponent<MapModule>();
		component2.module = _module;
		gameObject3.gameObject.name = _module.gameObject.name;
		gameObject3.transform.position = _module.transform.position * this.Scale + component.transform.position + this.GetLayerPosition(component.layer);
		this.MapModules.Add(component2);
		gameObject.transform.SetParent(gameObject3.transform);
		gameObject2.transform.SetParent(gameObject3.transform);
		return component2;
	}

	// Token: 0x06000DA6 RID: 3494 RVA: 0x00077608 File Offset: 0x00075808
	public void AddValuable(ValuableObject _valuable)
	{
		GameObject gameObject = Object.Instantiate<GameObject>(this.ValuableObject, this.OverLayerParent.transform);
		gameObject.gameObject.name = _valuable.gameObject.name;
		gameObject.transform.position = _valuable.transform.position * this.Scale + this.OverLayerParent.position;
		gameObject.transform.localPosition = new Vector3(gameObject.transform.localPosition.x, 0f, gameObject.transform.localPosition.z);
		MapValuable component = gameObject.GetComponent<MapValuable>();
		component.target = _valuable;
		if (_valuable.volumeType <= ValuableVolume.Type.Medium)
		{
			component.spriteRenderer.sprite = component.spriteSmall;
			return;
		}
		component.spriteRenderer.sprite = component.spriteBig;
	}

	// Token: 0x06000DA7 RID: 3495 RVA: 0x000776E4 File Offset: 0x000758E4
	public GameObject AddDoor(DirtFinderMapDoor door, GameObject doorPrefab)
	{
		MapLayer layerParent = this.GetLayerParent(door.transform.position.y);
		GameObject gameObject = Object.Instantiate<GameObject>(doorPrefab, layerParent.transform);
		gameObject.gameObject.name = door.gameObject.name;
		door.Target = gameObject.transform;
		DirtFinderMapDoorTarget component = gameObject.GetComponent<DirtFinderMapDoorTarget>();
		component.Target = door.transform;
		component.Layer = layerParent;
		this.DoorUpdate(component.HingeTransform, door.transform, layerParent);
		return gameObject;
	}

	// Token: 0x06000DA8 RID: 3496 RVA: 0x00077768 File Offset: 0x00075968
	public void DoorUpdate(Transform transformTarget, Transform transformSource, MapLayer _layer)
	{
		transformTarget.position = transformSource.transform.position * this.Scale + _layer.transform.position + this.GetLayerPosition(_layer.layer);
		transformTarget.rotation = transformSource.rotation;
	}

	// Token: 0x06000DA9 RID: 3497 RVA: 0x000777C0 File Offset: 0x000759C0
	public MapLayer GetLayerParent(float _positionY)
	{
		int num = Mathf.FloorToInt((_positionY + 0.1f) / this.LayerHeight);
		foreach (MapLayer mapLayer in this.Layers)
		{
			if (mapLayer.layer == num)
			{
				return mapLayer;
			}
		}
		GameObject gameObject = Object.Instantiate<GameObject>(this.LayerPrefab, base.transform);
		MapLayer component = gameObject.GetComponent<MapLayer>();
		component.layer = num;
		this.Layers.Add(component);
		gameObject.transform.localPosition = new Vector3(gameObject.transform.localPosition.x, this.LayerHeight * this.Scale * (float)num, gameObject.transform.localPosition.z);
		gameObject.name = "Layer " + num.ToString();
		this.Layers = Enumerable.ToList<MapLayer>(Enumerable.OrderBy<MapLayer, int>(this.Layers, (MapLayer x) => x.layer));
		this.Layers.Reverse();
		this.OverLayerParent.SetSiblingIndex(0);
		int num2 = 1;
		foreach (MapLayer mapLayer2 in this.Layers)
		{
			mapLayer2.transform.SetSiblingIndex(num2);
			num2++;
		}
		return component;
	}

	// Token: 0x06000DAA RID: 3498 RVA: 0x00077954 File Offset: 0x00075B54
	public Vector3 GetLayerPosition(int _layerIndex)
	{
		return new Vector3(0f, -(this.LayerHeight * this.Scale) * (float)_layerIndex, 0f);
	}

	// Token: 0x06000DAB RID: 3499 RVA: 0x00077978 File Offset: 0x00075B78
	private MapObject MapObjectSetup(GameObject _parent, GameObject _object)
	{
		MapObject component = _object.GetComponent<MapObject>();
		if (!component)
		{
			Debug.LogError("Map Object missing component!", _object);
		}
		else
		{
			component.parent = _parent.transform;
			DirtFinderMapFloor component2 = _parent.GetComponent<DirtFinderMapFloor>();
			if (component2)
			{
				component2.MapObject = component;
			}
		}
		return component;
	}

	// Token: 0x040015C7 RID: 5575
	public static Map Instance;

	// Token: 0x040015C8 RID: 5576
	public bool Active;

	// Token: 0x040015C9 RID: 5577
	public bool ActivePrevious;

	// Token: 0x040015CA RID: 5578
	public GameObject ActiveParent;

	// Token: 0x040015CB RID: 5579
	public int PlayerLayer;

	// Token: 0x040015CC RID: 5580
	[Space]
	public GameObject LayerPrefab;

	// Token: 0x040015CD RID: 5581
	public GameObject ModulePrefab;

	// Token: 0x040015CE RID: 5582
	public Transform OverLayerParent;

	// Token: 0x040015CF RID: 5583
	[Space]
	public List<MapLayer> Layers = new List<MapLayer>();

	// Token: 0x040015D0 RID: 5584
	public List<MapModule> MapModules = new List<MapModule>();

	// Token: 0x040015D1 RID: 5585
	[Space]
	public GameObject EnemyObject;

	// Token: 0x040015D2 RID: 5586
	public GameObject CustomObject;

	// Token: 0x040015D3 RID: 5587
	public GameObject ValuableObject;

	// Token: 0x040015D4 RID: 5588
	[Space]
	public GameObject FloorObject1x1;

	// Token: 0x040015D5 RID: 5589
	public GameObject FloorObject1x1Diagonal;

	// Token: 0x040015D6 RID: 5590
	public GameObject FloorObject1x1Curve;

	// Token: 0x040015D7 RID: 5591
	public GameObject FloorObject1x1CurveInverted;

	// Token: 0x040015D8 RID: 5592
	[Space]
	public GameObject FloorObject1x05;

	// Token: 0x040015D9 RID: 5593
	public GameObject FloorObject1x05Diagonal;

	// Token: 0x040015DA RID: 5594
	public GameObject FloorObject1x05Curve;

	// Token: 0x040015DB RID: 5595
	public GameObject FloorObject1x05CurveInverted;

	// Token: 0x040015DC RID: 5596
	[Space]
	public GameObject FloorObject1x025;

	// Token: 0x040015DD RID: 5597
	public GameObject FloorObject1x025Diagonal;

	// Token: 0x040015DE RID: 5598
	[Space]
	public GameObject RoomVolume;

	// Token: 0x040015DF RID: 5599
	public GameObject RoomVolumeOutline;

	// Token: 0x040015E0 RID: 5600
	[Space]
	public GameObject FloorTruck;

	// Token: 0x040015E1 RID: 5601
	public GameObject WallTruck;

	// Token: 0x040015E2 RID: 5602
	[Space]
	public GameObject FloorUsed;

	// Token: 0x040015E3 RID: 5603
	public GameObject WallUsed;

	// Token: 0x040015E4 RID: 5604
	[Space]
	public GameObject FloorInactive;

	// Token: 0x040015E5 RID: 5605
	public GameObject WallInactive;

	// Token: 0x040015E6 RID: 5606
	[Space]
	public GameObject Wall1x1Object;

	// Token: 0x040015E7 RID: 5607
	public GameObject Wall1x1DiagonalObject;

	// Token: 0x040015E8 RID: 5608
	public GameObject Wall1x1CurveObject;

	// Token: 0x040015E9 RID: 5609
	[Space]
	public GameObject Wall1x05Object;

	// Token: 0x040015EA RID: 5610
	public GameObject Wall1x05DiagonalObject;

	// Token: 0x040015EB RID: 5611
	public GameObject Wall1x05CurveObject;

	// Token: 0x040015EC RID: 5612
	[Space]
	public GameObject Wall1x025Object;

	// Token: 0x040015ED RID: 5613
	public GameObject Wall1x025DiagonalObject;

	// Token: 0x040015EE RID: 5614
	[Space]
	public GameObject Door1x1Object;

	// Token: 0x040015EF RID: 5615
	public GameObject Door1x05Object;

	// Token: 0x040015F0 RID: 5616
	public GameObject Door1x1DiagonalObject;

	// Token: 0x040015F1 RID: 5617
	public GameObject Door1x05DiagonalObject;

	// Token: 0x040015F2 RID: 5618
	public GameObject Door1x2Object;

	// Token: 0x040015F3 RID: 5619
	public GameObject Door1x1WizardObject;

	// Token: 0x040015F4 RID: 5620
	public GameObject Door1x1ArcticObject;

	// Token: 0x040015F5 RID: 5621
	[Space]
	public GameObject DoorBlockedObject;

	// Token: 0x040015F6 RID: 5622
	public GameObject DoorBlockedWizardObject;

	// Token: 0x040015F7 RID: 5623
	public GameObject DoorBlockedArcticObject;

	// Token: 0x040015F8 RID: 5624
	public GameObject DoorDiagonalObject;

	// Token: 0x040015F9 RID: 5625
	public GameObject StairsObject;

	// Token: 0x040015FA RID: 5626
	[Space]
	public float Scale = 0.1f;

	// Token: 0x040015FB RID: 5627
	private float LayerHeight = 4f;

	// Token: 0x040015FC RID: 5628
	[Space]
	public Transform playerTransformSource;

	// Token: 0x040015FD RID: 5629
	public Transform playerTransformTarget;

	// Token: 0x040015FE RID: 5630
	[Space]
	public Transform CompletedTransform;

	// Token: 0x040015FF RID: 5631
	internal bool debugActive;

	// Token: 0x04001600 RID: 5632
	[Space]
	public List<Map.RoomVolumeOutlineCustom> RoomVolumeOutlineCustoms;

	// Token: 0x020003A2 RID: 930
	[Serializable]
	public class RoomVolumeOutlineCustom
	{
		// Token: 0x04002BDB RID: 11227
		public Mesh mesh;

		// Token: 0x04002BDC RID: 11228
		public Mesh meshOutline;
	}
}
