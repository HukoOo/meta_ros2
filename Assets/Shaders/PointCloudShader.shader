Shader "Custom/PointCloudShader"
{
    Properties
    {
        _PointSize ("Point Size", Float) = 0.02      // 점 크기
        _Alpha ("Alpha", Range(0,1)) = 1.0           // 투명도
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            float _PointSize;
            float _Alpha;

            struct appdata
            {
                float4 vertex : POSITION;
                fixed4 color  : COLOR;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                fixed4 color  : COLOR;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.color = v.color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return fixed4(i.color.rgb, _Alpha); // 투명도 적용
            }
            ENDCG
        }
    }
    FallBack "Unlit/Color"
}