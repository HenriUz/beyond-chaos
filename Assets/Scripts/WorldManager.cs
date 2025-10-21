using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldManager : MonoBehaviour {
    private static WorldManager _instance;

    private int _playerLife = 100;
    private Vector3 _playerPosition = new(5, -7, 0);

    [SerializeField] private List<GameObject> enemies;
    private List<Transform> _enemiesSpawns;
    private readonly List<bool> _enemiesAlive = new();
    
    private void Awake() {
        if (_instance != null) {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);

        for (var i = 0; i < enemies.Count; i++) {
            _enemiesAlive.Add(true);
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
    
    public int GetPlayerLife() {
        return _playerLife;
    }

    public void SetPlayerLife(int newPlayerLife) {
        _playerLife = newPlayerLife;
    }
    
    public Vector3 GetPlayerPosition() {
        return _playerPosition;
    }

    public void SetPlayerPosition(Vector3 newPlayerPosition) {
        _playerPosition = newPlayerPosition;
    }
}
