using Dialogue;
using UnityEngine;

public class ExitFactory : MonoBehaviour {
    [SerializeField] private NpcDialogue dialogueData;
    
    private void OnCollisionEnter2D(Collision2D other) {
        if (WorldManager.Instance.AreAllDead()) {
            // LoadScene
        }

        if (DialogueManager.Instance.CanInteract()) {
            DialogueManager.Instance.Interact(dialogueData);
        }
    }
}