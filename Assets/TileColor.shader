Shader "Last Token/Tile Color"
{
    Properties { _Color ("Color", Color) = (1,1,1,1) }
    SubShader
    {
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            fixed4 _Color;
            struct Pixel
            {
                float4 position : SV_POSITION;
                float light : TEXCOORD0;
            };

            Pixel vert(appdata_base vertex)
            {
                Pixel pixel;
                pixel.position = UnityObjectToClipPos(vertex.vertex);
                float3 normal = UnityObjectToWorldNormal(vertex.normal);
                pixel.light = 0.58 + 0.42 * saturate(dot(normal, float3(0.2, 0.95, -0.23)));
                return pixel;
            }

            fixed4 frag(Pixel pixel) : SV_Target
            {
                return fixed4(_Color.rgb * pixel.light, 1);
            }
            ENDCG
        }
    }
}
