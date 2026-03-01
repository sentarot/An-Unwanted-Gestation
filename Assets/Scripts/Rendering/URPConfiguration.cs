using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace UWG.Rendering
{
    /// <summary>
    /// Creates and validates the URP pipeline configuration at editor time
    /// or runtime. Attach to a bootstrap GameObject or call from an Editor script.
    ///
    /// SETUP INSTRUCTIONS (do this once in the Unity Editor):
    /// 1. Install "Universal RP" from Package Manager (com.unity.render-pipelines.universal)
    /// 2. Create a UniversalRenderPipelineAsset via:
    ///    Assets > Create > Rendering > URP Asset (with Universal Renderer)
    /// 3. Assign it in Edit > Project Settings > Graphics > Scriptable Render Pipeline Settings
    /// 4. Assign it in Edit > Project Settings > Quality > Render Pipeline Asset (for each level)
    /// 5. Drag the asset into this component's urpAsset field, OR let ValidateConfiguration()
    ///    find and report what's missing.
    /// </summary>
    public class URPConfiguration : MonoBehaviour
    {
        [Header("URP Pipeline Asset")]
        [Tooltip("Assign your UniversalRenderPipelineAsset here. " +
                 "Create one via Assets > Create > Rendering > URP Asset (with Universal Renderer).")]
        [SerializeField] private UniversalRenderPipelineAsset urpAsset;

        [Header("Rendering Settings")]
        [SerializeField] private bool enableHDR = true;
        [SerializeField] private bool enableSRPBatcher = true;
        [SerializeField] private MsaaQuality msaaQuality = MsaaQuality._2x;

        [Header("Shadow Settings")]
        [SerializeField] private float shadowDistance = 30f;

        [Header("Post-Processing")]
        [Tooltip("Enable Bloom, Vignette, and Color Grading for the clinical dashboard aesthetic")]
        [SerializeField] private bool enablePostProcessing = true;

        private void Awake()
        {
            ValidateConfiguration();
        }

        public void ValidateConfiguration()
        {
            // Check that a URP asset is active
            var currentPipeline = UnityEngine.Rendering.GraphicsSettings.currentRenderPipeline;
            if (currentPipeline == null)
            {
                Debug.LogError(
                    "[UWG/URP] No Scriptable Render Pipeline asset assigned! " +
                    "Go to Edit > Project Settings > Graphics and assign a " +
                    "UniversalRenderPipelineAsset. The game requires URP.");
                return;
            }

            if (currentPipeline is not UniversalRenderPipelineAsset activeURP)
            {
                Debug.LogError(
                    $"[UWG/URP] Active pipeline is '{currentPipeline.GetType().Name}', " +
                    "but UWG requires UniversalRenderPipelineAsset (URP). " +
                    "Install the Universal RP package and reassign.");
                return;
            }

            // Apply runtime overrides if we have a reference
            if (urpAsset != null)
            {
                ConfigureAsset(urpAsset);
            }
            else
            {
                ConfigureAsset(activeURP);
            }

            Debug.Log("[UWG/URP] Pipeline validated. URP is active and configured.");
        }

        private void ConfigureAsset(UniversalRenderPipelineAsset asset)
        {
            asset.supportsHDR = enableHDR;
            asset.useSRPBatcher = enableSRPBatcher;
            asset.msaaSampleCount = (int)msaaQuality;
            asset.shadowDistance = shadowDistance;
        }
    }
}
