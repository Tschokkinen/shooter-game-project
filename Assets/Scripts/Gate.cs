using UnityEngine;
using UnityEngine.SceneManagement;

public class Gate : MonoBehaviour
{
    //Get all nessesary ScriptableObjects
    [SerializeField] private ScriptableFloat level;
    [SerializeField] private ScriptableBool exitGateActive;
    [SerializeField] private ScriptableFloat playTime;
    [SerializeField] private ScriptableFloat totalPlayTime;

    //Define save
    Save save;

    void Start()
    {
        //Get Save.cs script
        save = this.GetComponent<Save>();
    }

    //Detects if player is at the gate
    private void OnCollisionEnter2D(Collision2D other)
    {

        if (other.gameObject.tag == "Player" && exitGateActive.value)
        {
            level.value++; //Level up
            totalPlayTime.value += playTime.value; //Update total play time

            //Update both save systems
            save.SaveData();
            SaveSystem.SavePlayer(save);

            SceneManager.LoadScene("GenerationTest", LoadSceneMode.Single); //Reset map for new level
            exitGateActive.value = false; //Set gate off to prevent advancing without boss kill
            AstarPath.active.Scan(); //Scan paths for new level
        }
    }
}
