Shader "UI/CRT Static"
{
    Properties
    {
        _Color ("Static Color", Color) = (1,1,1,1)

        _StaticIntensity ("Static Intensity", Range(0,1)) = 0.35
        _StaticScale ("Static Scale", Range(50,1000)) = 350
        _StaticSpeed ("Static Speed", Range(0,20)) = 8

        _ScanlineIntensity ("Scanline Intensity", Range(0,1)) = 0.2
        _ScanlineCount ("Scanline Count", Range(50,1000)) = 400
        _ScanlineSpeed ("Scanline Speed", Range(0,20)) = 1

        _Flicker ("Screen Flicker", Range(0,1)) = 0.08

        _Distortion ("Horizontal Distortion", Range(0,0.1)) = 0.015
        _DistortionSpeed ("Distortion Speed", Range(0,20)) = 4

        _Vignette ("Vignette", Range(0,1)) = 0.25

        _Opacity ("Opacity", Range(0,1)) = 0.5
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

            float4 _Color;

            float _StaticIntensity;
            float _StaticScale;
            float _StaticSpeed;

            float _ScanlineIntensity;
            float _ScanlineCount;
            float _ScanlineSpeed;

            float _Flicker;

            float _Distortion;
            float _DistortionSpeed;

            float _Vignette;

            float _Opacity;

            CBUFFER_END


            Varyings vert(Attributes input)
            {
                Varyings output;

                output.positionHCS =
                    TransformObjectToHClip(input.positionOS.xyz);

                output.uv = input.uv;

                return output;
            }


            // ---------------------------------------------------------
            // RANDOM
            // ---------------------------------------------------------

            float random(float2 uv)
            {
                return frac(
                    sin(dot(uv, float2(12.9898, 78.233)))
                    * 43758.5453
                );
            }


            // ---------------------------------------------------------
            // 1D RANDOM
            // ---------------------------------------------------------

            float random1(float x)
            {
                return frac(
                    sin(x * 127.1) * 43758.5453
                );
            }


            // ---------------------------------------------------------
            // STATIC
            // ---------------------------------------------------------

            float staticNoise(float2 uv)
            {
                float time = _Time.y * _StaticSpeed;

                uv *= _StaticScale;

                // Move the static diagonally
                uv += float2(
                    time * 17.0,
                    time * 11.0
                );

                // Pixelated television snow
                uv = floor(uv);

                return random(uv);
            }


            // ---------------------------------------------------------
            // HORIZONTAL CRT DISTORTION
            // ---------------------------------------------------------

            float horizontalDistortion(float y)
            {
                float time = _Time.y * _DistortionSpeed;

                float distortionNoise =
                    random1(floor(y * 30.0 + time));

                float distortion =
                    (distortionNoise - 0.5)
                    * _Distortion;

                return distortion;
            }


            // ---------------------------------------------------------
            // SCANLINES
            // ---------------------------------------------------------

            float scanlines(float2 uv)
            {
                float time = _Time.y * _ScanlineSpeed;

                float lines =
                    sin(
                        (uv.y * _ScanlineCount)
                        + time * 10.0
                    );

                // Convert to 0-1
                lines = lines * 0.5 + 0.5;

                return lines;
            }


            // ---------------------------------------------------------
            // FLICKER
            // ---------------------------------------------------------

            float screenFlicker()
            {
                float time = floor(_Time.y * 30.0);

                float flicker =
                    random1(time);

                return lerp(
                    1.0 - _Flicker,
                    1.0,
                    flicker
                );
            }


            // ---------------------------------------------------------
            // VIGNETTE
            // ---------------------------------------------------------

            float vignette(float2 uv)
            {
                float2 centered =
                    uv - 0.5;

                float distance =
                    length(centered);

                return 1.0 -
                    smoothstep(
                        0.35,
                        0.75,
                        distance
                    ) * _Vignette;
            }


            // ---------------------------------------------------------
            // FRAGMENT
            // ---------------------------------------------------------

            half4 frag(Varyings input) : SV_Target
            {
                float2 uv = input.uv;


                // -------------------------
                // CRT HORIZONTAL DISTORTION
                // -------------------------

                uv.x += horizontalDistortion(uv.y);


                // -------------------------
                // STATIC
                // -------------------------

                float noise =
                    staticNoise(uv);

                // Make the snow more aggressive
                float staticValue =
                    step(0.45, noise);


                // -------------------------
                // SCANLINES
                // -------------------------

                float scan =
                    scanlines(uv);

                float scanlineEffect =
                    lerp(
                        1.0 - _ScanlineIntensity,
                        1.0,
                        scan
                    );


                // -------------------------
                // FLICKER
                // -------------------------

                float flicker =
                    screenFlicker();


                // -------------------------
                // VIGNETTE
                // -------------------------

                float vignetteEffect =
                    vignette(uv);


                // -------------------------
                // COMBINE
                // -------------------------

                float brightness =
                    lerp(
                        1.0 - _StaticIntensity,
                        1.0,
                        staticValue
                    );

                brightness *= scanlineEffect;
                brightness *= flicker;
                brightness *= vignetteEffect;


                float3 finalColor =
                    _Color.rgb * brightness;


                return half4(
                    finalColor,
                    _Opacity
                );
            }

            ENDHLSL
        }
    }
}