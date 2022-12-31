using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

// 원거리 몬스터가 공격할 때 사용하는 투사체
public class MonsterBullet : MonoBehaviourPun
{
	[SerializeField] Rigidbody rigid;
	[SerializeField] ParticleSystem explosionPrefab;

	float power;

	private void OnDisable()
	{
		// 사라질 때 파티클 생성
		Instantiate(explosionPrefab, transform.position, Quaternion.identity);
	}
	// 투사체의 초기설정
	public void Setup(float bulletSpeed, float power, Vector3 dir)
	{
		// 방향과 공격력 설정
		rigid.velocity = bulletSpeed * dir;
		this.power = power;
		// 무한히 날아가는 것을 방지하기 위해 일정시간 후 파괴
		StartCoroutine(TimeOut());
	}

	// 충돌
	private void OnCollisionEnter(Collision collision)
	{
		// 충돌체에서 IDamageable 컴포넌트 추출
		IDamageable hit = collision.gameObject.GetComponent<IDamageable>();
		// 충졸체가 IDamageable함수가 있을 경우
		if (hit != null && collision.gameObject.CompareTag("Player"))
		{
			// rpc로 IDamageable의 OnDamaged함수 실행
			collision.gameObject.GetComponent<PhotonView>().RPC("OnDamage", RpcTarget.All, power);
		}
		// 투사체 파괴
		photonView.RPC(nameof(DestroyBullert), RpcTarget.All);
	}

	// 투사체 파괴
	[PunRPC] 
	private void DestroyBullert()
	{
		// 호스트만 파괴가능
		if (PhotonNetwork.IsMasterClient)
			PhotonNetwork.Destroy(gameObject);
	}
	// 일정시간 후에 투사체 파괴
	IEnumerator TimeOut()
	{
		yield return new WaitForSeconds(3f);
		photonView.RPC(nameof(DestroyBullert), RpcTarget.All);
	}
}
