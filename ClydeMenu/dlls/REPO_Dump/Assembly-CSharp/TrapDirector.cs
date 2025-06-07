using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000242 RID: 578
public class TrapDirector : MonoBehaviourPunCallbacks, IPunObservable
{
	// Token: 0x060012E5 RID: 4837 RVA: 0x000A92E5 File Offset: 0x000A74E5
	private void Awake()
	{
		TrapDirector.instance = this;
	}

	// Token: 0x060012E6 RID: 4838 RVA: 0x000A92ED File Offset: 0x000A74ED
	private void Start()
	{
		base.StartCoroutine(this.Generate());
	}

	// Token: 0x060012E7 RID: 4839 RVA: 0x000A92FC File Offset: 0x000A74FC
	private void Update()
	{
		if (this.TrapCooldown > 0f)
		{
			this.TrapCooldown -= Time.deltaTime;
		}
	}

	// Token: 0x060012E8 RID: 4840 RVA: 0x000A931D File Offset: 0x000A751D
	private IEnumerator Generate()
	{
		while (!LevelGenerator.Instance.Generated)
		{
			yield return new WaitForSeconds(0.1f);
		}
		if (GameManager.instance.gameMode == 0 || PhotonNetwork.IsMasterClient)
		{
			this.UpdateTrapList();
		}
		this.TrapListUpdated = true;
		yield break;
	}

	// Token: 0x060012E9 RID: 4841 RVA: 0x000A932C File Offset: 0x000A752C
	private void UpdateTrapList()
	{
		Dictionary<string, List<GameObject>> dictionary = new Dictionary<string, List<GameObject>>();
		foreach (GameObject gameObject in this.TrapList)
		{
			TrapTypeIdentifier component = gameObject.GetComponent<TrapTypeIdentifier>();
			if (component != null)
			{
				string trapType = component.trapType;
				if (!dictionary.ContainsKey(trapType))
				{
					dictionary[trapType] = new List<GameObject>();
				}
				dictionary[trapType].Add(gameObject);
			}
		}
		if (this.DebugAllTraps)
		{
			return;
		}
		foreach (KeyValuePair<string, List<GameObject>> keyValuePair in dictionary)
		{
			if (keyValuePair.Value.Count > 0)
			{
				GameObject gameObject2 = keyValuePair.Value[Random.Range(0, keyValuePair.Value.Count)];
				this.SelectedTraps.Add(gameObject2);
			}
		}
		using (List<GameObject>.Enumerator enumerator = this.TrapList.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				GameObject gameObject3 = enumerator.Current;
				if (!this.SelectedTraps.Contains(gameObject3))
				{
					TrapTypeIdentifier component2 = gameObject3.GetComponent<TrapTypeIdentifier>();
					if (component2 != null)
					{
						if (component2.Trigger != null && component2.OnlyRemoveTrigger)
						{
							if (GameManager.instance.gameMode == 0)
							{
								Object.Destroy(component2.Trigger);
								component2.TriggerRemoved = true;
							}
							else
							{
								gameObject3.GetComponent<PhotonView>().RPC("DestroyTrigger", RpcTarget.AllBuffered, Array.Empty<object>());
							}
						}
						else if (GameManager.instance.gameMode == 0)
						{
							Object.Destroy(gameObject3);
						}
						else
						{
							gameObject3.GetComponent<PhotonView>().RPC("DestroyTrap", RpcTarget.AllBuffered, Array.Empty<object>());
						}
					}
				}
			}
			goto IL_278;
		}
		IL_1C0:
		int num = Random.Range(0, this.SelectedTraps.Count);
		GameObject gameObject4 = this.SelectedTraps[num];
		this.SelectedTraps.RemoveAt(num);
		TrapTypeIdentifier component3 = gameObject4.GetComponent<TrapTypeIdentifier>();
		if (component3 != null)
		{
			if (component3.Trigger != null)
			{
				if (GameManager.instance.gameMode == 0)
				{
					Object.Destroy(component3.Trigger);
					component3.TriggerRemoved = true;
				}
				else
				{
					gameObject4.GetComponent<PhotonView>().RPC("DestroyTrigger", RpcTarget.AllBuffered, Array.Empty<object>());
				}
			}
			else if (GameManager.instance.gameMode == 0)
			{
				Object.Destroy(gameObject4);
			}
			else
			{
				gameObject4.GetComponent<PhotonView>().RPC("DestroyTrap", RpcTarget.AllBuffered, Array.Empty<object>());
			}
		}
		IL_278:
		if (this.SelectedTraps.Count <= this.TrapCount)
		{
			return;
		}
		goto IL_1C0;
	}

	// Token: 0x060012EA RID: 4842 RVA: 0x000A95F0 File Offset: 0x000A77F0
	private string RandomType(Dictionary<string, List<GameObject>> trapsByType)
	{
		List<string> list = new List<string>(trapsByType.Keys);
		int num = Random.Range(0, list.Count);
		return list[num];
	}

	// Token: 0x060012EB RID: 4843 RVA: 0x000A961D File Offset: 0x000A781D
	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (!SemiFunc.MasterOnlyRPC(info))
		{
			return;
		}
		if (stream.IsWriting)
		{
			stream.SendNext(this.TrapListUpdated);
			return;
		}
		this.TrapListUpdated = (bool)stream.ReceiveNext();
	}

	// Token: 0x04002026 RID: 8230
	public static TrapDirector instance;

	// Token: 0x04002027 RID: 8231
	public bool DebugAllTraps;

	// Token: 0x04002028 RID: 8232
	[Space]
	public List<GameObject> TrapList = new List<GameObject>();

	// Token: 0x04002029 RID: 8233
	public List<GameObject> SelectedTraps = new List<GameObject>();

	// Token: 0x0400202A RID: 8234
	public float TrapCooldown;

	// Token: 0x0400202B RID: 8235
	internal bool TrapListUpdated;

	// Token: 0x0400202C RID: 8236
	public int TrapCount = 2;
}
