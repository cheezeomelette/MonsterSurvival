using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 들고있는 무기
public class WeaponSlotUI : MonoBehaviour
{
    [SerializeField] Image gunImage;
	[SerializeField] protected AudioSource source;
	[SerializeField] int index;
	[HideInInspector]
    public Gun gun;

    public delegate void SlotDelegate(WeaponSlotUI slot);
    public event SlotDelegate onEnterEvent;
    public event SlotDelegate onExitEvent;
	public event SlotDelegate onClickEvent;
	private void Awake()
	{
		source = GetComponent<AudioSource>();
	}
	public void Setup(Gun gun)
	{
        this.gun = gun;
        gunImage.sprite = gun.weaponData.sprite;
	}
	public void DropGun()
	{
		gunImage.sprite = null;
		gun = null;
	}
	public void OnPointerEnter()
	{
		if (gun != null)
			onEnterEvent?.Invoke(this);
		source.Play();
	}
	public void OnPointerExit()
	{
		if (gun != null)
			onExitEvent?.Invoke(this);
	}
	public virtual void OnPointerClick()
	{
		if (gun != null)
			onClickEvent?.Invoke(this);
	}
}
