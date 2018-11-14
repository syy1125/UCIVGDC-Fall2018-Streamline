using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ValueDisplayManager : MonoBehaviour {

    // Use this for initialization
    public static List<ValueDisplay> ValueDisplays = new List<ValueDisplay>();
    public Transform displayPrefab;
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
        
        
	}
    public void AddAllValueDisplays()
    {
        GameObject g = null;
        for(int i = 0; i < Grid.Instance.Width; i++)
        {
            for (int j = 0; j < Grid.Instance.Height; j++)
            {
                g = Grid.Instance.GetGridComponent(i, j);
                if (Grid.Instance.IsWire(g) || Grid.Instance.IsOperator(g))
                {
                    AddValueDisplay(g.transform);
                }
            }
        }
    }
    public void AddValueDisplay(Transform target)
    {
        Transform vDisplay = Instantiate(displayPrefab, target);
        ValueDisplays.Add(vDisplay.GetComponent<ValueDisplay>());
    }
    public static void DestroyAllValueDisplays()
    {
        GameObject g = null;
        for(int i = ValueDisplays.Count-1; i >= 0; i--)
        {
            g = ValueDisplays[i].gameObject;
            ValueDisplays.RemoveAt(i);
            Destroy(g);
        }
    }

}
