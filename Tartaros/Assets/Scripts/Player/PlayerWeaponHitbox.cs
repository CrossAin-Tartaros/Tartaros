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
    [SerializeField] private GameObject parryingMeleeAnimation;
    [SerializeField] private GameObject parryingRangeAnimation;
    [SerializeField] private float parryingAnimationOffset = 1f;

    private BoxCollider2D _col;
    private Vector2 _rightColliderOffset;
    private int _monsterAttackLayer;

    private bool isLeft;


    private void Awake()
    {
        _player = GetComponentInParent<Player>();
        _col = GetComponent<BoxCollider2D>();
        if (_col) { _col.isTrigger = true; _rightColliderOffset = _col.offset; }

        _monsterAttackLayer = LayerMask.NameToLayer("MonsterAttack");
        if (_monsterAttackLayer < 0) Debug.LogWarning("MonsterAttack 못찾음");

        if (!mirrorByColliderOffset && rightLocalOffset == Vector2.zero)
            rightLocalOffset = transform.localPosition;

        gameObject.SetActive(true);
        // 게임오브젝트 자체는 켜두고, PlayerWeaponHitbox를 부르는 객체쪽에서 Collider만 껐다 켰다 해줄 겁니다.
        /*gameObject.SetActive(false); // 시작 시 OFF*/
    }



    private void OnEnable()
    {
        /*_hitOnce.Clear();
        _parriedThisWindow.Clear(); //패링 중복 방지*/
    }

    private void LateUpdate()
    {
        if (_player == null) return;

        // 왼쪽 보고 있으면 true
        isLeft =
            (_player.sprite && _player.sprite.flipX) || (_player.transform.lossyScale.x < 0f);

        if (mirrorByColliderOffset && _col)
        {
            //콜라이더 x만 좌/우 반전
            var o = _rightColliderOffset;
            _col.offset = new Vector2(isLeft ? -o.x : o.x, o.y);
        }
        else
        {
            //Transform 위치로 미러링
            var o = rightLocalOffset;
            transform.localPosition = new Vector3(isLeft ? -o.x : o.x, o.y, 0f);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == _monsterAttackLayer)
        {
            //무기 콜라이더 부모에서 몬스터 탐색
            var monsterWeapon = other.GetComponentInParent<MonsterWeapon>();
            if (monsterWeapon != null)

                if (monsterWeapon.monster.IsStunned) return; //이미 패링상태면 패스
            if (_player != null && _player.IsParryWindow) return; //추가 패링 무시
            
            // 어차피 TriggerStay 가 아니고 TriggerEnter로 체크하기 때문에 이제 이 중복 패링 금지 부분은 제거하셔도 돌아갑니다!
            // if (!_parriedThisWindow.Add(monsterWeapon.monster)) return; //중복 몬스터 패링 금지

            int parryDamage = (_player && _player.stat) ? _player.stat.attack : 1;
            monsterWeapon.Parry(parryDamage); //몬스터 패링 상태
            _player?.BeginParryWindow(2f); //2초 약점 공격 가능

            //여기 추가: 패링 성공 시 플레이어 무적 0.2초 부여
            if (_player != null)
            {
                StartCoroutine(_player.IFramesCustom(0.2f));
            }

            GameObject go;
            if (other.gameObject.TryGetComponent(out MeleeMonsterWeapon melee))
            {
                go = Instantiate(parryingMeleeAnimation, _player.GetAimPoint(0.8f),  Quaternion.identity);
            }
            else
            {
                go = Instantiate(parryingRangeAnimation, _player.GetAimPoint(0.8f),  Quaternion.identity);
            }
            
            go.transform.position +=  (Vector3)((isLeft ? Vector2.left : Vector2.right) * parryingAnimationOffset);
            var psRenderer = go.GetComponent<ParticleSystemRenderer>(); 
            psRenderer.flip = isLeft ? new Vector3(1f, 0f, 0f) : Vector3.zero;
            psRenderer.sortingOrder = 200;
            go.SetActive(true);

            Debug.Log($"[PARRY SUCCESS] {monsterWeapon.monster.name} dmg={parryDamage} (stun & expose), Player weak-spot window 2s");
            return;
        }

        // 2) 기본 타격: 맞은 콜라이더에서 부모로 올라가 Monster 찾기
        var hitMonster = other.GetComponentInParent<Monster>();
        if (hitMonster == null) return; // 몬스터가 아니면 무시

        var root = hitMonster.transform;           // 부모(몬스터 본체) 기준
        /*if (!_hitOnce.Add(root)) return;           // 같은 공격창 1회만*/

        int hitDamage = (_player && _player.stat) ? _player.stat.attack : 1;

        // 패링 중이면 1.5배 (약점 타격)
        if (_player != null && _player.IsParryWindow)
            hitDamage = Mathf.RoundToInt(hitDamage * 1.5f);
        Debug.Log($"플레이어가 몬스터에게 준 데미지: {hitDamage} (atk={_player.stat.attack})");


        Debug.Log(_player != null && _player.IsParryWindow
            ? $"[WEAK SPOT HIT] {root.name} dmg={hitDamage} (1.5x)"
            : $"[HIT] {root.name} dmg={hitDamage}");

        hitMonster.Damaged(hitDamage);
    }
}

