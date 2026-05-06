using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class AutoWoodTilingForSprite : MonoBehaviour
{
    [Tooltip("Muốn ~bao nhiêu pixel cho 1 lần lặp vân gỗ. Nhỏ hơn => vân dày hơn.")]
    public float pixelsPerRepeat = 64f;

    static readonly int WoodTilingId = Shader.PropertyToID("_WoodTiling");

    SpriteRenderer sr;
    MaterialPropertyBlock mpb;
    Sprite last;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        mpb = new MaterialPropertyBlock();
        Apply();
    }

    void LateUpdate()
    {
        if (sr.sprite != last) Apply();
    }

    void Apply()
    {
        var sp = sr.sprite;
        last = sp;
        if (sp == null) return;

        // kích thước sprite theo pixel (không cần atlas)
        var r = sp.textureRect; // pixel rect
        float tileX = Mathf.Max(1f, r.width / Mathf.Max(1f, pixelsPerRepeat));
        float tileY = Mathf.Max(1f, r.height / Mathf.Max(1f, pixelsPerRepeat));

        sr.GetPropertyBlock(mpb);
        mpb.SetVector(WoodTilingId, new Vector4(tileX, tileY, 0, 0));
        sr.SetPropertyBlock(mpb);
    }
}
