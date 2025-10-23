using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldManager : MonoBehaviour {
    public static WorldManager Instance {get; private set;}

    private int OriginalPlayerLife { get; } = 100;
    public int PlayerLife { get; private set; } = 100;
    private Vector3 OriginalPlayerPosition { get; } = new(5, -7, 0);
    public Vector3 PlayerPosition { get; private set; } = new(5, -7, 0);

    [SerializeField] private List<GameObject> enemies;
    private List<Transform> _enemiesSpawns;
    private readonly List<bool> _enemiesAlive = new();
    
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

    public void Setup() {
        PlayerLife = OriginalPlayerLife;
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
        if (scene.name != "WorldFactory") return;

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

    public void SetEnemyDead(int index) {
        _enemiesAlive[index] = false;
    }

    public bool AreAllDead() {
        return _enemiesAlive.All(state => !state);
    }
    
    /* Player's functions. */
    
    public void DamagePlayer(int newPlayerLife) {
        PlayerLife = newPlayerLife;
    }

    public void UpdatePlayerPosition(Vector3 newPlayerPosition) {
        PlayerPosition = newPlayerPosition;
    }
}
