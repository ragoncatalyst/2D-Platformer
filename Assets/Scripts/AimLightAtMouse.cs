using UnityEngine;

[RequireComponent(typeof(Light))]
public class AimLightAtMouse : MonoBehaviour
{
    [Tooltip("保持与玩家的本地位置不变")]
    [SerializeField] bool lockLocalPosition = true;

    [Tooltip("让光线带一点朝向屏幕内的 -Z，避免2D精灵打不亮")]
    [SerializeField] float zBiasTowardCamera = 1f;

    Vector3 initialLocalPos;
    Camera cam;

    void Awake()
    {
        cam = Camera.main;
        if (lockLocalPosition) initialLocalPos = transform.localPosition;
    }

    void LateUpdate()
    {
        if (lockLocalPosition) transform.localPosition = initialLocalPos;
        if (!cam) { cam = Camera.main; if (!cam) return; }

        Vector3 mouseWorld = cam.ScreenToWorldPoint(Input.mousePosition);
        // 保证有一点 -Z 分量
        mouseWorld.z = transform.position.z - Mathf.Abs(zBiasTowardCamera);

        Vector3 dir = mouseWorld - transform.position;
        if (dir.sqrMagnitude > 1e-6f)
            transform.rotation = Quaternion.LookRotation(dir, Vector3.up);
    }
}