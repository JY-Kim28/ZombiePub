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
            float3 worldNormal; // ���� ���� ��� ������ �߰�
            INTERNAL_DATA       // Unity �⺻ ���� ��� ������
        };

        sampler2D _MainTex;
        fixed4 _Color;
        fixed4 _ShadeColor;
        float _ShadowStrength;

        // Photoshop �������� ���� �Լ�
        half3 OverlayBlend(half3 base, half3 blend) {
            return (base < 0.5) ? (2.0 * base * blend) : (1.0 - 2.0 * (1.0 - base) * (1.0 - blend));
        }

        void surf(Input IN, inout SurfaceOutput o) {
            // �ؽ�ó�� _Color ���� (��Ƽ�÷��� ȿ��)
            fixed4 texColor = tex2D(_MainTex, IN.uv_MainTex);
            fixed4 baseColor = texColor * _Color;

            // Unity�� �⺻ ��� ��� ��� ���
            fixed3 normal = normalize(IN.worldNormal); 
            o.Normal = normal; // ��� ������ ����

            o.Albedo = baseColor.rgb; // �ؽ�ó * _Color ���
            o.Alpha = baseColor.a;    // ���� �� ����
        }

        half4 LightingToon(SurfaceOutput s, half3 lightDir, half3 viewDir, half atten) {
            half3 normal = normalize(s.Normal); // ���̴����� ��� ���
            half NdotL = dot(normal, lightDir);

            // Unity�� �⺻ �׸��� ���� (atten ���)
            half shadow = 1.0 - saturate(atten); // �ݴ�� ���� �׸��� �� ����

            // �ε巯�� �׸��� ó���� ���� ������ soft transition �߰�
            half smoothShadow = smoothstep(0.4, 0.6, shadow); // ������ �ε巯�� ���� ���

            // �׸��� ����: Photoshop �������� ����
            half3 blendedShadow = OverlayBlend(s.Albedo, _ShadeColor.rgb);

            // �׸��� ������ ����
            half3 litColor = lerp(s.Albedo, blendedShadow, smoothShadow * _ShadowStrength);

            return half4(litColor, s.Alpha);
        }
        ENDCG
    }

    FallBack "Diffuse"
}
