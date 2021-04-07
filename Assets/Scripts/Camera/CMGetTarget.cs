using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CMGetTarget : MonoBehaviour
{
    PlayerCharacter player;

    private void Start()
    {
        player = FindObjectOfType<PlayerCharacter>();
        var vcam = GetComponent<CinemachineVirtualCamera>();
        vcam.Follow = player.transform.GetChild(0);
        vcam.LookAt = player.transform.GetChild(0);
    }
}
