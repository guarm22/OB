using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PunctureCollapse : MonoBehaviour {
    public static PunctureCollapse Instance;
    [HideInInspector]
    public List<GameObject> rooms;
    public GameObject spherePrefab;
    public bool isCollapsing = false;

    void Start() {
        Instance = this;
    }

    public IEnumerator Collapse() {
        if(!DivergenceControl.Instance.ActivateEndgameCollapse) {
            yield break;
        }

        if(isCollapsing) {
            yield break;
        }

        PlayerUI.Instance.ScrambleReportUI(true);
        
        rooms = DivergenceControl.Instance.RoomObjects;
        isCollapsing = true;

        foreach (GameObject room in rooms) {
            if(PlayerUI.paused) {
                yield return new WaitUntil(() => !PlayerUI.paused);
            }

            GameObject sphere = Instantiate(spherePrefab, room.transform.position, Quaternion.identity);
            sphere.transform.position = new Vector3(sphere.transform.position.x, sphere.transform.position.y + 1f, sphere.transform.position.z);
            sphere.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            sphere.transform.SetParent(room.transform);

            //change expansion speed based map
            float expansionSpeed = 1f;
            if(SceneManager.GetActiveScene().name == "Graveyard") {
                expansionSpeed = 2.5f;
            }

            sphere.GetComponent<ExpandingSphere>().ManualActivation(expansionSpeed);

            float delay = Random.Range(.8f, 1.1f);
            yield return new WaitForSeconds(delay);
        }
    }
}
