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
        private bool _noRagdoll = false;
        private bool _noBreak = false;
        private bool _esp = false;

        private bool _showUpgrades = false;
        private int _upgradeValue = 10;

        // PlayerHealth.health        - Token: 0x04002377, internal int, default 100
        // PlayerAvatar.isLocal       - Token: 0x040020B1, internal bool
        // PlayerAvatar.instance      - Token: 0x040020E4, public static
        // PlayerAvatar.playerHealth  - Token: 0x0400209E, public PlayerHealth
        // PlayerAvatar.steamID       - Token: 0x040020AC, internal string
        // PlayerAvatar.playerName    - Token: 0x040020AB, internal string
        // PlayerController.instance  - Token: 0x040021EE, public static
        // PlayerController.DebugNoTumble - Token: 0x04002228, public bool
        //   TumbleRequest checks this - if true and _playerInput is false, ragdoll is blocked
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
        // PhysGrabObject.isValuable  - Token: 0x04001F0B, internal bool
        // PhysGrabObject.OverrideIndestructible(float time) - Token: 0x060013DD
        //   sets impactDetector.isIndestructible = true for duration
        //   called every frame on all isValuable objects to prevent breaking
        private static readonly FieldInfo _healthField = typeof(PlayerHealth)
            .GetField("health", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

        private static readonly FieldInfo _steamIDField = typeof(PlayerAvatar)
            .GetField("steamID", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

        private static readonly FieldInfo _isValuableField = typeof(PhysGrabObject)
            .GetField("isValuable", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

        private static readonly FieldInfo _playerNameField = typeof(PlayerAvatar)
            .GetField("playerName", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

        // ESP label style, initialized once
        private GUIStyle _espStyle;

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

                // PlayerController.DebugNoTumble blocks TumbleRequest when _playerInput is false
                // player-triggered tumble (key press) still works since it passes _playerInput = true
                pc.DebugNoTumble = _noRagdoll;
            }

            if (_noBreak)
            {
                foreach (var obj in UnityEngine.Object.FindObjectsOfType<PhysGrabObject>())
                {
                    if ((bool)(_isValuableField?.GetValue(obj) ?? false))
                        obj.OverrideIndestructible(0.5f);
                }
            }
        }

        void OnGUI()
        {
            // init style once
            if (_espStyle == null)
            {
                _espStyle = new GUIStyle(UnityEngine.GUI.skin.label);
                _espStyle.normal.textColor = Color.green;
                _espStyle.fontStyle = FontStyle.Bold;
                _espStyle.fontSize = 14;
            }

            if (_esp)
                DrawESP();

            if (!_menuOpen) return;

            if (!_showUpgrades)
            {
                UnityEngine.GUI.Box(new Rect(20, 20, 220, 270), "REPO Cheat");

                _godMode = UnityEngine.GUI.Toggle(new Rect(30, 50, 180, 25), _godMode, "God Mode");
                _speedHack = UnityEngine.GUI.Toggle(new Rect(30, 80, 180, 25), _speedHack, "Speed Multiplier");
                _noRagdoll = UnityEngine.GUI.Toggle(new Rect(30, 110, 180, 25), _noRagdoll, "No Ragdoll");
                _noBreak = UnityEngine.GUI.Toggle(new Rect(30, 140, 180, 25), _noBreak, "No Break");
                _esp = UnityEngine.GUI.Toggle(new Rect(30, 170, 180, 25), _esp, "Player ESP");

                if (_speedHack)
                {
                    _speedMultiplier = UnityEngine.GUI.HorizontalSlider(new Rect(30, 198, 160, 20), _speedMultiplier, 1f, 5f);
                    UnityEngine.GUI.Label(new Rect(30, 213, 180, 20), $"x{_speedMultiplier:F1} speed");
                }

                if (UnityEngine.GUI.Button(new Rect(30, 235, 180, 30), "Upgrades"))
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

        private void DrawESP()
        {
            var cam = Camera.main;
            if (cam == null) return;

            foreach (var avatar in UnityEngine.Object.FindObjectsOfType<PlayerAvatar>())
            {
                // skip local player
                bool isLocal = (bool)(_isLocalField?.GetValue(avatar) ?? false);
                if (isLocal) continue;

                // world to screen - Unity screen origin is bottom-left, GUI is top-left so flip Y
                Vector3 screenPos = cam.WorldToScreenPoint(avatar.transform.position + Vector3.up * 1.5f);

                // behind camera check
                if (screenPos.z < 0) continue;

                float screenY = Screen.height - screenPos.y;
                string name = _playerNameField?.GetValue(avatar) as string ?? "Player";
                int hp = avatar.playerHealth != null
                    ? (int)(_healthField?.GetValue(avatar.playerHealth) ?? 0)
                    : 0;

                float distance = Vector3.Distance(
                    PlayerAvatar.instance?.transform.position ?? Vector3.zero,
                    avatar.transform.position
                );

                string label = $"{name}\nHP: {hp}\n{distance:F0}m";
                UnityEngine.GUI.Label(new Rect(screenPos.x - 30, screenY - 10, 120, 60), label, _espStyle);
            }
        }

        // PlayerAvatar.isLocal - Token: 0x040020B1, internal bool
        private static readonly FieldInfo _isLocalField = typeof(PlayerAvatar)
            .GetField("isLocal", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

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