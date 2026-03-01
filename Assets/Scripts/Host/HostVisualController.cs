using UnityEngine;

namespace UWG
{
    /// <summary>
    /// Drives the Host's visual representation in the primary viewport.
    /// Listens to game events and updates:
    /// - Abdominal rig scale (belly expansion)
    /// - Posture (swayback curvature)
    /// - Movement speed (waddle cadence)
    /// - Particle effects (sweat, glow, undulation)
    /// - Sprite/material swaps (wardrobe failure, petitification)
    ///
    /// Designed to work with either 2D Sprite Rigging or 3D skeletal meshes.
    /// Assign the relevant transforms in the Inspector.
    /// </summary>
    public class HostVisualController : MonoBehaviour
    {
        [Header("Rig References")]
        [SerializeField] private Transform bellyBone;
        [SerializeField] private Transform spineBone;
        [SerializeField] private Transform bodyRoot;

        [Header("Belly Scaling")]
        [SerializeField] private Vector3 baseBellyScale = Vector3.one;
        [SerializeField] private AnimationCurve bellyCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 4f);

        [Header("Posture")]
        [SerializeField] private float maxSwaybackAngle = 30f;

        [Header("Movement")]
        [SerializeField] private Animator hostAnimator;
        [SerializeField] private string moveSpeedParam = "MoveSpeed";
        [SerializeField] private string waddleIntensityParam = "WaddleIntensity";

        [Header("Particle Systems")]
        [SerializeField] private ParticleSystem sweatParticles;
        [SerializeField] private ParticleSystem glowVeinParticles;
        [SerializeField] private ParticleSystem undulationParticles;

        [Header("Sprite Layers (2D Mode)")]
        [SerializeField] private SpriteRenderer bodySprite;
        [SerializeField] private SpriteRenderer clothingSprite;
        [SerializeField] private Sprite[] clothingStages; // normal, ill-fitting, ruined

        [Header("Material Properties (3D Mode — URP HostSkin shader)")]
        [SerializeField] private Renderer skinRenderer;
        [Tooltip("UWG/HostSkin_URP shader property: subsurface translucency")]
        [SerializeField] private string translucencyParam = "_Translucency";
        [Tooltip("UWG/HostSkin_URP shader property: thermal flush")]
        [SerializeField] private string flushParam = "_FlushIntensity";
        [Tooltip("UWG/HostSkin_URP shader property: sweat glisten")]
        [SerializeField] private string sweatParam = "_SweatIntensity";
        [Tooltip("UWG/HostSkin_URP shader property: vein glow")]
        [SerializeField] private string veinGlowParam = "_VeinGlowIntensity";
        [Tooltip("UWG/HostSkin_URP shader property: stretch marks")]
        [SerializeField] private string stretchParam = "_StretchIntensity";

        private GameState _state;
        private MaterialPropertyBlock _mpb;

        private void Awake()
        {
            _mpb = new MaterialPropertyBlock();
        }

        private void OnEnable()
        {
            GameEvents.OnGestationChanged += OnGestationChanged;
            GameEvents.OnSkillPurchased += _ => RefreshVisualFlags();
            GameEvents.OnTickEnd += RefreshVisualFlags;
        }

        private void OnDisable()
        {
            GameEvents.OnGestationChanged -= OnGestationChanged;
            GameEvents.OnTickEnd -= RefreshVisualFlags;
        }

        private void OnGestationChanged(float gestation)
        {
            _state = GameManager.Instance?.State;
            if (_state == null) return;

            UpdateBellyScale(gestation);
            UpdatePosture(gestation);
            UpdateMovement(gestation);
        }

        private void UpdateBellyScale(float gestation)
        {
            if (bellyBone == null) return;

            float cap = _state.GestationCap;
            float t = Mathf.Clamp01(gestation / cap);
            float scale = bellyCurve.Evaluate(t);

            // Get class-specific phase scale multiplier
            float phaseMult = GetPhaseScaleMultiplier();
            scale *= phaseMult;

            bellyBone.localScale = baseBellyScale * scale;
        }

        private void UpdatePosture(float gestation)
        {
            if (spineBone == null) return;

            float cap = _state.GestationCap;
            float t = Mathf.Clamp01(gestation / cap);

            // Get phase-specific swayback
            float swayback = 0f;
            var cls = _state.SelectedClass;
            var phase = _state.GetCurrentPhase();
            var descriptor = phase switch
            {
                GestationPhase.Early => cls.earlyPhase,
                GestationPhase.Mid => cls.midPhase,
                GestationPhase.Late => cls.latePhase,
                GestationPhase.Terminal => cls.terminalPhase,
                _ => null
            };
            if (descriptor != null)
                swayback = descriptor.postureSwayback;

            float angle = maxSwaybackAngle * swayback * t;
            spineBone.localRotation = Quaternion.Euler(-angle, 0f, 0f);
        }

        private void UpdateMovement(float gestation)
        {
            if (hostAnimator == null) return;

            float cap = _state.GestationCap;
            float t = Mathf.Clamp01(gestation / cap);

            float moveSpeed = Mathf.Lerp(1f, 0.2f, t) * _state.Mobility;
            float waddle = Mathf.Lerp(0f, 1f, t);

            hostAnimator.SetFloat(moveSpeedParam, moveSpeed);
            hostAnimator.SetFloat(waddleIntensityParam, waddle);
        }

        private void RefreshVisualFlags()
        {
            _state = GameManager.Instance?.State;
            if (_state == null) return;

            var flags = _state.ActiveVisualFlags;

            // Particle systems
            ToggleParticles(sweatParticles, flags.Contains(SkillEffectType.VisualSweat));
            ToggleParticles(glowVeinParticles, flags.Contains(SkillEffectType.VisualGlowingVeins));
            ToggleParticles(undulationParticles, flags.Contains(SkillEffectType.VisualAbdominalUndulation));

            // Clothing stages (2D)
            if (clothingSprite != null && clothingStages != null && clothingStages.Length >= 3)
            {
                if (flags.Contains(SkillEffectType.VisualWardrobeFailure))
                    clothingSprite.sprite = clothingStages[2]; // ruined
                else if (_state.Gestation > 40f)
                    clothingSprite.sprite = clothingStages[1]; // ill-fitting
                else
                    clothingSprite.sprite = clothingStages[0]; // normal
            }

            // Skin material properties (3D — URP HostSkin_URP shader)
            if (skinRenderer != null)
            {
                skinRenderer.GetPropertyBlock(_mpb);

                // Translucency — taut, balloon-like skin at high gestation
                float translucency = flags.Contains(SkillEffectType.VisualSkinTranslucent) ? 0.8f : 0f;
                _mpb.SetFloat(translucencyParam, translucency);

                // Thermal flush — deep red blush
                float flush = flags.Contains(SkillEffectType.VisualSweat) ? 0.6f : 0f;
                _mpb.SetFloat(flushParam, flush);

                // Sweat glisten — boosts specular via URP shader
                float sweat = flags.Contains(SkillEffectType.VisualSweat) ? 0.8f : 0f;
                _mpb.SetFloat(sweatParam, sweat);

                // Vein glow — emissive overlay driven by skill
                float veinGlow = flags.Contains(SkillEffectType.VisualGlowingVeins) ? 1.2f : 0f;
                _mpb.SetFloat(veinGlowParam, veinGlow);

                // Stretch marks — progressive with gestation
                float stretch = Mathf.Clamp01(_state.Gestation / _state.GestationCap);
                if (flags.Contains(SkillEffectType.VisualSkinTranslucent))
                    stretch *= 1.5f; // more visible on taut skin
                _mpb.SetFloat(stretchParam, Mathf.Clamp01(stretch));

                skinRenderer.SetPropertyBlock(_mpb);
            }

            // Petitification: scale down body root while belly stays large
            if (bodyRoot != null)
            {
                if (flags.Contains(SkillEffectType.VisualPetitification))
                {
                    float shrink = Mathf.Lerp(1f, 0.7f,
                        _state.Gestation / _state.GestationCap);
                    bodyRoot.localScale = new Vector3(shrink, shrink, shrink);
                }
                else
                {
                    bodyRoot.localScale = Vector3.one;
                }
            }
        }

        private float GetPhaseScaleMultiplier()
        {
            var cls = _state.SelectedClass;
            var phase = _state.GetCurrentPhase();
            var descriptor = phase switch
            {
                GestationPhase.Early => cls.earlyPhase,
                GestationPhase.Mid => cls.midPhase,
                GestationPhase.Late => cls.latePhase,
                GestationPhase.Terminal => cls.terminalPhase,
                _ => null
            };
            return descriptor?.bellyScaleMult ?? 1f;
        }

        private void ToggleParticles(ParticleSystem ps, bool active)
        {
            if (ps == null) return;
            if (active && !ps.isPlaying) ps.Play();
            else if (!active && ps.isPlaying) ps.Stop();
        }
    }
}
