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

    void OnEnable()
    {
        usingDrone = !startWithPlayer;
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
    }
}
