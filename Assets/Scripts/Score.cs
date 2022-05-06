using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;


public class Score
{
    private string name;
    private float level;
    private int kills;
    private int bossKills;
    private float score;
    private float totalPlayTime;


    public Score(float level, int kills, int bossKills, float totalPlayTime)
    {
        this.level = level;
        this.kills = kills;
        this.bossKills = bossKills;
        this.totalPlayTime = totalPlayTime;

        this.score = ((kills * level) + (bossKills * (level * 2))) / (totalPlayTime / 10);
    }

    public Score(string name, float level, int kills, int bossKills, float totalPlayTime)
    {
        this.name = name;
        this.level = level;
        this.kills = kills;
        this.bossKills = bossKills;
        this.totalPlayTime = totalPlayTime;

        this.score = ((kills * level) + (bossKills * (level*2))) / (totalPlayTime/10);
    }

    public float getScore()
    {
        return this.score;
    }

    // Return stats with strings formatted for death screen scoreboard
    public string GetScoreboardFormattedText()
    {

        return this.name + new String(' ', 17 - this.name.Length)  
            + this.level +  new String(' ', 22 - Mathf.FloorToInt(this.level/10))
            + this.score +   new String(' ', 24 - Mathf.FloorToInt(this.score/100))
            + this.kills +  new String(' ', 18 - this.kills/10)
            + this.bossKills + new String(' ', 20 - Mathf.FloorToInt(this.bossKills / 10))
            + this.totalPlayTime;
    }

    public void setName(string name)
    {
        this.name = name;
    }

    override public string ToString()
    {
        return "\"" + this.name + "\", " + this.level + ", " + this.score + ", " + this.kills + ", " + this.bossKills + ", " + this.totalPlayTime;
    }

}
