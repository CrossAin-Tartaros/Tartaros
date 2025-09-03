using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponHitbox : MonoBehaviour
{
    private Player _player;
    /*private readonly HashSet<Transform> _hitOnce = new HashSet<Transform>();
    private readonly HashSet<Monster> _parriedThisWindow = new HashSet<Monster>();*/

    [Header("Mirror Mode")]
    [SerializeField] private bool mirrorByColliderOffset = true;
    [SerializeField] private Vector2 rightLocalOffset = Vector2.zero;

    private BoxCollider2D _col;
    private Vector2 _rightColliderOffset;
    private int _monsterAttackLayer;


    private void Awake()
    {
        _player = GetComponentInParent<Player>();
        _col = GetComponent<BoxCollider2D>();
        if (_col) { _col.isTrigger = true; _rightColliderOffset = _col.offset; }

        _monsterAttackLayer = LayerMask.NameToLayer("MonsterAttack");
        if (_monsterAttackLayer < 0) Debug.LogWarning("MonsterAttack ��ã��");

        if (!mirrorByColliderOffset && rightLocalOffset == Vector2.zero)
            rightLocalOffset = transform.localPosition;

        // ���ӿ�����Ʈ ��ü�� �ѵΰ�, PlayerWeaponHitbox�� �θ��� ��ü�ʿ��� Collider�� ���� �״� ���� �̴ϴ�.
        /*gameObject.SetActive(false); // ���� �� OFF*/
    }



    private void OnEnable()
    {
        /*_hitOnce.Clear();
        _parriedThisWindow.Clear(); //�и� �ߺ� ����*/
    }

    private void LateUpdate()
    {
        if (_player == null) return;

        // ���� ���� ������ true
        bool left =
            (_player.sprite && _player.sprite.flipX) || (_player.transform.lossyScale.x < 0f);

        if (mirrorByColliderOffset && _col)
        {
            //�ݶ��̴� x�� ��/�� ����
            var o = _rightColliderOffset;
            _col.offset = new Vector2(left ? -o.x : o.x, o.y);
        }
        else
        {
            //Transform ��ġ�� �̷���
            var o = rightLocalOffset;
            transform.localPosition = new Vector3(left ? -o.x : o.x, o.y, 0f);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == _monsterAttackLayer)
        {
            //���� �ݶ��̴� �θ𿡼� ���� Ž��
            var monsterWeapon = other.GetComponentInParent<MonsterWeapon>();
            if (monsterWeapon != null)

                if (monsterWeapon.monster.IsStunned) return; //�̹� �и����¸� �н�
            if (_player != null && _player.IsParryWindow) return; //�߰� �и� ����
            
            // ������ TriggerStay �� �ƴϰ� TriggerEnter�� üũ�ϱ� ������ ���� �� �ߺ� �и� ���� �κ��� �����ϼŵ� ���ư��ϴ�!
            // if (!_parriedThisWindow.Add(monsterWeapon.monster)) return; //�ߺ� ���� �и� ����

            int parryDamage = (_player && _player.stat) ? _player.stat.attack : 1;
            monsterWeapon.Parry(parryDamage); //���� �и� ����
            _player?.BeginParryWindow(2f); //2�� ���� ���� ����

            Debug.Log($"[PARRY SUCCESS] {monsterWeapon.monster.name} dmg={parryDamage} (stun & expose), Player weak-spot window 2s");
            return;
        }

        // 2) �⺻ Ÿ��: ���� �ݶ��̴����� �θ�� �ö� Monster ã��
        var hitMonster = other.GetComponentInParent<Monster>();
        if (hitMonster == null) return; // ���Ͱ� �ƴϸ� ����

        var root = hitMonster.transform;           // �θ�(���� ��ü) ����
        /*if (!_hitOnce.Add(root)) return;           // ���� ����â 1ȸ��*/

        int hitDamage = (_player && _player.stat) ? _player.stat.attack : 1;

        // �и� ���̸� 1.5�� (���� Ÿ��)
        if (_player != null && _player.IsParryWindow)
            hitDamage = Mathf.RoundToInt(hitDamage * 1.5f);

        Debug.Log(_player != null && _player.IsParryWindow
            ? $"[WEAK SPOT HIT] {root.name} dmg={hitDamage} (1.5x)"
            : $"[HIT] {root.name} dmg={hitDamage}");

        hitMonster.Damaged(hitDamage);
    }
}

