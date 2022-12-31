using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

// �÷��̾��� ��������
public class PlayerStatus : MonoBehaviourPun, IDamageable, IPunObservable
{
	// ���Ͱ� Ÿ���� �����ϱ� ���� ����Ʈ
	public static List<Transform> playerTransformList = new();

	[SerializeField] private GameObject hpPrefab;

	public float power;
	public float attackRate;
	public float maxHp;
	public float hp;
	public float maxStemina;
	public float stemina;
	public bool dead;

	new Transform transform;

	PlayerControl playerControl;
	PlayerMovement playerMovement;
	PlayerHud playerHud;
	Score score;
	MyStat myStat;

	private void Awake()
	{
		// ������Ʈ ĳ��
		playerControl = GetComponent<PlayerControl>();
		playerMovement = GetComponent<PlayerMovement>();
		transform = base.transform;
	}

	private void Start()
	{
		if(photonView.IsMine)
		{
			// �κ��丮�� ����
			Inventory.Instance.status = this;
			// �ڽ��� �÷��̾�� UI�� �����ϱ� ���� ĳ��
			playerHud = PlayerHud.Instance;
			score = Score.Instance;
			myStat = MyStat.Instance;
		}

		if (hpPrefab != null)
		{
			// hpBar ����
			GameObject hpUI = Instantiate(hpPrefab);
			// ���ӿ�����Ʈ�� SetTarget�Լ� ����
			hpUI.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
		}
		else
		{
			// hp�������� ���� �� �����
			Debug.LogWarning("<Color=Red><a>Missing</a></Color> PlayerUiPrefab reference on player Prefab.", this);
		}
		// �÷��̾� ��ġ ����Ʈ�� �ڽ��� �߰�
		playerTransformList.Add(transform);

		// �ڽ��� hp�� ui�� ���
		if (photonView.IsMine)
		{
			myStat.UpdateHp(hp, maxHp);
			myStat.UpdateStemina(stemina, maxStemina);
		}
	}

	private void Update()
	{
		// ���׹̳� �ڿ� ȸ��
		IncreaseStemina(2 * Time.deltaTime);
	}

	// ���׹̳� ���
	public void UseStemina(float amount)
	{
		// �ִ� �ּ� ����
		stemina = Mathf.Clamp(stemina - amount, 0, maxStemina);
		// �ڽ��� �������ͽ��� ui������Ʈ
		if (photonView.IsMine)
			myStat.UpdateStemina(stemina, maxStemina);
	}

	// ���׹̳� ȸ��
	public void IncreaseStemina(float amount)
	{
		// �ִ� �ּ� ����
		stemina = Mathf.Clamp(stemina + amount, 0, maxStemina);
		// �ڽ��� �������ͽ��� ui������Ʈ
		if (photonView.IsMine)
			myStat.UpdateStemina(stemina, maxStemina);
	}

	// ������ �� ȣ��
	private void ReStart()
	{
		// �÷��̾� ��ġ ����Ʈ�� �߰�
		playerTransformList.Add(transform);
		dead = false;
		hp = maxHp;
		stemina = maxStemina;
		// �ڽ��� hp�� ui�� ���
		if (photonView.IsMine)
		{
			myStat.UpdateHp(hp, maxHp);
			myStat.UpdateStemina(stemina, maxStemina);
		}

		// �÷��̾� ������ �޴� ������Ʈ�� Ȱ��ȭ
		playerMovement.enabled = true;
		playerControl.enabled = true;
	}


	// hp�� ����ȭ �ϱ� ���� �Լ�
	[PunRPC]
	public void ApplyUpdatedHealth(float newHp, bool isDead)
	{
		hp = newHp;
		dead = isDead;
	}

	// ü�� ȸ�� �Լ�
	public void IncreaseHp(float amount)
	{
		hp += amount;
		if (hp > maxHp)
			hp = maxHp;
		if (photonView.IsMine)
		{
			myStat.UpdateHp(hp, maxHp);
		}
	}

	// �ǰ� �Լ�
	[PunRPC]
	public void OnDamage(float damage)
	{
		// �׾��ٸ� �Լ� �������� ����
		if (dead)
			return;

		if (PhotonNetwork.IsMasterClient)
		{
			// ȣ��Ʈ�� ���� ���°��� ����
			hp -= damage;
			if (hp <= 0)
				hp = 0;
			
			// ȣ��Ʈ���� Ŭ���̾�Ʈ�� ����ȭ
			photonView.RPC("ApplyUpdatedHealth", RpcTarget.Others, hp, dead);
			// �ٸ� Ŭ���̾�Ʈ�鵵 OnDamage�� �����ϵ��� ��
			photonView.RPC("OnDamage", RpcTarget.Others, damage);
		}

		// �ڽ��� �÷��̾� UI������Ʈ, ���� ����  
		if (photonView.IsMine)
		{
			playerHud.GetDamaged();
			score.GetDamaged(damage);
			myStat.UpdateHp(hp, maxHp);
		}

		// ü���� 0 ���� && ���� ���� �ʾҴٸ� ��� ó�� ����
		if (hp <= 0)
		{
			dead = true;
			// ĳ���� �Է��� ����
			playerMovement.enabled = false;
			playerControl.enabled = false;
			StartCoroutine(IDead());
			// ���� ������ ��� ī��Ʈ �߰�
			if (photonView.IsMine)
				score.Die();
		}
	}

	// hp�� ����ȭ
	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.IsWriting)
		{
			stream.SendNext(hp);
		}
		else if (stream.IsReading)
		{
			this.hp = (float)stream.ReceiveNext();
		}
	}
	
	IEnumerator IDead()
	{
		// ��� �� Ÿ�� ����Ʈ���� ����
		PlayerStatus.playerTransformList.Remove(transform);
		if(photonView.IsMine)
			playerHud.CountDown(5f);
		yield return new WaitForSeconds(5f);
		// 5�� �� ������
		ReStart();
	}
}
