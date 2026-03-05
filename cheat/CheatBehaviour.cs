using System.Collections;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace cheat
{
    public class CheatBehaviour : MonoBehaviour
    {
        // feature toggles
        public bool GodMode = false;
        public bool SpeedHack = false;
        public float SpeedMultiplier = 1f;
        public bool NoRagdoll = false;
        public bool NoBreak = false;
        public bool InfiniteStamina = false;

        // esp toggles
        public bool EspPlayers = false;
        public bool EspEnemies = false;
        public bool EspLoot = false;
        public bool EspExtraction = false;

        public bool EspPlayerDist = true;
        public bool EspPlayerHp = true;
        public bool EspEnemyDist = true;
        public bool EspEnemyHp = true;
        public bool EspLootPrice = true;

        // menu state
        public bool MenuOpen = false;
        public bool ShowUpgrades = false;
        public bool ShowTrolls = false;
        public int UpgradeValue = 10;

        // cached scene objects
        public ValuableObject[] Valuables = new ValuableObject[0];
        public ExtractionPoint[] Extractions = new ExtractionPoint[0];
        public Enemy[] Enemies = new Enemy[0];
        public PlayerController LocalPlayer;

        // reflection fields
        public static readonly FieldInfo HealthField =
            typeof(PlayerHealth).GetField("health", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

        public static readonly FieldInfo SteamIDField =
            typeof(PlayerAvatar).GetField("steamID", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

        public static readonly FieldInfo IsValuableField =
            typeof(PhysGrabObject).GetField("isValuable", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

        public static readonly FieldInfo PlayerNameField =
            typeof(PlayerController).GetField("playerName", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

        public static readonly FieldInfo DollarValueField =
            typeof(ValuableObject).GetField("dollarValueCurrent", BindingFlags.Instance | BindingFlags.NonPublic);

        // ----------------------------------------------------------------------------------------------------------------------

        void Start()
        {
            StartCoroutine(RefreshObjects());
        }

        private IEnumerator RefreshObjects()
        {
            while (true)
            {
                Valuables = UnityEngine.Object.FindObjectsOfType<ValuableObject>();
                Extractions = UnityEngine.Object.FindObjectsOfType<ExtractionPoint>();
                Enemies = UnityEngine.Object.FindObjectsOfType<Enemy>();

                var all = UnityEngine.Object.FindObjectsOfType<PlayerController>();
                LocalPlayer = all.FirstOrDefault(p => p.cameraGameObjectLocal != null);

                yield return new WaitForSeconds(10f);
            }
        }

        void Update()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.Insert))
                MenuOpen = !MenuOpen;

            var pc = PlayerController.instance;
            var ph = PlayerAvatar.instance?.playerHealth;

            if (GodMode && ph != null)
            {
                ph.InvincibleSet(9999f);
                HealthField?.SetValue(ph, 100);
            }

            if (pc != null)
            {
                pc.OverrideSpeed(SpeedHack ? SpeedMultiplier : 1f, SpeedHack ? 0.5f : 0.1f);
                pc.DebugNoTumble = NoRagdoll;
                pc.DebugEnergy = InfiniteStamina;
                if (InfiniteStamina)
                    pc.EnergyCurrent = pc.EnergyStart;
            }

            if (NoBreak)
            {
                foreach (var obj in UnityEngine.Object.FindObjectsOfType<PhysGrabObject>())
                {
                    if ((bool)(IsValuableField?.GetValue(obj) ?? false))
                        obj.OverrideIndestructible(0.5f);
                }
            }
        }

        void OnGUI()
        {
            ESP.Draw(this);

            if (!MenuOpen) return;

            if (ShowUpgrades) Menus.DrawUpgrades(this);
            else if (ShowTrolls) Menus.DrawTrolls(this);
            else Menus.DrawMain(this);
        }
    }
}