Shader "Skybox/VanGoghSkybox"
{
    Properties
    {
        [Header(Movement)]
        _Speed       ("Flow Speed",          Range(0.001, 0.2))  = 0.04
        _FlowScale1  ("Flow Noise Scale 1",  Range(1.0, 10.0))   = 3.0
        _FlowScale2  ("Flow Noise Scale 2",  Range(1.0, 10.0))   = 5.0
        _SwirlScale  ("Swirl Scale",         Range(1.0, 20.0))   = 8.0
        _SwirlAmount ("Swirl Distortion",    Range(0.0, 1.0))    = 0.35

        [Header(Brushstrokes)]
        _BrushScale  ("Brush Scale",         Range(1.0, 30.0))   = 14.0
        _BrushStretch("Brush Stretch",       Range(1.0, 10.0))   = 4.0
        _BrushAngle  ("Brush Angle (deg)",   Range(0.0, 360.0))  = 35.0
        _BrushStrength("Brush Strength",     Range(0.0, 1.0))    = 0.55
        _PosterizeSteps("Posterize Steps",   Range(2.0, 10.0))   = 4.0

        [Header(Sky Colors)]
        _DeepBlue    ("Deep Blue",   Color) = (0.039, 0.086, 0.157, 1)
        _MidBlue     ("Mid Blue",    Color) = (0.106, 0.227, 0.478, 1)
        _SwirlBlue   ("Swirl Blue",  Color) = (0.357, 0.561, 0.831, 1)
        _BrushLight  ("Brush Light", Color) = (0.600, 0.780, 0.980, 1)

        [Header(Stars)]
        _StarYellow  ("Star Yellow", Color) = (0.961, 0.902, 0.259, 1)
        _StarWhite   ("Star White",  Color) = (1.000, 0.984, 0.910, 1)
        _StarDensity ("Star Density",  Range(1.0, 10.0)) = 3.0
        _StarSharp   ("Star Sharpness",Range(4.0, 30.0)) = 18.0
        _StarTiling  ("Star Tiling",   Range(1.0, 10.0)) = 4.0
        _StarBrightness("Star Brightness", Range(0.0, 3.0)) = 1.2
    }

    SubShader
    {
        Tags { "Queue"="Background" "RenderType"="Background" "PreviewType"="Skybox" }
        Cull Off
        ZWrite Off
        ZTest LEqual

        Pass
        {
            CGPROGRAM
            #pragma vertex   vert
            #pragma fragment frag
            #pragma target   3.0
            #include "UnityCG.cginc"

            float  _Speed, _FlowScale1, _FlowScale2, _SwirlScale, _SwirlAmount;
            float  _BrushScale, _BrushStretch, _BrushAngle, _BrushStrength, _PosterizeSteps;
            float4 _DeepBlue, _MidBlue, _SwirlBlue, _BrushLight;
            float4 _StarYellow, _StarWhite;
            float  _StarDensity, _StarSharp, _StarTiling, _StarBrightness;

            struct appdata
            {
                float4 vertex : POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            struct v2f
            {
                float4 pos      : SV_POSITION;
                float3 worldDir : TEXCOORD0;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            v2f vert(appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                o.pos      = UnityObjectToClipPos(v.vertex);
                o.worldDir = v.vertex.xyz;
                return o;
            }

            // ── Hash ──────────────────────────────────────────────────
            float hash21(float2 p)
            {
                p = frac(p * float2(127.1, 311.7));
                p += dot(p, p + 19.19);
                return frac(p.x * p.y);
            }

            // ── Value noise ───────────────────────────────────────────
            float valueNoise(float2 p)
            {
                float2 i = floor(p);
                float2 f = frac(p);
                float2 u = f * f * (3.0 - 2.0 * f);
                float a = hash21(i);
                float b = hash21(i + float2(1, 0));
                float c = hash21(i + float2(0, 1));
                float d = hash21(i + float2(1, 1));
                return lerp(lerp(a, b, u.x), lerp(c, d, u.x), u.y);
            }

            // ── Anisotropic brush noise ────────────────────────────────
            // Stretches UVs along a rotated axis to simulate brushstrokes
            float brushNoise(float2 p, float angleDeg, float stretch, float scale)
            {
                float rad = angleDeg * 0.01745329; // deg → rad
                float2 dir = float2(cos(rad), sin(rad));
                float2 perp = float2(-dir.y, dir.x);
                // Decompose p into (along stroke, across stroke)
                float2 stretched = float2(dot(p, dir) * stretch,
                                          dot(p, perp));
                return valueNoise(stretched * scale);
            }

            // ── Posterize ─────────────────────────────────────────────
            float3 posterize(float3 col, float steps)
            {
                return floor(col * steps + 0.5) / steps;
            }

            // ── Voronoi ───────────────────────────────────────────────
            float voronoi(float2 p)
            {
                float2 i = floor(p);
                float2 f = frac(p);
                float minDist = 8.0;
                for (int y = -1; y <= 1; y++)
                for (int x = -1; x <= 1; x++)
                {
                    float2 n  = float2(x, y);
                    float2 pt = float2(
                        hash21(i + n),
                        hash21(i + n + float2(17.0, 31.0))
                    );
                    pt = 0.5 + 0.5 * sin(_Time.y * 0.08 + 6.2831 * pt);
                    float2 diff = n + pt - f;
                    minDist = min(minDist, dot(diff, diff));
                }
                return sqrt(minDist);
            }

            // ── Fragment ──────────────────────────────────────────────
            fixed4 frag(v2f i) : SV_Target
            {
                float3 dir   = normalize(i.worldDir);
                float2 skyUV = dir.xz / (abs(dir.y) + 0.5);

                float animTime = _Time.y * _Speed;

                // Flow field
                float flow1 = valueNoise((skyUV + animTime)       * _FlowScale1);
                float flow2 = valueNoise((skyUV - animTime * 0.7) * _FlowScale2);
                float flowC = flow1 + flow2;

                // Swirl distortion
                float2 distUV = skyUV + flowC * _SwirlAmount;
                float  swirl  = valueNoise(distUV * _SwirlScale);

                // Base sky color
                float3 sky = lerp(_DeepBlue.rgb, _MidBlue.rgb, swirl);
                       sky = lerp(sky, _SwirlBlue.rgb, saturate(flowC * 0.5));

                // ── Brushstroke layer ─────────────────────────────────
                // Animate brush angle slightly for living-painting feel
                float dynAngle = _BrushAngle + sin(_Time.y * _Speed * 0.3) * 15.0;
                float brush    = brushNoise(distUV, dynAngle,       _BrushStretch, _BrushScale);
                float brush2   = brushNoise(distUV, dynAngle + 90.0, _BrushStretch * 0.5, _BrushScale * 1.3);
                float brushMix = brush * 0.6 + brush2 * 0.4;

                // Blend brush highlights into sky
                sky = lerp(sky, _BrushLight.rgb, brushMix * _BrushStrength * swirl);

                // Posterize for cartoon / flat look
                sky = posterize(sky, _PosterizeSteps);

                // ── Stars ─────────────────────────────────────────────
                float2 starUV   = skyUV * _StarTiling;
                float  voro     = voronoi(starUV * _StarDensity);
                // Sharper cutoff = fewer, crisper stars
                float  starMask = pow(saturate(1.0 - voro * 1.4), _StarSharp);

                float3 starCol  = lerp(_StarYellow.rgb, _StarWhite.rgb, starMask * 0.6);
                float3 finalCol = sky + starCol * starMask * _StarBrightness;

                return fixed4(finalCol, 1.0);
            }
            ENDCG
        }
    }
    Fallback Off
}
