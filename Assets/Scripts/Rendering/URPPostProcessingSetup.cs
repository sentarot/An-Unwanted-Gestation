using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace UWG.Rendering
{
    /// <summary>
    /// Creates and configures the URP Volume Profile at runtime for
    /// the game's clinical "parasite dashboard" aesthetic.
    /// Attach to the main camera or a global Volume object.
    ///
    /// Alternatively, create a Volume Profile manually in the editor with:
    /// - Bloom (threshold 0.9, intensity 1.2, scatter 0.7)
    /// - Vignette (controlled by HostEffectsRendererFeature at runtime)
    /// - Color Adjustments (saturation -10 for the clinical feel)
    /// - Chromatic Aberration (controlled at runtime)
    /// - Film Grain (intensity 0.15 for surveillance-cam texture)
    /// </summary>
    [RequireComponent(typeof(Volume))]
    public class URPPostProcessingSetup : MonoBehaviour
    {
        [SerializeField] private bool createProfileAtRuntime = true;

        private Volume _volume;

        private void Awake()
        {
            _volume = GetComponent<Volume>();

            if (createProfileAtRuntime && _volume.profile == null)
            {
                CreateDashboardProfile();
            }
        }

        private void CreateDashboardProfile()
        {
            var profile = ScriptableObject.CreateInstance<VolumeProfile>();

            // Bloom — subtle glow for skin translucency and vein effects
            var bloom = profile.Add<Bloom>();
            bloom.threshold.Override(0.9f);
            bloom.intensity.Override(1.2f);
            bloom.scatter.Override(0.7f);

            // Vignette — base level, intensified by HostEffectsRendererFeature
            var vignette = profile.Add<Vignette>();
            vignette.intensity.Override(0.2f);
            vignette.smoothness.Override(0.4f);
            vignette.color.Override(new Color(0.1f, 0f, 0f));

            // Color Adjustments — desaturated clinical look
            var colorAdj = profile.Add<ColorAdjustments>();
            colorAdj.saturation.Override(-10f);
            colorAdj.contrast.Override(10f);

            // Chromatic Aberration — driven dynamically by humiliation
            var chromatic = profile.Add<ChromaticAberration>();
            chromatic.intensity.Override(0f);

            // Film Grain — surveillance camera texture
            var grain = profile.Add<FilmGrain>();
            grain.intensity.Override(0.15f);
            grain.type.Override(FilmGrainLookup.Thin1);

            // Lens Distortion — driven dynamically by undulation events
            var distortion = profile.Add<LensDistortion>();
            distortion.intensity.Override(0f);

            _volume.profile = profile;
            _volume.isGlobal = true;
        }
    }
}
