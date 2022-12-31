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

	// ���� ���踦 ���� ��������Ʈ
	System.Action<float> scoreDamage;

    public void Setup(float bulletSpeed, float power, Vector3 dir, 
		System.Action<float> scoreDamage)
	{
		this.scoreDamage = scoreDamage;
		this.power = power;

		// �Ѿ��� �����ϱ� ������ Ŭ���� ������Ѵ�
		trailRenderer.Clear();
		rigid.velocity = bulletSpeed * dir;
		// �Ѿ��� ������ ���ư��°��� ����
		StartCoroutine(TimeOut());
	}

	private void OnCollisionEnter(Collision collision)
	{
		Debug.Log(collision.gameObject.name);
		// �Ѿ� �浹 �� ����Ʈ �߻�
		BulletEffect effect = BulletEffectManager.Instance.GetPool();
		effect.transform.position = transform.position;

		IDamageable hit = collision.gameObject.GetComponent<IDamageable>();

		// �浹ü�� IDamageable�� �ִٸ� OnDamage�� ����
		if (hit != null)
		{
			collision.gameObject.GetComponent<PhotonView>().RPC("OnDamage", RpcTarget.All, power);
			scoreDamage?.Invoke(power);
		}

		BulletManager.Instance.ReturnPool(this);
	}

	// 3�ʵ� �ı�
	IEnumerator TimeOut()
	{
		yield return new WaitForSeconds(3f);
		BulletManager.Instance.ReturnPool(this);
	}
}
