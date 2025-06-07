using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;

// Token: 0x02000222 RID: 546
public class MenuPageSaves : MonoBehaviour
{
	// Token: 0x06001226 RID: 4646 RVA: 0x000A4240 File Offset: 0x000A2440
	private void Start()
	{
		this.saveFileInfoPanel = this.saveFileInfo.GetComponentInChildren<Image>();
		List<string> list = StatsManager.instance.SaveFileGetAll();
		float num = 0f;
		foreach (string text in list)
		{
			GameObject gameObject = Object.Instantiate<GameObject>(this.saveFilePrefab, this.Scroller);
			gameObject.transform.localPosition = this.saveFilePosition.localPosition;
			gameObject.transform.SetSiblingIndex(3);
			MenuElementSaveFile component = gameObject.GetComponent<MenuElementSaveFile>();
			component.saveFileName = text;
			string text2 = StatsManager.instance.SaveFileGetTeamName(text);
			string text3 = StatsManager.instance.SaveFileGetDateAndTime(text);
			int num2 = int.Parse(StatsManager.instance.SaveFileGetRunLevel(text)) + 1;
			component.saveFileHeaderDate.text = text3;
			string text4 = ColorUtility.ToHtmlStringRGB(SemiFunc.ColorDifficultyGet(1f, 10f, (float)num2));
			float time = StatsManager.instance.SaveFileGetTimePlayed(text);
			component.saveFileHeaderLevel.text = string.Concat(new string[]
			{
				"<sprite name=truck> <color=#",
				text4,
				">",
				num2.ToString(),
				"</color>"
			});
			component.saveFileHeader.text = text2;
			Color numberColor = new Color(0.1f, 0.4f, 0.8f);
			Color unitColor = new Color(0.05f, 0.3f, 0.6f);
			component.saveFileInfoRow1.text = "<sprite name=clock>  " + SemiFunc.TimeToString(time, true, numberColor, unitColor);
			gameObject.transform.localPosition = new Vector3(gameObject.transform.localPosition.x, gameObject.transform.localPosition.y + num, gameObject.transform.localPosition.z);
			float num3 = gameObject.GetComponent<RectTransform>().rect.height + 2f;
			num -= num3;
			this.saveFileYOffset = num3;
			this.saveFiles.Add(gameObject.GetComponent<MenuElementSaveFile>());
		}
		if (SemiFunc.MainMenuIsMultiplayer())
		{
			this.gameModeHeader.text = "Multiplayer mode";
			return;
		}
		this.gameModeHeader.text = "Singleplayer mode";
	}

	// Token: 0x06001227 RID: 4647 RVA: 0x000A44A0 File Offset: 0x000A26A0
	public void OnGoBack()
	{
		MenuManager.instance.PageCloseAll();
		MenuManager.instance.PageOpen(MenuPageIndex.Main, false);
	}

	// Token: 0x06001228 RID: 4648 RVA: 0x000A44BC File Offset: 0x000A26BC
	private void Update()
	{
		if (SemiFunc.InputDown(InputKey.Back) && MenuManager.instance.currentMenuPageIndex == MenuPageIndex.Saves)
		{
			this.OnGoBack();
		}
		if (this.saveFileInfoPanel.color != new Color(0f, 0f, 0f, 1f))
		{
			this.saveFileInfoPanel.color = Color.Lerp(this.saveFileInfoPanel.color, new Color(0f, 0f, 0f, 1f), Time.deltaTime * 10f);
		}
	}

	// Token: 0x06001229 RID: 4649 RVA: 0x000A4550 File Offset: 0x000A2750
	public void OnNewGame()
	{
		if (this.saveFiles.Count >= 10)
		{
			MenuManager.instance.PageCloseAllAddedOnTop();
			MenuManager.instance.PagePopUp("Save file limit reached", Color.red, "You can only have 10 save files at a time. Please delete some save files to make room for new ones.", "OK", true);
			return;
		}
		if (SemiFunc.MainMenuIsMultiplayer())
		{
			SemiFunc.MenuActionHostGame(null);
			return;
		}
		SemiFunc.MenuActionSingleplayerGame(null);
	}

	// Token: 0x0600122A RID: 4650 RVA: 0x000A45AA File Offset: 0x000A27AA
	public void OnLoadGame()
	{
		if (SemiFunc.MainMenuIsMultiplayer())
		{
			SemiFunc.MenuActionHostGame(this.currentSaveFileName);
			return;
		}
		SemiFunc.MenuActionSingleplayerGame(this.currentSaveFileName);
	}

	// Token: 0x0600122B RID: 4651 RVA: 0x000A45CC File Offset: 0x000A27CC
	public void OnDeleteGame()
	{
		if (this.currentSaveFileName.IsNullOrEmpty())
		{
			StatsManager.instance.SaveFileDelete("");
			MenuManager.instance.PageCloseAll();
			MenuManager.instance.PageOpen(MenuPageIndex.Main, false);
			return;
		}
		SemiFunc.SaveFileDelete(this.currentSaveFileName);
		bool flag = false;
		foreach (MenuElementSaveFile menuElementSaveFile in Enumerable.ToList<MenuElementSaveFile>(this.saveFiles))
		{
			if (flag && menuElementSaveFile)
			{
				RectTransform component = menuElementSaveFile.GetComponent<RectTransform>();
				component.localPosition = new Vector3(component.localPosition.x, component.localPosition.y + this.saveFileYOffset, component.localPosition.z);
				MenuElementAnimations component2 = menuElementSaveFile.GetComponent<MenuElementAnimations>();
				component2.UIAniNudgeY(10f, 0.2f, 1f);
				component2.UIAniRotate(2f, 0.2f, 1f);
				component2.UIAniNewInitialPosition(new Vector2(component.localPosition.x, component.localPosition.y));
			}
			if (menuElementSaveFile.saveFileName == this.currentSaveFileName)
			{
				Object.Destroy(menuElementSaveFile.gameObject);
				this.saveFiles.Remove(menuElementSaveFile);
				flag = true;
			}
		}
		this.GoBackToDefaultInfo();
	}

	// Token: 0x0600122C RID: 4652 RVA: 0x000A4734 File Offset: 0x000A2934
	public void GoBackToDefaultInfo()
	{
		MenuElementAnimations component = this.saveFileInfo.GetComponent<MenuElementAnimations>();
		component.UIAniNudgeX(10f, 0.2f, 1f);
		component.UIAniRotate(2f, 0.2f, 1f);
		this.saveInfoDefault.SetActive(true);
		this.saveInfoSelected.SetActive(false);
		this.saveFileInfoPanel.color = new Color(0.45f, 0f, 0f, 1f);
	}

	// Token: 0x0600122D RID: 4653 RVA: 0x000A47B4 File Offset: 0x000A29B4
	private void InfoPlayerNames(TextMeshProUGUI _textMesh, string _fileName)
	{
		_textMesh.text = "";
		List<string> list = StatsManager.instance.SaveFileGetPlayerNames(_fileName);
		list.Sort((string a, string b) => a.Length.CompareTo(b.Length));
		if (list != null)
		{
			int count = list.Count;
			int num = 0;
			foreach (string text in list)
			{
				if (num == count - 1)
				{
					_textMesh.text += text;
				}
				else if (num == count - 2)
				{
					_textMesh.text = _textMesh.text + text + "<color=#444444>   and   </color>";
				}
				else
				{
					_textMesh.text = _textMesh.text + text + "<color=#444444>,</color>   ";
				}
				num++;
			}
		}
		if (list == null || (list != null && list.Count == 0))
		{
			_textMesh.text += "You did it all alone!";
		}
	}

	// Token: 0x0600122E RID: 4654 RVA: 0x000A48C4 File Offset: 0x000A2AC4
	public void SaveFileSelected(string saveFileName)
	{
		MenuElementAnimations component = this.saveFileInfo.GetComponent<MenuElementAnimations>();
		component.UIAniNudgeX(10f, 0.2f, 1f);
		component.UIAniRotate(2f, 0.2f, 1f);
		this.saveInfoDefault.SetActive(false);
		this.saveInfoSelected.SetActive(true);
		this.saveFileInfoPanel.color = new Color(0f, 0.1f, 0.25f, 1f);
		string text = StatsManager.instance.SaveFileGetTeamName(saveFileName);
		string text2 = StatsManager.instance.SaveFileGetDateAndTime(saveFileName);
		this.saveFileHeader.text = text;
		this.saveFileHeader.color = new Color(1f, 0.54f, 0f);
		this.saveFileHeaderDate.text = text2;
		this.currentSaveFileName = saveFileName;
		string text3 = "      ";
		float time = StatsManager.instance.SaveFileGetTimePlayed(saveFileName);
		int num = int.Parse(StatsManager.instance.SaveFileGetRunLevel(saveFileName)) + 1;
		string text4 = ColorUtility.ToHtmlStringRGB(SemiFunc.ColorDifficultyGet(1f, 10f, (float)num));
		string text5 = StatsManager.instance.SaveFileGetRunCurrency(saveFileName);
		this.saveFileInfoRow1.text = string.Concat(new string[]
		{
			"<sprite name=truck>  <color=#",
			text4,
			"><b>",
			num.ToString(),
			"</b></color>"
		});
		TextMeshProUGUI textMeshProUGUI = this.saveFileInfoRow1;
		textMeshProUGUI.text += text3;
		TextMeshProUGUI textMeshProUGUI2 = this.saveFileInfoRow1;
		textMeshProUGUI2.text = textMeshProUGUI2.text + "<sprite name=clock>  " + SemiFunc.TimeToString(time, true, new Color(0.1f, 0.4f, 0.8f), new Color(0.05f, 0.3f, 0.6f));
		TextMeshProUGUI textMeshProUGUI3 = this.saveFileInfoRow1;
		textMeshProUGUI3.text += text3;
		string text6 = ColorUtility.ToHtmlStringRGB(new Color(0.2f, 0.5f, 0.3f));
		TextMeshProUGUI textMeshProUGUI4 = this.saveFileInfoRow1;
		textMeshProUGUI4.text = string.Concat(new string[]
		{
			textMeshProUGUI4.text,
			"<sprite name=$$>  <b>",
			text5,
			"</b><color=#",
			text6,
			">k</color>"
		});
		string text7 = SemiFunc.DollarGetString(int.Parse(StatsManager.instance.SaveFileGetTotalHaul(saveFileName)));
		this.saveFileInfoRow2.text = string.Concat(new string[]
		{
			"<color=#",
			text6,
			"><sprite name=$$$> TOTAL HAUL:      <b></b>$ </color><b>",
			text7,
			"</b><color=#",
			text6,
			">k</color>"
		});
		this.InfoPlayerNames(this.saveFileInfoRow3, saveFileName);
	}

	// Token: 0x04001E9D RID: 7837
	public RectTransform saveFileInfo;

	// Token: 0x04001E9E RID: 7838
	public GameObject saveInfoDefault;

	// Token: 0x04001E9F RID: 7839
	public GameObject saveInfoSelected;

	// Token: 0x04001EA0 RID: 7840
	public TextMeshProUGUI saveFileHeader;

	// Token: 0x04001EA1 RID: 7841
	public TextMeshProUGUI saveFileHeaderDate;

	// Token: 0x04001EA2 RID: 7842
	public TextMeshProUGUI saveFileInfoRow1;

	// Token: 0x04001EA3 RID: 7843
	public TextMeshProUGUI saveFileInfoRow2;

	// Token: 0x04001EA4 RID: 7844
	public TextMeshProUGUI saveFileInfoRow3;

	// Token: 0x04001EA5 RID: 7845
	private Image saveFileInfoPanel;

	// Token: 0x04001EA6 RID: 7846
	public RectTransform Scroller;

	// Token: 0x04001EA7 RID: 7847
	public RectTransform saveFilePosition;

	// Token: 0x04001EA8 RID: 7848
	public GameObject saveFilePrefab;

	// Token: 0x04001EA9 RID: 7849
	internal string currentSaveFileName;

	// Token: 0x04001EAA RID: 7850
	internal List<MenuElementSaveFile> saveFiles = new List<MenuElementSaveFile>();

	// Token: 0x04001EAB RID: 7851
	internal float saveFileYOffset;

	// Token: 0x04001EAC RID: 7852
	public TextMeshProUGUI gameModeHeader;
}
