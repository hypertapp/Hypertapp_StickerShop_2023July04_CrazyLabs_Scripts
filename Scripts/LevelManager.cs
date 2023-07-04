using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using MoreMountains.NiceVibrations;

public class LevelManager : MonoBehaviour
{
	public static int currentLevel;
	public static bool finished;
	public static int attempt;
	public static bool levelStarted;

    public float timeToLoadGameOver;

	void Awake ()
    {
		currentLevel = PlayerPrefs.GetInt ("CurrentLevel");
		attempt = PlayerPrefs.GetInt ("Attempt");

		if (currentLevel == 0)
        {
			currentLevel = 1;
			PlayerPrefs.SetInt ("CurrentLevel", 1);
		}

	}

	public void Init ()
    {
        finished = false;
        attempt++;
        PlayerPrefs.SetInt("Attempt", attempt);

        currentLevel = PlayerPrefs.GetInt ("CurrentLevel");
	}

	public void FinishLevel ()
    {
        if (!finished)
        {
            StartCoroutine(FinishLevelDelay());
        }
	}

	IEnumerator FinishLevelDelay ()
    {
		finished = true;
		attempt = 0;
		PlayerPrefs.SetInt ("Attempt", attempt);
        PlayerPrefs.SetInt("ReloadLevel", 0);

        CharacterManager.isAlive = false;
        CharacterManager.canTouch = false;

        currentLevel++;
        PlayerPrefs.SetInt("CurrentLevel", currentLevel);

        #if UNITY_IOS || UNITY_ANDROID
        if (SceneManager.hapticOff == 0)
            MMVibrationManager.Haptic(HapticTypes.HeavyImpact);
        #endif

        Managers.characterManager.finishParticle.Play();

        yield return new WaitForSeconds(0.198f);
        Managers.sceneManager.ShowUI(Managers.sceneManager.btnsCanvanGroup, 0.0f);
        Managers.sceneManager.ShowUI(Managers.sceneManager.btnDebugCanvasGroup, 0.0f);

        yield return new WaitForSeconds(0.198f);
        Managers.soundManager.finishLevel.Play ();
        #if UNITY_IOS || UNITY_ANDROID
        if (SceneManager.hapticOff == 0)
            MMVibrationManager.Haptic(HapticTypes.Success);
        #endif

        yield return new WaitForSeconds(timeToLoadGameOver);
        Managers.sceneManager.LoadGameOver ();
	}

}