using UnityEngine;

namespace cheat
{
    public static class Menus
    {
        public static void DrawMain(CheatBehaviour c)
        {
            GUI.Box(new Rect(20, 20, 230, 390), "REPO Cheat");

            int y = 50;
            c.GodMode = GUI.Toggle(new Rect(30, y, 190, 25), c.GodMode, "God Mode");
            c.SpeedHack = GUI.Toggle(new Rect(30, y += 30, 190, 25), c.SpeedHack, "Speed Multiplier");
            c.NoRagdoll = GUI.Toggle(new Rect(30, y += 30, 190, 25), c.NoRagdoll, "No Ragdoll");
            c.NoBreak = GUI.Toggle(new Rect(30, y += 30, 190, 25), c.NoBreak, "No Break");
            c.InfiniteStamina = GUI.Toggle(new Rect(30, y += 30, 190, 25), c.InfiniteStamina, "Infinite Stamina");

            GUI.Label(new Rect(30, y += 32, 190, 18), "── ESP ──");
            c.EspPlayers = GUI.Toggle(new Rect(30, y += 22, 190, 22), c.EspPlayers, "Player ESP");
            c.EspEnemies = GUI.Toggle(new Rect(30, y += 26, 190, 22), c.EspEnemies, "Enemy ESP");
            c.EspLoot = GUI.Toggle(new Rect(30, y += 26, 190, 22), c.EspLoot, "Loot ESP");
            c.EspExtraction = GUI.Toggle(new Rect(30, y += 26, 190, 22), c.EspExtraction, "Extraction ESP");

            if (c.SpeedHack)
            {
                c.SpeedMultiplier = GUI.HorizontalSlider(new Rect(30, y += 32, 160, 20), c.SpeedMultiplier, 1f, 5f);
                GUI.Label(new Rect(30, y += 16, 190, 20), $"x{c.SpeedMultiplier:F1} speed");
            }
            else
            {
                y += 36;
            }

            if (GUI.Button(new Rect(30, y += 10, 90, 28), "Upgrades")) c.ShowUpgrades = true;
            if (GUI.Button(new Rect(130, y, 90, 28), "TP Extract")) Helpers.TeleportToExtraction();
            if (GUI.Button(new Rect(30, y + 33, 190, 25), "Troll Chat")) c.ShowTrolls = true;
        }

        public static void DrawUpgrades(CheatBehaviour c)
        {
            GUI.Box(new Rect(20, 20, 220, 320), "Upgrades");

            GUI.Label(new Rect(30, 50, 80, 20), "Set value:");
            if (GUI.Button(new Rect(110, 48, 30, 22), "0")) c.UpgradeValue = 0;
            if (GUI.Button(new Rect(145, 48, 30, 22), "10")) c.UpgradeValue = 10;
            if (GUI.Button(new Rect(180, 48, 30, 22), "50")) c.UpgradeValue = 50;

            GUI.Label(new Rect(30, 75, 180, 20), $"Current: {c.UpgradeValue}");

            int btnY = 100, btnH = 28, gap = 33;
            if (GUI.Button(new Rect(30, btnY, 180, btnH), "Health")) Helpers.SetUpgrade("playerUpgradeHealth", c);
            if (GUI.Button(new Rect(30, btnY + gap, 180, btnH), "Stamina")) Helpers.SetUpgrade("playerUpgradeStamina", c);
            if (GUI.Button(new Rect(30, btnY + gap * 2, 180, btnH), "Speed")) Helpers.SetUpgrade("playerUpgradeSpeed", c);
            if (GUI.Button(new Rect(30, btnY + gap * 3, 180, btnH), "Strength")) Helpers.SetUpgrade("playerUpgradeStrength", c);
            if (GUI.Button(new Rect(30, btnY + gap * 4, 180, btnH), "Jump")) Helpers.SetUpgrade("playerUpgradeExtraJump", c);
            if (GUI.Button(new Rect(30, btnY + gap * 5, 180, btnH), "Range")) Helpers.SetUpgrade("playerUpgradeRange", c);

            if (GUI.Button(new Rect(30, 290, 180, 25), "Back"))
                c.ShowUpgrades = false;
        }

        public static void DrawTrolls(CheatBehaviour c)
        {
            GUI.Box(new Rect(20, 20, 220, 280), "Troll Chat");
            GUI.Label(new Rect(30, 48, 180, 20), "multiplayer only");

            int btnY = 70, btnH = 28, gap = 33;
            if (GUI.Button(new Rect(30, btnY, 180, btnH), "Flashbang")) Helpers.SendChat("<size=-111111>hi");
            if (GUI.Button(new Rect(30, btnY + gap, 180, btnH), "Big Text")) Helpers.SendChat("<size=999>HELLO");
            if (GUI.Button(new Rect(30, btnY + gap * 2, 180, btnH), "Invisible")) Helpers.SendChat("<alpha=#00>ghost message");
            if (GUI.Button(new Rect(30, btnY + gap * 3, 180, btnH), "Spam Hello")) c.StartCoroutine(Helpers.SpamChat("hello", 3));
            if (GUI.Button(new Rect(30, btnY + gap * 4, 180, btnH), "Max Chaos")) Helpers.SendChat("<size=-111111><size=999>CHAOS");
            if (GUI.Button(new Rect(30, btnY + gap * 5, 180, btnH), "Custom...")) Helpers.SendChat("<size=-111111>custom troll");

            if (GUI.Button(new Rect(30, 248, 180, 25), "Back"))
                c.ShowTrolls = false;
        }
    }
}