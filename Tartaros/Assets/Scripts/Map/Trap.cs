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
            //�÷��̾� teleportPosition ��ġ�� �̵�
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

        //���߿� �÷��̾� �ʿ� �ڽ��� ��ġ�� �ٲٴ� �Լ��� ��ġ�� �ű�� ���� �� ����
        collision.gameObject.GetComponent<Rigidbody2D>().MovePosition(teleportPosition);

        screenFader.FadeIn();
    }
}
