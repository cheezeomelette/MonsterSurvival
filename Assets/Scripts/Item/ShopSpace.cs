using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

// ������ �������� ����
public class ShopSpace : MonoBehaviour,IInputManager
{
    [SerializeField] Shop shop;
	WeaponManager weaponManager;

	// IInputManager ����
	public string IName { get ; set ; }
	const string scriptName = "ShopSpace";

	private void Start()
	{
		IName = scriptName;
	}

	// ���������� ���� �ڵ����� ������ ������
	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("Player"))
		{
			PhotonView pv = other.gameObject.GetComponent<PhotonView>();
			// �ڽ��� �÷��̾ ���� ���� ������ �Ҵ�
			if (pv.IsMine)
			{
				weaponManager = other.GetComponent<WeaponManager>();
				OpenShop();
			}
		}
	}
	// ���� �������� ������ ������ ������
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
	// ���� �ѱ� �Լ�
	public void OpenShop()
	{
		// ���콺 Ŀ�� Ȱ��ȭ
		Cursor.lockState = CursorLockMode.Confined;
		Cursor.visible = true;

		// ���� �����ֱ�
		shop.gameObject.SetActive(true);
		// ������ ���� �÷��̾��� WeaponManager�� Shop�� ����
		shop.SetManager(weaponManager);

		// IInputManager�� ���ÿ� ���� �߰�
		IInputManager.nameStack.Push(IName);
	}
	// ���� ���� �Լ�
	private void CloseShop()
	{
		// ���콺 Ŀ�� ��Ȱ��ȭ
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;

		// ��������
		shop.gameObject.SetActive(false);
		// IInputManager�� ���ÿ��� ����
		IInputManager.nameStack.Pop();

		// ����â ��Ȱ��ȭ
		WeaponDescription.Instance.gameObject.SetActive(false);
		ItemDescription.Instance.gameObject.SetActive(false);
	}

	// IInputManager�� �Լ�
	public bool CanInput()
	{
		return IInputManager.nameStack.Peek() == IName;
	}
}
