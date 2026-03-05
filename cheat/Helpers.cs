using System.Collections;
using UnityEngine;

namespace cheat
{
    public static class Helpers
    {
        public static void SendChat(string message)
        {
            if (ChatManager.instance == null) return;
            ChatManager.instance.ForceSendMessage(message);
        }

        public static IEnumerator SpamChat(string message, int times)
        {
            for (int i = 0; i < times; i++)
            {
                SendChat(message);
                yield return new WaitForSeconds(1.1f);
            }
        }

        public static void TeleportToExtraction()
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

        public static void SetUpgrade(string dictName, CheatBehaviour c)
        {
            string steamID = CheatBehaviour.SteamIDField?.GetValue(PlayerAvatar.instance) as string;
            if (string.IsNullOrEmpty(steamID)) return;
            StatsManager.instance.DictionaryUpdateValue(dictName, steamID, c.UpgradeValue);
        }
    }
}