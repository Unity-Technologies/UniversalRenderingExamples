Shader "Hidden/BasicBlit"
{
	Properties
	{
		_BlitTex("BlitTexture", 2D) = "white" {}
        _Alpha("Alpha", Float) = 0.5
	}
	SubShader
	{
		Tags { "RenderPipeline" = "UniversalPipeline" }
		
		Pass
	    {
	        Blend SrcAlpha OneMinusSrcAlpha
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			
			TEXTURE2D(_BlitTex);
			SAMPLER(sampler_BlitTex);
            half _Alpha;
			
			struct VertexOutput
			{
			    float4 positionCS : SV_POSITION;
			    float2 uv0 : TEXCOORD0;
			};
			
			VertexOutput vert (float4 positionOS : POSITION, float2 uv0 : TEXCOORD0)
			{
			    VertexOutput OUT;
				OUT.positionCS = TransformObjectToHClip(positionOS.xyz);
				OUT.uv0 = uv0;
				return OUT;
			}
			
			half4 frag (VertexOutput IN) : SV_Target
			{
			    half3 color = SAMPLE_TEXTURE2D(_BlitTex, sampler_BlitTex, IN.uv0).rgb;
			    return half4(color, _Alpha);
			}
			ENDHLSL
		}
	}
}