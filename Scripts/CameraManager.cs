using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraManager : MonoBehaviour
{
	public static int shakeCount;
	public static float shakeAmount;
	public static float ratio;
	public static float grayScaleIntensity;

	public static bool debugCam;

	public float minShakeAmount;
	public float maxShakeAmount;
	public float minShakeDuration;
	public float maxShakeDuration;
	public int minShakeCount;
	public int maxShakeCount;
	public float shakeAmountMult;

	public float fovIphoneX;
	public float fovIphone6;
	public float fovIphone4;
	public float fovIpad;

    public Transform camPlayPosTransform;
	public Transform camFinishPosTransform;
	public Transform startCam;
	public Transform startCamDebug;
	public Transform drawCam;
	public Transform cutCam;
	public Transform peelCam;
	public Transform stickCam;
	public float transformTime;
	public float transformDelay;

	public Toggle debugCamToggle;

	void Awake ()
	{
		ratio = (float)Screen.height / (float)Screen.width;

        if (ratio < 1.368f)
            Camera.main.fieldOfView = fovIpad;
        else if (ratio < 1.68f)
            Camera.main.fieldOfView = fovIphone4;
        else if (ratio < 1.8f)
            Camera.main.fieldOfView = fovIphone6;
        else
            Camera.main.fieldOfView = fovIphoneX;

		debugCamToggle.isOn = false;
	}

	public void Init ()
	{
		StopAllCoroutines ();

		if (!debugCam)
			transform.SetPositionAndRotation(startCam.position, startCam.rotation);
		else
			transform.SetPositionAndRotation(startCamDebug.position, startCamDebug.rotation);
	}

    public void SwitchtToCam (Transform targetCam_)
    {
		StartCoroutine(SwitchtToCam_(targetCam_));
    }
    IEnumerator SwitchtToCam_(Transform targetCam)
    {
		CharacterManager.canTouch = false;

		yield return new WaitForSeconds(transformDelay);

        for (float t = 0.0f; t <= transformTime; t += Time.deltaTime)
        {
			transform.SetPositionAndRotation(Vector3.Lerp(transform.position, targetCam.position, t / transformTime),
				Quaternion.Lerp(transform.rotation, targetCam.rotation, t / transformTime));
			yield return null;
        }
		transform.SetPositionAndRotation(targetCam.position, targetCam.rotation);

		CharacterManager.canTouch = true;
	}

	public void SetDebugCam()
    {
		debugCam = debugCamToggle.isOn;
    }
}
