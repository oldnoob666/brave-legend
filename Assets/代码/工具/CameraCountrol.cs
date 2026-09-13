using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraCountrol : MonoBehaviour
{
    private CinemachineConfiner2D confiner2D;
    public CinemachineImpulseSource impuseSource;
    public VoidEventSO caremaShakeEvent;
    private void Awake()
    {
        confiner2D = GetComponent<CinemachineConfiner2D>();
    }

    private void OnEnable()
    {
        caremaShakeEvent.OnEventRised += OnCameraShakeEvent;

    }

    

    private void OnDisable()
    {
        caremaShakeEvent.OnEventRised -= OnCameraShakeEvent;
    }

    private void OnCameraShakeEvent()
    {
        impuseSource.GenerateImpulse();
    }


    private void Start()
    {
        GetNewCameraBounds();
    }
    private void  GetNewCameraBounds()
    {
        var obj = GameObject.FindGameObjectWithTag("bounds");
        if(obj == null)
            return;
        confiner2D.m_BoundingShape2D = obj.GetComponent<Collider2D>();
        confiner2D.InvalidateCache();
    }

    public void RefreshForScene(Scene scene)
    {
        confiner2D.m_BoundingShape2D = null;
        foreach (var root in scene.GetRootGameObjects())
        {
            foreach (var bounds in root.GetComponentsInChildren<Collider2D>())
            {
                if (bounds.CompareTag("bounds"))
                {
                    confiner2D.m_BoundingShape2D = bounds;
                    break;
                }
            }
            if (confiner2D.m_BoundingShape2D != null)
                break;
        }
        confiner2D.InvalidateCache();
        // A teleport must not damp the camera across the old scene's position.
        var virtualCamera = GetComponent<CinemachineVirtualCamera>();
        if (virtualCamera != null)
            virtualCamera.PreviousStateIsValid = false;
    }
}
