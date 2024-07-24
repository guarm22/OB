using UnityEngine;
using System.Collections.Generic;

public enum ANOMALY_TYPE {
    NONE,
    Disappearance,
    Replacement,
    Movement,
    Creature,
    Audio,
};

public class DynamicObject {
    public DynamicData data;
    public string Room {get; set;}
    public string Name {get; set;}
    public GameObject Obj {get; set;}
    public float divTime;
    public DynamicObject(DynamicData data, string room, string name, GameObject obj) {
        this.data = data;
        Name = name;
        Room = room;
        Obj = obj;
        divTime = -1f;
    }

    public bool DoAnomalyAction(bool enable) {
        Debug.Log("Divergence on " + this.Name + " in " + this.Room + " of type " + AnomalyTypeToString(this.data.type) + 
        (enable ? " ENABLED" : " DISABLED"));

        if(enable) {
            this.divTime = Time.time;
        }
        else {
            
            this.divTime = -1f;
        }
        
        if(Obj.GetComponent<CustomDivergence>() != null) {
            Obj.GetComponent<CustomDivergence>().DoDivergenceAction(enable, this);
            return true;
        }

        switch(this.data.type) {
            case ANOMALY_TYPE.Disappearance:
                return this.ObjectDisappearance(enable);
            
            case ANOMALY_TYPE.Replacement:
                return this.ObjectChange(enable);

            case ANOMALY_TYPE.Creature:
                //done through custom class
                break;

            case ANOMALY_TYPE.Audio:
                //also done through custom class, but im bad at coding
                return this.Audio(enable);

            case ANOMALY_TYPE.Movement:
                //done through custom class
                break;
        }
        return true;
    }

//Makes an object disappear or reappear based on the enable argument
    private bool ObjectDisappearance(bool enable) {
        float moveAmt = enable ? -100f : 100f;
        Vector3 vec = new Vector3(this.Obj.transform.position.x, this.Obj.transform.position.y+moveAmt, this.Obj.transform.position.z);
        this.Obj.transform.position = vec;

        return true;
    }
    private bool Audio(bool enable) {
        if(this.Obj.GetComponent<PlayAudio>() == null) {
            Debug.Log("No audio script found on object " + this.Name + " in " + this.Room);
            return false;
        }

        this.Obj.GetComponent<PlayAudio>().enabled = enable;
        return true;
    }

    private bool ObjectChange(bool enable) {
        Transform replacement = this.Obj.transform.GetChild(0);
        if(replacement == null) {
            Debug.Log("No replacement object found on object " + this.Name + " in " + this.Room);
            return false;
        }

        Vector3 old = replacement.position;
        replacement.localPosition = new Vector3(0, enable ? 10f : -10f, 0);
        this.Obj.transform.position = old; 
        return true;
    }

    public static List<string> GetAllAnomalyTypes() {
        List<string> res = new List<string>
        {
            "Disappearance",
            "Replacement",
            "Creature",
            "Audio",
            "Movement"
        };
        return res;
    }

    /// <summary>
    /// Searches through a GameObject's parents to find the room it is in.
    /// The function does this by checking the tag of the parent object, if the tag is "Room" it returns the name of the room.
    /// If no room is found, it searches that parent's parent, and so on.
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static string getRoomName(Transform obj) {
        while (obj != null) {
            if (obj.tag == "Room") {
                return obj.name;
            }
            obj = obj.parent;
        }
        return "";
    }

      public static ANOMALY_TYPE GetAnomalyTypeByName(string name) {
        switch(name){
            case "Disappearance":
                return ANOMALY_TYPE.Disappearance;
            case "Replacement":
                return ANOMALY_TYPE.Replacement;
            case "Creature":
                return ANOMALY_TYPE.Creature;
            case "Audio":
                return ANOMALY_TYPE.Audio;
            case "Movement":
                return ANOMALY_TYPE.Movement;
            default:
                return ANOMALY_TYPE.NONE;
        }
  }

    public static string AnomalyTypeToString(ANOMALY_TYPE type) {
        switch(type) {
            case ANOMALY_TYPE.NONE:
                return "NONE";
            case ANOMALY_TYPE.Disappearance:
                return "Disappearance";
            case ANOMALY_TYPE.Replacement:
                return "Replacement";
            case ANOMALY_TYPE.Creature:
                return "Creature";
            case ANOMALY_TYPE.Audio:
                return "Audio";
            case ANOMALY_TYPE.Movement:
                return "Movement";
            default:
                return "Error";
        }
    }
}