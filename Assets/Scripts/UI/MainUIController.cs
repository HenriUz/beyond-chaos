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

        public void Exit() {
            Application.Quit();
        }
    }
}
