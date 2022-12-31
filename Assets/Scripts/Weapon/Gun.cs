using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Gun : MonoBehaviour
{
	[SerializeField] public Transform bulletPivot;
	// ���� ���� �� FOV�� �ٲ� ���� ȿ���� ����
	[SerializeField] public float magnification;
	[SerializeField] protected AudioClip shootClip;
	[SerializeField] private Bullet prefab;

	// scriptableobject�� ���� ���� ������
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

	// �⺻ fov
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

		// ĳ��
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

	// �ݹ� �Լ�
	public virtual void Shoot()
	{
		if (isReloading || currentBulletCount <= 0)
			return;

		if (fireTime > weaponData.fireRate)
		{
			Vector3 dir;

			// �Ѿ˹����� ũ�ν���� �߽ɹ������� ����
			if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, float.MaxValue))
			{
				dir = (hit.point - bulletPivot.position).normalized;
				Debug.Log(hit.transform.name);
			}
			else
				dir = transform.forward;

			// �Ѿ��� ������ƮǮ���� ����
			Bullet bullet = BulletManager.Instance.GetPool();
			bullet.transform.position = bulletPivot.position;
			// �Ѿ� �ݹ�
			bullet.Setup(weaponData.bulletSpeed, weaponData.power, dir, score.AddDamage);
			source.PlayOneShot(shootClip);

			fireTime = 0f;
			currentBulletCount -= 1;
			// �Ѿ� ui������Ʈ
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

	// ����
	public void ZoomIn()
	{
		isAim = !isAim;
		// ���λ��¿� ���� fov�� ��ȭ
		cam.fieldOfView = isAim ? FOV /magnification : FOV;
		gunHud.UpdateAim(isAim);
	}

	protected IEnumerator Reload()
	{
		isReloading = true;
		// ������ ����
		yield return new WaitForSeconds(weaponData.reloadRate);
		currentBulletCount = weaponData.maxBulletCount;
		gunHud.ReloadBullet(weaponData.maxBulletCount);
		isReloading = false;
		reloading = null;
	}
}
