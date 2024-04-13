using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
        //LoadColor();
    }

    public void SaveColor(float r, float g, float b)
    {
        PlayerPrefs.SetFloat("ColorR", r);
        PlayerPrefs.SetFloat("ColorG", g);
        PlayerPrefs.SetFloat("ColorB", b);
    }
    private void LoadColor() 
    {
        Debug.Log(PlayerPrefs.GetInt("Coins", 0));

        SpriteRenderer sr = player.GetComponent<SpriteRenderer>();

        Color newColor = new Color(PlayerPrefs.GetFloat("ColorR"), 
                                   PlayerPrefs.GetFloat("ColorG"), 
                                   PlayerPrefs.GetFloat("ColorB"), 
                                   PlayerPrefs.GetFloat("Color", 1));

        sr.color = newColor;
    }

    private void Update()
    {
        if(player.transform.position.x > distance)
            distance= player.transform.position.x;    
    }

    public void UnlockPlayer() => player.playerUnlocked = true;
    public void RestartLevel()
    {
        SaveInfo();
        SceneManager.LoadScene(0);
    } 

    public void SaveInfo()
    {
        int savedCoins = PlayerPrefs.GetInt("Coins");
        PlayerPrefs.SetInt("Coins", savedCoins + coins);

        float score = distance * coins;

        PlayerPrefs.SetFloat("LastScore", score);

        if(PlayerPrefs.GetFloat("HighScore") < score)
            PlayerPrefs.SetFloat("HighScore", score);
    }
}
