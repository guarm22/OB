using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DataPersistenceManager : MonoBehaviour
{
    [Header("File Storage Config")]
    [SerializeField] private string fileName;

    private FileDataHandler dataHandler;

    public static DataPersistenceManager instance {get; private set;} 
    private GameData gameData;
    private List<IDataPersistence> dataPersistenceObjects;
    
    private void Awake()
    {
        if(instance != null) { Debug.Log("oopsie"); }

        instance = this;
    }

    private void Start() {
        this.dataHandler = new FileDataHandler(Application.persistentDataPath, fileName);
        this.dataPersistenceObjects = FindAllDataPersistenceObjects();
        LoadGame();
    }

    public void NewGame() {
        this.gameData = new GameData();
    }

    public void LoadGame() {
        //load saved data
        this.gameData = dataHandler.Load();

        //if no data can be loaded, initialize new data
        if(this.gameData == null) {
            Debug.Log("No data was found, initializing new data");

            NewGame();
        }

        //push data to other scripts
        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects) {
            dataPersistenceObj.LoadData(gameData);
        }
    }

    public void SaveGame() {
        //allow other scripts to update data
        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects) {
            dataPersistenceObj.SaveData(ref gameData);
        }

        //save to file
        dataHandler.Save(gameData);
    }

    public void OnApplicationQuit()
    {
        SaveGame();
    }

    private List<IDataPersistence> FindAllDataPersistenceObjects() {
        IEnumerable<IDataPersistence> dataPersistenceObejcts = FindObjectsOfType<MonoBehaviour>().OfType<IDataPersistence>();

        return new List<IDataPersistence>(dataPersistenceObejcts);
    }
}
