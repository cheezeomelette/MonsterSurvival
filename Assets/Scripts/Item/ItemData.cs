using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ������ ������
[CreateAssetMenu(fileName ="ItemData", menuName = "ItemData")]
public class ItemData : ScriptableObject
{
    // ������ �̹���
    public Sprite sprite;
    public string itemName;
    public string description;
    public int price;
}
