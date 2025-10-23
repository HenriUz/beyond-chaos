using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldEnemy : MonoBehaviour {
    [SerializeField] private int index;
    
    private void OnCollisionEnter2D(Collision2D other) {
        WorldManager.Instance.UpdatePlayerPosition(other.transform.position);
        WorldManager.Instance.SetEnemyDead(index);
        SceneManager.LoadScene("Scenes/CombatScene");
    }
}
