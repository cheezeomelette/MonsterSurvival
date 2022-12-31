using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 총알을 발사하는 투사체형이 아닌 히트스캔형, 관통형
public class Sniper : Gun
{
	[SerializeField] BulletLine line;

	// 격발 함수
	public override void Shoot()
	{
		if (isReloading)
			return;

		// 공격속도가 지나면 
		if (fireTime > weaponData.fireRate)
		{
			RaycastHit[] hits;
			// Ray를 발사해 ray에 맞은 모든 충돌체를 가져옴
			hits = Physics.RaycastAll(cam.transform.position, cam.transform.forward, float.MaxValue);
			if(hits.Length > 0)
			{
				// 모든 충돌체에서
				foreach(RaycastHit hit in hits)
				{
					if (hit.collider.CompareTag("Player"))
						continue;
					// IDamageable을 가져와서 OnDamage 호출
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
