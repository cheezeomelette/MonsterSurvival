using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// ũ�ν� ���, ���ذ�, �Ѿ�UI ����
public class GunHUD : Singleton<GunHUD>
{
	[SerializeField] Text currentBulletText;
	[SerializeField] Text maxBulletText;
	[SerializeField] BulletUI bulletUI;
	[SerializeField] Image zoomOutImage;
	[SerializeField] Image zoomInImage;


	private void Start()
	{
		// �ʱ� �Ѿ˼��� ũ�ν����
		currentBulletText.text = 0.ToString();
		maxBulletText.text = 0.ToString();
		zoomOutImage.enabled = true;
		zoomInImage.enabled = false;
	}

	// �� ����� ui����
	public void SetGun(int current, int max)
	{
		// �Ѿ� UI ������Ʈ
		bulletUI.ChangeWeapon(current, max);
		currentBulletText.text = current.ToString();
		maxBulletText.text = max.ToString();
	}

	// �߻�� �Ѿ� ������Ʈ
	public void Shoot(int current)
	{
		currentBulletText.text = current.ToString();
		bulletUI.ShotBullet();
	}

	// �������� �Ѿ� ������Ʈ
	public void ReloadBullet(int max)
	{
		currentBulletText.text = max.ToString();
		bulletUI.Reload();
	}

	// ���� �ܾƿ��� ũ�ν����� ���ذ� ����
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
