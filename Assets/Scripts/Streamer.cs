using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Streamer : MonoBehaviour {

    // Use this for initialization
    public float trailWidth;

    private Vector2 velocity;
    public float rightVelocity;
    public float orthogonalVelocity;
    public float orthogonalChance;
    public float moveDuration;
    private float moveTimer;
    public float lifeTime;
    public float scale;
    private TrailRenderer tr;
	void Awake () {
        Destroy(gameObject, lifeTime);
        moveTimer = moveDuration;
        tr = GetComponent<TrailRenderer>();
        tr.startWidth = trailWidth*scale;
        tr.endWidth = trailWidth*scale;
	}
	
	// Update is called once per frame
	void Update () {
        if(moveTimer < 0)
        {
            moveTimer = moveDuration;
            float rng = Random.Range(0f, 1f);
            if(rng < orthogonalChance)
            {
                if(rng < orthogonalChance / 2)
                {
                    velocity = new Vector2(rightVelocity, orthogonalVelocity);
                } else
                {
                    velocity = new Vector2(rightVelocity, -orthogonalVelocity);
                }
            } else
            {
                velocity = Vector2.right * rightVelocity;
            }
        } else
        {
            moveTimer -= Time.deltaTime;
        }
        transform.position += ((transform.up*velocity.y)+(transform.right*velocity.x))*Time.deltaTime*scale;
	}
    public void SetScale(float s)
    {
        scale = s;
        tr.startWidth = trailWidth * scale;
        tr.endWidth = trailWidth * scale;
    }
}
