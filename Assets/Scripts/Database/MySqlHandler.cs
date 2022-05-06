using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MySql.Data.MySqlClient;
using System;

public class MySqlHandler : MonoBehaviour
{
    private MySqlConnection connection;
    private MySqlCommand command;
    public bool initialized;
    // Start is called before the first frame update
    void Start()
    {
        //Set connection credentials
        string server = "mysql.metropolia.fi";
        string db = "mikajki";
        string user = "mikajki";
        string password = "peli";

        //Create connection and command
        connection = new MySqlConnection("server=" + server + ";database=" + db + ";user=" + user + ";password=" + password + ";");
        command = connection.CreateCommand();
        try
        {
            connection.Open();
            Debug.Log("Database Connected");
            initialized = true;
        }
        catch (MySqlException e)
        {
            Debug.Log(String.Format("MySQL connection Error: " + e.Message));
        }
        catch (Exception exception)
        {
            Debug.Log("Error while trying to connect to database: " + exception.Message);
        }
    }

    public void Insert(Score toInsert)
    {
        // Add player score into database
        command.CommandText = "INSERT INTO Player(name, level, score, kills, bossKills, totalPlayTime) VALUES(" + toInsert + ");";
        if (command != null)
        {
            try
            {
                command.ExecuteNonQuery();
            }
            catch (MySqlException e)
            {
                Debug.Log(e.Message);
            }
            catch (Exception exception)
            {
                Debug.Log(exception.Message);
            }
        }
    }
    public List<Score> Query(string query)
    {
        // Use string given as parameter to query database
        command.CommandText = query;
        List<Score> scores = new List<Score>();
        if (command != null)
        {
            try
            {
                MySqlDataReader reader = command.ExecuteReader();

                // Create scores based on query return fields
                while (reader.Read())
                {
                    Score score = new Score(reader.GetString("name"), reader.GetFloat("level"), reader.GetInt32("kills"), reader.GetInt32("bossKills"), reader.GetFloat("totalPlayTime"));
                    Debug.Log("Fields in query" + reader.FieldCount);
                    scores.Add(score);
                }
                reader.Close();

            }
            catch (MySqlException e)
            {
                Debug.Log(String.Format("MySQL Error: " + e.Message));
            }
            catch (Exception exception)
            {
                Debug.Log(String.Format("Error: ", exception.Message));
            }
        }
        if (scores != null)
        {
            return scores;
        }
        else
        {
            return null;
        }
    }

    // Close connection and dispose of waste
    void OnApplicationQuit()
    {
        if (command != null)
        {
            command.Dispose();
        }

        if (connection != null)
        {
            connection.Close();
            connection.Dispose();
        }
    }

}

