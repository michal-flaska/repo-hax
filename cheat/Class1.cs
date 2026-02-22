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

        // multiplier: 1x = normal, 2x = double speed, etc.
        private float _speedMultiplier = 1f;

        // base speeds from dnSpy:
        // PlayerController.MoveSpeed   - Token: 0x04002201, public float, default 0.5f
        // PlayerController.SprintSpeed - Token: 0x04002208, public float, default 1f
        private const float DEFAULT_MOVE_SPEED = 0.5f;
        private const float DEFAULT_SPRINT_SPEED = 1f;

        // PlayerHealth.health   - Token: 0x04002377, internal int, default 100
        // PlayerAvatar.isLocal  - Token: 0x040020B1, internal bool
        // PlayerAvatar.instance - Token: 0x040020E4, public static
        // PlayerAvatar.playerHealth - Token: 0x0400209E, public PlayerHealth
        private static readonly FieldInfo _healthField = typeof(PlayerHealth)
            .GetField("health", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

        void Update()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.Insert))
                _menuOpen = !_menuOpen;

            // PlayerController.instance is set in PlayerAvatar.Start() when photonView.IsMine
            // safe to access as singleton after game loads
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
                {
                    pc.MoveSpeed = DEFAULT_MOVE_SPEED * _speedMultiplier;
                    pc.SprintSpeed = DEFAULT_SPRINT_SPEED * _speedMultiplier;
                }
                else
                {
                    // reset to defaults when disabled
                    pc.MoveSpeed = DEFAULT_MOVE_SPEED;
                    pc.SprintSpeed = DEFAULT_SPRINT_SPEED;
                }
            }
        }

        void OnGUI()
        {
            if (!_menuOpen) return;

            UnityEngine.GUI.Box(new Rect(20, 20, 220, 180), "REPO Cheat");

            _godMode = UnityEngine.GUI.Toggle(new Rect(30, 50, 180, 25), _godMode, "God Mode");
            _speedHack = UnityEngine.GUI.Toggle(new Rect(30, 80, 180, 25), _speedHack, "Speed Multiplier");

            if (_speedHack)
            {
                _speedMultiplier = UnityEngine.GUI.HorizontalSlider(new Rect(30, 110, 160, 20), _speedMultiplier, 1f, 5f);
                UnityEngine.GUI.Label(new Rect(30, 128, 180, 20), $"x{_speedMultiplier:F1} speed");
            }
        }
    }
}