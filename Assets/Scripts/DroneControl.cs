using UnityEngine;
using UnityEngine.InputSystem;

// Minimal WASD drone controller matching PlayerMovement's input style (OnMove callback)
[DisallowMultipleComponent]
public class DroneControl : MonoBehaviour
{
    [SerializeField] float moveSpeed = 8f;
    [SerializeField] InputActionReference flyAroundAction; // 可选：直接绑定 Drone/FlyAround 动作

    Rigidbody2D rb;
    Vector2 moveInput;
    float gravityBackup;
    bool isActive => ActiveControl.Instance.Current == ActiveControl.Actor.Drone;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void OnEnable()
    {
        if (!rb) return;
        gravityBackup = rb.gravityScale;
        rb.gravityScale = 0f; // drone 悬浮
    }

    void OnDisable()
    {
        if (rb) rb.gravityScale = gravityBackup;
        moveInput = Vector2.zero; // 防止上一次输入残留
    }

    void OnMove(InputValue value)
    {
        if (!isActive) return;
        moveInput = value.Get<Vector2>(); // WASD / 左摇杆
    }

    // 支持你当前 Action 名称为 "FlyAround" 的配置
    void OnFlyAround(InputValue value) => OnMove(value);

    void FixedUpdate()
    {
        if (!rb) return;
        if (!isActive)
        {
            // 非当前受控对象：停住
            rb.velocity = Vector2.zero;
            return;
        }
        Vector2 v = moveInput;
        if (flyAroundAction && flyAroundAction.action.enabled)
            v = flyAroundAction.action.ReadValue<Vector2>();
        rb.velocity = v * moveSpeed;
    }
}
