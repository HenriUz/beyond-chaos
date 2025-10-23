using Dialogue;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitFactory : MonoBehaviour {
    [SerializeField] private NpcDialogue dialogueData;
    
    private void OnCollisionEnter2D(Collision2D other) {
        if (WorldManager.Instance.AreAllDead()) {
            SceneManager.LoadScene("Scenes/EndGameMenu");
            return;
        }

        if (DialogueManager.Instance.CanInteract()) {
            DialogueManager.Instance.Interact(dialogueData);
        }
    }
}