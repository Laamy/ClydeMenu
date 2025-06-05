namespace ClydeMenu.Engine;

using HarmonyLib;

using UnityEngine.SceneManagement;

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
}
