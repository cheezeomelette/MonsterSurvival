using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 총알이 얼마나 남았는지 알려주는 UI 오브젝트 풀링으로 관리
public class BulletUI : MonoBehaviour
{
	// 총알 역할을 할 image의 RectTransform
	[SerializeField] RectTransform bullet;
	// 사용한 총알을 놓을 비활성화 Transform
	Transform disabledTransform;
	new Transform transform;

	// 남은 총알스택
	Stack<RectTransform> stack;
	// 사용한 총알 스택
	Stack<RectTransform> usedStack;
	RectTransform rect;
	int maxBullet;

	private void Awake()
	{
		// 캐싱
		transform = base.transform;
		rect = GetComponent<RectTransform>();

		// 스택 생성
		stack = new Stack<RectTransform>();
		usedStack = new Stack<RectTransform>();

		// 사용한 총알을 담아둘 BulletPool 생성
		disabledTransform = new GameObject("BulletPool").transform;
		disabledTransform.SetParent(transform);
		disabledTransform.gameObject.SetActive(false);
	}

	// 총교체 시 현재총알 업데이트
	public void ChangeWeapon(int current,int maxCount)	
	{
		maxBullet = maxCount;
		// 최대총알수에 맞게 총알UI 크기 조정
		float size = (rect.sizeDelta.x / maxCount) - 3;
		bullet.sizeDelta = new Vector2(size, rect.sizeDelta.y);

		// 사용한 총알에서 총알 가져옴
		while(usedStack.Count > 0)
		{
			GetBullet();
		}

		// 스택에 들어있는 총알 사이즈 조절
		foreach (RectTransform rectTransform in stack)
		{
			rectTransform.sizeDelta = new Vector2(size, rect.sizeDelta.y);
		}

		// 총알이 모자라면 새로운 총알 생성
		if (maxCount > stack.Count)
		{
			while (stack.Count < maxCount)
				AddBullet();
		}
		// 총알이 남으면 사용한 총알로 옮김
		if(maxCount < stack.Count)
		{
			while(maxCount < stack.Count)
				ReturnBullet();
		}
		// 총알이 남으면 사용한 총알로 옮김
		while (current < stack.Count)
			ReturnBullet();
	}

	// 새로운 총알 생성
	private void AddBullet()
	{
		stack.Push(Instantiate(bullet, transform));
	}
	// 사용한 총알에서 가져옴
	private void GetBullet()
	{
		RectTransform bullet = usedStack.Pop();
		bullet.transform.SetParent(transform);
		stack.Push(bullet);
	}
	// 사용한 총알로 반납
	private void ReturnBullet()
	{
		RectTransform bullet = stack.Pop();
		bullet.transform.SetParent(disabledTransform);
		usedStack.Push(bullet);
	}
	// 총알 발사 시 ui업데이트
	public void ShotBullet()
	{
		ReturnBullet();
	}
	// 재장전시 최대개수만큼 스택 충전
	public void Reload()
	{
		while (stack.Count < maxBullet)
			GetBullet();
	}
}
