// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Sprites/AddSmooth"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
		[MaterialToggle] _OutlineToggle ("Outline", Float) = 0
		_OutlineColor ("Outline", Color) = (1,1,1,1)
		_Multiplier ("Multiplier", Float) = 1
		[MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
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
		Fog { Mode Off }
		Blend SrcAlpha OneMinusSrcAlpha
		ColorMask RGB

		Pass
		{
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile DUMMY PIXELSNAP_ON
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
				half2 texcoord  : TEXCOORD0;
			};
			
			fixed4 _Color;

			v2f vert(appdata_t IN)
			{
				v2f OUT;
				OUT.vertex = UnityObjectToClipPos(IN.vertex);
				OUT.texcoord = IN.texcoord;
				OUT.color = IN.color * _Color;
				#ifdef PIXELSNAP_ON
				OUT.vertex = UnityPixelSnap(OUT.vertex);
				#endif

				return OUT;
			}

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _MainTex_TexelSize;
			fixed4 _OutlineColor;
			fixed _Multiplier;
			fixed _OutlineToggle;

			fixed4 frag(v2f IN) : COLOR
			{
				fixed4 col = IN.color * tex2D(_MainTex, IN.texcoord);
				col.rgb *= _Multiplier;

				float2 up = float2(0, _MainTex_TexelSize.y);
				float2 right = float2(_MainTex_TexelSize.x, 0);
				fixed pixelLeft = tex2D(_MainTex, IN.texcoord - right).a;
				fixed pixelRight = tex2D(_MainTex, IN.texcoord + right).a;
				fixed pixelUp = tex2D(_MainTex, IN.texcoord + up).a;
				fixed pixelDown = tex2D(_MainTex, IN.texcoord - up).a;

				// inner line
				fixed outline = (1 - pixelLeft * pixelRight * pixelUp * pixelDown) * col.a;

				// outline
				// fixed outline = max(max(pixelLeft, pixelRight), max(pixelUp, pixelDown)) - col.a;

				return lerp(col, _OutlineColor, outline * _OutlineToggle);
			}
		ENDCG
		}
	}
}
