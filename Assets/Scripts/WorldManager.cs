using System.Collections.Generic;
using System.Linq;
using Sound;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldManager : MonoBehaviour {
    public static WorldManager Instance {get; private set;}

    private Vector3 OriginalPlayerPosition { get; } = new(5, -7, 0);
    public Vector3 PlayerPosition { get; private set; } = new(5, -7, 0);
    
    public PlayerStatsData PlayerStats { get; private set; }

    [SerializeField] private List<GameObject> enemies;
    private List<Transform> _enemiesSpawns;
    private readonly List<bool> _enemiesAlive = new();

    public GameObject LastEnemyEncountered;
    public RuntimeAnimatorController LastEnemyAnimatorController;
    public bool IsLastEnemyBoss { get; private set; }
    public EnemyStatsData LastEnemyStats { get; private set; }
    
    private void Awake() {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        for (var i = 0; i < enemies.Count; i++) {
            _enemiesAlive.Add(true);
        }
    }
    
    public void InitializePlayerStats(PlayerStats stats) {
        if (PlayerStats == null && stats != null) {
            PlayerStats = stats.GetData();
        }
    }

    public void Setup() {
        PlayerStats.Reset();
        PlayerPosition = OriginalPlayerPosition;

        for (var i = 0; i < _enemiesAlive.Count; i++) {
            _enemiesAlive[i] = true;
        }
    }

    /* Scene's functions. */

    private void OnEnable() {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable() {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        if (scene.name != "WorldFactory") {
            SoundManager.PlayBackground(null);
            return;
        };

        SoundManager.PlayBackground("factory_background");
        
        var spawn = GameObject.Find("Spawns");
        _enemiesSpawns = new List<Transform>();
        foreach (Transform child in spawn.transform) {
            _enemiesSpawns.Add(child);
        }
        
        for (var i = 0; i < enemies.Count; i++) {
            if (_enemiesAlive[i]) {
                Instantiate(enemies[i], _enemiesSpawns[i].position, Quaternion.identity);
            }
        }
    }
    
    /* Enemy's functions. */

    public void SetEnemyEncoutered(GameObject enemyInstance, bool isBoss, EnemyStats enemyStats) {
        LastEnemyEncountered = enemyInstance;
        IsLastEnemyBoss = isBoss;
        LastEnemyStats = enemyStats != null ? enemyStats.GetData() : null;

        WorldEnemy worldEnemy = enemyInstance.GetComponent<WorldEnemy>();
        if (worldEnemy != null) {
            Animator animator = worldEnemy.GetComponent<Animator>();
            if (animator != null) {
                LastEnemyAnimatorController = animator.runtimeAnimatorController;
            }
        }
    }

    public void SetEnemyDead(int index) {
        _enemiesAlive[index] = false;
    }

    public bool AreAllDead() {
        return _enemiesAlive.All(state => !state);
    }
    
    /* Player's functions. */

    public void UpdatePlayerPosition(Vector3 newPlayerPosition) {
        PlayerPosition = newPlayerPosition;
    }
}
