using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;

// 포톤네트워크에 접속한 방 관리
public class RoomManager : MonoBehaviourPunCallbacks
{
    [SerializeField] Transform userListTransform;
    [SerializeField] RoomUserSlot userPrefab;

	private void Start()
	{
		if(PhotonNetwork.InRoom)
			UpdateUserList();
	}

	// 방에 입장 시
	public void EnterUser(string userName)
	{
		// 자신의 객체 생성
		PhotonNetwork.Instantiate(userPrefab.name, Vector3.zero, Quaternion.identity);
	}

	// 유저 리스트 업데이트
	private void UpdateUserList()
	{
		// layout컴포넌트로 정렬된 userListTransform에 있던 유저ui 삭제
		foreach (Transform lastSlot in userListTransform)
		{
			Destroy(lastSlot.gameObject) ;				
		}	

		// 방에 접속되어 있는 플레이어들을 가져옴
		Dictionary<int, Player> players = PhotonNetwork.CurrentRoom.Players;

		// 새로운 유저 목록 업데이트
		foreach (Player player in players.Values)
		{
			RoomUserSlot user = Instantiate(userPrefab, userListTransform);		
			user.Setup(player.NickName);
		}
	}

	// 자신이 방에 입장하면 리스트 업데이트
	public override void OnJoinedRoom()
	{
		UpdateUserList();
	}

	// 방에 유저가 입장할 때마다 리스트 업데이트
	public override void OnPlayerEnteredRoom(Player newPlayer)
	{
		UpdateUserList();
		Debug.Log("enter SomeOne");
	}

	// 방에서 유저가 나갈 때 리스트 업데이트
	public override void OnPlayerLeftRoom(Player otherPlayer)
	{
		UpdateUserList();
	}
}
