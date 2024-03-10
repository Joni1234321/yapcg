Shader "Primitives/Lit"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _AnimationColor ("AnimationColor", Color) = (1, 0.5, 0.5, 1)
        _Intensity ("Intensity", Range(0.0, 3.0)) = 0.7
        _Ambient ("Ambient", Range(0.0, 1.0)) = 0.2
    }
    SubShader
    {
        Pass
        {
            Tags
            {
                "RenderType"="Opaque"
                "RenderPipeline" = "UniversalRenderPipeline"
            }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            StructuredBuffer<float3> _Positions;
            StructuredBuffer<uint> _Animation;
            StructuredBuffer<float> _AnimationStart;

            CBUFFER_START(UnityPerMaterial)
            uniform float4 _Color, _AnimationColor;
            uniform float _Intensity;
            uniform float _Ambient;
            uniform sampler2D _MainTex;
            CBUFFER_END

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

                o.vertex = TransformObjectToHClip(v.vertex.xyz  + pos);
                o.uv = v.uv;
                o.diffuse = saturate(dot(v.normal, _MainLightPosition.xyz));
                o.instance_id = instance_id;

                return o;
            }

            half4 frag(const varyings i) : SV_Target
            {
                const float3 albedo = tex2D(_MainTex, i.uv);
                const float3 light =  i.diffuse * _Intensity + _Ambient;


                //    return (value + 1) / 2;
                const float sinSat = (1.0 + sin(_Time.y * 4.0)) / 2.0;
                const float animStrength = .25 * sinSat + .25 * (_Animation[i.instance_id] / 255.0);


                float t = (_Animation[i.instance_id] > 0) * saturate(0.75 + animStrength);
                const float diff = _Time.y - _AnimationStart[i.instance_id];
                t = (diff < 5) * saturate(0.75 + animStrength);
                
                const half4 color = lerp(_Color, _AnimationColor, t);
                return half4(albedo * color * light, 1);
            }
            ENDHLSL
        }
    }
}