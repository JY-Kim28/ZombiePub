Shader "JoJo/JoJo_ToonShader" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1, 1, 1, 1)
        _ShadeColor ("Shadow Color", Color) = (0.5, 0.5, 0.5, 1) // 이름 변경: Shade Color → Shadow Color
        _ShadowRange ("Shadow Range", Range(0.0, 1.0)) = 0.5
        _ShadowSoftness ("Shadow Softness", Range(0.0, 1.0)) = 0.5
        [Toggle] _EnableOutline ("Enable Outline", Float) = 1.0
        _OutlineColor ("Outline Color", Color) = (0, 0, 0, 1)
        _OutlineThickness ("Outline Thickness", Range(0.001, 0.03)) = 0.01
    }

    SubShader {
        Tags { "RenderType" = "Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Toon

        struct Input {
            float2 uv_MainTex;
        };

        sampler2D _MainTex;
        fixed4 _Color;
        fixed4 _ShadeColor;
        float _ShadowSoftness;
        float _ShadowRange;

        void surf(Input IN, inout SurfaceOutput o) {
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            o.Alpha = c.a;
        }

        half4 LightingToon(SurfaceOutput s, half3 lightDir, half3 viewDir, half atten) {
            half3 normal = normalize(s.Normal);
            half NdotL = dot(normal, lightDir);

            half3 litColor;
            if (_ShadowSoftness == 0) {
                if (NdotL > _ShadowRange) {
                    litColor = s.Albedo;
                } else {
                    litColor = _ShadeColor.rgb;
                }
            } else {
                if (NdotL > _ShadowRange + _ShadowSoftness) {
                    litColor = s.Albedo;
                } else if (NdotL > _ShadowRange) {
                    half t = (NdotL - _ShadowRange) / _ShadowSoftness;
                    litColor = lerp(_ShadeColor.rgb, s.Albedo, t);
                } else {
                    litColor = _ShadeColor.rgb;
                }
            }

            half3 result = litColor * atten;
            return half4(result, s.Alpha);
        }
        ENDCG

        Pass {
            Name "Outline"
            Tags { "LightMode" = "Always" }
            Cull Front
            ZWrite On
            ColorMask RGB

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            float _OutlineThickness;
            fixed4 _OutlineColor;
            float _EnableOutline;

            struct appdata {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float4 color : COLOR;
            };

            float4 vert(appdata v) : SV_POSITION {
                if (_EnableOutline == 0) {
                    return UnityObjectToClipPos(v.vertex);
                }
                float3 normal = normalize(v.normal);
                v.vertex.xyz += normal * _OutlineThickness;
                return UnityObjectToClipPos(v.vertex);
            }

            fixed4 frag(v2f i) : SV_Target {
                if (_EnableOutline == 0) {
                    discard;
                }
                return _OutlineColor;
            }
            ENDCG
        }
    }

    FallBack "Diffuse"
}
