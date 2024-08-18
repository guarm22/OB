using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SojaExiles {
	public class opencloseDoor : MonoBehaviour {

		public Animator openandclose;
		public bool open;
		public Transform Player;
		public bool locked = false;

		public AudioClip closeSound;
		public AudioClip openSound;
		public AudioClip lockedSound;

		public AudioSource audioSource;

		private bool inAnim = false;

		public String doorType = "none";

		void Start() {
			open = false;
			Player = GameObject.Find("Player").transform;
			if(audioSource == null) {
				audioSource = gameObject.AddComponent<AudioSource>();
				audioSource.volume = PlayerPrefs.GetInt("SFXVolume", 50)/100f;
				audioSource.spatialBlend = 1f;
			}

			if(doorType == "none") {
				return;
			}

			if(!closeSound){
				closeSound = Resources.Load<AudioClip>("Sounds/door_close");
			}
			if(!openSound){
				openSound = Resources.Load<AudioClip>("Sounds/door_open");
			}
			if(!lockedSound){
				lockedSound = Resources.Load<AudioClip>("Sounds/door_locked");
			}
		}

		public void ChangeLockState(bool state) {
			locked = state;
		}

		void OnMouseOver() {
			if(Cursor.lockState == CursorLockMode.Confined) {
				return;
        	}

			if (Player) {
				float dist = Vector3.Distance(Player.position, transform.position);

				if (dist < 4  && !inAnim) {

					if(locked) {
						if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.E)) {
							DoorSound(lockedSound, 0.0f);
						}
						return;
					}

					if (open == false) {

						if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.E)){
							StartCoroutine(opening());
							DoorSound(openSound, 0.0f);
						}
					}
					else {

						if (open == true) {
							if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.E)) {
								StartCoroutine(closing());
								DoorSound(closeSound, 0.25f);
							}
						}
					}
				}
			}
		}

		private void DoorSound(AudioClip sound, float delay) {
			if(sound == null) {
				return;
			}

			StartCoroutine(PlayDoorSound(sound, 0.0f));
		}

		IEnumerator PlayDoorSound(AudioClip sound, float delay) {
			yield return new WaitForSeconds(delay);
			audioSource.PlayOneShot(sound);
		}

		IEnumerator opening() {
			inAnim = true;
			openandclose.Play("Opening");
			open = true;
			yield return new WaitForSeconds(.5f);

			inAnim = false;
		}

		IEnumerator closing() {
			inAnim = true;
			openandclose.Play("Closing");
			open = false;
			yield return new WaitForSeconds(.5f);
			inAnim = false;
		}
	}
}