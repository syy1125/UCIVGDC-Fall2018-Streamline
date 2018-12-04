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
    private Vector2 originalDirection;
    private Vector2 orthoDirection;
	void Awake () {
        Destroy(gameObject, lifeTime);
        moveTimer = moveDuration;
        tr = GetComponent<TrailRenderer>();
        tr.startWidth = trailWidth*scale;
        tr.endWidth = trailWidth*scale;
        originalDirection = transform.right;
        orthoDirection = transform.up;
	}
	public void Rotate(float angle)
    {
        transform.Rotate(new Vector3(0,0,angle));
        originalDirection = transform.right;
        orthoDirection = transform.up;
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
                    velocity = originalDirection*rightVelocity + orthoDirection*orthogonalVelocity;
                } else
                {
                    velocity = originalDirection*rightVelocity - orthoDirection*orthogonalVelocity;
                }
            } else
            {
                velocity = transform.right * rightVelocity;
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
