using System;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour {

    private Image barImage;

    private void Awake() {
        InitializeBarImage();
    }
    
    private void InitializeBarImage() {
        if (barImage == null) {
            barImage = transform.Find("Bar").GetComponent<Image>();
        }
    }

    public void SetValue(float normalized) {
        InitializeBarImage();
        barImage.fillAmount = normalized;
    }
}
