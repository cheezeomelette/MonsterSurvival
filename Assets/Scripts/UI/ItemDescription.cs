using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// ������ ����â
public class ItemDescription : Singleton<ItemDescription>
{
    [SerializeField] Image itemImage;
    [SerializeField] Text itemName;
    [SerializeField] Text priceText;
    [SerializeField] Text itemDescription;

	private void Start()
	{
        gameObject.SetActive(false);
	}

    // ������ �����Ϳ� �ִ� ������ ǥ��
	public void SetItem(Item item)
	{
        itemImage.sprite = item.itemData.sprite;
        itemName.text = item.itemData.itemName;
        priceText.text = item.itemData.price.ToString();
        itemDescription.text = item.itemData.description;
    }
}
