using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasRoot : MonoBehaviour
{
    // Start is called before the first frame update
    private void Awake()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.canvasTransform = this.transform;
        }
    }
}
