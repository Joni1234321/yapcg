// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Space/Star/Sun"
{
	Properties
	{
		_Radius("Radius", Float) = 0.5
		_Light("Light",Color) = (1,1,1,1)
		_Color("Color", Color) = (1,1,0,1)
		_Base("Base", Color) = (1,0,0,1)
		_Dark("Dark", Color) = (1,0,1,1)
		_RayString("Ray String", Range(0.02,10.0)) = 1.0
		_RayLight("Ray Light", Color) = (1,0.95,1.0,1)
		_Ray("Ray End", Color) = (1,0.6,0.1,1)
		_Detail("Detail Body", Range(0,5)) = 3
		_Rays("Rays", Range(1.0,10.0)) = 2.0
		_RayRing("Ray Ring", Range(1.0,10.0)) = 1.0
		_RayGlow("Ray Glow", Range(1.0,10.0)) = 2.0
		_Glow("Glow", Range(1.0,100.0)) = 4.0
		_Zoom("Zoom", Float) = 1.0
		_SpeedHi("Speed Hi", Range(0.0,10)) = 2.0
		_SpeedLow("Speed Low", Range(0.0,10)) = 2.0
		_SpeedRay("Speed Ray", Range(0.0,10)) = 5.0
		_SpeedRing("Speed Ring", Range(0.0,20)) = 2.0
		_Seed("Seed", Range(-10,10)) = 0
	}
		SubShader
	{
		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
		LOD 100

		Pass
		{
			Blend One OneMinusSrcAlpha
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 4.0
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
			#if UNITY_5_0
				UNITY_FOG_COORDS(1)
			#endif
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float _Radius;
			float _RayString;
			fixed4 _Light;
			fixed4 _Color;
			fixed4 _Base;
			fixed4 _Dark;
			fixed4 _Ray;
			fixed4 _RayLight;
			int _Detail;
			float _Rays;
			float _RayRing;
			float _RayGlow;
			float _Zoom;
			float _SpeedHi;
			float _SpeedLow;
			float _SpeedRay;
			float _SpeedRing;
			float _Glow;
			float _Seed;

			float4 posGlob; // center position
									
			v2f vert (appdata v)
			{
				v2f o;		
				posGlob = float4(UNITY_MATRIX_MV[0].w, UNITY_MATRIX_MV[1].w, UNITY_MATRIX_MV[2].w,0);
				float3x3 r=transpose((float3x3)UNITY_MATRIX_MV);
				float3x3 m;
				m[2]=normalize(mul(r,(float3)posGlob));
				m[1]=normalize(cross(m[2],float3(0.0, 1.0, 0.0)));
				m[0]=normalize(cross(m[1],m[2]));
				o.uv1 = mul(transpose(m), (float3)v.vertex);
            	o.vertex = UnityObjectToClipPos(float4(o.uv1, 1.0));
				
				#if UNITY_5_0
				UNITY_TRANSFER_FOG(o,o.vertex);
				#endif
				return o;
			}

			// animated noise
			fixed4 hash4(fixed4 n) { return frac(sin(n)*(fixed)753.5453123); }

			// mix noise for alive animation
			fixed noise4q(fixed4 x)
			{
				fixed4 n3 = fixed4(0,0.25,0.5,0.75);
				fixed4 p2 = floor(x.wwww+n3);
				fixed4 b = floor(x.xxxx +n3) + floor(x.yyyy +n3)*157.0 + floor(x.zzzz +n3)*113.0;
				fixed4 p1 = b + frac(p2*0.00390625)*fixed4(164352.0, -164352.0, 163840.0, -163840.0);
				p2 = b + frac((p2+1)*0.00390625)*fixed4(164352.0, -164352.0, 163840.0, -163840.0);
				fixed4 f1 = frac(x.xxxx+n3);
				fixed4 f2 = frac(x.yyyy+n3);
				
				fixed4 n1 = fixed4(0,1.0,157.0,158.0);
				fixed4 n2 = fixed4(113.0,114.0,270.0,271.0);		
				fixed4 vs1 = lerp(hash4(p1), hash4(n1.yyyy+p1), f1);
				fixed4 vs2 = lerp(hash4(n1.zzzz+p1), hash4(n1.wwww+p1), f1);
				fixed4 vs3 = lerp(hash4(p2), hash4(n1.yyyy+p2), f1);
				fixed4 vs4 = lerp(hash4(n1.zzzz+p2), hash4(n1.wwww+p2), f1);	
				vs1 = lerp(vs1, vs2, f2);
				vs3 = lerp(vs3, vs4, f2);
				
				vs2 = lerp(hash4(n2.xxxx+p1), hash4(n2.yyyy+p1), f1);
				vs4 = lerp(hash4(n2.zzzz+p1), hash4(n2.wwww+p1), f1);
				vs2 = lerp(vs2, vs4, f2);
				vs4 = lerp(hash4(n2.xxxx+p2), hash4(n2.yyyy+p2), f1);
				fixed4 vs5 = lerp(hash4(n2.zzzz+p2), hash4(n2.wwww+p2), f1);
				vs4 = lerp(vs4, vs5, f2);
				f1 = frac(x.zzzz+n3);
				f2 = frac(x.wwww+n3);
				
				vs1 = lerp(vs1, vs2, f1);
				vs3 = lerp(vs3, vs4, f1);
				vs1 = lerp(vs1, vs3, f2);
				
				return dot(vs1,0.25);
			}
					
			float RayProj;
			float sqRadius; // sphere radius
			float fragTime;
			float sphere; // sphere distance
			float3 surfase; // position on surfase

			// body of a star
			fixed noiseSpere(float zoom, float3 subnoise, float anim)
			{
				fixed s = 0.0;

				if (sphere <sqRadius) {
					if (_Detail>0.0) s = noise4q(fixed4(surfase*zoom*3.6864 + subnoise, fragTime*_SpeedHi))*0.625;
					if (_Detail>1.0) s =s*0.85+noise4q(fixed4(surfase*zoom*61.44 + subnoise*3.0, fragTime*_SpeedHi*3.0))*0.125;
					if (_Detail>2.0) s =s*0.94+noise4q(fixed4(surfase*zoom*307.2 + subnoise*5.0, anim*5.0))*0.0625;//*0.03125;
					if (_Detail>3.0) s =s*0.98+noise4q(fixed4(surfase*zoom*600.0 + subnoise*6.0, fragTime*_SpeedLow*6.0))*0.03125;
					if (_Detail>4.0) s =s*0.98+noise4q(fixed4(surfase*zoom*1200.0 + subnoise*9.0, fragTime*_SpeedLow*9.0))*0.01125;
				}
				return s;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				float invz =1/_Zoom;
				_Radius*=invz;
				fragTime=_Time.x*10.0;
				posGlob = float4(UNITY_MATRIX_MV[0].w, UNITY_MATRIX_MV[1].w, UNITY_MATRIX_MV[2].w,0);
				float3x3 m = (float3x3)UNITY_MATRIX_MV;
				float3 ray = normalize(mul(m, i.uv1) + posGlob.xyz);
				m = transpose((float3x3)UNITY_MATRIX_V);

				RayProj = dot(ray, (float3)posGlob);
				float sqDist=dot((float3)posGlob, (float3)posGlob);
				sphere = sqDist - RayProj*RayProj;
				sqRadius = _Radius*_Radius;
				if (RayProj<=0.0) sphere=sqRadius;
				float3 pr = ray*abs(RayProj) - (float3)posGlob;
				
				if (sqDist<=sqRadius) {
					surfase=-posGlob;
					sphere=sqDist;
				} else if (sphere <sqRadius) {
					float l1 = sqrt(sqRadius - sphere);
					surfase = mul(m,pr - ray*l1);
				} else {
					surfase=(float3)0;
				}

				fixed4 col = fixed4(0,0,0,0);

				if (_Detail >= 1.0) {
					float s1 = noiseSpere(0.5*_Zoom, float3(45.78, 113.04, 28.957)*_Seed, fragTime*_SpeedLow);
					s1 = pow(s1*2.4, 2.0);
					float s2 = noiseSpere(4.0*_Zoom, float3(83.23, 34.34, 67.453)*_Seed, fragTime*_SpeedHi);
					s2 = s2*2.2;

					col.xyz = fixed3(lerp((float3)_Color, (float3)_Light, pow(s1, 60.0))*s1);
					col.xyz += fixed3(lerp(lerp((float3)_Base, (float3)_Dark, s2*s2), (float3)_Light, pow(s2, 10.0))*s2);
				}

				fixed c = length(pr)*_Zoom;
				pr = normalize(mul(m, pr));//-ray;
				fixed s = max(0.0, (1.0 - abs(_Radius*_Zoom - c) / _RayString));//*RayProj;
				fixed nd = noise4q(float4(pr+float3(83.23, 34.34, 67.453)*_Seed, -fragTime*_SpeedRing + c))*2.0;
				nd = pow(nd, 2.0);
				fixed dr=1.0;
				if (sphere < sqRadius) dr = sphere / sqRadius;
				pr*=10.0;
				fixed n = noise4q(float4(pr+ float3(83.23, 34.34, 67.453)*_Seed, -fragTime*_SpeedRing + c))*dr;
				pr*=5.0;
				fixed ns = noise4q(float4(pr+ float3(83.23, 34.34, 67.453)*_Seed, -fragTime*_SpeedRay + c))*2.0*dr;
				if (_Detail>=3.0) {
					pr *= 3.0;
					ns = ns*0.5+noise4q(float4(pr+ float3(83.23, 34.34, 67.453)*_Seed, -fragTime*_SpeedRay + 0))*dr;
				}
				n = pow(n, _Rays)*pow(nd,_RayRing)*ns;
				fixed s3 = pow(s, _Glow) + pow(s, _RayGlow)*n;

				if (sphere < sqRadius) col.w = 1.0-s3*dr;
				if (sqDist>sqRadius)
					col.xyz = col.xyz+lerp((fixed3)_Ray, (fixed3)_RayLight, s3*s3*s3)*s3; //pow(s3, 3.0)
				
				col = clamp(col, 0, 1);

#if UNITY_5_0
				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);
#endif
				return col;
			}
			ENDCG
		}
	}
}

