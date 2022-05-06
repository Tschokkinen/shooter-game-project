using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem
{
    //Method to save data to save file
    public static void SavePlayer(Save save)
    {
        //Binary formatter for encoding the data
        BinaryFormatter formatter = new BinaryFormatter();

        //Define location of save file
        string path = Application.persistentDataPath + "/saveData.txt";

        FileStream stream = new FileStream(path, FileMode.Create);

        SaveData data = new SaveData(save);

        formatter.Serialize(stream, data);
        stream.Close();
        Debug.Log("Data saved");
    }

    //Method for loading data from save file
    public static SaveData LoadPlayer()
    {
        //Define location of save file
        string path = Application.persistentDataPath + "/saveData.txt";

        //For accessing save
        if (File.Exists(path))
        {
            //Binary formatter for converting the data back to readable form
            BinaryFormatter formatter = new BinaryFormatter();

            FileStream stream = new FileStream(path, FileMode.Open);

            SaveData data = formatter.Deserialize(stream) as SaveData;
            stream.Close();
            Debug.Log("Data loaded");

            return data;
        }

        //Just in case file is missing and method is called
        else
        {
            Debug.LogError("No save file found in " + path);
            return null;
        }
    }

    //Method for deleting save file
    public static void DeleteSave() 
    {
        File.Delete(Application.persistentDataPath + "/savedData.txt");
    }
}