Shader "JoJo/Toon_Smoothing" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1, 1, 1, 1)
        _ShadeColor ("Shadow Color", Color) = (0.5, 0.5, 0.5, 1)
        _ShadowStrength ("Shadow Strength", Range(0, 1)) = 0.5
    }

    SubShader {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Toon addshadow

        struct Input {
            float2 uv_MainTex;
            float3 worldNormal; // 월드 공간 노멀 데이터 추가
            INTERNAL_DATA       // Unity 기본 조명 계산 데이터
        };

        sampler2D _MainTex;
        fixed4 _Color;
        fixed4 _ShadeColor;
        float _ShadowStrength;

        // Photoshop 오버레이 블렌딩 함수
        half3 OverlayBlend(half3 base, half3 blend) {
            return (base < 0.5) ? (2.0 * base * blend) : (1.0 - 2.0 * (1.0 - base) * (1.0 - blend));
        }

        void surf(Input IN, inout SurfaceOutput o) {
            // 텍스처와 _Color 곱셈 (멀티플레이 효과)
            fixed4 texColor = tex2D(_MainTex, IN.uv_MainTex);
            fixed4 baseColor = texColor * _Color;

            // Unity의 기본 노멀 계산 방식 사용
            fixed3 normal = normalize(IN.worldNormal); 
            o.Normal = normal; // 노멀 데이터 설정

            o.Albedo = baseColor.rgb; // 텍스처 * _Color 결과
            o.Alpha = baseColor.a;    // 알파 값 유지
        }

        half4 LightingToon(SurfaceOutput s, half3 lightDir, half3 viewDir, half atten) {
            half3 normal = normalize(s.Normal); // 쉐이더에서 노멀 사용
            half NdotL = dot(normal, lightDir);

            // Unity의 기본 그림자 강도 (atten 기반)
            half shadow = 1.0 - saturate(atten); // 반대로 계산된 그림자 값 수정

            // 부드러운 그림자 처리를 위해 고정된 soft transition 추가
            half smoothShadow = smoothstep(0.4, 0.6, shadow); // 고정된 부드러움 범위 사용

            // 그림자 영역: Photoshop 오버레이 블렌딩
            half3 blendedShadow = OverlayBlend(s.Albedo, _ShadeColor.rgb);

            // 그림자 강도와 섞기
            half3 litColor = lerp(s.Albedo, blendedShadow, smoothShadow * _ShadowStrength);

            return half4(litColor, s.Alpha);
        }
        ENDCG
    }

    FallBack "Diffuse"
}
