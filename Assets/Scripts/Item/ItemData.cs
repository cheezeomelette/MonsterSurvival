using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 아이템 데이터
[CreateAssetMenu(fileName ="ItemData", menuName = "ItemData")]
public class ItemData : ScriptableObject
{
    // 아이템 이미지
    public Sprite sprite;
    public string itemName;
    public string description;
    public int price;
}
