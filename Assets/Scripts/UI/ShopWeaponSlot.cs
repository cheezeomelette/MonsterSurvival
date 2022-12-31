using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopWeaponSlot : WeaponSlotUI
{
	[SerializeField] AudioClip buyClip;

	public delegate void OnClickDelegate(Gun gun);
	public event OnClickDelegate onclickEvent;

	public override void OnPointerClick()
	{
		onclickEvent?.Invoke(gun);
		source.PlayOneShot(buyClip);
	}
}
