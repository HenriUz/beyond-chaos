using UnityEngine;
using TMPro;
using Sound;

namespace UI
{
    public class ConfigController : MonoBehaviour
    {

        private TextMeshProUGUI text;
        [SerializeField] private TMP_Dropdown screenModeDropdown;

        void Awake()
        {
            text = GameObject.Find("ValueText (TMP)").GetComponent<TextMeshProUGUI>();
            UpdateVolumeText();
            InitializeScreenModeDropdown();
        }

        void InitializeScreenModeDropdown()
        {
            if (screenModeDropdown != null)
            {
                screenModeDropdown.ClearOptions();
                screenModeDropdown.AddOptions(new System.Collections.Generic.List<string> { "Tela Cheia", "Modo Janela" });

                // Carrega a preferência salva
                LoadScreenMode();

                // Adiciona listener para mudanças
                screenModeDropdown.onValueChanged.AddListener(OnScreenModeChanged);
            }
        }

        private void LoadScreenMode()
        {
            // Se existe uma preferência salva, usa ela
            if (PlayerPrefs.HasKey("ScreenMode"))
            {
                int savedMode = PlayerPrefs.GetInt("ScreenMode");
                screenModeDropdown.value = savedMode;
                ApplyScreenMode(savedMode);
            }
            else
            {
                // Caso contrário, usa o estado atual da tela
                screenModeDropdown.value = Screen.fullScreen ? 0 : 1;
            }

            screenModeDropdown.RefreshShownValue();
        }

        public void OnScreenModeChanged(int index)
        {
            ApplyScreenMode(index);

            // Salva a preferência
            PlayerPrefs.SetInt("ScreenMode", index);
            PlayerPrefs.Save();
        }

        private void ApplyScreenMode(int index)
        {
            switch (index)
            {
                case 0: // Tela Cheia
                    Screen.fullScreen = true;
                    break;
                case 1: // Modo Janela
                    Screen.fullScreen = false;
                    break;
            }
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

}
