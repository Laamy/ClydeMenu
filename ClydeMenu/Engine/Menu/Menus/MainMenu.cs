namespace ClydeMenu.Engine.Menu;

using System;
using System.Collections.Generic;
using System.Linq;

using ClydeMenu.Engine.Settings;

using Photon.Pun;

using UnityEngine;

class MenuStorage
{
    public static Action[] renderActions;
    public static string[] renderNames;

    public static int selectedCategory = 0;

    public static Vector2 menuPos = new(10, 10);
    public static Vector2 menuSize = new(400, 400);
}

class ThemeConfig
{
    public Color MenuTextDark = new(0.8f, 0.8f, 0.8f);
    public Color MenuText = new(1, 1, 1);
    public Color MenuTextSelected = new(0.7f, 0.7f, 0.8f);

    public Color TitlebarCloseButton = new(0.8f, 0.3f, 0.3f);
    public Color Titlebar = new(0.15f, 0.15f, 0.25f);
    public Color TitlebarText = new(0.9f, 0.9f, 1);

    public Color Sidebar = new(0.1f, 0.1f, 0.2f);
    public Color SidebarButton = new(0.2f, 0.2f, 0.35f);
    public Color SidebarButtonSelected = new(0.12f, 0.12f, 0.2f);

    public Color ContentBoxBackground = new(0.05f, 0.05f, 0.08f);

    public Color ContentBox = new(0.1f, 0.1f, 0.18f); // player selection

    public Color BooleanOn = new(0.2f, 0.6f, 0.2f); // booleans
    public Color BooleanOff = new(0.6f, 0.2f, 0.2f);

    public Color SliderHolder = new(1, 1, 1); // sliders
    public Color SliderFilled = new(0.5f, 0.2f, 1f);
    public Color SliderBackground = new(0.15f, 0.15f, 0.2f);

    public static ThemeConfig Create(
        Color basePrimary,
        Color baseSecondary,
        Color baseAccent,
        Color textPrimary,
        Color textSecondary,
        Color background,
        Color sidebarBackground,
        Color altBackground)
    {
        return new ThemeConfig
        {
            MenuTextDark = textSecondary,
            MenuText = textPrimary,
            MenuTextSelected = Color.Lerp(textPrimary, baseAccent, 0.2f),
            TitlebarCloseButton = Color.Lerp(baseAccent, new Color(1, 0, 0), 0.4f),
            Titlebar = basePrimary,
            TitlebarText = textPrimary,
            Sidebar = sidebarBackground,
            SidebarButton = baseSecondary,
            SidebarButtonSelected = Color.Lerp(baseSecondary, basePrimary, 0.3f),
            ContentBoxBackground = background,
            ContentBox = baseSecondary,
            BooleanOn = baseAccent,
            BooleanOff = Color.Lerp(baseAccent, background, 0.5f),
            SliderHolder = textPrimary,
            SliderFilled = baseAccent,
            SliderBackground = altBackground
        };
    }
}

[ClydeChange("New ClydeMenu (RightShift) clickgui with 6 themes", ClydeVersion.Release_v1_0)]
[ClydeChange("New MapStealer (Aka map downloader) useful for when your friends fall asleep as host", ClydeVersion.Release_v1_0)]
public class MainMenu : BaseMenu
{
    public override void OnPop() { }
    public override void OnPush() { }
    public override void OnUpdate() { }

    // ui comps
    private float sidebarWidth = 110f;
    private float titleBarHeight = 28f;
    private float padding = 4f;

    private Vector2 contentStart = Vector2.zero;
    private float yCursor = 0;

    private ThemeConfig StyleTheme = new();

    private void DrawCategory()
    {
        if (MenuStorage.selectedCategory < 0 || MenuStorage.selectedCategory >= MenuStorage.renderActions.Length)
            MenuStorage.selectedCategory = Mathf.Clamp(MenuStorage.selectedCategory, 0, MenuStorage.renderActions.Length - 1);

        MenuStorage.renderActions[MenuStorage.selectedCategory]?.Invoke();
    }

    public MainMenu()
    {
        if (Storage.InternalThemeStyle != null)
            StyleTheme = Storage.InternalThemeStyle;
        else
        {
            if (MenuSettings.GameTheme != null)
                StyleTheme = Storage.StyleThemes[MenuSettings.GameTheme.Value];
            else
                StyleTheme = Storage.StyleThemes["Dark"];
        }
        Storage.SETTINGS_Theme = Array.IndexOf(Storage.StyleThemes.Keys.ToArray(), MenuSettings.GameTheme.Value);

        MenuStorage.renderNames =
        [
            "Visual",
            "Privacy",
            "Server",
            "Waypoints",
            "Settings",
            //"DebugTab",
        ];

        if (ClydeVersion.IsDebug)
        {
            var old = MenuStorage.renderNames.ToList();
            old.Add("Debug (DEV");
            MenuStorage.renderNames = old.ToArray();
        }

        MenuStorage.renderActions =
        [
           //DrawSettingLabel("Combat");
           //Storage.Example_SelectionBox = DrawPlayerSelect("Player Selection", Storage.Example_SelectionBox);
           //
           //if (DrawButton("Button"))
           //    Entry.Log($"Player is {SemiFunc.PlayerGetName(GetPlayerFromID(Storage.Example_SelectionBox))}");
           //
           //Storage.Example_Slider70 = DrawSlider("Slider70", Storage.Example_Slider70);
           //Storage.Example_Boolean = DrawBoolean("Booleanoff", Storage.Example_Boolean);
            () => {
                DrawSettingLabel("Visual");
                MenuSettings.VISUAL_MAPINFO.Value = DrawBoolean("MapInfo (Lvl & maxHaul)", MenuSettings.VISUAL_MAPINFO.Value);
                MenuSettings.VISUAL_NOISELOGGER.Value = DrawBoolean("NoiseLogger", MenuSettings.VISUAL_NOISELOGGER.Value);
                MenuSettings.VISUAL_FreeLook.Value = DrawBoolean("Freelook (Hold ALT)", MenuSettings.VISUAL_FreeLook.Value);
                //Storage.DEBUGBOX = DrawNumberField("DebugBox", Storage.DEBUGBOX);
            },
            // might expose this tab if localhost
            //() => {
            //    DrawSettingLabel("Network");
            //    Storage.CHEAT_PLAYERSELECT = DrawPlayerSelect("Select Player", Storage.CHEAT_PLAYERSELECT);
            //
            //    //if (DrawExpandBox("Power Utils"))
            //    //{
            //    //    Storage.CHEAT_PLAYERSELECT_MSGSPOOF = DrawTextField("MessageSpoof", Storage.CHEAT_PLAYERSELECT_MSGSPOOF);
            //    //    if (DrawButton("Send Spoof"))
            //    //    {
            //    //        try
            //    //        {
            //    //            var plyr = SemiFunc.PlayerGetList()[Storage.CHEAT_PLAYERSELECT];
            //    //            ClientInstance.SpoofMsg(plyr, Storage.CHEAT_PLAYERSELECT_MSGSPOOF);
            //    //        }
            //    //        catch (Exception ex)
            //    //        {
            //    //             Entry.Log($"Error in MainMenu {ex.Message}");
            //    //        }
            //    //    }
            //    //}
            //    //
            //    //if (DrawExpandBox("Health Utils"))
            //    //{
            //    //    if (DrawButton("Kill Player"))
            //    //    {
            //    //        try
            //    //        {
            //    //            var plyr = SemiFunc.PlayerGetList()[Storage.CHEAT_PLAYERSELECT];
            //    //            ClientInstance.KillPlayer(plyr);
            //    //        }
            //    //        catch (Exception ex)
            //    //        {
            //    //             Entry.Log($"Error in MainMenu {ex.Message}");
            //    //        }
            //    //    }
            //    //
            //    //    if (DrawButton("Revive Player"))
            //    //    {
            //    //        try
            //    //        {
            //    //            var plyr = SemiFunc.PlayerGetList()[Storage.CHEAT_PLAYERSELECT];
            //    //            ClientInstance.RevivePlayer(plyr);
            //    //        }
            //    //        catch (Exception ex)
            //    //        {
            //    //             Entry.Log($"Error in MainMenu {ex.Message}");
            //    //        }
            //    //    }
            //    //
            //    //    if (DrawButton("Heal Player (+Amount)"))
            //    //    {
            //    //        try
            //    //        {
            //    //            var plyr = SemiFunc.PlayerGetList()[Storage.CHEAT_PLAYERSELECT];
            //    //            if (float.TryParse(Storage.CHEAT_NETWORK_HEALTHAMOUNT, out var value))
            //    //            {
            //    //                var nextHealthTick = Mathf.Clamp(plyr.CHEAT_GetHealth() + (int)Math.Floor(value), 0, plyr.CHEAT_GetMaxHealth());
            //    //                plyr.CHEAT_SetHealth(nextHealthTick);
            //    //            }
            //    //        }
            //    //        catch (Exception ex)
            //    //        {
            //    //             Entry.Log($"Error in MainMenu {ex.Message}");
            //    //        }
            //    //    }
            //    //
            //    //    if (DrawButton("Heal Player (MaxHealth)"))
            //    //    {
            //    //        try
            //    //        {
            //    //            var plyr = SemiFunc.PlayerGetList()[Storage.CHEAT_PLAYERSELECT];
            //    //                plyr.CHEAT_SetHealth(plyr.CHEAT_GetMaxHealth());
            //    //        }
            //    //        catch (Exception ex)
            //    //        {
            //    //             Entry.Log($"Error in MainMenu {ex.Message}");
            //    //        }
            //    //    }
            //    //
            //    //    Storage.CHEAT_NETWORK_HEALTHAMOUNT = DrawNumberField("Health Amount", Storage.CHEAT_NETWORK_HEALTHAMOUNT);
            //    //}
            //
            //    if (DrawExpandBox("Give Utils"))
            //    {
            //        if (DrawButton("Steal Crown"))
            //        {
            //            try
            //            {
            //                var plyr = SemiFunc.PlayerGetList()[Storage.CHEAT_PLAYERSELECT];
            //
            //                var view = ClientInstance.GetPhotonView(PunManager.instance);
            //                view.RPC("CrownPlayerRPC", RpcTarget.AllBuffered, [SemiFunc.PlayerGetSteamID(plyr)]);
            //                Entry.Log($"Gave crown to player {plyr.name}.");
            //            }
            //            catch (Exception ex)
            //            {
            //                 Entry.Log($"Error in MainMenu {ex.Message}");
            //            }
            //        }
            //
            //        if (DrawButton("Give all Upgrades(255)"))
            //        {
            //            try
            //            {
            //                var plyr = SemiFunc.PlayerGetList()[Storage.CHEAT_PLAYERSELECT];
            //
            //                var steamId = SemiFunc.PlayerGetSteamID(plyr);
            //                var view = ClientInstance.GetPhotonView(PunManager.instance);
            //
            //                view.RPC("UpgradeItemBatteryRPC", RpcTarget.AllBuffered, [steamId, 255]);
            //                view.RPC("UpgradePlayerExtraJumpRPC", RpcTarget.AllBuffered, [steamId, 255]);
            //                view.RPC("UpgradePlayerTumbleLaunchRPC", RpcTarget.AllBuffered, [steamId, 255]);
            //                view.RPC("UpgradePlayerSprintSpeedRPC", RpcTarget.AllBuffered, [steamId, 255]);
            //                view.RPC("UpgradePlayerGrabStrengthRPC", RpcTarget.AllBuffered, [steamId, 255]);
            //                view.RPC("UpgradePlayerGrabRangeRPC", RpcTarget.AllBuffered, [steamId, 255]);
            //                view.RPC("UpgradeMapPlayerCountRPC", RpcTarget.AllBuffered, [steamId, 255]);
            //                view.RPC("UpgradePlayerThrowStrengthRPC", RpcTarget.AllBuffered, [steamId, 255]);
            //                view.RPC("UpgradePlayerEnergyRPC", RpcTarget.AllBuffered, [steamId, 255]);
            //                view.RPC("UpgradePlayerHealthRPC", RpcTarget.AllBuffered, [steamId, 255]);
            //
            //                Entry.Log($"Gave all upgrades to player {plyr.name}.");
            //            }
            //            catch (Exception ex)
            //            {
            //                 Entry.Log($"Error in MainMenu {ex.Message}");
            //            }
            //        }
            //
            //        Storage.CHEAT_Upgrade_Type = DrawEnum("Upgrade", ItemUtils.Upgrades, Storage.CHEAT_Upgrade_Type);
            //
            //        Storage.CHEAT_Upgrade_Amount = DrawNumberField("Amount", Storage.CHEAT_Upgrade_Amount);
            //
            //        if (DrawButton("Give Upgrade")) {
            //            if (float.TryParse(Storage.CHEAT_Upgrade_Amount, out var amount))
            //            {
            //                var parsedAmount = (int)Math.Floor(Mathf.Clamp(amount, 0, 255));
            //
            //                var plyr = SemiFunc.PlayerGetList()[Storage.CHEAT_PLAYERSELECT];
            //
            //                var steamId = SemiFunc.PlayerGetSteamID(plyr);
            //                var view = ClientInstance.GetPhotonView(PunManager.instance);
            //
            //                var upgrade = $"Upgrade{ItemUtils.Upgrades[Storage.CHEAT_Upgrade_Type]}RPC";
            //                view.RPC(upgrade, RpcTarget.AllBuffered, [steamId, parsedAmount]);
            //            }
            //        }
            //    }
            //},
            //() => {
            //    DrawSettingLabel("Server");
            //
            //    if (DrawButton("Everyones a cheater (Upgrades)"))
            //    {
            //        var view = ClientInstance.GetPhotonView(PunManager.instance);
            //    
            //        foreach (var plyr in SemiFunc.PlayerGetList())
            //        {
            //            var steamId = SemiFunc.PlayerGetSteamID(plyr);
            //            view.RPC("UpgradeItemBatteryRPC", RpcTarget.AllBuffered, [steamId, 255]);
            //            view.RPC("UpgradePlayerExtraJumpRPC", RpcTarget.AllBuffered, [steamId, 255]);
            //            view.RPC("UpgradePlayerTumbleLaunchRPC", RpcTarget.AllBuffered, [steamId, 255]);
            //            view.RPC("UpgradePlayerSprintSpeedRPC", RpcTarget.AllBuffered, [steamId, 255]);
            //            view.RPC("UpgradePlayerGrabStrengthRPC", RpcTarget.AllBuffered, [steamId, 255]);
            //            view.RPC("UpgradePlayerGrabRangeRPC", RpcTarget.AllBuffered, [steamId, 255]);
            //            view.RPC("UpgradeMapPlayerCountRPC", RpcTarget.AllBuffered, [steamId, 255]);
            //            view.RPC("UpgradePlayerThrowStrengthRPC", RpcTarget.AllBuffered, [steamId, 255]);
            //            view.RPC("UpgradePlayerEnergyRPC", RpcTarget.AllBuffered, [steamId, 255]);
            //            view.RPC("UpgradePlayerHealthRPC", RpcTarget.AllBuffered, [steamId, 255]);
            //        }
            //    }
            //},
            () => {
                DrawSettingLabel("Privacy");

                // these stay
                MenuSettings.AccountSpoofer.Value = DrawBoolean("AccountSpoofer", MenuSettings.AccountSpoofer.Value);
                MenuSettings.PingSpoofer.Value = DrawBoolean("PingSpoofer", MenuSettings.PingSpoofer.Value);
            },
            () => {
                DrawSettingLabel("Server");

                // im using this for me and my friends (the host sometimes falls asleep midgame)
                if (DrawButton("Steal map"))
                {
                    Patches.OverrideMaster = true;
                    StatsManager.instance.SaveFileCreate();
                    Patches.OverrideMaster = false;
                }
            },
            () => {
                DrawSettingLabel("Waypoints");

                var waypoints = Storage.WAYPOINTS_POINTS.Select(wp => wp.Label).ToArray();
                var newEnum = DrawEnum("Waypoints", waypoints, Storage.WAYPOINTS_POINT);
                if (newEnum != Storage.WAYPOINTS_POINT)
                    Storage.WAYPOINTS_POINT = newEnum;

                if (DrawButton("Delete"))
                    Storage.WAYPOINTS_POINTS.RemoveAt(Storage.WAYPOINTS_POINT);

                if (DrawExpandBox("Configure"))
                {
                    var _newEnum = DrawEnum("Add Colour", ["Red", "Green", "Blue"], Storage.WAYPOINTS_COLOR);
                    if (_newEnum  != Storage.WAYPOINTS_COLOR)
                        Storage.WAYPOINTS_COLOR = _newEnum;

                    Storage.WAYPOINTS_NAME = DrawTextField("Waypoint Name", Storage.WAYPOINTS_NAME);

                    if (DrawButton("Add"))
                    {
                        Color colour = Storage.WAYPOINTS_COLOR switch
                        {
                            0 => new Color(0.9f, 0.15f, 0.15f),
                            1 => new Color(0.15f, 0.9f, 0.15f),
                            2 => new Color(0.15f, 0.15f, 0.9f),
                            _ => Color.yellow
                        };

                        var newPoint = new Storage.WaypointInfo
                        {
                            Label = Storage.WAYPOINTS_NAME,
                            Position = ClientInstance.GetLocalPlayer().gameObject.transform.position + new Vector3(0,1.5f,0),
                            Color = colour
                        };
                        Storage.WAYPOINTS_POINTS.Add(newPoint);
                        Storage.WAYPOINTS_POINT = Storage.WAYPOINTS_POINTS.Count - 1;
                    }
                }
            },
            () => {
                DrawSettingLabel("Settings");

                //StyleThemes.Keys.ToArray()
                var newTheme = DrawEnum("Theme", Storage.StyleThemes.Keys.ToArray(), Storage.SETTINGS_Theme);
                if (newTheme != Storage.SETTINGS_Theme)
                {
                    Storage.SETTINGS_Theme = newTheme;
                    StyleTheme = Storage.StyleThemes[Storage.StyleThemes.Keys.ToArray()[newTheme]];
                    Storage.InternalThemeStyle = StyleTheme;
                    MenuSettings.GameTheme.Value = Storage.StyleThemes.Keys.ToArray()[newTheme];
                }
            },
            () => {
                DrawSettingLabel("Debug");
                DrawSettingLabel("NOTE: this tab will NOT be exposed in release mode");

                var newEnum = DrawEnum("Enum", ["Option1", "Sex2", "Piggy3"], Storage.DEBUG_ENUM);
                if (newEnum != Storage.DEBUG_ENUM)
                    Storage.DEBUG_ENUM = newEnum;

                MenuSettings.ESP_Player.Value = DrawBoolean("Player ESP", MenuSettings.ESP_Player.Value);
                MenuSettings.ESP_Enemy.Value = DrawBoolean("Enemy ESP", MenuSettings.ESP_Enemy.Value);
                MenuSettings.ESP_Valuable.Value = DrawBoolean("Valuable ESP", MenuSettings.ESP_Valuable.Value);
                MenuSettings.ESP_Extractions.Value = DrawBoolean("Extraction ESP", MenuSettings.ESP_Extractions.Value);

                if (DrawButton("Button"))
                    Entry.Log($"Debug button clicked!");

                if (DrawExpandBox("ExpandBox"))
                {
                    DrawSettingLabel("ExpandBox Content here");
                    DrawSettingLabel("ExpandBox Content second");
                }

                Storage.DEBUG_NUMBERFIELD = DrawNumberField("NumberField", Storage.DEBUG_NUMBERFIELD);
                Storage.DEBUG_TEXTFIELD = DrawTextField("TextField", Storage.DEBUG_TEXTFIELD);

                Storage.DEBUG_PLAYERSELECT = DrawPlayerSelect("PlayerSelect", Storage.DEBUG_PLAYERSELECT);

                Storage.DEBUG_SLIDER = DrawSlider("Slider", Storage.DEBUG_SLIDER);
            },
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

    float scrollOffset;

    public override void Render()
    {
        HandleBorder();
        GUI.BeginClip(new Rect(MenuStorage.menuPos.x, MenuStorage.menuPos.y, MenuStorage.menuSize.x, MenuStorage.menuSize.y));
        RenderUtils.DrawRect(new Vector2(0, 0), MenuStorage.menuSize, StyleTheme.ContentBoxBackground);

        RenderUtils.DrawRect(new Vector2(0, 0), new Vector2(MenuStorage.menuSize.x, titleBarHeight), StyleTheme.Titlebar);
        RenderUtils.DrawString(new Vector2(12, 6), $"ClydeMenu {ClydeVersion.ToVersionString(ClydeVersion.Current)}", StyleTheme.TitlebarText);
        RenderUtils.DrawRect(new Vector2(MenuStorage.menuSize.x - 17, 6), new Vector2(12, 12), StyleTheme.TitlebarCloseButton);

        if (LabelPressed("TitlebarCloseButton", new Rect(MenuStorage.menuSize.x - 17, 6, 12, 12)))
            MenuSceneComponent.Instance.PopMenu(this);

        var sidebarPos = new Vector2(0, titleBarHeight);
        var sidebarHeight = MenuStorage.menuSize.y - titleBarHeight;
        RenderUtils.DrawRect(sidebarPos, new Vector2(sidebarWidth, sidebarHeight), StyleTheme.Sidebar);

        for (int i = 0; i < MenuStorage.renderNames.Length; i++)
        {
            var y = sidebarPos.y + padding + i * 27;
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

        var cur = Event.current;
        var scrollAreaHeight = MenuStorage.menuSize.y - titleBarHeight - 2 * padding;
        var scrollRect = new Rect(sidebarWidth + padding, titleBarHeight + padding, MenuStorage.menuSize.x - sidebarWidth - 2 * padding, scrollAreaHeight);

        if (cur.type == EventType.ScrollWheel && scrollRect.Contains(cur.mousePosition))
        {
            scrollOffset += cur.delta.y * 10f;
            scrollOffset = Mathf.Max(scrollOffset, 0f);
            cur.Use();
        }

        GUI.BeginGroup(scrollRect);
        contentStart = new Vector2(0, -scrollOffset);
        yCursor = contentStart.y;

        DrawCategory();

        GUI.EndGroup();
        GUI.EndClip();
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

    Dictionary<string, bool> enumDropdownOpen = new();
    Dictionary<string, string> enumSearchText = new();

    int DrawEnum(string label, string[] enums, int selection = 0)
    {
        if (!enumDropdownOpen.ContainsKey(label))
            enumDropdownOpen[label] = false;
        if (!enumSearchText.ContainsKey(label))
            enumSearchText[label] = "";

        var open = enumDropdownOpen[label];
        var search = enumSearchText[label];
        var labelHeight = 20f;
        var boxWidth = 200f;

        DrawSettingLabel(label);

        Rect labelRect = new Rect(contentStart.x, yCursor, boxWidth, labelHeight);
        if (enums.Length > 0)
        {
            RenderUtils.DrawRect(labelRect.position, labelRect.size, StyleTheme.ContentBox);
            RenderUtils.DrawString(labelRect.position + new Vector2(4, 2), enums[selection], StyleTheme.MenuText);
        }

        var cur = Event.current;
        Vector2 mousePos = cur.mousePosition;

        if (cur.type == EventType.MouseDown && labelRect.Contains(mousePos))
        {
            enumDropdownOpen[label] = !open;
            cur.Use();
        }

        yCursor += labelHeight + padding;

        if (open)
        {
            Rect searchRect = new Rect(contentStart.x, yCursor, boxWidth, labelHeight);
            RenderUtils.DrawRect(searchRect.position, searchRect.size, StyleTheme.ContentBoxBackground);

            if (cur.type == EventType.KeyDown && !labelRect.Contains(mousePos))
            {
                if (cur.keyCode == KeyCode.Backspace && search.Length > 0)
                    search = search.Substring(0, search.Length - 1);
                else if (!char.IsControl(cur.character) && cur.character != '\0')
                    search += cur.character;
                enumSearchText[label] = search;
                cur.Use();
            }

            RenderUtils.DrawString(searchRect.position + new Vector2(4, 2), search.Length > 0 ? search : "Search...", StyleTheme.MenuTextDark);
            yCursor += labelHeight + padding;

            var filteredEnums = enums
                .Where(e => e.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0)
                .ToArray();

            var dropdownHeight = filteredEnums.Length * labelHeight;
            Rect dropdownRect = new Rect(contentStart.x, yCursor, boxWidth, dropdownHeight);
            RenderUtils.DrawRect(dropdownRect.position, dropdownRect.size, StyleTheme.ContentBox);

            for (int i = 0; i < filteredEnums.Length; i++)
            {
                Rect itemRect = new Rect(contentStart.x, yCursor + i * labelHeight, boxWidth, labelHeight);
                RenderUtils.DrawString(itemRect.position + new Vector2(4, 2), filteredEnums[i], StyleTheme.MenuText);

                if (cur.type == EventType.MouseDown && itemRect.Contains(mousePos))
                {
                    selection = Array.IndexOf(enums, filteredEnums[i]);
                    enumDropdownOpen[label] = false;
                    enumSearchText[label] = "";
                    cur.Use();
                }
            }

            yCursor += dropdownHeight + padding;
        }

        return selection;
    }

    // gonna switch up the theme or smth
    private Dictionary<string, bool> expandBoxes = [];
    bool DrawExpandBox(string label, bool _default = false)
    {
        if (!expandBoxes.ContainsKey(label))
            expandBoxes.Add(label, _default);

        var strSub = expandBoxes[label] == true ? "[x]" : "[ ]";
        var str = $"{strSub} {label}";
        var strMsur = RenderUtils.StringSize(str);
        RenderUtils.DrawString(new Vector2(contentStart.x, yCursor), str, StyleTheme.MenuText);
        RenderUtils.DrawRect(new Vector2(contentStart.x + strMsur.x + padding, yCursor + 7), new Vector2(MenuStorage.menuSize.x - (contentStart.x + strMsur.x + (padding * 2)), 3), StyleTheme.Titlebar);

        if (LabelPressed(label, new Rect(contentStart.x, yCursor, MenuStorage.menuSize.x, 16)))// hhh
            expandBoxes[label] = !expandBoxes[label];

        yCursor += 20 + padding;
        return expandBoxes[label];
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

    private static GUIStyle fontStyle = null;
    string DrawTextField(string label, string text)
    {
        if (fontStyle == null)
        {
            fontStyle = new GUIStyle();
            fontStyle.font = Font.CreateDynamicFontFromOSFont("Consolas", 12);
            fontStyle.normal.textColor = Color.white;
        }

        DrawSettingLabel(label);
        Rect textFieldBounds = new Rect(contentStart.x, yCursor, MenuStorage.menuSize.x - contentStart.x - padding, 20);
        GUI.Box(textFieldBounds, GUIContent.none);
        text = GUI.TextField(textFieldBounds, text, 24, fontStyle);

        yCursor += 20 + padding;

        return text;
    }

    private static GUIStyle fontStyleGreen = null;
    private static GUIStyle fontStyleRed = null;
    string DrawNumberField(string label, string text)
    {
        if (fontStyleGreen == null)
        {
            fontStyleGreen = new GUIStyle();
            fontStyleGreen.font = Font.CreateDynamicFontFromOSFont("Consolas", 20);
            fontStyleGreen.normal.textColor = new Color(0, 0.5f, 0);
            //fontStyleGreen.normal.background = TextureUtils.CreateSolid(new Color(0, 0.2f, 0));

            fontStyleRed = new GUIStyle();
            fontStyleRed.font = Font.CreateDynamicFontFromOSFont("Consolas", 20);
            fontStyleRed.normal.textColor = new Color(0.5f, 0, 0);
           // fontStyleRed.normal.background = TextureUtils.CreateSolid(new Color(0.2f, 0, 0));
        }

        DrawSettingLabel(label);
        Rect textFieldBounds = new Rect(contentStart.x, yCursor, MenuStorage.menuSize.x - contentStart.x - padding, 20);
        GUI.Box(textFieldBounds, GUIContent.none);

        if (text.Length >= 1 && float.TryParse(text, out _))
        {
            GUI.Box(textFieldBounds, GUIContent.none, fontStyleGreen);
            text = GUI.TextField(textFieldBounds, text, 24, fontStyleGreen);
        }
        else
        {
            GUI.Box(textFieldBounds, GUIContent.none, fontStyleRed);
            text = GUI.TextField(textFieldBounds, text, 24, fontStyleRed);
        }

        yCursor += 20 + padding;

        return text;
    }

    // TODO: scrollbar, enum selection, textfield/numberfield
}
