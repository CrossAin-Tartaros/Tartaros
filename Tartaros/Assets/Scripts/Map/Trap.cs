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

            //�÷��̾� ������

        }

        //���Ͱ� ������ �ɸ��� ���(Destroy or ü�� ��� 0)
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
