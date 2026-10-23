Shader "CGVR/PhongFurniture"
{
    Properties
    {
        _Color ("Furniture Tint", Color) = (1,1,1,1)
        _MainTex ("Albedo Texture", 2D) = "white" {}
        _BumpMap ("Normal / Bump Map", 2D) = "bump" {}
        _UseTexture ("Use Texture", Float) = 1
        _UseBump ("Use Bump Mapping", Float) = 1
        _BumpStrength ("Bump Strength", Range(0,2)) = 1
        _AmbientStrength ("Ambient", Range(0,1.5)) = 0.28
        _DiffuseStrength ("Diffuse", Range(0,2)) = 1
        _SpecularStrength ("Specular", Range(0,2)) = 0.25
        _Shininess ("Specular Shininess", Range(1,128)) = 28
    }
    SubShader
    {
        Tags { "Queue"="Geometry" "RenderType"="Opaque" }
        LOD 200
        Pass
        {
            Tags { "LightMode"="ForwardBase" }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdbase
            #include "UnityCG.cginc"
            #include "Lighting.cginc"

            sampler2D _MainTex;
            sampler2D _BumpMap;
            float4 _MainTex_ST;
            fixed4 _Color;
            float _UseTexture, _UseBump, _BumpStrength;
            float _AmbientStrength, _DiffuseStrength, _SpecularStrength, _Shininess;

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 uv : TEXCOORD0;
            };
            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
                float3 worldNormal : TEXCOORD2;
                float3 worldTangent : TEXCOORD3;
                float3 worldBitangent : TEXCOORD4;
            };
            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.worldTangent = UnityObjectToWorldDir(v.tangent.xyz);
                o.worldBitangent = cross(o.worldNormal, o.worldTangent) * (v.tangent.w * unity_WorldTransformParams.w);
                return o;
            }
            fixed4 frag(v2f i) : SV_Target
            {
                float3 N = normalize(i.worldNormal);
                if (_UseBump > 0.5)
                {
                    // Runtime texture is raw linear RGB, not importer-packed DXT5nm.
                    float3 tangentNormal = tex2D(_BumpMap, i.uv).rgb * 2.0 - 1.0;
                    tangentNormal.xy *= _BumpStrength;
                    tangentNormal = normalize(tangentNormal);
                    N = normalize(tangentNormal.x * normalize(i.worldTangent) +
                                  tangentNormal.y * normalize(i.worldBitangent) +
                                  tangentNormal.z * N);
                }

                float3 L = normalize(UnityWorldSpaceLightDir(i.worldPos));
                float3 V = normalize(_WorldSpaceCameraPos.xyz - i.worldPos);
                float diffuseTerm = max(0.0, dot(N, L));
                float3 R = reflect(-L, N);
                float specularTerm = pow(max(0.0, dot(R, V)), _Shininess) * step(0.0001, diffuseTerm);
                float3 albedo = _Color.rgb;
                if (_UseTexture > 0.5) albedo *= tex2D(_MainTex, i.uv).rgb;

                // Phong illumination: ambient + diffuse + specular.
                float3 ambient = albedo * _AmbientStrength;
                float3 diffuse = albedo * _DiffuseStrength * _LightColor0.rgb * diffuseTerm;
                float3 specular = _SpecularStrength * _LightColor0.rgb * specularTerm;
                return fixed4(saturate(ambient + diffuse + specular), 1.0);
            }
            ENDCG
        }
    }
    Fallback "Diffuse"
}
