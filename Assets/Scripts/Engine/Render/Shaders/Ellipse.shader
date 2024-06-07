Shader "Primitives/Ellipse"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        
        _AlternativeColor ("State Color", Color) = (0.5, .8, .8, 1)
        
        _FadeDuration ("Fade Duration", Range(0, 10)) = 2
        _FadeColor ("Alternative Color", Color) = (1, 0.5, 0.5, 1)

        _Intensity ("Intensity", Range(0.0, 3.0)) = 0.7
        _Ambient ("Ambient", Range(0.0, 1.0)) = 0.2
        
    }
    SubShader
    {
        Pass
        {
            Tags
            {
                "Queue"="Transparent"
                "RenderType"="Transparent"
                "RenderPipeline" = "UniversalRenderPipeline"
            }



            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define PI2 PI * 2
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Helper.hlsl"
            
            StructuredBuffer<float3> _Positions;
            StructuredBuffer<float> _Scales;
            
            StructuredBuffer<float> _AlternativeColorRatios;
            StructuredBuffer<float> _FadeStartTimes;

            CBUFFER_START(UnityPerMaterial)
            // Coloring
            uniform float4 _Color;
            uniform float4 _AlternativeColor;

            // Alternative
            uniform float4 _FadeColor;
            uniform float _FadeDuration;

            // Light
            uniform float _Intensity, _Ambient;
            uniform sampler2D _MainTex;
            CBUFFER_END

            void stroke(float dist, float3 color, inout float3 fragColor, float thickness, float aa)
            {
                float alpha = smoothstep(0.5 * (thickness + aa), 0.5 * (thickness - aa), abs(dist));
                fragColor = lerp(fragColor, color, alpha);
            }

            void render_circle(float2 center, float radius, float2 pos, inout float3 fragColor)
            {
                float dist = length(pos - center) - radius;
                stroke(dist, float3(0, 0, 1), fragColor, 0.05, length(fwidth(pos)));
            }
            
            struct attributes
            {
                float3 normal : NORMAL;
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
            };

            struct varyings
            {
                float4 vertex : SV_POSITION;
                float3 diffuse : TEXCOORD2;
                float2 uv : TEXCOORD0;
                uint instance_id : SV_InstanceID;
            };

            varyings vert(const attributes v, const uint instance_id : SV_InstanceID)
            {
                varyings o;
                const float3 pos = _Positions[instance_id];
                o.vertex = TransformObjectToHClip(v.vertex.xyz );
                o.uv = v.uv;
                o.diffuse = saturate(dot(v.normal, _MainLightPosition.xyz));
                o.instance_id = instance_id;
                return o;
            }

            float4 frag(const varyings i) : SV_Target
            {
                half4 frag = float4(0,0,0,0);
                
                render_circle(float2(0,0), 0.455f, i.uv - 0.5f, frag.xyz);

                //frag.rg = i.uv;
                frag.a = 1;
                return _Color;
            }
            ENDHLSL
        }
    }
}