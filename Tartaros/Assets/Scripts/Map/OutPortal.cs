using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutPortal : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        StartCoroutine(LateMove());
    }

    IEnumerator LateMove()
    {
        UIManager.Instance.GetUI<ScreenFader>().FadeOut();
        yield return new WaitForSeconds(1f);

        SceneLoadManager.Instance.LoadScene(SceneType.MainScene);
    }
}
