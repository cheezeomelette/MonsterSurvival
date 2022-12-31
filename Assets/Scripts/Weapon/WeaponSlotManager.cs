using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// WeaponSlot����
public class WeaponSlotManager : Singleton<WeaponSlotManager>
{
    [SerializeField] WeaponSlotUI[] weaponSlots;    // �ѱ� ����

	WeaponDescription description;

	private void Start()
	{
		// ĳ��
		description = WeaponDescription.Instance;
		// ���콺 �Է¿� ���� ��������Ʈ ����
		foreach (WeaponSlotUI slot in weaponSlots)
		{
			slot.onEnterEvent += ShowItemInfo;
			slot.onExitEvent += OffItemInfo;
			slot.onClickEvent += DropWeapon;
		}
	}

	// ���Կ� �� ���
	public void SetSlot(int i, Gun gun)
	{
		weaponSlots[i].Setup(gun);
	}

	/// ���� ���� ������Ʈ
	public void ShowItemInfo(WeaponSlotUI slot)
	{
		description.gameObject.SetActive(true);
		description.transform.position = slot.transform.position;
		description.SetItem(slot.gun);
	}

	// ���� ����â ����
	public void OffItemInfo(WeaponSlotUI slot)
	{
		description.gameObject.SetActive(false);
	}

	// ���� ������
	public void DropWeapon(WeaponSlotUI slot)
	{
		Destroy(slot.gun.gameObject);
		slot.DropGun();
	}
}
