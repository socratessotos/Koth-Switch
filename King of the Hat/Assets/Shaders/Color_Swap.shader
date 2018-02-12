Shader "Sprites/ColorSwap"
{
	Properties
	{

		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_SwapTex("Color Data", 2D) = "transparent" {}
		_Color ("Tint", Color) = (1,1,1,1)
		_OverrideColor ("Override", Color) = (1,1,1,0)
		[MaterialToggle] PixelSnap ("Pixel snap", Float) = 0

		_SliceGuide ("Slice Guide (RGB)", 2D) = "white" {}
		_SliceAmount ("Slice Amount", Range(0.0, 1.0)) = 0.5
		_BurnRamp ("Burn Ramp (RGB)", 2D) = "white" {}
		_BurnSize ("Burn Size", Range(0.0, 1.0)) = 0.15

		[PerRendererData] _Outline("Outline", Float) = 0
		[HDR][PerRendererData] _OutlineColor("Outline Color", Color) = (1,1,1,1)
		[PerRendererData] _OutlineSize("Outline Size", int) = 1

	}

	SubShader
	{
		Tags
		{ 
			"Queue"="Transparent" 
			"IgnoreProjector"="True" 
			"RenderType"="Transparent" 
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
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
			#pragma multi_compile _ ETC1_EXTERNAL_ALPHA
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
				fixed4 color    : COLOR;
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
				#ifdef PIXELSNAP_ON
				OUT.vertex = UnityPixelSnap (OUT.vertex);
				#endif

				return OUT;
			}

			sampler2D _MainTex;
			sampler2D _AlphaTex;
			sampler2D _SwapTex;

			float4 _MainTex_TexelSize;

			fixed4 SampleSpriteTexture (float2 uv)
			{
				fixed4 color = tex2D (_MainTex, uv);

				#if ETC1_EXTERNAL_ALPHA
				// get the color from an external texture (usecase: Alpha support for ETC1 on android)
				color.a = tex2D (_AlphaTex, uv).r;
				#endif //ETC1_EXTERNAL_ALPHA

				return color;
			}

			fixed4 frag(v2f IN) : SV_Target
			{

				clip(tex2D (_SliceGuide, IN.texcoord).rgb - _SliceAmount);
				half4 c = SampleSpriteTexture (IN.texcoord);

//				// If outline is enabled and there is a pixel, try to draw an outline.
//				if (_Outline > 0 && c.a != 0) 
//				{
//					float totalAlpha = 1.0;
//
//					[unroll(16)]
//					for (int i = 1; i < _OutlineSize + 1; i++) 
//					{
//						fixed4 pixelUp = tex2D(_MainTex, IN.texcoord + fixed2(0, i * _MainTex_TexelSize.y));
//						fixed4 pixelDown = tex2D(_MainTex, IN.texcoord - fixed2(0, i *  _MainTex_TexelSize.y));
//						fixed4 pixelRight = tex2D(_MainTex, IN.texcoord + fixed2(i * _MainTex_TexelSize.x, 0));
//						fixed4 pixelLeft = tex2D(_MainTex, IN.texcoord - fixed2(i * _MainTex_TexelSize.x, 0));
//
//						totalAlpha = totalAlpha * pixelUp.a * pixelDown.a * pixelRight.a * pixelLeft.a;
//					}
//
//					if (totalAlpha == 0) 
//
//					{	
//						c.rgba = _OutlineColor;
//					}
//					
//				}
			
				fixed4 swapCol = tex2D(_SwapTex, float2(c.r, 0));
				fixed4 final = lerp(c, swapCol, swapCol.a);
    			final = lerp(final, _OverrideColor, _OverrideColor.a);

		    	half test = tex2D (_SliceGuide, IN.texcoord).rgb - _SliceAmount;

		        if(test < _BurnSize && _SliceAmount > 0 && _SliceAmount < 1){

		        	fixed4 burn = tex2D(_BurnRamp, float2(test * (1/_BurnSize), 0));
		        	final = burn;

		        }

		        final.a = c.a;
    			final.rgb *= c.a;
 
    			return final;

			}

		ENDCG

		}

	}

}
