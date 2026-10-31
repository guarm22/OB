
Shader "Custom/SG_Waterfall"
{
    Properties
    {
        _WaterColor ("Water Color", Color) = (0.45, 0.8, 1, 1)
        _FoamColor ("Foam Color", Color) = (0.9, 0.97, 1, 1)
        _Opacity ("Opacity", Range(0, 1)) = 0.65
        _FlowSpeed ("Flow Speed", Range(0, 5)) = 1.5
        _StreakScale ("Streak Scale", Range(5, 60)) = 28
        _Distortion ("Turbulence", Range(0, 1)) = 0.25
        _FoamAmount ("Bottom Foam", Range(0, 1)) = 0.5
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
            "RenderType" = "Transparent"
            "Queue" = "Transparent"
        }

        Pass
        {
            Name "Waterfall"
            Tags { "LightMode" = "UniversalForward" }

            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off

            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            CBUFFER_START(UnityPerMaterial)
                float4 _WaterColor;
                float4 _FoamColor;
                float _Opacity;
                float _FlowSpeed;
                float _StreakScale;
                float _Distortion;
                float _FoamAmount;
            CBUFFER_END

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

            Varyings vert(Attributes input)
            {
                Varyings output;
                output.positionHCS =
                    TransformObjectToHClip(input.positionOS.xyz);
                output.uv = input.uv;
                return output;
            }

            float hash(float2 p)
            {
                return frac(sin(dot(p, float2(127.1, 311.7)))
                    * 43758.5453);
            }

            // Smooth, continuous value noise.
            float noise(float2 p)
            {
                float2 i = floor(p);
                float2 f = frac(p);

                f = f * f * f * (f * (f * 6.0 - 15.0) + 10.0);

                float a = hash(i);
                float b = hash(i + float2(1, 0));
                float c = hash(i + float2(0, 1));
                float d = hash(i + float2(1, 1));

                return lerp(
                    lerp(a, b, f.x),
                    lerp(c, d, f.x),
                    f.y
                );
            }

            // Layer multiple scales of noise for organic variation.
            float fbm(float2 p)
            {
                float value = 0.0;
                value += noise(p) * 0.57;
                value += noise(p * 2.0) * 0.28;
                value += noise(p * 4.0) * 0.15;
                return value;
            }

            half4 frag(Varyings input) : SV_Target
            {
                float2 uv = input.uv;
                float t = _Time.y * _FlowSpeed;

                // Slowly changing distortion.
                float broadNoise = fbm(
                    float2(uv.x * 3.5, uv.y * 2.0 - t * 0.3)
                );

                float fineNoise = fbm(
                    float2(uv.x * 8.0, uv.y * 5.0 - t * 0.6)
                );

                float warp = (broadNoise - 0.5) * _Distortion;

                // Smooth vertical streaks.
                float streakPattern = sin(
                    (uv.x + warp) * _StreakScale
                    + (fineNoise - 0.5) * 3.0
                );

                // Soft highlights instead of harsh bands.
                float streaks = smoothstep(
                    -0.75, 0.95, streakPattern
                );

                float detail = fbm(
                    float2(
                        (uv.x + warp) * 10.0,
                        uv.y * 7.0 - t
                    )
                );

                // Blend broad flow and fine detail.
                float flow = streaks * 0.55 + detail * 0.45;
                flow = saturate(flow);

                // Gradual shading across the water.
                float brightness = 0.65 + flow * 0.45;

                float3 water = _WaterColor.rgb * brightness;

                // Soft, sparse white highlights.
                float highlights = smoothstep(
                    0.68, 0.95, flow
                );

                water = lerp(
                    water,
                    _FoamColor.rgb,
                    highlights * 0.22
                );

                // Irregular foam near the bottom.
                float bottomMask = 1.0 - smoothstep(
                    0.0, 0.24, uv.y
                );

                float foamNoise = fbm(
                    float2(uv.x * 12.0, uv.y * 18.0 + t * 0.2)
                );

                float foam = bottomMask
                    * smoothstep(0.48, 0.78, foamNoise)
                    * _FoamAmount;

                water = lerp(water, _FoamColor.rgb, foam * 0.65);

                // Softer opacity variation.
                float alpha = _Opacity * lerp(
                    0.55, 0.95, flow
                );

                alpha = lerp(alpha, _Opacity, foam * 0.5);

                // Fade the ends of the sheet.
                float topFade = smoothstep(0.0, 0.025, uv.y);
                float bottomFade = smoothstep(
                    0.0, 0.035, 1.0 - uv.y
                );

                alpha *= topFade * bottomFade;

                return half4(water, saturate(alpha));
            }

            ENDHLSL
        }
    }

    FallBack Off
}
