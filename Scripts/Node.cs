using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public Rigidbody theRigidbody;
    public List<SpringJoint> springJoints;
    public FixedJoint fixedJoint;

    private void OnCollisionEnter(Collision collision)
    {
        if (StickerManager.currentState == "Sticking")
        {
            theRigidbody.isKinematic = true;
        }
    }

    void OnJointBreak(float breakForce)
    {
        StickerManager.brokenNodeCount++;
        Managers.soundManager.peels[Random.Range(0, 100) % Managers.soundManager.peels.Length].Play();
        if (Time.time - CharacterManager.lastPeelingHapticTime >= Managers.characterManager.hapticInterval)
        {
            CharacterManager.lastPeelingHapticTime = Time.time;
#if UNITY_IOS || UNITY_ANDROID
            if (SceneManager.hapticOff == 0)
                MoreMountains.NiceVibrations.MMVibrationManager.Haptic(MoreMountains.NiceVibrations.HapticTypes.LightImpact);
#endif
        }
    }

    private void Awake()
    {
        springJoints = new List<SpringJoint>();
    }

    public void Reset()
    {
        theRigidbody.isKinematic = true;
        transform.position = Vector3.left * 68.0f;
        if (!StickerManager.availableNodes.Contains(this))
            StickerManager.availableNodes.Add(this);
        if (StickerManager.activeNodes.Contains(this))
            StickerManager.activeNodes.Remove(this);
        if (springJoints.Count > 0)
        {
            for (int i = 0; i < springJoints.Count; i++)
                Destroy(springJoints[i]);
            springJoints.Clear();
        }
        if (fixedJoint)
            Destroy(fixedJoint);
    }

    public void AddSpringJoint(Rigidbody _rigidbody)
    {
        SpringJoint sj = gameObject.AddComponent<SpringJoint>();
        sj.connectedBody = _rigidbody;
        sj.anchor = transform.InverseTransformPoint(_rigidbody.transform.position);
        if (!StickerManager.jointSpringDebug)
        {
            sj.spring = Managers.stickerManager.jointSpring;
            sj.damper = Managers.stickerManager.jointDamper;
        }
        else
        {
            sj.spring = Managers.stickerManager.jointSpring_Debug;
            sj.damper = Managers.stickerManager.jointDamper_Debug;
        }
        sj.enableCollision = true;
        sj.enablePreprocessing = false;
        springJoints.Add(sj);
    }

    public void AddFixedJoint()
    {
        FixedJoint fj = gameObject.AddComponent<FixedJoint>();
        fj.breakForce = Managers.stickerManager.breakForce;
        fixedJoint = fj;
    }
}
