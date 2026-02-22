using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class RoomGlitch : CustomDivergence
{
    private GameObject roomText;
    public float glitchFrequency = 0.25f;
    private String roomName;
    private GameObject roomTextOnDevice;
    private List<String> roomNames = new List<String>();

    void Awake() {
        roomText = GameObject.Find("RoomText");
        foreach(String roomName in GameObject.FindGameObjectsWithTag("Room").ToList().Select(r => r.name)) {
            roomNames.Add(roomName);
        }
    }

    public override void DoDivergenceAction(bool enable, DynamicObject obj) {
        roomName = obj.Room;
        List<GameObject> rooms = GameObject.FindGameObjectsWithTag("RoomUI").ToList();
        roomTextOnDevice = rooms.Where(r => r.name.Contains(roomName)).FirstOrDefault();
        if(enable) {
            StartCoroutine(GlitchRoomText());
        }
        else {
            StopAllCoroutines();
            roomText.GetComponent<TMP_Text>().text = PlayerUI.Instance.GetCurrentRoom();
            roomTextOnDevice.GetComponentInChildren<TMP_Text>().text = roomName;
        }
    }

    private IEnumerator GlitchRoomText() {
        //every glitchFrequency seconds, change the room text to a random room name
        float elapsedTime = glitchFrequency;
        while(true) {
            if(PlayerUI.paused || GameSystem.Instance.GameOver) {
                yield return null;
                continue;
            }

            elapsedTime += Time.deltaTime;
            if(elapsedTime >= glitchFrequency) {
                elapsedTime = 0f;

                roomText.GetComponent<TMP_Text>().text = roomNames[UnityEngine.Random.Range(0, roomNames.Count)];
                //also scramble the letters a bit
                char[] chars = roomText.GetComponent<TMP_Text>().text.ToCharArray();
                for(int i = 0; i < chars.Length; i++) {
                    if(UnityEngine.Random.Range(0, 8) == 0) {
                        chars[i] = (char)UnityEngine.Random.Range(65, 91);
                    }
                }
                roomTextOnDevice.GetComponentInChildren<TMP_Text>().text = new string(chars);
                if(PlayerUI.Instance.GetCurrentRoom() != roomName) {
                    roomText.GetComponent<TMP_Text>().text = PlayerUI.Instance.GetCurrentRoom();
                    yield return null;
                    continue;
                }
                roomText.GetComponent<TMP_Text>().text = new string(chars);
            }
            yield return null;
        }
    }
}
