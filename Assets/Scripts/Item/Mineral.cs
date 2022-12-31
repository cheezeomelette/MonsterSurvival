using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

// ��ȣ�ۿ��� ���� �������̽�
public interface IInteraction
{
	// ��ȣ�ۿ� ui�� ��� ��ġ
	Vector3 Position { get; }
	// ��ȣ�ۿ� �Լ�
	public void OnInteraction();
}

// �̳׶� Ŭ����
public class Mineral : MonoBehaviourPun, IInteraction
{
	// ��ȣ�ۿ� ui ��ġ
	public Vector3 Position => transform.position + Vector3.up * 3f;

	// ��ȣ�ۿ� �Լ�
	public void OnInteraction()
	{
		Inventory.Instance.EarnMineral();
		photonView.RPC(nameof(OffMineral), RpcTarget.All);
	}

	private void OnEnable()
	{
		photonView.RPC(nameof(OnMineral), RpcTarget.All);
		
	}
	[PunRPC]
	private void OffMineral()
	{
		// ��� Ŭ���̾�Ʈ�� �̳׶� ������Ʈ�� ��
		gameObject.SetActive(false);
	}
	
	[PunRPC]
	private void OnMineral()
	{
		// ��� Ŭ���̾�Ʈ�� �̳׶� ������Ʈ�� ��
		gameObject.SetActive(true);
	}
}
