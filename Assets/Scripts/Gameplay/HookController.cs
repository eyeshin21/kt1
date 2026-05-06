using DG.Tweening;
using UnityEngine;

public class HookController : MonoBehaviour
{
    [SerializeField] private Renderer ropeRenderer;
    [SerializeField] private Renderer hookRenderer;
    [SerializeField] private Transform[] bones;
    [SerializeField] private float shootSpeed = 30f;
    [SerializeField] private ScrewController hookedScrew;
    [SerializeField] private Transform screwParent;

    private ShooterController shooter;
    private Vector3 startPos;
    private Vector3 endPos;
    private AnimationCurve curve;
    private float curveHeight;
    private float moveTime;

    public void Init(ShooterController shooter, ColorEnum color, Vector3 startPos, Vector3 endPos, AnimationCurve curve, float curveHeight)
    {
        this.shooter = shooter;
        this.startPos = startPos;
        this.endPos = endPos;
        this.curve = curve;
        this.curveHeight = curveHeight;

        float distance = CalculateLength();
        moveTime = distance / shootSpeed;

        transform.position = startPos;
        Vector3 dir = (endPos - startPos).normalized;
        transform.rotation = Quaternion.FromToRotation(Vector3.up, dir);

        var mat = MaterialCache.GetShooterActiveMat(color);
        //var mat2 = MaterialCache.GetShooterInactive2Mat(color);

        hookRenderer.sharedMaterial = mat;
        ropeRenderer.sharedMaterial = mat;

        UpdateWaterFlow(0);
    }

    Tween tweenShoot;
    public void Shoot(ScrewController screw)
    {
        hookedScrew = screw;

        tweenShoot = DOVirtual.Float(0f, 1f, moveTime, result =>
        {
            UpdateWaterFlow(result);
        }).SetEase(Ease.Linear).OnComplete(() =>
        {
            shooter.AddHookScrew();
            hookedScrew.transform.parent = screwParent;
            hookedScrew.gameObject.SetActive(true);
            hookedScrew.transform.DOLocalMove(Vector3.zero, 0.1f);
            hookedScrew.transform.DOLocalRotate(Vector3.zero, 0.1f);
            hookedScrew.Hook(() =>
            {
                PullBack();
            });

            tweenShoot = null;
        });
    }

    Tween tweenPullBack;
    public void PullBack()
    {
        tweenPullBack = DOVirtual.Float(1f, 0f, moveTime, result =>
        {
            UpdateWaterFlow(result);
        }).SetEase(Ease.Linear).OnComplete(() =>
        {
            tweenPullBack = null;
            shooter.hooks.Remove(this);
            Recycle();
        });
    }

    private void UpdateWaterFlow(float flowPercent)
    {
        Vector3 dir = (endPos - startPos).normalized;
        Vector3 up = Vector3.right;
        float distance = Vector3.Distance(startPos, endPos);

        for (int i = 0; i < bones.Length; i++)
        {
            float percent = (i / (float)(bones.Length - 1)) * flowPercent;

            Vector3 basePos = Vector3.Lerp(startPos, endPos, percent);
            float offset = curve.Evaluate(percent) * curveHeight;
            Vector3 pos = basePos + up * offset;
            bones[i].transform.position = pos;

            if (i < bones.Length - 1)
            {
                float step = (1f / (bones.Length - 1)) * flowPercent;
                float percentNext = Mathf.Clamp01(percent + step);
                Vector3 baseNext = Vector3.Lerp(startPos, endPos, percentNext);
                float offsetNext = curve.Evaluate(percentNext) * curveHeight;
                Vector3 posNext = baseNext + up * offsetNext;
                Vector3 dirNext = (posNext - pos).normalized;
                bones[i].rotation = Quaternion.FromToRotation(Vector3.up, dirNext);
            }
            else
            {
                bones[i].transform.rotation = bones[i - 1].rotation;
            }
        }
    }

    private float CalculateLength()
    {
        float length = 0f;
        Vector3 up = Vector3.back;
        Vector3 prevPoint = startPos + up * (curve.Evaluate(0f) * curveHeight);

        for (int i = 1; i < bones.Length; i++)
        {
            float percent = i / (bones.Length - 1);
            Vector3 basePos = Vector3.Lerp(startPos, endPos, percent);
            float offset = curve.Evaluate(percent) * curveHeight;
            Vector3 currentPoint = basePos + up * offset;

            length += Vector3.Distance(prevPoint, currentPoint);
            prevPoint = currentPoint;
        }

        return length;
    }

    public void Recycle()
    {
        if (tweenShoot != null)
        {
            tweenShoot.Kill();
            tweenShoot = null;
        }

        if (tweenPullBack != null)
        {
            tweenPullBack.Kill();
            tweenPullBack = null;
        }

        if (hookedScrew != null)
        {
            Destroy(hookedScrew.gameObject);
        }

        shooter = null;

        gameObject.Recycle();
    }
}
