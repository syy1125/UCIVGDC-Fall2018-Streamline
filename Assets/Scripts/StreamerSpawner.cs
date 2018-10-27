using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StreamerSpawner : MonoBehaviour {

    // Use this for initialization
    public Transform streamerPrefab;
    public float spawnDelay;
    public int waveSize;
    public Vector2 randomOffset;
    public float smallScale = .35f;
    public float mediumScale = 0.7f;
    public float largeScale = 1.2f;
    public Color smallColor;
    public Color mediumColor;
    public Color largeColor;
	void Start () {
        StartCoroutine(SpawnLoop());
    }

    // Update is called once per frame
    void Update () {
	}
    private IEnumerator SpawnLoop()
    {
        while (true)
        {
            SpawnStreamer(waveSize);
            yield return new WaitForSeconds(spawnDelay);
        }
    }
    private void SpawnStreamer(int n)
    {
        for (int i = 0; i < n; i++)
        {
            Vector3 offset = new Vector3(Random.Range(-randomOffset.x, randomOffset.x), Random.Range(-randomOffset.y, randomOffset.y));
            Transform newStreamer = Instantiate(streamerPrefab, transform.position + (Vector3)offset, transform.rotation);
            int rng = (int)Random.Range(0, 3);
            if(rng == 0)
            {
                newStreamer.GetComponent<Streamer>().SetScale(smallScale);
                newStreamer.GetComponent<TrailRenderer>().startColor = newStreamer.GetComponent<TrailRenderer>().endColor = smallColor;
            } else if(rng == 1)
            {
                newStreamer.GetComponent<Streamer>().SetScale(mediumScale);
                newStreamer.GetComponent<TrailRenderer>().startColor = newStreamer.GetComponent<TrailRenderer>().endColor = mediumColor;
            } else if(rng == 2)
            {
                newStreamer.GetComponent<Streamer>().SetScale(largeScale);
                newStreamer.GetComponent<TrailRenderer>().startColor = newStreamer.GetComponent<TrailRenderer>().endColor = largeColor;
            }

        }
    }
    
}
