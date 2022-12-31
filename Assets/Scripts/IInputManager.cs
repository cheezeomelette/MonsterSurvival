using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 다른 창이 열렸을 때 입력을 막기위한 인터페이스
interface IInputManager
{
	// 입력 가능 스택
	static Stack<string> nameStack = new();	
	string IName { get; set; }
	bool CanInput();
}
