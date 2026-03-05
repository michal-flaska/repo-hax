using UnityEngine;

namespace cheat
{
    // ESP is based on the repo-internal project by Dark Form
    // https://github.com/Dark-Form/REPO-Internal

    public static class ESP
    {
        private static GUIStyle _style;

        public static void Draw(CheatBehaviour c)
        {
            var cam = Camera.main;
            if (cam == null) return;

            if (_style == null)
            {
                _style = new GUIStyle(GUI.skin.label)
                {
                    fontSize = 13,
                    fontStyle = FontStyle.Bold,
                    alignment = TextAnchor.MiddleCenter
                };
            }

            if (c.EspPlayers) DrawPlayers(c, cam);
            if (c.EspEnemies) DrawEnemies(c, cam);
            if (c.EspLoot) DrawLoot(c, cam);
            if (c.EspExtraction) DrawExtractions(c, cam);
        }

        private static void DrawPlayers(CheatBehaviour c, Camera cam)
        {
            foreach (var player in UnityEngine.Object.FindObjectsOfType<PlayerController>())
            {
                if (player.cameraGameObjectLocal != null) continue;

                Vector3 screenPos = WorldToScreen(cam, player.transform.position + Vector3.up);
                if (!IsOnScreen(screenPos)) continue;

                string name = CheatBehaviour.PlayerNameField?.GetValue(player) as string ?? "Player";
                int hp = (int)(CheatBehaviour.HealthField?.GetValue(player.playerAvatarScript?.playerHealth) ?? 0);
                if (hp <= 0) continue;

                string label = name;
                if (c.EspPlayerDist && c.LocalPlayer != null)
                    label += $" [{Vector3.Distance(c.LocalPlayer.transform.position, player.transform.position):F0}m]";
                if (c.EspPlayerHp)
                    label += $"\n{hp}HP";

                DrawLabel(screenPos, label, Color.cyan);
            }
        }

        private static void DrawEnemies(CheatBehaviour c, Camera cam)
        {
            foreach (var enemy in c.Enemies)
            {
                if (enemy == null) continue;

                EnemyParent parent = enemy.GetComponentInParent<EnemyParent>();
                EnemyHealth eHealth = parent?.GetComponentInChildren<EnemyHealth>();
                if (eHealth == null || eHealth.health <= 0) continue;

                Vector3 screenPos = WorldToScreen(cam, enemy.CenterTransform.position);
                if (!IsOnScreen(screenPos)) continue;

                string label = parent.enemyName;
                if (c.EspEnemyDist && c.LocalPlayer != null)
                    label += $" [{Vector3.Distance(c.LocalPlayer.transform.position, enemy.CenterTransform.position):F0}m]";
                if (c.EspEnemyHp)
                    label += $"\n{eHealth.health}HP";

                DrawLabel(screenPos, label, Color.red);
            }
        }

        private static void DrawLoot(CheatBehaviour c, Camera cam)
        {
            foreach (var item in c.Valuables)
            {
                if (item == null) continue;

                Vector3 screenPos = WorldToScreen(cam, item.transform.position);
                if (!IsOnScreen(screenPos)) continue;

                float price = (float)(CheatBehaviour.DollarValueField?.GetValue(item) ?? 0f);
                string label = item.name + (c.EspLootPrice ? $"\n${price:F0}" : "");

                DrawLabel(screenPos, label, Color.yellow);
            }
        }

        private static void DrawExtractions(CheatBehaviour c, Camera cam)
        {
            foreach (var ep in c.Extractions)
            {
                if (ep == null) continue;

                Vector3 screenPos = WorldToScreen(cam, ep.transform.position);
                if (!IsOnScreen(screenPos)) continue;

                Color col = ep.isLocked ? Color.red : Color.green;
                string label = ep.isLocked ? "Extraction [LOCKED]" : "Extraction";

                DrawLabel(screenPos, label, col);
            }
        }

        private static void DrawLabel(Vector3 screenPos, string text, Color color)
        {
            _style.normal.textColor = color;
            var content = new GUIContent(text);
            Vector2 size = _style.CalcSize(content);
            GUI.Label(
                new Rect(screenPos.x - size.x / 2f, screenPos.y - size.y / 2f, size.x, size.y),
                content, _style
            );
        }

        public static Vector3 WorldToScreen(Camera cam, Vector3 worldPos)
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

        public static bool IsOnScreen(Vector3 pos)
        {
            return pos.x > 0 && pos.x < Screen.width &&
                   pos.y > 0 && pos.y < Screen.height &&
                   pos.z > 0;
        }
    }
}