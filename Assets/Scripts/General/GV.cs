

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

enum Scenes
{
    Menu,
    game
}

public enum GameModes : int
{
    Single, //only defense against spawners
    Multi, //defend against ai or whatever
    Menu, //at menu screen
}

public class GV : MonoBehaviour
{
    #region Singleton


    internal static GV instance;

    public static GV Singleton()
    {
        if (!instance) instance = FindObjectOfType<GV>();
        return instance;
    }
    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(this);
        gameStart();
    }

    private void Start()
    {
        
    }

    private IEnumerator RemoveTouchText()
    {
        yield return new WaitForSecondsRealtime(3);
        touchText.enabled = false;
    }

    #endregion


    #region Variables

    public GameObject playerGun;
    public GameObject PlayerMinion;
    public TMPro.TextMeshProUGUI scoreText;
    public TMPro.TextMeshProUGUI levelText;
    public TMPro.TextMeshProUGUI touchText;
    public GameObject fracturedMinion1;
    public GameObject powerUp;
    public GameObject playerGoal;
    public GameObject gameOverCanvas;
    internal GameObject playerOne;
    internal GameObject playerTwo;

    Vector3 playgroundSize;
    [SerializeField] internal GameModes gameModes = GameModes.Single;
    internal Color enemyColor = new Color(1, .1f, .1f);
    internal int score = 0;
    internal bool powerUpAvailable = true;
    internal float level = 1;
    internal float levelBase = 7;
    internal float generation = 0;
    internal bool showTouchText = true;
    internal bool hasGameEnded = false;

    #endregion

    #region functions

    public delegate void GameEnded();
    public GameEnded gameEnded;

    public void SetGameModeAndLoadScene(GameModes mode)
    {
        SetGameMode(mode);
        LoadGame();
    }

    public void gameStart()
    {
        if (gameModes != GameModes.Menu)
        {
            playgroundSize = GameObject.Find("Plane").GetComponent<Renderer>().bounds.size;
            scoreText = GameObject.Find("score").GetComponent<TMPro.TextMeshProUGUI>();
            touchText = GameObject.Find("touch text").GetComponent<TMPro.TextMeshProUGUI>();
            levelText = GameObject.Find("level text").GetComponent<TMPro.TextMeshProUGUI>();
            gameOverCanvas = GameObject.Find("game over canvas");
            gameOverCanvas.active = false;
            scoreText.text = score.ToString();
            StartCoroutine(RemoveTouchText());
        }
    }

    public void SetGameMode(GameModes mode)
    {
        gameModes = mode;    
    }

    public void setPlayerOne(GameObject p1)
    {
        playerOne = p1;
    }

    public void setPlayerTwo(GameObject p2)
    {
        playerTwo = p2;
    }

    public void SetGameEnded(bool ended = true)
    {
        hasGameEnded = true;
        gameEnded();
        gameOverCanvas.SetActive(true);
        gameOverCanvas.transform.GetChild(1).gameObject.GetComponent<TMPro.TextMeshProUGUI>().text = score.ToString();
    }

    public Vector3 getRandompointOnPlane()
    {
        var z = UnityEngine.Random.Range(-playgroundSize.z / 2, playgroundSize.z / 2);
        var x = UnityEngine.Random.Range(-playgroundSize.x / 2, playgroundSize.x / 2);
       
        return new Vector3(x,0,z);
    }

    public void UpdateScoreText()
    {
        score++;
        scoreText.text = (  score * level).ToString();
        if (score % levelBase == 0) Invoke( "UpdateLevelText", 0);
    }

    public void UpdateGeneration(int gen)
    {
        generation = gen;
        levelText.text = level.ToString() + "\n Generation" + generation.ToString();
    }

    public delegate void LevelUpdated(bool value = true);
    public LevelUpdated levelUpdated;
    public void UpdateLevelText()
    {
        level++;
        levelText.text = "Level " + level.ToString();
        levelUpdated();
    }

    public bool IsSingleMode()
    {
        return gameModes == GameModes.Single;
    }
    #endregion


    #region Scene manager

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("main menu");
    }

    public void LoadGame()
    {
        SceneManager.LoadScene("game");
    }

    public void LoadSingleGame() => SetGameModeAndLoadScene(GameModes.Single);
    public void LoadMultiGame() => SetGameModeAndLoadScene(GameModes.Multi);
    public void LoadMenu() => SetGameModeAndLoadScene(GameModes.Menu); // never call

    #endregion
}
