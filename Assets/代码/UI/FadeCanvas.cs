using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class FadeCanvas : MonoBehaviour
{
    [Header("ÊÂ¼þ¼àÌý")]
    public FadeEventSO fadeEventSO;



    public Image fadeImage;


    private void OnEnable()
    {
        fadeEventSO.OnEventRised += OnFadeEvent;
    }


    private void OnDisable()
    {
        fadeEventSO.OnEventRised -= OnFadeEvent;
    }
    public void OnFadeEvent(Color target ,float duration,bool fadeIn)
    {
        fadeImage.DOBlendableColor(target, duration);
    }
}
