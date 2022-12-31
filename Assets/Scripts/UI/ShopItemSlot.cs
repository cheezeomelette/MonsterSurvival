using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopItemSlot : ItemSlotUI
{
	[SerializeField] AudioClip buyClip;

	public delegate void OnClickDelegate(Item item);
	public event OnClickDelegate onclickEvent;

	// Ŭ���� ������ �Ǹ� ��������Ʈ ����
	public void OnPointerClick()
	{
		onclickEvent?.Invoke(item);
		source.PlayOneShot(buyClip);
	}
}
