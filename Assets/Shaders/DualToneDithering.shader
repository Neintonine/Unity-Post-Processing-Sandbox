Shader "PostProcessing/DualToneDithering"
{
    HLSLINCLUDE

    #pragma target 4.5
    #pragma only_renderers d3d11 playstation xboxone xboxseries vulkan metal switch
    #pragma multi_compile _ VIEW_SCENE_BRIGHTHNESS VIEW_SCENE_BRIGHTNESS_WITH_DEPTH
    #pragma multi_compile _ DEPTH_AFFECTED

    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
    #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
    #include "Assets/Shaders/Framework/Common.cginc"

    TEXTURE2D_X(_SceneColor);

    TEXTURE2D_X(_DitherPattern);
    float4 _DitherPattern_TexelSize;
    float _DitherPatternScale;

    float3 _BackgroundColor;
    float3 _ForegroundColor;

    float CalculateSceneBrightness(float3 sceneColor)
    {
        const float3 luminanceMultiplier = float3(0.299, 0.587, 0.114);
        return luminanceMultiplier.x * sceneColor.x +
                luminanceMultiplier.y * sceneColor.y +
                luminanceMultiplier.z * sceneColor.z;
    }
    
    float GetDitherValue(float2 screenPos)
    {
        return SAMPLE_TEXTURE2D_X(_DitherPattern, sampler_PointRepeat, screenPos * _DitherPattern_TexelSize.xy * _DitherPatternScale).r;
    }
    
    float4 Frag(Varyings input) : SV_Target
    {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

        const float2 screenPos = input.texcoord.xy * _ScreenParams.xy;

        const float4 sceneColor = SAMPLE_TEXTURE2D_X(_SceneColor, sampler_PointClamp, input.texcoord);
        
        const float ditherValue = GetDitherValue(screenPos);
        float sceneBrightness = CalculateSceneBrightness(sceneColor);
        #if defined VIEW_SCENE_BRIGHTHNESS
            return debug_color(sceneBrightness);
        #endif
        
        #if defined DEPTH_AFFECTED
            sceneBrightness *= SampleSceneDepth(input.texcoord.xy);

            #if defined VIEW_SCENE_BRIGHTNESS_WITH_DEPTH
                return debug_color(sceneBrightness);
            #endif
        #endif
        
        const float ditherResult = step(ditherValue, sceneBrightness);

        return float4(lerp(_BackgroundColor,_ForegroundColor, ditherResult), 1);
    }

    ENDHLSL
    
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline"}
        LOD 100
        ZWrite Off Cull Off
        ZTest Always
        ZClip Off
        Pass
        {
            Name "DualTonePass"

            HLSLPROGRAM

            #pragma vertex Vert
            #pragma fragment Frag
            
            ENDHLSL
        }
    }
}