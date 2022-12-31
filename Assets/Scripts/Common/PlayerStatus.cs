using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

// 플레이어의 상태정보
public class PlayerStatus : MonoBehaviourPun, IDamageable, IPunObservable
{
	// 몬스터가 타겟을 설정하기 위한 리스트
	public static List<Transform> playerTransformList = new();

	[SerializeField] private GameObject hpPrefab;

	public float power;
	public float attackRate;
	public float maxHp;
	public float hp;
	public float maxStemina;
	public float stemina;
	public bool dead;

	new Transform transform;

	PlayerControl playerControl;
	PlayerMovement playerMovement;
	PlayerHud playerHud;
	Score score;
	MyStat myStat;

	private void Awake()
	{
		// 컴포넌트 캐싱
		playerControl = GetComponent<PlayerControl>();
		playerMovement = GetComponent<PlayerMovement>();
		transform = base.transform;
	}

	private void Start()
	{
		if(photonView.IsMine)
		{
			// 인벤토리에 연결
			Inventory.Instance.status = this;
			// 자신의 플레이어와 UI를 연결하기 위해 캐싱
			playerHud = PlayerHud.Instance;
			score = Score.Instance;
			myStat = MyStat.Instance;
		}

		if (hpPrefab != null)
		{
			// hpBar 생성
			GameObject hpUI = Instantiate(hpPrefab);
			// 게임오브젝트의 SetTarget함수 실행
			hpUI.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
		}
		else
		{
			// hp프리팹이 없을 때 디버깅
			Debug.LogWarning("<Color=Red><a>Missing</a></Color> PlayerUiPrefab reference on player Prefab.", this);
		}
		// 플레이어 위치 리스트에 자신을 추가
		playerTransformList.Add(transform);

		// 자신의 hp를 ui에 출력
		if (photonView.IsMine)
		{
			myStat.UpdateHp(hp, maxHp);
			myStat.UpdateStemina(stemina, maxStemina);
		}
	}

	private void Update()
	{
		// 스테미너 자연 회복
		IncreaseStemina(2 * Time.deltaTime);
	}

	// 스테미너 사용
	public void UseStemina(float amount)
	{
		// 최대 최소 설정
		stemina = Mathf.Clamp(stemina - amount, 0, maxStemina);
		// 자신의 스테이터스만 ui업데이트
		if (photonView.IsMine)
			myStat.UpdateStemina(stemina, maxStemina);
	}

	// 스테미너 회복
	public void IncreaseStemina(float amount)
	{
		// 최대 최소 설정
		stemina = Mathf.Clamp(stemina + amount, 0, maxStemina);
		// 자신의 스테이터스만 ui업데이트
		if (photonView.IsMine)
			myStat.UpdateStemina(stemina, maxStemina);
	}

	// 리스폰 시 호출
	private void ReStart()
	{
		// 플레이어 위치 리스트에 추가
		playerTransformList.Add(transform);
		dead = false;
		hp = maxHp;
		stemina = maxStemina;
		// 자신의 hp를 ui에 출력
		if (photonView.IsMine)
		{
			myStat.UpdateHp(hp, maxHp);
			myStat.UpdateStemina(stemina, maxStemina);
		}

		// 플레이어 조작을 받는 컴포넌트들 활성화
		playerMovement.enabled = true;
		playerControl.enabled = true;
	}


	// hp를 동기화 하기 위한 함수
	[PunRPC]
	public void ApplyUpdatedHealth(float newHp, bool isDead)
	{
		hp = newHp;
		dead = isDead;
	}

	// 체력 회복 함수
	public void IncreaseHp(float amount)
	{
		hp += amount;
		if (hp > maxHp)
			hp = maxHp;
		if (photonView.IsMine)
		{
			myStat.UpdateHp(hp, maxHp);
		}
	}

	// 피격 함수
	[PunRPC]
	public void OnDamage(float damage)
	{
		// 죽었다면 함수 실행하지 않음
		if (dead)
			return;

		if (PhotonNetwork.IsMasterClient)
		{
			// 호스트가 먼저 제력감소 적용
			hp -= damage;
			if (hp <= 0)
				hp = 0;
			
			// 호스트에서 클라이언트로 동기화
			photonView.RPC("ApplyUpdatedHealth", RpcTarget.Others, hp, dead);
			// 다른 클라이언트들도 OnDamage를 실행하도록 함
			photonView.RPC("OnDamage", RpcTarget.Others, damage);
		}

		// 자신의 플레이어 UI업데이트, 최종 점수  
		if (photonView.IsMine)
		{
			playerHud.GetDamaged();
			score.GetDamaged(damage);
			myStat.UpdateHp(hp, maxHp);
		}

		// 체력이 0 이하 && 아직 죽지 않았다면 사망 처리 실행
		if (hp <= 0)
		{
			dead = true;
			// 캐릭터 입력을 막음
			playerMovement.enabled = false;
			playerControl.enabled = false;
			StartCoroutine(IDead());
			// 최종 점수에 사망 카운트 추가
			if (photonView.IsMine)
				score.Die();
		}
	}

	// hp를 동기화
	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.IsWriting)
		{
			stream.SendNext(hp);
		}
		else if (stream.IsReading)
		{
			this.hp = (float)stream.ReceiveNext();
		}
	}
	
	IEnumerator IDead()
	{
		// 사망 시 타겟 리스트에서 제거
		PlayerStatus.playerTransformList.Remove(transform);
		if(photonView.IsMine)
			playerHud.CountDown(5f);
		yield return new WaitForSeconds(5f);
		// 5초 후 리스폰
		ReStart();
	}
}
