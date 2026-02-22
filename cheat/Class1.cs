using System;
using System.Reflection;
using UnityEngine;

namespace cheat
{
    public class Loader
    {
        private static GameObject _cheatObject;

        public static void Load()
        {
            _cheatObject = new GameObject("REPOCheat");
            _cheatObject.AddComponent<CheatBehaviour>();
            UnityEngine.Object.DontDestroyOnLoad(_cheatObject);
        }

        public static void Unload()
        {
            UnityEngine.Object.Destroy(_cheatObject);
        }
    }

    public class CheatBehaviour : MonoBehaviour
    {
        private bool _menuOpen = false;
        private bool _godMode = false;
        private bool _speedHack = false;
        private float _speedMultiplier = 1f;

        private bool _showUpgrades = false;
        private int _upgradeValue = 10;

        // PlayerHealth.health        - Token: 0x04002377, internal int, default 100
        // PlayerAvatar.isLocal       - Token: 0x040020B1, internal bool
        // PlayerAvatar.instance      - Token: 0x040020E4, public static
        // PlayerAvatar.playerHealth  - Token: 0x0400209E, public PlayerHealth
        // PlayerAvatar.steamID       - Token: 0x040020AC, internal string
        // PlayerController.instance  - Token: 0x040021EE, public static
        // PlayerController.OverrideSpeed(float _speedMulti, float _time) - Token: 0x06001572
        //   internally multiplies playerOriginalMoveSpeed/SprintSpeed/CrouchSpeed
        //   playerOriginalMoveSpeed   - Token: 0x04002261, private float, set in LateStart() after upgrades
        //   playerOriginalSprintSpeed - Token: 0x04002263, internal float, set in LateStart() after upgrades
        //   playerOriginalCrouchSpeed - Token: 0x04002264, private float, set in LateStart() after upgrades
        // StatsManager.instance      - Token: 0x04001CB3, public static
        // StatsManager.DictionaryUpdateValue(string dictName, string steamID, int value) - Token: 0x06001292
        // upgrade dicts (all public Dictionary<string,int>, keyed by steamID):
        //   playerUpgradeHealth     - Token: 0x04001CBF  (each point = +20 max hp)
        //   playerUpgradeStamina    - Token: 0x04001CC0
        //   playerUpgradeSpeed      - Token: 0x04001CC8  (adds directly to SprintSpeed in LateStart)
        //   playerUpgradeStrength   - Token: 0x04001CC9
        //   playerUpgradeExtraJump  - Token: 0x04001CC1
        //   playerUpgradeRange      - Token: 0x04001CCB
        //   playerUpgradeThrow      - Token: 0x04001CCA
        private static readonly FieldInfo _healthField = typeof(PlayerHealth)
            .GetField("health", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

        private static readonly FieldInfo _steamIDField = typeof(PlayerAvatar)
            .GetField("steamID", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

        void Update()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.Insert))
                _menuOpen = !_menuOpen;

            var pc = PlayerController.instance;
            var ph = PlayerAvatar.instance?.playerHealth;

            if (_godMode && ph != null)
            {
                // PlayerHealth.InvincibleSet(float _time) - Token: 0x060015D6
                ph.InvincibleSet(9999f);
                _healthField?.SetValue(ph, 100);
            }

            if (pc != null)
            {
                if (_speedHack)
                    pc.OverrideSpeed(_speedMultiplier, 0.5f);
                else
                    pc.OverrideSpeed(1f, 0.1f);
            }
        }

        void OnGUI()
        {
            if (!_menuOpen) return;

            if (!_showUpgrades)
            {
                UnityEngine.GUI.Box(new Rect(20, 20, 220, 210), "REPO Cheat");

                _godMode = UnityEngine.GUI.Toggle(new Rect(30, 50, 180, 25), _godMode, "God Mode");
                _speedHack = UnityEngine.GUI.Toggle(new Rect(30, 80, 180, 25), _speedHack, "Speed Multiplier");

                if (_speedHack)
                {
                    _speedMultiplier = UnityEngine.GUI.HorizontalSlider(new Rect(30, 110, 160, 20), _speedMultiplier, 1f, 5f);
                    UnityEngine.GUI.Label(new Rect(30, 128, 180, 20), $"x{_speedMultiplier:F1} speed");
                }

                if (UnityEngine.GUI.Button(new Rect(30, 160, 180, 30), "Upgrades"))
                    _showUpgrades = true;
            }
            else
            {
                UnityEngine.GUI.Box(new Rect(20, 20, 220, 320), "Upgrades");

                UnityEngine.GUI.Label(new Rect(30, 50, 80, 20), "Set value:");
                if (UnityEngine.GUI.Button(new Rect(110, 48, 30, 22), "0")) _upgradeValue = 0;
                if (UnityEngine.GUI.Button(new Rect(145, 48, 30, 22), "10")) _upgradeValue = 10;
                if (UnityEngine.GUI.Button(new Rect(180, 48, 30, 22), "50")) _upgradeValue = 50;

                UnityEngine.GUI.Label(new Rect(30, 75, 180, 20), $"Current: {_upgradeValue}");

                int btnY = 100;
                int btnH = 28;
                int gap = 33;

                if (UnityEngine.GUI.Button(new Rect(30, btnY, 180, btnH), "Health")) SetUpgrade("playerUpgradeHealth");
                if (UnityEngine.GUI.Button(new Rect(30, btnY + gap, 180, btnH), "Stamina")) SetUpgrade("playerUpgradeStamina");
                if (UnityEngine.GUI.Button(new Rect(30, btnY + gap * 2, 180, btnH), "Speed")) SetUpgrade("playerUpgradeSpeed");
                if (UnityEngine.GUI.Button(new Rect(30, btnY + gap * 3, 180, btnH), "Strength")) SetUpgrade("playerUpgradeStrength");
                if (UnityEngine.GUI.Button(new Rect(30, btnY + gap * 4, 180, btnH), "Jump")) SetUpgrade("playerUpgradeExtraJump");
                if (UnityEngine.GUI.Button(new Rect(30, btnY + gap * 5, 180, btnH), "Range")) SetUpgrade("playerUpgradeRange");

                if (UnityEngine.GUI.Button(new Rect(30, 290, 180, 25), "Back"))
                    _showUpgrades = false;
            }
        }

        private void SetUpgrade(string dictName)
        {
            string steamID = _steamIDField?.GetValue(PlayerAvatar.instance) as string;
            if (string.IsNullOrEmpty(steamID)) return;

            // StatsManager.DictionaryUpdateValue updates the in-memory dict
            // note: speed/stamina upgrades apply at LateStart() on level load,
            // so they won't affect the current run — use speed multiplier instead
            StatsManager.instance.DictionaryUpdateValue(dictName, steamID, _upgradeValue);
        }
    }
}