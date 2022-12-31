using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

// ���� ���� ����
public class MonsterStatus : MonoBehaviourPun, IDamageable, IPunObservable
{
	[SerializeField] GameObject hpPrefab;
	[SerializeField] GameObject deadParticle;
	[SerializeField] new CapsuleCollider collider;
	// óġ �� ���
	[SerializeField] int price;

	public float maxHp;
	public float hp;

	[HideInInspector]
	public bool dead = false;

	Animator anim;
	Score score;

	private void Start()
	{
		// ĳ��
		anim = GetComponent<Animator>();
		score = Score.Instance;

		// hpBar ����
		if (hpPrefab != null)
		{
			GameObject hpUI = Instantiate(hpPrefab);
			// hpBar�� SetTarget�Լ� ����
			hpUI.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
		}
		else
		{
			Debug.LogWarning("<Color=Red><a>Missing</a></Color> PlayerUiPrefab reference on player Prefab.", this);
		}
		hp = maxHp;
	}

	// hp ����ȭ �Լ�
	[PunRPC]
	public void ApplyUpdatedHealth(float newHp, bool isDead)
	{
		hp = newHp;
		dead = isDead;
	}

	// �ǰ� �Լ�
	[PunRPC]
	public void OnDamage(float damage)
	{
		// ��� �� �������� ����
		if (dead)
			return;
		anim.SetTrigger("onDamaged");
		if (PhotonNetwork.IsMasterClient)
		{
			// ȣ��Ʈ���� ���� hp����
			hp -= damage;

			// ȣ��Ʈ���� Ŭ���̾�Ʈ�� ����ȭ
			photonView.RPC("ApplyUpdatedHealth", RpcTarget.Others, hp, dead);

			// �ٸ� Ŭ���̾�Ʈ�鵵 OnDamage�� �����ϵ��� ��
			photonView.RPC("OnDamage", RpcTarget.Others, damage);
		}

		// ü���� 0 ���� && ���� ���� �ʾҴٸ� ��� ó�� ����
		if (hp <= 0 && !dead)
		{
			dead = true;
			// ��� ����Ʈ ����
			Instantiate(deadParticle, transform.position, transform.rotation);
			
			collider.enabled = false;
			// ���� óġ �� ��� ȹ��
			Inventory.Instance.EarnGold(price);
			// ���� ���� ������Ʈ
			score.AddKill();

			// �ִϸ��̼� ����
			anim.SetTrigger("onDead");
			anim.SetBool("isDead", dead);

			// ȣ��Ʈ�� ���� �ı�
			if (PhotonNetwork.IsMasterClient)
				StartCoroutine(IDead());
		}
	}

	// ���漭������ hp����ȭ
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

	// 2�� �� ����
	IEnumerator IDead()
	{
		yield return new WaitForSeconds(2f);
		PhotonNetwork.Destroy(gameObject);
	}
}
