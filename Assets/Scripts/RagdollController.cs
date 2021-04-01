using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollController : MonoBehaviour
{
    private List<Rigidbody> ragRb = new List<Rigidbody>();
    private List<Quaternion> boneRotations = new List<Quaternion>();

    [SerializeField]
    float fixUpSpeed = 0.3f, boneRotSpeed = 3f;

    [SerializeField]
    public Transform hips;
    Vector3 hipPos;
    float hipY;

    public bool activateRagdoll;

    private void Awake()
    {
        foreach (Rigidbody rb in GetComponentsInChildren<Rigidbody>())
        {
            ragRb.Add(rb);
            boneRotations.Add(rb.transform.localRotation);
        }
        ragRb.Remove(GetComponent<Rigidbody>());
        boneRotations.Remove(transform.localRotation);
        hipY = hips.localPosition.y;
    }

    private void GetCurrent()
    {
        hipPos = new Vector3(hips.localPosition.x, hipY, hips.localPosition.z);
        for (int i = 0; i < ragRb.Count; i++)
        {
            boneRotations[i] = ragRb[i].transform.localRotation;
        }
    }

    public void ActivateRagdoll()
    {
        activateRagdoll = true;
        GetCurrent();
        SetKinematic(false);
    }

    public IEnumerator DeactivateRagdoll()
    {
        bool recovering = true;
        activateRagdoll = false;
        SetKinematic(true);

        while (recovering)
        {
            for (int i = 0; i < boneRotations.Count; i++)
            {
                ragRb[i].transform.localRotation = Quaternion.Lerp(ragRb[i].transform.localRotation, boneRotations[i], Time.deltaTime * boneRotSpeed);
            }
            hips.localPosition = Vector3.MoveTowards(hips.localPosition, hipPos, Time.deltaTime * fixUpSpeed);
            yield return null;
        }
        recovering = false;
    }

    public void SetKinematic(bool isKinematic)
    {
        foreach (Rigidbody rb in ragRb)
        {
            rb.isKinematic = isKinematic;
        }
    }

}
