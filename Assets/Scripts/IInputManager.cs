using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �ٸ� â�� ������ �� �Է��� �������� �������̽�
interface IInputManager
{
	// �Է� ���� ����
	static Stack<string> nameStack = new();	
	string IName { get; set; }
	bool CanInput();
}
