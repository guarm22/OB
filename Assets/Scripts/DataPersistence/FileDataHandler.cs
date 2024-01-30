using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class FileDataHandler 
{
   private string dataDirPath = "";
   private string dataFileName = "";

   public FileDataHandler(string dataDirPath, string dataFileName) {
        this.dataDirPath = dataDirPath;
        this.dataFileName = dataFileName;
   }

   public GameData Load() {
        string fullPath = Path.Combine(dataDirPath, dataFileName);
        GameData loadedData = null;
        if(File.Exists(fullPath)) {
            try {
                //load data from file
                string dataToLoad = "";
                using(FileStream stream = new FileStream(fullPath, FileMode.Open)) {
                    using(StreamReader reader = new StreamReader(stream)) {
                        dataToLoad= reader.ReadToEnd();
                    }
                }
                loadedData = JsonUtility.FromJson<GameData>(dataToLoad);
            }
            catch (Exception e) {
                Debug.Log("Error occured while loading from " + fullPath);
            }   
            return loadedData;
        }
        return null;
   }

   public GameData Save(GameData data) {
        //path.combine accounts for different operating systems
        string fullPath = Path.Combine(dataDirPath, dataFileName);
        try {
            //create directory in case it doesnt exist
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            //serialze game data into json
            string dataToStore = JsonUtility.ToJson(data, true);

            using(FileStream stream = new FileStream(fullPath, FileMode.Create)) {
                using(StreamWriter writer = new StreamWriter(stream)) {
                    writer.Write(dataToStore);
                }    
            }
        }
        catch (Exception e) {
            Debug.Log("Error occured while saving to " + fullPath);
        }
        return null;
   }
}
