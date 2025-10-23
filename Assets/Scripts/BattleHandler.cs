using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class BattleHandler : MonoBehaviour {
    private State _state;
    private static BattleHandler _instance;
    
    [SerializeField] private Transform pfCharacterBattle;
    public Sprite playerSprite;
    public Sprite enemySprite;
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
            Debug.Log("State: " + _state);
            _enemyCharacterBattle.Attack(_playerCharacterBattle, () => {
                Debug.Log("Enemy Attack Finished!");

                SelectNextActiveCharacter();
                Debug.Log("State: " + _state);
            });
        } else {
            SetActiveCharacterBattle(_playerCharacterBattle);
            _state = State.WaitingForInput;
        }
    }

    private bool IsBattleOver() {
        if (_playerCharacterBattle.IsDead()) {
            Debug.Log("Enemy Wins!");
            SceneManager.LoadScene("Scenes/EndGameMenu");
            return true;
        }

        if (!_enemyCharacterBattle.IsDead()) return false;
        
        Debug.Log("Player Wins!");
        WorldManager.Instance.DamagePlayer(_playerCharacterBattle.GetLife());
        SceneManager.LoadScene("Scenes/WorldFactory");
        return true;
    }
    
    /* Combat functions. */
    
    private void OnAttack(InputValue inputValue) {
        switch (_state) {
            case State.WaitingForInput:
                _state = State.Busy;
                _playerCharacterBattle.Attack(_enemyCharacterBattle, SelectNextActiveCharacter);
                break;
            case State.EnemyTurn:
                _state = State.Busy;
                _playerCharacterBattle.StartDefending();
                break;
        }
    }

    private void OnJump() {
        if (_state != State.EnemyTurn) return;
        
        _state = State.Busy;
        _playerCharacterBattle.Dodge();
    }
}
