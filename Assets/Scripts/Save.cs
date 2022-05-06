using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Save : MonoBehaviour
{
    //Get all nessesary ScribtableObjects
    [SerializeField] private ScriptableFloat Level;
    [SerializeField] private ScriptableInt Kills;
    [SerializeField] private ScriptableInt BossKills;
    [SerializeField] private ScriptableFloat TotalPlayTime;

    //Define and give base values
    public float level = 0f;
    public int kills = 0;
    public int bossKills = 0;
    public float totalPlayTime = 0f;

    //Method for transfering data from ScribtableObjects to values
    public void SaveData()
    {
        level = Level.value;
        kills = Kills.value;
        bossKills = BossKills.value;
        totalPlayTime = TotalPlayTime.value;
    }

}
