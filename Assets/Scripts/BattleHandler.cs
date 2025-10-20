using UnityEngine;
using UnityEngine.InputSystem;

public class BattleHandler : MonoBehaviour {

    private State state;
    private static BattleHandler _instance;

    [SerializeField] private Transform pfCharacterBattle;
    public Sprite playerSprite;
    public Sprite enemySprite;
    public RuntimeAnimatorController playerAnimatorOverride;
    public RuntimeAnimatorController enemyAnimatorOverride;

    private CharacterBattle playerCharacterBattle;
    private CharacterBattle enemyCharacterBattle;
    private CharacterBattle activeCharacterBattle;

    public static BattleHandler GetInstance() {
        return _instance;
    }

    private enum State {
        WAITING_FOR_INPUT,
        BUSY,
        ENEMY_TURN
    }

    private void SetActiveCharacterBattle(CharacterBattle characterBattle) {
        if (activeCharacterBattle != null) {
            activeCharacterBattle.HideSelectionIndicator();
        }

        activeCharacterBattle = characterBattle;
        activeCharacterBattle.ShowSelectionIndicator();
    }

    private void Awake() {
        _instance = this;
    }

    private void Start() {
        playerCharacterBattle = SpawnCharacter(true);
        enemyCharacterBattle = SpawnCharacter(false);       

        SetActiveCharacterBattle(playerCharacterBattle);    
        state = State.WAITING_FOR_INPUT;
    }

    private CharacterBattle SpawnCharacter(bool isPlayerTeam) {
        Vector3 position = isPlayerTeam ? new Vector3(-8, 0) : new Vector3(+8, 0);

        Transform characterTransform = Instantiate(pfCharacterBattle, position, Quaternion.identity);
        CharacterBattle characterBattle = characterTransform.GetComponent<CharacterBattle>();
        characterBattle.Setup(isPlayerTeam);
        return characterBattle;
    }

    private void OnAttack(InputValue inputValue) {
        if (state == State.WAITING_FOR_INPUT) {
            state = State.BUSY;
            playerCharacterBattle.Attack(enemyCharacterBattle, () => {
                SelectNextActiveCharacter();
            });
        } else if (state == State.ENEMY_TURN) {
            state = State.BUSY;
            playerCharacterBattle.StartDefending();
        }
    }

    private void OnJump() {
        if (state == State.ENEMY_TURN) {
            state = State.BUSY;

            playerCharacterBattle.Dodge();
        }
    }

    private void SelectNextActiveCharacter() {
        if (IsBattleOver()) {
            return;
        }
        
        if (activeCharacterBattle == playerCharacterBattle) {
            SetActiveCharacterBattle(enemyCharacterBattle);

            state = State.ENEMY_TURN;
            Debug.Log("State: " + state);
            enemyCharacterBattle.Attack(playerCharacterBattle, () => {
                Debug.Log("Enemy Attack Finished!");

                SelectNextActiveCharacter();
                Debug.Log("State: " + state);
            });
        } else {
            SetActiveCharacterBattle(playerCharacterBattle);
            state = State.WAITING_FOR_INPUT;
        }
    }

    private bool IsBattleOver() {
        if (playerCharacterBattle.IsDead()) {
            Debug.Log("Enemy Wins!");
            return true;
        } else if (enemyCharacterBattle.IsDead()) {
            Debug.Log("Player Wins!");
            return true;
        } 

        return false;
    }
}
