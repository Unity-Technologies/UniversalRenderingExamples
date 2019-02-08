Shader "Custom/AnimateShader_noHelper"
{
	Properties
	{
		
		    _MainTex ("MainTex", 2D) = "white" {}
		    _SubTex ("Subtex",2D) = "white" {}
		    [Enum(UnityEngine.Rendering.BlendMode)] _Src("Source",Float)=1
			[Enum(UnityEngine.Rendering.BlendMode)] _Des("Destination",Float) = 1
			_SpeedMainU("Speed Main U",Float) =0 
			_SpeedMainV("Speed Main V",Float) =0
			_SpeedSubU("Speed Sub U",Float) = 0
			_SpeedSubV("Speed Sub V",Float) =0
			_SpeedMainAlphaU("Speed Main Alpha U",Float) = 0
			_SpeedMainAlphaV("Speed Main Alpha V",Float) =0
			_SpeedSubAlphaU("Speed Sub Alpha U",Float) =0
			_SpeedSubAlphaV("Speed Sub Alpha V",Float) = 0
	     	_BlendMap ("Blend Map", 2D) = "white" {}
			_SpeedBlendU("Speed Blend U",Float) = 0
			_SpeedBlendV("Speed Blend V",Float) =0
		    _Blend("Blend",Range(0,1))=0
		    _AlphaMap1 ("Alpha 1",2D) = "white" { }
		    _AlphaMap2 ("Alpha 2",2D) = "white" { }
			_Index("Index",Int)=0
		
	


			_Octaves("Octaves", Float) = 8.0
			_Frequency("Frequency", Float) = 1.0
			_Amplitude("Amplitude", Float) = 1.0
			_Lacunarity("Lacunarity", Float) = 1.92
			_Persistence("Persistence", Float) = 0.8
			_Offset("Offset", Vector) = (0.0, 0.0, 0.0, 0.0)
			_AnimSpeed("Animation Speed", Float) = 1.0
	}
		CGINCLUDE


			void FAST32_hash_3D(float3 gridcell,
				out float4 lowz_hash_0,
				out float4 lowz_hash_1,
				out float4 lowz_hash_2,
				out float4 highz_hash_0,
				out float4 highz_hash_1,
				out float4 highz_hash_2)		//	generates 3 random numbers for each of the 8 cell corners
		{
			const float2 OFFSET = float2(50.0, 161.0);
			const float DOMAIN = 69.0;
			const float3 SOMELARGEFLOATS = float3(635.298681, 682.357502, 668.926525);
			const float3 ZINC = float3(48.500388, 65.294118, 63.934599);

			//	truncate the domain
			gridcell.xyz = gridcell.xyz - floor(gridcell.xyz * (1.0 / DOMAIN)) * DOMAIN;
			float3 gridcell_inc1 = step(gridcell, float3(DOMAIN - 1.5, DOMAIN - 1.5, DOMAIN - 1.5)) * (gridcell + 1.0);

			//	calculate the noise
			float4 P = float4(gridcell.xy, gridcell_inc1.xy) + OFFSET.xyxy;
			P *= P;
			P = P.xzxz * P.yyww;
			float3 lowz_mod = float3(1.0 / (SOMELARGEFLOATS.xyz + gridcell.zzz * ZINC.xyz));
			float3 highz_mod = float3(1.0 / (SOMELARGEFLOATS.xyz + gridcell_inc1.zzz * ZINC.xyz));
			lowz_hash_0 = frac(P * lowz_mod.xxxx);
			highz_hash_0 = frac(P * highz_mod.xxxx);
			lowz_hash_1 = frac(P * lowz_mod.yyyy);
			highz_hash_1 = frac(P * highz_mod.yyyy);
			lowz_hash_2 = frac(P * lowz_mod.zzzz);
			highz_hash_2 = frac(P * highz_mod.zzzz);
		}

		float3 Interpolation_C2(float3 x) { return x * x * x * (x * (x * 6.0 - 15.0) + 10.0); }

		//	http://briansharpe.files.wordpress.com/2011/11/perlinsample.jpg
		//
		float Perlin3D(float3 P)
		{
			//	establish our grid cell and unit position
			float3 Pi = floor(P);
			float3 Pf = P - Pi;
			float3 Pf_min1 = Pf - 1.0;

			//
			//	classic noise.
			//	requires 3 random values per point.  with an efficent hash function will run faster than improved noise
			//

			//	calculate the hash.
			//	( various hashing methods listed in order of speed )
			float4 hashx0, hashy0, hashz0, hashx1, hashy1, hashz1;
			FAST32_hash_3D(Pi, hashx0, hashy0, hashz0, hashx1, hashy1, hashz1);

			//	calculate the gradients
			float4 grad_x0 = hashx0 - 0.49999;
			float4 grad_y0 = hashy0 - 0.49999;
			float4 grad_z0 = hashz0 - 0.49999;
			float4 grad_x1 = hashx1 - 0.49999;
			float4 grad_y1 = hashy1 - 0.49999;
			float4 grad_z1 = hashz1 - 0.49999;
			float4 grad_results_0 = rsqrt(grad_x0 * grad_x0 + grad_y0 * grad_y0 + grad_z0 * grad_z0) * (float2(Pf.x, Pf_min1.x).xyxy * grad_x0 + float2(Pf.y, Pf_min1.y).xxyy * grad_y0 + Pf.zzzz * grad_z0);
			float4 grad_results_1 = rsqrt(grad_x1 * grad_x1 + grad_y1 * grad_y1 + grad_z1 * grad_z1) * (float2(Pf.x, Pf_min1.x).xyxy * grad_x1 + float2(Pf.y, Pf_min1.y).xxyy * grad_y1 + Pf_min1.zzzz * grad_z1);

			//	Classic Perlin Interpolation
			float3 blend = Interpolation_C2(Pf);
			float4 res0 = lerp(grad_results_0, grad_results_1, blend.z);
			float2 res1 = lerp(res0.xy, res0.zw, blend.y);
			float final = lerp(res1.x, res1.y, blend.x);
			final *= 1.1547005383792515290182975610039;		//	(optionally) scale things to a strict -1.0->1.0 range    *= 1.0/sqrt(0.75)
			return final;
		}
		float PerlinBillowed(float3 p, int octaves, float3 offset, float frequency, float amplitude, float lacunarity, float persistence)
		{
			float sum = 0;
			for (int i = 0; i < octaves; i++)
			{
				float h = 0;
				h = abs(Perlin3D((p + offset) * frequency));
				sum += h * amplitude;
				frequency *= lacunarity;
				amplitude *= persistence;
			}
			return sum;
		}

		ENDCG
	SubShader
	{
		Tags { "RenderType"="Opaque" "Queue"="Transparent" }
		LOD 100

		Pass
		{
		     ZWrite off
			 Blend [_Src] [_Des]
			 ColorMask RGB
			 AlphaToMask off
			 CGPROGRAM
// Upgrade NOTE: excluded shader from DX11; has structs without semantics (struct v2f members pos)

			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float4 uv : TEXCOORD0;
				float2 uv_alpha : TEXCOORD1;
			    float2 uv_alpha1 : TEXCOORD2;
				// add more vertex color
				float4 color :COLOR;
				float4 texcoord :TEXCOORD3;
				
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;				
				float2 uv_alpha1 : TEXCOORD1;
				float2 uv_alpha2 : TEXCOORD2;
				UNITY_FOG_COORDS(1)
				// add more vertex color
				float4 color :COLOR;
				float4 vertex : SV_POSITION;
				float3 pos :TEXCOORD3;
				
			};
			sampler2D _AlphaMap1;
			sampler2D _AlphaMap2;
			sampler2D _MainTex;
			sampler2D _SubTex;
			sampler2D _BlendMap;
		
			float4 _MoveAlpha;
			float4 _MainTex_ST;
			float4 _SubTex_ST;
			float4 _AlphaMap1_ST;
			float4 _AlphaMap2_ST;
			float _Blend;
			float _Timex;
			float _SpeedMainU, _SpeedSubU, _SpeedMainV, _SpeedSubV;
			float  _SpeedMainAlphaU, _SpeedSubAlphaU, _SpeedMainAlphaV, _SpeedSubAlphaV;
			float _SpeedBlendU, _SpeedBlendV;
			// for noise map
			fixed _Octaves;
			float _Frequency;
			float _Amplitude;
			float3 _Offset;
			float _Lacunarity;
			float _Persistence;
			float _AnimSpeed;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv.xy,_MainTex);				
				o.uv_alpha1 = TRANSFORM_TEX(v.uv_alpha, _AlphaMap1);
				o.uv_alpha2 = TRANSFORM_TEX(v.uv_alpha1, _AlphaMap2);
				o.pos = float3(v.texcoord.xy, _Time.y * _AnimSpeed);
				//pass vertex color
				o.color =v.color;
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				float h = 1; // PerlinBillowed(i.pos, _Octaves, _Offset, _Frequency, _Amplitude, _Lacunarity, _Persistence);
				
			  float4 color = float4(h, h, h, h);
				// sample the texture
				
				fixed2 _MainSpeed = fixed2(i.uv.x+_SpeedMainU*_Time.y,i.uv.y+_SpeedMainV*_Time.y);
				fixed2 _SubSpeed =  fixed2(i.uv.x +_SpeedSubU*_Time.y,i.uv.y+_SpeedSubV*_Time.y);
				fixed2 AlphaSpeed1 = fixed2(i.uv_alpha1.x+_SpeedMainAlphaU*_Time.y,i.uv_alpha1.y+_SpeedMainAlphaV*_Time.y);
				fixed2 AlphaSpeed2 = fixed2(i.uv_alpha2.x + _SpeedSubAlphaU *_Time.y,i.uv_alpha2.y+ _SpeedSubAlphaV *_Time.y);
					//Alpha1
				fixed4 Alpha1 =  tex2D(_AlphaMap1, AlphaSpeed1);
									//Alpha2
				fixed4 Alpha2 =  tex2D(_AlphaMap2, AlphaSpeed2);
					//BlendMap
				fixed4 Blendmap = tex2D(_BlendMap,i.uv.xy+float2(_SpeedBlendU*_Time.y,_SpeedBlendV*_Time.y));
				fixed4 MainColor = tex2D(_MainTex,_MainSpeed)*Alpha1;
				fixed4 SubColor = tex2D(_SubTex,_SubSpeed)*Alpha2;

				 // animate main alpha 
				 
					//Final
				fixed4 BlendColor =lerp(SubColor*color,MainColor,Blendmap*_Blend)*i.color.a;

			   fixed4 col =tex2D(_MainTex,i.uv);
				

				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, BlendColor);

				return BlendColor*i.color;
			}
			ENDCG
		}
	}
	CustomEditor "CustomGUIShader"
}
