using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum GameMode
{
    idle,
    playing,
    levelEnd
}
public class MissionDemolition : MonoBehaviour
{
    static private MissionDemolition S; // add a private singleton

    [Header("Inscribed")]
    public Text uitLevel;   // the UIText_Level Text
    public Text uitShots;   // the UIText_Shots Text
    public Vector3 castlePos;   // the place to put castles
    public GameObject[] castles;   // An array of the castles

    [Header("Game Over UI")]
    public GameObject gameOverPanel;
    public Text gameOverScore;
    public Text gameOverStarsText;

    [Header("Dynamic")]
    public int level;   // the current level
    public int levelMax;   // the number of levels
    public int shotsTaken;
    public GameObject castle;   // the current castle
    public GameMode mode = GameMode.idle;
    public string showing = "Show Slingshot";   // FollowCam mode
    void Start()
    {
        S = this;   // define the singleton

        level = 0;
        shotsTaken = 0;
        levelMax = castles.Length;

        StartLevel();
    }

    // Calculates stars based on shots taken
    private int CalculateStars()
    {
        if (shotsTaken <= 5) return 4;
        if (shotsTaken <= 10) return 3;
        if (shotsTaken <= 20) return 2;
        return 1;
    }

    void StartLevel()
    {
        // Get rid of the old castle if one exists
        if (castle != null)
        {
            Destroy(castle);
        }

        // Destory old prohectiles if they exist (TODO)
        Projectile.DESTROY_PROJECTILES();

        // instantiate the new castle
        castle = Instantiate<GameObject>(castles[level]);
        castle.transform.position = castlePos;

        // Reset the goal
        Goal.goalMet = false;

        UpdateGUI();

        mode = GameMode.playing;

        // Zoom out to show both
        FollowCam.SWITCH_VIEW(FollowCam.eView.both);
    }

    void UpdateGUI()
    {
        // show the data in the GUITexts
        uitLevel.text = "LeveL: " + (level + 1) + " of " + levelMax;
        uitShots.text = "Shots Taken: " + shotsTaken;
    }

    void Update()
    {
        UpdateGUI();

        // check for level end
        if ((mode == GameMode.playing) && Goal.goalMet)
        {
            //change mode to stop checking for level end
            mode = GameMode.levelEnd;

            // Zoom out to show both
            FollowCam.SWITCH_VIEW(FollowCam.eView.both);

            // start the next level in 2 seconds
            Invoke("NextLevel", 2f);
        }
    }

    void NextLevel()
    {
        level++;
        if (level == levelMax)
        {
            GameOver();
            return;
        }
        StartLevel();
    }

    void GameOver()
    {
        mode = GameMode.levelEnd;

        // show both camera view
        FollowCam.SWITCH_VIEW(FollowCam.eView.both);

        //show Game Over UI
        gameOverPanel.SetActive(true);

        // display final score
        gameOverScore.text = "Score: " + shotsTaken;

        int starsEarned = CalculateStars();
        int maxStars = 4;
        if (gameOverStarsText != null)
            gameOverStarsText.text = "Stars: " + starsEarned + " / " + maxStars;
    }
    // Static method that allows code anywhere to incement shotsTaken
    static public void SHOT_FIRED()
    {
        S.shotsTaken++;
    }

    // Static method that allows code anywhere to get a reference to S.castle
    static public GameObject GET_CASTLE()
    {
        return S.castle;
    }

    public void RestartGame()
    {
        level = 0;
        shotsTaken = 0;

        gameOverPanel.SetActive(false);

        StartLevel();
    }
}
