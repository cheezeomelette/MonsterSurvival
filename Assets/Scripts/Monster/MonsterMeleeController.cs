using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;
using System.Linq;

// ���� ������ ������, ������ ���
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(MonsterStatus))]
public class MonsterMeleeController : MonsterController
{
    [SerializeField] Monster type;
    [SerializeField] MonsterStatus stat;
    // ���� ������ Transform
    [SerializeField] Transform attackPivot;
    [SerializeField] float attackRange;
    [SerializeField] float attackRadius;
    public float power;
    public float attackRate;

    [HideInInspector]
    public Transform target;

    private Animator anim;
    private NavMeshAgent agent;
    // ���ݹ����� �÷��̾ �ִ���
    private bool isInAttackRange;
    // �÷��̾ �����ϱ� ���� ���̾�
    private int playerLayerMask;

    STATE state;
    bool isMove;
    float waitTime = 0f;
    float rotateSpeed = 1000f;

    void Start()
    {
        // ������Ʈ ĳ��
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        // Ÿ�� �� ���ݹ������� ����� �ʰ� �ϱ� ����
        agent.stoppingDistance = attackRange - 0.5f;
        // layerMask ����
        playerLayerMask = 1 << LayerMask.NameToLayer("Player");
    }

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

        // �ֺ��� �÷��̾ �ִ��� Ž��
        Collider[] colliders = Physics.OverlapSphere(transform.position, attackRange, playerLayerMask);

        // ���ݹ��� ���� �÷��̾ �ִٸ� Ÿ�� �缳��
        isInAttackRange = colliders.Length > 0;
        if (isInAttackRange)
            target = colliders.
                OrderBy(x => Vector3.Distance(x.transform.position, transform.position)).First().transform;

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

        // ���ݹ��� ���� �÷��̾� �˻�
        Collider[] colliders = Physics.OverlapSphere(attackPivot.position, attackRadius, playerLayerMask);
        foreach (Collider collider in colliders)
        {
            IDamageable hit = collider.GetComponent<IDamageable>();

            // �÷��̾��� OnDamage�Լ� ����
            if (hit != null)
            {
                collider.GetComponent<PhotonView>().RPC("OnDamage", RpcTarget.All, power);
            }
        }
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

    // ���� ������ �������� �����
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(attackPivot.position, attackRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
