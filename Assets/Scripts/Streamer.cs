using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Streamer : MonoBehaviour {

    // Use this for initialization
    private Vector2 velocity;
    public float rightVelocity;
    public float orthogonalVelocity;
    public float orthogonalChance;
    public float moveDuration;
    private float moveTimer;
    public float lifeTime;
	void Start () {
        Destroy(gameObject, lifeTime);
        moveTimer = moveDuration;
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
        transform.position += (Vector3)velocity*Time.deltaTime;
	}
}
