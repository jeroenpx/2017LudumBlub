Shader "Custom/PortalShader" {
	Properties {
		_MainTex ("Off texture (RGBA)", 2D) = "white" {}
		_MainTexOn ("On texture (RGBA)", 2D) = "white" {}
		_GlowTex ("Glow texture (RGBA)", 2D) = "white" {}
		_TransitionTex ("Transition (R,G)", 2D) = "white" {}
		_TransGlowLength ("Glow transition length", Range(0,1)) = 0.03
		_TransMainLength ("Bg transition length", Range(0,1)) = 0.55
		_TransPos ("Transition position", Range(0,1)) = 0.45
		_InitTransPerc ("Initial transition percent", Range(0,1)) = 0.05
		_TransAll ("Overal transition", Range(0,1)) = 0.0
		_AlphaMultiplier ("Alpha multiplier", Range(0,1)) = 1.0
	}
	SubShader {
		Tags { "Queue"="Transparent" "RenderType"="Transparent" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf NoLighting alpha

		sampler2D _MainTex;
		sampler2D _MainTexOn;
		sampler2D _TransitionTex;
		sampler2D _GlowTex;
		float _TransGlowLength;
		float _TransMainLength;
		float _TransPos;
		float _InitTransPerc;
		float _TransAll;
		float _AlphaMultiplier;

		struct Input {
			float2 uv_MainTex;
		};

		fixed4 LightingNoLighting(SurfaceOutput s, fixed3 lightDir, fixed atten)
	     {
	         fixed4 c;
	         c.rgb = s.Albedo; 
	         c.a = s.Alpha;
	         return c;
	     }

		void surf (Input IN, inout SurfaceOutput o) {
			half4 c_off = tex2D (_MainTex, IN.uv_MainTex);
			half4 c_on = tex2D (_MainTexOn, IN.uv_MainTex);
			half4 c_transition = tex2D (_TransitionTex, IN.uv_MainTex);
			half4 c_glow = tex2D (_GlowTex, IN.uv_MainTex);
			float movingTransitionPosition = clamp(_TransPos*(1.0+_InitTransPerc)-_InitTransPerc, 0.0, 1.0);
			float initialTransitionPosition = clamp(_TransPos/_InitTransPerc, 0.0, 1.0);
			
			float transpercentglow = clamp(clamp ((movingTransitionPosition+_TransGlowLength-c_transition.r)/(_TransGlowLength), 0.0, 1.0)*c_transition.g+(1.0-c_transition.g), 0.0, 1.0);
			float transpercentmain = clamp(clamp ((movingTransitionPosition+_TransMainLength-c_transition.r)/(_TransMainLength), 0.0, 1.0)*c_transition.g+(1.0-c_transition.g), 0.0, 1.0);
			o.Albedo = lerp(lerp (c_off.rgb, c_on.rgb, (transpercentmain*initialTransitionPosition)*(1-_TransAll)+_TransAll), c_glow.rgb, c_glow.a*((transpercentglow*initialTransitionPosition)*(1-_TransAll)+_TransAll));
			o.Alpha = lerp (c_off.a*_AlphaMultiplier, c_on.a*_AlphaMultiplier, transpercentmain*initialTransitionPosition);
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
