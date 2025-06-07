using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

// Token: 0x02000177 RID: 375
public class StatsManager : MonoBehaviour
{
	// Token: 0x06000CD3 RID: 3283 RVA: 0x00071804 File Offset: 0x0006FA04
	private void Awake()
	{
		if (!StatsManager.instance)
		{
			StatsManager.instance = this;
			Object.DontDestroyOnLoad(base.gameObject);
			return;
		}
		Object.Destroy(base.gameObject);
	}

	// Token: 0x06000CD4 RID: 3284 RVA: 0x0007182F File Offset: 0x0006FA2F
	public int TimePlayedGetHours(float _timePlayed)
	{
		return (int)(_timePlayed / 3600f);
	}

	// Token: 0x06000CD5 RID: 3285 RVA: 0x00071839 File Offset: 0x0006FA39
	public int TimePlayedGetMinutes(float _timePlayed)
	{
		return (int)(_timePlayed % 3600f / 60f);
	}

	// Token: 0x06000CD6 RID: 3286 RVA: 0x00071849 File Offset: 0x0006FA49
	public int TimePlayedGetSeconds(float _timePlayed)
	{
		return (int)(_timePlayed % 60f);
	}

	// Token: 0x06000CD7 RID: 3287 RVA: 0x00071853 File Offset: 0x0006FA53
	private void Update()
	{
		if (!SemiFunc.RunIsLobbyMenu() && !SemiFunc.IsMainMenu())
		{
			this.timePlayed += Time.deltaTime;
		}
	}

	// Token: 0x06000CD8 RID: 3288 RVA: 0x00071878 File Offset: 0x0006FA78
	private void Start()
	{
		this.dictionaryOfDictionaries.Add("runStats", this.runStats);
		this.dictionaryOfDictionaries.Add("itemsPurchased", this.itemsPurchased);
		this.dictionaryOfDictionaries.Add("itemsPurchasedTotal", this.itemsPurchasedTotal);
		this.dictionaryOfDictionaries.Add("itemsUpgradesPurchased", this.itemsUpgradesPurchased);
		this.dictionaryOfDictionaries.Add("itemBatteryUpgrades", this.itemBatteryUpgrades);
		this.dictionaryOfDictionaries.Add("playerHealth", this.playerHealth);
		this.dictionaryOfDictionaries.Add("playerUpgradeHealth", this.playerUpgradeHealth);
		this.dictionaryOfDictionaries.Add("playerUpgradeStamina", this.playerUpgradeStamina);
		this.dictionaryOfDictionaries.Add("playerUpgradeExtraJump", this.playerUpgradeExtraJump);
		this.dictionaryOfDictionaries.Add("playerUpgradeLaunch", this.playerUpgradeLaunch);
		this.dictionaryOfDictionaries.Add("playerUpgradeMapPlayerCount", this.playerUpgradeMapPlayerCount);
		this.dictionaryOfDictionaries.Add("playerColor", this.playerColor);
		this.dictionaryOfDictionaries.Add("playerUpgradeSpeed", this.playerUpgradeSpeed);
		this.dictionaryOfDictionaries.Add("playerUpgradeStrength", this.playerUpgradeStrength);
		this.dictionaryOfDictionaries.Add("playerUpgradeRange", this.playerUpgradeRange);
		this.dictionaryOfDictionaries.Add("playerUpgradeThrow", this.playerUpgradeThrow);
		this.dictionaryOfDictionaries.Add("playerUpgradeCrouchRest", this.playerUpgradeCrouchRest);
		this.dictionaryOfDictionaries.Add("playerUpgradeTumbleWings", this.playerUpgradeTumbleWings);
		this.dictionaryOfDictionaries.Add("playerInventorySpot1", this.playerInventorySpot1);
		this.dictionaryOfDictionaries.Add("playerInventorySpot2", this.playerInventorySpot2);
		this.dictionaryOfDictionaries.Add("playerInventorySpot3", this.playerInventorySpot3);
		this.dictionaryOfDictionaries.Add("playerHasCrown", this.playerHasCrown);
		this.dictionaryOfDictionaries.Add("item", this.item);
		this.dictionaryOfDictionaries.Add("itemStatBattery", this.itemStatBattery);
		this.doNotSaveTheseDictionaries.Add("playerInventorySpot1");
		this.doNotSaveTheseDictionaries.Add("playerInventorySpot2");
		this.doNotSaveTheseDictionaries.Add("playerInventorySpot3");
		this.doNotSaveTheseDictionaries.Add("playerColor");
		this.RunStartStats();
	}

	// Token: 0x06000CD9 RID: 3289 RVA: 0x00071ADC File Offset: 0x0006FCDC
	public void DictionaryFill(string dictionaryName, int value)
	{
		foreach (string text in new List<string>(this.dictionaryOfDictionaries[dictionaryName].Keys))
		{
			this.dictionaryOfDictionaries[dictionaryName][text] = value;
		}
	}

	// Token: 0x06000CDA RID: 3290 RVA: 0x00071B4C File Offset: 0x0006FD4C
	public void PlayerAdd(string _steamID, string _playerName)
	{
		this.SetPlayerHealthStart(_steamID, 100);
		this.PlayerInventorySpotsInit(_steamID);
		this.PlayerAddName(_steamID, _playerName);
		foreach (Dictionary<string, int> dictionary in this.AllDictionariesWithPrefix("player"))
		{
			if (!dictionary.ContainsKey(_steamID))
			{
				dictionary.Add(_steamID, 0);
			}
		}
		if (!this.playerColor.ContainsKey(_steamID))
		{
			this.playerColor[_steamID] = -1;
		}
	}

	// Token: 0x06000CDB RID: 3291 RVA: 0x00071BE4 File Offset: 0x0006FDE4
	private void PlayerInventorySpotsInit(string _steamID)
	{
		if (!this.playerInventorySpot1.ContainsKey(_steamID))
		{
			this.playerInventorySpot1.Add(_steamID, -1);
		}
		if (!this.playerInventorySpot2.ContainsKey(_steamID))
		{
			this.playerInventorySpot2.Add(_steamID, -1);
		}
		if (!this.playerInventorySpot3.ContainsKey(_steamID))
		{
			this.playerInventorySpot3.Add(_steamID, -1);
		}
		if (!this.playerInventorySpot1Taken.ContainsKey(_steamID))
		{
			this.playerInventorySpot1Taken.Add(_steamID, 0);
		}
		if (!this.playerInventorySpot2Taken.ContainsKey(_steamID))
		{
			this.playerInventorySpot2Taken.Add(_steamID, 0);
		}
		if (!this.playerInventorySpot3Taken.ContainsKey(_steamID))
		{
			this.playerInventorySpot3Taken.Add(_steamID, 0);
		}
	}

	// Token: 0x06000CDC RID: 3292 RVA: 0x00071C94 File Offset: 0x0006FE94
	public List<string> SaveFileGetPlayerNames(string fileName)
	{
		string text = string.Concat(new string[]
		{
			Application.persistentDataPath,
			"/saves/",
			fileName,
			"/",
			fileName,
			".es3"
		});
		if (!File.Exists(text))
		{
			Debug.LogWarning("Save file not found in " + text);
			return null;
		}
		ES3Settings settings = new ES3Settings(text, ES3.EncryptionType.AES, this.totallyNormalString, null);
		if (ES3.KeyExists("playerNames", settings))
		{
			Dictionary<string, string> dictionary = ES3.Load<Dictionary<string, string>>("playerNames", settings);
			List<string> list = new List<string>();
			foreach (KeyValuePair<string, string> keyValuePair in dictionary)
			{
				list.Add(keyValuePair.Value);
			}
			return list;
		}
		Debug.LogWarning("Key 'playerNames' not found in loaded data.");
		return null;
	}

	// Token: 0x06000CDD RID: 3293 RVA: 0x00071D70 File Offset: 0x0006FF70
	public float SaveFileGetTimePlayed(string _fileName)
	{
		string text = string.Concat(new string[]
		{
			Application.persistentDataPath,
			"/saves/",
			_fileName,
			"/",
			_fileName,
			".es3"
		});
		if (!File.Exists(text))
		{
			Debug.LogWarning("Save file not found in " + text);
			return 0f;
		}
		ES3Settings settings = new ES3Settings(text, ES3.EncryptionType.AES, this.totallyNormalString, null);
		if (ES3.KeyExists("timePlayed", settings))
		{
			return ES3.Load<float>("timePlayed", settings);
		}
		Debug.LogWarning("Key 'timePlayed' not found in loaded data.");
		return 0f;
	}

	// Token: 0x06000CDE RID: 3294 RVA: 0x00071E08 File Offset: 0x00070008
	public void SaveFileCreate()
	{
		string fileName = "REPO_SAVE_" + DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss", CultureInfo.InvariantCulture);
		this.saveFileCurrent = fileName;
		this.SaveGame(fileName);
	}

	// Token: 0x06000CDF RID: 3295 RVA: 0x00071E45 File Offset: 0x00070045
	public void SaveFileSave()
	{
		this.SaveGame(this.saveFileCurrent);
	}

	// Token: 0x06000CE0 RID: 3296 RVA: 0x00071E54 File Offset: 0x00070054
	public string SaveFileGetTeamName(string fileName)
	{
		string text = string.Concat(new string[]
		{
			Application.persistentDataPath,
			"/saves/",
			fileName,
			"/",
			fileName,
			".es3"
		});
		if (File.Exists(text))
		{
			ES3Settings settings = new ES3Settings(text, ES3.EncryptionType.AES, this.totallyNormalString, null);
			return ES3.Load<string>("teamName", settings);
		}
		Debug.LogWarning("Save file not found in " + text);
		return null;
	}

	// Token: 0x06000CE1 RID: 3297 RVA: 0x00071ECC File Offset: 0x000700CC
	public string SaveFileGetDateAndTime(string fileName)
	{
		string text = string.Concat(new string[]
		{
			Application.persistentDataPath,
			"/saves/",
			fileName,
			"/",
			fileName,
			".es3"
		});
		if (File.Exists(text))
		{
			ES3Settings settings = new ES3Settings(text, ES3.EncryptionType.AES, this.totallyNormalString, null);
			return ES3.Load<string>("dateAndTime", settings);
		}
		Debug.LogWarning("Save file not found in " + text);
		return null;
	}

	// Token: 0x06000CE2 RID: 3298 RVA: 0x00071F44 File Offset: 0x00070144
	private string SaveFileGetRunStat(string fileName, string _runStat)
	{
		string text = string.Concat(new string[]
		{
			Application.persistentDataPath,
			"/saves/",
			fileName,
			"/",
			fileName,
			".es3"
		});
		if (!File.Exists(text))
		{
			Debug.LogWarning("Save file not found in " + text);
			return null;
		}
		ES3Settings settings = new ES3Settings(text, ES3.EncryptionType.AES, this.totallyNormalString, null);
		Dictionary<string, Dictionary<string, int>> dictionary = ES3.Load<Dictionary<string, Dictionary<string, int>>>("dictionaryOfDictionaries", settings);
		if (dictionary == null || !dictionary.ContainsKey("runStats"))
		{
			Debug.LogWarning("Key 'runStats' not found in loaded data.");
			return null;
		}
		Dictionary<string, int> dictionary2 = dictionary["runStats"];
		if (dictionary2 != null && dictionary2.ContainsKey(_runStat))
		{
			return dictionary2[_runStat].ToString();
		}
		Debug.LogWarning("Key 'level' not found in 'runStats'.");
		return null;
	}

	// Token: 0x06000CE3 RID: 3299 RVA: 0x0007200A File Offset: 0x0007020A
	public string SaveFileGetRunLevel(string fileName)
	{
		return this.SaveFileGetRunStat(fileName, "level");
	}

	// Token: 0x06000CE4 RID: 3300 RVA: 0x00072018 File Offset: 0x00070218
	public string SaveFileGetRunCurrency(string fileName)
	{
		return this.SaveFileGetRunStat(fileName, "currency");
	}

	// Token: 0x06000CE5 RID: 3301 RVA: 0x00072026 File Offset: 0x00070226
	public string SaveFileGetTotalHaul(string fileName)
	{
		return this.SaveFileGetRunStat(fileName, "totalHaul");
	}

	// Token: 0x06000CE6 RID: 3302 RVA: 0x00072034 File Offset: 0x00070234
	private void RunStartStats()
	{
		this.runStats.Clear();
		this.runStats.Add("level", 0);
		this.runStats.Add("currency", 0);
		this.runStats.Add("lives", 3);
		this.runStats.Add("chargingStationCharge", 1);
		this.runStats.Add("chargingStationChargeTotal", 100);
		this.runStats.Add("totalHaul", 0);
		this.statsSynced = true;
		this.LoadItemsFromFolder();
		this.DictionaryFill("itemsPurchased", 0);
		this.DictionaryFill("itemsPurchasedTotal", 0);
		this.DictionaryFill("itemsUpgradesPurchased", 0);
		this.itemsPurchased["Item Power Crystal"] = 1;
		this.itemsPurchasedTotal["Item Power Crystal"] = 1;
		this.itemsPurchased["Item Cart Medium"] = 1;
		this.itemsPurchasedTotal["Item Cart Medium"] = 1;
		this.playerColorIndex = 0;
	}

	// Token: 0x06000CE7 RID: 3303 RVA: 0x0007212F File Offset: 0x0007032F
	private void PlayerAddName(string _steamID, string _playerName)
	{
		if (this.playerNames.ContainsKey(_steamID))
		{
			this.playerNames[_steamID] = _playerName;
			return;
		}
		this.playerNames.Add(_steamID, _playerName);
	}

	// Token: 0x06000CE8 RID: 3304 RVA: 0x0007215C File Offset: 0x0007035C
	public Dictionary<string, int> FetchPlayerUpgrades(string _steamID)
	{
		Dictionary<string, int> dictionary = new Dictionary<string, int>();
		Regex regex = new Regex("(?<!^)(?=[A-Z])");
		foreach (KeyValuePair<string, Dictionary<string, int>> keyValuePair in this.dictionaryOfDictionaries)
		{
			if (keyValuePair.Key.StartsWith("playerUpgrade") && keyValuePair.Value.ContainsKey(_steamID))
			{
				string text = "";
				string[] array = regex.Split(keyValuePair.Key);
				bool flag = false;
				foreach (string text2 in array)
				{
					if (flag)
					{
						text = text + text2 + " ";
					}
					if (text2 == "Upgrade")
					{
						flag = true;
					}
				}
				text = text.Trim();
				int num = keyValuePair.Value[_steamID];
				dictionary.Add(text, num);
			}
		}
		return dictionary;
	}

	// Token: 0x06000CE9 RID: 3305 RVA: 0x00072260 File Offset: 0x00070460
	public void DictionaryUpdateValue(string dictionaryName, string key, int value)
	{
		if (this.dictionaryOfDictionaries.ContainsKey(dictionaryName) && this.dictionaryOfDictionaries[dictionaryName].ContainsKey(key))
		{
			this.dictionaryOfDictionaries[dictionaryName][key] = value;
		}
	}

	// Token: 0x06000CEA RID: 3306 RVA: 0x00072297 File Offset: 0x00070497
	public void ItemUpdateStatBattery(string itemName, int value, bool sync = true)
	{
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		if (this.itemStatBattery.ContainsKey(itemName))
		{
			this.itemStatBattery[itemName] = value;
			if (sync)
			{
				PunManager.instance.UpdateStat("itemStatBattery", itemName, value);
			}
		}
	}

	// Token: 0x06000CEB RID: 3307 RVA: 0x000722D0 File Offset: 0x000704D0
	public void PlayerInventoryUpdate(string _steamID, string itemName, int spot, bool sync = true)
	{
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		int num = itemName.GetHashCode();
		if (itemName == "")
		{
			num = -1;
		}
		if (spot == 0)
		{
			this.playerInventorySpot1[_steamID] = num;
			if (this.playerInventorySpot1[_steamID] != -1)
			{
				this.playerInventorySpot1Taken[_steamID] = 1;
			}
			else
			{
				this.playerInventorySpot1Taken[_steamID] = 0;
			}
			if (sync)
			{
				PunManager.instance.UpdateStat("playerInventorySpot1", itemName, spot);
			}
		}
		if (spot == 1)
		{
			this.playerInventorySpot2[_steamID] = num;
			if (this.playerInventorySpot2[_steamID] != -1)
			{
				this.playerInventorySpot2Taken[_steamID] = 1;
			}
			else
			{
				this.playerInventorySpot2Taken[_steamID] = 0;
			}
			if (sync)
			{
				PunManager.instance.UpdateStat("playerInventorySpot2", itemName, spot);
			}
		}
		if (spot == 2)
		{
			this.playerInventorySpot3[_steamID] = num;
			if (this.playerInventorySpot3[_steamID] != -1)
			{
				this.playerInventorySpot3Taken[_steamID] = 1;
			}
			else
			{
				this.playerInventorySpot3Taken[_steamID] = 0;
			}
			if (sync)
			{
				PunManager.instance.UpdateStat("playerInventorySpot3", itemName, spot);
			}
		}
	}

	// Token: 0x06000CEC RID: 3308 RVA: 0x000723F0 File Offset: 0x000705F0
	public void ItemFetchName(string itemName, ItemAttributes itemAttributes, int photonViewID)
	{
		string name = itemName;
		bool flag = false;
		foreach (string text in this.item.Keys)
		{
			if (text.Contains('/') && text.Split('/', 0)[0] == itemName && !this.takenItemNames.Contains(text))
			{
				name = text;
				this.takenItemNames.Add(text);
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			this.ItemAdd(itemName, itemAttributes, photonViewID);
			return;
		}
		PunManager.instance.SetItemName(name, itemAttributes, photonViewID);
	}

	// Token: 0x06000CED RID: 3309 RVA: 0x0007249C File Offset: 0x0007069C
	public void StuffNeedingResetAtTheEndOfAScene()
	{
		this.takenItemNames.Clear();
		foreach (PlayerAvatar playerAvatar in GameDirector.instance.PlayerList)
		{
			this.playerInventorySpot1Taken[playerAvatar.steamID] = 0;
			this.playerInventorySpot2Taken[playerAvatar.steamID] = 0;
			this.playerInventorySpot3Taken[playerAvatar.steamID] = 0;
		}
	}

	// Token: 0x06000CEE RID: 3310 RVA: 0x00072530 File Offset: 0x00070730
	public int GetIndexThatHoldsThisItemFromItemDictionary(string itemName)
	{
		int num = 0;
		using (Dictionary<string, Item>.KeyCollection.Enumerator enumerator = this.itemDictionary.Keys.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current == itemName)
				{
					return num;
				}
				num++;
			}
		}
		return -1;
	}

	// Token: 0x06000CEF RID: 3311 RVA: 0x00072598 File Offset: 0x00070798
	public string GetItemNameFromIndexInItemDictionary(int index)
	{
		if (index >= 0 && index < this.itemDictionary.Count)
		{
			return Enumerable.ElementAt<string>(this.itemDictionary.Keys, index);
		}
		return null;
	}

	// Token: 0x06000CF0 RID: 3312 RVA: 0x000725C0 File Offset: 0x000707C0
	public void ItemAdd(string itemName, ItemAttributes itemAttributes = null, int photonViewID = -1)
	{
		int num = 1;
		foreach (string text in this.item.Keys)
		{
			if (text.Contains('/'))
			{
				string[] array = text.Split('/', 0);
				if (array[0] == itemName)
				{
					int num2 = int.Parse(array[1]);
					if (num2 >= num)
					{
						num = num2 + 1;
					}
				}
			}
		}
		int indexThatHoldsThisItemFromItemDictionary = this.GetIndexThatHoldsThisItemFromItemDictionary(itemName);
		itemName = itemName + "/" + num.ToString();
		this.AddingItem(itemName, indexThatHoldsThisItemFromItemDictionary, photonViewID, itemAttributes);
	}

	// Token: 0x06000CF1 RID: 3313 RVA: 0x00072670 File Offset: 0x00070870
	private void AddingItem(string itemName, int index, int photonViewID, ItemAttributes itemAttributes)
	{
		PunManager.instance.AddingItem(itemName, index, photonViewID, itemAttributes);
	}

	// Token: 0x06000CF2 RID: 3314 RVA: 0x00072684 File Offset: 0x00070884
	public void ItemRemove(string instanceName)
	{
		string text = instanceName.Split('/', 0)[0];
		if (this.item.ContainsKey(instanceName))
		{
			Dictionary<string, int> dictionary = this.itemsPurchased;
			string text2 = text;
			dictionary[text2]--;
			this.itemsPurchased[text] = Mathf.Max(0, this.itemsPurchased[text]);
		}
		else
		{
			Debug.LogError("Item " + text + " not found in item dictionary");
		}
		if (this.item.ContainsKey(instanceName))
		{
			this.item.Remove(instanceName);
			this.itemStatBattery.Remove(instanceName);
		}
	}

	// Token: 0x06000CF3 RID: 3315 RVA: 0x00072724 File Offset: 0x00070924
	public void ItemPurchase(string itemName)
	{
		Dictionary<string, int> dictionary = this.itemsPurchased;
		dictionary[itemName]++;
		dictionary = this.itemsPurchasedTotal;
		dictionary[itemName]++;
		if (this.itemDictionary[itemName].physicalItem)
		{
			this.ItemAdd(itemName, null, -1);
		}
	}

	// Token: 0x06000CF4 RID: 3316 RVA: 0x00072780 File Offset: 0x00070980
	private List<Dictionary<string, int>> AllDictionariesWithPrefix(string prefix)
	{
		List<Dictionary<string, int>> list = new List<Dictionary<string, int>>();
		foreach (KeyValuePair<string, Dictionary<string, int>> keyValuePair in this.dictionaryOfDictionaries)
		{
			if (keyValuePair.Key.StartsWith(prefix) && keyValuePair.Value != null)
			{
				list.Add(keyValuePair.Value);
			}
		}
		return list;
	}

	// Token: 0x06000CF5 RID: 3317 RVA: 0x000727F8 File Offset: 0x000709F8
	public int GetBatteryLevel(string itemName)
	{
		return this.itemStatBattery[itemName];
	}

	// Token: 0x06000CF6 RID: 3318 RVA: 0x00072806 File Offset: 0x00070A06
	public void SetBatteryLevel(string itemName, int value)
	{
		this.itemStatBattery[itemName] = value;
	}

	// Token: 0x06000CF7 RID: 3319 RVA: 0x00072815 File Offset: 0x00070A15
	public int GetItemPurchased(Item _item)
	{
		return this.itemsPurchased[_item.itemAssetName];
	}

	// Token: 0x06000CF8 RID: 3320 RVA: 0x00072828 File Offset: 0x00070A28
	public void SetItemPurchase(Item _item, int value)
	{
		this.itemsPurchased[_item.itemAssetName] = value;
	}

	// Token: 0x06000CF9 RID: 3321 RVA: 0x0007283C File Offset: 0x00070A3C
	public int GetItemsUpgradesPurchased(string itemName)
	{
		return this.itemsUpgradesPurchased[itemName];
	}

	// Token: 0x06000CFA RID: 3322 RVA: 0x0007284A File Offset: 0x00070A4A
	public int GetItemsUpgradesPurchasedTotal(string itemName)
	{
		return this.itemsPurchasedTotal[itemName];
	}

	// Token: 0x06000CFB RID: 3323 RVA: 0x00072858 File Offset: 0x00070A58
	public void SetItemsUpgradesPurchased(string itemName, int value)
	{
		this.itemsUpgradesPurchased[itemName] = value;
	}

	// Token: 0x06000CFC RID: 3324 RVA: 0x00072868 File Offset: 0x00070A68
	public void AddItemsUpgradesPurchased(string itemName)
	{
		Dictionary<string, int> dictionary = this.itemsUpgradesPurchased;
		int num = dictionary[itemName];
		dictionary[itemName] = num + 1;
	}

	// Token: 0x06000CFD RID: 3325 RVA: 0x00072890 File Offset: 0x00070A90
	public void SetPlayerColor(string _steamID, int _colorIndex = -1)
	{
		if (_colorIndex != -1)
		{
			this.playerColor[_steamID] = _colorIndex;
			return;
		}
		if (this.playerColor[_steamID] == -1)
		{
			this.playerColor[_steamID] = this.playerColorIndex;
			this.playerColorIndex++;
		}
	}

	// Token: 0x06000CFE RID: 3326 RVA: 0x000728DE File Offset: 0x00070ADE
	public int GetPlayerColor(string _steamID)
	{
		return this.playerColor[_steamID];
	}

	// Token: 0x06000CFF RID: 3327 RVA: 0x000728EC File Offset: 0x00070AEC
	public void SetPlayerHealthStart(string _steamID, int health)
	{
		if (!this.playerHealth.ContainsKey(_steamID))
		{
			this.playerHealth[_steamID] = health;
		}
	}

	// Token: 0x06000D00 RID: 3328 RVA: 0x00072909 File Offset: 0x00070B09
	public void SetPlayerHealth(string _steamID, int health, bool setInShop)
	{
		if (SemiFunc.RunIsShop() && !setInShop)
		{
			return;
		}
		this.playerHealth[_steamID] = health;
	}

	// Token: 0x06000D01 RID: 3329 RVA: 0x00072923 File Offset: 0x00070B23
	public int GetPlayerHealth(string _steamID)
	{
		if (!this.playerHealth.ContainsKey(_steamID))
		{
			return 0;
		}
		return this.playerHealth[_steamID];
	}

	// Token: 0x06000D02 RID: 3330 RVA: 0x00072941 File Offset: 0x00070B41
	public int GetPlayerMaxHealth(string _steamID)
	{
		if (!this.playerUpgradeHealth.ContainsKey(_steamID))
		{
			return 0;
		}
		return this.playerUpgradeHealth[_steamID] * 20;
	}

	// Token: 0x06000D03 RID: 3331 RVA: 0x00072962 File Offset: 0x00070B62
	public int GetRunStatCurrency()
	{
		return this.runStats["currency"];
	}

	// Token: 0x06000D04 RID: 3332 RVA: 0x00072974 File Offset: 0x00070B74
	public int GetRunStatLives()
	{
		return this.runStats["lives"];
	}

	// Token: 0x06000D05 RID: 3333 RVA: 0x00072986 File Offset: 0x00070B86
	public int GetRunStatLevel()
	{
		return this.runStats["level"];
	}

	// Token: 0x06000D06 RID: 3334 RVA: 0x00072998 File Offset: 0x00070B98
	public int GetRunStatSaveLevel()
	{
		int result = 0;
		if (this.runStats.ContainsKey("save level"))
		{
			result = this.runStats["save level"];
		}
		return result;
	}

	// Token: 0x06000D07 RID: 3335 RVA: 0x000729CB File Offset: 0x00070BCB
	public int GetRunStatTotalHaul()
	{
		return this.runStats["totalHaul"];
	}

	// Token: 0x06000D08 RID: 3336 RVA: 0x000729E0 File Offset: 0x00070BE0
	private void DebugSync()
	{
		int num = 0;
		foreach (KeyValuePair<string, Dictionary<string, int>> keyValuePair in this.dictionaryOfDictionaries)
		{
			foreach (string text in new List<string>(keyValuePair.Value.Keys))
			{
				keyValuePair.Value[text] = 1;
				num++;
			}
		}
		SemiFunc.StatSyncAll();
	}

	// Token: 0x06000D09 RID: 3337 RVA: 0x00072A90 File Offset: 0x00070C90
	public void ResetAllStats()
	{
		this.saveFileReady = false;
		ItemManager.instance.ResetAllItems();
		foreach (KeyValuePair<string, Dictionary<string, int>> keyValuePair in this.dictionaryOfDictionaries)
		{
			keyValuePair.Value.Clear();
		}
		this.takenItemNames.Clear();
		this.runStats.Clear();
		this.playerNames.Clear();
		this.timePlayed = 0f;
		this.RunStartStats();
	}

	// Token: 0x06000D0A RID: 3338 RVA: 0x00072B2C File Offset: 0x00070D2C
	private void LoadItemsFromFolder()
	{
		Item[] array = Resources.LoadAll<Item>(this.folderPath);
		int i = 0;
		while (i < array.Length)
		{
			Item item = array[i];
			if (!string.IsNullOrEmpty(item.itemAssetName))
			{
				if (!this.itemDictionary.ContainsKey(item.itemAssetName))
				{
					this.itemDictionary.Add(item.itemAssetName, item);
				}
				using (List<Dictionary<string, int>>.Enumerator enumerator = this.AllDictionariesWithPrefix("item").GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Dictionary<string, int> dictionary = enumerator.Current;
						dictionary.Add(item.itemAssetName, 0);
					}
					goto IL_92;
				}
				goto IL_88;
			}
			goto IL_88;
			IL_92:
			i++;
			continue;
			IL_88:
			Debug.LogWarning("Item with empty or null itemName found and will be skipped.");
			goto IL_92;
		}
	}

	// Token: 0x06000D0B RID: 3339 RVA: 0x00072BE8 File Offset: 0x00070DE8
	public void EmptyAllBatteries()
	{
		foreach (string text in new List<string>(this.itemStatBattery.Keys))
		{
			this.itemStatBattery[text] = 0;
		}
	}

	// Token: 0x06000D0C RID: 3340 RVA: 0x00072C4C File Offset: 0x00070E4C
	public void BuyAllItems()
	{
		foreach (string itemName in new List<string>(this.itemDictionary.Keys))
		{
			this.ItemPurchase(itemName);
		}
	}

	// Token: 0x06000D0D RID: 3341 RVA: 0x00072CAC File Offset: 0x00070EAC
	private void ManualEntry()
	{
		List<KeyValuePair<string, Item>> list = new List<KeyValuePair<string, Item>>();
		foreach (KeyValuePair<string, Item> keyValuePair in this.itemDictionary)
		{
			string itemAssetName = keyValuePair.Value.itemAssetName;
			if (!string.IsNullOrEmpty(itemAssetName))
			{
				list.Add(new KeyValuePair<string, Item>(itemAssetName, keyValuePair.Value));
			}
			else
			{
				Debug.LogWarning("Item with empty or null name found and will be skipped.");
			}
		}
		this.itemDictionary.Clear();
		foreach (KeyValuePair<string, Item> keyValuePair2 in list)
		{
			if (!this.itemDictionary.ContainsKey(keyValuePair2.Key))
			{
				this.itemDictionary.Add(keyValuePair2.Key, keyValuePair2.Value);
			}
			else
			{
				Debug.LogWarning("Duplicate key found: " + keyValuePair2.Key + ". This entry will be skipped.");
			}
		}
	}

	// Token: 0x06000D0E RID: 3342 RVA: 0x00072DC0 File Offset: 0x00070FC0
	public List<string> SaveFileGetAll()
	{
		List<string> list = new List<string>();
		string text = Application.persistentDataPath + "/saves";
		if (Directory.Exists(text))
		{
			using (var enumerator = Enumerable.ToList(Enumerable.OrderByDescending(Enumerable.Select(Directory.GetDirectories(text), (string dir) => new
			{
				Path = dir,
				CreationTime = Directory.GetCreationTime(dir)
			}), x => x.CreationTime)).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					var <>f__AnonymousType = enumerator.Current;
					string fileName = Path.GetFileName(<>f__AnonymousType.Path);
					list.Add(fileName);
				}
				return list;
			}
		}
		Debug.LogWarning("Saves directory not found at " + text);
		return list;
	}

	// Token: 0x06000D0F RID: 3343 RVA: 0x00072E9C File Offset: 0x0007109C
	public void SaveFileDelete(string saveFileName)
	{
		string text = Application.persistentDataPath + "/saves/" + saveFileName;
		if (Directory.Exists(text))
		{
			Directory.Delete(text, true);
			Debug.Log("Deleted save file and all backups for '" + saveFileName + "'");
			return;
		}
		Debug.LogWarning("Save folder not found: " + text);
	}

	// Token: 0x06000D10 RID: 3344 RVA: 0x00072EF0 File Offset: 0x000710F0
	public void SaveGame(string fileName)
	{
		if (string.IsNullOrEmpty(fileName))
		{
			return;
		}
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		this.dateAndTime = DateTime.Now.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
		string text = Application.persistentDataPath + "/saves";
		if (!Directory.Exists(text))
		{
			Directory.CreateDirectory(text);
		}
		string text2 = text + "/" + fileName;
		if (!Directory.Exists(text2))
		{
			Directory.CreateDirectory(text2);
		}
		string text3 = text2 + "/" + fileName + ".es3";
		if (File.Exists(text3))
		{
			int num = 1;
			string text4;
			do
			{
				text4 = string.Concat(new string[]
				{
					text2,
					"/",
					fileName,
					"_BACKUP",
					num.ToString(),
					".es3"
				});
				num++;
			}
			while (File.Exists(text4));
			File.Move(text3, text4);
		}
		Dictionary<string, Dictionary<string, int>> dictionary = new Dictionary<string, Dictionary<string, int>>();
		foreach (string text5 in this.doNotSaveTheseDictionaries)
		{
			if (this.dictionaryOfDictionaries.ContainsKey(text5))
			{
				dictionary[text5] = this.dictionaryOfDictionaries[text5];
				this.dictionaryOfDictionaries.Remove(text5);
			}
		}
		ES3Settings es3Settings = new ES3Settings(new Enum[]
		{
			ES3.Location.Cache
		});
		es3Settings.encryptionType = ES3.EncryptionType.AES;
		es3Settings.encryptionPassword = this.totallyNormalString;
		es3Settings.path = text3;
		ES3.Save<string>("teamName", this.teamName, es3Settings);
		ES3.Save<string>("dateAndTime", this.dateAndTime, es3Settings);
		ES3.Save<float>("timePlayed", this.timePlayed, es3Settings);
		ES3.Save<Dictionary<string, string>>("playerNames", this.playerNames, es3Settings);
		ES3.Save<Dictionary<string, Dictionary<string, int>>>("dictionaryOfDictionaries", this.dictionaryOfDictionaries, es3Settings);
		ES3.StoreCachedFile(es3Settings);
		foreach (KeyValuePair<string, Dictionary<string, int>> keyValuePair in dictionary)
		{
			this.dictionaryOfDictionaries[keyValuePair.Key] = keyValuePair.Value;
		}
		this.PlayersAddAll();
		this.saveFileReady = true;
	}

	// Token: 0x06000D11 RID: 3345 RVA: 0x00073140 File Offset: 0x00071340
	private void PlayersAddAll()
	{
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		foreach (PlayerAvatar playerAvatar in GameDirector.instance.PlayerList)
		{
			this.PlayerAdd(playerAvatar.steamID, playerAvatar.playerName);
			this.SetPlayerColor(playerAvatar.steamID, -1);
		}
	}

	// Token: 0x06000D12 RID: 3346 RVA: 0x000731B8 File Offset: 0x000713B8
	public void LoadGame(string fileName)
	{
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		string text = string.Concat(new string[]
		{
			Application.persistentDataPath,
			"/saves/",
			fileName,
			"/",
			fileName,
			".es3"
		});
		if (File.Exists(text))
		{
			this.saveFileCurrent = fileName;
			ES3Settings settings = new ES3Settings(text, ES3.EncryptionType.AES, this.totallyNormalString, null);
			this.teamName = ES3.Load<string>("teamName", settings);
			this.dateAndTime = ES3.Load<string>("dateAndTime", settings);
			this.timePlayed = ES3.Load<float>("timePlayed", settings);
			this.playerNames = ES3.Load<Dictionary<string, string>>("playerNames", settings);
			using (Dictionary<string, Dictionary<string, int>>.Enumerator enumerator = ES3.Load<Dictionary<string, Dictionary<string, int>>>("dictionaryOfDictionaries", settings).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					KeyValuePair<string, Dictionary<string, int>> keyValuePair = enumerator.Current;
					Dictionary<string, int> dictionary;
					if (this.dictionaryOfDictionaries.TryGetValue(keyValuePair.Key, ref dictionary))
					{
						using (Dictionary<string, int>.Enumerator enumerator2 = keyValuePair.Value.GetEnumerator())
						{
							while (enumerator2.MoveNext())
							{
								KeyValuePair<string, int> keyValuePair2 = enumerator2.Current;
								dictionary[keyValuePair2.Key] = keyValuePair2.Value;
							}
							continue;
						}
					}
					this.dictionaryOfDictionaries.Add(keyValuePair.Key, new Dictionary<string, int>(keyValuePair.Value));
				}
				goto IL_166;
			}
		}
		Debug.LogWarning("Save file not found in " + text);
		IL_166:
		RunManager.instance.levelsCompleted = this.GetRunStatLevel();
		RunManager.instance.runLives = this.GetRunStatLives();
		RunManager.instance.loadLevel = this.GetRunStatSaveLevel();
		this.PlayersAddAll();
		this.saveFileReady = true;
	}

	// Token: 0x06000D13 RID: 3347 RVA: 0x00073384 File Offset: 0x00071584
	public void UpdateCrown(string steamID)
	{
		foreach (string text in new List<string>(this.playerHasCrown.Keys))
		{
			this.playerHasCrown[text] = 0;
		}
		if (this.playerHasCrown.ContainsKey(steamID))
		{
			this.playerHasCrown[steamID] = 1;
		}
	}

	// Token: 0x040014CE RID: 5326
	public static StatsManager instance;

	// Token: 0x040014CF RID: 5327
	public string folderPath = "ScriptableObjects";

	// Token: 0x040014D0 RID: 5328
	internal string dateAndTime;

	// Token: 0x040014D1 RID: 5329
	internal string teamName = "R.E.P.O.";

	// Token: 0x040014D2 RID: 5330
	private string totallyNormalString = "Why would you want to cheat?... :o It's no fun. :') :'D";

	// Token: 0x040014D3 RID: 5331
	public Dictionary<string, Item> itemDictionary = new Dictionary<string, Item>();

	// Token: 0x040014D4 RID: 5332
	public Dictionary<string, int> runStats = new Dictionary<string, int>();

	// Token: 0x040014D5 RID: 5333
	public Dictionary<string, int> itemsPurchased = new Dictionary<string, int>();

	// Token: 0x040014D6 RID: 5334
	public Dictionary<string, int> itemsUpgradesPurchased = new Dictionary<string, int>();

	// Token: 0x040014D7 RID: 5335
	public Dictionary<string, int> itemBatteryUpgrades = new Dictionary<string, int>();

	// Token: 0x040014D8 RID: 5336
	public Dictionary<string, int> itemsPurchasedTotal = new Dictionary<string, int>();

	// Token: 0x040014D9 RID: 5337
	public Dictionary<string, int> playerHealth = new Dictionary<string, int>();

	// Token: 0x040014DA RID: 5338
	public Dictionary<string, int> playerUpgradeHealth = new Dictionary<string, int>();

	// Token: 0x040014DB RID: 5339
	public Dictionary<string, int> playerUpgradeStamina = new Dictionary<string, int>();

	// Token: 0x040014DC RID: 5340
	public Dictionary<string, int> playerUpgradeExtraJump = new Dictionary<string, int>();

	// Token: 0x040014DD RID: 5341
	public Dictionary<string, int> playerUpgradeLaunch = new Dictionary<string, int>();

	// Token: 0x040014DE RID: 5342
	public Dictionary<string, int> playerUpgradeMapPlayerCount = new Dictionary<string, int>();

	// Token: 0x040014DF RID: 5343
	internal Dictionary<string, int> playerColor = new Dictionary<string, int>();

	// Token: 0x040014E0 RID: 5344
	private int playerColorIndex;

	// Token: 0x040014E1 RID: 5345
	public Dictionary<string, int> playerUpgradeSpeed = new Dictionary<string, int>();

	// Token: 0x040014E2 RID: 5346
	public Dictionary<string, int> playerUpgradeStrength = new Dictionary<string, int>();

	// Token: 0x040014E3 RID: 5347
	public Dictionary<string, int> playerUpgradeThrow = new Dictionary<string, int>();

	// Token: 0x040014E4 RID: 5348
	public Dictionary<string, int> playerUpgradeRange = new Dictionary<string, int>();

	// Token: 0x040014E5 RID: 5349
	public Dictionary<string, int> playerUpgradeCrouchRest = new Dictionary<string, int>();

	// Token: 0x040014E6 RID: 5350
	public Dictionary<string, int> playerUpgradeTumbleWings = new Dictionary<string, int>();

	// Token: 0x040014E7 RID: 5351
	public Dictionary<string, int> playerInventorySpot1 = new Dictionary<string, int>();

	// Token: 0x040014E8 RID: 5352
	public Dictionary<string, int> playerInventorySpot2 = new Dictionary<string, int>();

	// Token: 0x040014E9 RID: 5353
	public Dictionary<string, int> playerInventorySpot3 = new Dictionary<string, int>();

	// Token: 0x040014EA RID: 5354
	public Dictionary<string, int> playerInventorySpot1Taken = new Dictionary<string, int>();

	// Token: 0x040014EB RID: 5355
	public Dictionary<string, int> playerInventorySpot2Taken = new Dictionary<string, int>();

	// Token: 0x040014EC RID: 5356
	public Dictionary<string, int> playerInventorySpot3Taken = new Dictionary<string, int>();

	// Token: 0x040014ED RID: 5357
	public Dictionary<string, int> playerHasCrown = new Dictionary<string, int>();

	// Token: 0x040014EE RID: 5358
	public Dictionary<string, int> item = new Dictionary<string, int>();

	// Token: 0x040014EF RID: 5359
	public Dictionary<string, int> itemStatBattery = new Dictionary<string, int>();

	// Token: 0x040014F0 RID: 5360
	[HideInInspector]
	public float chargingStationCharge = 1f;

	// Token: 0x040014F1 RID: 5361
	public Dictionary<string, Dictionary<string, int>> dictionaryOfDictionaries = new Dictionary<string, Dictionary<string, int>>();

	// Token: 0x040014F2 RID: 5362
	[HideInInspector]
	public bool statsSynced;

	// Token: 0x040014F3 RID: 5363
	internal List<string> takenItemNames = new List<string>();

	// Token: 0x040014F4 RID: 5364
	internal float timePlayed;

	// Token: 0x040014F5 RID: 5365
	internal Dictionary<string, string> playerNames = new Dictionary<string, string>();

	// Token: 0x040014F6 RID: 5366
	internal string saveFileCurrent;

	// Token: 0x040014F7 RID: 5367
	internal bool saveFileReady;

	// Token: 0x040014F8 RID: 5368
	internal List<string> doNotSaveTheseDictionaries = new List<string>();

	// Token: 0x02000390 RID: 912
	[Serializable]
	public class SerializableDictionary
	{
		// Token: 0x04002B84 RID: 11140
		public List<string> keys = new List<string>();

		// Token: 0x04002B85 RID: 11141
		public List<StatsManager.SerializableInnerDictionary> values = new List<StatsManager.SerializableInnerDictionary>();
	}

	// Token: 0x02000391 RID: 913
	[Serializable]
	public class SerializableInnerDictionary
	{
		// Token: 0x04002B86 RID: 11142
		public List<string> keys = new List<string>();

		// Token: 0x04002B87 RID: 11143
		public List<int> values = new List<int>();
	}
}
