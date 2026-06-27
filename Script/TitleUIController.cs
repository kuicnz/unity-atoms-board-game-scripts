using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TitleUIController : MonoBehaviour 
{
    public GameObject audioManager;

    public GameObject background;
    public GameObject transitionBackground;
    public GameObject tilePrefab;
    //public GameObject atomsPrefab;
    public Material white;
    public Material black;

    int column = 5;
    int row = 8;
    float distanceBetweenTiles = 2.25f;
    bool gameStart = false;
    Text[] animStart;
    GameObject canvas;
    Animator animControl;

    public GameObject titlePage;
    public GameObject inGameUI;
    public static int numPlayers = 2;
    public GameObject titleRim;
    public Text playerText;
    public Button startButton;
    public Button minusButton;
    public Button plusButton;

    public Sprite whiteMoon;
    public Sprite blackMoon;
    public GameObject TitlePageUI;
    public Text gameTitleText;
    public Button nightModeButton;

    public Button volumnButton;
    public Sprite whiteVolumnUp;
    public Sprite whiteVolumnDown;
    public Sprite blackVolumnUp;
    public Sprite blackVolumnDown;

    public static bool darkTheme;
    public static bool mute;

    Color Greyout = new Color(0.5f, 0.5f, 0.5f);

    void Start()
    {
        animStart = startButton.GetComponentsInChildren<Text>();
        canvas = startButton.transform.parent.parent.gameObject;
        animControl = canvas.GetComponent<Animator>();
        InitialNightMode();
        startButton.onClick.AddListener(InitialiseGame);

        playerText.text = numPlayers.ToString();

        minusButton.onClick.AddListener(delegate {
            AddPlayers(false);
        });
        plusButton.onClick.AddListener(delegate {
            AddPlayers(true);   
        });

        nightModeButton.onClick.AddListener(NightMode);
        volumnButton.onClick.AddListener(Mute);

        if(numPlayers == 2)
            minusButton.GetComponentInChildren<Text>().color = Greyout;
        else if(numPlayers == 6)
            plusButton.GetComponentInChildren<Text>().color = Greyout;
    }

    void Update()
    {
        playerText.text = numPlayers.ToString();
        PlusMinusColourPicker();
    }

    void AddPlayers(bool yes)
    {
        if (yes && numPlayers < 6)
        {
            animControl.SetTrigger("Plus");
            audioManager.GetComponent<AudioManager>().Play("pickSound");
            numPlayers++;
        }
        else if (!yes && numPlayers > 2)
        {
            animControl.SetTrigger("Minus");
            audioManager.GetComponent<AudioManager>().Play("pickSound");
            numPlayers--;
        }
    }

    void InitialiseGame()
    {
        if (gameStart)
            return;
        
        gameStart = true;
        animControl.SetTrigger("Start");
        StartCoroutine(GameStartAnim());
        audioManager.GetComponent<AudioManager>().Play("startSound");
        float xOffset = 0.0f;
        float zOffset = 0.0f;

        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < column; j++)
            {
                GameObject currentTile = Instantiate(tilePrefab, new Vector3(transform.position.x + xOffset, transform.position.y, transform.position.z + zOffset), transform.rotation);
                //GameObject currentAtoms = Instantiate(atomsPrefab, new Vector3(currentTile.transform.position.x, currentTile.transform.position.y + 1, currentTile.transform.position.z), currentTile.transform.rotation);
                //currentAtoms.transform.parent = currentTile.transform;
                //currentAtoms.SetActive(false);
                //currentAtoms.GetComponent<Spin>().enabled = false;
                //foreach (Transform child in currentAtoms.transform)
                    //child.gameObject.SetActive(false);
                
                xOffset += distanceBetweenTiles;

                if (j == column - 1)
                {
                    zOffset += distanceBetweenTiles;
                    xOffset = 0;
                }
            }
        }
        GetComponent<Atoms>().GameInitialiser();
    }

    IEnumerator GameStartAnim()
    {
        animControl.SetTrigger("Start");
        yield return new WaitForSeconds(0.3f);
        titlePage.SetActive(false);
        inGameUI.SetActive(true);
    }

    void NightMode()
    {
        audioManager.GetComponent<AudioManager>().Play("nightmodeSound");
        if(!darkTheme)
        {
            titlePage.GetComponent<Image>().color = Color.black;
            transitionBackground.GetComponent<Image>().color = Color.black;

            gameTitleText.color = Color.white;
            playerText.color = Color.white;
            foreach (Text text in animStart)
                text.color = Color.white;
            
            plusButton.transform.GetChild(0).GetComponent<Text>().color = Color.white;
            minusButton.transform.GetChild(0).GetComponent<Text>().color = Color.white;
            titleRim.GetComponent<Image>().color = Color.white;
            nightModeButton.GetComponent<Image>().sprite = whiteMoon;
            background.GetComponent<Renderer>().material = black;
            tilePrefab.GetComponent<Renderer>().material = black;
            darkTheme = true;
        }

        else if(darkTheme)
        {
            TitlePageUI.GetComponent<Image>().color = Color.white;
            transitionBackground.GetComponent<Image>().color = Color.white;

            gameTitleText.color = Color.black;
            playerText.color = Color.black;
            foreach (Text text in animStart)
                text.color = Color.black;
            
            plusButton.transform.GetChild(0).GetComponent<Text>().color = Color.black;
            minusButton.transform.GetChild(0).GetComponent<Text>().color = Color.black;
            titleRim.GetComponent<Image>().color = Color.black;
            nightModeButton.GetComponent<Image>().sprite = blackMoon;
            background.GetComponent<Renderer>().material = white;
            tilePrefab.GetComponent<Renderer>().material = white;
            darkTheme = false;
        }

        VolumnSpriteChanger();
    }

    void InitialNightMode()
    {
        if (darkTheme)
        {
            titlePage.GetComponent<Image>().color = Color.black;
            transitionBackground.GetComponent<Image>().color = Color.black;
            gameTitleText.color = Color.white;
            playerText.color = Color.white;

            foreach (Text text in animStart)
                text.color = Color.white;
            plusButton.transform.GetChild(0).GetComponent<Text>().color = Color.white;
            minusButton.transform.GetChild(0).GetComponent<Text>().color = Color.white;
            nightModeButton.GetComponent<Image>().sprite = whiteMoon;
            background.GetComponent<Renderer>().material = black;
            tilePrefab.GetComponent<Renderer>().material = black;
            VolumnSpriteChanger();
        }

        else if (!darkTheme)
        {
            TitlePageUI.GetComponent<Image>().color = Color.white;
            transitionBackground.GetComponent<Image>().color = Color.white;
            gameTitleText.color = Color.black;
            playerText.color = Color.black;

            foreach (Text text in animStart)
                text.color = Color.black;
            plusButton.transform.GetChild(0).GetComponent<Text>().color = Color.black;
            minusButton.transform.GetChild(0).GetComponent<Text>().color = Color.black;
            nightModeButton.GetComponent<Image>().sprite = blackMoon;
            background.GetComponent<Renderer>().material = white;
            tilePrefab.GetComponent<Renderer>().material = white;
            VolumnSpriteChanger();
        }
    }

    void Mute()
    {
        if(mute)
        {
            foreach (AudioSource sound in audioManager.GetComponents<AudioSource>())
                sound.mute = false;
            mute = false;
        }
        else if(!mute)
        {
            foreach (AudioSource sound in audioManager.GetComponents<AudioSource>())
                sound.mute = true;
            mute = true;
        }
        audioManager.GetComponent<AudioManager>().Play("pickSound");
        VolumnSpriteChanger();
    }

    void VolumnSpriteChanger()
    {
        if (mute && darkTheme)
            volumnButton.GetComponent<Image>().sprite = whiteVolumnDown;
        else if(mute && !darkTheme)
            volumnButton.GetComponent<Image>().sprite = blackVolumnDown;
        else if(!mute && darkTheme)
            volumnButton.GetComponent<Image>().sprite = whiteVolumnUp;
        else if(!mute && !darkTheme)
            volumnButton.GetComponent<Image>().sprite = blackVolumnUp;
    }

    void PlusMinusColourPicker()
    {
        if (numPlayers == 2)
            minusButton.GetComponentInChildren<Text>().color = Greyout;

        else if (numPlayers == 3)
        {
            if(!darkTheme)
                minusButton.GetComponentInChildren<Text>().color = Color.black;
            else
                minusButton.GetComponentInChildren<Text>().color = Color.white;
        }
        else if (numPlayers == 6)
            plusButton.GetComponentInChildren<Text>().color = Greyout;

        else if (numPlayers == 5)
        {
            if(!darkTheme)
                plusButton.GetComponentInChildren<Text>().color = Color.black;
            else
                plusButton.GetComponentInChildren<Text>().color = Color.white;
        }   
    }
}
