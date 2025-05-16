namespace ClydeMenu.Engine;

public static class MyExtensions
{
    public static int CHEAT_GetHealth(this PlayerAvatar avatar)
        => ClientInstance.FetchFieldValue<int, PlayerHealth>("health", avatar.playerHealth);

    public static int CHEAT_GetMaxHealth(this PlayerAvatar avatar)
        => ClientInstance.FetchFieldValue<int, PlayerHealth>("maxHealth", avatar.playerHealth);

    public static void CHEAT_SetHealth(this PlayerAvatar avatar, int health)
        => ClientInstance.GetPhotonView(avatar.playerHealth).RPC("UpdateHealthRPC", Photon.Pun.RpcTarget.All, [health, avatar.CHEAT_GetMaxHealth(), false]);

    public static void CHEAT_SetMaxHealth(this PlayerAvatar avatar, int maxHealth)
        => ClientInstance.GetPhotonView(avatar.playerHealth).RPC("UpdateHealthRPC", Photon.Pun.RpcTarget.All, [avatar.CHEAT_GetHealth(), maxHealth, false]);
}
