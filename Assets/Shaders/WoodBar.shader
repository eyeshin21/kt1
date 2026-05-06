Shader "Custom/WoodBar"
{
    Properties
    {
      _MainTex("Background One", 2D) = "defaulttexture" {}
      _SecondaryTex("Background Two", 2D) = "defaulttexture" {}
    }
    SubShader
    {
                Tags { "Queue"="Transparent" "RenderType"="Transparent"}
                Blend SrcAlpha OneMinusSrcAlpha
                ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _SecondaryTex;
            float4 _SecondaryTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);

                o.uv = TRANSFORM_TEX(v.uv, _MainTex);

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col1 = tex2D(_MainTex, i.uv);
                fixed4 col2 = tex2D(_SecondaryTex, i.uv);

                fixed4 col = col1 * col2;
                return col;
            }
            ENDCG
        }
    }
}