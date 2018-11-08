using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WireEffect : MonoBehaviour {

    // Use this for initialization
    public enum FollowColor
    {
        RED, GREEN
    }
    public static HashSet<Vector2Int> exploredRedTiles = new HashSet<Vector2Int>();
    public static HashSet<Vector2Int> exploredGreenTiles = new HashSet<Vector2Int>();

    public Vector2Int destTile;
    public Vector2 destination;
    public float speed;
    public Color myColor;
    private TrailRenderer tr;
    public Transform effectPrefab;
    private bool atDestination;
    public FollowColor followColor;
	void Awake () {
        tr = GetComponent<TrailRenderer>();
        SetColor(myColor);
        atDestination = false;
	}
	
	// Update is called once per frame
	void Update () {
        
		if((destination-(Vector2)transform.position).magnitude < speed * Time.deltaTime)
        {
            transform.position = destination;
            if (!atDestination)
            {
                if (followColor == FollowColor.GREEN)
                    exploredGreenTiles.Add(destTile);
                else if (followColor == FollowColor.RED)
                    exploredRedTiles.Add(destTile);
                SpawnMoreEffects();
            }
            atDestination = true;
            
        } else
        {
            transform.position += (Vector3)((destination - (Vector2)transform.position).normalized * speed * Time.deltaTime);
        }
	}
    public void SetColor(Color c)
    {
        myColor = c;
        tr.startColor = c;
        tr.endColor = c;
    }
    public static void ClearExploredSet()
    {
        exploredRedTiles.Clear();
        exploredGreenTiles.Clear();
    }
    private void SpawnMoreEffects()
    {
        Grid grid = Grid.Instance;
        GameObject self = grid.GetGridComponent(destTile);
        if (self == null)
            return;
        Vector2Int myLocation = Vector2Int.zero;
        Vector3 origin = Vector3.zero;
        GameObject[] neighbors = new GameObject[4];

        if (self.GetComponent<Wire>() != null)
        {
            myLocation = self.GetComponent<Wire>().Location;
            neighbors[0] = grid.GetGridComponent(destTile + Vector2Int.right);
            neighbors[1] = grid.GetGridComponent(destTile + Vector2Int.up);
            neighbors[2] = grid.GetGridComponent(destTile + Vector2Int.left);
            neighbors[3] = grid.GetGridComponent(destTile + Vector2Int.down);
            if (followColor == FollowColor.RED)
                origin = self.GetComponent<Wire>().RedParts.Center.transform.position;
            else
                origin = self.GetComponent<Wire>().GreenParts.Center.transform.position;

        }
        else if (self.GetComponent<Transmitter>() != null)
        {
            neighbors[0] = grid.GetGridComponent(destTile + self.GetComponent<Transmitter>().OutputDirection);
            myLocation = self.GetComponent<Receiver>().Location;
            origin = transform.position;
        }
        
        Wire wire = null;
        Receiver op = null;
        if (followColor == FollowColor.RED)
        {
            for(int i = 0; i < neighbors.Length; i++)
            {
                if (neighbors[i] == null)
                    continue;
                wire = neighbors[i].GetComponent<Wire>();
                op = neighbors[i].GetComponent<Receiver>();
                if (wire != null)
                {
                    if (wire.HasRed && !exploredRedTiles.Contains(wire.Location))
                    {
                        Transform newEffect = Instantiate(effectPrefab, origin, Quaternion.identity);
                        newEffect.GetComponent<WireEffect>().destTile = wire.Location;
                        newEffect.GetComponent<WireEffect>().destination = (Vector2)wire.RedParts.Center.transform.position;
                        newEffect.GetComponent<WireEffect>().SetColor(Color.red);
                        newEffect.GetComponent<WireEffect>().followColor = FollowColor.RED;

                    }
                } else if(op != null)
                {
                    if(myLocation == op.Location+op.InputDirection1 
                        || myLocation == op.Location + op.InputDirection2)
                    {
                        Transform newEffect = Instantiate(effectPrefab, origin, Quaternion.identity);
                        newEffect.GetComponent<WireEffect>().destTile = op.Location;
                        newEffect.GetComponent<WireEffect>().destination = (Vector2)op.transform.position;
                        newEffect.GetComponent<WireEffect>().SetColor(Color.red);
                        newEffect.GetComponent<WireEffect>().followColor = FollowColor.RED;

                    }
                }
            }
        }
        if (followColor == FollowColor.GREEN)
        {
            for (int i = 0; i < neighbors.Length; i++)
            {
                if (neighbors[i] == null)
                    continue;
                wire = neighbors[i].GetComponent<Wire>();
                op = neighbors[i].GetComponent<Receiver>();
                if (wire != null)
                {
                    if (wire.HasGreen && !exploredGreenTiles.Contains(wire.Location))
                    {
                        Transform newEffect = Instantiate(effectPrefab, origin, Quaternion.identity);
                        newEffect.GetComponent<WireEffect>().destTile = wire.Location;
                        newEffect.GetComponent<WireEffect>().destination = (Vector2)wire.GreenParts.Center.transform.position;
                        newEffect.GetComponent<WireEffect>().SetColor(Color.green);
                        newEffect.GetComponent<WireEffect>().followColor = FollowColor.GREEN;

                    }
                }
                else if(op != null)
                {
                    if (myLocation == op.Location + op.InputDirection1
                        || myLocation == op.Location + op.InputDirection2)
                    {
                        Transform newEffect = Instantiate(effectPrefab, origin, Quaternion.identity);
                        newEffect.GetComponent<WireEffect>().destTile = op.Location;
                        newEffect.GetComponent<WireEffect>().destination = (Vector2)op.transform.position;
                        newEffect.GetComponent<WireEffect>().SetColor(Color.green);
                        newEffect.GetComponent<WireEffect>().followColor = FollowColor.GREEN;

                    }
                }
            }
        }
    }
}
