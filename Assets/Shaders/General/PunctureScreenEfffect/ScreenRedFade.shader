Shader "UI/Screen Red Swirl"
{
    Properties
    {
        _RedColor ("Red Color", Color) = (0.45, 0.0, 0.0, 1)
        _BlackColor ("Black Color", Color) = (0.005, 0.0, 0.0, 1)

        _Progress ("Progress", Range(0, 1)) = 0

        _SwirlAmount ("Swirl Amount", Range(0, 10)) = 3
        _SwirlSpeed ("Swirl Speed", Range(0, 5)) = 0.5

        _NoiseScale ("Noise Scale", Range(1, 20)) = 4
        _NoiseSpeed ("Noise Speed", Range(0, 5)) = 0.5

        _Contrast ("Red/Black Contrast", Range(0.1, 10)) = 2

        _Vignette ("Vignette", Range(0, 2)) = 0.7
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "RenderPipeline"="UniversalPipeline"
        }

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

            CBUFFER_START(UnityPerMaterial)

            float4 _RedColor;
            float4 _BlackColor;

            float _Progress;

            float _SwirlAmount;
            float _SwirlSpeed;

            float _NoiseScale;
            float _NoiseSpeed;

            float _Contrast;
            float _Vignette;

            CBUFFER_END


            // =========================================================
            // RANDOM
            // =========================================================

            float random(float2 p)
            {
                return frac(
                    sin(
                        dot(
                            p,
                            float2(127.1, 311.7)
                        )
                    ) * 43758.5453123
                );
            }


            // =========================================================
            // VALUE NOISE
            // =========================================================

            float noise(float2 p)
            {
                float2 i = floor(p);
                float2 f = frac(p);

                f = f * f * (3.0 - 2.0 * f);

                float a = random(i);
                float b = random(i + float2(1, 0));
                float c = random(i + float2(0, 1));
                float d = random(i + float2(1, 1));

                return lerp(
                    lerp(a, b, f.x),
                    lerp(c, d, f.x),
                    f.y
                );
            }


            // =========================================================
            // FRACTAL NOISE
            // =========================================================

            float fbm(float2 p)
            {
                float value = 0.0;

                value += noise(p) * 0.5;

                p *= 2.0;
                value += noise(p) * 0.25;

                p *= 2.0;
                value += noise(p) * 0.125;

                p *= 2.0;
                value += noise(p) * 0.0625;

                return value;
            }


            // =========================================================
            // SWIRL
            // =========================================================

            float2 swirl(float2 uv, float amount)
            {
                float2 center = uv - 0.5;

                float radius = length(center);

                float angle =
                    atan2(center.y, center.x);

                // Stronger rotation toward the center
                float rotation =
                    amount *
                    (1.0 - radius);

                rotation *=
                    (0.5 + 0.5 * sin(
                        radius * 8.0
                        - _Time.y * _SwirlSpeed
                    ));

                angle += rotation;

                return float2(
                    cos(angle),
                    sin(angle)
                ) * radius + 0.5;
            }


            // =========================================================
            // VERTEX
            // =========================================================

            Varyings vert(Attributes input)
            {
                Varyings output;

                output.positionHCS =
                    TransformObjectToHClip(
                        input.positionOS.xyz
                    );

                output.uv = input.uv;

                return output;
            }


            // =========================================================
            // FRAGMENT
            // =========================================================

            half4 frag(Varyings input) : SV_Target
            {
                float2 uv = input.uv;

                float time = _Time.y;


                // =====================================================
                // SWIRL THE UV SPACE
                // =====================================================

                float2 swirledUV =
                    swirl(
                        uv,
                        _SwirlAmount
                        * _Progress
                    );


                // =====================================================
                // ORGANIC DISTORTION
                // =====================================================

                float2 noiseUV =
                    swirledUV * _NoiseScale;

                noiseUV.x += time * _NoiseSpeed;
                noiseUV.y += time * _NoiseSpeed * 0.35;


                float n1 =
                    fbm(noiseUV);


                // Second layer moving in another direction
                float n2 =
                    fbm(
                        noiseUV * 1.7
                        + float2(
                            -time * 0.25,
                            time * 0.15
                        )
                    );


                // Combine the noise
                float pattern =
                    n1 * 0.65 +
                    n2 * 0.35;


                // =====================================================
                // SPIRAL PATTERN
                // =====================================================

                float2 centered =
                    swirledUV - 0.5;

                float radius =
                    length(centered);

                float angle =
                    atan2(
                        centered.y,
                        centered.x
                    );


                float spiral =
                    sin(
                        angle * 5.0
                        + radius * 18.0
                        - time * _SwirlSpeed * 2.0
                    );


                spiral =
                    spiral * 0.5 + 0.5;


                // Blend spiral into organic noise
                pattern =
                    lerp(
                        pattern,
                        pattern * 0.6 +
                        spiral * 0.4,
                        0.6
                    );


                // =====================================================
                // CONTRAST
                // =====================================================

                pattern =
                    saturate(
                        (pattern - 0.5)
                        * _Contrast
                        + 0.5
                    );


                // =====================================================
                // PROGRESS
                // =====================================================

                // At the beginning the effect is almost invisible.
                //
                // As progress increases, the red/black pattern
                // expands toward the edges.

                float threshold =
                    1.0 - _Progress;

                float mask =
                    smoothstep(
                        threshold - 0.25,
                        threshold + 0.25,
                        radius
                    );


                // Make sure the effect gradually takes over
                float effectAmount =
                    _Progress;


                // =====================================================
                // RED / BLACK MIX
                // =====================================================

                float3 swirlColor =
                    lerp(
                        _BlackColor.rgb,
                        _RedColor.rgb,
                        pattern
                    );


                // Darken the outside
                float vignette =
                    smoothstep(
                        0.2,
                        0.8,
                        radius
                    );

                swirlColor *=
                    1.0 -
                    vignette * _Vignette *
                    _Progress;


                // =====================================================
                // FINAL ALPHA
                // =====================================================

                float alpha =
                    effectAmount *
                    mask;


                // Slightly soften the beginning
                alpha =
                    smoothstep(
                        0.0,
                        1.0,
                        alpha
                    );


                return half4(
                    swirlColor,
                    alpha
                );
            }

            ENDHLSL
        }
    }
}