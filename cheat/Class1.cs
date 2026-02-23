using System;
using System.Reflection;
using UnityEngine;

// this file looks like absolute mess
// i will update this later
// just too lazy to do it rn

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
        private bool _infiniteStamina = false;

        private bool _showUpgrades = false;
        private bool _showTrolls = false;
        private int _upgradeValue = 10;

        // dnspy shit notes

        // PlayerHealth.health        - Token: 0x04002377, internal int, default 100
        // PlayerAvatar.isLocal       - Token: 0x040020B1, internal bool
        // PlayerAvatar.instance      - Token: 0x040020E4, public static
        // PlayerAvatar.playerHealth  - Token: 0x0400209E, public PlayerHealth
        // PlayerAvatar.steamID       - Token: 0x040020AC, internal string
        // PlayerAvatar.playerName    - Token: 0x040020AB, internal string
        // PlayerController.instance  - Token: 0x040021EE, public static
        // PlayerController.DebugNoTumble - Token: 0x04002228, public bool
        //   TumbleRequest checks this - if true and _playerInput is false, ragdoll is blocked
        // PlayerController.DebugEnergy - Token: 0x0400222A, public bool
        //   if true, SprintDrainTimer never drains EnergyCurrent and slide costs 0
        // PlayerController.EnergyCurrent - Token: 0x0400222C, public float
        // PlayerController.EnergyStart   - Token: 0x0400222B, public float, default 100
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
        // ExtractionPoint.safetySpawn - public Transform
        //   isLocked - public bool
        // ChatManager.instance       - Token: 0x040024CC, public static
        // ChatManager.ForceSendMessage(string _message) - Token: 0x06001681, public
        //   sets chatMessage then calls ForceConfirmChat() -> StateSet(Send)
        //   only works in multiplayer - Update() returns early in singleplayer
        // chat tricks (TMP rich text parsed by the game):
        //   flashbang: <size=-111111>text   - sets text size to near-zero causing screen flash
        //   big text:  <size=999>text       - massive text on everyone's screen
        //   invisible: <alpha=#00>text      - sends invisible message
        //   rainbow:   <gradient>text       - colored gradient text
        private static readonly FieldInfo _healthField = typeof(PlayerHealth)
            .GetField("health", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

        private static readonly FieldInfo _steamIDField = typeof(PlayerAvatar)
            .GetField("steamID", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

        private static readonly FieldInfo _isValuableField = typeof(PhysGrabObject)
            .GetField("isValuable", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

        private static readonly FieldInfo _playerNameField = typeof(PlayerAvatar)
            .GetField("playerName", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

        private static readonly FieldInfo _isLocalField = typeof(PlayerAvatar)
            .GetField("isLocal", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

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

                // DebugEnergy skips SprintDrainTimer drain and slide energy cost
                pc.DebugEnergy = _infiniteStamina;
                if (_infiniteStamina)
                    pc.EnergyCurrent = pc.EnergyStart;
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

            if (!_showUpgrades && !_showTrolls)
                DrawMainMenu();
            else if (_showUpgrades)
                DrawUpgradesMenu();
            else if (_showTrolls)
                DrawTrollMenu();
        }

        private void DrawMainMenu()
        {
            UnityEngine.GUI.Box(new Rect(20, 20, 220, 310), "REPO Cheat");

            _godMode = UnityEngine.GUI.Toggle(new Rect(30, 50, 180, 25), _godMode, "God Mode");
            _speedHack = UnityEngine.GUI.Toggle(new Rect(30, 80, 180, 25), _speedHack, "Speed Multiplier");
            _noRagdoll = UnityEngine.GUI.Toggle(new Rect(30, 110, 180, 25), _noRagdoll, "No Ragdoll");
            _noBreak = UnityEngine.GUI.Toggle(new Rect(30, 140, 180, 25), _noBreak, "No Break");
            _esp = UnityEngine.GUI.Toggle(new Rect(30, 170, 180, 25), _esp, "Player ESP");
            _infiniteStamina = UnityEngine.GUI.Toggle(new Rect(30, 200, 180, 25), _infiniteStamina, "Infinite Stamina");

            if (_speedHack)
            {
                _speedMultiplier = UnityEngine.GUI.HorizontalSlider(new Rect(30, 228, 160, 20), _speedMultiplier, 1f, 5f);
                UnityEngine.GUI.Label(new Rect(30, 243, 180, 20), $"x{_speedMultiplier:F1} speed");
            }

            if (UnityEngine.GUI.Button(new Rect(30, 268, 85, 28), "Upgrades"))
                _showUpgrades = true;

            if (UnityEngine.GUI.Button(new Rect(125, 268, 85, 28), "TP Extract"))
                TeleportToExtraction();

            if (UnityEngine.GUI.Button(new Rect(30, 300, 180, 25), "Troll Chat"))
                _showTrolls = true;
        }

        private void DrawUpgradesMenu()
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

        private void DrawTrollMenu()
        {
            UnityEngine.GUI.Box(new Rect(20, 20, 220, 280), "Troll Chat");
            UnityEngine.GUI.Label(new Rect(30, 48, 180, 20), "multiplayer only");

            int btnY = 70;
            int btnH = 28;
            int gap = 33;

            // <size=-111111> causes TMP to render at near-zero size, creating a flashbang effect
            if (UnityEngine.GUI.Button(new Rect(30, btnY, 180, btnH), "Flashbang"))
                SendChat("<size=-111111>hi");

            // oversized text renders huge on everyone's screen
            if (UnityEngine.GUI.Button(new Rect(30, btnY + gap, 180, btnH), "Big Text"))
                SendChat("<size=999>HELLO");

            // alpha=00 makes text invisible but still sent
            if (UnityEngine.GUI.Button(new Rect(30, btnY + gap * 2, 180, btnH), "Invisible"))
                SendChat("<alpha=#00>ghost message");

            // spam the chat 3 times (spamTimer is 1s so we bypass via ForceSendMessage calls)
            if (UnityEngine.GUI.Button(new Rect(30, btnY + gap * 3, 180, btnH), "Spam Hello"))
                StartCoroutine(SpamChat("hello", 3));

            // combine flashbang + big text for max chaos
            if (UnityEngine.GUI.Button(new Rect(30, btnY + gap * 4, 180, btnH), "Max Chaos"))
                SendChat("<size=-111111><size=999>CHAOS");

            if (UnityEngine.GUI.Button(new Rect(30, btnY + gap * 5, 180, btnH), "Custom..."))
                SendChat("<size=-111111>custom troll");

            if (UnityEngine.GUI.Button(new Rect(30, 248, 180, 25), "Back"))
                _showTrolls = false;
        }

        private void SendChat(string message)
        {
            if (ChatManager.instance == null) return;
            ChatManager.instance.ForceSendMessage(message);
        }

        private System.Collections.IEnumerator SpamChat(string message, int times)
        {
            for (int i = 0; i < times; i++)
            {
                SendChat(message);
                yield return new WaitForSeconds(1.1f); // just over spamTimer (1s)
            }
        }

        private void DrawESP()
        {
            var cam = Camera.main;
            if (cam == null) return;

            foreach (var avatar in UnityEngine.Object.FindObjectsOfType<PlayerAvatar>())
            {
                bool isLocal = (bool)(_isLocalField?.GetValue(avatar) ?? false);
                if (isLocal) continue;

                // Unity screen origin is bottom-left, GUI is top-left so flip Y
                Vector3 screenPos = cam.WorldToScreenPoint(avatar.transform.position + Vector3.up * 1.5f);
                if (screenPos.z < 0) continue;

                float screenY = Screen.height - screenPos.y;
                string name = _playerNameField?.GetValue(avatar) as string ?? "Player";
                int hp = avatar.playerHealth != null ? (int)(_healthField?.GetValue(avatar.playerHealth) ?? 0) : 0;
                float distance = Vector3.Distance(PlayerAvatar.instance?.transform.position ?? Vector3.zero, avatar.transform.position);

                UnityEngine.GUI.Label(new Rect(screenPos.x - 30, screenY - 10, 120, 60), $"{name}\nHP: {hp}\n{distance:F0}m", _espStyle);
            }
        }

        private void TeleportToExtraction()
        {
            var pc = PlayerController.instance;
            if (pc == null) return;

            // find first unlocked extraction point and teleport to its safetySpawn
            foreach (var ep in UnityEngine.Object.FindObjectsOfType<ExtractionPoint>())
            {
                if (ep.isLocked) continue;

                // fallback to extraction point itself if no safetySpawn
                pc.transform.position = ep.safetySpawn != null
                    ? ep.safetySpawn.position
                    : ep.transform.position + Vector3.up;
                return;
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