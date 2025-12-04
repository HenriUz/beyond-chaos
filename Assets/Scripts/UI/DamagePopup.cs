using UnityEngine;
using TMPro;

public class DamagePopup : MonoBehaviour {
    [SerializeField] private float lifetime = 0.7f;
    [SerializeField] private float speed = 1f;

    private TextMeshProUGUI text;

    private void Awake() {
        text = GetComponent<TextMeshProUGUI>();
    }

    public void Setup(string value, Color color) {
        text.text = value;
        text.color = color;
        Destroy(gameObject, lifetime);
    }

    void Update() {
        transform.position += new Vector3(speed, speed, 0) * Time.deltaTime;
    }
}
