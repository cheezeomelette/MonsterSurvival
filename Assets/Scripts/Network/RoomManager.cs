using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;

// �����Ʈ��ũ�� ������ �� ����
public class RoomManager : MonoBehaviourPunCallbacks
{
    [SerializeField] Transform userListTransform;
    [SerializeField] RoomUserSlot userPrefab;

	private void Start()
	{
		if(PhotonNetwork.InRoom)
			UpdateUserList();
	}

	// �濡 ���� ��
	public void EnterUser(string userName)
	{
		// �ڽ��� ��ü ����
		PhotonNetwork.Instantiate(userPrefab.name, Vector3.zero, Quaternion.identity);
	}

	// ���� ����Ʈ ������Ʈ
	private void UpdateUserList()
	{
		// layout������Ʈ�� ���ĵ� userListTransform�� �ִ� ����ui ����
		foreach (Transform lastSlot in userListTransform)
		{
			Destroy(lastSlot.gameObject) ;				
		}	

		// �濡 ���ӵǾ� �ִ� �÷��̾���� ������
		Dictionary<int, Player> players = PhotonNetwork.CurrentRoom.Players;

		// ���ο� ���� ��� ������Ʈ
		foreach (Player player in players.Values)
		{
			RoomUserSlot user = Instantiate(userPrefab, userListTransform);		
			user.Setup(player.NickName);
		}
	}

	// �ڽ��� �濡 �����ϸ� ����Ʈ ������Ʈ
	public override void OnJoinedRoom()
	{
		UpdateUserList();
	}

	// �濡 ������ ������ ������ ����Ʈ ������Ʈ
	public override void OnPlayerEnteredRoom(Player newPlayer)
	{
		UpdateUserList();
		Debug.Log("enter SomeOne");
	}

	// �濡�� ������ ���� �� ����Ʈ ������Ʈ
	public override void OnPlayerLeftRoom(Player otherPlayer)
	{
		UpdateUserList();
	}
}
