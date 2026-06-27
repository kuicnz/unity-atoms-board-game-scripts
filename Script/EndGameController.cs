using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class EndGameController : MonoBehaviour 
{
    public GameObject audioManager;
    public GameObject endGameUI;
    public GameObject titleUI;

    public Text winnerText;
    public Text winsText;

    public Button backButton;

    bool isDark = false;
    GameObject canvas;

    Text[] animBack;


    void Start()
    {
        animBack = backButton.GetComponentsInChildren<Text>();
        canvas = endGameUI.transform.parent.gameObject;
    }

    void EndGameAwake(Player player)
    {
        audioManager.GetComponent<AudioManager>().Play("victorySound");
        backButton.onClick.AddListener(BackToMain);
        GetComponent<Atoms>().enabled = false;

        if (player.colour == "Red")
        {
            winnerText.color = player.material.color;
            winnerText.text = "R e d";
        }
        if (player.colour == "Green")
        {
            winnerText.color = player.material.color;
            winnerText.text = "G r e e n";
        }
        if (player.colour == "Blue")
        {
            winnerText.color = player.material.color;
            winnerText.text = "B l u e";
        }
        if (player.colour == "Orange")
        {
            winnerText.color = player.material.color;
            winnerText.text = "O r a n g e";
        }
        if (player.colour == "Yellow")
        {
            winnerText.color = player.material.color;
            winnerText.text = "Y e l l o w";
        }
        if (player.colour == "Qing")
        {
            winnerText.color = player.material.color;
            winnerText.text = "C y a n";
        }
        
        if(TitleUIController.darkTheme && !isDark)
        {
            endGameUI.GetComponent<Image>().color = Color.black;
            winsText.color = Color.white;
            foreach (Text text in animBack)
                text.color = Color.white;
            isDark = true;
        }

        else if(!TitleUIController.darkTheme && isDark)
        {
            endGameUI.GetComponent<Image>().color = Color.white;
            winsText.color = Color.black;
            foreach (Text text in animBack)
                text.color = Color.white;
            isDark = false;
        }

        StartCoroutine(EndGameAnim());
    }

    void BackToMain()
    {
        StartCoroutine(LoadGame());
    }

    IEnumerator LoadGame()
    {
        Time.timeScale = 1f;
        canvas.GetComponent<Animator>().SetTrigger("Main");
        audioManager.GetComponent<AudioManager>().Play("pickSound");
        yield return new WaitForSeconds(0.2f);
        GetComponent<Atoms>().enabled = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        GetComponent<Fading>().BeginFade(1);
        yield break;
    }

    IEnumerator EndGameAnim()
    {
        canvas.GetComponent<Animator>().SetTrigger("End");
        yield return new WaitForSeconds(0.2f);
        Time.timeScale = 0f;
    }
}
