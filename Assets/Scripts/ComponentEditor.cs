using System;
using UnityEngine;
using UnityEngine.UI;

public class ComponentEditor : MonoBehaviour
{
	public Text ComponentName;
	public GameObject ComponentPreview;

	[Serializable]
	public class DirectionGroup
	{
		public GameObject Input1;
		public GameObject Input2;
		public GameObject Output;
	}

	public DirectionGroup Up;
	public DirectionGroup Down;
	public DirectionGroup Left;
	public DirectionGroup Right;

	public void UpdateUI()
	{
		foreach (Transform t in ComponentPreview.transform)
		{
			Destroy(t.gameObject);
		}
		
		if (Grid.Instance.InGrid(Grid.Instance.Selected))
		{
			if (Grid.Instance.IsOperator(Grid.Instance.GetGridComponent(Grid.Instance.Selected)))
			{
				GameObject gridComponent = Grid.Instance.GetGridComponent(Grid.Instance.Selected);
				Instantiate(gridComponent, ComponentPreview.transform);
				// LOL super hacky. Is there a better way?
				ComponentName.text = gridComponent.GetComponent<Operator>().GetType().Name;

				return;
			}
		}

		ComponentName.text = "Empty";
	}
}