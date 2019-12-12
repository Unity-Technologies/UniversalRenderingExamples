// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Noise-Simplex" {
Properties {
	_Freq ("Frequency", Float) = 1
	_Speed ("Speed", Float) = 1
}

SubShader {
	Pass {
		CGPROGRAM
		
		#pragma target 3.0
		
		#pragma vertex vert
		#pragma fragment frag
		
		#include "noiseSimplex.cginc"
		
		struct v2f {
			float4 pos : SV_POSITION;
			float3 srcPos : TEXCOORD0;
		};
		
		uniform float
			_Freq,
			_Speed
		;
		
		v2f vert(float4 objPos : POSITION)
		{
			v2f o;

			o.pos =	UnityObjectToClipPos(objPos);
			
			o.srcPos = mul(unity_ObjectToWorld, objPos).xyz;
			o.srcPos *= _Freq;
			o.srcPos.y += _Time.y * _Speed;
			
			return o;
		}
		
		float4 frag(v2f i) : COLOR
		{
			float ns = snoise(i.srcPos) / 2 + 0.5f;
			return float4(ns, ns, ns, 1.0f);
		}
		
		ENDCG
	}
}

}