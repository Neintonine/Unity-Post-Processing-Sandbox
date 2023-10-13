Shader "PostProcessing/OutlineShader"
{
    HLSLINCLUDE

    #pragma target 4.5
    #pragma only_renderers d3d11 playstation xboxone xboxseries vulkan metal switch
    #pragma multi_compile _ OUTLINE_VIEW
    #pragma multi_compile _ ENABLE_FEATHERING

    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
    #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareOpaqueTexture.hlsl"
    #include "Assets/Shaders/Framework/Common.cginc"

    // List of properties to control your post process effect
    float _Intensity;

    float3 _OutlineColor;
    int _OutlineWidth;
    
    float _SobelPower;
    float _HorizDiagCoeff;
    float _HorizAxisCoeff;
    float _VertDiagCoeff;
    float _VertAxisCoeff;


    float _EdgeFeathering;
    float _EdgeFeatheringInverse;

    
    float SampleDepth(float2 screenUv, float centerDepth) {

        float2 textureUv = screenUv / _ScreenSize.xy;
        if (
            textureUv.x < 0 || textureUv.x > 1 ||
            textureUv.y < 0 || textureUv.y > 1
        )
        {
            return centerDepth;
        }
        
        return SampleSceneDepth(textureUv);
    }
    
    float4 GetDiagDepths(uint2 uv, float centerDepth)
    {
        uint3 coordnates = uint3(0,_OutlineWidth,-_OutlineWidth);
        return float4(
            SampleDepth(uv + coordnates.yy, centerDepth), //1,1
            SampleDepth(uv + coordnates.zy, centerDepth), //-1,1
            SampleDepth(uv + coordnates.zz, centerDepth), //-1,-1
            SampleDepth(uv + coordnates.yz, centerDepth)  //1,-1
        );
    }

    float4 GetAxisDepths(uint2 uv, float centerDepth)
    {
        uint3 coordnates = uint3(0,_OutlineWidth,-_OutlineWidth);
        return float4(
            SampleDepth(uv + coordnates.xx, centerDepth), //1,0
            SampleDepth(uv + coordnates.xy, centerDepth), //0,1
            SampleDepth(uv + coordnates.zx, centerDepth), //-1,0
            SampleDepth(uv + coordnates.xz, centerDepth)  //0,-1
        );
    }

    float GetScreenEdgeMask(float2 coordnates)
    {
        float left = saturate((_EdgeFeathering -  coordnates.x));
        float right = saturate(((1 - _EdgeFeathering) - coordnates.x) * -1);
        float top = saturate((_EdgeFeathering -  coordnates.y));
        float bottom = saturate(((1 - _EdgeFeathering) - coordnates.y) * -1);
        return (left + right + top + bottom) * _EdgeFeatheringInverse;
    }
    
    float4 BorderlandsWay(Varyings input) : SV_Target
    {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

        const uint2 position_ss = input.texcoord * _ScreenSize.xy;
        const float3 base_color = SampleSceneColor(input.texcoord);
        
        float center_sample = SampleSceneDepth(input.texcoord);
        float4 diag_depths = GetDiagDepths(position_ss, center_sample);
        float4 axis_depths = GetAxisDepths(position_ss, center_sample);

        diag_depths = diag_depths > center_sample.xxxx ? diag_depths : center_sample.xxxx;
        axis_depths = axis_depths > center_sample.xxxx ? axis_depths : center_sample.xxxx;
        
        diag_depths -= center_sample;
        axis_depths -= center_sample;

        const float4 sobel_h = diag_depths * _HorizDiagCoeff + axis_depths * _HorizAxisCoeff;
        const float4 sobel_v = diag_depths * _VertDiagCoeff + axis_depths * _VertAxisCoeff;

        const float sobel_x = dot(sobel_h, ffloat4(1));
        const float sobel_y = dot(sobel_v, ffloat4(1));

        const float sobel = sqrt(sobel_x * sobel_x + sobel_y * sobel_y);
        float value = saturate(pow(saturate(sobel), _SobelPower));
        value *= _Intensity;
        #if defined ENABLE_FEATHERING
        value *= 1 - GetScreenEdgeMask(input.texcoord);
        #endif

        #if defined OUTLINE_VIEW
            return ffloat4(lerp(1 - _OutlineColor, _OutlineColor, value), 1);
        #endif
        
        return ffloat4(lerp(base_color, _OutlineColor, value), 1);
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
            Name "OutlineP"

            HLSLPROGRAM

            #pragma vertex Vert
            #pragma fragment BorderlandsWay
            
            ENDHLSL
        }
    }
}