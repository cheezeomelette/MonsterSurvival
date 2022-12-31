using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 상호작용UI 관리
public class InteractionUI : Singleton<InteractionUI>
{
	[SerializeField] GameObject panel;
	Camera cam;

	private void Start()
	{
		cam = Camera.main;
		panel.SetActive(false);
	}

	// 상호작용UI 위치를 업데이트
	public void UpdateUI(IInteraction target)
	{
		panel.SetActive(true);
		Vector3 screenPoint = cam.WorldToScreenPoint(target.Position);

		// 뒤를 돌았을 때 보이는 UI를 화면밖으로 빼서 안보이게 만듬
		if(screenPoint.z < 0f)
		{
			screenPoint = new Vector3(-100, -100, 0);
		}
		transform.position = screenPoint;
	}

	// UI끄기
	public void OffUI()
	{
		panel.SetActive(false);
	}
}
