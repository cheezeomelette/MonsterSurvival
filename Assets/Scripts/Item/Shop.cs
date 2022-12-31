using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

// ����
public class Shop : MonoBehaviourPun
{
	[Header("All Item")]
	[SerializeField] Gun[] guns;                        // ��� �ѱ� �迭
	[SerializeField] Item[] items;						// ��� ������ �迭

	[SerializeField] Transform itemTransform;			// layout������Ʈ�� �ִ� ������ �θ� Transform
	[SerializeField] Transform weaponTransform;			// layout������Ʈ�� �ִ� ���� �θ� Transform
	[SerializeField] ShopItemSlot itemSlot;
	[SerializeField] ShopWeaponSlot weaponSlot;
	[SerializeField] WeaponDescription weaponDescription;
	[SerializeField] ItemDescription itemDescription;

	private Dictionary<string, Item> itemDictionary = new();  // �����۸��

	WeaponManager weaponManager;

	private void Start()
	{
		foreach (Item item in items)                            // ��� ������ ���
		{
			//Item newItem = Instantiate(item);
			itemDictionary.Add(item.itemData.itemName, item);
		}

		foreach (Transform slot in itemTransform)           // �̸������ ���� ����
		{
			Destroy(slot.gameObject);
		}
		foreach (Transform slot in weaponTransform)           // �̸������ ���� ����
		{
			Destroy(slot.gameObject);
		}
		for (int i = 0; i < guns.Length; i++)               // �ѱ� ���� ä���
		{
			ShopWeaponSlot newSlot = Instantiate(weaponSlot, weaponTransform);
			newSlot.Setup(guns[i]);
			// ������ eventTrigger ��������Ʈ�� �Լ� ���
			newSlot.onEnterEvent += ShowWeaponInfo;
			newSlot.onExitEvent += OffWeaponInfo;
			newSlot.onclickEvent += BuyWeapon;
		}
		for (int i = 0; i < items.Length; i++)              // ������ ���� ä���
		{
			ShopItemSlot newSlot = Instantiate(itemSlot, itemTransform);
			newSlot.Setup(items[i]);
			// ������ eventTrigger ��������Ʈ�� �Լ� ���
			newSlot.onEnterEvent += ShowItemInfo;
			newSlot.onExitEvent += OffItemInfo;
			newSlot.onclickEvent += BuyItem;
		}
		// ������ �� ���� ����
		gameObject.SetActive(false);
	}
	// ��������â�� ���� ���������� ���
	public void ShowWeaponInfo(WeaponSlotUI slot)
	{
		weaponDescription.gameObject.SetActive(true);
		weaponDescription.transform.position = slot.transform.position;
		weaponDescription.SetItem(slot.gun);
	}
	// ��������â ����
	public void OffWeaponInfo(WeaponSlotUI slot)
	{
		weaponDescription.gameObject.SetActive(false);
	}
	// ����������â ���� ���� ���
	public void ShowItemInfo(ItemSlotUI slot)
	{
		itemDescription.gameObject.SetActive(true);
		itemDescription.transform.position = slot.transform.position;
		itemDescription.SetItem(slot.item);
	}
	// ����â ����
	public void OffItemInfo(ItemSlotUI slot)
	{
		itemDescription.gameObject.SetActive(false);
	}
	//���� ����
	private void BuyWeapon(Gun gun)
	{
		// �� ������ �ִ���
		if (!weaponManager.IsEnoughWeaponSlot())
		{
			PopupMessage.Instance.Show("���� ������ �����մϴ�.",1f);
			return;
		}
		
		// ������ �������
		if(!Inventory.Instance.IsEnoughMineral(gun.weaponData.price))
		{
			PopupMessage.Instance.Show("��尡 �����մϴ�.",1f);
			return;
		}

		// ����
		Inventory.Instance.UseMineral(gun.weaponData.price);
		weaponManager.photonView.RPC("AddWeapon", RpcTarget.All, gun.name);
	}
	// ������ ����
	private void BuyItem(Item item)
	{
		// ��尡 �������
		if (!Inventory.Instance.IsEnoughGold(item.itemData.price))
		{
			PopupMessage.Instance.Show("��尡 �����մϴ�.", 1f);
			return;
		}
		// �����
		if (Inventory.Instance.UseGold(item.itemData.price))
		{
			// ������ �߰�
			Inventory.Instance.AddItem(Instantiate(item, Inventory.Instance.transform));
		}
	}
	// ���ӽ��� �� ������ �÷��̾ ����ִ� WeaponManager�� ���
	public void SetManager(WeaponManager manager)
	{
		weaponManager = manager;
	}
}
