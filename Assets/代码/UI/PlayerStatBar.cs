using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PlayerStatBar : MonoBehaviour
{
    public Image healthImage;
    public Image healthDelayImage;
    public float speed;


    private void Update()
    {
        if(healthImage.fillAmount < healthDelayImage.fillAmount)
        {
            healthDelayImage.fillAmount -= Time.deltaTime*speed;
        }
    }

    public void OnHealthChange(float persentage)
    {
        healthImage.fillAmount = persentage;
    }
}
