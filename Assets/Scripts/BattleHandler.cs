using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class BattleHandler : MonoBehaviour {
    private State _state;
    private static BattleHandler _instance;
    
    [SerializeField] private Transform pfCharacterBattle;
    
    [SerializeField] private Canvas worldCanvas;
    [SerializeField] private GameObject healthBarPrefab;
    [SerializeField] private GameObject damagePopupPrefab;
    [SerializeField] private List<KeyUI> playerTurnKeysUI;
    [SerializeField] private KeyUI specialKeyUI;

    public Canvas WorldCanvas => worldCanvas;
    public GameObject HealthBarPrefab => healthBarPrefab;
    public GameObject DamagePopupPrefab => damagePopupPrefab;
    public List<KeyUI> PlayerTurnKeysUI => playerTurnKeysUI;


    public RuntimeAnimatorController playerAnimatorOverride;
    public RuntimeAnimatorController enemyAnimatorOverride;

    private CharacterBattle _playerCharacterBattle;
    private CharacterBattle _enemyCharacterBattle;
    private CharacterBattle _activeCharacterBattle;

    private enum State {
        WaitingForInput,
        Busy,
        EnemyTurn
    }
    
    private void Awake() {
        _instance = this;
    }
    
    private void Start() {
        _playerCharacterBattle = SpawnCharacter(true);
        _enemyCharacterBattle = SpawnCharacter(false);       

        SetActiveCharacterBattle(_playerCharacterBattle);    
        _state = State.WaitingForInput;

        specialKeyUI.SetEnabled(false);
    }

    private void Update() {
        // Update UI based on player character's special ability status
        // if (_playerCharacterBattle != null && specialKeyUI != null) {
        //     specialKeyUI.SetEnabled(_playerCharacterBattle.IsSpecialAbilityReady());
        // }
    }
    
    public static BattleHandler GetInstance() {
        return _instance;
    }

    /* Setup functions. */
    
    private CharacterBattle SpawnCharacter(bool isPlayerTeam) {
        var position = isPlayerTeam ? new Vector3(-8, 0) : new Vector3(+8, 0);

        var characterTransform = Instantiate(pfCharacterBattle, position, Quaternion.identity);
        var characterBattle = characterTransform.GetComponent<CharacterBattle>();

        characterBattle.Setup(isPlayerTeam, WorldManager.Instance.PlayerLife);
        return characterBattle;
    }

    /* UI functions. */
    public void EnableUIKeys(bool enable) {
        foreach (var keyUI in playerTurnKeysUI) {
            keyUI.SetEnabled(enable);
        }
    }
    
    /* Turn management. */
    
    private void SetActiveCharacterBattle(CharacterBattle characterBattle) {
        if (_activeCharacterBattle != null) {
            _activeCharacterBattle.HideSelectionIndicator();
        }

        _activeCharacterBattle = characterBattle;
        _activeCharacterBattle.ShowSelectionIndicator();
    }

    private void SelectNextActiveCharacter() {
        if (IsBattleOver()) {
            return;
        }
        
        if (_activeCharacterBattle == _playerCharacterBattle) {
            SetActiveCharacterBattle(_enemyCharacterBattle);

            _state = State.EnemyTurn;
            // Debug.Log("State: " + _state);
            _enemyCharacterBattle.Attack(_playerCharacterBattle, () => {
                // Debug.Log("Enemy Attack Finished!");

                SelectNextActiveCharacter();
                // Debug.Log("State: " + _state);
            });
        } else {
            SetActiveCharacterBattle(_playerCharacterBattle);
            _state = State.WaitingForInput;
        }
    }

    private bool IsBattleOver() {
        if (_playerCharacterBattle.IsDead()) {
            // Debug.Log("Enemy Wins!");
            SceneManager.LoadScene("Scenes/EndGameMenu");
            return true;
        }

        if (!_enemyCharacterBattle.IsDead()) return false;
        
        // Debug.Log("Player Wins!");
        WorldManager.Instance.DamagePlayer(_playerCharacterBattle.GetLife());
        SceneManager.LoadScene("Scenes/WorldFactory");
        return true;
    }
    
    /* Combat functions. */
    
    private void OnAttack(InputValue inputValue) {
        if (_state != State.WaitingForInput) return;
        
        _state = State.Busy;
        _playerCharacterBattle.Attack(_enemyCharacterBattle, SelectNextActiveCharacter);
    }

    private void OnParry(InputValue inputValue) {
        if (_state != State.EnemyTurn) return;
        
        _state = State.Busy;
        _playerCharacterBattle.StartDefending();
    }

    private void OnJump() {
        if (_state != State.EnemyTurn) return;
        
        _state = State.Busy;
        _playerCharacterBattle.Dodge();
    }
}
