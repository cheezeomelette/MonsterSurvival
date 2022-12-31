using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

// 상점
public class Shop : MonoBehaviourPun
{
	[Header("All Item")]
	[SerializeField] Gun[] guns;                        // 모든 총기 배열
	[SerializeField] Item[] items;						// 모든 아이템 배열

	[SerializeField] Transform itemTransform;			// layout컴포넌트가 있는 아이템 부모 Transform
	[SerializeField] Transform weaponTransform;			// layout컴포넌트가 있는 무기 부모 Transform
	[SerializeField] ShopItemSlot itemSlot;
	[SerializeField] ShopWeaponSlot weaponSlot;
	[SerializeField] WeaponDescription weaponDescription;
	[SerializeField] ItemDescription itemDescription;

	private Dictionary<string, Item> itemDictionary = new();  // 아이템목록

	WeaponManager weaponManager;

	private void Start()
	{
		foreach (Item item in items)                            // 모든 아이템 등록
		{
			//Item newItem = Instantiate(item);
			itemDictionary.Add(item.itemData.itemName, item);
		}

		foreach (Transform slot in itemTransform)           // 미리보기용 슬롯 비우기
		{
			Destroy(slot.gameObject);
		}
		foreach (Transform slot in weaponTransform)           // 미리보기용 슬롯 비우기
		{
			Destroy(slot.gameObject);
		}
		for (int i = 0; i < guns.Length; i++)               // 총기 슬롯 채우기
		{
			ShopWeaponSlot newSlot = Instantiate(weaponSlot, weaponTransform);
			newSlot.Setup(guns[i]);
			// 포인터 eventTrigger 델리게이트에 함수 등록
			newSlot.onEnterEvent += ShowWeaponInfo;
			newSlot.onExitEvent += OffWeaponInfo;
			newSlot.onclickEvent += BuyWeapon;
		}
		for (int i = 0; i < items.Length; i++)              // 아이템 슬롯 채우기
		{
			ShopItemSlot newSlot = Instantiate(itemSlot, itemTransform);
			newSlot.Setup(items[i]);
			// 포인터 eventTrigger 델리게이트에 함수 등록
			newSlot.onEnterEvent += ShowItemInfo;
			newSlot.onExitEvent += OffItemInfo;
			newSlot.onclickEvent += BuyItem;
		}
		// 시작할 때 상점 꺼둠
		gameObject.SetActive(false);
	}
	// 무기정보창을 띄우고 무기정보를 등록
	public void ShowWeaponInfo(WeaponSlotUI slot)
	{
		weaponDescription.gameObject.SetActive(true);
		weaponDescription.transform.position = slot.transform.position;
		weaponDescription.SetItem(slot.gun);
	}
	// 무기정보창 끄기
	public void OffWeaponInfo(WeaponSlotUI slot)
	{
		weaponDescription.gameObject.SetActive(false);
	}
	// 아이템정보창 띄우고 정보 등록
	public void ShowItemInfo(ItemSlotUI slot)
	{
		itemDescription.gameObject.SetActive(true);
		itemDescription.transform.position = slot.transform.position;
		itemDescription.SetItem(slot.item);
	}
	// 정보창 끄기
	public void OffItemInfo(ItemSlotUI slot)
	{
		itemDescription.gameObject.SetActive(false);
	}
	//무기 구매
	private void BuyWeapon(Gun gun)
	{
		// 빈 슬롯이 있는지
		if (!weaponManager.IsEnoughWeaponSlot())
		{
			PopupMessage.Instance.Show("무기 공간이 부족합니다.",1f);
			return;
		}
		
		// 광석이 충분한지
		if(!Inventory.Instance.IsEnoughMineral(gun.weaponData.price))
		{
			PopupMessage.Instance.Show("골드가 부족합니다.",1f);
			return;
		}

		// 구매
		Inventory.Instance.UseMineral(gun.weaponData.price);
		weaponManager.photonView.RPC("AddWeapon", RpcTarget.All, gun.name);
	}
	// 아이템 구매
	private void BuyItem(Item item)
	{
		// 골드가 충분한지
		if (!Inventory.Instance.IsEnoughGold(item.itemData.price))
		{
			PopupMessage.Instance.Show("골드가 부족합니다.", 1f);
			return;
		}
		// 골드사용
		if (Inventory.Instance.UseGold(item.itemData.price))
		{
			// 아이템 추가
			Inventory.Instance.AddItem(Instantiate(item, Inventory.Instance.transform));
		}
	}
	// 게임시작 후 생성된 플레이어가 들고있는 WeaponManager를 등록
	public void SetManager(WeaponManager manager)
	{
		weaponManager = manager;
	}
}
