// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "UGameTech/F_Effect_T"
{
	Properties
	{
		_TintColor("Tint Color", Color)		= (1.0,1.0,1.0,1.0)
		_MainTex("Base (RGB)", 2D)			= "white" {}
		_ColorFactor("Color Factor", Float) = 2.0 
	}

	SubShader
	{
		LOD 200

		Tags{ "Queue" = "Transparent+5" "IgnoreProjector" = "True" "RenderType" = "Transparent" }

		Pass
		{
			ZWrite		Off
			Cull		Off
			Blend		SrcAlpha	OneMinusSrcAlpha

			CGPROGRAM

		#include "UnityCG.cginc"
		#pragma vertex		vert
		#pragma fragment	frag


			struct appdata_t
			{
				half4	vertex 	: POSITION;
				half4	color	: COLOR;
				half2	texcoord: TEXCOORD0;
			};

			struct v2f
			{
				float4	pos		: SV_POSITION;
				half4	color	: COLOR;
				float2	uv		: TEXCOORD0;
				UNITY_FOG_COORDS(1)
			};

			sampler2D	_MainTex;
			half4		_MainTex_ST;
			float4 		_TintColor;
			half		_ColorFactor;

			v2f vert(appdata_t v)
			{
				//> 这里传入的顶点和法线是骨骼空间的，要做世界空间转换
				//> ----------------------------------------------------------------
				v2f o;
				o.pos	= UnityObjectToClipPos(v.vertex);
				o.uv	= TRANSFORM_TEX(v.texcoord, _MainTex);
				o.color = v.color;

				return o;
			}

			float4 frag(v2f IN) : COLOR
			{
				half4 finalColor= tex2D(_MainTex, IN.uv) * IN.color * _TintColor * _ColorFactor;

				return finalColor;
			}

			ENDCG
		}
	}

	//Fallback "Transparent/VertexLit"
}
