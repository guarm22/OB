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

    public int DoAnomalyAction(bool enable) {
        Debug.Log("Anomaly on " + this.Name + " in " + this.Room + " of type " + AnomalyTypeToString(this.data.type) + 
        (enable ? " ENABLED" : " DISABLED"));

        if(enable) {
            this.divTime = Time.time;
        }
        else {
            this.divTime = -1f;
        }
        
        if(Obj.GetComponent<CustomDivergence>() != null) {
            Obj.GetComponent<CustomDivergence>().DoDivergenceAction(enable, this);
            return 1;
        }

        switch(this.data.type) {
            case ANOMALY_TYPE.Disappearance:
                this.ObjectDisappearance(enable);
                break;
            
            case ANOMALY_TYPE.Replacement:
                this.ObjectChange(enable);
                break;

            case ANOMALY_TYPE.Creature:
                //done through custom class
                break;

            case ANOMALY_TYPE.Audio:
                //also done through custom class, but im bad at coding
                this.Audio(enable);
                break;

            case ANOMALY_TYPE.Movement:
                //done through custom class
                break;
        }
        return 1;
    }

//Makes an object disappear or reappear based on the enable argument
    private void ObjectDisappearance(bool enable) {
        float moveAmt = enable ? 100f : -100f;
        Vector3 vec = new Vector3(this.Obj.transform.position.x, this.Obj.transform.position.y+moveAmt, this.Obj.transform.position.z);
        this.Obj.transform.position = vec;
    }
    private void Audio(bool enable) {
        this.Obj.GetComponent<PlayAudio>().enabled = enable;
    }

    private void ObjectChange(bool enable) {
        Transform replacement = this.Obj.transform.GetChild(0);
        Vector3 old = replacement.position;
        replacement.localPosition = new Vector3(0, enable ? 10f : -10f, 0);
        this.Obj.transform.position = old; 
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