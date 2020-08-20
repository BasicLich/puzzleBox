Shader "Unlit/UnlitShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _AtlasWidth("Atlas Width", int) = 48
        _AtlasHeight("Atlas Height", int) = 22
        _TileX("X", int) = 8
        _TileY("Y", int) = 10
    }
    SubShader
    {
        Tags { 
            "Queue" = "Transparent"
            "RenderType"="Transparent"
            "LightMode" = "ForwardBase"
            "PassFlags" = "OnlyDirectional"
        }
        LOD 100

        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                float3 worldNormal : NORMAL;
                float3 viewDir : TEXCOORD1;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            UNITY_INSTANCING_BUFFER_START(Props)
                UNITY_DEFINE_INSTANCED_PROP(int, _TileX)
                UNITY_DEFINE_INSTANCED_PROP(int, _TileY)
            UNITY_INSTANCING_BUFFER_END(Props)

            sampler2D _MainTex;
            float4 _MainTex_ST;
            int _AtlasWidth;
            int _AtlasHeight;

            v2f vert (appdata v)
            {
                v2f o;

                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.viewDir = WorldSpaceViewDir(v.vertex);

                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(i);

                float3 normal = normalize(i.worldNormal);
                float NdotL = dot(_WorldSpaceLightPos0, normal);
                float3 viewDir = normalize(i.viewDir);
                float4 rimDot = 1 - dot(viewDir, normal);
                

                int tileX = UNITY_ACCESS_INSTANCED_PROP(Props, _TileX);
                int tileY = UNITY_ACCESS_INSTANCED_PROP(Props, _TileY);

                // sample the texture
                float2 uv = float2(i.uv.x + tileX, i.uv.y + tileY);
                float2 singleTileUV = float2(uv.x / _AtlasWidth, uv.y / _AtlasHeight);


                fixed4 col = tex2D(_MainTex, singleTileUV);

                float dist = max(abs(i.uv.x - 0.5), abs(i.uv.y - 0.5));
                float isBorder = step(0, dist);


                float4 finalUnlitColor = col;
                return finalUnlitColor * saturate(NdotL + 1.1); // + rimDot * 0.2;
                return finalUnlitColor;
            }
            ENDCG
        }
    }
}
