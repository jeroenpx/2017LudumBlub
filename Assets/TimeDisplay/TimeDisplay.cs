using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TimeDisplay : MonoBehaviour {

	private UnityEngine.UI.Text text;

	private bool showFinal = false;

	// Use this for initialization
	void Start () {
		if (SceneManager.GetActiveScene ().name.Equals ("Level01") || SceneManager.GetActiveScene ().name.Equals ("Level01_b")) {
			showFinal = true;
		}
		if (!(ScoreKeeper.isRunning || (showFinal && (ScoreKeeper.bestTime!=float.MaxValue || ScoreKeeper.lastFishesCaught>0)))) {
			Destroy (gameObject);
		}

		text = GetComponent<UnityEngine.UI.Text> ();
	}

	string GetTimeStr(float time) {
		int seconds = Mathf.FloorToInt (time);
		int millis = Mathf.FloorToInt ((time - seconds) * 1000);
		int minutes = seconds / 60;
		seconds = seconds % 60;
		return minutes.ToString ().PadLeft (2, '0')+":"+seconds.ToString ().PadLeft (2, '0')+"."+millis.ToString().PadLeft(3, '0');
	}
	
	// Update is called once per frame
	void Update () {
		if (showFinal) {
			text.text = "Last: " + GetTimeStr (ScoreKeeper.lastTime) + " with "+ScoreKeeper.lastFishesCaught+" of "+ScoreKeeper.TOTALFISHES+" fishes / Best: " + GetTimeStr (ScoreKeeper.bestTime);
		} else {
			if (ScoreKeeper.isTimeLapse) {
				text.text = GetTimeStr (ScoreKeeper.GetCurrentTime ());
			} else {
				// DO NOT SHOW THIS...
				//text.text = "Fishes: " + ScoreKeeper.fishesCaught + "/" + ScoreKeeper.TOTALFISHES;
				text.text = "";
			}
		}
	}
}
