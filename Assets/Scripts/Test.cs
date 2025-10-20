using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Test : MonoBehaviour {
    private void OnMove(InputValue inputValue) {
        SceneManager.LoadScene("Scenes/WorldFactory");
    }
}
