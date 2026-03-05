using System;
using System.Collections;
using System.Reflection;
using System.Linq;
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
        // -- toggles --
        private bool _menuOpen = false;
        private bool _godMode = false;
        private bool _speedHack = false;
        private float _speedMultiplier = 1f;
        private bool _noRagdoll = false;
        private bool _noBreak = false;
        private bool _infiniteStamina = false;

        // esp toggles
        private bool _espPlayers = false;
        private bool _espEnemies = false;
        private bool _espLoot = false;
        private bool _espExtraction = false;

        // sub-toggles
        private bool _espPlayerDist = true;
        private bool _espPlayerHp = true;
        private bool _espEnemyDist = true;
        private bool _espEnemyHp = true;
        private bool _espLootPrice = true;

        // menu tabs
        private bool _showUpgrades = false;
        private bool _showTrolls = false;
        private int _upgradeValue = 10;

        // -- cached objects (refreshed every 10s) --
        private ValuableObject[] _valuables = new ValuableObject[0];
        private ExtractionPoint[] _extractions = new ExtractionPoint[0];
        private Enemy[] _enemies = new Enemy[0];
        private PlayerController _localPlayer;

        // -- reflection fields --
        private static readonly FieldInfo _healthField =
            typeof(PlayerHealth).GetField("health", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

        private static readonly FieldInfo _steamIDField =
            typeof(PlayerAvatar).GetField("steamID", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

        private static readonly FieldInfo _isValuableField =
            typeof(PhysGrabObject).GetField("isValuable", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

        // playerName lives on PlayerController, not PlayerAvatar
        private static readonly FieldInfo _playerNameField =
            typeof(PlayerController).GetField("playerName", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

        private static readonly FieldInfo _dollarValueField =
            typeof(ValuableObject).GetField("dollarValueCurrent", BindingFlags.Instance | BindingFlags.NonPublic);

        // -- esp style ---
        private GUIStyle _espStyle;

        // ----------------

        void Start()
        {
            StartCoroutine(RefreshObjects());
        }

        private IEnumerator RefreshObjects()
        {
            while (true)
            {
                _valuables = UnityEngine.Object.FindObjectsOfType<ValuableObject>();
                _extractions = UnityEngine.Object.FindObjectsOfType<ExtractionPoint>();
                _enemies = UnityEngine.Object.FindObjectsOfType<Enemy>();

                var allControllers = UnityEngine.Object.FindObjectsOfType<PlayerController>();
                _localPlayer = allControllers.FirstOrDefault(p => p.cameraGameObjectLocal != null);

                yield return new WaitForSeconds(10f);
            }
        }

        void Update()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.Insert))
                _menuOpen = !_menuOpen;

            var pc = PlayerController.instance;
            var ph = PlayerAvatar.instance?.playerHealth;

            if (_godMode && ph != null)
            {
                ph.InvincibleSet(9999f);
                _healthField?.SetValue(ph, 100);
            }

            if (pc != null)
            {
                pc.OverrideSpeed(_speedHack ? _speedMultiplier : 1f, _speedHack ? 0.5f : 0.1f);
                pc.DebugNoTumble = _noRagdoll;
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
                _espStyle = new GUIStyle(UnityEngine.GUI.skin.label)
                {
                    fontSize = 13,
                    fontStyle = FontStyle.Bold,
                    alignment = TextAnchor.MiddleCenter
                };
            }

            DrawESP();

            if (!_menuOpen) return;

            if (_showUpgrades) DrawUpgradesMenu();
            else if (_showTrolls) DrawTrollMenu();
            else DrawMainMenu();
        }

        // -- ESP --

        private void DrawESP()
        {
            var cam = Camera.main;
            if (cam == null) return;

            // Players
            if (_espPlayers)
            {
                var allControllers = UnityEngine.Object.FindObjectsOfType<PlayerController>();
                foreach (var player in allControllers)
                {
                    if (player.cameraGameObjectLocal != null) continue; // skip local

                    Vector3 screenPos = WorldToScreen(cam, player.transform.position + Vector3.up);
                    if (!IsOnScreen(screenPos)) continue;

                    string name = _playerNameField?.GetValue(player) as string ?? "Player";
                    int hp = (int)(_healthField?.GetValue(player.playerAvatarScript?.playerHealth) ?? 0);
                    if (hp <= 0) continue;

                    string label = name;
                    if (_espPlayerDist && _localPlayer != null)
                        label += $" [{Vector3.Distance(_localPlayer.transform.position, player.transform.position):F0}m]";
                    if (_espPlayerHp)
                        label += $"\n{hp}HP";

                    DrawLabel(screenPos, label, Color.cyan);
                }
            }

            // Enemies
            if (_espEnemies)
            {
                foreach (var enemy in _enemies)
                {
                    if (enemy == null) continue;

                    EnemyParent parent = enemy.GetComponentInParent<EnemyParent>();
                    EnemyHealth eHealth = parent?.GetComponentInChildren<EnemyHealth>();
                    if (eHealth == null || eHealth.health <= 0) continue;

                    Vector3 screenPos = WorldToScreen(cam, enemy.CenterTransform.position);
                    if (!IsOnScreen(screenPos)) continue;

                    string label = parent.enemyName;
                    if (_espEnemyDist && _localPlayer != null)
                        label += $" [{Vector3.Distance(_localPlayer.transform.position, enemy.CenterTransform.position):F0}m]";
                    if (_espEnemyHp)
                        label += $"\n{eHealth.health}HP";

                    DrawLabel(screenPos, label, Color.red);
                }
            }

            // Loot
            if (_espLoot)
            {
                foreach (var item in _valuables)
                {
                    if (item == null) continue;

                    Vector3 screenPos = WorldToScreen(cam, item.transform.position);
                    if (!IsOnScreen(screenPos)) continue;

                    float price = (float)(_dollarValueField?.GetValue(item) ?? 0f);
                    string label = item.name + (_espLootPrice ? $"\n${price:F0}" : "");

                    DrawLabel(screenPos, label, Color.yellow);
                }
            }

            // Extraction points
            if (_espExtraction)
            {
                foreach (var ep in _extractions)
                {
                    if (ep == null) continue;

                    Vector3 screenPos = WorldToScreen(cam, ep.transform.position);
                    if (!IsOnScreen(screenPos)) continue;

                    Color col = ep.isLocked ? Color.red : Color.green;
                    DrawLabel(screenPos, ep.isLocked ? "Extraction [LOCKED]" : "Extraction", col);
                }
            }
        }

        private Vector3 WorldToScreen(Camera cam, Vector3 worldPos)
        {
            Matrix4x4 vp = cam.projectionMatrix * cam.worldToCameraMatrix;
            Vector4 clip = vp * new Vector4(worldPos.x, worldPos.y, worldPos.z, 1f);

            if (clip.w <= 0f)
                return new Vector3(-1, -1, -1);

            Vector3 ndc = new Vector3(clip.x / clip.w, clip.y / clip.w, clip.z / clip.w);

            return new Vector3(
                (ndc.x + 1f) * 0.5f * Screen.width,
                (1f - ndc.y) * 0.5f * Screen.height,
                ndc.z
            );
        }

        private bool IsOnScreen(Vector3 pos)
        {
            return pos.x > 0 && pos.x < Screen.width &&
                   pos.y > 0 && pos.y < Screen.height &&
                   pos.z > 0;
        }

        private void DrawLabel(Vector3 screenPos, string text, Color color)
        {
            _espStyle.normal.textColor = color;
            var content = new GUIContent(text);
            Vector2 size = _espStyle.CalcSize(content);
            UnityEngine.GUI.Label(
                new Rect(screenPos.x - size.x / 2f, screenPos.y - size.y / 2f, size.x, size.y),
                content, _espStyle
            );
        }

        // -- Menus --

        private void DrawMainMenu()
        {
            UnityEngine.GUI.Box(new Rect(20, 20, 230, 390), "REPO Cheat");

            int y = 50;
            _godMode = UnityEngine.GUI.Toggle(new Rect(30, y, 190, 25), _godMode, "God Mode");
            _speedHack = UnityEngine.GUI.Toggle(new Rect(30, y += 30, 190, 25), _speedHack, "Speed Multiplier");
            _noRagdoll = UnityEngine.GUI.Toggle(new Rect(30, y += 30, 190, 25), _noRagdoll, "No Ragdoll");
            _noBreak = UnityEngine.GUI.Toggle(new Rect(30, y += 30, 190, 25), _noBreak, "No Break");
            _infiniteStamina = UnityEngine.GUI.Toggle(new Rect(30, y += 30, 190, 25), _infiniteStamina, "Infinite Stamina");

            // ESP toggles
            UnityEngine.GUI.Label(new Rect(30, y += 32, 190, 18), "-- ESP --");
            _espPlayers = UnityEngine.GUI.Toggle(new Rect(30, y += 22, 190, 22), _espPlayers, "Player ESP");
            _espEnemies = UnityEngine.GUI.Toggle(new Rect(30, y += 26, 190, 22), _espEnemies, "Enemy ESP");
            _espLoot = UnityEngine.GUI.Toggle(new Rect(30, y += 26, 190, 22), _espLoot, "Loot ESP");
            _espExtraction = UnityEngine.GUI.Toggle(new Rect(30, y += 26, 190, 22), _espExtraction, "Extraction ESP");

            if (_speedHack)
            {
                _speedMultiplier = UnityEngine.GUI.HorizontalSlider(new Rect(30, y += 32, 160, 20), _speedMultiplier, 1f, 5f);
                UnityEngine.GUI.Label(new Rect(30, y += 16, 190, 20), $"x{_speedMultiplier:F1} speed");
            }
            else
            {
                y += 36;
            }

            if (UnityEngine.GUI.Button(new Rect(30, y += 10, 90, 28), "Upgrades"))
                _showUpgrades = true;

            if (UnityEngine.GUI.Button(new Rect(130, y, 90, 28), "TP Extract"))
                TeleportToExtraction();

            if (UnityEngine.GUI.Button(new Rect(30, y + 33, 190, 25), "Troll Chat"))
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

            int btnY = 100, btnH = 28, gap = 33;
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

            int btnY = 70, btnH = 28, gap = 33;
            if (UnityEngine.GUI.Button(new Rect(30, btnY, 180, btnH), "Flashbang")) SendChat("<size=-111111>hi");
            if (UnityEngine.GUI.Button(new Rect(30, btnY + gap, 180, btnH), "Big Text")) SendChat("<size=999>HELLO");
            if (UnityEngine.GUI.Button(new Rect(30, btnY + gap * 2, 180, btnH), "Invisible")) SendChat("<alpha=#00>ghost message");
            if (UnityEngine.GUI.Button(new Rect(30, btnY + gap * 3, 180, btnH), "Spam Hello")) StartCoroutine(SpamChat("hello", 3));
            if (UnityEngine.GUI.Button(new Rect(30, btnY + gap * 4, 180, btnH), "Max Chaos")) SendChat("<size=-111111><size=999>CHAOS");
            if (UnityEngine.GUI.Button(new Rect(30, btnY + gap * 5, 180, btnH), "Custom...")) SendChat("<size=-111111>custom troll");

            if (UnityEngine.GUI.Button(new Rect(30, 248, 180, 25), "Back"))
                _showTrolls = false;
        }

        // -- Helpers --

        private void SendChat(string message)
        {
            if (ChatManager.instance == null) return;
            ChatManager.instance.ForceSendMessage(message);
        }

        private IEnumerator SpamChat(string message, int times)
        {
            for (int i = 0; i < times; i++)
            {
                SendChat(message);
                yield return new WaitForSeconds(1.1f);
            }
        }

        private void TeleportToExtraction()
        {
            var pc = PlayerController.instance;
            if (pc == null) return;

            foreach (var ep in UnityEngine.Object.FindObjectsOfType<ExtractionPoint>())
            {
                if (ep.isLocked) continue;
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
            StatsManager.instance.DictionaryUpdateValue(dictName, steamID, _upgradeValue);
        }
    }
}