using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class KeyUI : MonoBehaviour {
    private Image key;
    private TextMeshProUGUI keyText;

    [SerializeField] private Color enabledColor = Color.white;
    [SerializeField] private Color disabledColor = new Color32(215, 215, 215, 70);

    private void Awake() {
        key = GetComponentInChildren<Image>();
        keyText = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void SetEnabled(bool value) {
        Color bg = value ? enabledColor : disabledColor;
        Color txt = value ? enabledColor : disabledColor;

        if (key != null)
            key.color = bg;

        if (keyText != null)
            keyText.color = txt;
    }
}
