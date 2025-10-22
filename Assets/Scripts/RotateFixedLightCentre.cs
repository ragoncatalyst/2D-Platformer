using UnityEngine;

// 简化版：作为 Player 的子物体时，自动随父物体移动，只负责绕 Z 指向鼠标
public class RotateFixedLightCentre : MonoBehaviour
{
    Camera cam;
    [SerializeField] bool correctParentFlip = true; // 通过本地缩放抵消父级 X 轴镜像
    [SerializeField] float angleOffset = 0f;        // 如灯锥本地不指向 +X，可用偏移对齐
    [SerializeField] float smoothTime = 0.2f;       // 旋转缓冲时长（秒），实现变速起止
    float angularVel;                                // SmoothDampAngle 的速度缓存
    Vector3 initialLocalScale;

    void Awake()
    {
        cam = Camera.main;
        initialLocalScale = transform.localScale;
    }

    void LateUpdate()
    {
        if (!cam) { cam = Camera.main; if (!cam) return; }

        // 用本地缩放抵消父级的左右镜像，使得合成后的 X 始终为正，避免旋转方向跳变
        if (correctParentFlip && transform.parent != null)
        {
            float parentSign = Mathf.Sign(transform.parent.lossyScale.x);
            var ls = initialLocalScale;
            ls.x = Mathf.Abs(initialLocalScale.x) * parentSign;
            transform.localScale = ls;
        }

        Vector3 mouseWorld = cam.ScreenToWorldPoint(Input.mousePosition);
        Vector2 dir = (mouseWorld - transform.position);
        if (dir.sqrMagnitude > 1e-6f)
        {
            float targetAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + angleOffset;
            float current = transform.eulerAngles.z;
            float next = Mathf.SmoothDampAngle(current, targetAngle, ref angularVel, Mathf.Max(0.0001f, smoothTime));
            transform.rotation = Quaternion.AngleAxis(next, Vector3.forward);
        }
    }
}