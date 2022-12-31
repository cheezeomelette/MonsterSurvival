using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// �ѱ� ��ȭ ������ UI ����
public class InventoryUI : MonoBehaviour
{
	[SerializeField] Text mineralText; 
	[SerializeField] Text goldText; 
    [SerializeField] ItemSlotUI[] slots;
	[SerializeField] ItemDescription description;

	private void Start()
	{
		// ������ ���Կ� ������ ������ �˷��ִ� ��������Ʈ ���
		foreach(ItemSlotUI slot in slots)
		{
			slot.onEnterEvent += ShowItemInfo;
			slot.onExitEvent += OffItemInfo;
		}
	}

	// ��ȭui ������Ʈ
	public void UpdateUI(int mineral, int gold)
	{
		mineralText.text = mineral.ToString();
		goldText.text = gold.ToString();
	}

	// ������ ���� ����
	public void SetupItemSlot(int index, Item item)
	{
		slots[index].Setup(item);
	}

	// ������ ���� ������Ʈ
	public void UpdateItemSlot(int index)
	{
		slots[index].UpdateUI();
	}

	// ������ ������ ������
	public void ShowItemInfo(ItemSlotUI slot)
	{
		description.gameObject.SetActive(true);
		description.transform.position = slot.transform.position;
		description.SetItem(slot.item);
	}

	// ������ ���� ����
	public void OffItemInfo(ItemSlotUI slot)
	{
		description.gameObject.SetActive(false);
	}
}
