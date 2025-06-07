using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000016 RID: 22
[CreateAssetMenu(fileName = "Level Music - _____", menuName = "Level/Level Music Preset", order = 3)]
public class LevelMusicAsset : ScriptableObject
{
	// Token: 0x04000090 RID: 144
	public List<LevelMusic.LevelMusicTrack> tracks;
}
