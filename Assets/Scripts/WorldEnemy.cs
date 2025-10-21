using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldEnemy : MonoBehaviour {
    [SerializeField] private int index;
    
    private void OnCollisionEnter2D(Collision2D other) {
        var world = FindFirstObjectByType<WorldManager>();
        
        world.SetPlayerPosition(other.transform.position);
        world.SetEnemyDead(index);
        SceneManager.LoadScene("Scenes/CombatScene");
    }
}
