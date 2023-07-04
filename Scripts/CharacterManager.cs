using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MoreMountains.NiceVibrations;

public class CharacterManager : MonoBehaviour
{
	public static bool isAlive;
	public static bool canTouch;
	public static bool firstTouch;
	public static bool canMove;

	public static bool canHideBlackScreen;

	public static bool haptic;
	public static bool holding;
	public static float lastPeelingHapticTime;

	public static List<Node> selectingNodes;

	public ParticleSystem finishParticle;

    public CanvasGroup blackScreenCanvasGroup;
	public float blackScreenShowTime;
	public float blackScreenHideTime;

	public Collider dragCollider;
	public Collider drawCollider;

	public Collider nodeSelectingCollider;
	public float nodeSelectingRadius;
	public float dragForceMult;
	public float pullForceRatio;

	public float itemRotatingMult;

	public float hapticInterval;

	Ray theRay;
	RaycastHit rayCastHit;
	RaycastHit[] hits;
	public static Vector3 lastPos;
	int nodeLayer;
	public static bool validCutStart;
	bool validDrawStart;

    void Awake ()
    {
		theRay = new Ray ();
		rayCastHit = new RaycastHit ();

        canHideBlackScreen = true;
		HideBlackScreen ();

		selectingNodes = new();
		nodeLayer = LayerMask.GetMask("Node");

        canTouch = true;
	}

	void Update ()
    {
        if (isAlive)
            CheckTouch();
	}

	public void Init ()
    {
		isAlive = true;
		canTouch = false;
		canMove = false;
		validCutStart = false;

		lastPeelingHapticTime = Time.time;

        finishParticle.Stop ();
		finishParticle.Clear ();

		selectingNodes.Clear();

        StartCoroutine (EnableTouch_ ());
	}

	public void SelectNodesAt(Vector3 pos)
    {
		selectingNodes.Clear();
		for (int i = 0; i < StickerManager.activeNodes.Count; i++)
        {
			if (Vector3.Scale(StickerManager.activeNodes[i].transform.position - pos, Vector3.right + Vector3.up).magnitude < nodeSelectingRadius)
				selectingNodes.Add(StickerManager.activeNodes[i]);
        }
		if (selectingNodes.Count == 0)
        {
			float dist = Vector3.Scale(StickerManager.activeNodes[0].transform.position - pos, Vector3.right + Vector3.up).magnitude;
			int index = 0;
			for (int i = 1; i < StickerManager.activeNodes.Count; i++)
            {
				float d = Vector3.Scale(StickerManager.activeNodes[i].transform.position - pos, Vector3.right + Vector3.up).magnitude;
				if (d < dist)
                {
					dist = d;
					index = i;
                }
            }
			selectingNodes.Add(StickerManager.activeNodes[index]);
        }
    }

    void CheckTouch()
    {
        if (canTouch && (Input.GetMouseButton(0) || Input.GetMouseButtonDown(0) || Input.GetMouseButtonUp(0)))
        {
            theRay = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Input.GetMouseButtonDown(0))
            {
				selectingNodes.Clear();

				if (firstTouch && !SceneManager.instructionShown)
                {
                    firstTouch = false;
                }

                if (SceneManager.instructionShown)
                {
                    SceneManager.instructionShown = false;
                    Managers.sceneManager.HideUI(Managers.sceneManager.instructionCanvasGroup);
                }

				if (StickerManager.currentState == "Coloring")
                {
					drawCollider.Raycast(theRay, out rayCastHit, 128.0f);
					validDrawStart = true;
					lastPos = rayCastHit.point;
					Managers.stickerManager.strokeStart.position = rayCastHit.point;
				}
				else if (StickerManager.currentState == "Peeling")
				{
					nodeSelectingCollider.Raycast(theRay, out rayCastHit, 128.0f);
					SelectNodesAt(rayCastHit.point);
					dragCollider.Raycast(theRay, out rayCastHit, 128.0f);
					lastPos = rayCastHit.point;
				}
				else if (StickerManager.currentState == "Cutting")
                {
					validCutStart = true;
					holding = true;
					Managers.soundManager.scissors.Play();
					StickerManager.currentScissorsSpeed = Managers.stickerManager.scissorsSpeed;
					Managers.stickerManager.scissorsAnim.SetTrigger("Cut");
				}
				else if (StickerManager.currentState == "SelectingArt")
                {
					if (Physics.Raycast(theRay, out rayCastHit, 128.0f, LayerMask.GetMask("ArtworkSelecting")))
                    {
                        if (int.TryParse(rayCastHit.collider.name, out int id))
                            Managers.stickerManager.SelectArtPiece(id);
                    }
                }
				else if (StickerManager.currentState == "PrepareToStick")
                {
					dragCollider.Raycast(theRay, out rayCastHit, 128.0f);
					lastPos = rayCastHit.point;
				}
            }
			else if (Input.GetMouseButton(0))
            {
				if (StickerManager.currentState == "Coloring")
                {
					drawCollider.Raycast(theRay, out rayCastHit, 128.0f);
					Managers.stickerManager.strokeStart.position = lastPos;
					Managers.stickerManager.strokeEnd.position = rayCastHit.point;
					Managers.stickerManager.strokeBody.position = lastPos;
					Managers.stickerManager.strokeBody.rotation = Quaternion.LookRotation(rayCastHit.point - lastPos, Vector3.up);
					Managers.stickerManager.strokeBody.localScale = StickerManager.strokeWidth * Vector3.right + (rayCastHit.point - lastPos).magnitude * Vector3.forward + Vector3.up;
					lastPos = rayCastHit.point;
				}
				else if (StickerManager.currentState == "Cutting")
                {
                    if (validCutStart)
                    {
                        Managers.stickerManager.strokeStart.position = lastPos;
						Managers.stickerManager.strokeEnd.position = Managers.stickerManager.scissorsAppearAnim.transform.position;
						Managers.stickerManager.strokeBody.position = lastPos;
						if ((Managers.stickerManager.strokeStart.position - Managers.stickerManager.strokeEnd.position).magnitude > 0.0f)
							Managers.stickerManager.strokeBody.rotation = Quaternion.LookRotation(Managers.stickerManager.strokeEnd.position - Managers.stickerManager.strokeStart.position, Vector3.up);
						Managers.stickerManager.strokeBody.localScale = StickerManager.strokeWidth * Vector3.right + (Managers.stickerManager.strokeEnd.position - Managers.stickerManager.strokeStart.position).magnitude * Vector3.forward + Vector3.up;
						lastPos = Managers.stickerManager.strokeEnd.position;
                    }
                }
				else if (StickerManager.currentState == "PrepareToStick")
                {
					dragCollider.Raycast(theRay, out rayCastHit, 128.0f);
					Managers.stickerManager.itemAnim.transform.Rotate((rayCastHit.point.x - lastPos.x) * itemRotatingMult * Vector3.up, Space.World);
					lastPos = rayCastHit.point;
				}
				else if (selectingNodes.Count > 0)
                {
					dragCollider.Raycast(theRay, out rayCastHit, 128.0f);
					for (int i = 0; i < selectingNodes.Count; i++)
					{
						selectingNodes[i].theRigidbody.velocity = (dragForceMult / Time.deltaTime) * Vector3.Scale(rayCastHit.point - lastPos, Vector3.right + Vector3.up);
						selectingNodes[i].theRigidbody.velocity += selectingNodes[i].theRigidbody.velocity.magnitude * pullForceRatio * Vector3.back;
					}
					lastPos = rayCastHit.point;
                }
            }
			else if (Input.GetMouseButtonUp(0))
            {
				holding = false;

				if (StickerManager.currentState == "Cutting")
				{
					validCutStart = false;
					Managers.soundManager.scissors.Stop();
					StickerManager.currentScissorsSpeed = 0.0f;
					Managers.stickerManager.scissorsAnim.SetTrigger("Idle");
				}

				if (StickerManager.currentState == "Coloring")
                {
					validDrawStart = false;
					Managers.stickerManager.HideStroke();
                }
				else if (selectingNodes.Count > 0)
				{
					for (int i = 0; i < selectingNodes.Count; i++)
					{
						selectingNodes[i].theRigidbody.velocity = Vector3.zero;
					}
					selectingNodes.Clear();
				}
			}
        }
    }

    public void EnableTouch ()
    {
        StartCoroutine(EnableTouch_());
    }
    IEnumerator EnableTouch_ ()
    {
		yield return new WaitForSeconds (0.468f);

		canTouch = true;
		firstTouch = true;
	}

	public void ShowBlackScreen ()
    {
		canHideBlackScreen = false;

		StopCoroutine ("ShowBlackScreen_");
		StopCoroutine ("HideBlackScreen_");

		StartCoroutine (ShowBlackScreen_ ());
	}

	public void HideBlackScreen ()
    {
		StopCoroutine ("ShowBlackScreen_");
		StopCoroutine ("HideBlackScreen_");

		StartCoroutine (HideBlackScreen_ ());
	}

	IEnumerator ShowBlackScreen_ ()
    {
		blackScreenCanvasGroup.gameObject.SetActive (true);
		for (float t = 0.0f; t <= blackScreenShowTime; t += Time.deltaTime)
        {
			blackScreenCanvasGroup.alpha = Mathf.Lerp (0.0f, 1.0f, t / blackScreenShowTime);
			yield return null;
		}

		blackScreenCanvasGroup.alpha = 1.0f;
		canHideBlackScreen = true;
	}

	IEnumerator HideBlackScreen_ ()
    {
		while (!canHideBlackScreen)
        {
			yield return null;
		}

		for (float t = 0.0f; t <= blackScreenHideTime; t += Time.deltaTime)
        {
			blackScreenCanvasGroup.alpha = Mathf.Lerp (1.0f, 0.0f, t / blackScreenHideTime);
			yield return null;
		}

		blackScreenCanvasGroup.alpha = 0.0f;
		blackScreenCanvasGroup.gameObject.SetActive (false);
	}

	public void Die ()
    {
		isAlive = false;
        canTouch = false;

		#if UNITY_IOS || UNITY_ANDROID
		if (SceneManager.hapticOff == 0)
            MMVibrationManager.Haptic(HapticTypes.Failure);
        #endif

        Managers.sceneManager.LoadGameOver ();
	}

	public void StartHaptic()
    {
		haptic = true;
		StartCoroutine(StartHaptic_());
    }
	public void StopHaptic()
    {
		StopCoroutine(StartHaptic_());
		haptic = false;
    }
	IEnumerator StartHaptic_()
    {
		while (haptic)
        {
			if (holding)
			{
#if UNITY_IOS || UNITY_ANDROID
				if (SceneManager.hapticOff == 0)
					MMVibrationManager.Haptic(HapticTypes.LightImpact);
#endif
			}
			yield return new WaitForSeconds(hapticInterval);
		}
	}
}