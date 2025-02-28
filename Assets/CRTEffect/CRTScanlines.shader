Shader "Custom/CRTScanlines"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _ScanlineDensity ("Scanline Density", Float) = 800
        _ScanlineIntensity ("Scanline Intensity", Range(0, 1)) = 0.5
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            float _ScanlineDensity;
            float _ScanlineIntensity;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float scanline = sin(i.uv.y * _ScanlineDensity * UNITY_PI) * 0.5 + 0.5;
                fixed4 col = tex2D(_MainTex, i.uv);
                col.rgb *= lerp(1.0, scanline, _ScanlineIntensity);
                return col;
            }
            ENDCG
        }
    }
}
