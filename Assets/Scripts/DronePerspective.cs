using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
[AddComponentMenu("Camera/Drone Perspective Toggle")]
public class DronePerspective : MonoBehaviour
{
    [Header("Cinemachine Virtual Cameras")]
    [SerializeField] CinemachineVirtualCameraBase playerCam; // 玩家视角虚拟相机
    [SerializeField] CinemachineVirtualCameraBase droneCam;  // Drone 视角虚拟相机

    [Header("Toggle")]
    [SerializeField] KeyCode toggleKey = KeyCode.E;
    [SerializeField] bool startWithPlayer = true;
    [SerializeField] int activePriority = 20;
    [SerializeField] int inactivePriority = 10;

    bool usingDrone;
    [Header("Input (optional)")]
    [SerializeField] PlayerInput playerInput; // 方案1：单一 PlayerInput，切换 Action Map
    [SerializeField] PlayerInput droneInput;  // 方案2：两个 PlayerInput，切换启用状态
    [SerializeField] string playerMap = "Player";
    [SerializeField] string droneMap = "Drone";

    [Header("Lens per group (optional)")]
    [Tooltip("If true, use Orthographic Size (2D). If false, use Field of View (3D).")]
    [SerializeField] bool useOrthographicLens = true;
    [Tooltip("Player State Driven camera group's orthographic size or FOV.")]
    [SerializeField] float playerView = 6f;
    [Tooltip("Drone State Driven camera group's orthographic size or FOV.")]
    [SerializeField] float droneView = 10f;

    void OnEnable()
    {
        usingDrone = !startWithPlayer;
        // 先确保镜头尺寸设置正确
        ApplyLens(playerCam, playerView);
        ApplyLens(droneCam, droneView);
        Apply();
    }

    void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            usingDrone = !usingDrone;
            Apply();
        }
    }

    void Apply()
    {
        // 仅通过优先级切换视角，需已在 Inspector 绑定 playerCam & droneCam，主摄像机挂有 CinemachineBrain
        playerCam.Priority = usingDrone ? inactivePriority : activePriority;
        droneCam.Priority  = usingDrone ? activePriority  : inactivePriority;

        // 输入同步：优先支持“双 PlayerInput”启用/禁用；否则用单 PlayerInput 切换 Action Map
        if (playerInput != null && droneInput != null)
        {
            if (usingDrone)
            {
                playerInput.DeactivateInput();
                droneInput.ActivateInput();
            }
            else
            {
                droneInput.DeactivateInput();
                playerInput.ActivateInput();
            }
        }
        else if (playerInput != null)
        {
            playerInput.SwitchCurrentActionMap(usingDrone ? droneMap : playerMap);
        }

        // 告知全局“当前受控对象”，用于在移动脚本中做门控
        ActiveControl.Instance.Current = usingDrone ? ActiveControl.Actor.Drone : ActiveControl.Actor.Player;
    }

    void ApplyLens(CinemachineVirtualCameraBase root, float value)
    {
        if (root == null) return;
        // 查找该组下的所有真实虚拟相机（排除父级 StateDrivenCamera 本身）
        var leafs = root.GetComponentsInChildren<CinemachineVirtualCamera>(true);
        foreach (var vcam in leafs)
        {
            var lens = vcam.m_Lens; // LensSettings 是 struct，需要回写
            if (useOrthographicLens)
                lens.OrthographicSize = value;
            else
                lens.FieldOfView = value;
            vcam.m_Lens = lens;
        }
    }
}
