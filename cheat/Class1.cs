using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Windows;

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

        private PlayerHealth _localPlayerHealth;

        void Update()
        {
            // Toggle menu with Insert key
            if (Input.GetKeyDown(KeyCode.Insert))
                _menuOpen = !_menuOpen;

            // Try to find local player health if we don't have it
            if (_localPlayerHealth == null)
                _localPlayerHealth = FindLocalPlayer();

            if (_godMode && _localPlayerHealth != null)
            {
                _localPlayerHealth.InvincibleSet(9999f);
                _localPlayerHealth.health = 100;
            }
        }

        void OnGUI()
        {
            if (!_menuOpen) return;

            GUI.Box(new Rect(20, 20, 200, 100), "REPO Cheat");

            _godMode = GUI.Toggle(new Rect(30, 50, 180, 25), _godMode, "God Mode");
        }

        private PlayerHealth FindLocalPlayer()
        {
            // Find all PlayerHealth instances and return the local one
            PlayerHealth[] players = UnityEngine.Object.FindObjectsOfType<PlayerHealth>();
            foreach (var p in players)
            {
                // PlayerAvatar is the local player component
                PlayerAvatar avatar = p.GetComponent<PlayerAvatar>();
                if (avatar != null && avatar.isLocal)
                    return p;
            }
            return null;
        }
    }
}