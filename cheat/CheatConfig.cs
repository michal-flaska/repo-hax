using System;
using System.IO;
using UnityEngine;

namespace cheat
{
    [Serializable]
    public class CheatConfig
    {
        // combat
        public bool GodMode = false;
        public bool SpeedHack = false;
        public float SpeedMultiplier = 1f;
        public bool NoRagdoll = false;
        public bool NoBreak = false;
        public bool InfiniteStamina = false;
        public bool RainbowColor = false;
        public float RainbowSpeed = 0.5f;

        public bool Spinbot = false;
        public float SpinSpeed = 720f;

        // esp
        public bool EspPlayers = false;
        public bool EspEnemies = false;
        public bool EspLoot = false;
        public bool EspExtraction = false;
        public bool EspBoxes = false;
        public bool EspSnaplines = false;
        public bool EspPlayerDist = true;
        public bool EspPlayerHp = true;
        public bool EspEnemyDist = true;
        public bool EspEnemyHp = true;
        public bool EspLootPrice = true;
        public float MaxDistance = 50f;
        public bool DistanceFilterEnemies = true;
        public bool DistanceFilterLoot = true;
        public bool DistanceFilterPlayers = false;
        public bool HighlightBestLoot = false;
        public bool EnemyNearbyWarning = false;
        public float MinLootValue = 0f;
        public bool FilterLootByValue = false;

        // visual
        public bool BrightMode = false;
        public bool NoChromaticAberration = false;
        public bool NoBloom = false;
        public bool NoLensDistortion = false;
        public bool FlashlightCustomColor = false;
        public float FlashlightColorR = 1f;
        public float FlashlightColorG = 1f;
        public float FlashlightColorB = 1f;
        public float FlashlightIntensity = 3f;

        // misc
        public bool Noclip = false;
        public float NoclipSpeed = 10f;

        // keybinds
        public KeyCode ToggleMenuKey = KeyCode.Insert;

        // file path: next to the DLL in Managed/
        private static string ConfigPath =>
            Path.Combine(Path.GetDirectoryName(
                System.Reflection.Assembly.GetExecutingAssembly().Location),
                "repo-hax.json");

        public static CheatConfig Load()
        {
            try
            {
                if (File.Exists(ConfigPath))
                    return JsonUtility.FromJson<CheatConfig>(File.ReadAllText(ConfigPath));
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[repo-hax] failed to load config: {e.Message}");
            }
            return new CheatConfig();
        }

        public void Save()
        {
            try
            {
                File.WriteAllText(ConfigPath, JsonUtility.ToJson(this, true));
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[repo-hax] failed to save config: {e.Message}");
            }
        }

        // apply config onto CheatBehaviour
        public void ApplyTo(CheatBehaviour c)
        {
            c.GodMode = GodMode;
            c.SpeedHack = SpeedHack;
            c.SpeedMultiplier = SpeedMultiplier;
            c.NoRagdoll = NoRagdoll;
            c.NoBreak = NoBreak;
            c.InfiniteStamina = InfiniteStamina;
            c.RainbowColor = RainbowColor;
            c.RainbowSpeed = RainbowSpeed;

            c.Spinbot = Spinbot;
            c.SpinSpeed = SpinSpeed;

            c.EspPlayers = EspPlayers;
            c.EspEnemies = EspEnemies;
            c.EspLoot = EspLoot;
            c.EspExtraction = EspExtraction;
            c.EspBoxes = EspBoxes;
            c.EspSnaplines = EspSnaplines;
            c.EspPlayerDist = EspPlayerDist;
            c.EspPlayerHp = EspPlayerHp;
            c.EspEnemyDist = EspEnemyDist;
            c.EspEnemyHp = EspEnemyHp;
            c.EspLootPrice = EspLootPrice;
            c.MaxDistance = MaxDistance;
            c.DistanceFilterEnemies = DistanceFilterEnemies;
            c.DistanceFilterLoot = DistanceFilterLoot;
            c.DistanceFilterPlayers = DistanceFilterPlayers;
            c.HighlightBestLoot = HighlightBestLoot;
            c.EnemyNearbyWarning = EnemyNearbyWarning;
            c.MinLootValue = MinLootValue;
            c.FilterLootByValue = FilterLootByValue;

            c.BrightMode = BrightMode;
            c.NoChromaticAberration = NoChromaticAberration;
            c.NoBloom = NoBloom;
            c.NoLensDistortion = NoLensDistortion;
            c.FlashlightCustomColor = FlashlightCustomColor;
            c.FlashlightColor = new Color(FlashlightColorR, FlashlightColorG, FlashlightColorB);
            c.FlashlightIntensity = FlashlightIntensity;

            c.Noclip = Noclip;
            c.NoclipSpeed = NoclipSpeed;

            c.ToggleMenuKey = ToggleMenuKey;
        }

        // snapshot current state from CheatBehaviour
        public static CheatConfig From(CheatBehaviour c)
        {
            return new CheatConfig
            {
                GodMode = c.GodMode,
                SpeedHack = c.SpeedHack,
                SpeedMultiplier = c.SpeedMultiplier,
                NoRagdoll = c.NoRagdoll,
                NoBreak = c.NoBreak,
                InfiniteStamina = c.InfiniteStamina,
                RainbowColor = c.RainbowColor,
                RainbowSpeed = c.RainbowSpeed,

                Spinbot = c.Spinbot,
                SpinSpeed = c.SpinSpeed,

                EspPlayers = c.EspPlayers,
                EspEnemies = c.EspEnemies,
                EspLoot = c.EspLoot,
                EspExtraction = c.EspExtraction,
                EspBoxes = c.EspBoxes,
                EspSnaplines = c.EspSnaplines,
                EspPlayerDist = c.EspPlayerDist,
                EspPlayerHp = c.EspPlayerHp,
                EspEnemyDist = c.EspEnemyDist,
                EspEnemyHp = c.EspEnemyHp,
                EspLootPrice = c.EspLootPrice,
                MaxDistance = c.MaxDistance,
                DistanceFilterEnemies = c.DistanceFilterEnemies,
                DistanceFilterLoot = c.DistanceFilterLoot,
                DistanceFilterPlayers = c.DistanceFilterPlayers,
                HighlightBestLoot = c.HighlightBestLoot,
                EnemyNearbyWarning = c.EnemyNearbyWarning,
                MinLootValue = c.MinLootValue,
                FilterLootByValue = c.FilterLootByValue,

                BrightMode = c.BrightMode,
                NoChromaticAberration = c.NoChromaticAberration,
                NoBloom = c.NoBloom,
                NoLensDistortion = c.NoLensDistortion,
                FlashlightCustomColor = c.FlashlightCustomColor,
                FlashlightColorR = c.FlashlightColor.r,
                FlashlightColorG = c.FlashlightColor.g,
                FlashlightColorB = c.FlashlightColor.b,
                FlashlightIntensity = c.FlashlightIntensity,

                Noclip = c.Noclip,
                NoclipSpeed = c.NoclipSpeed,

                ToggleMenuKey = c.ToggleMenuKey,
            };
        }
    }
}