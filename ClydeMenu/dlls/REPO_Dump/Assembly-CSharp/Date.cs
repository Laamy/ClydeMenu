using System;
using TMPro;
using UnityEngine;

// Token: 0x02000275 RID: 629
public class Date : MonoBehaviour
{
	// Token: 0x060013DC RID: 5084 RVA: 0x000AFF34 File Offset: 0x000AE134
	private void Start()
	{
		this.year = Random.Range(this.yearMin, this.yearMax);
		this.currentMonth = Random.Range(0, this.Months.Length);
		this.currentDay = Random.Range(1, this.Days[this.currentMonth]);
		this.UpdateText();
	}

	// Token: 0x060013DD RID: 5085 RVA: 0x000AFF8C File Offset: 0x000AE18C
	public void UpdateDay()
	{
		this.currentDay++;
		if (this.currentDay > this.Days[this.currentMonth])
		{
			this.currentDay = 1;
			this.currentMonth++;
			if (this.currentMonth >= this.Months.Length)
			{
				this.currentMonth = 0;
				this.year++;
			}
		}
		this.UpdateText();
	}

	// Token: 0x060013DE RID: 5086 RVA: 0x000AFFFC File Offset: 0x000AE1FC
	private void UpdateText()
	{
		this.textMesh.text = this.Months[this.currentMonth] + this.currentDay.ToString() + " " + this.year.ToString();
	}

	// Token: 0x04002219 RID: 8729
	public TextMeshProUGUI textMesh;

	// Token: 0x0400221A RID: 8730
	public int yearMin;

	// Token: 0x0400221B RID: 8731
	public int yearMax;

	// Token: 0x0400221C RID: 8732
	private int year;

	// Token: 0x0400221D RID: 8733
	public string[] Months;

	// Token: 0x0400221E RID: 8734
	public int[] Days;

	// Token: 0x0400221F RID: 8735
	private int currentMonth;

	// Token: 0x04002220 RID: 8736
	private int currentDay;
}
