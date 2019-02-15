Shader "Unlit/SobelFilter"
{
	Properties 
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Delta ("Delta", Float) = 0.01
	}
	SubShader 
	{
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		Pass
		{
            HLSLPROGRAM
            #include "Packages/com.unity.render-pipelines.lightweight/ShaderLibrary/SurfaceInput.hlsl"
            
            sampler2D _CameraDepthTexture;
            sampler2D _MainTex;
            float _Delta;
            
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
                
                hr += LinearEyeDepth(tex2D(tex, (uv + float2(-1.0, -1.0) * delta)), _ZBufferParams) *  1.0;
                hr += LinearEyeDepth(tex2D(tex, (uv + float2( 1.0, -1.0) * delta)), _ZBufferParams) * -1.0;
                hr += LinearEyeDepth(tex2D(tex, (uv + float2(-1.0,  0.0) * delta)), _ZBufferParams) *  2.0;
                hr += LinearEyeDepth(tex2D(tex, (uv + float2( 1.0,  0.0) * delta)), _ZBufferParams) * -2.0;
                hr += LinearEyeDepth(tex2D(tex, (uv + float2(-1.0,  1.0) * delta)), _ZBufferParams) *  1.0;
                hr += LinearEyeDepth(tex2D(tex, (uv + float2( 1.0,  1.0) * delta)), _ZBufferParams) * -1.0;
                
                //hr = LinearEyeDepth(hr, _ZBufferParams);
                
                vt += LinearEyeDepth(tex2D(tex, (uv + float2(-1.0, -1.0) * delta)), _ZBufferParams) *  1.0;
                vt += LinearEyeDepth(tex2D(tex, (uv + float2( 0.0, -1.0) * delta)), _ZBufferParams) *  2.0;
                vt += LinearEyeDepth(tex2D(tex, (uv + float2( 1.0, -1.0) * delta)), _ZBufferParams) *  1.0;
                vt += LinearEyeDepth(tex2D(tex, (uv + float2(-1.0,  1.0) * delta)), _ZBufferParams) * -1.0;
                vt += LinearEyeDepth(tex2D(tex, (uv + float2( 0.0,  1.0) * delta)), _ZBufferParams) * -2.0;
                vt += LinearEyeDepth(tex2D(tex, (uv + float2( 1.0,  1.0) * delta)), _ZBufferParams) * -1.0;
                
                //vt = LinearEyeDepth(vt, _ZBufferParams);
                
                return sqrt(hr * hr + vt * vt);
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
                float s = pow(sobel(_CameraDepthTexture, input.uv), 10);
                half4 col = tex2D(_MainTex, input.uv) * (1 - s);
                return col;
            }
            
			#pragma vertex vert
			#pragma fragment frag
			
			ENDHLSL
		}
	} 
	FallBack "Diffuse"
}
