Shader "Custom/Sprite/Wood_ScaleOnlyWhenNeeded_Combined_OutlineToWhite"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite (Color+Mask)", 2D) = "white" {}
        _Color ("Renderer Tint", Color) = (1,1,1,1)

        _WoodTex ("Wood Texture", 2D) = "white" {}
        _WoodTint ("Wood Tint", Color) = (1,1,1,1)

        _SpritePPU ("Sprite Pixels Per Unit", Float) = 100
        _WoodPPU   ("Wood Pixels Per Unit", Float) = 100

        _Combine ("Combine Strength", Range(0,1)) = 1
        _KeepWood ("Keep Wood Detail", Range(0,1)) = 1

        _OutlineBlackThreshold ("Outline Black Threshold", Range(0,0.5)) = 0.12
        _OutlineMinAlpha ("Outline Min Alpha", Range(0,1)) = 0.2
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "IgnoreProjector"="True"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend One OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _MainTex_TexelSize;

            sampler2D _WoodTex;
            float4 _WoodTex_TexelSize;

            fixed4 _Color;
            fixed4 _WoodTint;

            float _SpritePPU;
            float _WoodPPU;
            float _Combine;
            float _KeepWood;

            float _OutlineBlackThreshold;
            float _OutlineMinAlpha;

            struct appdata
            {
                float4 vertex : POSITION;
                fixed4 color  : COLOR;
                float2 uv     : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos     : SV_POSITION;
                fixed4 tint    : COLOR;
                float2 uvLocal : TEXCOORD0;
                float2 uvMask  : TEXCOORD1;
            };

            float2 GetObjectScaleXY()
            {
                float3 worldX = float3(unity_ObjectToWorld._m00, unity_ObjectToWorld._m10, unity_ObjectToWorld._m20);
                float3 worldY = float3(unity_ObjectToWorld._m01, unity_ObjectToWorld._m11, unity_ObjectToWorld._m21);
                float sx = length(worldX);
                float sy = length(worldY);
                return float2(max(sx, 1e-6), max(sy, 1e-6));
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.tint = v.color * _Color;
                o.uvLocal = v.uv;
                o.uvMask = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // --- Sprite sample (màu shape + alpha mask) ---
                fixed4 spr = tex2D(_MainTex, i.uvMask) * i.tint;

                // === ĐỔ OUTLINE ĐEN -> TRẮNG ===
                // Chỉ áp dụng nếu alpha đủ lớn (tránh vùng viền anti-alias alpha rất nhỏ)
                // Và độ sáng (luminance) đủ thấp => coi là “đen”
                fixed lum = dot(spr.rgb, fixed3(0.299, 0.587, 0.114));
                if (spr.a >= _OutlineMinAlpha && lum <= _OutlineBlackThreshold)
                {
                    spr.rgb = fixed3(1,1,1);
                }

                // --- Ước lượng kích thước sprite theo world units ---
                float2 objScale = GetObjectScaleXY();

                float2 spriteSizeLocal = (_MainTex_TexelSize.zw / max(_SpritePPU, 1e-6));
                float2 spriteSizeWorld = spriteSizeLocal * objScale;
                float spriteLong = max(spriteSizeWorld.x, spriteSizeWorld.y);

                // --- Kích thước wood texture theo world units ---
                float2 woodSizeWorld = (_WoodTex_TexelSize.zw / max(_WoodPPU, 1e-6));
                float woodLong = max(woodSizeWorld.x, woodSizeWorld.y);

                // --- Rule: chỉ scale wood khi shape lớn hơn ---
                float targetLong = max(woodLong, spriteLong);
                float2 targetSize = targetLong * (woodSizeWorld / max(woodLong, 1e-6));

                // --- UV wood: center + crop theo targetSize ---
                float2 uvWood = (i.uvLocal - 0.5) * (spriteSizeWorld / max(targetSize, 1e-6)) + 0.5;

                fixed4 wood = tex2D(_WoodTex, uvWood) * _WoodTint;

                // --- Combine màu ---
                fixed3 mult = wood.rgb * spr.rgb;
                fixed sprLum = dot(spr.rgb, fixed3(0.299, 0.587, 0.114));
                fixed3 multKeep = lerp(mult, wood.rgb, _KeepWood * sprLum);
                fixed3 rgb = lerp(wood.rgb, multKeep, _Combine);

                fixed a = spr.a;

                // premultiply
                fixed4 outCol;
                outCol.rgb = rgb * a;
                outCol.a = a;
                return outCol;
            }
            ENDCG
        }
    }
}
