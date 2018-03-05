Shader "Sprites/Swapper"
{
	Properties
	{

		[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}

		_SwapTex("Color Data", 2D) = "transparent" {}

		_Color("Tint Color", Color) = (1,1,1,1)
		_OverrideColor("Override Color", Color) = (1,1,1,0)

		[MaterialToggle] PixelSnap("Pixel snap", Float) = 0

		_SliceGuide("Slice Guide (RGB)", 2D) = "white" {}
		_SliceAmount("Slice Amount", Range(0.0, 1.0)) = 0.5

		_BurnRamp("Burn Ramp (RGB)", 2D) = "white" {}
		_BurnSize("Burn Size", Range(0.0, 1.0)) = 0.15

	}

		SubShader
	{
		Tags
	{
		"Queue" = "Transparent"
		"IgnoreProjector" = "True"
		"RenderType" = "Transparent"
		"PreviewType" = "Plane"
		"CanUseSpriteAtlas" = "True"
	}

		Cull Off
		Lighting Off
		ZWrite Off
		Blend One OneMinusSrcAlpha

		Pass
	{
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		#pragma target 2.0
#pragma shader_feature COLOR_ON COLOR_OFF
#pragma shader_feature SLICE_ON SLICE_OFF
#pragma shader_feature BURN_ON BURN_OFF

		#include "UnityCG.cginc"

		struct appdata_t
	{
		float4 vertex   : POSITION;
		float4 color    : COLOR;
		float2 texcoord : TEXCOORD0;
	};

	struct v2f
	{
		float4 vertex   : SV_POSITION;
		fixed4 color : COLOR;
		float2 texcoord  : TEXCOORD0;
	};

	fixed4 _Color;
	fixed4 _OverrideColor;

	sampler2D _SliceGuide;
	float _SliceAmount;
	sampler2D _BurnRamp;
	float _BurnSize;

	float _Outline;
	fixed4 _OutlineColor;
	int _OutlineSize;

	v2f vert(appdata_t IN)
	{
		v2f OUT;
		OUT.vertex = UnityObjectToClipPos(IN.vertex);
		OUT.texcoord = IN.texcoord;
		OUT.color = IN.color * _Color;

		return OUT;
	}

	sampler2D _MainTex;
	sampler2D _AlphaTex;
	sampler2D _SwapTex;

	float4 _MainTex_TexelSize;

	fixed4 SampleSpriteTexture(float2 uv)
	{
		fixed4 color = tex2D(_MainTex, uv);

		return color;
	}

	fixed4 frag(v2f IN) : SV_Target
	{

#if SLICE_ON
	clip(tex2D(_SliceGuide, IN.texcoord).rgb - _SliceAmount);
#endif

	half4 c = SampleSpriteTexture(IN.texcoord);

	fixed4 swapCol = tex2D(_SwapTex, float2(c.r, 0));
	fixed4 final = lerp(c, swapCol, swapCol.a);
	final = lerp(final, _OverrideColor, _OverrideColor.a);

#if SLICE_ON || BURN_ON	
	half test = tex2D(_SliceGuide, IN.texcoord).rgb - _SliceAmount;
#endif

#if BURN_ON
	if (test < _BurnSize && _SliceAmount > 0 && _SliceAmount < 1) {

		fixed4 burn = tex2D(_BurnRamp, float2(test * (1 / _BurnSize), 0));
		final = burn;

	}
#endif

	final.a = c.a;
	final.rgb *= c.a;

	return final;

	}

		ENDCG

	}

	}
	CustomEditor "SwapperToggleInspector"
}
