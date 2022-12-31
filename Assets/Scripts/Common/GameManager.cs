using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using Photon.Pun;
using Photon.Realtime;

// 게임의 흐름 관리
public class GameManager : MonoBehaviourPunCallbacks, IInputManager
{
	// 게임 종료 시 움직임을 멈추기 위해 바꿀 수 있는 Time 생성
	public static float gameDeltaTime => Time.deltaTime * magnification;
	// 시간을 조절을 위한 변수
	public static float magnification;

	[SerializeField] GameObject playerPrefab;
	[SerializeField] GameObject optionWindow;
	[SerializeField] GameObject mineralPrefab;
	[SerializeField] Transform playerSpawnPoint;
	[SerializeField] Transform mineralsTransform;
	[SerializeField] Timer timer;
	[SerializeField] Option option;
	[SerializeField] Score score;

	Mineral[] minerals;
	// 광석 생성 간격
	int interval = 30;
	int index = 0;
	bool isEnd = false;
	// 입력매니저
	public string IName { get; set; }
	const string scriptName = "GameManager";
	const string scoreName = "Score";

	private void Start()
	{
		Debug.Log("Sync" + PhotonNetwork.AutomaticallySyncScene);
		IName = scriptName;
		// 기존 Time과 동일하게 시간 적용
		magnification = 1f;
		if (playerPrefab == null)
		{
			Debug.Log("프리팹 없음");
			return;
		}
		else
		{
			// 씬이 로드되었을 때 PlayerControl의 static변수인 player이 생성되지 않았다면
			if (PlayerControl.player == null)
			{
				// 플레이어 생성
				GameObject playerObject = PhotonNetwork.Instantiate(playerPrefab.name, playerSpawnPoint.position, Quaternion.identity, 0);
				PlayerControl player = playerObject.GetComponent<PlayerControl>();
				// 옵션창의 감도와 볼륨 연결
				option.Setup(player);
			}
		}

		// 호스트가 광석 관리
		if (PhotonNetwork.IsMasterClient)
		{
			List<Mineral> list = new();
			// 광석이 생성될 Transform들을 씬에 배치해 두고 하나씩 가져옴
			foreach (Transform mt in mineralsTransform)
			{
				// 위치에 생성
				GameObject m = PhotonNetwork.Instantiate(mineralPrefab.name, mt.position, mt.rotation);
				Mineral mineral = m.GetComponent<Mineral>();
				// 광석을 관리하기 위한 리스트 생성
				list.Add(mineral);
			}
			minerals = list.ToArray();
		}
		optionWindow.SetActive(false);
	}

	private void Update()
	{
		// 옵션창 열기
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			optionWindow.SetActive(!optionWindow.activeSelf);
			// 커서 활성화
			Cursor.lockState = optionWindow.activeSelf ? CursorLockMode.Confined : CursorLockMode.Locked;
			Cursor.visible = optionWindow.activeSelf;
			// 총기 입력 막기
			if (optionWindow.activeSelf)
				IInputManager.nameStack.Push(IName);
			else
				IInputManager.nameStack.Pop();
		}

		// 게임 종료 알림
		if (timer.time > 300f && !isEnd)
		{
			isEnd = true;
			// 시간 배율을 0으로 바꿔서 멈춘다
			magnification = 0f;
			// 결과창 오픈
			score.ShowResult();
			// 입력 막기
			IInputManager.nameStack.Push(scoreName);
			// 커서 활성화
			Cursor.lockState = score.Panel.activeSelf ? CursorLockMode.Confined : CursorLockMode.Locked;
			Cursor.visible = score.Panel.activeSelf;
			return;
		}
		// 호스트가 광석 관리
		if (!PhotonNetwork.IsMasterClient)
			return;

		// interval마다 광석 재생성
		if (timer.time > interval * index)
		{
			foreach (Mineral mineral in minerals)
				mineral.gameObject.SetActive(true);
			index++;
		}
	}
	// 재도전
	public void Retry()
	{
		if (PhotonNetwork.IsMasterClient)
		{
			PhotonNetwork.LoadLevel("Network");
		}
	}
	// 게임종료
	public void QuitGame()
	{
		Application.Quit();
	}

	public bool CanInput()
	{
		return IInputManager.nameStack.Peek() == IName;
	}
}
