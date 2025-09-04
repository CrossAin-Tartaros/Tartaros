using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    [SerializeField] LayerMask interactableLayer;
    [SerializeField] float interactionRange;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("EŰ ����");
            Collider2D col = Physics2D.OverlapCircle(transform.position, interactionRange, interactableLayer);

            if (col != null)
            {
                Debug.Log("��ȣ�ۿ� Ȯ��");
                IInteractable interactable = col.gameObject.GetComponent<IInteractable>();
                if (interactable.isInteractable)
                {
                    interactable.OnInteract();
                }
            }
        }
    }

}
