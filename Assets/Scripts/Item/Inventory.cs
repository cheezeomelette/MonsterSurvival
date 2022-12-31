using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 재화와 아이템 사용 관리
public class Inventory : Singleton<Inventory>   // 재화 아이템 UI 관리
{
	[SerializeField] InventoryUI inventoryUI;
	// 아이템 사용을 위한 플레이어 정보
	public PlayerStatus status;
    public int mineral { get; private set; }
    public int gold { get; private set; }
	// 보유 아이템들
	private Item[] items;

	private void Start()
	{
		// 재화 초기화
		mineral = 0;
		gold = 0;
		inventoryUI.UpdateUI(mineral, gold);

		// 아이템 공간 생성
		items = new Item[3];
	}
	private void Update()
	{
		UseItem();
	}

	private void UseItem()
	{
		// 아이템 사용
		if (Input.GetKeyDown(KeyCode.Alpha3))
		{
			if (items[0] != null)
			{
				items[0].UseItem(status);
				// ui 업데이트
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

	// 광석 사용하기전 충분히 있는지 확인
	public bool IsEnoughMineral(int amount)
	{
		return mineral >= amount;
	}
	// 광석 채광 함수
	public void EarnMineral()
	{
        mineral += 1;
		// ui업데이트
		inventoryUI.UpdateUI(mineral,gold);
	}
	// 광석 사용
	public void UseMineral(int amount)
	{
		if(mineral < amount)
		{
			// 부족할 시 PopupMessage로 메시지 출력
			PopupMessage.Instance.Show("미네랄이 부족합니다.", 1f);
			return;
		}
		mineral -= amount;
		inventoryUI.UpdateUI(mineral,gold);
	}
	// 골드 사용 전 충분한지 확인
	public bool IsEnoughGold(int amount)
	{
		return gold >= amount;
	}
	// 골드 획득
	public void EarnGold(int amount)
	{
		gold += amount;
		inventoryUI.UpdateUI(mineral,gold);
	}
	// 골드 사용
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
	// 인벤토리에 있는 아이템을 검색해서 아이템 위치 리턴
	private int FindItem(string itemName)
	{
		for (int i = 0; i < items.Length; i++)
		{
			if (items[i] != null && items[i].itemData.itemName.Equals(itemName))
				return i;
		}
		// 아이템에 없다면 -1리턴
		return -1;
	}
	// 아이템 추가
	public void AddItem(Item newItem)
	{
		// 아이템 인덱스 찾기
		int index = FindItem(newItem.itemData.itemName);
		// 아이템을 찾았다면
		if (index != -1)
		{
			// 아이템 갯수 추가
			items[index].count += 1;
			inventoryUI.UpdateItemSlot(index);
			return;
		}
		// 빈 아이템슬롯을 찾아 아이템을 추가한다
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
