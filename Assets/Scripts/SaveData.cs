using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This file is used to convert information from Save.cs to form that is accessible by SaveSystem.cs

[System.Serializable]
public class SaveData
{
    //Declare variables used by SaveSystem.cs
    public float level;
    public int kills;
    public int bossKills;
    public float totalPlayTime;

    //Method to that converts data when called
    public SaveData(Save save)
    {
        level = save.level;
        kills = save.kills;
        bossKills = save.bossKills;
        totalPlayTime = save.totalPlayTime;
    }
}
