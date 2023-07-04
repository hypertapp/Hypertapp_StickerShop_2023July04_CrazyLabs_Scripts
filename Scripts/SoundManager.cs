using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
	public static int soundOff;
	public static float soundVolume;

	[Header ("Sprites -------------------------------")]
	public Sprite soundOnSprite;
	public Sprite soundOffSprite;

	[Header ("Buttons -------------------------------")]
	public Image soundBtnImg;

	[Header ("AudioClips ----------------------------")]
	public AudioSource finishLevel;
	public AudioSource complete;
	public AudioSource click;
	public AudioSource startGame;
	public AudioSource gameOver;
	public AudioSource scissors;
	public AudioSource[] peels;
	public float peelVolume;

	void Awake ()
	{	
		soundOff = PlayerPrefs.GetInt ("SoundOff");
		
		if (soundOff == 0)
			soundOff = -1;
		
		UpdateVolume ();
		UpdateSprites ();
	}

	public void Init ()
	{
	}

	public void UpdateVolume ()
	{	
		soundVolume = (1.0f - (float)soundOff) * 0.5f;

		// sound volume goes here
		finishLevel.volume = soundVolume;
		click.volume = soundVolume;
		startGame.volume = soundVolume * 0.468f;
		gameOver.volume = soundVolume;
        complete.volume = soundVolume;
		scissors.volume = soundVolume;
		for (int i = 0; i < peels.Length; i++)
			peels[i].volume = soundVolume * peelVolume;
	}
	
	public void ToggleSound ()
	{	
		soundOff *= -1;
		UpdateVolume ();
		Save ();
		UpdateSprites ();
	}
	
	public void Save ()
	{	
		PlayerPrefs.SetInt ("SoundOff", soundOff);
	}
	
	public void UpdateSprites ()
	{	
		if (soundOff == 1)
			soundBtnImg.sprite = soundOffSprite;
		else
			soundBtnImg.sprite = soundOnSprite;
	}
}
