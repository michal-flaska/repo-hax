using UnityEngine;

namespace cheat
{
    public static class Menus
    {
        private const int MenuX = 20;
        private const int MenuW = 240;
        private const int ContentX = 30;
        private const int ContentW = 200;
        private const int SliderW = 175;

        private const int RowH = 26;       // toggle row height
        private const int SliderH = 18;    // slider height
        private const int LabelH = 18;     // small label height
        private const int SectionGap = 10; // gap before section headers
        private const int HeaderH = 20;    // section header height
        private const int BtnH = 28;
        private const int BtnGap = 6;

        public static void DrawMain(CheatBehaviour c)
        {
            // first pass: calculate total height
            int height = CalcMainHeight(c);
            GUI.Box(new Rect(MenuX, 20, MenuW, height), "REPO Hax by @pilot2254");

            int y = 48;

            // features
            c.GodMode = GUI.Toggle(new Rect(ContentX, y, ContentW, RowH), c.GodMode, "God Mode"); y += RowH;
            c.SpeedHack = GUI.Toggle(new Rect(ContentX, y, ContentW, RowH), c.SpeedHack, "Speed Multiplier"); y += RowH;
            c.NoRagdoll = GUI.Toggle(new Rect(ContentX, y, ContentW, RowH), c.NoRagdoll, "No Ragdoll"); y += RowH;
            c.NoBreak = GUI.Toggle(new Rect(ContentX, y, ContentW, RowH), c.NoBreak, "No Break (host only)"); y += RowH;
            c.InfiniteStamina = GUI.Toggle(new Rect(ContentX, y, ContentW, RowH), c.InfiniteStamina, "Infinite Stamina"); y += RowH;
            c.RainbowColor = GUI.Toggle(new Rect(ContentX, y, ContentW, RowH), c.RainbowColor, "Rainbow Color"); y += RowH;

            if (c.RainbowColor)
            {
                c.RainbowSpeed = GUI.HorizontalSlider(new Rect(ContentX, y, SliderW, SliderH), c.RainbowSpeed, 0.1f, 2f); y += SliderH + 2;
                GUI.Label(new Rect(ContentX, y, ContentW, LabelH), $"Speed: {c.RainbowSpeed:F1}s"); y += LabelH + 4;
            }

            if (c.SpeedHack)
            {
                c.SpeedMultiplier = GUI.HorizontalSlider(new Rect(ContentX, y, SliderW, SliderH), c.SpeedMultiplier, 1f, 5f); y += SliderH + 2;
                GUI.Label(new Rect(ContentX, y, ContentW, LabelH), $"x{c.SpeedMultiplier:F1} speed"); y += LabelH + 4;
            }

            // esp
            y += SectionGap;
            GUI.Label(new Rect(ContentX, y, ContentW, HeaderH), "── ESP ──"); y += HeaderH + 2;
            c.EspPlayers = GUI.Toggle(new Rect(ContentX, y, ContentW, RowH), c.EspPlayers, "Player ESP"); y += RowH;
            c.EspEnemies = GUI.Toggle(new Rect(ContentX, y, ContentW, RowH), c.EspEnemies, "Enemy ESP"); y += RowH;
            c.EspLoot = GUI.Toggle(new Rect(ContentX, y, ContentW, RowH), c.EspLoot, "Loot ESP"); y += RowH;
            c.EspExtraction = GUI.Toggle(new Rect(ContentX, y, ContentW, RowH), c.EspExtraction, "Extraction ESP"); y += RowH;

            // distance filter
            y += SectionGap;
            GUI.Label(new Rect(ContentX, y, ContentW, HeaderH), "── Distance Filter ──"); y += HeaderH + 2;
            GUI.Label(new Rect(ContentX, y, ContentW, LabelH), $"Max: {c.MaxDistance:F0}m"); y += LabelH + 2;
            c.MaxDistance = GUI.HorizontalSlider(new Rect(ContentX, y, SliderW, SliderH), c.MaxDistance, 5f, 200f); y += SliderH + 6;

            c.DistanceFilterEnemies = GUI.Toggle(new Rect(ContentX, y, ContentW, RowH), c.DistanceFilterEnemies, "Filter Enemies"); y += RowH;
            c.DistanceFilterLoot = GUI.Toggle(new Rect(ContentX, y, ContentW, RowH), c.DistanceFilterLoot, "Filter Loot"); y += RowH;
            c.DistanceFilterPlayers = GUI.Toggle(new Rect(ContentX, y, ContentW, RowH), c.DistanceFilterPlayers, "Filter Players"); y += RowH;

            // misc
            y += SectionGap;
            GUI.Label(new Rect(ContentX, y, ContentW, HeaderH), "── Misc ──"); y += HeaderH + 2;
            c.HighlightBestLoot = GUI.Toggle(new Rect(ContentX, y, ContentW, RowH), c.HighlightBestLoot, "Highlight Best Loot"); y += RowH;
            c.EnemyNearbyWarning = GUI.Toggle(new Rect(ContentX, y, ContentW, RowH), c.EnemyNearbyWarning, "Enemy Nearby Warning"); y += RowH;
            c.FilterLootByValue = GUI.Toggle(new Rect(ContentX, y, ContentW, RowH), c.FilterLootByValue, "Min Loot Value Filter"); y += RowH;

            if (c.FilterLootByValue)
            {
                c.MinLootValue = GUI.HorizontalSlider(new Rect(ContentX, y, SliderW, SliderH), c.MinLootValue, 0f, 5000f); y += SliderH + 2;
                GUI.Label(new Rect(ContentX, y, ContentW, LabelH), $"Min: ${c.MinLootValue:F0}"); y += LabelH + 4;
            }

            // buttons
            y += SectionGap + 4;
            if (GUI.Button(new Rect(ContentX, y, 100, BtnH), "Upgrades")) c.ShowUpgrades = true;
            if (GUI.Button(new Rect(ContentX + 110, y, 90, BtnH), "TP Extract")) Helpers.TeleportToExtraction();
            y += BtnH + BtnGap;
            if (GUI.Button(new Rect(ContentX, y, 100, BtnH), "Troll Chat")) c.ShowTrolls = true;
            if (GUI.Button(new Rect(ContentX + 110, y, 90, BtnH), "Yeet Loot")) Helpers.YeetValuables(c);
        }

        // calculates the exact menu height based on current toggle states
        private static int CalcMainHeight(CheatBehaviour c)
        {
            int h = 48; // title bar

            h += RowH * 6; // 6 main toggles

            if (c.RainbowColor) h += SliderH + 2 + LabelH + 4;
            if (c.SpeedHack) h += SliderH + 2 + LabelH + 4;

            h += SectionGap + HeaderH + 2 + RowH * 4; // esp

            h += SectionGap + HeaderH + 2 + LabelH + 2 + SliderH + 6 + RowH * 3; // distance filter

            h += SectionGap + HeaderH + 2 + RowH * 3; // misc
            if (c.FilterLootByValue) h += SliderH + 2 + LabelH + 4;

            h += SectionGap + 4 + BtnH + BtnGap + BtnH + 12; // buttons + bottom padding

            return h;
        }

        public static void DrawUpgrades(CheatBehaviour c)
        {
            GUI.Box(new Rect(MenuX, 20, 220, 320), "Upgrades");

            GUI.Label(new Rect(30, 50, 80, 20), "Set value:");
            if (GUI.Button(new Rect(110, 48, 30, 22), "0")) c.UpgradeValue = 0;
            if (GUI.Button(new Rect(145, 48, 30, 22), "10")) c.UpgradeValue = 10;
            if (GUI.Button(new Rect(180, 48, 30, 22), "50")) c.UpgradeValue = 50;

            GUI.Label(new Rect(30, 75, 180, 20), $"Current: {c.UpgradeValue}");

            int btnY = 100;
            if (GUI.Button(new Rect(30, btnY, 180, BtnH), "Health")) Helpers.SetUpgrade("playerUpgradeHealth", c);
            if (GUI.Button(new Rect(30, btnY + 33, 180, BtnH), "Stamina")) Helpers.SetUpgrade("playerUpgradeStamina", c);
            if (GUI.Button(new Rect(30, btnY + 66, 180, BtnH), "Speed")) Helpers.SetUpgrade("playerUpgradeSpeed", c);
            if (GUI.Button(new Rect(30, btnY + 99, 180, BtnH), "Strength")) Helpers.SetUpgrade("playerUpgradeStrength", c);
            if (GUI.Button(new Rect(30, btnY + 132, 180, BtnH), "Jump")) Helpers.SetUpgrade("playerUpgradeExtraJump", c);
            if (GUI.Button(new Rect(30, btnY + 165, 180, BtnH), "Range")) Helpers.SetUpgrade("playerUpgradeRange", c);

            if (GUI.Button(new Rect(30, 290, 180, 25), "Back")) c.ShowUpgrades = false;
        }

        public static void DrawTrolls(CheatBehaviour c)
        {
            int btnY = 72;
            int rows = 6; // Flashbang, Big Text, Invisible, Spam Hello, Max Chaos, Custom
            int backY = btnY + rows * 33 + 6;
            int boxH = backY + 25 + 12;

            GUI.Box(new Rect(MenuX, 20, 220, boxH), "Troll Chat");
            GUI.Label(new Rect(30, 48, 180, 20), "multiplayer only");

            if (GUI.Button(new Rect(30, btnY, 180, BtnH), "Flashbang")) Helpers.SendChat("<size=-111111>hi");
            if (GUI.Button(new Rect(30, btnY + 33, 180, BtnH), "Big Text")) Helpers.SendChat("<size=999>HELLO");
            if (GUI.Button(new Rect(30, btnY + 66, 180, BtnH), "Invisible")) Helpers.SendChat("<alpha=#00>ghost message");
            if (GUI.Button(new Rect(30, btnY + 99, 180, BtnH), "Spam Hello")) c.StartCoroutine(Helpers.SpamChat("hello", 3));
            if (GUI.Button(new Rect(30, btnY + 132, 180, BtnH), "Max Chaos")) Helpers.SendChat("<size=-111111><size=999>CHAOS");
            if (GUI.Button(new Rect(30, btnY + 165, 180, BtnH), "Custom...")) Helpers.SendChat("<size=-111111>custom troll");

            if (GUI.Button(new Rect(30, backY, 180, 25), "Back")) c.ShowTrolls = false;
        }
    }
}