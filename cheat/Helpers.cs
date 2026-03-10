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
                dir = (dir + Vector3.up * 0.5f).normalized;
                rb.AddForce(dir * 25f, ForceMode.Impulse);
            }
        }

        // Teleports the single most valuable item outside 10m to just in front of the player.
        // Uses physGrabObject.Teleport so it syncs to all clients via Photon.
        public static void FetchBestLoot(CheatBehaviour c)
        {
            var pc = PlayerController.instance;
            if (pc == null) return;

            ValuableObject best = null;
            float bestValue = float.MinValue;

            foreach (var item in c.Valuables)
            {
                if (item == null) continue;
                if (Vector3.Distance(pc.transform.position, item.transform.position) <= 10f) continue;
                float price = (float)(CheatBehaviour.DollarValueField?.GetValue(item) ?? 0f);
                if (price > bestValue)
                {
                    bestValue = price;
                    best = item;
                }
            }

            if (best == null) return;
            var bestPgo = best.GetComponent<PhysGrabObject>();
            if (bestPgo == null) return;

            Vector3 dest = pc.transform.position + pc.transform.forward * 1.5f + Vector3.up * 0.5f;
            bestPgo.Teleport(dest, best.transform.rotation);
        }

        public static void FetchCheapestLoot(CheatBehaviour c)
        {
            var pc = PlayerController.instance;
            if (pc == null) return;

            ValuableObject worst = null;
            float worstValue = float.MaxValue;

            foreach (var item in c.Valuables)
            {
                if (item == null) continue;
                if (Vector3.Distance(pc.transform.position, item.transform.position) <= 10f) continue;
                float price = (float)(CheatBehaviour.DollarValueField?.GetValue(item) ?? 0f);
                if (price < worstValue)
                {
                    worstValue = price;
                    worst = item;
                }
            }

            if (worst == null) return;
            var pgo = worst.GetComponent<PhysGrabObject>();
            if (pgo == null) return;

            Vector3 dest = pc.transform.position + pc.transform.forward * 1.5f + Vector3.up * 0.5f;
            pgo.Teleport(dest, worst.transform.rotation);
        }

        // Teleports every valuable outside the extraction zone into it.
        // Uses physGrabObject.Teleport so it syncs to all clients via Photon.
        public static void AutoExtract(CheatBehaviour c)
        {
            ExtractionPoint target = null;
            float closest = float.MaxValue;
            var pc = PlayerController.instance;
            Vector3 origin = pc != null ? pc.transform.position : Vector3.zero;

            foreach (var ep in c.Extractions)
            {
                if (ep == null || ep.isLocked) continue;
                float d = Vector3.Distance(origin, ep.transform.position);
                if (d < closest) { closest = d; target = ep; }
            }

            if (target == null) return;

            var col = target.GetComponent<Collider>();
            Vector3 dropPos = col != null ? col.bounds.center : target.transform.position;

            int i = 0;
            foreach (var item in c.Valuables)
            {
                if (item == null) continue;
                var pgo = item.GetComponent<PhysGrabObject>();
                if (pgo == null) continue;
                if (col != null && col.bounds.Contains(item.transform.position)) continue;

                float ox = (i % 4) * 0.4f - 0.6f;
                float oz = (i / 4) * 0.4f - 0.6f;
                Vector3 dest = dropPos + new Vector3(ox, 0.3f, oz);
                pgo.Teleport(dest, item.transform.rotation);
                i++;
            }
        }

        public static void RespawnDeadPlayers()
        {
            var spawnPoints = UnityEngine.Object.FindObjectsOfType<SpawnPoint>();
            if (spawnPoints.Length == 0) return;

            foreach (PlayerAvatar player in GameDirector.instance.PlayerList)
            {
                if (!(bool)(CheatBehaviour.DeadSetField?.GetValue(player) ?? false)) continue;

                if (player.playerDeathHead != null &&
                    player.playerDeathHead.transform.position == new Vector3(0f, 3000f, 0f)) continue;

                var spawn = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)];
                player.Revive(false);
                player.Spawn(spawn.transform.position, spawn.transform.rotation);
                player.playerHealth.HealOther(100, true);
            }
        }

        public static void MaxLootValue(CheatBehaviour c)
        {
            foreach (var item in c.Valuables)
            {
                if (item == null) continue;
                CheatBehaviour.DollarValueField?.SetValue(item, 99999f);
            }
        }
    }
}