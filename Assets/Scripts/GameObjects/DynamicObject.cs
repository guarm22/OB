using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

public enum ANOMALY_TYPE {
    NONE,
    Light,
    ObjectDisappearance,
    ObjectMovement,
    ObjectChange,
    Creature,
};

public class DynamicObject {
    public DynamicData data;
    public string Room {get; set;}
    public string Name {get; set;}

    private float minMovement = 3.5f;
    private float maxMovement = 7.5f;

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

        if(this.data.customDivergence != null) {
            this.data.customDivergence.DoDivergenceAction(enable, this);
            return 1;
        }

        switch(this.data.type) {
            case ANOMALY_TYPE.ObjectDisappearance:
                this.ObjectDisappearance(enable);
                break;

            case ANOMALY_TYPE.Light:
                this.Light(enable);
                break;

            case ANOMALY_TYPE.ObjectMovement: 
                this.ObjectMovement(enable);
                break;
            
            case ANOMALY_TYPE.ObjectChange:
                this.ObjectChange(enable);
                break;

            case ANOMALY_TYPE.Creature:
                this.Creature(enable);
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

    private void ObjectMovement(bool enable) {
        if(enable) {
            float negativeX = Random.Range(0f, 1f);
            float negativeZ = Random.Range(0f, 1f);
            float randomX = (negativeX > 0.5f) ? 1f : -1f * Random.Range(minMovement, maxMovement);
            float randomZ = (negativeZ > 0.5f) ? 1f : -1f * Random.Range(minMovement, maxMovement);
            this.Obj.transform.DOMove(new Vector3(
                                                    randomX + this.Obj.transform.position.x, 
                                                    this.Obj.transform.position.y, 
                                                    randomZ + this.Obj.transform.position.z
                                                ), 8.0f);   
        }
        else {
            this.Obj.transform.DOMove(this.originalPosition, 2.0f);
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

    public static List<string> GetAllAnomalyTypes() {
        List<string> res = new List<string>();
        res.Add("Light");
        res.Add("ObjectDisappearance");
        res.Add("ObjectMovement");
        res.Add("ObjectChange");
        res.Add("Creature");
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
            case ANOMALY_TYPE.ObjectMovement:
                return "ObjectMovement";
            case ANOMALY_TYPE.ObjectChange:
                return "ObjectChange";
            case ANOMALY_TYPE.Creature:
                return "Creature";
            default:
                return "Error";
        }
    }

}