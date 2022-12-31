using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

// 몬스터 상태 정보
public class MonsterStatus : MonoBehaviourPun, IDamageable, IPunObservable
{
	[SerializeField] GameObject hpPrefab;
	[SerializeField] GameObject deadParticle;
	[SerializeField] new CapsuleCollider collider;
	// 처치 시 골드
	[SerializeField] int price;

	public float maxHp;
	public float hp;

	[HideInInspector]
	public bool dead = false;

	Animator anim;
	Score score;

	private void Start()
	{
		// 캐싱
		anim = GetComponent<Animator>();
		score = Score.Instance;

		// hpBar 생성
		if (hpPrefab != null)
		{
			GameObject hpUI = Instantiate(hpPrefab);
			// hpBar의 SetTarget함수 실행
			hpUI.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
		}
		else
		{
			Debug.LogWarning("<Color=Red><a>Missing</a></Color> PlayerUiPrefab reference on player Prefab.", this);
		}
		hp = maxHp;
	}

	// hp 동기화 함수
	[PunRPC]
	public void ApplyUpdatedHealth(float newHp, bool isDead)
	{
		hp = newHp;
		dead = isDead;
	}

	// 피격 함수
	[PunRPC]
	public void OnDamage(float damage)
	{
		// 사망 시 실행하지 않음
		if (dead)
			return;
		anim.SetTrigger("onDamaged");
		if (PhotonNetwork.IsMasterClient)
		{
			// 호스트에서 먼저 hp감소
			hp -= damage;

			// 호스트에서 클라이언트로 동기화
			photonView.RPC("ApplyUpdatedHealth", RpcTarget.Others, hp, dead);

			// 다른 클라이언트들도 OnDamage를 실행하도록 함
			photonView.RPC("OnDamage", RpcTarget.Others, damage);
		}

		// 체력이 0 이하 && 아직 죽지 않았다면 사망 처리 실행
		if (hp <= 0 && !dead)
		{
			dead = true;
			// 사망 이펙트 생성
			Instantiate(deadParticle, transform.position, transform.rotation);
			
			collider.enabled = false;
			// 몬스터 처치 시 골드 획득
			Inventory.Instance.EarnGold(price);
			// 최종 점수 업데이트
			score.AddKill();

			// 애니메이션 실행
			anim.SetTrigger("onDead");
			anim.SetBool("isDead", dead);

			// 호스트가 몬스터 파괴
			if (PhotonNetwork.IsMasterClient)
				StartCoroutine(IDead());
		}
	}

	// 포톤서버에서 hp동기화
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

	// 2초 후 실행
	IEnumerator IDead()
	{
		yield return new WaitForSeconds(2f);
		PhotonNetwork.Destroy(gameObject);
	}
}
