using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreKeeper {

	// About the current run
	public static int fishesCaught = 0;
	public static int fishesCaughtLevel = 0;
	public static bool isTimeLapse = false;
	public static bool isRunning = false;
	public static float timeRunning = 0;

	public delegate void OnUpdate ();
	public static event OnUpdate fishCaught;

	public static float lastTime = float.MaxValue;
	public static float bestTime = float.MaxValue;
	public static int lastFishesCaught = 0;

	public const int TOTALFISHES = 24;

	public static void BeforeNextLevel() {
		fishCaught = null;
		fishesCaughtLevel = 0;
	}

	public static void StartGame() {
		isRunning = true;
		fishesCaught = 0;
		timeRunning = Time.realtimeSinceStartup;
	}

	public static float GetCurrentTime() {
		return Time.realtimeSinceStartup-timeRunning;
	}

	public static void MakeTimeLapse() {
		isTimeLapse = true;
	}

	public static void CaughtFish() {
		fishesCaughtLevel++;
		fishesCaught++;
		if (fishCaught != null) {
			fishCaught ();
		}
	}

	public static void EndGame() {
		lastTime = GetCurrentTime();
		if (lastTime < bestTime) {
			bestTime = lastTime;
		}
		lastFishesCaught = fishesCaught;
		isRunning = false;
		isTimeLapse = false;
	}

}
