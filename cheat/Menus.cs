using UnityEngine;

namespace cheat
{
    public static class Menus
    {
        private const int MenuX = 20;
        private const int MenuW = 250;
        private const int ContentX = 30;
        private const int ContentW = 210;
        private const int SliderW = 185;

        private const int RowH = 26;
        private const int SliderH = 18;
        private const int LabelH = 18;
        private const int SectionGap = 8;
        private const int HeaderH = 20;
        private const int BtnH = 28;
        private const int BtnGap = 6;
        private const int TabH = 28;
        private const int TabY = 48;
        private const int ContentStartY = 82;

        private static int _tab = 0;
        private static readonly string[] _tabNames = { "Combat", "ESP", "Misc", "Visual" };

        public static void DrawMain(CheatBehaviour c)
        {
            Theme.Init();

            int height = CalcHeight(c);
            GUI.Box(new Rect(MenuX, 20, MenuW, height), "REPO Hax by @pilot2254", Theme.Box);

            // tab row
            int tabW = (MenuW - 20) / _tabNames.Length;
            for (int i = 0; i < _tabNames.Length; i++)
            {
                var style = i == _tab ? Theme.TabActive : Theme.TabInactive;
                if (GUI.Button(new Rect(ContentX + i * tabW, TabY, tabW, TabH), _tabNames[i], style))
                    _tab = i;
            }

            int y = ContentStartY;

            switch (_tab)
            {
                case 0: DrawCombat(c, ref y); break;
                case 1: DrawESP(c, ref y); break;
                case 2: DrawMisc(c, ref y); break;
                case 3: DrawVisual(c, ref y); break;
            }

            // bottom action buttons — always visible
            y += SectionGap + 4;
            if (GUI.Button(new Rect(ContentX, y, 100, BtnH), "Upgrades", Theme.Button)) c.ShowUpgrades = true;
            if (GUI.Button(new Rect(ContentX + 110, y, 90, BtnH), "TP Extract", Theme.Button)) Helpers.TeleportToExtraction();
            y += BtnH + BtnGap;
            if (GUI.Button(new Rect(ContentX, y, 100, BtnH), "Troll Chat", Theme.Button)) c.ShowTrolls = true;
            if (GUI.Button(new Rect(ContentX + 110, y, 90, BtnH), "Yeet Loot", Theme.Button)) Helpers.YeetValuables(c);
            y += BtnH + BtnGap;
            if (GUI.Button(new Rect(ContentX, y, 100, BtnH), "Respawn All", Theme.Button)) Helpers.RespawnDeadPlayers();
            if (GUI.Button(new Rect(ContentX + 110, y, 90, BtnH), "Fetch Loot", Theme.Button)) Helpers.FetchBestLoot(c);
            y += BtnH + BtnGap;
            if (GUI.Button(new Rect(ContentX, y, 100, BtnH), "Auto Extract", Theme.Button)) Helpers.AutoExtract(c);
            if (GUI.Button(new Rect(ContentX + 110, y, 90, BtnH), "Config", Theme.Button)) c.ShowConfig = true;
        }

        // tab content

        private static void DrawCombat(CheatBehaviour c, ref int y)
        {
            c.GodMode = Toggle(c.GodMode, "God Mode", ref y);
            c.NoRagdoll = Toggle(c.NoRagdoll, "No Ragdoll", ref y);
            c.NoBreak = Toggle(c.NoBreak, "No Break (host)", ref y);
            c.InfiniteStamina = Toggle(c.InfiniteStamina, "Infinite Stamina", ref y);

            y += SectionGap;
            c.SpeedHack = Toggle(c.SpeedHack, "Speed Multiplier", ref y);
            if (c.SpeedHack)
            {
                c.SpeedMultiplier = Slider(c.SpeedMultiplier, 1f, 5f, ref y);
                Label($"x{c.SpeedMultiplier:F1}", ref y);
            }

            y += SectionGap;
            c.RainbowColor = Toggle(c.RainbowColor, "Rainbow Color", ref y);
            if (c.RainbowColor)
            {
                c.RainbowSpeed = Slider(c.RainbowSpeed, 0.1f, 2f, ref y);
                Label($"Speed: {c.RainbowSpeed:F1}s", ref y);
            }
        }

        private static void DrawESP(CheatBehaviour c, ref int y)
        {
            c.EspPlayers = Toggle(c.EspPlayers, "Player ESP", ref y);
            c.EspEnemies = Toggle(c.EspEnemies, "Enemy ESP", ref y);
            c.EspLoot = Toggle(c.EspLoot, "Loot ESP", ref y);
            c.EspExtraction = Toggle(c.EspExtraction, "Extraction ESP", ref y);
            c.EspBoxes = Toggle(c.EspBoxes, "Box ESP", ref y);
            c.EspSnaplines = Toggle(c.EspSnaplines, "Snap Lines", ref y);

            y += SectionGap;
            Header("── Sub-options ──", ref y);
            c.EspPlayerDist = Toggle(c.EspPlayerDist, "Player Distance", ref y);
            c.EspPlayerHp = Toggle(c.EspPlayerHp, "Player HP", ref y);
            c.EspEnemyDist = Toggle(c.EspEnemyDist, "Enemy Distance", ref y);
            c.EspEnemyHp = Toggle(c.EspEnemyHp, "Enemy HP", ref y);
            c.EspLootPrice = Toggle(c.EspLootPrice, "Loot Price", ref y);

            y += SectionGap;
            Header("── Distance Filter ──", ref y);
            Label($"Max: {c.MaxDistance:F0}m", ref y);
            c.MaxDistance = Slider(c.MaxDistance, 5f, 200f, ref y);
            c.DistanceFilterEnemies = Toggle(c.DistanceFilterEnemies, "Filter Enemies", ref y);
            c.DistanceFilterLoot = Toggle(c.DistanceFilterLoot, "Filter Loot", ref y);
            c.DistanceFilterPlayers = Toggle(c.DistanceFilterPlayers, "Filter Players", ref y);

            y += SectionGap;
            c.HighlightBestLoot = Toggle(c.HighlightBestLoot, "Highlight Best Loot", ref y);
            c.EnemyNearbyWarning = Toggle(c.EnemyNearbyWarning, "Enemy Nearby Warning", ref y);
            c.FilterLootByValue = Toggle(c.FilterLootByValue, "Min Loot Value Filter", ref y);
            if (c.FilterLootByValue)
            {
                c.MinLootValue = Slider(c.MinLootValue, 0f, 5000f, ref y);
                Label($"Min: ${c.MinLootValue:F0}", ref y);
            }
        }

        private static void DrawMisc(CheatBehaviour c, ref int y)
        {
            c.Noclip = Toggle(c.Noclip, "Noclip / Fly", ref y);
            if (c.Noclip)
            {
                c.NoclipSpeed = Slider(c.NoclipSpeed, 1f, 50f, ref y);
                Label($"Speed: {c.NoclipSpeed:F0}", ref y);
            }
        }

        private static void DrawVisual(CheatBehaviour c, ref int y)
        {
            c.BrightMode = Toggle(c.BrightMode, "Bright Mode", ref y);
            c.NoChromaticAberration = Toggle(c.NoChromaticAberration, "No Chromatic Aberration", ref y);
            c.NoBloom = Toggle(c.NoBloom, "No Bloom", ref y);
            c.NoLensDistortion = Toggle(c.NoLensDistortion, "No Lens Distortion", ref y);

            y += SectionGap;
            Header("── Flashlight ──", ref y);
            c.FlashlightCustomColor = Toggle(c.FlashlightCustomColor, "Custom Color", ref y);
            if (c.FlashlightCustomColor)
            {
                GUI.Label(new Rect(ContentX, y, 20, LabelH), "R", Theme.Label);
                c.FlashlightColor.r = GUI.HorizontalSlider(new Rect(ContentX + 20, y, SliderW - 20, SliderH), c.FlashlightColor.r, 0f, 1f); y += SliderH + 2;
                GUI.Label(new Rect(ContentX, y, 20, LabelH), "G", Theme.Label);
                c.FlashlightColor.g = GUI.HorizontalSlider(new Rect(ContentX + 20, y, SliderW - 20, SliderH), c.FlashlightColor.g, 0f, 1f); y += SliderH + 2;
                GUI.Label(new Rect(ContentX, y, 20, LabelH), "B", Theme.Label);
                c.FlashlightColor.b = GUI.HorizontalSlider(new Rect(ContentX + 20, y, SliderW - 20, SliderH), c.FlashlightColor.b, 0f, 1f); y += SliderH + 4;
            }
            c.FlashlightIntensity = Slider(c.FlashlightIntensity, 1f, 7f, ref y);
            Label($"Intensity: {c.FlashlightIntensity:F1}", ref y);
        }

        // config submenu

        // keybind capture state
        private static bool _waitingForKey = false;

        public static void DrawConfig(CheatBehaviour c)
        {
            Theme.Init();

            GUI.Box(new Rect(MenuX, 20, MenuW, 160), "Config", Theme.Box);

            int y = 50;

            // keybind row
            GUI.Label(new Rect(ContentX, y, 120, LabelH), "Toggle Menu Key:", Theme.Label);
            string btnLabel = _waitingForKey ? "Press any key..." : c.ToggleMenuKey.ToString();
            if (GUI.Button(new Rect(ContentX + 125, y, 95, BtnH), btnLabel, Theme.Button))
                _waitingForKey = true;

            if (_waitingForKey && Event.current.type == EventType.KeyDown && Event.current.keyCode != KeyCode.None)
            {
                c.ToggleMenuKey = Event.current.keyCode;
                _waitingForKey = false;
            }

            y += BtnH + SectionGap + 4;

            if (GUI.Button(new Rect(ContentX, y, 95, BtnH), "Save", Theme.Button)) CheatConfig.From(c).Save();
            if (GUI.Button(new Rect(ContentX + 105, y, 95, BtnH), "Load", Theme.Button)) CheatConfig.Load().ApplyTo(c);
            y += BtnH + BtnGap;
            if (GUI.Button(new Rect(ContentX, y, 200, BtnH), "Back", Theme.Button)) c.ShowConfig = false;
        }

        // sub-menus

        public static void DrawUpgrades(CheatBehaviour c)
        {
            Theme.Init();

            GUI.Box(new Rect(MenuX, 20, 220, 320), "Upgrades", Theme.Box);

            GUI.Label(new Rect(30, 50, 80, 20), "Set value:", Theme.Label);
            if (GUI.Button(new Rect(110, 48, 30, 22), "0", Theme.Button)) c.UpgradeValue = 0;
            if (GUI.Button(new Rect(145, 48, 30, 22), "10", Theme.Button)) c.UpgradeValue = 10;
            if (GUI.Button(new Rect(180, 48, 30, 22), "50", Theme.Button)) c.UpgradeValue = 50;

            GUI.Label(new Rect(30, 75, 180, 20), $"Current: {c.UpgradeValue}", Theme.Label);

            int btnY = 100;
            if (GUI.Button(new Rect(30, btnY, 180, BtnH), "Health", Theme.Button)) Helpers.SetUpgrade("playerUpgradeHealth", c);
            if (GUI.Button(new Rect(30, btnY + 33, 180, BtnH), "Stamina", Theme.Button)) Helpers.SetUpgrade("playerUpgradeStamina", c);
            if (GUI.Button(new Rect(30, btnY + 66, 180, BtnH), "Speed", Theme.Button)) Helpers.SetUpgrade("playerUpgradeSpeed", c);
            if (GUI.Button(new Rect(30, btnY + 99, 180, BtnH), "Strength", Theme.Button)) Helpers.SetUpgrade("playerUpgradeStrength", c);
            if (GUI.Button(new Rect(30, btnY + 132, 180, BtnH), "Jump", Theme.Button)) Helpers.SetUpgrade("playerUpgradeExtraJump", c);
            if (GUI.Button(new Rect(30, btnY + 165, 180, BtnH), "Range", Theme.Button)) Helpers.SetUpgrade("playerUpgradeRange", c);

            if (GUI.Button(new Rect(30, 290, 180, 25), "Back", Theme.Button)) c.ShowUpgrades = false;
        }

        public static void DrawTrolls(CheatBehaviour c)
        {
            Theme.Init();

            int btnY = 72;
            int rows = 6;
            int backY = btnY + rows * 33 + 6;

            GUI.Box(new Rect(MenuX, 20, 220, backY + 25 + 12), "Troll Chat", Theme.Box);
            GUI.Label(new Rect(30, 48, 180, 20), "multiplayer only", Theme.Label);

            if (GUI.Button(new Rect(30, btnY, 180, BtnH), "Flashbang", Theme.Button)) Helpers.SendChat("<size=-111111>hi");
            if (GUI.Button(new Rect(30, btnY + 33, 180, BtnH), "Big Text", Theme.Button)) Helpers.SendChat("<size=999>HELLO");
            if (GUI.Button(new Rect(30, btnY + 66, 180, BtnH), "Invisible", Theme.Button)) Helpers.SendChat("<alpha=#00>ghost message");
            if (GUI.Button(new Rect(30, btnY + 99, 180, BtnH), "Spam Hello", Theme.Button)) c.StartCoroutine(Helpers.SpamChat("hello", 3));
            if (GUI.Button(new Rect(30, btnY + 132, 180, BtnH), "Max Chaos", Theme.Button)) Helpers.SendChat("<size=-111111><size=999>CHAOS");
            if (GUI.Button(new Rect(30, btnY + 165, 180, BtnH), "Custom...", Theme.Button)) Helpers.SendChat("<size=-111111>custom troll");

            if (GUI.Button(new Rect(30, backY, 180, 25), "Back", Theme.Button)) c.ShowTrolls = false;
        }

        // height calc

        private static int CalcHeight(CheatBehaviour c)
        {
            int h = ContentStartY;

            switch (_tab)
            {
                case 0: // Combat
                    h += RowH * 4 + SectionGap + RowH;
                    if (c.SpeedHack) h += SliderH + 2 + LabelH + 4;
                    h += SectionGap + RowH;
                    if (c.RainbowColor) h += SliderH + 2 + LabelH + 4;
                    break;

                case 1: // ESP
                    h += RowH * 6 + SectionGap;
                    h += HeaderH + 2 + RowH * 5 + SectionGap;
                    h += HeaderH + 2 + LabelH + 4 + SliderH + 6 + RowH * 3 + SectionGap;
                    h += RowH * 3;
                    if (c.FilterLootByValue) h += SliderH + 2 + LabelH + 4;
                    break;

                case 2: // Misc
                    h += RowH;
                    if (c.Noclip) h += SliderH + 2 + LabelH + 4;
                    break;

                case 3: // Visual
                    h += RowH * 4 + SectionGap + HeaderH + 2 + RowH;
                    if (c.FlashlightCustomColor) h += (SliderH + 2) * 3 + 2;
                    h += SliderH + 2 + LabelH + 4;
                    break;
            }

            // bottom buttons
            h += SectionGap + 4 + (BtnH + BtnGap) * 4 + 12;
            return h;
        }

        // IMGUI helpers

        private static bool Toggle(bool val, string label, ref int y)
        {
            bool result = GUI.Toggle(new Rect(ContentX, y, ContentW, RowH), val, label, Theme.Toggle);
            y += RowH;
            return result;
        }

        private static float Slider(float val, float min, float max, ref int y)
        {
            float result = GUI.HorizontalSlider(new Rect(ContentX, y, SliderW, SliderH), val, min, max);
            y += SliderH + 2;
            return result;
        }

        private static void Label(string text, ref int y)
        {
            GUI.Label(new Rect(ContentX, y, ContentW, LabelH), text, Theme.Label);
            y += LabelH + 4;
        }

        private static void Header(string text, ref int y)
        {
            GUI.Label(new Rect(ContentX, y, ContentW, HeaderH), text, Theme.HeaderLabel);
            y += HeaderH + 2;
        }
    }
}