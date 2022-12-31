using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

using Photon.Pun;

// 플레이어의 움직임을 제외한 조작
public class PlayerControl : MonoBehaviourPun, IInputManager
{
	// 포톤서버에서 자신의 플레이어를 생성하기 위한 변수
	public static GameObject player;	

	[SerializeField] WeaponManager weaponManager;
	[SerializeField] Transform verticalPivot;
	[SerializeField] Transform weaponPivot;
	[SerializeField] Camera minimapCam;
	[SerializeField] float moveSpeed;
	[SerializeField] float interactionRadius;

	// 감도 조절을 위한 변수(감도배율)
	public float seneitivity = 1f;	

	private new Transform transform;
	// 카메라 회전 속도
	private float rotateSpeed = 300f;
	// 카메라 회전 시 최대각도를 설정하기 위한 현재각도
	private float currentVertical;
	// 줌을 했을 때 회전속도를 바꿔 주기위한 카메라 회전 속도
	private float currentRotateSpeed;

	// IInputManager 변수
	public string IName { get ; set; }
	const string scriptName = "PControl";

	private void Awake()
	{
		if (photonView.IsMine)
		{
			// 포톤서버를 통해 자신의 오브젝트가 생겼을 때 불림
			PlayerControl.player = this.gameObject;
		}
		else
			Destroy(minimapCam.gameObject);		// 미니맵이 플레이어마다 가지고있기 때문에 자신의 것 만 남기고 파괴한다

		IName = scriptName;
		IInputManager.nameStack.Push(IName);	// IInputManager의 스택에 플레이어 조작을 등록한다.
	}
	private void Start()
	{
		if (photonView.IsMine)
		{
			// 자신의 플레이어 오브젝트에 카메라를 고정하기 위한 작업
			Transform cam = Camera.main.transform;
			cam.position = verticalPivot.position;
			cam.rotation = verticalPivot.rotation;
			cam.SetParent(verticalPivot);
		}

		transform = base.transform;
		currentRotateSpeed = rotateSpeed;

		// 마우스를 감춘다
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
	}

	void Update()
	{
		// 여러개의 플레이어 오브젝트 중 자신의 것 이외의 오브젝트는 조종할 수 없게 한다
		if (!photonView.IsMine)
			return;
		CameraRotate();
		CheckInteraction();
		MouseClick();
		// 인벤토리의 총을 손에 든다
		if (Input.GetKeyDown(KeyCode.Alpha1))
		{
			weaponManager.SwapWeapon(0);
			currentRotateSpeed = rotateSpeed;
		}
		else if (Input.GetKeyDown(KeyCode.Alpha2))
		{
			weaponManager.SwapWeapon(1);
			currentRotateSpeed = rotateSpeed;
		}
		// 재장전
		if (Input.GetKeyDown(KeyCode.R))
			weaponManager.Reloading();
	}

	// 카메라 회전
	private void CameraRotate()
	{
		if (!CanInput())
			return;
		float x = -Input.GetAxis("Mouse Y") * GameManager.gameDeltaTime * currentRotateSpeed * seneitivity;
		float y = Input.GetAxis("Mouse X") * GameManager.gameDeltaTime * currentRotateSpeed * seneitivity;

		Vector3 dirHorizontal = new Vector3(0, y, 0);

		currentVertical += x;
		currentVertical = Mathf.Clamp(currentVertical, -90, 90);

		verticalPivot.localEulerAngles = new Vector3(currentVertical, 0f, 0f);

		transform.eulerAngles += dirHorizontal;
	}

	// 인터페이스 캐릭터 주변에서 IInteraction을 보유한 오브젝트를 탐색한다
	private void CheckInteraction()
	{
		Collider[] colliders = Physics.OverlapSphere(transform.position, interactionRadius);
		// colliders중 IInteraction을 갖고있는 것을 거리순으로 정렬한다.
		IInteraction[] interactions = colliders.
			Select(collider => collider.GetComponent<IInteraction>()).
			Where(c => c != null).
			OrderBy(x => Vector3.Distance(x.Position, transform.position)).ToArray();
		if (interactions.Count() <= 0)
		{
			InteractionUI.Instance.OffUI();
			return;
		}
		// 가장가까운 오브젝트의 상호작용 ui를 켠다.
		IInteraction findInteraction = interactions[0];
		InteractionUI.Instance.UpdateUI(findInteraction);
		// 상호작용
		if (Input.GetKeyDown(KeyCode.F))
			findInteraction.OnInteraction();
	}

	private void MouseClick()
	{
		// 입력이 가능한지 확인
		if (!CanInput())
			return;
		// 좌클릭 총 발사
		if (Input.GetMouseButton(0))
		{
			weaponManager.Shoot();
		}
		// 우클릭 줌인 & 줌아웃
		if (Input.GetMouseButtonDown(1))
		{
			if (weaponManager.currentWeapon == null)
				return;

			// 배율이 커질수록 회전 감도를 낮추기 위한 변수
			float magnitude = weaponManager.ZoomIn();
			if (weaponManager.currentWeapon.isAim)
				currentRotateSpeed = rotateSpeed * magnitude;
			else
				currentRotateSpeed = rotateSpeed;
		}
	}

	// IInputManager의 함수
	public bool CanInput()
	{
		return IInputManager.nameStack.Peek() == IName;
	}
}