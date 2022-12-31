using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;
using System.Linq;

// ���Ÿ� ������ ������, ������ ���
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(MonsterStatus))]
public class MonsterRangeController : MonsterController
{
    [SerializeField] Monster type;
    [SerializeField] MonsterStatus stat;
    [SerializeField] Transform attackPivot;
    [SerializeField] float bulletSpeed;
    [SerializeField] float attackRange;
    // ����ü ������
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
        // ������Ʈ ĳ��
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        // Ÿ�� �� ���ݹ������� ����� �ʰ� �ϱ� ����
        agent.stoppingDistance = attackRange - 0.5f;
    }

    // Update is called once per frame
    void Update()
    {
        // ���ʹ� ȣ��Ʈ�� ����
        if (!PhotonNetwork.IsMasterClient)
            return;
        // �׾��ٸ� Ȱ������
        if (stat.dead)
        {
            Debug.Log("Dead");
            agent.SetDestination(transform.position);
            return;
        }

        // ����ִ� �÷��̾ ���ٸ� ����
        if (PlayerStatus.playerTransformList.Count <= 0)
        {
            agent.SetDestination(transform.position);
            return;
        }
        // ���� �Ѱ��ִ� Ÿ���� �׾��ְų� ���ٸ� ���ο� Ÿ�� Ž��
        if (!PlayerStatus.playerTransformList.Contains(target) || target == null)
            target = PlayerStatus.playerTransformList.
                OrderBy(x => Vector3.Distance(x.position, transform.position)).First();

        // Ÿ���� ���ݹ����� �ִ���
        isInAttackRange = (Vector3.Distance(transform.position, target.position) < attackRange);

        // ���� ���º��� �Լ� ����
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

    // ������
    protected override void Movement()
    {
        // �������� Ÿ������ ����
        agent.SetDestination(target.position);
        // �ȴ� �ִϸ��̼� ����
        isMove = true;
        anim.SetBool("isMove", isMove);
        // ���ݹ��� ���� ������ ���ݻ��·� ����
        if (isInAttackRange)
            state = STATE.ATTACK;
    }

    // ���ݻ��� �Լ�
    protected override void Attack()
    {
        // ������ �ִϸ��̼� ����
        isMove = false;
        anim.SetBool("isMove", isMove);
        // ���ݽ� �ٶ󺸴� ������ Ÿ�ٹ������� ����
        Vector3 dir = new Vector3(target.position.x - transform.position.x, 0, target.position.z - transform.position.z);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(dir), GameManager.gameDeltaTime * rotateSpeed);
        // ������ ����
        agent.SetDestination(transform.position);
        // ���ݽ���
        if (isInAttackRange)
            OnAttack();
        else
            state = STATE.MOVE;
    }

    // ���� ���� �Լ�
    protected override void OnAttack()
    {
        // ���ݾִϸ��̼� ����ȭ
        photonView.RPC("AttackAnim", RpcTarget.All);

        // ����ü ����
        GameObject bullet = PhotonNetwork.Instantiate(bulletPrefab.name, attackPivot.position, attackPivot.rotation);

        // ����ü ����
        MonsterBullet monsterBullet = bullet.GetComponent<MonsterBullet>();
        monsterBullet.Setup(bulletSpeed, power, (target.position - transform.position).normalized);
        
        // ������ �����·� ���ݼӵ� ����
        state = STATE.WAIT;
    }

    // �ִϸ��̼� ����ȭ
    [PunRPC]
    private void AttackAnim()
    {
        anim.SetTrigger("onAttack");
    }

    // ������ �Լ�
    protected override void Wait()
    {
        waitTime += GameManager.gameDeltaTime;

        // �̵� ����
        agent.SetDestination(transform.position);
        // ������ �ִµ��� Ÿ�� �������� �ٶ󺸰� ����
        Vector3 dir = new Vector3(target.position.x - transform.position.x, 0, target.position.z - transform.position.z);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(dir), GameManager.gameDeltaTime * rotateSpeed);
        // ���ݼӵ���ŭ ��� �� �̵����·� ����
        if (waitTime > attackRate)
        {
            waitTime = 0f;
            state = STATE.MOVE;
        }
    }

    // ���� �� �ʱ�ȭ �Լ�
    public override void Init(Vector3 position, Transform target)
    {
        // �¾�� ��ġ ����
        transform.position = position;

        // Ÿ�� ����, hpȸ��, �����̴� ���·� ����
        this.target = target;
        stat.hp = stat.maxHp;
        state = STATE.MOVE;
    }

    // ���� ���� �����
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
