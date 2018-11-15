using UnityEngine;

public class SelectionButton : MonoBehaviour
{
	public ComponentSelection Master;
	
	public string Keybind;
	public ComponentType Type;

	private void Update()
	{
		if (Input.GetKeyDown(Keybind))
		{
			Master.setSelection(Type);
		}
	}

	public void OnButtonClick()
	{
		Master.setSelection(Type);
	}
}