using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseUI : MonoBehaviour 
{
    bool isDark = false;
    bool pressed = false;

    public GameObject audioManager;
    public GameObject pauseMenu;

    GameObject canvas;
    Animator animControl;

    public Button option;
    public Button restart;
    public Button mainMenu;
    public Button backToGame;

    public Text redPlayer;
    public Text greenPlayer;
    public Text orangePlayer;
    public Text bluePlayer;
    public Text yellowPlayer;
    public Text cyanPlayer;

    Text[] animHome;
    Text[] animRestart;
    Text[] animBack;

    void Start()
    {
        animHome = mainMenu.GetComponentsInChildren<Text>();
        animRestart = restart.GetComponentsInChildren<Text>();
        animBack = backToGame.GetComponentsInChildren<Text>();

        canvas = pauseMenu.transform.parent.gameObject;
        animControl = canvas.GetComponent<Animator>();

        option.onClick.AddListener(PauseActivate);
        backToGame.onClick.AddListener(PauseActivate);
        restart.onClick.AddListener(GameRestart);
        mainMenu.onClick.AddListener(MainMenuActivate);
    }

    public void PauseActivate()
    {
        if (!animControl.GetBool("Paused"))
            StartCoroutine(PauseGame());
        else
            StartCoroutine(UnpauseGame());
    }

    void GameRestart()
    {
        animControl.SetTrigger("Reset_P");
        audioManager.GetComponent<AudioManager>().Play("startSound");
        GetComponent<Atoms>().GameReset();
    }

    void MainMenuActivate()
    {
        if (!pressed)
        {
            pressed = true;
            StartCoroutine(LoadGame());
        }
    }

    IEnumerator LoadGame()
    {
        Time.timeScale = 1f;
        canvas.GetComponent<Animator>().SetTrigger("Start_P");
        audioManager.GetComponent<AudioManager>().Play("pickSound");
        yield return new WaitForSeconds(0.2f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        GetComponent<Fading>().BeginFade(1);
        yield break;
    }

    IEnumerator PauseGame()
    {
        audioManager.GetComponent<AudioManager>().Play("pickSound");
        if (TitleUIController.darkTheme && !isDark)
        {
            pauseMenu.GetComponent<Image>().color = Color.black;
            restart.GetComponentInChildren<Text>().color = Color.white;
            mainMenu.GetComponentInChildren<Text>().color = Color.white;
            backToGame.GetComponentInChildren<Text>().color = Color.white;
            for (int i = 0; i < 3; i++)
            {
                animBack[i].color = Color.white;
                animHome[i].color = Color.white;
                animRestart[i].color = Color.white;
            }
            isDark = true;
        }

        else if (!TitleUIController.darkTheme && isDark)
        {
            pauseMenu.GetComponent<Image>().color = Color.white;
            restart.GetComponentInChildren<Text>().color = Color.black;
            mainMenu.GetComponentInChildren<Text>().color = Color.black;
            backToGame.GetComponentInChildren<Text>().color = Color.black;
            for (int i = 0; i < 3; i++)
            {
                animBack[i].color = Color.black;
                animHome[i].color = Color.black;
                animRestart[i].color = Color.black;
            }
            isDark = false;
        }

        GetComponent<Atoms>().RequestPlayerStats();
        GetComponent<Atoms>().enabled = false;
        animControl.SetBool("Paused",true);
        yield return null;
    }

    IEnumerator UnpauseGame()
    {
        animControl.SetBool("Paused", false);
        audioManager.GetComponent<AudioManager>().Play("pickSound");
        GetComponent<Atoms>().enabled = true;
        pressed = false;
        yield return null;
    }

    void PlayerStatsDisplay(Player[] players)
    {
        for (int i = 0; i < players.Length; i++)
        {
            if(players[i].colour == "Red")
            {
                redPlayer.gameObject.SetActive(true);
                redPlayer.text = "red: " + players[i].gridsOwned;
                redPlayer.color = players[i].material.color;
            }

            else if(players[i].colour == "Green")
            {
                greenPlayer.gameObject.SetActive(true);
                greenPlayer.text = "green: " + players[i].gridsOwned;
                greenPlayer.color = players[i].material.color;
            }

            else if (players[i].colour == "Orange")
            {
                orangePlayer.gameObject.SetActive(true);
                orangePlayer.text = "orange: " + players[i].gridsOwned;
                orangePlayer.color = players[i].material.color;
            }

            else if (players[i].colour == "Blue")
            {
                bluePlayer.gameObject.SetActive(true);
                bluePlayer.text = "blue: " + players[i].gridsOwned;
                bluePlayer.color = players[i].material.color;
            }

            else if (players[i].colour == "Yellow")
            {
                yellowPlayer.gameObject.SetActive(true);
                yellowPlayer.text = "yellow: " + players[i].gridsOwned;
                yellowPlayer.color = players[i].material.color;
            }

            else if (players[i].colour == "Qing")
            {
                cyanPlayer.gameObject.SetActive(true);
                cyanPlayer.text = "cyan: " + players[i].gridsOwned;
                cyanPlayer.color = players[i].material.color;
            }
        }
    }
}
