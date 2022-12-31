using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;

// 게임 로비와 시작을 관리함
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

	// 코루틴을 중간에 종료하기 위한 변수
	Coroutine UICoroutine;

	private void Awake()
	{
		// 룸에 접속한 클라이언트들과 씬 이동을 동기화한다
		PhotonNetwork.AutomaticallySyncScene = true;
	}
	private void Start()
	{
		// 현재 진행상태
		progressBack.gameObject.SetActive(false);
		// 유저리스트
		userList.SetActive(false);
		userListText.SetActive(false);
		if(PhotonNetwork.InRoom)
		{
			OnJoinedRoom();
			return;
		}
		UICoroutine = StartCoroutine(CloseUI(pannelTransform));
		Debug.Log("isRoom? " + PhotonNetwork.InRoom);
		// 연결되어있지 않다면
		if (!PhotonNetwork.IsConnected)
		{
			PhotonNetwork.GameVersion = gameVersion;
			// 마스터서버에 연결
			PhotonNetwork.ConnectUsingSettings();
		}
	}

	// 룸에 연결
	public void Connect()
	{
		progressBack.gameObject.SetActive(true);

		// 연결되어 있다면 방에 입장
		if (PhotonNetwork.IsConnected)
		{
			Debug.Log("마스터서버에 이미 연결되어서 랜덤방 입장 시도");
			PhotonNetwork.JoinRandomRoom();
		}
		else
		{
			PhotonNetwork.GameVersion = gameVersion;
			// 마스터 서버에 연결	
			PhotonNetwork.ConnectUsingSettings();		
		}

		// 버튼 애니메이션 실행
		if (UICoroutine != null)
			StopCoroutine(UICoroutine);
		UICoroutine = StartCoroutine(CloseUI(pannelTransform));
	}

	// 게임시작 버튼
	public void StartGame()
	{
		// 호스트만 실행가능
		if (PhotonNetwork.IsMasterClient)
		{
			// 방을 닫아서 게임 시작 후 난입을 막음
			PhotonNetwork.CurrentRoom.IsOpen = false;
			// 씬이동
			PhotonNetwork.LoadLevel("InGame");
		}
	}
	// 방에 접속해 있다면 방을 나가고 아니라면 게임종료
	public void QuitApp()
	{
		Debug.Log(PhotonNetwork.CurrentRoom);
		if (PhotonNetwork.CurrentRoom != null)
			PhotonNetwork.LeaveRoom();
		else
			Application.Quit();
	}
	
	// 애니메이션 역할을 하는 코루틴
	IEnumerator OpenUI(RectTransform target)
	{
		// 왼쪽에서 오른쪽으로 버튼 등장
		while (target.anchoredPosition.x < 0)
		{
			target.anchoredPosition = Vector3.MoveTowards(target.anchoredPosition, Vector3.zero, 10000*Time.deltaTime);
			yield return null;
		}
		UICoroutine = null;
	}
	// 애니메이션 역할을 하는 코루틴
	IEnumerator CloseUI(RectTransform target)
	{
		// 오른쪽에서 왼쪽으로 버튼 퇴장
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
			progressLabel.text = "연결 성공";
		}
		else
		{
			joinButton.SetActive(false);
			playButton.SetActive(false);
			progressLabel.text = "연결 성공";
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
