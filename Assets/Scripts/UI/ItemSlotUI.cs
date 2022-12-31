using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 인벤토리의 아이템 슬롯
public class ItemSlotUI : MonoBehaviour
{
    [SerializeField] Image itemImage;
	[SerializeField] Text itemCountText;
	[SerializeField] protected AudioSource source;

	[HideInInspector]
    public Item item;

	// 델리게이트로 아이템 설명창 관리
	public delegate void SlotDelegate(ItemSlotUI slot);
	public event SlotDelegate onEnterEvent;
	public event SlotDelegate onExitEvent;

	private void Start()
	{
		itemCountText.gameObject.SetActive(false);
	}

	// 아이템 셋팅
	public void Setup(Item item)
    {
        this.item = item;
		itemCountText.gameObject.SetActive(true);
		itemCountText.text = item.count.ToString();
		itemImage.sprite = item.itemData.sprite;
    }

	// 아이템 정보 업데이트
	public void UpdateUI()
	{
		// 아이템이 없을 때 ui에서 아이템 제거
		if(item.count <= 0)
		{
			item = null;
			itemImage.sprite = null;
			itemCountText.gameObject.SetActive(false);
			return;
		}
		// 갯수 업데이트
		itemCountText.text = item.count.ToString();
	}

	// 포인터가 들어왔을 때 함수
	public void OnPointerEnter()
	{
		if (item != null)
			onEnterEvent?.Invoke(this);
		source.Play();
	}

	// 포인터 나갔을 때 함수
	public void OnPointerExit()
	{
		if (item != null)
			onExitEvent?.Invoke(this);
	}
}
