namespace ClydeMenu.Engine;

using System;
using HarmonyLib;

using UnityEngine.SceneManagement;

[ClydeChange("Added DebugWorld so you can experiment easily (F8)", ClydeVersion.Release_v1_5)]
internal class DebugWorld
{
    internal static void Load()
    {
        Patches.isDebugWorld = true;
        RunManager.instance.levelCurrent = RunManager.instance.levelLobby;
        RunManager.instance.ResetProgress();
        GameManager.instance.SetGameMode(0);
        SceneManager.LoadScene("Main");
        RunManager.instance.ChangeLevel(true, false, RunManager.ChangeLevelType.Tutorial);
    }

    internal static void OnDebugStart(PlayerController __instance)
    {
        Entry.Log("DebugWorld loaded successfully");

        PlayerController.instance.EnergyStart = 420;
        PlayerController.instance.EnergyCurrent = 69;
        PlayerController.instance.EnergySprintDrain = 0;

        PlayerController.instance.SprintSpeed = 10;

        for (var i = 0; i < 17; i++)
            TutorialDirector.instance.NextPage();
    }
}
