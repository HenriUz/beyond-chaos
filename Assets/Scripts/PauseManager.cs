using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour {
    public static PauseManager Instance { get; private set; }

    public bool IsGamePaused { get; private set; }
    public bool IsMenuActive { get; private set; }
    [SerializeField] private GameObject pauseMenu;

    private void Awake() {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
    
    public void SetPause(bool pause) {
        IsGamePaused = pause;
    }

    private void SetMenu(bool active) {
        IsMenuActive = active;
        pauseMenu.SetActive(active);
    }
    
    public void ShowMenu() {
        if (!IsGamePaused) {
            IsGamePaused = true;
        }
        
        SetMenu(true);
    }

    public void Resume() {
        IsGamePaused = false;
        SetMenu(false);
    }

    public void MainMenu() {
        IsGamePaused = false;
        SetMenu(false);
        SceneManager.LoadScene("Scenes/MainMenu");
    }

    public void Exit() {
        Application.Quit();
    }
}
