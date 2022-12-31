using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 여러가지 아이템을 만들기 위한 추상클래스
public abstract class Item : MonoBehaviour
{
	// scriptableobject로 만들어둔 아이템 데이터
	public ItemData itemData;
	// 아이템 갯수
	public int count;

	// 아이템별로 다른 효과를 내기위한 함수
	public abstract void UseItem(PlayerStatus status);
}
