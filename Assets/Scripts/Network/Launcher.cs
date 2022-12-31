using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;

// ���� �κ�� ������ ������
public class Launcher : MonoBehaviourPunCallbacks
{
	[SerializeField] byte maxPlayersPerRoom = 4;

	[SerializeField] private Text progressLabel;
	[SerializeField] private Image progressBack;
	[SerializeField] private GameObject inputField;

	[SerializeField] GameObject joinButton;
	[SerializeField] GameObject playButton;

	[SerializeField] RectTransform pannelTransform;
	[SerializeField] GameObject userList;
	[SerializeField] GameObject userListText;

    string gameVersion = "1.0";

	// �ڷ�ƾ�� �߰��� �����ϱ� ���� ����
	Coroutine UICoroutine;

	private void Awake()
	{
		// �뿡 ������ Ŭ���̾�Ʈ��� �� �̵��� ����ȭ�Ѵ�
		PhotonNetwork.AutomaticallySyncScene = true;
	}
	private void Start()
	{
		// ���� �������
		progressBack.gameObject.SetActive(false);
		// ��������Ʈ
		userList.SetActive(false);
		userListText.SetActive(false);
		if(PhotonNetwork.InRoom)
		{
			OnJoinedRoom();
			return;
		}
		UICoroutine = StartCoroutine(CloseUI(pannelTransform));
		Debug.Log("isRoom? " + PhotonNetwork.InRoom);
		// ����Ǿ����� �ʴٸ�
		if (!PhotonNetwork.IsConnected)
		{
			PhotonNetwork.GameVersion = gameVersion;
			// �����ͼ����� ����
			PhotonNetwork.ConnectUsingSettings();
		}
	}

	// �뿡 ����
	public void Connect()
	{
		progressBack.gameObject.SetActive(true);

		// ����Ǿ� �ִٸ� �濡 ����
		if (PhotonNetwork.IsConnected)
		{
			Debug.Log("�����ͼ����� �̹� ����Ǿ ������ ���� �õ�");
			PhotonNetwork.JoinRandomRoom();
		}
		else
		{
			PhotonNetwork.GameVersion = gameVersion;
			// ������ ������ ����	
			PhotonNetwork.ConnectUsingSettings();		
		}

		// ��ư �ִϸ��̼� ����
		if (UICoroutine != null)
			StopCoroutine(UICoroutine);
		UICoroutine = StartCoroutine(CloseUI(pannelTransform));
	}

	// ���ӽ��� ��ư
	public void StartGame()
	{
		// ȣ��Ʈ�� ���డ��
		if (PhotonNetwork.IsMasterClient)
		{
			// ���� �ݾƼ� ���� ���� �� ������ ����
			PhotonNetwork.CurrentRoom.IsOpen = false;
			// ���̵�
			PhotonNetwork.LoadLevel("InGame");
		}
	}
	// �濡 ������ �ִٸ� ���� ������ �ƴ϶�� ��������
	public void QuitApp()
	{
		Debug.Log(PhotonNetwork.CurrentRoom);
		if (PhotonNetwork.CurrentRoom != null)
			PhotonNetwork.LeaveRoom();
		else
			Application.Quit();
	}
	
	// �ִϸ��̼� ������ �ϴ� �ڷ�ƾ
	IEnumerator OpenUI(RectTransform target)
	{
		// ���ʿ��� ���������� ��ư ����
		while (target.anchoredPosition.x < 0)
		{
			target.anchoredPosition = Vector3.MoveTowards(target.anchoredPosition, Vector3.zero, 10000*Time.deltaTime);
			yield return null;
		}
		UICoroutine = null;
	}
	// �ִϸ��̼� ������ �ϴ� �ڷ�ƾ
	IEnumerator CloseUI(RectTransform target)
	{
		// �����ʿ��� �������� ��ư ����
		while (target.anchoredPosition.x > -Screen.width)
		{
			target.anchoredPosition = Vector3.MoveTowards(target.anchoredPosition, new Vector3(-Screen.width,0,0), 5000 * Time.deltaTime);
			yield return null;
		}
		UICoroutine = null;
	}

	#region PhotonCallback
	public override void OnConnectedToMaster()
	{
		if (UICoroutine != null)
			StopCoroutine(UICoroutine);
		UICoroutine = StartCoroutine(OpenUI(pannelTransform));

		Debug.Log("connect master");
	}
	public override void OnDisconnected(DisconnectCause cause)
	{
		progressLabel.gameObject.SetActive(false);
		userListText.SetActive(false);
		userList.SetActive(false);
	}

	public override void OnJoinRandomFailed(short returnCode, string message)
	{
		RoomOptions roomOptions = new RoomOptions { MaxPlayers = maxPlayersPerRoom };
		PhotonNetwork.CreateRoom(null, roomOptions);
	}
	public override void OnJoinedRoom()
	{
		if (UICoroutine != null)
			StopCoroutine(UICoroutine);
		UICoroutine = StartCoroutine(OpenUI(pannelTransform));

		userList.SetActive(true);
		userListText.SetActive(true);

		if(PhotonNetwork.IsMasterClient)
		{
			joinButton.SetActive(false);
			playButton.SetActive(true);
			progressLabel.text = "���� ����";
		}
		else
		{
			joinButton.SetActive(false);
			playButton.SetActive(false);
			progressLabel.text = "���� ����";
		}
	}
	public override void OnLeftRoom()
	{
		progressBack.gameObject.SetActive(false);
		joinButton.SetActive(true);
		playButton.SetActive(false);
		userListText.SetActive(false);
		userList.SetActive(false);
	}
	#endregion
}
