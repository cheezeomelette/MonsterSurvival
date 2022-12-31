using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Gun : MonoBehaviour
{
	[SerializeField] public Transform bulletPivot;
	// 줌을 했을 때 FOV를 바꿔 줌한 효과를 보임
	[SerializeField] public float magnification;
	[SerializeField] protected AudioClip shootClip;
	[SerializeField] private Bullet prefab;

	// scriptableobject로 만든 무기 데이터
	public WeaponData weaponData;

	[HideInInspector]
	public int currentBulletCount;
	[HideInInspector]
	public bool isReloading;
	[HideInInspector]
	public bool isAim = false;

	new protected Transform transform;
	protected AudioSource source;
	protected Camera cam;
	protected GunHUD gunHud;

	protected float fireTime = 10f;
	protected Score score;

	private RaycastHit hit;
	public bool canFire => fireTime > weaponData.fireRate;

	// 기본 fov
	const float FOV = 60;

	Coroutine reloading;

	private void Awake()
	{
		source = GetComponent<AudioSource>();
		currentBulletCount = weaponData.maxBulletCount;
	}

	// Start is called before the first frame update
	void Start()
	{
		transform = base.transform;

		// 캐싱
		cam = Camera.main;
		gunHud = GunHUD.Instance;
		score = Score.Instance;
		currentBulletCount = weaponData.maxBulletCount;
	}

	private void OnEnable()
	{
		if (currentBulletCount <= 0)
		{
			StartCoroutine(Reload());
		}
		isAim = false;
		cam = Camera.main;
	}

	void Update()
	{
		fireTime += Time.deltaTime;
	}

	// 격발 함수
	public virtual void Shoot()
	{
		if (isReloading || currentBulletCount <= 0)
			return;

		if (fireTime > weaponData.fireRate)
		{
			Vector3 dir;

			// 총알방향을 크로스헤어 중심방향으로 설정
			if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, float.MaxValue))
			{
				dir = (hit.point - bulletPivot.position).normalized;
				Debug.Log(hit.transform.name);
			}
			else
				dir = transform.forward;

			// 총알을 오브젝트풀에서 꺼냄
			Bullet bullet = BulletManager.Instance.GetPool();
			bullet.transform.position = bulletPivot.position;
			// 총알 격발
			bullet.Setup(weaponData.bulletSpeed, weaponData.power, dir, score.AddDamage);
			source.PlayOneShot(shootClip);

			fireTime = 0f;
			currentBulletCount -= 1;
			// 총알 ui업데이트
			if (currentBulletCount <= 0)
				Reloading();
			GunHUD.Instance.Shoot(currentBulletCount);
		}
	}

	public void Reloading()
	{
		if (reloading == null)
			reloading = StartCoroutine(Reload());
	}

	// 줌인
	public void ZoomIn()
	{
		isAim = !isAim;
		// 줌인상태에 따라 fov를 변화
		cam.fieldOfView = isAim ? FOV /magnification : FOV;
		gunHud.UpdateAim(isAim);
	}

	protected IEnumerator Reload()
	{
		isReloading = true;
		// 재장전 동작
		yield return new WaitForSeconds(weaponData.reloadRate);
		currentBulletCount = weaponData.maxBulletCount;
		gunHud.ReloadBullet(weaponData.maxBulletCount);
		isReloading = false;
		reloading = null;
	}
}
