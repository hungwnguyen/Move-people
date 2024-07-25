Shader "Paint in 3D/Solid" {
	Properties {
		[NoScaleOffset] _MainTex ("Albedo (RGB) Alpha (A)", 2D) = "white" {}
		[NoScaleOffset] [Normal] _BumpMap ("Normal (RGBA)", 2D) = "bump" {}
		[NoScaleOffset] _MetallicGlossMap ("Metallic (R) Occlusion (G) Smoothness (B)", 2D) = "white" {}
		[NoScaleOffset] _EmissionMap ("Emission (RGB)", 2D) = "white" {}
		_Color ("Color", Vector) = (1,1,1,1)
		_BumpScale ("Normal Map Strength", Range(0, 5)) = 1
		_Metallic ("Metallic", Range(0, 1)) = 0
		_GlossMapScale ("Smoothness", Range(0, 1)) = 1
		_Emission ("Emission", Vector) = (0,0,0,1)
		_Tiling ("Tiling", Float) = 1
		[Toggle(_USE_UV2)] _UseUV2 ("Use Second UV", Float) = 0
		[Header(OVERRIDE SETTINGS)] [Toggle(_USE_UV2_ALT)] _UseUV2Alt ("	Use Second UV", Float) = 1
		[Toggle(_OVERRIDE_OPACITY)] _EnableOpacity ("	Enable Opacity", Float) = 0
		[Toggle(_OVERRIDE_NORMAL)] _EnableNormal ("	Enable Normal", Float) = 0
		[Toggle(_OVERRIDE_MOS)] _EnableMos ("	Enable MOS", Float) = 0
		[Toggle(_OVERRIDE_EMISSION)] _EnableEmission ("	Enable Emission", Float) = 0
		[Header(OVERRIDES)] [NoScaleOffset] _AlbedoTex ("	Premultiplied Albedo (RGB) Weight (A)", 2D) = "black" {}
		[NoScaleOffset] _OpacityTex ("	Premultiplied Opacity (R) Weight (A)", 2D) = "black" {}
		[NoScaleOffset] _NormalTex ("	Premultiplied Normal (RG) Weight (A)", 2D) = "black" {}
		[NoScaleOffset] _MosTex ("	Premultiplied Metallic (R) Occlusion (G) Smoothness (B) Weight (A)", 2D) = "black" {}
		[NoScaleOffset] _EmissionTex ("	Premultiplied Emission (RGB) Weight (A)", 2D) = "black" {}
	}
	//DummyShaderTextExporter
	SubShader{
		Tags { "RenderType"="Opaque" }
		LOD 200
		CGPROGRAM
#pragma surface surf Standard
#pragma target 3.0

		sampler2D _MainTex;
		fixed4 _Color;
		struct Input
		{
			float2 uv_MainTex;
		};
		
		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	}
}