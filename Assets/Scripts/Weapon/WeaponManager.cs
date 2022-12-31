using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

// 플레이어가 보유한 무기를 관리
public class WeaponManager : MonoBehaviourPun
{
	// 모든 총기 배열
	[SerializeField] Gun[] allGuns;                          
	// 총을 들 손의 위치
	[SerializeField] Transform gunPivot;
	[SerializeField] GameObject effect;

	// 현재 들고있는 무기
	[HideInInspector]
	public Gun currentWeapon;

	// 총기목록
	private Dictionary<string, Gun> gunDictionary = new();  
	// 보유한 총 목록
	private Gun[] guns;

	private void Start()
	{
		// 모든 총기 등록
		foreach (Gun gun in allGuns)                           
		{
			gunDictionary.Add(gun.weaponData.weaponName, gun);
		}

		guns = new Gun[2];
	}
	// 격발 함수
	public void Shoot()
	{
		// 현재 들고있는 무기의 shoot함수 실행
		if (currentWeapon != null && currentWeapon.canFire && !currentWeapon.isReloading)
		{
			currentWeapon.Shoot();
			ShootEffect();
		}
	}
	// 현재 들고있는 무기의 재장전
	public void Reloading()
	{
		if (currentWeapon == null)
			return;
		currentWeapon.Reloading();
	}
	// 현재 들고있는 무기의 배율에 맞게 줌인
	public float ZoomIn()
	{
		currentWeapon.ZoomIn();
		return 1 / currentWeapon.magnification;
	}

	// 격발 이펙트 생성
	[PunRPC]
	private void ShootEffect()
	{
		Instantiate(effect, currentWeapon.bulletPivot.position, currentWeapon.bulletPivot.rotation);
	}

	// 손에 들 무기 변경
	public void SwapWeapon(int index)
	{
		if (guns[index] == null)
			return;
		// 손에 든 무기 동기화
		photonView.RPC(nameof(ChangeGunModel), RpcTarget.All, index);

		// 자신이 총을 바꿨다면 총알ui업데이트
		if(photonView.IsMine)
			GunHUD.Instance.SetGun(currentWeapon.currentBulletCount, currentWeapon.weaponData.maxBulletCount); // 총알수 ui 업데이트
	}

	// 무기 동기화
	[PunRPC]
	public void ChangeGunModel(int index)
	{
		if (currentWeapon != null)
			currentWeapon.gameObject.SetActive(false);

		currentWeapon = guns[index];
		currentWeapon.gameObject.SetActive(true);
	}

	// 무기 구매시 슬롯에 여유가 있는지
	public bool IsEnoughWeaponSlot()
	{
		for(int i = 0; i < guns.Length; i++)
		{
			if (guns[i] == null)
				return true;
		}
		return false;
	}

	// 무기 추가
	[PunRPC]
	public void AddWeapon(string gunName)
	{
		for (int i = 0; i < guns.Length; i++)
		{
			// 빈 슬롯을 찾아 총기 추가
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
