Shader "UWG/HostSkin_URP"
{
    Properties
    {
        // --- Base PBR (URP Lit compatible) ---
        [MainTexture] _BaseMap ("Base Map (Albedo)", 2D) = "white" {}
        [MainColor]   _BaseColor ("Base Color", Color) = (1, 0.85, 0.78, 1)
        _BumpMap ("Normal Map", 2D) = "bump" {}
        _BumpScale ("Normal Scale", Float) = 1.0
        _Smoothness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0

        // --- UWG Custom: Host Condition Effects ---
        [Header(Thermal Flush)]
        _FlushIntensity ("Flush Intensity", Range(0,1)) = 0.0
        _FlushColor ("Flush Color", Color) = (0.9, 0.3, 0.3, 1)

        [Header(Sweat Glisten)]
        _SweatIntensity ("Sweat Glisten", Range(0,1)) = 0.0
        _SweatSpecBoost ("Sweat Specular Boost", Range(0,2)) = 1.5

        [Header(Skin Translucency)]
        _Translucency ("Translucency", Range(0,1)) = 0.0
        _TranslucencyColor ("Subsurface Color", Color) = (0.9, 0.5, 0.5, 1)
        _TranslucencyMap ("Translucency Map (belly mask)", 2D) = "white" {}

        [Header(Vein Glow)]
        _VeinGlowIntensity ("Vein Glow Intensity", Range(0,2)) = 0.0
        _VeinGlowColor ("Vein Glow Color", Color) = (0.3, 0.6, 1.0, 1)
        _VeinMap ("Vein Pattern Map", 2D) = "black" {}

        [Header(Belly Stretch Marks)]
        _StretchIntensity ("Stretch Mark Visibility", Range(0,1)) = 0.0
        _StretchMap ("Stretch Mark Map", 2D) = "black" {}
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
            "RenderPipeline" = "UniversalPipeline"
            "Queue" = "Geometry"
        }

        LOD 300

        // ================================================================
        // FORWARD PASS (URP Lit equivalent with custom host effects)
        // ================================================================
        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _ADDITIONAL_LIGHTS
            #pragma multi_compile _ _SHADOWS_SOFT
            #pragma multi_compile_fog

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceInput.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
                float4 tangentOS  : TANGENT;
                float2 uv         : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS  : SV_POSITION;
                float2 uv          : TEXCOORD0;
                float3 positionWS  : TEXCOORD1;
                float3 normalWS    : TEXCOORD2;
                float3 viewDirWS   : TEXCOORD3;
                float  fogFactor   : TEXCOORD4;
            };

            // Textures
            TEXTURE2D(_BaseMap);        SAMPLER(sampler_BaseMap);
            TEXTURE2D(_BumpMap);        SAMPLER(sampler_BumpMap);
            TEXTURE2D(_TranslucencyMap); SAMPLER(sampler_TranslucencyMap);
            TEXTURE2D(_VeinMap);        SAMPLER(sampler_VeinMap);
            TEXTURE2D(_StretchMap);     SAMPLER(sampler_StretchMap);

            // Properties
            CBUFFER_START(UnityPerMaterial)
                float4 _BaseMap_ST;
                half4  _BaseColor;
                half   _BumpScale;
                half   _Smoothness;
                half   _Metallic;

                half   _FlushIntensity;
                half4  _FlushColor;

                half   _SweatIntensity;
                half   _SweatSpecBoost;

                half   _Translucency;
                half4  _TranslucencyColor;

                half   _VeinGlowIntensity;
                half4  _VeinGlowColor;

                half   _StretchIntensity;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;

                VertexPositionInputs posInputs = GetVertexPositionInputs(IN.positionOS.xyz);
                VertexNormalInputs normInputs  = GetVertexNormalInputs(IN.normalOS, IN.tangentOS);

                OUT.positionCS = posInputs.positionCS;
                OUT.positionWS = posInputs.positionWS;
                OUT.normalWS   = normInputs.normalWS;
                OUT.viewDirWS  = GetWorldSpaceNormalizeViewDir(posInputs.positionWS);
                OUT.uv         = TRANSFORM_TEX(IN.uv, _BaseMap);
                OUT.fogFactor  = ComputeFogFactor(posInputs.positionCS.z);

                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                // --- Base albedo ---
                half4 baseMap = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.uv);
                half3 albedo  = baseMap.rgb * _BaseColor.rgb;

                // --- Thermal Flush: tint skin red ---
                albedo = lerp(albedo, albedo * _FlushColor.rgb, _FlushIntensity);

                // --- Stretch Marks: darken in stretch areas ---
                half stretchMask = SAMPLE_TEXTURE2D(_StretchMap, sampler_StretchMap, IN.uv).r;
                albedo = lerp(albedo, albedo * 0.7, stretchMask * _StretchIntensity);

                // --- Lighting (URP main light) ---
                float3 normalWS = normalize(IN.normalWS);

                // Sample normal map
                half3 normalTS = UnpackNormalScale(
                    SAMPLE_TEXTURE2D(_BumpMap, sampler_BumpMap, IN.uv), _BumpScale);
                // Simplified: use world normal directly (full TBN in production)
                // normalWS = TransformTangentToWorld(normalTS, ...);

                InputData inputData = (InputData)0;
                inputData.positionWS = IN.positionWS;
                inputData.normalWS = normalWS;
                inputData.viewDirectionWS = IN.viewDirWS;
                inputData.fogCoord = IN.fogFactor;
                inputData.normalizedScreenSpaceUV = GetNormalizedScreenSpaceUV(IN.positionCS);

                // --- Smoothness: boost from sweat ---
                half smoothness = _Smoothness + (_SweatIntensity * _SweatSpecBoost * 0.3);
                smoothness = saturate(smoothness);

                SurfaceData surfData = (SurfaceData)0;
                surfData.albedo = albedo;
                surfData.metallic = _Metallic;
                surfData.smoothness = smoothness;
                surfData.normalTS = normalTS;
                surfData.occlusion = 1.0;
                surfData.alpha = 1.0;

                half4 litColor = UniversalFragmentPBR(inputData, surfData);

                // --- Translucency: subsurface scattering approximation ---
                half transMap = SAMPLE_TEXTURE2D(_TranslucencyMap, sampler_TranslucencyMap, IN.uv).r;
                Light mainLight = GetMainLight();
                half transNdotL = saturate(dot(-normalWS, mainLight.direction));
                half3 transContrib = _TranslucencyColor.rgb * transNdotL * _Translucency * transMap;
                litColor.rgb += transContrib * mainLight.color;

                // --- Vein Glow: emissive overlay ---
                half veinMask = SAMPLE_TEXTURE2D(_VeinMap, sampler_VeinMap, IN.uv).r;
                half3 veinGlow = _VeinGlowColor.rgb * veinMask * _VeinGlowIntensity;
                litColor.rgb += veinGlow;

                // --- Fog ---
                litColor.rgb = MixFog(litColor.rgb, IN.fogFactor);

                return litColor;
            }
            ENDHLSL
        }

        // ================================================================
        // SHADOW CASTER PASS (required for URP shadow rendering)
        // ================================================================
        Pass
        {
            Name "ShadowCaster"
            Tags { "LightMode" = "ShadowCaster" }

            ZWrite On
            ZTest LEqual
            ColorMask 0

            HLSLPROGRAM
            #pragma vertex ShadowPassVertex
            #pragma fragment ShadowPassFragment

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/ShadowCasterPass.hlsl"
            ENDHLSL
        }

        // ================================================================
        // DEPTH ONLY PASS (required for URP depth prepass)
        // ================================================================
        Pass
        {
            Name "DepthOnly"
            Tags { "LightMode" = "DepthOnly" }

            ZWrite On
            ColorMask R

            HLSLPROGRAM
            #pragma vertex DepthOnlyVertex
            #pragma fragment DepthOnlyFragment

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/DepthOnlyPass.hlsl"
            ENDHLSL
        }
    }

    FallBack "Universal Render Pipeline/Lit"
}
