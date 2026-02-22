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
        private float _speedValue = 0.5f; // default MoveSpeed value from dnSpy: PlayerController.MoveSpeed = 0.5f

        private PlayerHealth _localPlayerHealth;
        private PlayerController _localPlayerController;

        // PlayerHealth.health - Token: 0x04002377, internal int, default 100
        private static readonly FieldInfo _healthField = typeof(PlayerHealth).GetField("health", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

        // PlayerAvatar.isLocal - Token: 0x040020B1, internal bool
        private static readonly FieldInfo _isLocalField = typeof(PlayerAvatar).GetField("isLocal", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

        void Update()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.Insert))
                _menuOpen = !_menuOpen;

            if (_localPlayerHealth == null || _localPlayerController == null)
                FindLocalPlayer();

            if (_godMode && _localPlayerHealth != null)
            {
                // PlayerHealth.InvincibleSet(float _time) - Token: 0x060015D6
                _localPlayerHealth.InvincibleSet(9999f);
                // PlayerHealth.health - clamped to 100 every frame
                _healthField?.SetValue(_localPlayerHealth, 100);
            }

            if (_speedHack && _localPlayerController != null)
            {
                // PlayerController.MoveSpeed - Token: 0x04002201, public float, default 0.5f
                _localPlayerController.MoveSpeed = _speedValue;
            }
            else if (!_speedHack && _localPlayerController != null)
            {
                // reset to default when disabled
                _localPlayerController.MoveSpeed = 0.5f;
            }
        }

        void OnGUI()
        {
            if (!_menuOpen) return;

            GUI.Box(new Rect(20, 20, 220, 160), "REPO Cheat");

            _godMode = UnityEngine.GUI.Toggle(new Rect(30, 50, 180, 25), _godMode, "God Mode");

            _speedHack = UnityEngine.GUI.Toggle(new Rect(30, 80, 180, 25), _speedHack, "Speed Hack");

            if (_speedHack)
            {
                // slider: min 0.5 (default), max 5.0
                _speedValue = UnityEngine.GUI.HorizontalSlider(new Rect(30, 110, 160, 20), _speedValue, 0.5f, 5f);
                UnityEngine.GUI.Label(new Rect(30, 128, 180, 20), $"Speed: {_speedValue:F2}");
            }
        }

        private void FindLocalPlayer()
        {
            // find PlayerHealth on local player via PlayerAvatar.isLocal
            foreach (var p in UnityEngine.Object.FindObjectsOfType<PlayerHealth>())
            {
                PlayerAvatar avatar = p.GetComponent<PlayerAvatar>();
                if (avatar != null && (bool)(_isLocalField?.GetValue(avatar) ?? false))
                {
                    _localPlayerHealth = p;
                    // PlayerController is not on the same GO as PlayerHealth, find separately
                    _localPlayerController = p.GetComponentInParent<PlayerController>()
                                          ?? p.GetComponentInChildren<PlayerController>()
                                          ?? UnityEngine.Object.FindObjectOfType<PlayerController>();
                    return;
                }
            }
        }
    }
}