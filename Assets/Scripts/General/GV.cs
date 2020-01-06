

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
        if (PlayerPrefs.HasKey("mode"))
        {
            SetGameMode((GameModes)PlayerPrefs.GetInt("mode", 2));
        }
        else
        {
            SetGameMode(gameModes);
        }



        gameStart();
    }

    private void Start()
    {
        Debug.Log(PlayerPrefs.GetString("mode"));

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
    public GameObject shell;
    internal GameObject playerOne;
    internal GameObject playerTwo;

   internal Vector3 playgroundSize;
    [SerializeField] internal GameModes gameModes = GameModes.Menu;
    internal Color enemyColor = new Color(1, .1f, .1f);
    internal int score = 0;
    internal bool powerUpAvailable = true;
    internal float level = 1;
    internal float levelBase = 4;
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
        PlayerPrefs.SetInt("mode", (int) mode);
        PlayerPrefs.Save();
        LoadGame();
    }

    private void OnLevelWasLoaded(int level)
    {

        
        gameStart();

    }

    public void gameStart()
    {
        
        if (gameModes != GameModes.Menu)
        {
            playgroundSize = GameObject.Find("stage").transform.Find("Plane").GetComponent<Renderer>().bounds.size;
            scoreText = GameObject.Find("score").GetComponent<TMPro.TextMeshProUGUI>();
            touchText = GameObject.Find("touch text").GetComponent<TMPro.TextMeshProUGUI>();
            levelText = GameObject.Find("level text").GetComponent<TMPro.TextMeshProUGUI>();
            scoreText.text = score.ToString();
            StartCoroutine(RemoveTouchText());

        }
    }

    private void OnApplicationQuit()
    {
        PlayerPrefs.DeleteAll();
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
        gameOverCanvas.transform.GetChild(1).gameObject.GetComponent<TMPro.TextMeshProUGUI>().text = scoreText.text;
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
        PlayerPrefs.SetInt("mode", 2);
        PlayerPrefs.Save();
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
