using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 총기 재화 아이템 UI 관리
public class InventoryUI : MonoBehaviour
{
	[SerializeField] Text mineralText; 
	[SerializeField] Text goldText; 
    [SerializeField] ItemSlotUI[] slots;
	[SerializeField] ItemDescription description;

	private void Start()
	{
		// 아이템 슬롯에 아이템 정보를 알려주는 델리게이트 등록
		foreach(ItemSlotUI slot in slots)
		{
			slot.onEnterEvent += ShowItemInfo;
			slot.onExitEvent += OffItemInfo;
		}
	}

	// 재화ui 업데이트
	public void UpdateUI(int mineral, int gold)
	{
		mineralText.text = mineral.ToString();
		goldText.text = gold.ToString();
	}

	// 아이템 슬롯 셋팅
	public void SetupItemSlot(int index, Item item)
	{
		slots[index].Setup(item);
	}

	// 아이템 슬롯 업데이트
	public void UpdateItemSlot(int index)
	{
		slots[index].UpdateUI();
	}

	// 아이템 정보를 보여줌
	public void ShowItemInfo(ItemSlotUI slot)
	{
		description.gameObject.SetActive(true);
		description.transform.position = slot.transform.position;
		description.SetItem(slot.item);
	}

	// 아이템 정보 끄기
	public void OffItemInfo(ItemSlotUI slot)
	{
		description.gameObject.SetActive(false);
	}
}
