Shader "Sprites/PaletteSwap"
{
	Properties
	{
		
		[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
		_SwapTex("Color Data", 2D) = "transparent" {}

		//Color Effects Layer
		_EffectsLayer1Color("Tint", Color) = (1,1,1,1)
		_EffectsLayer1OverrideColor("Override", Color) = (1,1,1,0)
		_EffectsLayer1SliceGuide("Slice Guide (RGB)", 2D) = "white" {}
		_EffectsLayer1SliceAmount("Slice Amount", Range(0.0, 1.0)) = 0.5

		[MaterialToggle] PixelSnap("Pixel snap", Float) = 0

		//Slice Effects Layer
		_EffectsLayer2SliceGuide("Slice Guide (RGB)", 2D) = "white" {}
		_EffectsLayer2SliceAmount("Slice Amount", Range(0.0, 1.0)) = 0.5

		//Burn Effects Layer
		_BurnEffectsLayerBurnRamp("Burn Ramp (RGB)", 2D) = "white" {}
		_BurnEffectsLayerBurnSize("Burn Size", Range(0.0, 1.0)) = 0.15

		[PerRendererData] _Outline("Outline", Float) = 0
		[HDR][PerRendererData] _OutlineColor("Outline Color", Color) = (1,1,1,1)
		[PerRendererData] _OutlineSize("Outline Size", int) = 1

	}
	SubShader
	{
		Tags { 
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
			#pragma multi_compile _ PIXELSNAP_ON
			//#pragma multi_compile _ ETC1_EXTERNAL_ALPHA
			
			#pragma shader_feature EFFECTS_LAYER_1_ON EFFECTS_LAYER_1_OFF
			#pragma shader_feature EFFECTS_LAYER_2_ON EFFECTS_LAYER_2_OFF
			#pragma shader_feature BURN_EFFECTS_LAYER_ON BURN_EFFECTS_LAYER_OFF

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

#if EFFECTS_LAYER_1_ON	
				float2 colorEffectUV : TEXCOORD1;
#endif
#if EFFECTS_LAYER_2_ON	
				float2 sliceEffectUV : TEXCOORD2;
#endif
#if BURN_EFFECTS_LAYER_ON	
				float2 burnEffectUV : TEXCOORD3;
#endif

			};

			fixed4 _Color;
			fixed4 _OverrideColor;

			sampler2D _SliceGuide;
			float _SliceAmount;
			sampler2D _BurnRamp;
			float _BurnSize;
			
			v2f vert(appdata_t IN)
			{
				v2f OUT;
				OUT.vertex = UnityObjectToClipPos(IN.vertex);
				OUT.texcoord = IN.texcoord;
#if EFFECTS_LAYER_1_ON
				OUT.color = IN.color * _Color;
#endif
#ifdef PIXELSNAP_ON
				OUT.vertex = UnityPixelSnap(OUT.vertex);
#endif

				return OUT;
			}
			
			sampler2D _MainTex;
			sampler2D _AlphaTex;
			sampler2D _SwapTex;

			float4 _MainTex_TexelSize;

			fixed4 SampleSpriteTexture(float2 uv)
			{
				fixed4 color = tex2D(_MainTex, uv);

#if ETC1_EXTERNAL_ALPHA
				// get the color from an external texture (usecase: Alpha support for ETC1 on android)
				//color.a = tex2D (_AlphaTex, uv).r;
#endif //ETC1_EXTERNAL_ALPHA

				return color;
			}

			fixed4 frag(v2f IN) : SV_Target
			{

#if EFFECTS_LAYER_2_ON
				clip(tex2D(_SliceGuide, IN.texcoord).rgb - _SliceAmount);
#endif

				half4 c = SampleSpriteTexture(IN.texcoord);

				fixed4 swapCol = tex2D(_SwapTex, float2(c.r, 0));
				fixed4 final = lerp(c, swapCol, swapCol.a);
				final = lerp(final, _OverrideColor, _OverrideColor.a);

#if EFFECTS_LAYER_2_ON
				half test = tex2D(_SliceGuide, IN.texcoord).rgb - _SliceAmount;
#endif

#if BURN_EFFECTS_LAYER_ON
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
	CustomEditor "PaletteMaterialEditor"
}
