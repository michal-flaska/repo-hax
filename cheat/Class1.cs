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

        // PlayerHealth.health        - Token: 0x04002377, internal int, default 100
        // PlayerAvatar.isLocal       - Token: 0x040020B1, internal bool
        // PlayerAvatar.instance      - Token: 0x040020E4, public static
        // PlayerAvatar.playerHealth  - Token: 0x0400209E, public PlayerHealth
        // PlayerController.instance  - Token: 0x040021EE, public static
        // PlayerController.OverrideSpeed(float _speedMulti, float _time) - Token: 0x06001572
        //   internally multiplies playerOriginalMoveSpeed/SprintSpeed/CrouchSpeed
        //   playerOriginalMoveSpeed   - Token: 0x04002261, private float, set in LateStart() after upgrades
        //   playerOriginalSprintSpeed - Token: 0x04002263, internal float, set in LateStart() after upgrades
        //   playerOriginalCrouchSpeed - Token: 0x04002264, private float, set in LateStart() after upgrades
        private static readonly FieldInfo _healthField = typeof(PlayerHealth)
            .GetField("health", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

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
                    // keep refreshing the timer so it never expires
                    pc.OverrideSpeed(_speedMultiplier, 0.5f);
                else
                    pc.OverrideSpeed(1f, 0.1f);
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