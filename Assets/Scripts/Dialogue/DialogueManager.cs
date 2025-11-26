using System.Collections;
using Sound;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Dialogue {
    public class DialogueManager : MonoBehaviour {
        public static DialogueManager Instance { get; private set; }

        private NpcDialogue _dialogueData;
        [SerializeField] private GameObject dialoguePanel;
        [SerializeField] private TMP_Text dialogueText, nameText;
        [SerializeField] private Image portraitImage;
        
        private int _dialogueIndex;
        private bool _isTyping, _isDialogueActive;

        private void Awake() {
            if (Instance != null) {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        
        /* Interaction functions. */
    
        public bool CanInteract() {
            return !_isDialogueActive;
        }

        public void Interact(NpcDialogue npcDialogue) {
            if (npcDialogue == null || (PauseManager.IsGamePaused && !_isDialogueActive)) return;
            
            _dialogueData = npcDialogue;
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
            
            nameText.SetText(_dialogueData.npcName);
            portraitImage.sprite = _dialogueData.npcPortrait;

            dialoguePanel.SetActive(true);
            PauseManager.SetPause(true);
            
            StartCoroutine(TypeLine());
        }

        private void NextLine() {
            if (_isTyping) {
                // Skip typing animation and show the full line.
                StopAllCoroutines();
                dialogueText.SetText(_dialogueData.dialogueLines[_dialogueIndex]);
                _isTyping = false;
            } else if (++_dialogueIndex < _dialogueData.dialogueLines.Length) {
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

            foreach (var letter in _dialogueData.dialogueLines[_dialogueIndex]) {
                dialogueText.text += letter;
                SoundManager.PlayVoice(_dialogueData.voiceSound, _dialogueData.voicePitch);
                yield return new WaitForSeconds(_dialogueData.typingSpeed);
            }
            
            _isTyping = false;

            if (_dialogueData.autoProgressLines.Length <= _dialogueIndex || !_dialogueData.autoProgressLines[_dialogueIndex]) yield break;
            yield return new WaitForSeconds(_dialogueData.autoProgressDelay);
            NextLine();
        }

        public void EndDialogue() {
            StopAllCoroutines();
            _isDialogueActive = false;
            dialogueText.SetText("");
            dialoguePanel.SetActive(false);
            PauseManager.SetPause(false);
        }
    }
}
