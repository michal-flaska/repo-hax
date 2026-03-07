using UnityEngine;

namespace cheat
{
    public static class Theme
    {
        public static readonly Color Accent = new Color(1f, 0f, 0.467f, 1f);
        public static readonly Color AccentDim = new Color(0.6f, 0f, 0.28f, 1f);
        public static readonly Color BgColor = new Color(0.07f, 0.07f, 0.07f, 0.96f);
        public static readonly Color TextColor = Color.white;
        public static readonly Color DimText = new Color(0.65f, 0.65f, 0.65f, 1f);

        public static GUIStyle Box;
        public static GUIStyle Button;
        public static GUIStyle Toggle;
        public static GUIStyle Label;
        public static GUIStyle HeaderLabel;
        public static GUIStyle TabActive;
        public static GUIStyle TabInactive;

        private static bool _initialized;

        public static void Init()
        {
            if (_initialized) return;
            _initialized = true;

            Box = new GUIStyle(GUI.skin.box);
            Box.normal.background = MakeTex(BgColor);
            Box.normal.textColor = Accent;
            Box.fontSize = 13;
            Box.fontStyle = FontStyle.Bold;
            Box.alignment = TextAnchor.UpperCenter;
            Box.border = new RectOffset(4, 4, 4, 4);
            Box.padding = new RectOffset(6, 6, 6, 6);

            Button = new GUIStyle(GUI.skin.button);
            Button.normal.background = MakeTex(new Color(0.13f, 0.13f, 0.13f, 1f));
            Button.hover.background = MakeTex(new Color(0.2f, 0f, 0.1f, 1f));
            Button.active.background = MakeTex(AccentDim);
            Button.normal.textColor = TextColor;
            Button.hover.textColor = Accent;
            Button.active.textColor = TextColor;
            Button.fontSize = 12;
            Button.fontStyle = FontStyle.Bold;
            Button.alignment = TextAnchor.MiddleCenter;
            Button.border = new RectOffset(4, 4, 4, 4);

            Toggle = new GUIStyle(GUI.skin.toggle);
            Toggle.normal.textColor = TextColor;
            Toggle.hover.textColor = Accent;
            Toggle.active.textColor = Accent;
            Toggle.onNormal.textColor = Accent;
            Toggle.onHover.textColor = Accent;
            Toggle.fontSize = 12;

            Label = new GUIStyle(GUI.skin.label);
            Label.normal.textColor = TextColor;
            Label.fontSize = 12;

            HeaderLabel = new GUIStyle(GUI.skin.label);
            HeaderLabel.normal.textColor = AccentDim;
            HeaderLabel.fontSize = 11;
            HeaderLabel.fontStyle = FontStyle.Bold;
            HeaderLabel.alignment = TextAnchor.MiddleCenter;

            // active tab: accent background
            TabActive = new GUIStyle(GUI.skin.button);
            TabActive.normal.background = MakeTex(AccentDim);
            TabActive.hover.background = MakeTex(AccentDim);
            TabActive.normal.textColor = Color.white;
            TabActive.hover.textColor = Color.white;
            TabActive.fontSize = 11;
            TabActive.fontStyle = FontStyle.Bold;
            TabActive.alignment = TextAnchor.MiddleCenter;

            // inactive tab: dim background
            TabInactive = new GUIStyle(GUI.skin.button);
            TabInactive.normal.background = MakeTex(new Color(0.13f, 0.13f, 0.13f, 1f));
            TabInactive.hover.background = MakeTex(new Color(0.2f, 0f, 0.1f, 1f));
            TabInactive.normal.textColor = DimText;
            TabInactive.hover.textColor = Accent;
            TabInactive.fontSize = 11;
            TabInactive.fontStyle = FontStyle.Bold;
            TabInactive.alignment = TextAnchor.MiddleCenter;
        }

        private static Texture2D MakeTex(Color col)
        {
            var tex = new Texture2D(1, 1);
            tex.SetPixel(0, 0, col);
            tex.Apply();
            return tex;
        }
    }
}