using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteAtlasRectBinder : MonoBehaviour
{
    static readonly int SpriteRectId = Shader.PropertyToID("_SpriteRect");
    static readonly int SpriteRotId = Shader.PropertyToID("_SpriteRot");

    SpriteRenderer sr;
    MaterialPropertyBlock mpb;
    Sprite lastSprite;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        mpb = new MaterialPropertyBlock();
        Apply();
    }

    void LateUpdate()
    {
        // Nếu sprite thay đổi runtime (animation, swap sprite...) thì tự cập nhật
        if (sr.sprite != lastSprite) Apply();
    }

    void Apply()
    {
        var sp = sr.sprite;
        lastSprite = sp;

        if (sp == null || sp.texture == null) return;

        var tex = sp.texture;
        var r = sp.textureRect; // pixel rect in atlas texture

        // normalized uv rect
        Vector4 rectUv = new Vector4(
            r.x / tex.width,
            r.y / tex.height,
            r.width / tex.width,
            r.height / tex.height
        );

        float rot = 0f;
        // Unity packs sprite can be flipped/rot180
        switch (sp.packingRotation)
        {
            case SpritePackingRotation.FlipHorizontal: rot = 1f; break;
            case SpritePackingRotation.FlipVertical: rot = 2f; break;
            case SpritePackingRotation.Rotate180: rot = 3f; break;
            default: rot = 0f; break;
        }

        sr.GetPropertyBlock(mpb);
        mpb.SetVector(SpriteRectId, rectUv);
        mpb.SetFloat(SpriteRotId, rot);
        sr.SetPropertyBlock(mpb);
    }
}
