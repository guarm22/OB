using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

public enum ANOMALY_TYPE {
    NONE,
    Light,
    ObjectDisappearance,
    ObjectChange,
    Movement,
    Creature,
    Audio,
};

public class DynamicObject {
    public DynamicData data;
    public string Room {get; set;}
    public string Name {get; set;}
    public bool normal;

    public GameObject Obj {get; set;}

    public Vector3 originalPosition {get; set;}

    public DynamicObject(DynamicData data, string room, string name, GameObject obj) {
        this.data = data;
        Name = name;
        Room = room;
        Obj = obj;
        normal = true;
        originalPosition = obj.transform.position;
    }

    public int DoAnomalyAction(bool enable) {
        if(enable == false) {
            this.data.beenKilled = true;
            this.data.cooldown = 20f;
            this.data.killedTime = Time.time;
        }
        else if(Time.time > 10 && Time.time <= this.data.killedTime + this.data.cooldownTime) {
            //there is a bug in this if statement: fix later if cooldown should be re added
            //return 0;
        }
        
        Debug.Log("Anomaly on " + this.Name + " in " + this.Room + " of type " + AnomalyTypeToString(this.data.type) + 
        (enable ? " ENABLED" : " DISABLED"));
        this.normal = !enable;
        this.data.beenKilled = false;

        if(Obj.GetComponent<CustomDivergence>() != null) {
            Obj.GetComponent<CustomDivergence>().DoDivergenceAction(enable, this);
            return 1;
        }

        switch(this.data.type) {
            case ANOMALY_TYPE.ObjectDisappearance:
                this.ObjectDisappearance(enable);
                break;

            case ANOMALY_TYPE.Light:
                this.Light(enable);
                break;
            
            case ANOMALY_TYPE.ObjectChange:
                this.ObjectChange(enable);
                break;

            case ANOMALY_TYPE.Creature:
                this.Creature(enable);
                break;

            case ANOMALY_TYPE.Audio:
                this.Audio(enable);
                break;

            case ANOMALY_TYPE.Movement:
                //not implemented
                break;

            default:
                this.normal = true;
                return 0;
        }
        return 1;
    }

//Makes an object disappear or reappear based on the enable argument
    private void ObjectDisappearance(bool enable) {
        if(enable) {
          Vector3 vec = new Vector3(this.Obj.transform.position.x, this.Obj.transform.position.y-100f, this.Obj.transform.position.z);
            this.Obj.transform.position = vec;
        }

        else {
            Vector3 vec = new Vector3(this.Obj.transform.position.x, this.Obj.transform.position.y+100f, this.Obj.transform.position.z);
            this.Obj.transform.position = vec;
        }
    }

    private void Light(bool enable) {
        if(enable) {
            this.Obj.GetComponent<Light>().intensity += 1.2f;
        }
        else {
            this.Obj.GetComponent<Light>().intensity -= 1.2f;
        }
    }

    private void ObjectChange(bool enable) {
        Transform replacement = this.Obj.transform.GetChild(0);
        Vector3 old = replacement.position;
        replacement.localPosition = new Vector3(0, enable ? 10f : -10f, 0);
        this.Obj.transform.position = old; 
    }

    private void Creature(bool enable) {
        this.Obj.SetActive(enable);
    }

    private void Audio(bool enable) {
        PlayAudio pa = this.Obj.GetComponent<PlayAudio>();
        pa.enabled = true;
        //footstep sounds
    }

    public static List<string> GetAllAnomalyTypes() {
        List<string> res = new List<string>
        {
            "Light",
            "ObjectDisappearance",
            "ObjectChange",
            "Creature",
            "Audio",
            "Movement"
        };
        return res;
    }


    public static string AnomalyTypeToString(ANOMALY_TYPE type) {
        switch(type) {
            case ANOMALY_TYPE.NONE:
                return "NONE";
            case ANOMALY_TYPE.Light:
                return "Light";
            case ANOMALY_TYPE.ObjectDisappearance:
                return "ObjectDisappearance";
            case ANOMALY_TYPE.ObjectChange:
                return "ObjectChange";
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