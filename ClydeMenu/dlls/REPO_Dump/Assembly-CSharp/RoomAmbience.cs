using System;
using UnityEngine;

// Token: 0x0200001F RID: 31
[CreateAssetMenu(fileName = "Room Ambience - _____", menuName = "Audio/Room Ambience", order = 0)]
public class RoomAmbience : ScriptableObject
{
	// Token: 0x04000107 RID: 263
	[Range(0f, 2f)]
	public float volume = 1f;
}
