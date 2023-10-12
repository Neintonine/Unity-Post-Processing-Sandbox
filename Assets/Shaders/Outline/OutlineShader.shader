Shader "PostProcessing/OutlineShader"
{
    HLSLINCLUDE

    #pragma target 4.5
    #pragma only_renderers d3d11 playstation xboxone xboxseries vulkan metal switch
    #pragma multi_compile _ OUTLINE_VIEW

    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
    #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareOpaqueTexture.hlsl"
    #include "Assets/Shaders/Framework/Common.cginc"

    // List of properties to control your post process effect
    float _Intensity;

    float3 _OutlineColor;
        
    float SampleDepth(float2 uv) {        
        //return SampleSceneDepth(uv).r;
        //return LOAD_TEXTURE2D_X_LOD(_CameraDepthTexture, uv, 0).r;
        return SampleSceneDepth(uv / _ScreenSize.xy);
    }

    float _SobelPower;
    float _HorizDiagCoeff;
    float _HorizAxisCoeff;
    float _VertDiagCoeff;
    float _VertAxisCoeff;
    
    float4 GetDiagDepths(uint2 uv)
    {
        uint3 coordnates = uint3(0,1,-1);
        return float4(
            SampleDepth(uv + coordnates.yy), //1,1
            SampleDepth(uv + coordnates.zy), //-1,1
            SampleDepth(uv + coordnates.zz), //-1,-1
            SampleDepth(uv + coordnates.yz)  //1,-1
        );
    }

    float4 GetAxisDepths(uint2 uv)
    {
        uint3 coordnates = uint3(0,1,-1);
        return float4(
            SampleDepth(uv + coordnates.xx), //1,0
            SampleDepth(uv + coordnates.xy), //0,1
            SampleDepth(uv + coordnates.zx), //-1,0
            SampleDepth(uv + coordnates.xz)  //0,-1
        );
    }

    float4 BorderlandsWay(Varyings input) : SV_Target
    {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

        uint2 positionSS = input.texcoord * _ScreenSize.xy;
        float3 baseColor = SampleSceneColor(input.texcoord);

        float centerSample = SampleDepth(positionSS);
        float4 diagDepths = GetDiagDepths(positionSS);
        float4 axisDepths = GetAxisDepths(positionSS);

        diagDepths = diagDepths > centerSample.xxxx ? diagDepths : centerSample.xxxx;
        axisDepths = axisDepths > centerSample.xxxx ? axisDepths : centerSample.xxxx;
        
        diagDepths -= centerSample;
        axisDepths -= centerSample;
        //return axisDepths;

        float4 sobelH = diagDepths * _HorizDiagCoeff + axisDepths * _HorizAxisCoeff;
        float4 sobelV = diagDepths * _VertDiagCoeff + axisDepths * _VertAxisCoeff;

        float sobelX = dot(sobelH, ffloat4(1));
        float sobelY = dot(sobelV, ffloat4(1));

        float sobel = sqrt(sobelX * sobelX + sobelY * sobelY);
        float value = saturate(1 - pow(saturate(sobel), _SobelPower));

        #if defined OUTLINE_VIEW
            return ffloat4(lerp(_OutlineColor, ffloat3(0), value), 1);
        #endif
        
        
        return ffloat4(lerp(_OutlineColor, baseColor, value), 1);
    }

    ENDHLSL
    
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline"}
        LOD 100
        ZWrite Off Cull Off
        Pass
        {
            Name "ColorBlitPass"

            HLSLPROGRAM

            #pragma vertex Vert
            #pragma fragment BorderlandsWay
            
            ENDHLSL
        }
    }
}