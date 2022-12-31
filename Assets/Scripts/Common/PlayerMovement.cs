using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

// 플레이어의 움직임을 담당
public class PlayerMovement : MonoBehaviourPun
{
	[SerializeField] float moveSpeed;
	[SerializeField] float runSpeed;
	[SerializeField] float jumpHeight;

	[Range(1.0f, 4.0f)]
	[SerializeField] float gravityScale;        // 중력 배율.

	[SerializeField] float groundRadius;
	[SerializeField] LayerMask groundMask;

	// 캐릭터 컨트롤러를 사용해서 움직인다
	CharacterController controller;
	PlayerStatus status;

	Vector3 velocity;
	float GRAVITY => -9.8f * gravityScale;
	bool isGrounded;

	private void Start()
	{
		// 캐싱
		controller = GetComponent<CharacterController>();
		status = GetComponent<PlayerStatus>();
	}
	private void Update()
	{
		// 나의 플레이어만 조종 가능
		if (!photonView.IsMine)
			return;

		CheckGround();
		Movement();
		Jump();
		Gravity();
	}

	// 바닥에 닿아있는지 체크
	void CheckGround()
	{
		isGrounded = Physics.CheckSphere(transform.position + (Vector3.up * 0.4f), groundRadius, groundMask);
	}

	// 앞뒤좌우 움직임
	void Movement()
	{
		float x = Input.GetAxisRaw("Horizontal");
		float z = Input.GetAxisRaw("Vertical");

		// stemina가 충분하고 shift를 누르면 달리기 가능
		bool isRun = Input.GetKey(KeyCode.LeftShift) && status.stemina > 1;
		
		// 방향만 나타내기 위해 단위벡터로 변환
		Vector3 direction = ((transform.right * x) + (transform.forward * z).normalized);
		controller.Move(direction * (isRun ? runSpeed : moveSpeed) * GameManager.gameDeltaTime);

		// 달리기 시 스테미너 사용
		if (isRun && direction != Vector3.zero)
			status.UseStemina(10 * Time.deltaTime);
	}

	// 점프
	void Jump()
	{
		if (isGrounded && Input.GetKeyDown(KeyCode.Space))
		{
			// 높이에 따른 속도 공식
			velocity.y = Mathf.Sqrt(jumpHeight * -2f * GRAVITY);
		}
	}
	// 중력 적용
	void Gravity()
	{
		// 바닥에 닿아 있을 때 깃털처럼 떨어짐 방지
		if (isGrounded && velocity.y < 0.0f)
			velocity.y = -2f;

		velocity.y += GRAVITY * GameManager.gameDeltaTime;         // 중력 가속도.
		controller.Move(velocity * GameManager.gameDeltaTime);     // 아래쪽으로 이동.
	}

	// 바닥 범위 체크를 위한 기즈모
	private void OnDrawGizmos()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawSphere(transform.position + (Vector3.up * 0.4f), groundRadius);
	}
}
