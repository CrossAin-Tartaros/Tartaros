using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirtualCamera : MonoBehaviour
{
    private CinemachineVirtualCamera virtualCamera;
    
    private void Awake()
    {
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
    }

    private void Start()
    {
        StartCoroutine(LateLoad());
    }

    IEnumerator LateLoad()
    {
        yield return new WaitForSeconds(1f);
        virtualCamera.Follow = PlayerManager.Instance.CurrentPlayerInstance.transform;
    }
}
