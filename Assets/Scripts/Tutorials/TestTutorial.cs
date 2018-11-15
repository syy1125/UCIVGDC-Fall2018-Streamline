using System.Collections;
using UnityEngine;

public class TestTutorial : Tutorial
{
	private IEnumerator Start()
	{
		yield return new WaitForSeconds(5);
		Next();
		yield return new WaitForSeconds(5);
		Next();
		yield return new WaitForSeconds(5);
		Next();
	}
}