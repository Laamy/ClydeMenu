namespace ClydeMenu.Engine.Menu;

using System;
using System.Collections.Generic;

using ExitGames.Client.Photon;

using Photon.Pun;
using Photon.Realtime;

using UnityEngine;

class MenuStorage
{
    public static Action[] renderActions;
    public static string[] renderNames;

    public static int selectedCategory = 0;

    public static Vector2 menuPos = new(10, 10);
    public static Vector2 menuSize = new(400, 340);
}

public class MainMenu : BaseMenu
{
    public override void OnPop() {}
    public override void OnPush() {}
    public override void OnUpdate() {}

    // ui comps
    private float sidebarWidth = 110f;
    private float titleBarHeight = 28f;
    private float padding = 4f;

    private Vector2 contentStart = Vector2.zero;
    private float yCursor = 0;

    private void DrawCategory()
    {
        if (MenuStorage.selectedCategory < 0 || MenuStorage.selectedCategory >= MenuStorage.renderActions.Length)
            MenuStorage.selectedCategory = Mathf.Clamp(MenuStorage.selectedCategory, 0, MenuStorage.renderActions.Length - 1);

        MenuStorage.renderActions[MenuStorage.selectedCategory]?.Invoke();
    }

    public MainMenu()
    {
        MenuStorage.renderNames =
        [
            "Visuals",
            "Network",
            "Player",
            "Misc"
        ];

        MenuStorage.renderActions =
        [
           //DrawSettingLabel("Combat");
           //Storage.Example_SelectionBox = DrawPlayerSelect("Player Selection", Storage.Example_SelectionBox);
           //
           //if (DrawButton("Button"))
           //    Console.WriteLine($"Player is {SemiFunc.PlayerGetName(GetPlayerFromID(Storage.Example_SelectionBox))}");
           //
           //Storage.Example_Slider70 = DrawSlider("Slider70", Storage.Example_Slider70);
           //Storage.Example_Boolean = DrawBoolean("Booleanoff", Storage.Example_Boolean);
            () => {
                DrawSettingLabel("Visual");
                Storage.CHEAT_ESP_Player = DrawBoolean("Player ESP", Storage.CHEAT_ESP_Player);
                Storage.CHEAT_ESP_Enemy = DrawBoolean("Enemy ESP", Storage.CHEAT_ESP_Enemy);
                Storage.CHEAT_ESP_Valuable = DrawBoolean("Valuable ESP", Storage.CHEAT_ESP_Valuable);
                Storage.CHEAT_ESP_Extraction = DrawBoolean("Extraction ESP", Storage.CHEAT_ESP_Extraction);
            },
            () => {
                DrawSettingLabel("Network");
                Storage.CHEAT_PLAYERSELECT = DrawPlayerSelect("Select Player", Storage.CHEAT_PLAYERSELECT);
                if (DrawButton("Kick Player"))
                {
                    try
                    {
                        var plyr = SemiFunc.PlayerGetList()[Storage.CHEAT_PLAYERSELECT];
                        var plyrActorId = plyr.photonView.OwnerActorNr;

                        var options = new RaiseEventOptions();
                        options.TargetActors = new[] { plyrActorId };
                        PhotonNetwork.RaiseEvent(199, null, options, SendOptions.SendReliable);
                        Console.WriteLine($"Kicked player {plyr.name} from the server.");
                    }
                    catch (Exception ex)
                    {
                         Console.WriteLine($"Error in MainMenu {ex.Message}");
                    }
                }

                if (DrawButton("Steal Crown"))
                {
                    try
                    {
                        var plyr = SemiFunc.PlayerGetList()[Storage.CHEAT_PLAYERSELECT];

                        var view = ClientInstance.GetPhotonView(PunManager.instance);
                        view.RPC("CrownPlayerRPC", RpcTarget.AllBuffered, [SemiFunc.PlayerGetSteamID(plyr)]);
                        Console.WriteLine($"Gave crown to player {plyr.name}.");
                    }
                    catch (Exception ex)
                    {
                         Console.WriteLine($"Error in MainMenu {ex.Message}");
                    }
                }

                DrawSettingLabel("Server stuff");

                if (DrawButton("Crash server"))
                {
                    for (var i = 0; i < 0xFFF; i++)
                        ItemUtils.SpawnSurplus(Vector2.zero, false);
                }

                if (DrawButton("Everyones a cheater (Upgrades)"))
                {
                    var view = ClientInstance.GetPhotonView(PunManager.instance);

                    foreach (var plyr in SemiFunc.PlayerGetList())
                    {
                        var steamId = SemiFunc.PlayerGetSteamID(plyr);
                        view.RPC("UpgradeItemBatteryRPC", RpcTarget.AllBuffered, [steamId, 255]);
                        view.RPC("UpgradePlayerExtraJumpRPC", RpcTarget.AllBuffered, [steamId, 255]);
                        view.RPC("UpgradePlayerTumbleLaunchRPC", RpcTarget.AllBuffered, [steamId, 255]);
                        view.RPC("UpgradePlayerSprintSpeedRPC", RpcTarget.AllBuffered, [steamId, 255]);
                        view.RPC("UpgradePlayerGrabStrengthRPC", RpcTarget.AllBuffered, [steamId, 255]);
                        view.RPC("UpgradePlayerGrabRangeRPC", RpcTarget.AllBuffered, [steamId, 255]);
                        view.RPC("UpgradeMapPlayerCountRPC", RpcTarget.AllBuffered, [steamId, 255]);
                        view.RPC("UpgradePlayerThrowStrengthRPC", RpcTarget.AllBuffered, [steamId, 255]);
                        view.RPC("UpgradePlayerEnergyRPC", RpcTarget.AllBuffered, [steamId, 255]);
                        view.RPC("UpgradePlayerHealthRPC", RpcTarget.AllBuffered, [steamId, 255]);
                    }
                }
            },
            () => {
                DrawSettingLabel("Player");
                Storage.CHEAT_PLAYER_Namespoof = DrawBoolean("NameSpoof", Storage.CHEAT_PLAYER_Namespoof);
            },
            () => {
                DrawSettingLabel("Misc");
                if (DrawButton("Infinite Health"))
                    ClientInstance.SetHealth(99999,99999);

                if (DrawButton("Reset Health"))
                    ClientInstance.SetHealth(100,100);
            }
        ];
    }

    private PlayerAvatar GetPlayerFromID(int plyr)
    {
        var players = SemiFunc.PlayerGetList();
        if (plyr < 0 || plyr >= players.Count)
            return null;
        return players[plyr];
    }

    private Vector2 oldMouse;
    private bool isDragging = false;
    public void HandleBorder()
    {
        var curEvent = Event.current;
        Vector2 mousePos = curEvent.mousePosition;

        Rect titleBarRect = new Rect(MenuStorage.menuPos.x, MenuStorage.menuPos.y, MenuStorage.menuSize.x, titleBarHeight);

        switch (curEvent.type)
        {
            case EventType.MouseDown:
                if (titleBarRect.Contains(mousePos))
                {
                    isDragging = true;
                    oldMouse = mousePos;
                }
                break;
            case EventType.MouseDrag:
                if (isDragging)
                {
                    Vector2 delta = mousePos - oldMouse;
                    MenuStorage.menuPos += delta;
                    oldMouse = mousePos;
                }
                break;
            case EventType.MouseUp:
                if (isDragging)
                {
                    isDragging = false;
                }
                break;
        }
    }

    public override void Render()
    {
        HandleBorder();
        RenderUtils.DrawRect(MenuStorage.menuPos, MenuStorage.menuSize, StyleTheme.ContentBoxBackground);

        RenderUtils.DrawRect(MenuStorage.menuPos, new Vector2(MenuStorage.menuSize.x, titleBarHeight), StyleTheme.Titlebar);
        RenderUtils.DrawString(new Vector2(MenuStorage.menuPos.x + 12, MenuStorage.menuPos.y + 6), "clickgui debug", StyleTheme.TitlebarText);
        RenderUtils.DrawRect(new Vector2(MenuStorage.menuPos.x + MenuStorage.menuSize.x - 17, MenuStorage.menuPos.y + 6), new Vector2(12, 12), StyleTheme.TitlebarCloseButton);
        
        if (LabelPressed("TitlebarCloseButton", new Rect(MenuStorage.menuPos.x + MenuStorage.menuSize.x - 17, MenuStorage.menuPos.y + 6, 12, 12)))
            MenuSceneComponent.Instance.PopMenu(this);

        var sidebarPos = new Vector2(MenuStorage.menuPos.x, MenuStorage.menuPos.y + titleBarHeight);
        var sidebarHeight = MenuStorage.menuSize.y - titleBarHeight;
        RenderUtils.DrawRect(sidebarPos, new Vector2(sidebarWidth, sidebarHeight), StyleTheme.Sidebar);

        for (int i = 0; i < MenuStorage.renderNames.Length; i++)
        {
            float y = sidebarPos.y + padding + i * 27;
            var isSelected = i == MenuStorage.selectedCategory;
            var bgCol = isSelected ? StyleTheme.SidebarButton : StyleTheme.SidebarButtonSelected;
            RenderUtils.DrawRect(new Vector2(sidebarPos.x + 6, y), new Vector2(sidebarWidth - 12, 24), bgCol);

            if (LabelPressed(MenuStorage.renderNames[i], new Rect(sidebarPos.x + 6, y, sidebarWidth - 12, 24)))
            {
                MenuStorage.selectedCategory = i;
                Event.current.Use();
            }

            var textColor = isSelected ? StyleTheme.MenuText : StyleTheme.MenuTextSelected;
            RenderUtils.DrawString(new Vector2(sidebarPos.x + 16, y + 5), MenuStorage.renderNames[i], textColor);
        }

        contentStart = new Vector2(MenuStorage.menuPos.x + sidebarWidth + padding, MenuStorage.menuPos.y + titleBarHeight + padding);
        yCursor = contentStart.y;

        DrawCategory();
    }

    void DrawSettingLabel(string text)
    {
        RenderUtils.DrawString(new Vector2(contentStart.x, yCursor), text, StyleTheme.MenuText);
        yCursor += 20;
    }

    float DrawSlider(string label, float percent)
    {
        var barWidth = 200f;
        var barHeight = 12f;
        var sliderX = contentStart.x;
        var filledWidth = barWidth * percent;
        DrawSettingLabel(label);

        RenderUtils.DrawRect(new Vector2(sliderX, yCursor), new Vector2(barWidth, barHeight), StyleTheme.SliderBackground);
        RenderUtils.DrawRect(new Vector2(sliderX, yCursor), new Vector2(filledWidth, barHeight), StyleTheme.SliderFilled);
        RenderUtils.DrawRect(new Vector2(sliderX + filledWidth - 2, yCursor - 2), new Vector2(4, barHeight + 4), StyleTheme.SliderHolder);

        Rect sliderHolderRect = new Rect(sliderX, yCursor - 2, barWidth, barHeight + 4);
        var cur = Event.current;
        Vector2 mousePos = cur.mousePosition;
        if (cur.type == EventType.MouseDrag && sliderHolderRect.Contains(mousePos))
        {
            Console.WriteLine("Slider clicked");
            percent = Mathf.Clamp01((mousePos.x - sliderX) / barWidth);
            cur.Use();
        }

        string valueText = $"{Math.Round(percent * 100)}%";
        var valueSize = RenderUtils.StringSize(valueText);
        var valueX = sliderX + barWidth - valueSize.x - 4;
        var valueY = yCursor - 1;
        RenderUtils.DrawString(new Vector2(valueX, valueY), valueText, StyleTheme.MenuTextDark);

        yCursor += barHeight + padding;
        return percent;
    }

    private Dictionary<string, bool> buttonStates = new();
    bool DrawBoolean(string label, bool cur)
    {
        DrawSettingLabel(label);
        RenderUtils.DrawRect(new Vector2(contentStart.x, yCursor), new Vector2(16, 16), cur == true ? StyleTheme.BooleanOn : StyleTheme.BooleanOff);
        RenderUtils.DrawString(new Vector2(contentStart.x + 20, yCursor + 1), cur == true ? "[ ]" : "[x]", new Color(1, 1, 1));

        if (LabelPressed(label, new Rect(contentStart.x, yCursor, 16, 16)))
            cur = !cur;

        yCursor += 20 + padding;
        return cur;
    }

    bool LabelPressed(string selectionKey, Rect buttonRect)
    {
        var curEvent = Event.current;
        Vector2 mousePos = curEvent.mousePosition;

        if (!buttonStates.ContainsKey(selectionKey))
            buttonStates[selectionKey] = false;

        if (curEvent.type == EventType.MouseDown && buttonRect.Contains(mousePos))
        {
            if (!buttonStates[selectionKey])
            {
                buttonStates[selectionKey] = true;
                return true;
            }
        }
        else if (curEvent.type == EventType.MouseUp && buttonStates[selectionKey])
        {
            buttonStates[selectionKey] = false;
        }

        return false;
    }

    private bool IsHeld(string label)
    {
        if (!buttonStates.ContainsKey(label))
            buttonStates[label] = false;

        return buttonStates[label];
    }

    int DrawPlayerSelect(string label, int selection)
    {
        DrawSettingLabel(label);
        RenderUtils.DrawRect(new Vector2(contentStart.x, yCursor), new Vector2(MenuStorage.menuSize.x - sidebarWidth - (padding * 2), 6 * 20), StyleTheme.ContentBox);

        var players = SemiFunc.PlayerGetList();
        if (selection < 0 || selection >= players.Count)
            selection = Mathf.Clamp(selection, 0, players.Count - 1);

        int i = 0;
        foreach (var player in players)
        {
            var isSelected = i == selection;
            RenderUtils.DrawRect(new Vector2(contentStart.x, yCursor), new Vector2(MenuStorage.menuSize.x - sidebarWidth - (padding * 2), 20), isSelected == true ? StyleTheme.SidebarButton : StyleTheme.Sidebar);

            var curEvent = Event.current;
            Vector2 mousePos = curEvent.mousePosition;
            Rect buttonRect = new Rect(contentStart.x, yCursor, MenuStorage.menuSize.x - sidebarWidth - (padding * 2), 20);
            var selectionKey = $"{label}_{i}";

            if (LabelPressed(selectionKey, buttonRect))
            {
                selection = i;
                curEvent.Use();
            }

            var plyrName = SemiFunc.PlayerGetName(player);

            if (PhotonNetwork.IsConnected && PhotonNetwork.LocalPlayer != null && PhotonNetwork.MasterClient != null)
            {
                if (PhotonNetwork.LocalPlayer.ActorNumber == player.photonView.OwnerActorNr)
                    plyrName += " [LOCAL]";

                if (PhotonNetwork.MasterClient.ActorNumber == player.photonView.OwnerActorNr)
                    plyrName += " [HOST]";
            }
            else plyrName += " [LOCAL]";

                var strMsurs = RenderUtils.StringSize(plyrName);
            RenderUtils.DrawString(new Vector2(contentStart.x + padding, yCursor + ((20 / 2) - strMsurs.y / 2)), plyrName, StyleTheme.MenuTextDark);
            yCursor += 20;
            i++;
        }

        yCursor += (6 - Math.Min(players.Count, 6)) * 20;
        yCursor += padding;
        return selection;
    }

    bool DrawButton(string label)
    {
        var strMsurs = RenderUtils.StringSize(label);
        var curEvent = Event.current;
        Vector2 mousePos = curEvent.mousePosition;
        Rect buttonRect = new Rect(contentStart.x, yCursor, strMsurs.x + (padding * 2), 25);

        var style = IsHeld(label) ? StyleTheme.SidebarButton : StyleTheme.Sidebar;
        RenderUtils.DrawRect(new Vector2(contentStart.x, yCursor), new Vector2(strMsurs.x + (padding * 2), 25), style);
        RenderUtils.DrawString(new Vector2(contentStart.x + padding, yCursor + ((25 / 2f) - strMsurs.y / 2f)), label, StyleTheme.MenuText);

        yCursor += 25 + padding;
        return LabelPressed(label, buttonRect);
    }

    class StyleTheme
    {
        public static Color MenuTextDark = new(0.8f, 0.8f, 0.8f);
        public static Color MenuText = new(1, 1, 1);
        public static Color MenuTextSelected = new(0.7f, 0.7f, 0.8f);

        public static Color TitlebarCloseButton = new(0.8f, 0.3f, 0.3f);
        public static Color Titlebar = new(0.15f, 0.15f, 0.25f);
        public static Color TitlebarText = new(0.9f, 0.9f, 1);

        public static Color Sidebar = new(0.1f, 0.1f, 0.2f);
        public static Color SidebarButton = new(0.2f, 0.2f, 0.35f);
        public static Color SidebarButtonSelected = new(0.12f, 0.12f, 0.2f);

        public static Color ContentBoxBackground = new(0.05f, 0.05f, 0.08f);

        public static Color ContentBox = new(0.1f, 0.1f, 0.18f); // player selection

        public static Color BooleanOn = new(0.2f, 0.6f, 0.2f); // booleans
        public static Color BooleanOff = new(0.6f, 0.2f, 0.2f);

        public static Color SliderHolder = new(1, 1, 1); // sliders
        public static Color SliderFilled = new(0.5f, 0.2f, 1f);
        public static Color SliderBackground = new(0.15f, 0.15f, 0.2f);
    }
}
