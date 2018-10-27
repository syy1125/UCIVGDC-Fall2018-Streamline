using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ColArray : MonoBehaviour {

    // Use this for initialization
    private List<Text> text;
    private List<int> nums;
    [HideInInspector]
    public int columnHeight;
	void Start () {
        text = new List<Text>();
        foreach (Transform child in transform)
        {
            text.Add(child.GetComponent<Text>());
        }
        nums = new List<int>();
        ClearNums();
    }

    public void ClearNums()
    {
        nums = new List<int>();
        for (int i = 0; i < text.Count; i++)
        {
            nums.Add(-1000);
            text[i].text = "";
        }
    }
    public int GetValue(int i)
    {
        if (i >= nums.Count)
        {
            Debug.Log("Accessing index out of bounds: (Count=" + nums.Count + ",Index=" + i);
            return -1000;
        }
        return nums[i];
    }
    public void SetValue(int i, int newX)
    {
        if (i >= nums.Count)
        {
            Debug.Log("Accessing index out of bounds: (Count=" + nums.Count + ",Index=" + i);
            return;
        }
        nums[i] = newX;
        text[i].text = nums[i].ToString();
    }
}
