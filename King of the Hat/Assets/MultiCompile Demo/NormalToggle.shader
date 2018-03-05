Shader "Multi-CompileTutorial/NormalToggle"
{
	Properties
	{
		_MainTex("Base (RGB)", 2D) = "white" {}
	_NormalMap("Normal Map", 2D) = "bump" {}
	}

		SubShader
	{
		Tags{ "RenderType" = "Opaque" }
		LOD 300

		CGPROGRAM
#pragma surface surf Lambert
#pragma shader_feature NORMALMAP_ON NORMALMAP_OFF

		sampler2D _MainTex;
	sampler2D _NormalMap;

	struct Input
	{
		float2 uv_MainTex;
		float2 uv_NormalMap;
	};

	void surf(Input IN, inout SurfaceOutput o)
	{
		fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
		o.Albedo = c.rgb;
		o.Alpha = c.a;
#if NORMALMAP_ON
		o.Normal = UnpackNormal(tex2D(_NormalMap, IN.uv_NormalMap));
#endif
	}
	ENDCG
	}
		CustomEditor "NormalToggleInspector"
}