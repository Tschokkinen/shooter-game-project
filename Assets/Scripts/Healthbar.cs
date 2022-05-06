using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class Healthbar : MonoBehaviour
{
    //Get nessesary ScriptableObjects
    [SerializeField]private ScriptableFloat playTime;
    [SerializeField]private ScriptableFloat totalPlayTime;

    [SerializeField]private Timer timer;
    [SerializeField]private Image healthbarImage;
    [SerializeField]private TextMeshProUGUI playerDead;

    Save save;

    // Start is called before the first frame update
    void Start()
    {
        if(healthbarImage == null)
        {
            healthbarImage = transform.Find("Health").GetComponent<Image>();
        }

        if(timer == null)
        {
            timer = GameObject.Find("Timer").GetComponent<Timer>();
        }

        save = this.GetComponent<Save>();
    }

    //Reduce player health
    public void ReduceHealth(float value)
    {
        healthbarImage.fillAmount -= value;

        if(healthbarImage.fillAmount == 0)
        {
            Debug.Log("Player dead!");
            totalPlayTime.value += playTime.value;
            GameObject player = GameObject.Find("Player");
            player.gameObject.SetActive(false);
            playerDead.gameObject.SetActive(true);
            timer.playerAlive = false; //Stop timer

            save.SaveData();
            SaveSystem.SavePlayer(save);
            Debug.Log("Game was saved because player died");

            SceneManager.LoadScene(sceneName: "DeathScene");
        }
    }

    //Increase player health
    public void AddHealth(float value)
    {
        healthbarImage.fillAmount += value;
    }
}
