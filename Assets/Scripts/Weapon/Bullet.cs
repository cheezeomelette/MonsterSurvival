using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Bullet : MonoBehaviour
{
    [SerializeField] Rigidbody rigid;
	[SerializeField] ParticleSystem prefab;
	[SerializeField] TrailRenderer trailRenderer;
	float power;

	// 점수 집계를 위한 델리게이트
	System.Action<float> scoreDamage;

    public void Setup(float bulletSpeed, float power, Vector3 dir, 
		System.Action<float> scoreDamage)
	{
		this.scoreDamage = scoreDamage;
		this.power = power;

		// 총알을 재사용하기 때문에 클리어 해줘야한다
		trailRenderer.Clear();
		rigid.velocity = bulletSpeed * dir;
		// 총알이 무한히 날아가는것을 방지
		StartCoroutine(TimeOut());
	}

	private void OnCollisionEnter(Collision collision)
	{
		Debug.Log(collision.gameObject.name);
		// 총알 충돌 시 이펙트 발생
		BulletEffect effect = BulletEffectManager.Instance.GetPool();
		effect.transform.position = transform.position;

		IDamageable hit = collision.gameObject.GetComponent<IDamageable>();

		// 충돌체가 IDamageable이 있다면 OnDamage를 실행
		if (hit != null)
		{
			collision.gameObject.GetComponent<PhotonView>().RPC("OnDamage", RpcTarget.All, power);
			scoreDamage?.Invoke(power);
		}

		BulletManager.Instance.ReturnPool(this);
	}

	// 3초뒤 파괴
	IEnumerator TimeOut()
	{
		yield return new WaitForSeconds(3f);
		BulletManager.Instance.ReturnPool(this);
	}
}
