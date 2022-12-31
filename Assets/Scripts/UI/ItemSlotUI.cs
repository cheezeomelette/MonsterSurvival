using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// �κ��丮�� ������ ����
public class ItemSlotUI : MonoBehaviour
{
    [SerializeField] Image itemImage;
	[SerializeField] Text itemCountText;
	[SerializeField] protected AudioSource source;

	[HideInInspector]
    public Item item;

	// ��������Ʈ�� ������ ����â ����
	public delegate void SlotDelegate(ItemSlotUI slot);
	public event SlotDelegate onEnterEvent;
	public event SlotDelegate onExitEvent;

	private void Start()
	{
		itemCountText.gameObject.SetActive(false);
	}

	// ������ ����
	public void Setup(Item item)
    {
        this.item = item;
		itemCountText.gameObject.SetActive(true);
		itemCountText.text = item.count.ToString();
		itemImage.sprite = item.itemData.sprite;
    }

	// ������ ���� ������Ʈ
	public void UpdateUI()
	{
		// �������� ���� �� ui���� ������ ����
		if(item.count <= 0)
		{
			item = null;
			itemImage.sprite = null;
			itemCountText.gameObject.SetActive(false);
			return;
		}
		// ���� ������Ʈ
		itemCountText.text = item.count.ToString();
	}

	// �����Ͱ� ������ �� �Լ�
	public void OnPointerEnter()
	{
		if (item != null)
			onEnterEvent?.Invoke(this);
		source.Play();
	}

	// ������ ������ �� �Լ�
	public void OnPointerExit()
	{
		if (item != null)
			onExitEvent?.Invoke(this);
	}
}
