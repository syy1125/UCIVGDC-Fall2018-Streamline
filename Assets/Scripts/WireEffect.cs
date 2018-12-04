using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WireEffect : MonoBehaviour {

    // Use this for initialization
    public enum FollowColor
    {
        RED, GREEN
    }
    private HashSet<Vector2Int> exploredRedTiles;
    private HashSet<Vector2Int> exploredGreenTiles;

    public Vector2Int destTile;
    public Vector2 destination;
    public float speed;
    private TrailRenderer tr;
    public Transform effectPrefab;
    private bool atDestination;
    public FollowColor followColor;
    [HideInInspector]
    public bool noSpread = false;
    public Color greenColor;
    public Color redColor;
	void Awake () {
        tr = GetComponent<TrailRenderer>();
        SetColor(redColor);
        atDestination = false;
        exploredRedTiles = null;
        exploredGreenTiles = null;
	}
	
	// Update is called once per frame
	void Update () {
        
		if((destination-(Vector2)transform.position).magnitude < speed * Time.deltaTime)
        {
            transform.position = destination;
            if (!atDestination)
            {
                //Destroy(gameObject, tr.time);
                if (followColor == FollowColor.GREEN)
                    exploredGreenTiles.Add(destTile);
                else if (followColor == FollowColor.RED)
                    exploredRedTiles.Add(destTile);
                if(!noSpread)
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
        tr.startColor = c;
        tr.endColor = c;
    }
    public void SetHashSets(ref HashSet<Vector2Int> exploredRed, ref HashSet<Vector2Int> exploredGreen)
    {
        exploredRedTiles = exploredRed;
        exploredGreenTiles = exploredGreen;
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
        WireEffect we = null;
        for (int i = 0; i < neighbors.Length; i++)
        {
            if (neighbors[i] == null)
                continue;
            wire = neighbors[i].GetComponent<Wire>();
            op = neighbors[i].GetComponent<Receiver>();
            if (wire != null)
            {
                if (followColor == FollowColor.RED && wire.HasRed && !exploredRedTiles.Contains(wire.Location))
                {
                    Transform newEffect = Instantiate(effectPrefab, origin, Quaternion.identity);
                    we = newEffect.GetComponent<WireEffect>();
                    we.SetHashSets(ref exploredRedTiles, ref exploredGreenTiles);
                    we.destTile = wire.Location;
                    we.destination = (Vector2)wire.RedParts.Center.transform.position;
                    we.SetColor(redColor);
                    we.followColor = FollowColor.RED;
                }
                else if (followColor == FollowColor.GREEN && wire.HasGreen && !exploredGreenTiles.Contains(wire.Location))
                {
                    Transform newEffect = Instantiate(effectPrefab, origin, Quaternion.identity);
                    we = newEffect.GetComponent<WireEffect>();
                    we.SetHashSets(ref exploredRedTiles, ref exploredGreenTiles);
                    we.destTile = wire.Location;
                    we.destination = (Vector2)wire.GreenParts.Center.transform.position;
                    we.SetColor(greenColor);
                    we.followColor = FollowColor.GREEN;
                }
            }
            else if (op != null)
            {
                if (myLocation == op.Location + op.InputDirection1
                    || myLocation == op.Location + op.InputDirection2 || (op.GetComponent<Operator>().OpName.Equals("Output")))
                {
                    Transform newEffect = Instantiate(effectPrefab, origin, Quaternion.identity);
                    we = newEffect.GetComponent<WireEffect>();
                    we.SetHashSets(ref exploredRedTiles, ref exploredGreenTiles);
                    we.destTile = op.Location;
                    we.destination = (Vector2)op.transform.position;
                    if (followColor == FollowColor.RED)
                    {
                        we.SetColor(redColor);
                        we.destination += (Vector2)(self.GetComponent<Wire>().RedParts.Center.transform.position 
                            - self.GetComponent<Wire>().transform.position);
                    }
                    else
                    {
                        we.SetColor(greenColor);
                        we.destination += (Vector2)(self.GetComponent<Wire>().GreenParts.Center.transform.position 
                            - self.GetComponent<Wire>().transform.position);
                    }
                    we.followColor = followColor;
                    we.noSpread = true;
                }
            }
        }
    
    }
}
