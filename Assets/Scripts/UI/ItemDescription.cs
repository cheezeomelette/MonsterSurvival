using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 아이템 설명창
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

    // 아이템 데이터에 있는 정보를 표기
	public void SetItem(Item item)
	{
        itemImage.sprite = item.itemData.sprite;
        itemName.text = item.itemData.itemName;
        priceText.text = item.itemData.price.ToString();
        itemDescription.text = item.itemData.description;
    }
}
