using System;
using TMPro;
using UnityEngine;

// Token: 0x0200028A RID: 650
public class Timecode : MonoBehaviour
{
	// Token: 0x06001453 RID: 5203 RVA: 0x000B3828 File Offset: 0x000B1A28
	public Timecode.TimeSnapshot GetSnapshot()
	{
		return new Timecode.TimeSnapshot
		{
			TimecodeSecond = Mathf.FloorToInt(this.timeSec),
			TimecodeMinute = Mathf.RoundToInt(this.timeMin),
			TimecodeHour = Mathf.RoundToInt(this.timeHour),
			TimeMinute = this.time.minute,
			TimeHour = this.time.hour
		};
	}

	// Token: 0x06001454 RID: 5204 RVA: 0x000B3890 File Offset: 0x000B1A90
	public void SetTime(Timecode.TimeSnapshot snapshot)
	{
		this.timeSec = (float)snapshot.TimecodeSecond;
		this.timeMin = (float)snapshot.TimecodeMinute;
		this.timeHour = (float)snapshot.TimecodeHour;
		this.time.minute = snapshot.TimeMinute;
		this.time.hour = snapshot.TimeHour;
	}

	// Token: 0x06001455 RID: 5205 RVA: 0x000B38E6 File Offset: 0x000B1AE6
	public void SetToStartSnapshot()
	{
		this.SetTime(this.StartSnapshot);
	}

	// Token: 0x06001456 RID: 5206 RVA: 0x000B38F4 File Offset: 0x000B1AF4
	private void Update()
	{
		if (!this.RewindEffect.PlayRewind && GameDirector.instance.currentState < GameDirector.gameState.Outro)
		{
			if (this.SetStartSnapshot)
			{
				this.StartSnapshot = this.GetSnapshot();
				this.SetStartSnapshot = false;
			}
			this.timeSec += Time.deltaTime;
			if (Mathf.Round(this.timeSec) >= 60f)
			{
				this.timeSec = 0f;
				this.timeMin += 1f;
				this.time.minute++;
				if (this.time.minute >= 60)
				{
					this.time.minute = 0;
					this.time.hour++;
					if (this.time.hour >= 24)
					{
						this.time.hour = 0;
						this.date.UpdateDay();
					}
				}
				if (this.timeMin >= 60f)
				{
					this.timeMin = 0f;
					this.timeHour += 1f;
				}
			}
		}
		string text = this.timeHour.ToString();
		if (this.timeHour < 10f)
		{
			text = "0" + text;
		}
		string text2 = this.timeMin.ToString();
		if (this.timeMin < 10f)
		{
			text2 = "0" + text2;
		}
		float num = Mathf.Round(this.timeSec);
		string text3 = num.ToString();
		if (num < 10f)
		{
			text3 = "0" + text3;
		}
		this.textMesh.text = string.Concat(new string[]
		{
			text,
			":",
			text2,
			":",
			text3
		});
	}

	// Token: 0x040022DB RID: 8923
	public RewindEffect RewindEffect;

	// Token: 0x040022DC RID: 8924
	public TextMeshProUGUI textMesh;

	// Token: 0x040022DD RID: 8925
	public CurrentTime time;

	// Token: 0x040022DE RID: 8926
	public Date date;

	// Token: 0x040022DF RID: 8927
	private float timeSec;

	// Token: 0x040022E0 RID: 8928
	private float timeMin;

	// Token: 0x040022E1 RID: 8929
	private float timeHour;

	// Token: 0x040022E2 RID: 8930
	private bool SetStartSnapshot = true;

	// Token: 0x040022E3 RID: 8931
	private Timecode.TimeSnapshot StartSnapshot;

	// Token: 0x0200040A RID: 1034
	[Serializable]
	public class TimeSnapshot
	{
		// Token: 0x04002D87 RID: 11655
		public int TimecodeHour;

		// Token: 0x04002D88 RID: 11656
		public int TimecodeMinute;

		// Token: 0x04002D89 RID: 11657
		public int TimecodeSecond;

		// Token: 0x04002D8A RID: 11658
		public int TimeHour;

		// Token: 0x04002D8B RID: 11659
		public int TimeMinute;
	}
}
