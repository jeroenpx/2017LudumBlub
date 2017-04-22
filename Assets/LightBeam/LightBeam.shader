Shader "GameOfColor/Light/LightBeam"
{
	Properties
	{
		_MainTex ("LightBeamText (BW)", 2D) = "white" {}
		
		_ColorMult ("Color Multiplication (RGBA)", Color) = (1,1,1,.8)
	}
	SubShader
	{
		Tags { "Queue"="Transparent" "RenderType"="Transparent" }
		LOD 100
		
		Blend SrcAlpha OneMinusSrcAlpha
		
		ZWrite Off

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _ColorMult;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// Get the right position in the texture...
				float2 textureTimeSample = float2(_Time.x, i.uv.y);
			
				// sample the texture
				fixed4 col = tex2D(_MainTex, textureTimeSample);
				
				col = fixed4(1, 1, 1, clamp(col.r*(1-i.uv.x), 0, 1));
				col = col*_ColorMult;
							
				return col;
			}
			ENDCG
		}
	}
}
