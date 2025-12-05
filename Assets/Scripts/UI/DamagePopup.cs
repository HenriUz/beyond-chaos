using UnityEngine;
using TMPro;

public class DamagePopup : MonoBehaviour {
    [SerializeField] private float lifetime = 0.7f;
    [SerializeField] private float speed;

    private TextMeshProUGUI text;
    private RectTransform rect;

    private void Awake() {
        text = GetComponent<TextMeshProUGUI>();
        rect = GetComponent<RectTransform>();
    }

    public void Setup(string value, Color color) {
        text.text = value;
        text.color = color;
        Destroy(gameObject, lifetime);
    }

    private void Update() {
        Vector2 direction = (Vector2.up + Vector2.right).normalized; // diagonal suave
        rect.anchoredPosition += direction * speed * Time.deltaTime;

        // fade out suave
        text.alpha -= 1f / lifetime * Time.deltaTime;
}

}
