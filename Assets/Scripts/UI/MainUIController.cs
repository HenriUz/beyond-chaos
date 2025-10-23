using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{ 
    public class MainUIController : MonoBehaviour
    {
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        public void StartGame()
        {
            SceneManager.LoadScene("EndGameMenu");
        }

        // Update is called once per frame
        public void Exit()
        {
            Application.Quit();
        }
    }
}
