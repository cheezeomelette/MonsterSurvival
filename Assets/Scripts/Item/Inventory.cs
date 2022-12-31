using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// ��ȭ�� ������ ��� ����
public class Inventory : Singleton<Inventory>   // ��ȭ ������ UI ����
{
	[SerializeField] InventoryUI inventoryUI;
	// ������ ����� ���� �÷��̾� ����
	public PlayerStatus status;
    public int mineral { get; private set; }
    public int gold { get; private set; }
	// ���� �����۵�
	private Item[] items;

	private void Start()
	{
		// ��ȭ �ʱ�ȭ
		mineral = 0;
		gold = 0;
		inventoryUI.UpdateUI(mineral, gold);

		// ������ ���� ����
		items = new Item[3];
	}
	private void Update()
	{
		UseItem();
	}

	private void UseItem()
	{
		// ������ ���
		if (Input.GetKeyDown(KeyCode.Alpha3))
		{
			if (items[0] != null)
			{
				items[0].UseItem(status);
				// ui ������Ʈ
				inventoryUI.UpdateItemSlot(0);
			}
		}
		if (Input.GetKeyDown(KeyCode.Alpha4))
		{
			if (items[1] != null)
			{
				items[1].UseItem(status);
				inventoryUI.UpdateItemSlot(1);
			}
		}
		if (Input.GetKeyDown(KeyCode.Alpha5))
		{
			if (items[2] != null)
			{
				items[2].UseItem(status);
				inventoryUI.UpdateItemSlot(2);
			}
		}
	}

	// ���� ����ϱ��� ����� �ִ��� Ȯ��
	public bool IsEnoughMineral(int amount)
	{
		return mineral >= amount;
	}
	// ���� ä�� �Լ�
	public void EarnMineral()
	{
        mineral += 1;
		// ui������Ʈ
		inventoryUI.UpdateUI(mineral,gold);
	}
	// ���� ���
	public void UseMineral(int amount)
	{
		if(mineral < amount)
		{
			// ������ �� PopupMessage�� �޽��� ���
			PopupMessage.Instance.Show("�̳׶��� �����մϴ�.", 1f);
			return;
		}
		mineral -= amount;
		inventoryUI.UpdateUI(mineral,gold);
	}
	// ��� ��� �� ������� Ȯ��
	public bool IsEnoughGold(int amount)
	{
		return gold >= amount;
	}
	// ��� ȹ��
	public void EarnGold(int amount)
	{
		gold += amount;
		inventoryUI.UpdateUI(mineral,gold);
	}
	// ��� ���
	public bool UseGold(int amount)
	{
		if(gold < amount)
		{
			return false;
		}
		gold -= amount;
		inventoryUI.UpdateUI(mineral,gold);
		return true;
	}
	// �κ��丮�� �ִ� �������� �˻��ؼ� ������ ��ġ ����
	private int FindItem(string itemName)
	{
		for (int i = 0; i < items.Length; i++)
		{
			if (items[i] != null && items[i].itemData.itemName.Equals(itemName))
				return i;
		}
		// �����ۿ� ���ٸ� -1����
		return -1;
	}
	// ������ �߰�
	public void AddItem(Item newItem)
	{
		// ������ �ε��� ã��
		int index = FindItem(newItem.itemData.itemName);
		// �������� ã�Ҵٸ�
		if (index != -1)
		{
			// ������ ���� �߰�
			items[index].count += 1;
			inventoryUI.UpdateItemSlot(index);
			return;
		}
		// �� �����۽����� ã�� �������� �߰��Ѵ�
		for(int i = 0; i < items.Length; i++)
		{
			if (items[i] == null)
			{
				items[i] = newItem;
				inventoryUI.SetupItemSlot(i, items[i]);
				return;
			}
		}
	}
}
