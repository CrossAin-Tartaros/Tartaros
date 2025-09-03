using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenFader : UIBase
{
    private Image colorScreen;
    [SerializeField] float fadeSpeed;

    // Start is called before the first frame update
    private void Awake()
    {
        colorScreen = GetComponent<Image>();
    }


    public void FadeIn()
    {
        StartCoroutine(FadeInProcess());  
    }

    IEnumerator FadeInProcess()
    {
        float startAlpha = 1.0f;
        float a = startAlpha;
        
        while (a > 0)
        {
            a -= (startAlpha * fadeSpeed) * Time.deltaTime;
            colorScreen.color = new Color(0, 0, 0, a);
            yield return null;
        }

        colorScreen.enabled = false;
    }

    public void FadeOut()
    {
        StartCoroutine(FadeOutProcss());
    }

    IEnumerator FadeOutProcss()
    {
        colorScreen.enabled = true;

        float startAlpha = 0f;
        float a = startAlpha;

        while (a < 1)
        {
            a += (1f * fadeSpeed) * Time.deltaTime;
            colorScreen.color = new Color(0, 0, 0, a);
            yield return null;
        }
    }
}
