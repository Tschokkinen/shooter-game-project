using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class InfoSceneController : MonoBehaviour
{
    //Get all nessesary ScribtableObjects
    [SerializeField] private ScriptableBool isPaused;
    [SerializeField] private ScriptableFloat level;
    [SerializeField] private ScriptableFloat playTime;
    [SerializeField] private ScriptableFloat totalPlayTime;
    [SerializeField] private ScriptableBool exitGateActive;
    [SerializeField] private ScriptableInt kills;
    [SerializeField] private ScriptableInt bossKills;
    [SerializeField] private ScriptableInt wowKills;

    Button start;

    // Start is called before the first frame update
    void Start()
    {
        start = GameObject.Find("Start").GetComponent<Button>();

        start.onClick.AddListener(() => buttonClicked(start));
    }

    private void buttonClicked(Button button)
    {
        if (button == start)
        {
            //Nessesary values for new game
            exitGateActive.value = false;
            isPaused.value = false;
            level.value = 1f;
            playTime.value = 0f;
            totalPlayTime.value = 0f;
            kills.value = 0;
            bossKills.value = 0;
            wowKills.value = 0;

            SceneManager.LoadScene(sceneName: "GenerationTest");
        }
    }
}
