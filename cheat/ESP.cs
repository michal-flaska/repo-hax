using UnityEngine;

namespace cheat
{
    // ESP is based on the repo-internal project by Dark-Form
    // https://github.com/Dark-Form/REPO-Internal

    public static class ESP
    {
        private static GUIStyle _style;
        private static GUIStyle _overlayStyle;
        private static Texture2D _lineTex;

        private const float HighlightThreshold = 5000f;
        private const float BoxHalfWidth = 30f;
        private const float BoxHalfHeight = 50f;
        private const float LineThickness = 1.5f;

        private static readonly Color _hotColor = new Color(1f, 0f, 0.47f); // #ff0077

        // public draw entry point

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

            if (_overlayStyle == null)
            {
                _overlayStyle = new GUIStyle(GUI.skin.label)
                {
                    fontSize = 15,
                    fontStyle = FontStyle.Bold,
                    alignment = TextAnchor.UpperLeft
                };
                _overlayStyle.normal.textColor = Color.white;
            }

            if (c.EspPlayers) DrawPlayers(c, cam);
            if (c.EspEnemies) DrawEnemies(c, cam);
            if (c.EspLoot) DrawLoot(c, cam);
            if (c.EspExtraction) DrawExtractions(c, cam);
            if (c.EnemyNearbyWarning) DrawEnemyWarning(c);

            // debug labels
            //  GUI.Label(new Rect(10, 40, 300, 20), $"Players cached: {c.Players.Length}");
            //  GUI.Label(new Rect(10, 60, 300, 20), $"Avatars cached: {c.Avatars.Length}");
            //  GUI.Label(new Rect(10, 80, 300, 20), $"Avatars: {UnityEngine.Object.FindObjectsOfType<PlayerAvatar>().Length}");

            DrawLootOverlay(c);
        }

        // per category drawers

        private static void DrawPlayers(CheatBehaviour c, Camera cam)
        {
            foreach (var avatar in c.Avatars)
            {
                if (avatar == null) continue;

                bool isLocal = (bool)(CheatBehaviour.AvatarIsLocalField?.GetValue(avatar) ?? false);
                if (isLocal) continue;

                float dist = c.LocalPlayer != null
                    ? Vector3.Distance(c.LocalPlayer.transform.position, avatar.transform.position)
                    : 0f;

                if (c.DistanceFilterPlayers && dist > c.MaxDistance) continue;

                Vector3 screenPos = WorldToScreen(cam, avatar.transform.position + Vector3.up);
                if (!IsOnScreen(screenPos)) continue;

                string name = CheatBehaviour.AvatarNameField?.GetValue(avatar) as string ?? "Player";
                int hp = (int)(CheatBehaviour.HealthField?.GetValue(avatar.playerHealth) ?? 0);
                if (hp <= 0) continue;

                string label = name;
                if (c.EspPlayerDist) label += $" [{dist:F0}m]";
                if (c.EspPlayerHp) label += $"\n{hp}HP";

                if (c.EspBoxes) DrawBox(screenPos, Color.cyan);
                if (c.EspSnaplines) DrawSnapline(screenPos, Color.cyan);
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
                if (eHealth == null) continue;

                int enemyHp = (int)(CheatBehaviour.EnemyHealthCurrentField?.GetValue(eHealth) ?? eHealth.health);
                if (enemyHp <= 0) continue;

                float dist = c.LocalPlayer != null
                    ? Vector3.Distance(c.LocalPlayer.transform.position, enemy.CenterTransform.position)
                    : 0f;

                if (c.DistanceFilterEnemies && dist > c.MaxDistance) continue;

                Vector3 screenPos = WorldToScreen(cam, enemy.CenterTransform.position);
                if (!IsOnScreen(screenPos)) continue;

                string label = parent.enemyName;
                if (c.EspEnemyDist) label += $" [{dist:F0}m]";
                if (c.EspEnemyHp) label += $"\n{enemyHp}HP";

                if (c.EspBoxes) DrawBox(screenPos, Color.red);
                if (c.EspSnaplines) DrawSnapline(screenPos, Color.red);
                DrawLabel(screenPos, label, Color.red);
            }
        }

        private static void DrawLoot(CheatBehaviour c, Camera cam)
        {
            foreach (var item in c.Valuables)
            {
                if (item == null) continue;

                float dist = c.LocalPlayer != null
                    ? Vector3.Distance(c.LocalPlayer.transform.position, item.transform.position)
                    : 0f;

                if (c.DistanceFilterLoot && dist > c.MaxDistance) continue;

                float price = (float)(CheatBehaviour.DollarValueField?.GetValue(item) ?? 0f);
                if (c.FilterLootByValue && price < c.MinLootValue) continue;

                Vector3 screenPos = WorldToScreen(cam, item.transform.position);
                if (!IsOnScreen(screenPos)) continue;

                Color col = (c.HighlightBestLoot && price >= HighlightThreshold) ? _hotColor : Color.yellow;
                string label = CleanName(item.name) + (c.EspLootPrice ? $"\n${price:F0}" : "");

                if (c.EspBoxes) DrawBox(screenPos, col);
                if (c.EspSnaplines) DrawSnapline(screenPos, col);
                DrawLabel(screenPos, label, col);
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
                string label = ep.isLocked ? "Extraction [LOCKED]" : "Extraction [Open]";

                if (c.EspBoxes) DrawBox(screenPos, col);
                if (c.EspSnaplines) DrawSnapline(screenPos, col);
                DrawLabel(screenPos, label, col);
            }
        }

        private static void DrawEnemyWarning(CheatBehaviour c)
        {
            if (c.LocalPlayer == null) return;

            foreach (var enemy in c.Enemies)
            {
                if (enemy == null) continue;

                EnemyHealth eHealth = enemy.GetComponentInParent<EnemyParent>()?.GetComponentInChildren<EnemyHealth>();
                if (eHealth == null) continue;

                int hp = (int)(CheatBehaviour.EnemyHealthCurrentField?.GetValue(eHealth) ?? eHealth.health);
                if (hp <= 0) continue;

                float dist = Vector3.Distance(c.LocalPlayer.transform.position, enemy.CenterTransform.position);
                if (dist > 10f) continue;

                string text = "! ENEMY NEARBY !";
                Vector2 size = _overlayStyle.CalcSize(new GUIContent(text));
                float x = (Screen.width - size.x) / 2f;
                float y = (Screen.height / 2f) - size.y - 40f;

                Color prev = GUI.color;
                GUI.color = new Color(1f, 0.15f, 0.15f);
                GUI.Label(new Rect(x, y, size.x, size.y), text, _overlayStyle);
                GUI.color = prev;
                return;
            }
        }

        private static void DrawLootOverlay(CheatBehaviour c)
        {
            float total = 0f;
            int count = 0;

            foreach (var item in c.Valuables)
            {
                if (item == null) continue;
                float price = (float)(CheatBehaviour.DollarValueField?.GetValue(item) ?? 0f);
                total += price;
                count++;
            }

            string text = $"Loot: ${total:F0}  ({count} items)";
            Vector2 size = _overlayStyle.CalcSize(new GUIContent(text));
            GUI.Label(new Rect(Screen.width - size.x - 10f, 10f, size.x, size.y), text, _overlayStyle);
        }

        // primitives

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

        // draws a fixed size box centered on screenPos
        private static void DrawBox(Vector3 screenPos, Color color)
        {
            float x = screenPos.x - BoxHalfWidth;
            float y = screenPos.y - BoxHalfHeight;
            DrawBox(x, y, BoxHalfWidth * 2f, BoxHalfHeight * 2f, color, LineThickness);
        }

        // draws a line from bottom-center of the screen to the entity
        private static void DrawSnapline(Vector3 screenPos, Color color)
        {
            var origin = new Vector2(Screen.width / 2f, Screen.height);
            DrawLine(origin, new Vector2(screenPos.x, screenPos.y), color, LineThickness);
        }

        public static void DrawLine(Vector2 a, Vector2 b, Color color, float width)
        {
            if (_lineTex == null)
            {
                _lineTex = new Texture2D(1, 1);
                _lineTex.SetPixel(0, 0, Color.white);
                _lineTex.Apply();
            }

            Matrix4x4 savedMatrix = GUI.matrix;
            Color savedColor = GUI.color;

            GUI.color = color;

            float angle = Vector2.Angle(b - a, Vector2.right);
            if (a.y > b.y) angle = -angle;

            GUIUtility.ScaleAroundPivot(new Vector2((b - a).magnitude, width), new Vector2(a.x, a.y + 0.5f));
            GUIUtility.RotateAroundPivot(angle, a);
            GUI.DrawTexture(new Rect(a.x, a.y, 1f, 1f), _lineTex);

            GUI.matrix = savedMatrix;
            GUI.color = savedColor;
        }

        public static void DrawBox(float x, float y, float w, float h, Color color, float thickness)
        {
            DrawLine(new Vector2(x, y), new Vector2(x + w, y), color, thickness);
            DrawLine(new Vector2(x, y), new Vector2(x, y + h), color, thickness);
            DrawLine(new Vector2(x + w, y), new Vector2(x + w, y + h), color, thickness);
            DrawLine(new Vector2(x, y + h), new Vector2(x + w, y + h), color, thickness);
        }

        // helpers

        private static string CleanName(string raw)
        {
            if (raw.StartsWith("Valuable "))
                raw = raw.Substring("Valuable ".Length);

            int cloneIdx = raw.IndexOf("(Clone)");
            if (cloneIdx >= 0)
                raw = raw.Substring(0, cloneIdx).TrimEnd();

            return raw;
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