Shader "Custom/SplashRecuerdo"
{
    Properties
    {
        _MainTex ("Captura", 2D) = "white" {}
        _MaskTex ("Mascara Splash", 2D) = "white" {}
        [MainColor] _Color ("Tinte", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" "RenderPipeline"="UniversalPipeline" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
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

            TEXTURE2D(_MainTex);   SAMPLER(sampler_MainTex);
            TEXTURE2D(_MaskTex);   SAMPLER(sampler_MaskTex);
            float4 _Color;

            Varyings vert (Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;
                return OUT;
            }

            half4 frag (Varyings IN) : SV_Target
            {
                half4 captura = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv);
                half4 mascara = SAMPLE_TEXTURE2D(_MaskTex, sampler_MaskTex, IN.uv);

                // La forma del splash (parte blanca) define donde se ve la captura
                half alpha = mascara.r * captura.a;

                half4 col = captura * _Color;
                col.a = alpha;
                return col;
            }
            ENDHLSL
        }
    }
}
