Shader "WorldMask" {
Properties {
    _TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
    _MainTex ("Texture", 2D) = "white" {}
    _MaskTex ("Mask Texture", 2D) = "white" {}
}

Category {
    Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" }
    Blend SrcAlpha OneMinusSrcAlpha
    ColorMask RGB
    Cull Off Lighting Off ZWrite Off

    SubShader {
        Pass {

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0

            #include "UnityCG.cginc"

            sampler2D _MainTex;
            fixed4 _TintColor;
            sampler2D _MaskTex;
            uniform float4x4  _ObjectMatrix;
            struct appdata_t {
                float4 vertex : POSITION;
                fixed4 color : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f {
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
                float2 texcoord : TEXCOORD0;
                float2 maskTexcoord : TEXCOORD1;
            };

            float4 _MainTex_ST;
            float4 _MaskTex_ST;

            v2f vert (appdata_t v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.color = v.color * _TintColor;
                o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);

                float4 worldUV = mul(unity_ObjectToWorld, v.vertex);
                worldUV = mul(_ObjectMatrix, worldUV);
                o.maskTexcoord = worldUV.xy;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 texColor = tex2D(_MainTex, i.texcoord);
                fixed4 col = i.color * texColor;
                
                if(i.maskTexcoord.x > 0 && i.maskTexcoord.x < 1 && i.maskTexcoord.y > 0 && i.maskTexcoord.y < 1) {
                    float mask = tex2D(_MaskTex, i.maskTexcoord);
                    col.a *= mask;
                }
                return col;
            }
            ENDCG
        }
    }
}
}