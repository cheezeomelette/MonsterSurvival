using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

// �÷��̾ ������ ���⸦ ����
public class WeaponManager : MonoBehaviourPun
{
	// ��� �ѱ� �迭
	[SerializeField] Gun[] allGuns;                          
	// ���� �� ���� ��ġ
	[SerializeField] Transform gunPivot;
	[SerializeField] GameObject effect;

	// ���� ����ִ� ����
	[HideInInspector]
	public Gun currentWeapon;

	// �ѱ���
	private Dictionary<string, Gun> gunDictionary = new();  
	// ������ �� ���
	private Gun[] guns;

	private void Start()
	{
		// ��� �ѱ� ���
		foreach (Gun gun in allGuns)                           
		{
			gunDictionary.Add(gun.weaponData.weaponName, gun);
		}

		guns = new Gun[2];
	}
	// �ݹ� �Լ�
	public void Shoot()
	{
		// ���� ����ִ� ������ shoot�Լ� ����
		if (currentWeapon != null && currentWeapon.canFire && !currentWeapon.isReloading)
		{
			currentWeapon.Shoot();
			ShootEffect();
		}
	}
	// ���� ����ִ� ������ ������
	public void Reloading()
	{
		if (currentWeapon == null)
			return;
		currentWeapon.Reloading();
	}
	// ���� ����ִ� ������ ������ �°� ����
	public float ZoomIn()
	{
		currentWeapon.ZoomIn();
		return 1 / currentWeapon.magnification;
	}

	// �ݹ� ����Ʈ ����
	[PunRPC]
	private void ShootEffect()
	{
		Instantiate(effect, currentWeapon.bulletPivot.position, currentWeapon.bulletPivot.rotation);
	}

	// �տ� �� ���� ����
	public void SwapWeapon(int index)
	{
		if (guns[index] == null)
			return;
		// �տ� �� ���� ����ȭ
		photonView.RPC(nameof(ChangeGunModel), RpcTarget.All, index);

		// �ڽ��� ���� �ٲ�ٸ� �Ѿ�ui������Ʈ
		if(photonView.IsMine)
			GunHUD.Instance.SetGun(currentWeapon.currentBulletCount, currentWeapon.weaponData.maxBulletCount); // �Ѿ˼� ui ������Ʈ
	}

	// ���� ����ȭ
	[PunRPC]
	public void ChangeGunModel(int index)
	{
		if (currentWeapon != null)
			currentWeapon.gameObject.SetActive(false);

		currentWeapon = guns[index];
		currentWeapon.gameObject.SetActive(true);
	}

	// ���� ���Ž� ���Կ� ������ �ִ���
	public bool IsEnoughWeaponSlot()
	{
		for(int i = 0; i < guns.Length; i++)
		{
			if (guns[i] == null)
				return true;
		}
		return false;
	}

	// ���� �߰�
	[PunRPC]
	public void AddWeapon(string gunName)
	{
		for (int i = 0; i < guns.Length; i++)
		{
			// �� ������ ã�� �ѱ� �߰�
			if (guns[i] == null)
			{
				Gun gun = Instantiate(gunDictionary[gunName], gunPivot.position, gunPivot.rotation, gunPivot);
				gun.gameObject.SetActive(false);
				guns[i] = gun;
				if(photonView.IsMine)
					WeaponSlotManager.Instance.SetSlot(i, gun);
				return;
			}
		}
	}
}
