using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI { 
    public class MainUIController : MonoBehaviour {
        public void StartGame() {
            if (WorldManager.Instance != null) {
                WorldManager.Instance.Setup();
            }
            
            SceneManager.LoadScene("Scenes/WorldFactory");
        }

        public void ReturnMainMenu()
        {
            SceneManager.LoadScene("Scenes/MainMenu");
        }

        public void Exit() {
            Application.Quit();
        }

        public void OpenConfigs()
        {
            SceneManager.LoadScene("Scenes/Configs");
        }
    }
}
