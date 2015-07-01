﻿Shader "Custom/ColoredShader" {
	SubShader {
		Tags
		{ 
			"Queue"="Transparent" 
			"IgnoreProjector"="True" 
			"RenderType"="Opaque" 
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
		}
		
		Pass {
			Cull Off
			Lighting Off
			ZWrite Off
			Fog { Mode Off }
		
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			
			struct v2f {
				float4 pos : SV_POSITION;
				float3 color : COLOR0;
			};
			
			v2f vert (appdata_full v) 
			{
				v2f o;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.color = v.color;
				return o;
			}
			
			half4 frag (v2f i) : COLOR
			{
				return half4(i.color, 1);
			}
		
			ENDCG	
		}
	} 
	
	FallBack "VertexLit"
}
