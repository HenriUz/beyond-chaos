using UnityEngine;

public class PauseManager : MonoBehaviour {
    public static bool IsGamePaused { get; private set; }

    public static void SetPause(bool pause) {
        IsGamePaused = pause;
    }
}
