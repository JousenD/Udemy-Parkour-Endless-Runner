using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    
    public Player player;
    public bool colorEntirePlatform;
    [Header("Color Info")]
    public Color platformColor;
    public Color playercolor = Color.white;
    
    
    public int coins;
    public float distance;
    // Start is called before the first frame update
    private void Awake() 
    {
        instance = this;
    }

    private void Update()
    {
        if(player.transform.position.x > distance)
            distance= player.transform.position.x;    
    }

    public void UnlockPlayer() => player.playerUnlocked = true;
    public void RestartLevel() => SceneManager.LoadScene(0);
}
