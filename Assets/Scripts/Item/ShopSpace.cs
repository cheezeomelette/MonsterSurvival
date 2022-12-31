using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

// 상점에 들어가기위한 공간
public class ShopSpace : MonoBehaviour,IInputManager
{
    [SerializeField] Shop shop;
	WeaponManager weaponManager;

	// IInputManager 변수
	public string IName { get ; set ; }
	const string scriptName = "ShopSpace";

	private void Start()
	{
		IName = scriptName;
	}

	// 상점공간에 들어가면 자동으로 상점이 켜진다
	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("Player"))
		{
			PhotonView pv = other.gameObject.GetComponent<PhotonView>();
			// 자신의 플레이어가 들어갔을 때만 상점을 켠다
			if (pv.IsMine)
			{
				weaponManager = other.GetComponent<WeaponManager>();
				OpenShop();
			}
		}
	}
	// 상점 공간에서 나가면 상점이 꺼진다
	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject.CompareTag("Player"))
		{
			PhotonView pv = other.gameObject.GetComponent<PhotonView>();
			if (pv.IsMine)
			{
				weaponManager = other.GetComponent<WeaponManager>();
				CloseShop();
			}
		}
	}
	// 상점 켜기 함수
	public void OpenShop()
	{
		// 마우스 커서 활성화
		Cursor.lockState = CursorLockMode.Confined;
		Cursor.visible = true;

		// 상점 보여주기
		shop.gameObject.SetActive(true);
		// 상점에 들어온 플레이어의 WeaponManager를 Shop에 전달
		shop.SetManager(weaponManager);

		// IInputManager의 스택에 상점 추가
		IInputManager.nameStack.Push(IName);
	}
	// 상점 끄기 함수
	private void CloseShop()
	{
		// 마우스 커서 비활성화
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;

		// 상점끄기
		shop.gameObject.SetActive(false);
		// IInputManager의 스택에서 제거
		IInputManager.nameStack.Pop();

		// 설명창 비활성화
		WeaponDescription.Instance.gameObject.SetActive(false);
		ItemDescription.Instance.gameObject.SetActive(false);
	}

	// IInputManager의 함수
	public bool CanInput()
	{
		return IInputManager.nameStack.Peek() == IName;
	}
}
