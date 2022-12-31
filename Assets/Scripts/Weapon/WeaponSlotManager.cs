using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// WeaponSlot관리
public class WeaponSlotManager : Singleton<WeaponSlotManager>
{
    [SerializeField] WeaponSlotUI[] weaponSlots;    // 총기 슬롯

	WeaponDescription description;

	private void Start()
	{
		// 캐싱
		description = WeaponDescription.Instance;
		// 마우스 입력에 따른 델리게이트 설정
		foreach (WeaponSlotUI slot in weaponSlots)
		{
			slot.onEnterEvent += ShowItemInfo;
			slot.onExitEvent += OffItemInfo;
			slot.onClickEvent += DropWeapon;
		}
	}

	// 슬롯에 총 등록
	public void SetSlot(int i, Gun gun)
	{
		weaponSlots[i].Setup(gun);
	}

	/// 무기 정보 업데이트
	public void ShowItemInfo(WeaponSlotUI slot)
	{
		description.gameObject.SetActive(true);
		description.transform.position = slot.transform.position;
		description.SetItem(slot.gun);
	}

	// 무기 정보창 끄기
	public void OffItemInfo(WeaponSlotUI slot)
	{
		description.gameObject.SetActive(false);
	}

	// 무기 버리기
	public void DropWeapon(WeaponSlotUI slot)
	{
		Destroy(slot.gun.gameObject);
		slot.DropGun();
	}
}
