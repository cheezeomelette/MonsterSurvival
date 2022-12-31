using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;
using System.Linq;

// 원거리 몬스터의 움직임, 공격을 담당
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(MonsterStatus))]
public class MonsterRangeController : MonsterController
{
    [SerializeField] Monster type;
    [SerializeField] MonsterStatus stat;
    [SerializeField] Transform attackPivot;
    [SerializeField] float bulletSpeed;
    [SerializeField] float attackRange;
    // 투사체 프리팹
    [SerializeField] GameObject bulletPrefab;

    public float power;
    public float attackRate;

    [HideInInspector]
    public Transform target;

    private Animator anim;
    private NavMeshAgent agent;
    private bool isInAttackRange;

    STATE state;
    bool isMove;
    float waitTime = 0f;
    float rotateSpeed = 500f;

    void Start()
    {
        // 컴포넌트 캐싱
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        // 타격 시 공격범위에서 벗어나지 않게 하기 위해
        agent.stoppingDistance = attackRange - 0.5f;
    }

    // Update is called once per frame
    void Update()
    {
        // 몬스터는 호스트가 관리
        if (!PhotonNetwork.IsMasterClient)
            return;
        // 죽었다면 활동정지
        if (stat.dead)
        {
            Debug.Log("Dead");
            agent.SetDestination(transform.position);
            return;
        }

        // 살아있는 플레이어가 없다면 정지
        if (PlayerStatus.playerTransformList.Count <= 0)
        {
            agent.SetDestination(transform.position);
            return;
        }
        // 현재 쫓고있는 타겟이 죽어있거나 없다면 새로운 타겟 탐색
        if (!PlayerStatus.playerTransformList.Contains(target) || target == null)
            target = PlayerStatus.playerTransformList.
                OrderBy(x => Vector3.Distance(x.position, transform.position)).First();

        // 타겟이 공격범위에 있는지
        isInAttackRange = (Vector3.Distance(transform.position, target.position) < attackRange);

        // 몬스터 상태별로 함수 실행
        switch (state)
        {
            case STATE.MOVE:
                Movement();
                break;
            case STATE.ATTACK:
                Attack();
                break;
            case STATE.WAIT:
                Wait();
                break;
        }
    }

    // 움직임
    protected override void Movement()
    {
        // 목적지를 타겟으로 설정
        agent.SetDestination(target.position);
        // 걷는 애니메이션 실행
        isMove = true;
        anim.SetBool("isMove", isMove);
        // 공격범위 내에 있으면 공격상태로 변경
        if (isInAttackRange)
            state = STATE.ATTACK;
    }

    // 공격상태 함수
    protected override void Attack()
    {
        // 움직임 애니메이션 멈춤
        isMove = false;
        anim.SetBool("isMove", isMove);
        // 공격시 바라보는 방향을 타겟방향으로 설정
        Vector3 dir = new Vector3(target.position.x - transform.position.x, 0, target.position.z - transform.position.z);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(dir), GameManager.gameDeltaTime * rotateSpeed);
        // 움직임 멈춤
        agent.SetDestination(transform.position);
        // 공격실행
        if (isInAttackRange)
            OnAttack();
        else
            state = STATE.MOVE;
    }

    // 공격 실행 함수
    protected override void OnAttack()
    {
        // 공격애니메이션 동기화
        photonView.RPC("AttackAnim", RpcTarget.All);

        // 투사체 생성
        GameObject bullet = PhotonNetwork.Instantiate(bulletPrefab.name, attackPivot.position, attackPivot.rotation);

        // 투사체 세팅
        MonsterBullet monsterBullet = bullet.GetComponent<MonsterBullet>();
        monsterBullet.Setup(bulletSpeed, power, (target.position - transform.position).normalized);
        
        // 공격후 대기상태로 공격속도 구현
        state = STATE.WAIT;
    }

    // 애니메이션 동기화
    [PunRPC]
    private void AttackAnim()
    {
        anim.SetTrigger("onAttack");
    }

    // 대기상태 함수
    protected override void Wait()
    {
        waitTime += GameManager.gameDeltaTime;

        // 이동 정지
        agent.SetDestination(transform.position);
        // 정지해 있는동안 타겟 방향으로 바라보게 설정
        Vector3 dir = new Vector3(target.position.x - transform.position.x, 0, target.position.z - transform.position.z);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(dir), GameManager.gameDeltaTime * rotateSpeed);
        // 공격속도만큼 대기 후 이동상태로 변경
        if (waitTime > attackRate)
        {
            waitTime = 0f;
            state = STATE.MOVE;
        }
    }

    // 생성 시 초기화 함수
    public override void Init(Vector3 position, Transform target)
    {
        // 태어나는 위치 설정
        transform.position = position;

        // 타겟 설정, hp회복, 움직이는 상태로 변경
        this.target = target;
        stat.hp = stat.maxHp;
        state = STATE.MOVE;
    }

    // 공격 범위 기즈모
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
