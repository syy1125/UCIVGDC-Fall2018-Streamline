using UnityEngine;
using UnityEngine.UI;

public class TutorialStep : MonoBehaviour
{
	public RectTransform Position;
	public Text HintText;

	private void Reset()
	{
		Position = GetComponent<RectTransform>();
		HintText = GetComponentInChildren<Text>();
	}
}