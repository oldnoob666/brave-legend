using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraCountrol : MonoBehaviour
{
    [Header("ÊÂ¼þ¼àÌý")]
    public VoidEventSO afterLoadedSceneEvent;

    private CinemachineConfiner2D confiner2D;
    public CinemachineImpulseSource impuseSource;
    public VoidEventSO caremaShakeEvent;
    private void Awake()
    {
        confiner2D = GetComponent<CinemachineConfiner2D>();
    }

    private void OnEnable()
    {
        afterLoadedSceneEvent.OnEventRised += OnAfterSceneLoadedEvent;
        caremaShakeEvent.OnEventRised += OnCameraShakeEvent;

    }

    

    private void OnDisable()
    {
        afterLoadedSceneEvent.OnEventRised -= OnAfterSceneLoadedEvent;
        caremaShakeEvent.OnEventRised -= OnCameraShakeEvent;
    }

    private void OnAfterSceneLoadedEvent()
    {
        GetNewCameraBounds(); 
    }

    private void OnCameraShakeEvent()
    {
        impuseSource.GenerateImpulse();
    }


    //private void Start()
    //{
    //    GetNewCameraBounds();
    //}
    private void  GetNewCameraBounds()
    {
        var obj = GameObject.FindGameObjectWithTag("bounds");
        if(obj == null)
            return;
        confiner2D.m_BoundingShape2D = obj.GetComponent<Collider2D>();
        confiner2D.InvalidateCache();
    }
}
