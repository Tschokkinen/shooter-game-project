using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Scoreboeard : MonoBehaviour
{
    MySqlHandler sqlHandler;
    private Score bottomHighScore;
    private List<Score> scores;

    void Start()
    {

        sqlHandler = GetComponent<MySqlHandler>();
    }

    public void AddScore(Score score)
    {
        // Insert score with player name into database
        string name = GameObject.Find("PlayerName").GetComponent<InputField>().text;
        sqlHandler.Insert(score);
    }

    // Get lowest highscore to check if player is in top 10
    public float GetBottomHighscore()
    {
        string query = "SELECT * FROM Player ORDER BY score DESC LIMIT 10;";
        scores = sqlHandler.Query(query);
        Debug.Log("Scores" + scores.Count);
        if (scores.Count != 0)
        {
            bottomHighScore = scores[9];
            return bottomHighScore.getScore();
        }
        return 0f;
    }

    //Print highscores formatted correctly
    public void PrintHighScores()
    {
        // Make sure scores are set
        if (scores == null)
        {
            Debug.Log("No scores available");
            return;
        }

        // Only print scores that are available
        int max = 10;
        if (scores.Count < max)
        {
            max = scores.Count;
        }

        List<GameObject> players = new List<GameObject>();
        for (int i = 1; i < max; i++)
        {
            players.Add(GameObject.Find("Player" + i));

            players[i - 1].GetComponent<TextMeshProUGUI>().text = scores[i-1].GetScoreboardFormattedText();
        }

    }
}
