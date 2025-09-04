using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour
{
    [SerializeField] Vector2 teleportPosition;
    private ScreenFader screenFader;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            //플레이어 teleportPosition 위치로 이동
            StartCoroutine(TeleportPlayer(collision));

            PlayerManager.Instance.Player.ReceiveMonsterAttack(4, transform.position);

        }

        if (collision.gameObject.CompareTag("Monster"))
        {
            Destroy(collision.gameObject);
        }
    }

    IEnumerator TeleportPlayer(Collider2D collision)
    {
        if (screenFader == null)
            screenFader = UIManager.Instance.GetUI<ScreenFader>();

        screenFader.FadeOut();

        yield return new WaitForSeconds(1f);

        //나중에 플레이어 쪽에 자신의 위치를 바꾸는 함수로 위치를 옮기는 것이 더 좋음
        collision.gameObject.GetComponent<Rigidbody2D>().MovePosition(teleportPosition);

        screenFader.FadeIn();
    }
}
