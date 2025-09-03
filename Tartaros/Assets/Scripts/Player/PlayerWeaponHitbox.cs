using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponHitbox : MonoBehaviour
{
    private Player _player;
    private readonly HashSet<Transform> _hitOnce = new HashSet<Transform>();

    [Header("Mirror Mode")]
    [SerializeField] private bool mirrorByColliderOffset = true;
    [SerializeField] private Vector2 rightLocalOffset = Vector2.zero;

    private BoxCollider2D _col;
    private Vector2 _rightColliderOffset;

    private void Awake()
    {
        _player = GetComponentInParent<Player>();
        _col = GetComponent<BoxCollider2D>();
        if (_col)
        {
            _col.isTrigger = true;
            _rightColliderOffset = _col.offset;
        }

        if (!mirrorByColliderOffset && rightLocalOffset == Vector2.zero)
            rightLocalOffset = transform.localPosition;

        gameObject.SetActive(false); // 시작 시 OFF
    }

    private void OnEnable() => _hitOnce.Clear();

    private void LateUpdate()
    {
        if (_player == null) return;

        // 왼쪽 보고 있으면 true
        bool left =
            (_player.sprite && _player.sprite.flipX) || (_player.transform.lossyScale.x < 0f);

        if (mirrorByColliderOffset && _col)
        {
            //콜라이더 x만 좌/우 반전
            var o = _rightColliderOffset;
            _col.offset = new Vector2(left ? -o.x : o.x, o.y);
        }
        else
        {
            //Transform 위치로 미러링
            var o = rightLocalOffset;
            transform.localPosition = new Vector3(left ? -o.x : o.x, o.y, 0f);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Monster")) return;
        var root = other.attachedRigidbody ? other.attachedRigidbody.transform : other.transform;
        if (!_hitOnce.Add(root)) return; //한번만

        int damage = _player && _player.stat ? _player.stat.attack : 1;
        Debug.Log($"[HIT] {root.name} dmg={damage}");
        // 실제 데미지 주려면 아래 주석 해제
        // root.SendMessage("TakeDamage", damage, SendMessageOptions.DontRequireReceiver);
    }
}

