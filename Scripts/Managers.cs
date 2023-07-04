using UnityEngine;
using System.Collections;

public class Managers : MonoBehaviour
{
	public static SceneManager sceneManager;
	public static SoundManager soundManager;
	public static CharacterManager characterManager;
	public static CameraManager cameraManager;
	public static LevelManager levelManager;
	public static ThemeManager themeManager;
	public static StickerManager stickerManager;

	void Awake ()
    {
		sceneManager = GameObject.Find ("sceneManager").GetComponent <SceneManager>();
		soundManager = GameObject.Find ("soundManager").GetComponent <SoundManager>();
		characterManager = GameObject.Find ("characterManager").GetComponent <CharacterManager>();
		cameraManager = GameObject.Find ("Main Camera").GetComponent <CameraManager>();
		levelManager = GameObject.Find ("levelManager").GetComponent <LevelManager>();
		themeManager = GameObject.Find ("themeManager").GetComponent <ThemeManager>();
		stickerManager = GameObject.Find("stickerManager").GetComponent<StickerManager>();
	}
}
