using System.Collections;
using System.Collections.Generic;
using SojaExiles;
using UnityEngine;

public class PunctureRealm : MonoBehaviour {

    public GameObject punctureSpawnPoint;

    private Vector3 returnPoint;

    private GameObject correctDoor;
    private GameObject fakeDoor;

    public List<GameObject> doors = new List<GameObject>();

    public static PunctureRealm Instance;

    private bool returning = false;

    public GameObject expandingSpherePrefab;

    private GameObject triggeringSphere;

    private List<GameObject> spawnedSpheres = new List<GameObject>();

    void Awake() {
        Instance = this;
        correctDoor = doors[Random.Range(0, doors.Count)];
    }

    void Update() {
        if(Input.GetKeyDown(KeyCode.J) && GameSystem.InEditor()) {
            returnPoint = SC_FPSController.Instance.transform.position;
            SC_FPSController.Instance.TeleportToPosition(punctureSpawnPoint.transform.position);
        }
        if(!returning && correctDoor.GetComponent<opencloseDoor>().open) {
            StartCoroutine(LeavePunctureRealm());
        }
    }

    public void InitRealm(GameObject realmSphere) {
        triggeringSphere = realmSphere;
        correctDoor = doors[Random.Range(0, doors.Count)];
        fakeDoor = doors.Find(door => door != correctDoor);

        //get child of fake door
        fakeDoor.transform.GetChild(0).gameObject.SetActive(false);
        fakeDoor.GetComponent<opencloseDoor>().locked = true;

        returnPoint = SC_FPSController.Instance.transform.position;
        SC_FPSController.Instance.TeleportToPosition(punctureSpawnPoint.transform.position);
        StartCoroutine(TrapDoors());
    }

    private IEnumerator TrapDoors() {
        //spawn a sphere at each wrong door that expands and then disappears
        foreach(GameObject door in doors)
        {
            if(door != correctDoor && door != fakeDoor) {
                GameObject expandingSphere = Instantiate(expandingSpherePrefab, door.transform.position, Quaternion.identity);
                spawnedSpheres.Add(expandingSphere);
                expandingSphere.GetComponent<ExpandingSphere>().ManualActivation(1.8f);
                yield return new WaitForSeconds(0.2f);
            }
        }
    }

    private IEnumerator LeavePunctureRealm() {
        returning = true;
        triggeringSphere.GetComponent<RealmSphere>().Return();
        SC_FPSController.Instance.TeleportToPosition(returnPoint);
        spawnedSpheres.ForEach(sphere => Destroy(sphere));
        yield return new WaitForSeconds(2f);
        foreach(GameObject door in doors) {
            door.GetComponent<opencloseDoor>().StartCoroutine(door.GetComponent<opencloseDoor>().closing());
        }
        fakeDoor.transform.GetChild(0).gameObject.SetActive(true);
        fakeDoor.GetComponent<opencloseDoor>().locked = false;
        triggeringSphere = null;
        returning = false;
    }
}
