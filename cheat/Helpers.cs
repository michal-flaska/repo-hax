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

        // blasts all valuables away from the player using rigidbody force
        public static void YeetValuables(CheatBehaviour c)
        {
            var pc = PlayerController.instance;
            if (pc == null) return;

            foreach (var item in c.Valuables)
            {
                if (item == null) continue;

                var rb = item.GetComponent<Rigidbody>();
                if (rb == null) continue;

                Vector3 dir = (item.transform.position - pc.transform.position).normalized;
                // upward angle so items fly out and up, not just sideways
                dir = (dir + Vector3.up * 0.5f).normalized;
                rb.AddForce(dir * 25f, ForceMode.Impulse);
            }
        }

        // Teleports the single most valuable item on the map to just in front of the player.
        // Zeroes out velocity so it doesn't go flying after landing.
        public static void FetchBestLoot(CheatBehaviour c)
        {
            var pc = PlayerController.instance;
            if (pc == null) return;

            ValuableObject best = null;
            float bestValue = float.MinValue;

            foreach (var item in c.Valuables)
            {
                if (item == null) continue;
                float price = (float)(CheatBehaviour.DollarValueField?.GetValue(item) ?? 0f);
                if (price > bestValue)
                {
                    bestValue = price;
                    best = item;
                }
            }

            if (best == null) return;

            // drop it 1.5m in front and slightly above the player so it lands at feet
            Vector3 target = pc.transform.position + pc.transform.forward * 1.5f + Vector3.up * 0.5f;
            best.transform.position = target;

            var rb = best.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }

        public static void RespawnDeadPlayers()
        {
            var spawnPoints = UnityEngine.Object.FindObjectsOfType<SpawnPoint>();
            if (spawnPoints.Length == 0) return;

            foreach (PlayerAvatar player in GameDirector.instance.PlayerList)
            {
                if (!(bool)(CheatBehaviour.DeadSetField?.GetValue(player) ?? false)) continue;

                // skip players whose head is at the void position (already being respawned)
                if (player.playerDeathHead != null &&
                    player.playerDeathHead.transform.position == new Vector3(0f, 3000f, 0f)) continue;

                var spawn = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)];
                player.Revive(false);
                player.Spawn(spawn.transform.position, spawn.transform.rotation);
                player.playerHealth.HealOther(100, true);
            }
        }
    }
}