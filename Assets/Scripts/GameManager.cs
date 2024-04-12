using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    
    public Player player;
    public bool colorEntirePlatform;
    public Color platformColor;
    public int coins;

    // Start is called before the first frame update
    private void Awake() 
    {
        instance = this;
    }

    public void UnlockPlayer() => player.playerUnlocked = true;
    public void RestartLevel() => SceneManager.LoadScene(0);
}
