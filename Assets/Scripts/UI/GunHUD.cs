using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 크로스 헤어, 조준경, 총알UI 관리
public class GunHUD : Singleton<GunHUD>
{
	[SerializeField] Text currentBulletText;
	[SerializeField] Text maxBulletText;
	[SerializeField] BulletUI bulletUI;
	[SerializeField] Image zoomOutImage;
	[SerializeField] Image zoomInImage;


	private void Start()
	{
		// 초기 총알수와 크로스헤어
		currentBulletText.text = 0.ToString();
		maxBulletText.text = 0.ToString();
		zoomOutImage.enabled = true;
		zoomInImage.enabled = false;
	}

	// 총 변경시 ui설정
	public void SetGun(int current, int max)
	{
		// 총알 UI 업데이트
		bulletUI.ChangeWeapon(current, max);
		currentBulletText.text = current.ToString();
		maxBulletText.text = max.ToString();
	}

	// 발사시 총알 업데이트
	public void Shoot(int current)
	{
		currentBulletText.text = current.ToString();
		bulletUI.ShotBullet();
	}

	// 재장전시 총알 업데이트
	public void ReloadBullet(int max)
	{
		currentBulletText.text = max.ToString();
		bulletUI.Reload();
	}

	// 줌인 줌아웃시 크로스헤어와 조준경 변경
	public void UpdateAim(bool isAiming)
	{
		if(isAiming)
		{
			zoomInImage.enabled = true;
			zoomOutImage.enabled = false;
		}
		else
		{
			zoomInImage.enabled = false;
			zoomOutImage.enabled = true;
		}
	}
}
