using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    
    public UI_Main ui;
    public Player player;
    public bool colorEntirePlatform;

    [Header("Skybox Materials")]
    [SerializeField] private Material[] skyboxMat;

    [Header("Purchased Color")]
    public Color platformColor;
    public Color playerColor = Color.white;
    
    
    public int coins;
    public float distance;
    public float score;
    // Start is called before the first frame update
    private void Awake() 
    {
        instance = this;
        Time.timeScale = 1;
        
        SetupSkyBox(PlayerPrefs.GetInt("SkyBoxSetting"));
        //LoadColor();
    }

    public void SaveColor(float r, float g, float b)
    {
        PlayerPrefs.SetFloat("ColorR", r);
        PlayerPrefs.SetFloat("ColorG", g);
        PlayerPrefs.SetFloat("ColorB", b);
    }

    public void SetupSkyBox(int i)
    {
        if( i<= 1)
            RenderSettings.skybox = skyboxMat[i];
        else
            RenderSettings.skybox = skyboxMat[Random.Range(0,skyboxMat.Length)];

        PlayerPrefs.SetInt("SkyBoxSetting", i);
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

        score = distance * coins;

        PlayerPrefs.SetFloat("LastScore", score);

        if(PlayerPrefs.GetFloat("HighScore") < score)
            PlayerPrefs.SetFloat("HighScore", score);
    }

    public void GameEnded()
    {
        SaveInfo();
        ui.OpenEndGameUI();
    }
}
