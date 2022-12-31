using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopItemSlot : ItemSlotUI
{
	[SerializeField] AudioClip buyClip;

	public delegate void OnClickDelegate(Item item);
	public event OnClickDelegate onclickEvent;

	// 클릭시 아이템 판매 델리게이트 실행
	public void OnPointerClick()
	{
		onclickEvent?.Invoke(item);
		source.PlayOneShot(buyClip);
	}
}
