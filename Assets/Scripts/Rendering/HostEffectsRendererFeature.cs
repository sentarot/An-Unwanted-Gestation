using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.RenderGraphModule;

namespace UWG.Rendering
{
    /// <summary>
    /// URP Renderer Feature that adds host-condition-driven post-processing:
    /// - Vignette pulse synced to discomfort
    /// - Chromatic aberration during high humiliation
    /// - Screen distortion during abdominal undulation events
    ///
    /// Uses the Unity 6 RenderGraph API (RecordRenderGraph) instead of the
    /// obsolete Execute(ScriptableRenderContext, ref RenderingData).
    ///
    /// SETUP: Add this feature to your Universal Renderer Data asset:
    ///   Universal Renderer Data > Add Renderer Feature > Host Effects
    /// </summary>
    public class HostEffectsRendererFeature : ScriptableRendererFeature
    {
        [System.Serializable]
        public class Settings
        {
            [Header("Vignette Pulse (Discomfort)")]
            public bool enableVignettePulse = true;
            [Range(0f, 1f)] public float maxVignetteIntensity = 0.45f;
            public Color vignetteColor = new Color(0.4f, 0f, 0f, 1f);
            public float pulseSpeed = 2f;

            [Header("Chromatic Aberration (Humiliation)")]
            public bool enableChromaticAberration = true;
            [Range(0f, 1f)] public float maxAberration = 0.3f;

            [Header("Screen Distortion (Undulation Events)")]
            public bool enableDistortion = true;
            [Range(0f, 0.05f)] public float maxDistortion = 0.02f;
            public float distortionFrequency = 3f;
        }

        public Settings settings = new Settings();

        private HostEffectsRenderPass _pass;

        public override void Create()
        {
            _pass = new HostEffectsRenderPass(settings)
            {
                renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing
            };
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (renderingData.cameraData.isSceneViewCamera) return;
            renderer.EnqueuePass(_pass);
        }
    }

    public class HostEffectsRenderPass : ScriptableRenderPass
    {
        private readonly HostEffectsRendererFeature.Settings _settings;

        public HostEffectsRenderPass(HostEffectsRendererFeature.Settings settings)
        {
            _settings = settings;
        }

        /// <summary>
        /// Unity 6 RenderGraph API entry point. Replaces the obsolete
        /// Execute(ScriptableRenderContext, ref RenderingData).
        /// Volume overrides are driven from game state each frame.
        /// </summary>
        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            var state = GameManager.Instance?.State;
            if (state == null) return;

            // Volume overrides can be driven outside of render graph passes
            // since they modify shared state, not GPU resources.
            ApplyVolumeOverrides(state);
        }

        private void ApplyVolumeOverrides(GameState state)
        {
            var volumeStack = VolumeManager.instance.stack;

            // --- Vignette driven by Discomfort ---
            if (_settings.enableVignettePulse)
            {
                var vignette = volumeStack.GetComponent<Vignette>();
                if (vignette != null)
                {
                    float discomfortNorm = state.Discomfort / 100f;
                    float pulse = 1f + Mathf.Sin(Time.time * _settings.pulseSpeed) * 0.2f;
                    float intensity = discomfortNorm * _settings.maxVignetteIntensity * pulse;

                    vignette.intensity.Override(intensity);
                    vignette.color.Override(_settings.vignetteColor);
                }
            }

            // --- Chromatic Aberration driven by Humiliation ---
            if (_settings.enableChromaticAberration)
            {
                var chromatic = volumeStack.GetComponent<ChromaticAberration>();
                if (chromatic != null)
                {
                    float humNorm = state.Humiliation / 100f;
                    chromatic.intensity.Override(humNorm * _settings.maxAberration);
                }
            }

            // --- Lens Distortion during undulation ---
            if (_settings.enableDistortion &&
                state.ActiveVisualFlags.Contains(SkillEffectType.VisualAbdominalUndulation))
            {
                var distortion = volumeStack.GetComponent<LensDistortion>();
                if (distortion != null)
                {
                    float wave = Mathf.Sin(Time.time * _settings.distortionFrequency);
                    distortion.intensity.Override(wave * _settings.maxDistortion * 100f);
                }
            }
        }
    }
}
