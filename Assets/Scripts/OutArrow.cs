using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class OutArrow : MonoBehaviour {

    // Use this for initialization
    [HideInInspector]
    public Transmitter Trans;
    private Image Img;
	void Start () {
        if(Trans != null)
            transform.position = Trans.transform.position;
        Img = GetComponentInChildren<Image>();
	}
	
	// Update is called once per frame
	void Update () {
        if (Trans == null)
            Destroy(gameObject);
        Img.enabled = !Trans.Location.Equals(Grid.Instance.Selected);
        if (Trans.OutputDirection.Equals(Vector2Int.up))
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        } else if (Trans.OutputDirection.Equals(Vector2Int.right))
        {
            transform.rotation = Quaternion.Euler(0, 0, -90);
        } else if (Trans.OutputDirection.Equals(Vector2Int.down))
        {
            transform.rotation = Quaternion.Euler(0, 0, 180);
        } else
        {
            transform.rotation = Quaternion.Euler(0, 0, 90);
        }
    }
}
