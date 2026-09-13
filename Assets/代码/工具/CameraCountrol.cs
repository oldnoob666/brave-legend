using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

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
}
