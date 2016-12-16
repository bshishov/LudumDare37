Shader "Custom/NiceColorBlended"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}		
	}
	SubShader
	{
		Tags { "Queue" = "Transparent+1" "IgnoreProjector" = "True" "RenderType"="TransparentCutout" }
		LOD 200			
		Blend SrcAlpha OneMinusSrcAlpha
		AlphaTest Greater .01
		ColorMask RGB
		Cull Off 
		Lighting Off 
		ZWrite Off 
		Fog{ Color(0,0,0,0) }

		Pass
		{
			Lighting Off

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag			
			//#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				fixed4 color : COLOR;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
				fixed4 color : COLOR;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			fixed4 _TintColor;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.color = v.color;
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{				
				fixed4 col = tex2D(_MainTex, i.uv);				
				fixed a = col.r;
				fixed4 dist = fixed4(1,1,1,1) - col;
				fixed l = sqrt(length(dist));
				col.rgb = fixed3(1, 1, 1) * (1 - l) + i.color.xyz * l;
				//col.rgb = i.color.xyz;
				col.a = a;

				UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}
			ENDCG
		}
	}

	Fallback "Transparent/Cutout/VertexLit"
}
