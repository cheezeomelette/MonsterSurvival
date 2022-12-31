using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 템플릿을 이용한 싱글톤 클래스
public class Singleton<T>  : MonoBehaviour
    where T : class
{
    private static T instance;
    public static T Instance => instance;

	private void Awake()
	{
		instance = this as T;
	}

}
