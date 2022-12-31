using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

// 상호작용을 위한 인터페이스
public interface IInteraction
{
	// 상호작용 ui를 띄울 위치
	Vector3 Position { get; }
	// 상호작용 함수
	public void OnInteraction();
}

// 미네랄 클래스
public class Mineral : MonoBehaviourPun, IInteraction
{
	// 상호작용 ui 위치
	public Vector3 Position => transform.position + Vector3.up * 3f;

	// 상호작용 함수
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
		// 모든 클라이언트가 미네랄 오브젝트를 끔
		gameObject.SetActive(false);
	}
	
	[PunRPC]
	private void OnMineral()
	{
		// 모든 클라이언트가 미네랄 오브젝트를 켬
		gameObject.SetActive(true);
	}
}
