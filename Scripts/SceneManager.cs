using UnityEngine;
using System.Collections;
using UnityEngine.UI;
#if UNITY_IOS
using UnityEngine.iOS;
#endif
using UnityEngine.EventSystems;

public class SceneManager : MonoBehaviour
{
	public static string gameState;
	public static int session;
	public static bool iOSLaterThan103;
	public static int hapticOff;
    public static bool instructionShown;

	public static bool debug;

    public float showHideUITime;

	public Image hapticBtnImg;
	public Sprite hapticOffSprite;
	public Sprite hapticOnSprite;

	public float buttonAnimateScaleMult;
	public float buttonAnimateTime;

	public Text playBtnText;

    public CanvasGroup instructionCanvasGroup;
	public CanvasGroup btnsCanvanGroup;
	public CanvasGroup logoCanvasGroup;
	public CanvasGroup colorBtnCanvasGroup;
	public CanvasGroup btnStartColoringCanvasGroup;
	public CanvasGroup btnStopColoringCanvasGroup;
	public CanvasGroup btnStickCanvasGroup;
	public CanvasGroup orderCanvasGroup;
	public CanvasGroup selectArtworkTitleCanvasGroup;
	public CanvasGroup debugCanvasGroup;
	public CanvasGroup btnDebugCanvasGroup;

	public GameObject btnPlayObj;

    public TMPro.TextMeshProUGUI fpsText;
	public bool showFPS;
	float fpsTimer;
	int frameCount;

	void Awake ()
    {
		Application.targetFrameRate = 60;
		gameState = "Menu";
		session = 0;
		fpsText.text = "";
		hapticOff = PlayerPrefs.GetInt("HapticOff");
		if (hapticOff == 0)
			hapticBtnImg.sprite = hapticOnSprite;
		else
			hapticBtnImg.sprite = hapticOffSprite;

		fpsText.text = "";

		LoadScene();
	}

	void Update ()
    {
		if (showFPS)
			CalculateFPS ();
	}

	public void Init ()
    {
		session ++;
		PlayerPrefs.SetInt ("Session", session);
        btnPlayObj.SetActive(true);

        if (session == 1)
			HideUI(logoCanvasGroup);

		HideUI(btnsCanvanGroup);
		HideUI(colorBtnCanvasGroup);
		HideUI(btnDebugCanvasGroup);

		Managers.levelManager.Init();
		Managers.cameraManager.Init();
		Managers.stickerManager.Init();
		Managers.characterManager.Init();
		Managers.soundManager.Init();
		Managers.themeManager.Init();
	}

	public void Restart ()
    {
		Init ();
	}

	public void LoadScene ()
    {
        gameState = "Scene";
        StartNewLevel ();
	}

	public void LoadGameOver ()
    {
		if (gameState == "Scene") {

			gameState = "GameOver";
			StartCoroutine (LoadGameOver_ (0.298f));
				
			LevelManager.levelStarted = false;
        }
	}

	IEnumerator LoadGameOver_ (float delay)
    {
        yield return new WaitForSeconds(delay);

        if (!LevelManager.finished)
			Managers.soundManager.gameOver.Play ();

		if (!LevelManager.finished)
		{
			playBtnText.text = "TRY AGAIN";
			yield return new WaitForSeconds(0.268f);
			ShowUI(btnsCanvanGroup, 0.0f);
		}
		else
		{
			playBtnText.text = "NEXT LEVEL";
		}

		PlayerPrefs.Save();
		yield return null;
    }

    public void ShowUIImmediate(CanvasGroup canvas_)
    {
		canvas_.gameObject.SetActive(true);
		canvas_.alpha = 1.0f;
    }

    public void ShowUI(CanvasGroup canvas_, float delay)
    {
        StartCoroutine(ShowUI_(canvas_, delay));
    }
    public void HideUI(CanvasGroup canvas_)
    {
        StartCoroutine(HideUI_(canvas_));
    }
    IEnumerator ShowUI_(CanvasGroup canvas, float delay_)
    {
        canvas.alpha = 0.0f;
        yield return new WaitForSeconds(delay_);
        canvas.gameObject.SetActive(true);
        for (float t = 0.0f; t <= showHideUITime; t += Time.deltaTime)
        {
            canvas.alpha = Mathf.Lerp(0.0f, 1.0f, t / showHideUITime);
            yield return null;
        }
        canvas.alpha = 1.0f;
    }
    IEnumerator HideUI_(CanvasGroup canvas)
    {
        float startAlpha = canvas.alpha;
        for (float t = 0.0f; t <= showHideUITime; t += Time.deltaTime)
        {
            canvas.alpha = Mathf.Lerp(startAlpha, 0.0f, t / showHideUITime);
            yield return null;
        }
        canvas.alpha = 0.0f;
        canvas.gameObject.SetActive(false);
    }

	public void ToggleHaptic ()
    {
		hapticOff = 1 - hapticOff;
		PlayerPrefs.SetInt ("HapticOff", hapticOff);
		if (hapticOff == 0)
			hapticBtnImg.sprite = hapticOnSprite;
		else
			hapticBtnImg.sprite = hapticOffSprite;
	}

	public void AnimateButton ()
    {
		Managers.soundManager.click.Play ();
		StartCoroutine (AnimateButton_ (EventSystem.current.currentSelectedGameObject.transform));
	}

	IEnumerator AnimateButton_ (Transform btnTransform)
    {
		Vector3 originalScale = btnTransform.localScale;
		Vector3 maxScale = originalScale * buttonAnimateScaleMult;

		for (float t = 0.0f; t <= buttonAnimateTime * 0.268f; t += Time.deltaTime) {

			if (btnTransform != null)
				btnTransform.localScale = Vector3.Lerp (btnTransform.localScale, maxScale, t / (buttonAnimateTime * 0.268f));
			yield return null;
		}

		for (float t = 0.0f; t <= buttonAnimateTime * 0.732f; t += Time.deltaTime) {

			if (btnTransform != null)
				btnTransform.localScale = Vector3.Lerp (btnTransform.localScale, originalScale, t / (buttonAnimateTime * 0.732f));
			yield return null;
		}
	}

	void CalculateFPS ()
    {
		frameCount++;
		fpsTimer += Time.deltaTime;
		int fps;
		if (fpsTimer >= 0.368f) {

			fps = Mathf.RoundToInt (frameCount / fpsTimer);
			fpsText.text = fps.ToString ();
			fpsTimer = 0.0f;
			frameCount = 0;
		}
	}

	public void StartNewLevel()
	{
		LevelManager.finished = false;
		StopCoroutine(ShowNewLevel_());
		StartCoroutine(ShowNewLevel_());
	}
	IEnumerator ShowNewLevel_()
	{
		Managers.characterManager.blackScreenCanvasGroup.gameObject.SetActive(true);
		for (float t = 0.0f; t <= Managers.characterManager.blackScreenShowTime; t += Time.deltaTime)
		{
			Managers.characterManager.blackScreenCanvasGroup.alpha = Mathf.Lerp(0.0f, 1.0f, t / Managers.characterManager.blackScreenShowTime);
			yield return null;
		}

		Managers.characterManager.blackScreenCanvasGroup.alpha = 1.0f;
		CharacterManager.canHideBlackScreen = true;

		yield return new WaitForSeconds(0.128f);
		Managers.sceneManager.Init();

		yield return new WaitForSeconds(showHideUITime + 0.068f);

		for (float t = 0.0f; t <= Managers.characterManager.blackScreenHideTime; t += Time.deltaTime)
		{

			Managers.characterManager.blackScreenCanvasGroup.alpha = Mathf.Lerp(1.0f, 0.0f, t / Managers.characterManager.blackScreenHideTime);
			yield return null;
		}

		Managers.characterManager.blackScreenCanvasGroup.alpha = 0.0f;
		Managers.characterManager.blackScreenCanvasGroup.gameObject.SetActive(false);
	}

	public void ShowDebug()
    {
		debug = true;
		ShowUI(debugCanvasGroup, 0.0f);
		HideUI(btnDebugCanvasGroup);
    }
	public void HideDebug()
    {
		debug = false;
		HideUI(debugCanvasGroup);
		ShowUI(btnDebugCanvasGroup, 0.0f);
    }
}
