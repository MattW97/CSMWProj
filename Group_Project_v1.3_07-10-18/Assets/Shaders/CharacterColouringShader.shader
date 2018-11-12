Shader "Custom/CharacterColoruingShader" {
	Properties {
		_Color1 ("ColorPrimary", Color) = (1,1,1,1)
		_Color2 ("ColorSecondary", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_MaskTex("Mask", 2D) = "white" {}
		_Normal("Normal", 2D) = "bump" {}
		_Smoothness("Smoothness", 2D) = "white" {}
		_Occlusion("Occlusion", 2D) = "white" {}
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex, _MaskTex, _Normal, _Smoothness, _Occlusion;
		fixed3 _Color1, _Color2;

		struct Input {
			float2 uv_MainTex;
			float2 uv_MaskTex;
			float2 uv_Normal;
			float2 uv_Smoothness;
			float2 uv_Occlusion;
		};

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_CBUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_CBUFFER_END

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex);
			float3 mask = tex2D(_MaskTex, IN.uv_MainTex);
			float cmask = min (1.0, mask.r + mask.g);
			float s = tex2D(_Smoothness, IN.uv_Smoothness);
			float ao = tex2D(_Occlusion, IN.uv_Occlusion);
			float4 n = tex2D(_Normal, IN.uv_Normal);

			c.rgb = c.rgb * (1 - cmask) + (_Color1 * mask.r) + (_Color2 * mask.g);

			o.Albedo = c.rgb;
			o.Normal = UnpackNormal(n);
			o.Smoothness = s;
			o.Occlusion = ao;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
