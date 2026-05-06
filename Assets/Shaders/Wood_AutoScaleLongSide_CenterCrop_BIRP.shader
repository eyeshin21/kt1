Shader "Custom/Sprite/Wood_AutoCover_CenterCrop_Combined"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite (Color+Mask)", 2D) = "white" {}
        _Color ("Renderer Tint", Color) = (1,1,1,1)

        _WoodTex ("Wood Texture", 2D) = "white" {}
        _WoodTint ("Wood Tint", Color) = (1,1,1,1)

        _Combine ("Combine Strength", Range(0,1)) = 1
        _KeepWood ("Keep Wood Detail", Range(0,1)) = 1
        // _KeepWood: 1 = giữ vân gỗ mạnh hơn (ít bị “phẳng” khi sprite trắng/đậm)
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

            sampler2D _WoodTex;
            float4 _WoodTex_TexelSize; // z=w, w=h

            fixed4 _Color;
            fixed4 _WoodTint;
            float _Combine;
            float _KeepWood;

            struct appdata
            {
                float4 vertex : POSITION;
                fixed4 color  : COLOR;
                float2 uv     : TEXCOORD0; // local uv 0..1 của sprite mesh
            };

            struct v2f
            {
                float4 pos     : SV_POSITION;
                fixed4 color   : COLOR;
                float2 uvLocal : TEXCOORD0; // cho wood cover
                float2 uvMask  : TEXCOORD1; // cho sprite (đúng atlas)
            };

            float2 GetObjectScaleXY()
            {
                float3 worldX = float3(unity_ObjectToWorld._m00, unity_ObjectToWorld._m10, unity_ObjectToWorld._m20);
                float3 worldY = float3(unity_ObjectToWorld._m01, unity_ObjectToWorld._m11, unity_ObjectToWorld._m21);
                float sx = length(worldX);
                float sy = length(worldY);
                return float2(max(sx, 1e-6), max(sy, 1e-6));
            }

            float2 AutoCoverUV(float2 uv01, float shapeAspect, float texAspect)
            {
                float2 scale;
                if (shapeAspect > texAspect)
                    scale = float2(1.0, texAspect / shapeAspect);   // crop top/bottom
                else
                    scale = float2(shapeAspect / texAspect, 1.0);   // crop left/right

                return (uv01 - 0.5) * scale + 0.5;
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.color = v.color * _Color;           // SpriteRenderer color * material tint
                o.uvLocal = v.uv;
                o.uvMask = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Sprite sample: vừa là mask alpha vừa là “màu shape”
                fixed4 spr = tex2D(_MainTex, i.uvMask);
                spr *= i.color; // áp tint của SpriteRenderer lên luôn

                // Aspect shape (theo transform scale)
                float2 sxy = GetObjectScaleXY();
                float shapeAspect = sxy.x / sxy.y;

                // Aspect wood texture
                float texAspect = _WoodTex_TexelSize.z / _WoodTex_TexelSize.w;

                // UV cover + center crop cho wood
                float2 uvWood = AutoCoverUV(i.uvLocal, shapeAspect, texAspect);
                fixed4 wood = tex2D(_WoodTex, uvWood) * _WoodTint;

                // --- Combine ---
                // Multiply: wood * spriteColor
                fixed3 mult = wood.rgb * spr.rgb;

                // Giữ vân gỗ rõ hơn: trộn thêm wood theo độ sáng của sprite
                // sprite sáng -> giữ wood nhiều hơn; sprite tối -> multiply mạnh
                fixed sprLum = dot(spr.rgb, fixed3(0.299, 0.587, 0.114));
                fixed3 multKeep = lerp(mult, wood.rgb, _KeepWood * sprLum);

                fixed3 rgb = lerp(wood.rgb, multKeep, _Combine);

                // Alpha theo shape (sprite alpha)
                fixed a = spr.a;

                // Premultiply cho Blend One OneMinusSrcAlpha
                fixed4 outCol;
                outCol.rgb = rgb * a;
                outCol.a = a;
                return outCol;
            }
            ENDCG
        }
    }
}
