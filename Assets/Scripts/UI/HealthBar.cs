using System;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour {

    private Image barImage;

    private void Awake() {
        barImage = transform.Find("Bar").GetComponent<Image>();
    }

    public void SetValue(float normalized) {
        barImage.fillAmount = normalized;
    }
}
