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

        private const int RowH = 26;
        private const int SliderH = 18;
        private const int LabelH = 18;
        private const int SectionGap = 10;
        private const int HeaderH = 20;
        private const int BtnH = 28;
        private const int BtnGap = 6;

        public static void DrawMain(CheatBehaviour c)
        {
            Theme.Init();

            int height = CalcMainHeight(c);
            GUI.Box(new Rect(MenuX, 20, MenuW, height), "REPO Hax by @pilot2254", Theme.Box);

            int y = 48;

            c.GodMode = GUI.Toggle(new Rect(ContentX, y, ContentW, RowH), c.GodMode, "God Mode", Theme.Toggle); y += RowH;
            c.SpeedHack = GUI.Toggle(new Rect(ContentX, y, ContentW, RowH), c.SpeedHack, "Speed Multiplier", Theme.Toggle); y += RowH;
            c.NoRagdoll = GUI.Toggle(new Rect(ContentX, y, ContentW, RowH), c.NoRagdoll, "No Ragdoll", Theme.Toggle); y += RowH;
            c.NoBreak = GUI.Toggle(new Rect(ContentX, y, ContentW, RowH), c.NoBreak, "No Break (host only)", Theme.Toggle); y += RowH;
            c.InfiniteStamina = GUI.Toggle(new Rect(ContentX, y, ContentW, RowH), c.InfiniteStamina, "Infinite Stamina", Theme.Toggle); y += RowH;
            c.RainbowColor = GUI.Toggle(new Rect(ContentX, y, ContentW, RowH), c.RainbowColor, "Rainbow Color", Theme.Toggle); y += RowH;

            if (c.RainbowColor)
            {
                c.RainbowSpeed = GUI.HorizontalSlider(new Rect(ContentX, y, SliderW, SliderH), c.RainbowSpeed, 0.1f, 2f); y += SliderH + 2;
                GUI.Label(new Rect(ContentX, y, ContentW, LabelH), $"Speed: {c.RainbowSpeed:F1}s", Theme.Label); y += LabelH + 4;
            }

            if (c.SpeedHack)
            {
                c.SpeedMultiplier = GUI.HorizontalSlider(new Rect(ContentX, y, SliderW, SliderH), c.SpeedMultiplier, 1f, 5f); y += SliderH + 2;
                GUI.Label(new Rect(ContentX, y, ContentW, LabelH), $"x{c.SpeedMultiplier:F1} speed", Theme.Label); y += LabelH + 4;
            }

            y += SectionGap;
            GUI.Label(new Rect(ContentX, y, ContentW, HeaderH), "── ESP ──", Theme.HeaderLabel); y += HeaderH + 2;
            c.EspPlayers = GUI.Toggle(new Rect(ContentX, y, ContentW, RowH), c.EspPlayers, "Player ESP", Theme.Toggle); y += RowH;
            c.EspEnemies = GUI.Toggle(new Rect(ContentX, y, ContentW, RowH), c.EspEnemies, "Enemy ESP", Theme.Toggle); y += RowH;
            c.EspLoot = GUI.Toggle(new Rect(ContentX, y, ContentW, RowH), c.EspLoot, "Loot ESP", Theme.Toggle); y += RowH;
            c.EspExtraction = GUI.Toggle(new Rect(ContentX, y, ContentW, RowH), c.EspExtraction, "Extraction ESP", Theme.Toggle); y += RowH;

            c.EspBoxes = GUI.Toggle(new Rect(ContentX, y, ContentW, RowH), c.EspBoxes, "Box ESP", Theme.Toggle); y += RowH;
            c.EspSnaplines = GUI.Toggle(new Rect(ContentX, y, ContentW, RowH), c.EspSnaplines, "Snap Lines", Theme.Toggle); y += RowH;

            y += SectionGap;
            GUI.Label(new Rect(ContentX, y, ContentW, HeaderH), "── Distance Filter ──", Theme.HeaderLabel); y += HeaderH + 2;
            GUI.Label(new Rect(ContentX, y, ContentW, LabelH), $"Max: {c.MaxDistance:F0}m", Theme.Label); y += LabelH + 2;
            c.MaxDistance = GUI.HorizontalSlider(new Rect(ContentX, y, SliderW, SliderH), c.MaxDistance, 5f, 200f); y += SliderH + 6;
            c.DistanceFilterEnemies = GUI.Toggle(new Rect(ContentX, y, ContentW, RowH), c.DistanceFilterEnemies, "Filter Enemies", Theme.Toggle); y += RowH;
            c.DistanceFilterLoot = GUI.Toggle(new Rect(ContentX, y, ContentW, RowH), c.DistanceFilterLoot, "Filter Loot", Theme.Toggle); y += RowH;
            c.DistanceFilterPlayers = GUI.Toggle(new Rect(ContentX, y, ContentW, RowH), c.DistanceFilterPlayers, "Filter Players", Theme.Toggle); y += RowH;

            y += SectionGap;
            GUI.Label(new Rect(ContentX, y, ContentW, HeaderH), "── Misc ──", Theme.HeaderLabel); y += HeaderH + 2;
            c.HighlightBestLoot = GUI.Toggle(new Rect(ContentX, y, ContentW, RowH), c.HighlightBestLoot, "Highlight Best Loot", Theme.Toggle); y += RowH;
            c.EnemyNearbyWarning = GUI.Toggle(new Rect(ContentX, y, ContentW, RowH), c.EnemyNearbyWarning, "Enemy Nearby Warning", Theme.Toggle); y += RowH;
            c.FilterLootByValue = GUI.Toggle(new Rect(ContentX, y, ContentW, RowH), c.FilterLootByValue, "Min Loot Value Filter", Theme.Toggle); y += RowH;
            c.BrightMode = GUI.Toggle(new Rect(ContentX, y, ContentW, RowH), c.BrightMode, "Bright Mode", Theme.Toggle); y += RowH;
            bool nca = GUI.Toggle(new Rect(ContentX, y, ContentW, RowH), c.NoChromaticAberration, "No Chromatic Aberration", Theme.Toggle); y += RowH;
            bool nb = GUI.Toggle(new Rect(ContentX, y, ContentW, RowH), c.NoBloom, "No Bloom", Theme.Toggle); y += RowH;
            bool nld = GUI.Toggle(new Rect(ContentX, y, ContentW, RowH), c.NoLensDistortion, "No Lens Distortion", Theme.Toggle); y += RowH;

            if (nca != c.NoChromaticAberration || nb != c.NoBloom || nld != c.NoLensDistortion)
            {
                c.NoChromaticAberration = nca;
                c.NoBloom = nb;
                c.NoLensDistortion = nld;
                c._postProcessDirty = true;
            }
            c.Noclip = GUI.Toggle(new Rect(ContentX, y, ContentW, RowH), c.Noclip, "Noclip / Fly", Theme.Toggle); y += RowH;

            if (c.FilterLootByValue)
            {
                c.MinLootValue = GUI.HorizontalSlider(new Rect(ContentX, y, SliderW, SliderH), c.MinLootValue, 0f, 5000f); y += SliderH + 2;
                GUI.Label(new Rect(ContentX, y, ContentW, LabelH), $"Min: ${c.MinLootValue:F0}", Theme.Label); y += LabelH + 4;
            }

            if (c.Noclip)
            {
                c.NoclipSpeed = GUI.HorizontalSlider(new Rect(ContentX, y, SliderW, SliderH), c.NoclipSpeed, 1f, 50f); y += SliderH + 2;
                GUI.Label(new Rect(ContentX, y, ContentW, LabelH), $"Speed: {c.NoclipSpeed:F0}", Theme.Label); y += LabelH + 4;
            }

            //if (c.BrightMode)
            //{
            //    c.BrightIntensity = GUI.HorizontalSlider(new Rect(ContentX, y, SliderW, SliderH), c.BrightIntensity, 1f, 10f); y += SliderH + 2;
            //    GUI.Label(new Rect(ContentX, y, ContentW, LabelH), $"Intensity: {c.BrightIntensity:F1}", Theme.Label); y += LabelH + 4;
            //}

            y += SectionGap;
            GUI.Label(new Rect(ContentX, y, ContentW, HeaderH), "── Flashlight ──", Theme.HeaderLabel); y += HeaderH + 2;
            c.FlashlightCustomColor = GUI.Toggle(new Rect(ContentX, y, ContentW, RowH), c.FlashlightCustomColor, "Custom Color", Theme.Toggle); y += RowH;

            if (c.FlashlightCustomColor)
            {
                // R G B sliders
                GUI.Label(new Rect(ContentX, y, 20, LabelH), "R", Theme.Label);
                c.FlashlightColor.r = GUI.HorizontalSlider(new Rect(ContentX + 20, y, SliderW - 20, SliderH), c.FlashlightColor.r, 0f, 1f); y += SliderH + 2;
                GUI.Label(new Rect(ContentX, y, 20, LabelH), "G", Theme.Label);
                c.FlashlightColor.g = GUI.HorizontalSlider(new Rect(ContentX + 20, y, SliderW - 20, SliderH), c.FlashlightColor.g, 0f, 1f); y += SliderH + 2;
                GUI.Label(new Rect(ContentX, y, 20, LabelH), "B", Theme.Label);
                c.FlashlightColor.b = GUI.HorizontalSlider(new Rect(ContentX + 20, y, SliderW - 20, SliderH), c.FlashlightColor.b, 0f, 1f); y += SliderH + 4;
            }

            c.FlashlightIntensity = GUI.HorizontalSlider(new Rect(ContentX, y, SliderW, SliderH), c.FlashlightIntensity, 1f, 7f); y += SliderH + 2;
            GUI.Label(new Rect(ContentX, y, ContentW, LabelH), $"Intensity: {c.FlashlightIntensity:F1}", Theme.Label); y += LabelH + 4;

            y += SectionGap + 4;
            if (GUI.Button(new Rect(ContentX, y, 100, BtnH), "Upgrades", Theme.Button)) c.ShowUpgrades = true;
            if (GUI.Button(new Rect(ContentX + 110, y, 90, BtnH), "TP Extract", Theme.Button)) Helpers.TeleportToExtraction();
            y += BtnH + BtnGap;
            if (GUI.Button(new Rect(ContentX, y, 100, BtnH), "Troll Chat", Theme.Button)) c.ShowTrolls = true;
            if (GUI.Button(new Rect(ContentX + 110, y, 90, BtnH), "Yeet Loot", Theme.Button)) Helpers.YeetValuables(c);
            y += BtnH + BtnGap;
            if (GUI.Button(new Rect(ContentX, y, 100, BtnH), "Respawn All", Theme.Button)) Helpers.RespawnDeadPlayers();
        }

        private static int CalcMainHeight(CheatBehaviour c)
        {
            int h = 48;
            h += RowH * 6;
            if (c.RainbowColor) h += SliderH + 2 + LabelH + 4;
            if (c.SpeedHack) h += SliderH + 2 + LabelH + 4;
            h += SectionGap + HeaderH + 2 + RowH * 6;
            h += SectionGap + HeaderH + 2 + LabelH + 2 + SliderH + 6 + RowH * 3;
            h += SectionGap + HeaderH + 2 + RowH * 6;
            if (c.FilterLootByValue) h += SliderH + 2 + LabelH + 4;
            h += SectionGap + 4 + BtnH + BtnGap + BtnH + BtnGap + BtnH + 12;
            //if (c.BrightMode) h += SliderH + 2 + LabelH + 4;
            if (c.Noclip) h += SliderH + 2 + LabelH + 4;
            h += SectionGap + HeaderH + 2 + RowH + SliderH + 2 + LabelH + 4; // flashlight base
            if (c.FlashlightCustomColor) h += (SliderH + 2) * 3 + 2; // RGB sliders
            return h;
        }

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
            int boxH = backY + 25 + 12;

            GUI.Box(new Rect(MenuX, 20, 220, boxH), "Troll Chat", Theme.Box);
            GUI.Label(new Rect(30, 48, 180, 20), "multiplayer only", Theme.Label);

            if (GUI.Button(new Rect(30, btnY, 180, BtnH), "Flashbang", Theme.Button)) Helpers.SendChat("<size=-111111>hi");
            if (GUI.Button(new Rect(30, btnY + 33, 180, BtnH), "Big Text", Theme.Button)) Helpers.SendChat("<size=999>HELLO");
            if (GUI.Button(new Rect(30, btnY + 66, 180, BtnH), "Invisible", Theme.Button)) Helpers.SendChat("<alpha=#00>ghost message");
            if (GUI.Button(new Rect(30, btnY + 99, 180, BtnH), "Spam Hello", Theme.Button)) c.StartCoroutine(Helpers.SpamChat("hello", 3));
            if (GUI.Button(new Rect(30, btnY + 132, 180, BtnH), "Max Chaos", Theme.Button)) Helpers.SendChat("<size=-111111><size=999>CHAOS");
            if (GUI.Button(new Rect(30, btnY + 165, 180, BtnH), "Custom...", Theme.Button)) Helpers.SendChat("<size=-111111>custom troll");

            if (GUI.Button(new Rect(30, backY, 180, 25), "Back", Theme.Button)) c.ShowTrolls = false;
        }
    }
}