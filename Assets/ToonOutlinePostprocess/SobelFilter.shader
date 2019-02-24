Shader "Unlit/SobelFilter"
{
	Properties 
	{
	    [HideInInspector]_MainTex ("Base (RGB)", 2D) = "white" {}
		_Delta ("Line Thickness", Range(0.0005, 0.0025)) = 0.001
		[Toggle(RAW_OUTLINE)]_Raw ("Outline Only", Float) = 0
		[Toggle(POSTERIZE)]_Poseterize ("Posterize", Float) = 0
		_PosterizationCount ("Count", int) = 8
	}
	SubShader 
	{
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		Pass
		{
            HLSLPROGRAM
            #include "Packages/com.unity.render-pipelines.lightweight/ShaderLibrary/SurfaceInput.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
            
            #pragma shader_feature RAW_OUTLINE
            #pragma shader_feature POSTERIZE
            
            sampler2D _CameraDepthTexture;
#ifndef RAW_OUTLINE
            sampler2D _MainTex;
#endif
            float _Delta;
            int _PosterizationCount;
            
            struct Attributes
            {
                float4 positionOS       : POSITION;
                float2 uv               : TEXCOORD0;
            };

            struct Varyings
            {
                float2 uv        : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };
            
            float sobel (sampler2D tex, float2 uv) 
            {
                float2 delta = float2(_Delta, _Delta);
                
                float4 hr = float4(0, 0, 0, 0);
                float4 vt = float4(0, 0, 0, 0);
                
                hr += tex2D(tex, (uv + float2(-1.0, -1.0) * delta)) *  1.0;
                hr += tex2D(tex, (uv + float2( 1.0, -1.0) * delta)) * -1.0;
                hr += tex2D(tex, (uv + float2(-1.0,  0.0) * delta)) *  2.0;
                hr += tex2D(tex, (uv + float2( 1.0,  0.0) * delta)) * -2.0;
                hr += tex2D(tex, (uv + float2(-1.0,  1.0) * delta)) *  1.0;
                hr += tex2D(tex, (uv + float2( 1.0,  1.0) * delta)) * -1.0;
                
                vt += tex2D(tex, (uv + float2(-1.0, -1.0) * delta)) *  1.0;
                vt += tex2D(tex, (uv + float2( 0.0, -1.0) * delta)) *  2.0;
                vt += tex2D(tex, (uv + float2( 1.0, -1.0) * delta)) *  1.0;
                vt += tex2D(tex, (uv + float2(-1.0,  1.0) * delta)) * -1.0;
                vt += tex2D(tex, (uv + float2( 0.0,  1.0) * delta)) * -2.0;
                vt += tex2D(tex, (uv + float2( 1.0,  1.0) * delta)) * -1.0;
                
                return sqrt(hr * hr + vt * vt).x;
            }
            
            Varyings vert(Attributes input)
            {
                Varyings output = (Varyings)0;

                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
                output.vertex = vertexInput.positionCS;
                output.uv = input.uv;
                
                return output;
            }
            
            half4 frag (Varyings input) : SV_Target 
            {
                float s = pow(1 - saturate(sobel(_CameraDepthTexture, input.uv)), 50);
#ifdef RAW_OUTLINE
                return half4(s.xxx, 1);
#else
                half4 col = tex2D(_MainTex, input.uv);
#ifdef POSTERIZE
                col = pow(col, 0.4545);
                half3 c = RgbToHsv(col);
                c.z = round(c.z * _PosterizationCount) / _PosterizationCount;
                col = half4(HsvToRgb(c), col.a);
                col = pow(col, 2.2);
#endif
                return col * s;
#endif
            }
            
			#pragma vertex vert
			#pragma fragment frag
			
			ENDHLSL
		}
	} 
	FallBack "Diffuse"
}
