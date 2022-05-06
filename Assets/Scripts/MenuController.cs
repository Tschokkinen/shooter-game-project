using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    //Get ScribtableBool
    [SerializeField] private ScriptableBool isPaused;

    //Define pauseMenu
    private GameObject pauseMenu;

    //Define Buttons and Save
    Button returnToGame;
    Button saveGame;
    Button returnToMenu;
    Save save;

    // Start is called before the first frame update
    void Start()
    {
        //Finding all nessesary GameObjects and Components
        pauseMenu = GameObject.Find("PauseMenu");
        returnToGame = GameObject.Find("ReturnToGame").GetComponent<Button>();
        returnToMenu = GameObject.Find("ReturnToMain").GetComponent<Button>();

        //Getting Save.cs script
        save = this.GetComponent<Save>();

        //Set click listeners
        returnToGame.onClick.AddListener(() => buttonClicked(returnToGame));
        returnToMenu.onClick.AddListener(() => buttonClicked(returnToMenu));

        //Hiding pauseMenu
        pauseMenu.SetActive(false);

        //Setting game status to not paused
        isPaused.value = false;
    }

    // Update is called once per frame
    void Update()
    {
        //Defines ESC key functionality
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //Returns to game if pauseMenu is up
            if (isPaused.value)
            {
                ReturnToGame();
            }

            //Opens pauseMenu if game was playing
            else
            {
                isPaused.value = true;
                pauseMenu.SetActive(true);
                Time.timeScale = 0f;
            }
        }
    }

    //Method when unpausing
    private void ReturnToGame()
    {
        isPaused.value = false;
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
    }

    //Method for returning to main menu
    private void ReturnToMain()
    {
        Time.timeScale = 1f;
        isPaused.value = false;
        SceneManager.LoadScene(sceneName: "StartScreen");
    }

    //Method for button clicks
    private void buttonClicked(Button button)
    {
        if (button == returnToGame)
        {
            ReturnToGame();
        }
        else if (button == returnToMenu)
        {
            ReturnToMain();
        }
    }
}
