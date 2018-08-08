Shader "GUI/GradientFill"
{
	Properties
	{
		_Color("Color", Color) = (1, 1, 1, 1)
		_Y("Y", Float) = 0
	}

	SubShader 
	{
		Tags 
		{
		    "Queue" = "Transparent"
		    "IgnoreProjector" = "True"
		    "RenderType" = "Transparent"
		    "PreviewType" = "Plane"
	    }

		Lighting Off 
		Cull Off 
		ZTest Always 
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha

		Pass {
		
		    CGPROGRAM

#pragma vertex vert
#pragma fragment frag

#include "UnityCG.cginc"

	        struct appdata_t
	        {
		        float4 vertex : POSITION;
		        fixed4 color : COLOR;
		        float2 texcoord : TEXCOORD0;
	        };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
                float2 texcoord : TEXCOORD0; // 0..1
                float4 worldPosition : TEXCOORD1;
            };

            uniform fixed4 _Color;
            uniform float _Y;        
        
            v2f vert(appdata_t v)
            {
                v2f o;
                o.worldPosition = v.vertex;
                o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
                o.color = _Color;
                o.texcoord = v.texcoord;
                return o;
            }
        
            fixed4 frag(v2f i) : SV_Target
            {
                if (i.texcoord.y > _Y)
                    return half4(0, 0, 0, 0);
                    
                // Clipping is not working. 
                // Details: 
                // Default Unity shader(UI-Default.shader) is using UnityGet2DClipping to check clipping, but
                // there is variable _ClipRect which is used by that clipping function. I just don't know 
                // where they updating it.
                // More interesting why clipping in default shader is not working too (when I change it in material inspector).
                // Right now clipping is managed by Chart control.
                return half4(i.color.rgb, i.texcoord.y); 
            }

		    ENDCG
	    }
	}
}
