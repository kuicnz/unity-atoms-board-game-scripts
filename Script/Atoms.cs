using UnityEngine;
using UnityEngine.UI;

public class Atoms : MonoBehaviour 
{
    public Button option;
    public GameObject audioManager;
    public GameObject rim;
    public Material red;
    public Material green;
    public Material orange;
    public Material blue;
    public Material yellow;
    public Material qing;

    private GameObject[] eachTile;
    private Player[] players = new Player[TitleUIController.numPlayers];
    private Player currentPlayer;
    private int turn = 1;
    private Tile[,] tiles = new Tile[5,8];
    //private Tile previousTile;
    private Color lerp;
    private Color plain;

    bool gameEnd = false;
    bool isAnimating;

    private Ray ray;
    private RaycastHit hit;
    private bool nextTurn;

    public void GameInitialiser()
    {
        players = new Player[TitleUIController.numPlayers];
        InitGameBoard();
        InitPlayers();
        if (turn == 1)
            currentPlayer = players[0];
        nextTurn = true;
        ColourChanger(rim);
        option.GetComponentInChildren<Text>().color = currentPlayer.material.color;
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0) && nextTurn)
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 100))
            {
                if (hit.collider.tag == "Grid")
                {
                    Transform grid = hit.collider.gameObject.transform;
                    MakeMove(grid);
                    nextTurn = true;
                }
            }   
        }
    }
    //----------------------------------------------- Make a move ---------------------------------
    private void MakeMove(Transform grid)
    {
        nextTurn = false;
        Vector3 coordinates = CoordinatesConvertor(grid.position,"1");
        int x = (int)coordinates.x;
        int z = (int)coordinates.z;
        if(x < 0 && z < 0)
        {
            Debug.Log("Coordinates are fucked up!");
            return;
        }
        if(tiles[x,z].Owner != null && tiles[x,z].Owner.colour != currentPlayer.colour)
        {
            audioManager.GetComponent<AudioManager>().Play("deniedSound");
            return;
        }
        audioManager.GetComponent<AudioManager>().Play("placeSound");
        //lerp = currentPlayer.material.color;
        //Color blinkColor = Color.Lerp(plain, lerp, 0.85f);
        //tiles[x, z].grid.GetComponent<Renderer>().material.color = blinkColor;

        //if (previousTile != null)
        //    previousTile.grid.transform.GetChild(0).gameObject.SetActive(false);
        //    previousTile.grid.GetComponent<Renderer>().material.color = plain;
        //previousTile = tiles[x, z];

        TilesActivator(x, z);

        tiles[x, z].atomCount++;
        if (tiles[x, z].Owner == null || tiles[x, z].Owner != currentPlayer)
            currentPlayer.gridsOwned++;
        tiles[x, z].Owner = currentPlayer;
        ChainReaction(x, z);

        turn++;
        if (turn <= players.Length)
            currentPlayer = currentPlayer.nextPlayer;
        else if (turn > players.Length)
        {
            currentPlayer = currentPlayer.nextPlayer;
            while (currentPlayer.gridsOwned == 0)
                currentPlayer = currentPlayer.nextPlayer;
        }
        option.GetComponentInChildren<Text>().color = currentPlayer.material.color;
        ColourChanger(rim);

        //lerp = currentPlayer.material.color;
        //Color blinkColor = Color.Lerp(Color.white, lerp, 0.85f);
        //rim.GetComponent<Renderer>().material.color = blinkColor;
    }

    //----------------------------------------------- Initialise players -----------------------
    private void InitPlayers()
    {
        players[0] = new Player("Red", 0, null);
        players[0].material = red;

        players[1] = new Player("Green", 0, null);
        players[1].material = green;

        players[0].nextPlayer = players[1];
        players[1].nextPlayer = players[0];

        if(players.Length > 2)
        {
            players[2] = new Player("Orange", 0, players[0]);
            players[2].material = orange;

            players[1].nextPlayer = players[2];
        }

        if(players.Length > 3)
        {
            players[3] = new Player("Blue", 0, players[0]);
            players[3].material = blue;

            players[2].nextPlayer = players[3];
        }

        if(players.Length > 4)
        {
            players[4] = new Player("Yellow", 0, players[0]);
            players[4].material = yellow;

            players[3].nextPlayer = players[4];
        }

        if(players.Length > 5)
        {
            players[5] = new Player("Qing", 0, players[0]);
            players[5].material = qing;

            players[4].nextPlayer = players[5];
        }
    }

    //--------------------------------------------------- Initialise Game Board ----------------------------------
    private void InitGameBoard()
    {
        eachTile = GameObject.FindGameObjectsWithTag("Grid");
        for (int z = 0; z < 8; z++)
        {
            for (int x = 0; x < 5; x++)
            {
                tiles[x,z] = new Tile(null, 0);

                foreach (GameObject tempTile in eachTile)
                    if (Mathf.Approximately(tempTile.transform.position.x, x * 2.25f) && Mathf.Approximately(tempTile.transform.position.z, z * 2.25f))
                        tiles[x, z].grid = tempTile;   
            }
        }
        plain = tiles[0,0].grid.GetComponent<Renderer>().material.color;
    }

    //------------------------------------------------ Convert coordinates back and forth ------------------------------
    private Vector3 CoordinatesConvertor(Vector3 position, string scale)
    {
        if(scale == "1")
        {
            float x = position.x / 2.25f;
            float z = position.z / 2.25f;
            return new Vector3(x, position.y, z);
        }

        if (scale == "2.25")
        {
            float x = position.x * 2.25f;
            float z = position.z * 2.25f;
            return new Vector3(x, position.y, z);
        }

        return new Vector3(-1,-1,-1);
    }

    //---------------------------------------------------- Control the colour changes of the rim ------------------
    private void ColourChanger(GameObject needChange)
    {
        if(needChange.transform.childCount == 0)
            needChange.GetComponent<Renderer>().material.color = currentPlayer.material.color;

        else
        {
            foreach(Transform child in needChange.transform)
            {
                child.GetComponent<Renderer>().material.color = currentPlayer.material.color;
            }
        }
    }

    //------------------------------------------------------ Activate atom ------------------
    private void AtomsActivator(int x, int z)
    {
        int atomCount = tiles[x, z].atomCount;
        Transform hiddenAtoms = tiles[x, z].grid.transform.GetChild(0).transform;
        ColourChanger(hiddenAtoms.gameObject);

        if (atomCount == 0)
        {
            hiddenAtoms.GetChild(0).gameObject.SetActive(true);
            SpinSpeedSetting(x, z);
            hiddenAtoms.GetComponent<Spin>().enabled = true;
        }
        else if (atomCount == 1)
            hiddenAtoms.GetChild(1).gameObject.SetActive(true);
        else if (atomCount == 2)
            hiddenAtoms.GetChild(2).gameObject.SetActive(true);
        else if (atomCount == 3)
            hiddenAtoms.GetChild(3).gameObject.SetActive(true);
        
    }

    //----------------------------------------------- Activate tile -----------------------------------
    private void TilesActivator(int x, int z)
    {
        int atomCount = tiles[x, z].atomCount;
        Color recess1 = Color.Lerp(plain, currentPlayer.material.color, 0.75f);
        Color recess2 = Color.Lerp(plain, currentPlayer.material.color, 0.5f);

        if(TilePosChecker(x,z) == 1)
        {
            tiles[x, z].grid.transform.GetChild(1).gameObject.GetComponent<Renderer>().material.color = currentPlayer.material.color;
            tiles[x, z].grid.transform.GetChild(0).gameObject.GetComponent<Renderer>().material.color = currentPlayer.material.color;
            tiles[x, z].grid.GetComponent<Renderer>().material.color = currentPlayer.material.color;
        }

        else if(TilePosChecker(x,z) == 2)
        {
            tiles[x, z].grid.transform.GetChild(1).gameObject.GetComponent<Renderer>().material.color = currentPlayer.material.color;
            tiles[x, z].grid.transform.GetChild(0).gameObject.GetComponent<Renderer>().material.color = currentPlayer.material.color;
            tiles[x, z].grid.GetComponent<Renderer>().material.color = recess1;

            if (atomCount >= 1)
                tiles[x, z].grid.GetComponent<Renderer>().material.color = currentPlayer.material.color;
        }

        else if(TilePosChecker(x,z) == 3)
        {
            tiles[x, z].grid.transform.GetChild(1).gameObject.GetComponent<Renderer>().material.color = currentPlayer.material.color;
            tiles[x, z].grid.transform.GetChild(0).gameObject.GetComponent<Renderer>().material.color = recess1;
            tiles[x, z].grid.GetComponent<Renderer>().material.color = recess2;
            if (atomCount >= 1)
            {
                tiles[x, z].grid.transform.GetChild(0).gameObject.GetComponent<Renderer>().material.color = currentPlayer.material.color;
                tiles[x, z].grid.GetComponent<Renderer>().material.color = recess1;
            }
            if (atomCount >= 2)
                tiles[x, z].grid.GetComponent<Renderer>().material.color = currentPlayer.material.color;
        }
    }

    //--------------------------------------------- Check tile position ----------------------
    private int TilePosChecker(int x, int z)
    {
        //Corner case return 1
        if ((x == 0 || x == 4) && (z == 0 || z == 7))
            return 1;

        //Side case return 2
        else if ((x == 0 || x == 4 || z == 0 || z == 7))
            return 2;

        //Middle case return 3
        else if (x != 0 && x != 4 && z != 0 && z != 7)
            return 3;

        //error return 0
        else
            return 0;
            
    }

    //----------------------------------------------- Core Mechanics of the game ---------------------
    private void ChainReaction(int x, int z)
    {
        int count = 0;
        for (int i = 0; i < players.Length; i++)
        {
            if (turn > players.Length && players[i].gridsOwned != 0)
                count++;
        }

        if (count == 1)
        {
            if(!gameEnd)
            {
                SendMessage("EndGameAwake",currentPlayer);
                gameEnd = true;
            }
            return;
        }
        //SpinSpeedSetting(x, z);

        //-Corner Case------------------------------------
        if (tiles[x, z].atomCount >= 2 && (x == 0 || x == 4) && (z == 0 || z == 7))
        {
            audioManager.GetComponent<AudioManager>().Play("placeSound");
            //tiles[x, z].grid.transform.GetChild(0).GetComponent<Animator>().SetTrigger("Explode");
            tiles[x, z].Owner.gridsOwned--;
            tiles[x, z].atomCount = 0;
            tiles[x, z].Owner = null;

            tiles[x, z].grid.GetComponent<Renderer>().material.color = plain;
            tiles[x, z].grid.transform.GetChild(0).GetComponent<Renderer>().material.color = plain;
            tiles[x, z].grid.transform.GetChild(1).GetComponent<Renderer>().material.color = plain;

            //Transform currentAtom = tiles[x, z].grid.transform.GetChild(0).transform;
            //currentAtom.GetComponent<Spin>().enabled = false;
            //foreach (Transform child in currentAtom)
                //child.gameObject.SetActive(false);

            if (x == 4 && z == 0)
            {
                AtomExplode(x, z, "left");
                AtomExplode(x, z, "up");
            }

            if (x == 0 && z == 0)
            {
                AtomExplode(x, z, "up");
                AtomExplode(x, z, "right");
            }

            if (x == 0 && z == 7)
            {
                AtomExplode(x, z, "right");
                AtomExplode(x, z, "down");
            }

            if (x == 4 && z == 7)
            {
                AtomExplode(x, z, "left");
                AtomExplode(x, z, "down");
            }
        }

        //-Side Case--------------------------------------
        else if (tiles[x, z].atomCount >= 3 && (x == 0 || x == 4 || z == 0 || z == 7))
        {
            audioManager.GetComponent<AudioManager>().Play("placeSound");
            //tiles[x, z].grid.transform.GetChild(0).GetComponent<Animator>().SetTrigger("Explode");

            tiles[x, z].Owner.gridsOwned--;
            tiles[x, z].atomCount = 0;
            tiles[x, z].Owner = null;

            tiles[x, z].grid.GetComponent<Renderer>().material.color = plain;
            tiles[x, z].grid.transform.GetChild(0).GetComponent<Renderer>().material.color = plain;
            tiles[x, z].grid.transform.GetChild(1).GetComponent<Renderer>().material.color = plain;

            //Transform currentAtom = tiles[x, z].grid.transform.GetChild(0).transform;
            //currentAtom.GetComponent<Spin>().enabled = false;
            //foreach (Transform child in currentAtom)
                //child.gameObject.SetActive(false);

            if (x == 4)
            {
                AtomExplode(x, z, "left");
                AtomExplode(x, z, "up");
                AtomExplode(x, z, "down");
            }

            if (z == 7)
            {
                AtomExplode(x, z, "right");
                AtomExplode(x, z, "down");
                AtomExplode(x, z, "left");
            }

            if (x == 0)
            {
                AtomExplode(x, z, "up");
                AtomExplode(x, z, "right");
                AtomExplode(x, z, "down");
            }

            if (z == 0)
            {
                AtomExplode(x, z, "left");
                AtomExplode(x, z, "up");
                AtomExplode(x, z, "right");
            }
        }

        //-Middle Case-------------------------------------
        else if (tiles[x, z].atomCount >= 4 && x != 0 && x != 4 && z != 0 && z != 7)
        {
            audioManager.GetComponent<AudioManager>().Play("placeSound");

            //tiles[x, z].grid.transform.GetChild(0).GetComponent<Animator>().SetTrigger("Explode");

            tiles[x, z].Owner.gridsOwned--;
            tiles[x, z].atomCount = 0;
            tiles[x, z].Owner = null;

            tiles[x, z].grid.GetComponent<Renderer>().material.color = plain;
            tiles[x, z].grid.transform.GetChild(0).GetComponent<Renderer>().material.color = plain;
            tiles[x, z].grid.transform.GetChild(1).GetComponent<Renderer>().material.color = plain;

            //Transform currentAtom = tiles[x, z].grid.transform.GetChild(0).transform;
            //currentAtom.GetComponent<Spin>().enabled = false;
            //foreach (Transform child in currentAtom)
                //child.gameObject.SetActive(false);

            AtomExplode(x, z, "left");
            AtomExplode(x, z, "up");
            AtomExplode(x, z, "right");
            AtomExplode(x, z, "down");
        }

        else
            return;
    }

    private void AtomExplode(int x, int z, string direction)
    {
        int xsibling = x;
        int zsibling = z;

        if (direction == "up")
            zsibling = z + 1;

        if (direction == "right")
            xsibling = x + 1;

        if (direction == "down")
            zsibling = z - 1;

        if (direction == "left")
            xsibling = x - 1;

        if (tiles[xsibling, zsibling].Owner != null)
        {
            tiles[xsibling, zsibling].Owner.gridsOwned--;
        }
        //AtomsActivator(xsibling,zsibling);
        TilesActivator(xsibling, zsibling);
        tiles[xsibling, zsibling].Owner = currentPlayer;
        tiles[xsibling, zsibling].atomCount++;
        tiles[xsibling, zsibling].Owner.gridsOwned++;
        ChainReaction(xsibling, zsibling);
    }

    //- for endgame use----------------------------------------------
    public Color getWinnerColour()
    {
        return currentPlayer.material.color;
    }

    //- for setting speed of spinning---------------------------------
    private void SpinSpeedSetting(int x, int z)
    {
        if (tiles[x, z].atomCount == 1 && (x == 0 || x == 4) && (z == 0 || z == 7))
        {
            tiles[x, z].grid.transform.GetChild(0).GetComponent<Spin>().SetSpeed(200f);
        }

        else if (tiles[x, z].atomCount == 2 && (x == 0 || x == 4 || z == 0 || z == 7))
        {
            tiles[x, z].grid.transform.GetChild(0).GetComponent<Spin>().SetSpeed(200f);
        }

        else if (tiles[x, z].atomCount == 3 && x != 0 && x != 4 && z != 0 && z != 7)
        {
            tiles[x, z].grid.transform.GetChild(0).GetComponent<Spin>().SetSpeed(200f);
        }

        else
            tiles[x, z].grid.transform.GetChild(0).GetComponent<Spin>().SetSpeed(50f);
    }

    //- send playerStats to pauseMenu------------------------------------
    public void RequestPlayerStats()
    {
        SendMessage("PlayerStatsDisplay",players);
    }

    public void GameReset()
    {
        for (int i = 0; i < players.Length; i++)
            players[i].gridsOwned = 0;
        for (int z = 0; z < 8; z++)
        {
            for (int x = 0; x < 5; x++)
            {
                tiles[x, z].Owner = null;
                tiles[x, z].atomCount = 0;
               
                tiles[x, z].grid.GetComponent<Renderer>().material.color = plain;
                tiles[x, z].grid.transform.GetChild(0).GetComponent<Renderer>().material.color = plain;
                tiles[x, z].grid.transform.GetChild(1).GetComponent<Renderer>().material.color = plain;
                //Transform currentAtom = tiles[x, z].grid.transform.GetChild(0).transform;
                //currentAtom.GetComponent<Spin>().enabled = false;
                //foreach (Transform child in currentAtom)
                    //child.gameObject.SetActive(false);
                
            }
        }
        turn = 1;
        currentPlayer = players[0];
        ColourChanger(rim);
        //rim.GetComponent<Renderer>().material = currentPlayer.material;

        //lerp = currentPlayer.material.color;
        //Color blinkColor = Color.Lerp(plain, lerp, 0.85f);
        //rim.GetComponent<Renderer>().material.color = blinkColor;

        option.GetComponentInChildren<Text>().color = currentPlayer.material.color;
        //if(previousTile != null)
            //previousTile.grid.GetComponent<Renderer>().material.color = plain;
        GetComponent<PauseUI>().PauseActivate();
    }
}
