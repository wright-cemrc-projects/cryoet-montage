using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Cinemachine;

public class CameraController : MonoBehaviour
{
    public CinemachineVirtualCamera WorldCamera;
    public CinemachineVirtualCamera OrthoCamera;
    public CinemachineVirtualCamera StageCamera;

    private int HighPriority = 5;
    private int LowPriority = 1;

    void ResetPriorities() {
        WorldCamera.Priority = LowPriority;
        OrthoCamera.Priority = LowPriority;
        StageCamera.Priority = LowPriority;
    }

    // Start is called before the first frame update
    void Start()
    {
        ResetPriorities();
        WorldCamera.Priority = HighPriority;
    }

    public void ActivateWorldCamera() {
        ResetPriorities();
        WorldCamera.Priority = HighPriority;
    }

    public void ActivateOrthoCamera() {
        ResetPriorities();
        OrthoCamera.Priority = HighPriority;
    }

    public void ActivateStageCamera() {
        ResetPriorities();
        StageCamera.Priority = HighPriority;
    }
}
