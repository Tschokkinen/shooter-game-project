using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.IO;

public class SceneController : MonoBehaviour
{
    //Get all nessesary ScribtableObjects
    [SerializeField]private ScriptableBool isPaused;
    [SerializeField]private ScriptableFloat level;
    [SerializeField]private ScriptableFloat playTime;
    [SerializeField]private ScriptableFloat totalPlayTime;
    [SerializeField]private ScriptableBool exitGateActive;
    [SerializeField]private ScriptableInt kills;
    [SerializeField]private ScriptableInt bossKills;
    [SerializeField]private ScriptableInt wowKills;

    //Define button and text objects
    Button newGame;
    Button loadSave;
    Button exit;
    TextMeshProUGUI noSave;

    //Define bool 
    private bool saveFound;

    //Update is called once per frame
    void Start()
    {
        //Get references to GameObject's Button components
        newGame = GameObject.Find("NewGame").GetComponent<Button>();
        loadSave = GameObject.Find("LoadSave").GetComponent<Button>();
        exit = GameObject.Find("Exit").GetComponent<Button>();

        //Get references to output text object
        noSave = GameObject.Find("NoSave").GetComponent<TextMeshProUGUI>();
        noSave.gameObject.SetActive(false);

        //Set click listeners
        newGame.onClick.AddListener(() => buttonClicked(newGame));
        loadSave.onClick.AddListener(() => buttonClicked(loadSave));
        exit.onClick.AddListener(() => buttonClicked(exit));
    }

    //Method for buttons
    private void buttonClicked(Button button)
    {
        if(button == newGame)
        {
            //Loads scene for info scene
            SceneManager.LoadScene(sceneName: "InfoScene");
        }

        else if (button == exit)
        {
            //Closes game
            Application.Quit();
        }

        else if (button == loadSave)
        {
            //Gets path for safe file
            string path = Application.persistentDataPath + "/saveData.txt";

            //If file is found
            if (File.Exists(path))
            {
                SaveData data = SaveSystem.LoadPlayer();

                //Sets values for saved game
                level.value = data.level;
                kills.value = data.kills;
                bossKills.value = data.bossKills;
                totalPlayTime.value = data.totalPlayTime;

                //Nessesary values for basic game functions
                exitGateActive.value = false;
                isPaused.value = false;

                //Loads scene for main game
                SceneManager.LoadScene(sceneName: "GenerationTest");
            }

            else
            {
                //If no save is found
                StartCoroutine(FadeInText(1f, noSave));
            }
        }
    }

    //Mehod for fading in text to screen
    private IEnumerator FadeInText(float t, TextMeshProUGUI i)
    {
        noSave.gameObject.SetActive(true);

        i.color = new Color(i.color.r, i.color.g, i.color.b, 0);
        while (i.color.a < 1.0f)
        {
            i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a + (Time.deltaTime / t));
            yield return null;
        }
        //When text is completely faded in, starts method to fade it out
        StartCoroutine(FadeOutText(1f, noSave));
    }

    //Method for fading out text from screen
    private IEnumerator FadeOutText(float t, TextMeshProUGUI i)
    {
        i.color = new Color(i.color.r, i.color.g, i.color.b, 1);
        while (i.color.a > 0.0f)
        {
            i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a - (Time.deltaTime / t));
            yield return null;
        }
        noSave.gameObject.SetActive(false);
    }
}
