using UnityEngine;
using TMPro;
using Sound;

public class ConfigController : MonoBehaviour
{

    private TextMeshProUGUI text;

    void Awake()
    {
        text = GameObject.Find("ValueText (TMP)").GetComponent<TextMeshProUGUI>();
        UpdateVolumeText();
    }

    public void DecreaseVolume()
    {
        float currentVolume = SoundManager.CurrentVolume;
        SoundManager.SetVolume(currentVolume - 1);
        Debug.Log("Volume decreased to: " + SoundManager.CurrentVolume);
        UpdateVolumeText();
    }

    public void IncreaseVolume()
    {
        float currentVolume = SoundManager.CurrentVolume;
        SoundManager.SetVolume(currentVolume + 1);
        Debug.Log("Volume increased to: " + SoundManager.CurrentVolume);
        UpdateVolumeText();
    }

    void UpdateVolumeText()
    {
        if (text != null)
        {
            text.text = SoundManager.CurrentVolume.ToString();
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
