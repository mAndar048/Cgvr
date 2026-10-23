Shader "CGVR/Lab8/VertexFragmentRGB"
{
    Properties { _Angle ("Rotation angle (radians)", Float) = 0 }
    SubShader
    {
        Tags { "Queue"="Geometry" "RenderType"="Opaque" }
        Pass
        {
            Cull Off
            ZWrite On
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            float _Angle;
            struct appdata { float4 vertex : POSITION; float4 color : COLOR; };
            struct v2f { float4 position : SV_POSITION; fixed4 color : COLOR; };
            v2f vert(appdata v)
            {
                v2f o;
                float c = cos(_Angle);
                float s = sin(_Angle);
                float2 xy = float2(c * v.vertex.x - s * v.vertex.y,
                                    s * v.vertex.x + c * v.vertex.y);
                o.position = UnityObjectToClipPos(float4(xy, v.vertex.z, 1));
                o.color = v.color;
                return o;
            }
            fixed4 frag(v2f i) : SV_Target { return i.color; }
            ENDCG
        }
    }
    Fallback Off
}
