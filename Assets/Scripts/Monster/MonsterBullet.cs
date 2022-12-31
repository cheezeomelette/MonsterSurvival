using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

// ���Ÿ� ���Ͱ� ������ �� ����ϴ� ����ü
public class MonsterBullet : MonoBehaviourPun
{
	[SerializeField] Rigidbody rigid;
	[SerializeField] ParticleSystem explosionPrefab;

	float power;

	private void OnDisable()
	{
		// ����� �� ��ƼŬ ����
		Instantiate(explosionPrefab, transform.position, Quaternion.identity);
	}
	// ����ü�� �ʱ⼳��
	public void Setup(float bulletSpeed, float power, Vector3 dir)
	{
		// ����� ���ݷ� ����
		rigid.velocity = bulletSpeed * dir;
		this.power = power;
		// ������ ���ư��� ���� �����ϱ� ���� �����ð� �� �ı�
		StartCoroutine(TimeOut());
	}

	// �浹
	private void OnCollisionEnter(Collision collision)
	{
		// �浹ü���� IDamageable ������Ʈ ����
		IDamageable hit = collision.gameObject.GetComponent<IDamageable>();
		// ����ü�� IDamageable�Լ��� ���� ���
		if (hit != null && collision.gameObject.CompareTag("Player"))
		{
			// rpc�� IDamageable�� OnDamaged�Լ� ����
			collision.gameObject.GetComponent<PhotonView>().RPC("OnDamage", RpcTarget.All, power);
		}
		// ����ü �ı�
		photonView.RPC(nameof(DestroyBullert), RpcTarget.All);
	}

	// ����ü �ı�
	[PunRPC] 
	private void DestroyBullert()
	{
		// ȣ��Ʈ�� �ı�����
		if (PhotonNetwork.IsMasterClient)
			PhotonNetwork.Destroy(gameObject);
	}
	// �����ð� �Ŀ� ����ü �ı�
	IEnumerator TimeOut()
	{
		yield return new WaitForSeconds(3f);
		photonView.RPC(nameof(DestroyBullert), RpcTarget.All);
	}
}
