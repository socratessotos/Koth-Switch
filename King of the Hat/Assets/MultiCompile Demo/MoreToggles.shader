Shader "Multi-CompileTutorial/MoreToggles"
{
	Properties
	{
		_DiffuseColor("Diffuse Colour",color) = (1.0,1.0,1.0,1.0)
		_DiffuseMultiply("Diffuse Brightness",float) = 1.0
		_DiffuseMap("Diffuse (RGB)", 2D) = "white" {}

	_NormalMap("Normal Map(RGB)", 2D) = "bump" {}

	_SpecColor("Specular Color", Color) = (0.5, 0.5, 0.5, 1)
		_SpecularMultiply("Specular Brightness",float) = 1.0
		_SpecAdd("Specular Boost", float) = 0
		_SpecMap("Specular Map (RGB)", 2D) = "grey" {}
	_Gloss("Specular Glossiness", float) = 0.5

		_FresnelPower("Fresnel Power",float) = 1.0
		_FresnelMultiply("Fresnel Multiply", float) = 0.2
		_FresnelBias("Fresnel Bias", float) = -0.1

		_RimPower("RimLight Power",float) = 1.0
		_RimMultiply("RimLight Multiply", float) = 0.2
		_RimBias("RimLight Bias", float) = 0
	}

		SubShader
	{
		Tags{ "RenderType" = "Opaque" }
		LOD 300

		CGPROGRAM
#pragma surface surf BlinnPhong
#pragma target 3.0
#pragma shader_feature NORMALMAP_ON NORMALMAP_OFF
#pragma shader_feature SPECULAR_ON SPECULAR_OFF
#pragma shader_feature FRESNEL_ON FRESNEL_OFF
#pragma shader_feature RIMLIGHT_ON RIMLIGHT_OFF

		float3 _DiffuseColor;
	float _DiffuseMultiply;
	sampler2D _DiffuseMap;

	sampler2D _NormalMap;

	float _SpecularMultiply;
	float _SpecAdd;
	sampler2D _SpecMap;

	float _Gloss;

	float _FresnelPower;
	float _FresnelMultiply;
	float _FresnelBias;

	float _RimPower;
	float _RimMultiply;
	float _RimBias;

	struct Input
	{
		float2 uv_DiffuseMap;
#if SPECULAR_ON
		float2 uv_SpecMap;
#endif
#if NORMALMAP_ON
		float2 uv_NormalMap;
#endif
#if FRESNEL_ON || RIMLIGHT_ON
		float3 viewDir;
#endif
	};

	void surf(Input IN, inout SurfaceOutput o)
	{
		o.Albedo.rgb = _DiffuseMultiply * _DiffuseColor.rgb * tex2D(_DiffuseMap, IN.uv_DiffuseMap);

#if SPECULAR_ON
		o.Specular = _Gloss;
		o.Gloss = _SpecAdd + _SpecularMultiply * tex2D(_SpecMap, IN.uv_SpecMap);
#endif

#if NORMALMAP_ON
		o.Normal = UnpackNormal(tex2D(_NormalMap, IN.uv_NormalMap));
#endif

#if FRESNEL_ON && SPECULAR_ON || RIMLIGHT_ON
		float facing = saturate(1.0 - max(dot(normalize(IN.viewDir.xyz), normalize(o.Normal)), 0.0));

#if FRESNEL_ON && SPECULAR_ON
		float fresnel = max(_FresnelBias + (1.0 - _FresnelBias) * pow(facing, _FresnelPower), 0);
		fresnel = fresnel * o.Specular * _FresnelMultiply;
		o.Gloss *= 1 + fresnel;
#endif          

#if RIMLIGHT_ON
		float rim = max(_RimBias + (1.0 - _RimBias) * pow(facing, _RimPower), 0);
		rim = rim * o.Specular * _RimMultiply;
		o.Albedo *= 1 + rim;
#endif
#endif
	}
	ENDCG
	}
		CustomEditor "MoreTogglesInspector"
}