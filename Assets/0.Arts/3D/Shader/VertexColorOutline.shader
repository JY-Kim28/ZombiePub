Shader "JOJO_Custom/VertexAlphaOutline"
{
    Properties
    {
        _BaseMap ("Base Texture", 2D) = "white" {}
        _BaseColor ("Base Color", Color) = (1,1,1,1)  
        _OutlineColor ("Outline Color", Color) = (0,0,0,1)
        _OutlineWidth ("Outline Width", Range(0,0.1)) = 0.02
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Cull Off  // 앞뒤 면 모두 렌더링

        // **아웃라인 패스 (Vertex Alpha 기반)**
        Pass
        {
            Cull Front  // 앞면 제거 -> 뒷면(아웃라인)만 보이게
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            struct appdata_t
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 color : COLOR; // Vertex Color (RGB + Alpha)
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
            };

            float4 _OutlineColor;
            float _OutlineWidth;

            v2f vert(appdata_t v)
            {
                v2f o;

                // Vertex Alpha 값이 1 이상일 때만 아웃라인 적용
                if (v.color.a > 0.5)
                {
                    float outlineThickness = _OutlineWidth;
                    v.vertex.xyz += v.normal * outlineThickness; // 노멀 방향으로 확장
                }

                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                return _OutlineColor; // 아웃라인 컬러 적용
            }
            ENDCG
        }

        // **기본 모델 렌더링 (Base Map + Vertex Color)**
        Pass
        {
            Cull Back  // 뒷면 제거
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            sampler2D _BaseMap;
            float4 _BaseColor;

            v2f vert(appdata_t v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 texColor = tex2D(_BaseMap, i.uv);
                return texColor * _BaseColor * i.color; // 텍스처 × 기본 색상 × 버텍스 컬러
            }
            ENDCG
        }
    }
}
