using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace cheat
{
    public class CheatBehaviour : MonoBehaviour
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
        public bool BrightMode = false;

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

        // distance filter
        public float MaxDistance = 50f;
        public bool DistanceFilterEnemies = true;
        public bool DistanceFilterLoot = true;
        public bool DistanceFilterPlayers = false;

        // misc
        public bool HighlightBestLoot = false;
        public bool EnemyNearbyWarning = false;
        public float MinLootValue = 0f;
        public bool FilterLootByValue = false;
        public bool NoChromaticAberration = false;
        public bool NoBloom = false;
        public bool NoLensDistortion = false;
        public bool Noclip = false;
        public float NoclipSpeed = 10f;

        // flashlight
        public bool FlashlightCustomColor = false;
        public Color FlashlightColor = Color.white;
        public float FlashlightIntensity = 3f;

        // config
        public KeyCode ToggleMenuKey = KeyCode.Insert;

        // menu state
        public bool MenuOpen = false;
        public bool ShowUpgrades = false;
        public bool ShowTrolls = false;
        public bool ShowConfig = false;
        public int UpgradeValue = 10;

        // cached scene objects
        public ValuableObject[] Valuables = new ValuableObject[0];
        public ExtractionPoint[] Extractions = new ExtractionPoint[0];
        public Enemy[] Enemies = new Enemy[0];
        public PlayerController[] Players = new PlayerController[0];
        public PlayerAvatar[] Avatars = new PlayerAvatar[0];
        public PlayerController LocalPlayer;

        // rainbow state
        private float _colorTimer = 0f;
        private int _colorIndex = 0;

        // bright mode state
        private bool _brightWasOn = false;
        private Color _origAmbientLight;
        private float _origAmbientIntensity;
        private bool _origFog;
        private float _origFogDensity;
        private float _origFarClip;

        // noclip state
        private bool _noclipWasOn = false;
        private CharacterController _cc;
        private Rigidbody _rb;

        // reflection fields
        public static readonly FieldInfo HealthField =
            typeof(PlayerHealth).GetField("health", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

        public static readonly FieldInfo SteamIDField =
            typeof(PlayerAvatar).GetField("steamID", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

        public static readonly FieldInfo IsValuableField =
            typeof(PhysGrabObject).GetField("isValuable", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

        public static readonly FieldInfo PlayerNameField =
            typeof(PlayerController).GetField("playerName", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

        public static readonly FieldInfo AvatarNameField =
            typeof(PlayerAvatar).GetField("playerName", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

        public static readonly FieldInfo AvatarIsLocalField =
            typeof(PlayerAvatar).GetField("isLocal", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

        public static readonly FieldInfo DollarValueField =
            typeof(ValuableObject).GetField("dollarValueCurrent", BindingFlags.Instance | BindingFlags.NonPublic);

        public static readonly FieldInfo EnemyHealthCurrentField =
            typeof(EnemyHealth).GetField("healthCurrent", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

        private static readonly FieldInfo BaseIntensityField =
            typeof(FlashlightController).GetField("baseIntensity", BindingFlags.Instance | BindingFlags.NonPublic);

        public static readonly FieldInfo DeadSetField =
            typeof(PlayerAvatar).GetField("deadSet", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

        // my dnspy shit notes

        // PlayerHealth.health        - Token: 0x04002377, internal int, default 100
        // PlayerAvatar.isLocal       - Token: 0x040020B1, internal bool
        // PlayerAvatar.instance      - Token: 0x040020E4, public static
        // PlayerAvatar.playerHealth  - Token: 0x0400209E, public PlayerHealth
        // PlayerAvatar.steamID       - Token: 0x040020AC, internal string
        // PlayerAvatar.playerName    - Token: 0x040020AB, internal string
        // PlayerController.instance  - Token: 0x040021EE, public static
        // PlayerController.cameraGameObject - Token: 0x04002232, public GameObject (actual render camera)
        // PlayerController.DebugNoTumble - Token: 0x04002228, public bool
        //   TumbleRequest checks this - if true and _playerInput is false, ragdoll is blocked
        // PlayerController.DebugEnergy - Token: 0x0400222A, public bool
        //   if true, SprintDrainTimer never drains EnergyCurrent and slide costs 0
        // PlayerController.EnergyCurrent - Token: 0x0400222C, public float
        // PlayerController.EnergyStart   - Token: 0x0400222B, public float, default 100
        // PlayerController.OverrideSpeed(float _speedMulti, float _time) - Token: 0x06001572
        //   internally multiplies playerOriginalMoveSpeed/SprintSpeed/CrouchSpeed
        //   playerOriginalMoveSpeed   - Token: 0x04002261, private float, set in LateStart() after upgrades
        //   playerOriginalSprintSpeed - Token: 0x04002263, internal float, set in LateStart() after upgrades
        //   playerOriginalCrouchSpeed - Token: 0x04002264, private float, set in LateStart() after upgrades
        // StatsManager.instance      - Token: 0x04001CB3, public static
        // StatsManager.DictionaryUpdateValue(string dictName, string steamID, int value) - Token: 0x06001292
        // upgrade dicts (all public Dictionary<string,int>, keyed by steamID):
        //   playerUpgradeHealth     - Token: 0x04001CBF  (each point = +20 max hp)
        //   playerUpgradeStamina    - Token: 0x04001CC0
        //   playerUpgradeSpeed      - Token: 0x04001CC8  (adds directly to SprintSpeed in LateStart)
        //   playerUpgradeStrength   - Token: 0x04001CC9
        //   playerUpgradeExtraJump  - Token: 0x04001CC1
        //   playerUpgradeRange      - Token: 0x04001CCB
        //   playerUpgradeThrow      - Token: 0x04001CCA
        // PhysGrabObject.isValuable  - Token: 0x04001F0B, internal bool
        // PhysGrabObject.OverrideIndestructible(float time) - Token: 0x060013DD
        // ExtractionPoint.safetySpawn - public Transform
        //   isLocked - public bool
        // ChatManager.instance       - Token: 0x040024CC, public static
        // ChatManager.ForceSendMessage(string _message) - Token: 0x06001681, public
        //   sets chatMessage then calls ForceConfirmChat() -> StateSet(Send)
        //   only works in multiplayer - Update() returns early in singleplayer
        // chat tricks (TMP rich text parsed by the game):
        //   flashbang: <size=-111111>text   - sets text size to near-zero causing screen flash
        //   big text:  <size=999>text       - massive text on everyone's screen
        //   invisible: <alpha=#00>text      - sends invisible message
        //   rainbow:   <gradient>text       - colored gradient text

        // ----------------------------------------------------------------------------------------------------------------------

        void Start()
        {
            CheatConfig.Load().ApplyTo(this);
            StartCoroutine(RefreshObjects());
        }

        private IEnumerator RefreshObjects()
        {
            while (true)
            {
                Valuables = UnityEngine.Object.FindObjectsOfType<ValuableObject>();
                Extractions = UnityEngine.Object.FindObjectsOfType<ExtractionPoint>();
                Enemies = UnityEngine.Object.FindObjectsOfType<Enemy>();
                Players = UnityEngine.Object.FindObjectsOfType<PlayerController>();
                Avatars = UnityEngine.Object.FindObjectsOfType<PlayerAvatar>();
                LocalPlayer = Players.FirstOrDefault(p => p.cameraGameObjectLocal != null);

                yield return new WaitForSeconds(3f);
            }
        }

        private void ApplyPostProcess()
        {
            var obj = GameObject.Find("Game Director/Post Processing/Post Processing Overlay");
            if (obj == null) return;
            var volume = obj.GetComponent<PostProcessVolume>();
            if (volume?.profile == null) return;

            foreach (var setting in volume.profile.settings)
            {
                if (setting is Bloom bloom) bloom.active = !NoBloom;
                else if (setting is ChromaticAberration ca) ca.active = !NoChromaticAberration;
                else if (setting is LensDistortion ld) ld.active = !NoLensDistortion;
            }
        }

        void Update()
        {
            if (UnityEngine.Input.GetKeyDown(ToggleMenuKey))
                MenuOpen = !MenuOpen;

            Cursor.visible = MenuOpen;

            var pc = PlayerController.instance;
            var ph = PlayerAvatar.instance?.playerHealth;

            // combat

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

            // rainbow

            if (RainbowColor && PlayerAvatar.instance != null && AssetManager.instance != null)
            {
                _colorTimer += Time.deltaTime;
                if (_colorTimer >= RainbowSpeed)
                {
                    _colorTimer = 0f;
                    int count = AssetManager.instance.playerColors.Count;
                    _colorIndex = (_colorIndex + 1) % count;
                    PlayerAvatar.instance.photonView.RPC("SetColorRPC", RpcTarget.All, _colorIndex);
                }
            }

            // bright Mode

            if (BrightMode)
            {
                if (!_brightWasOn)
                {
                    _origAmbientLight = RenderSettings.ambientLight;
                    _origAmbientIntensity = RenderSettings.ambientIntensity;
                    _origFog = RenderSettings.fog;
                    _origFogDensity = RenderSettings.fogDensity;
                    _origFarClip = Camera.main?.farClipPlane ?? 1000f;
                    _brightWasOn = true;
                }

                RenderSettings.ambientLight = Color.white;
                RenderSettings.ambientIntensity = 5f;
                RenderSettings.fog = false;
                RenderSettings.fogDensity = 0f;
                if (Camera.main != null) Camera.main.farClipPlane = 2000f;
            }
            else if (_brightWasOn)
            {
                RenderSettings.ambientLight = _origAmbientLight;
                RenderSettings.ambientIntensity = _origAmbientIntensity;
                RenderSettings.fog = _origFog;
                RenderSettings.fogDensity = _origFogDensity;
                if (Camera.main != null) Camera.main.farClipPlane = _origFarClip;
                _brightWasOn = false;
            }

            // noclip

            if (Noclip)
            {
                if (!_noclipWasOn)
                {
                    _cc = pc?.GetComponent<CharacterController>();
                    _rb = pc?.GetComponent<Rigidbody>();
                    if (_cc != null) _cc.enabled = false;
                    if (_rb != null) { _rb.useGravity = false; _rb.velocity = Vector3.zero; _rb.isKinematic = true; }
                    _noclipWasOn = true;
                }

                var cam = Camera.main;
                if (cam != null && pc != null)
                {
                    float h = Input.GetAxis("Horizontal");
                    float v = Input.GetAxis("Vertical");
                    float up = Input.GetKey(KeyCode.Space) ? 1f : Input.GetKey(KeyCode.LeftControl) ? -1f : 0f;
                    pc.transform.position += (cam.transform.right * h + cam.transform.forward * v + Vector3.up * up) * NoclipSpeed * Time.deltaTime;
                }
            }
            else if (_noclipWasOn)
            {
                if (_cc != null) _cc.enabled = true;
                if (_rb != null) { _rb.useGravity = true; _rb.isKinematic = false; }
                _noclipWasOn = false;
            }

            // post process
            // reapplied every frame so the game can't silently re-enable effects

            if (NoChromaticAberration || NoBloom || NoLensDistortion)
                ApplyPostProcess();

            // flashlight

            if (FlashlightCustomColor || FlashlightIntensity != 3f)
            {
                var fc = PlayerAvatar.instance?.GetComponentInChildren<FlashlightController>();
                if (fc?.spotlight != null)
                {
                    if (FlashlightCustomColor)
                        fc.spotlight.color = FlashlightColor;
                    BaseIntensityField?.SetValue(fc, FlashlightIntensity);
                }
            }
        }

        void OnGUI()
        {
            ESP.Draw(this);

            if (!MenuOpen) return;

            if (ShowUpgrades) Menus.DrawUpgrades(this);
            else if (ShowTrolls) Menus.DrawTrolls(this);
            else if (ShowConfig) Menus.DrawConfig(this);
            else Menus.DrawMain(this);
        }
    }
}