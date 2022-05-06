using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.IO;

public class DeathScene : MonoBehaviour
{
    //Get ScribtableBool paused
    [SerializeField] private ScriptableBool isPaused;

    //Defenition for button and text objects
    Button mainMenu;
    Button saveScore;
    TextMeshProUGUI level;
    TextMeshProUGUI kills;
    TextMeshProUGUI time;


    private GameObject saveScoreParent;
    private Scoreboeard scoreboard;
    private GameObject Playername;
    private string input;
    private bool dbInitialized;
    private Score score;

    //Update is called once per frame
    void Start()
    {
        //Get references to GameObject's Button components
        saveScoreParent = GameObject.Find("SaveScore");
        mainMenu = GameObject.Find("MainMenu").GetComponent<Button>();
        saveScore = saveScoreParent.GetComponent<Button>();

        // Get references for use in scoreboard
        saveScoreParent = GameObject.Find("SaveScore");
        Playername = GameObject.Find("PlayerName");

        //Get references to output text object
        level = GameObject.Find("Level").GetComponent<TextMeshProUGUI>();
        kills = GameObject.Find("Kills").GetComponent<TextMeshProUGUI>();
        time = GameObject.Find("Time").GetComponent<TextMeshProUGUI>();

        //Set click listeners
        mainMenu.onClick.AddListener(() => buttonClicked(mainMenu));
        saveScore.onClick.AddListener(() => buttonClicked(saveScore));

        //Initialize scoreboard
        scoreboard = GameObject.Find("Scoreboard").GetComponent<Scoreboeard>();
        dbInitialized = scoreboard.GetComponent<MySqlHandler>().initialized;

        // Hide highscore stuff if score is not high enough
        saveScoreParent.SetActive(false);
        Playername.SetActive(false);

        //Set status to pause
        isPaused.value = true;

        WaitForDatabase();

        PrintScores();
    }

    //Method for printing out stats to the death screen
    private void PrintScores()
    {
        //Loading save from file
        SaveData data = SaveSystem.LoadPlayer();

        //Printing out stats
        level.text = "You reached level " + data.level;
        kills.text = "You got " + (data.kills - data.bossKills) +
            " basic enemy kills and " + data.bossKills + " boss kills!";
        time.text = "Your total time was " + System.Math.Round(data.totalPlayTime, 2) + " seconds";

        // Initialize score
        score = new Score(data.level, data.kills, data.bossKills, data.totalPlayTime);

        //Check if score was better than lowest score in top 10
        if (scoreboard.GetBottomHighscore() < score.getScore())
        {
            saveScoreParent.SetActive(true);
            Playername.SetActive(true);
        }
        scoreboard.PrintHighScores();
        //Deleting save file after all info have been used to prevent reloadin save after death
        File.Delete(Application.persistentDataPath + "/saveData.txt");
    }

    //Method for buttons
    private void buttonClicked(Button button)
    {
        if (button == mainMenu)
        {   
            //Loads scene for main menu
            SceneManager.LoadScene(sceneName: "StartScreen");
        }
        else if (button == saveScore)
        {
            string name = Playername.GetComponent<InputField>().text;
            score.setName(name);

            // Add score to database
            scoreboard.AddScore(score);
            Debug.Log("Save score clicked");
        }
    }

    public void ReadStringInput(string i)
    {
        input = i;
        Debug.Log(input);
    }

    IEnumerator WaitForDatabase()
    {
        //Wait for database
        while (!dbInitialized)
        {
            yield return new WaitForSecondsRealtime(0.03f);
        }
    }
}