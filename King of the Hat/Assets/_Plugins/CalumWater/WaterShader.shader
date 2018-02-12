// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Calum/Water"
{
	Properties
	{
		_Input("Input Texture", 2D) = "white" {}
		_Height("Image Height", float) = 0
		_Divider("Division Variable", float) = 1
		_ImgMin("Image on Screen Mimimum", float) = 0

		_Disp("Displacement Texture", 2D) = "white" {}
		_Mag("Magitude", Range(0, 0.1)) = 1

		_Tint("Tint Color", Color) = (1, 1, 1, 1)
	}

	SubShader
	{
		Tags
		{
			"Queue" = "Transparent"
			"PreviewType" = "Plane"
		}

		Pass
		{
			ZWrite Off
			Cull Off
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				half4 color : COLOR;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
				float2 screenuv : TEXCOORD1;
				half4 color : COLOR;
			};

			v2f vert(appdata v)
			{
				v2f o;

				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				o.screenuv = (o.vertex.xy / o.vertex.w + 1) * 0.5;
				o.color = v.color;

				return o;
			}

			uniform sampler2D _GlobalReflectionTex;
			float _Height;
			float _Divider;
			float _ImgMin;

			sampler2D _Disp;
			float _Mag;

			float4 _Tint;
			sampler2D _Input;

			float4 frag(v2f i) : SV_Target
			{
				float2 distuv = float2(i.screenuv.x + _Time.x * 2, i.uv.y + _Time.x * 2);
				float2 disp = tex2D(_Disp, distuv).xy;
				disp = ((disp * 2) - 1) * _Mag;

				// Get uv
				float2 newUV = i.screenuv;

				// Flip UV 'y' coord
				newUV.y = (1 - ((newUV.y - _ImgMin) * _Divider)) * (_ImgMin + _Height);

				// Get coord above shape
				newUV.y = newUV.y + _Height;

				// Get color
				float4 c = tex2D(_Input, i.uv);
				float4 output = tex2D(_GlobalReflectionTex, newUV + disp);
				output = output * _Tint * c;
				return output;

			}
			ENDCG
		}
	}
}