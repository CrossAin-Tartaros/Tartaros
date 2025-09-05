using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonKiosk : MonoBehaviour, IInteractable
{
    [SerializeField] public AudioClip OnKiosk;
    public bool isInteractable { get; set; } = true;

    public void OnInteract()
    {
        PlayerManager.Instance.Player.SceneChanging = true;
        SoundManager.Instance.PlayClip(OnKiosk, false);
        StartCoroutine(LateMove());
    }

    IEnumerator LateMove()
    {
        UIManager.Instance.GetUI<ScreenFader>().FadeOut();
        yield return new WaitForSeconds(1f);

        SceneLoadManager.Instance.LoadScene(SceneType.DungeonScene);
    }
}
