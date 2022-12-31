using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �Ѿ��� �߻��ϴ� ����ü���� �ƴ� ��Ʈ��ĵ��, ������
public class Sniper : Gun
{
	[SerializeField] BulletLine line;

	// �ݹ� �Լ�
	public override void Shoot()
	{
		if (isReloading)
			return;

		// ���ݼӵ��� ������ 
		if (fireTime > weaponData.fireRate)
		{
			RaycastHit[] hits;
			// Ray�� �߻��� ray�� ���� ��� �浹ü�� ������
			hits = Physics.RaycastAll(cam.transform.position, cam.transform.forward, float.MaxValue);
			if(hits.Length > 0)
			{
				// ��� �浹ü����
				foreach(RaycastHit hit in hits)
				{
					if (hit.collider.CompareTag("Player"))
						continue;
					// IDamageable�� �����ͼ� OnDamage ȣ��
					IDamageable enemy = hit.collider.GetComponent<IDamageable>();
					if(enemy!= null)
						enemy.OnDamage(weaponData.power);
				}
			}

			BulletLine bulletLine = Instantiate(line, bulletPivot.position, Quaternion.identity);
			if (hits.Length > 0)
				bulletLine.Shoot(hits[hits.Length - 1].point - bulletPivot.position);
			else
				bulletLine.Shoot(bulletPivot.forward);

			fireTime = 0f;
			currentBulletCount -= 1;
			if (currentBulletCount <= 0)
				StartCoroutine(Reload());
			GunHUD.Instance.Shoot(currentBulletCount);
			source.PlayOneShot(shootClip);
		}
	}
}
