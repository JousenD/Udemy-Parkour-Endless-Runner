using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class UI_Main : MonoBehaviour
{
    public bool gamePaused;
    
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject endGame;
    [Space]

    [SerializeField] private TextMeshProUGUI lastScoreText;
    [SerializeField] private TextMeshProUGUI highScoreText;
    [SerializeField] private TextMeshProUGUI coinsText;

    [Header("Volume Sliders")]
    [SerializeField] private UI_VolumeSlider[] slider;
    private void Start()
    {
        for (int i = 0; i < slider.Length; i++)
        {
            slider[i].SetupSlider();
        }
        SwitchMenuTo(mainMenu);
        Time.timeScale = 1;

        lastScoreText.text = "Last score:  " + PlayerPrefs.GetFloat("LastScore").ToString("#,#");
        highScoreText.text = "High score:  " + PlayerPrefs.GetFloat("HighScore").ToString("#,#");
    }
    public void SwitchMenuTo(GameObject uiMenu)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
        uiMenu.SetActive(true);

        AudioManager.instance.PlaySFX(4);
        coinsText.text = PlayerPrefs.GetInt("Coins").ToString("#,#");
    }

    public void StartGameButton() => GameManager.instance.UnlockPlayer();

    public void PauseGameButton()
    {
        if (gamePaused)
        {
            Time.timeScale = 1;
            gamePaused = false;
        }
        else
        {
            Time.timeScale = 0;
            gamePaused = true;
        }
    }

    public void RestartGameButton() => GameManager.instance.RestartLevel();

    public void OpenEndGameUI()
    {
        SwitchMenuTo(endGame);
    }
}
