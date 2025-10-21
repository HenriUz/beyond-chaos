using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ExitFactory : MonoBehaviour {
    private WorldManager _worldManager;

    [SerializeField] private NpcDialogue dialogueData;
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TMP_Text dialogueText, nameText;
    [SerializeField] private Image portraitImage;
    
    private int _dialogueIndex;
    private bool _isTyping, _isDialogueActive;
    
    private void Start() {
        _worldManager = FindFirstObjectByType<WorldManager>();
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if (_worldManager.AreAllDead()) {
            // LoadScene
        }

        if (CanInteract()) {
            Interact();
        }
    }

    /* Interaction functions. */
    
    private bool CanInteract() {
        return !_isDialogueActive;
    }

    private void Interact() {
        if (dialogueData == null || _isDialogueActive) return;

        if (_isDialogueActive) {
            NextLine();
        } else {
            StartDialogue();
        }
    }
    
    /* Dialogue functions. */

    private void StartDialogue() {
        _isDialogueActive = true;
        _dialogueIndex = 0;
        
        nameText.SetText(dialogueData.npcName);
        portraitImage.sprite = dialogueData.npcPortrait;

        dialoguePanel.SetActive(true);
        
        StartCoroutine(TypeLine());
    }

    private void NextLine() {
        if (_isTyping) {
            // Skip typing animation and show the full line.
            StopAllCoroutines();
            dialogueText.SetText(dialogueData.dialogueLines[_dialogueIndex]);
            _isTyping = false;
        } else if (++_dialogueIndex < dialogueData.dialogueLines.Length) {
            // If another line, type next line.
            StartCoroutine(TypeLine());
        } else {
            // End dialogue.
            EndDialogue();
        }
    }
    
    private IEnumerator TypeLine() {
        _isTyping = true;
        dialogueText.SetText("");

        foreach (var letter in dialogueData.dialogueLines[_dialogueIndex]) {
            dialogueText.text += letter;
            yield return new WaitForSeconds(dialogueData.typingSpeed);
        }
        
        _isTyping = false;

        if (dialogueData.autoProgressLines.Length <= _dialogueIndex || !dialogueData.autoProgressLines[_dialogueIndex]) yield break;
        yield return new WaitForSeconds(dialogueData.autoProgressDelay);
        NextLine();
    }

    public void EndDialogue() {
        StopAllCoroutines();
        _isDialogueActive = false;
        dialogueText.SetText("");
        dialoguePanel.SetActive(false);
    }
}
