Shader "Custom/TransparentScanline"
{
    Properties
    {
        _ScanlineDensity("Scanline Density", Float) = 400
        _ScanlineIntensity("Scanline Intensity", Range(0, 1)) = 0.6
        _TransparencyStrength("Transparency Strength", Range(0, 1)) = 0.5
        _ScanlineColor("Scanline Color", Color) = (0, 0, 0, 1)
    }

    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" "Queue"="Transparent" "RenderType"="Transparent" }
        Pass
        {
            Name "ScanlinePass"
            ZTest Always Cull Off ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            Varyings Vert(Attributes input)
            {
                Varyings output;
                output.positionHCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = input.uv;
                return output;
            }

            TEXTURE2D(_BlitTexture);
            SAMPLER(sampler_BlitTexture);

            float _ScanlineDensity;
            float _ScanlineIntensity;
            float _TransparencyStrength;
            float4 _ScanlineColor;

            half4 Frag(Varyings i) : SV_Target
            {
                float2 uv = i.uv;

                // Generate horizontal wave pattern for scanlines
                float scanline = sin(uv.y * _ScanlineDensity) * 0.5 + 0.5;

                // Sample the screen color
                half4 col = SAMPLE_TEXTURE2D(_BlitTexture, sampler_BlitTexture, uv);

                // Mix between original color and scanline tint
                float lineMix = lerp(1.0, scanline, _ScanlineIntensity);
                col.rgb = lerp(_ScanlineColor.rgb, col.rgb, lineMix);

                // Adjust alpha to fade between lines
                float transparency = lerp(1.0, scanline, _TransparencyStrength);
                col.a *= transparency * _ScanlineColor.a;

                return col;
            }
            ENDHLSL
        }
    }
}
