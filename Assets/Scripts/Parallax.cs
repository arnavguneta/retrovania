using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Code adapted from: https://www.youtube.com/watch?v=zit45k6CUMk

public class Parallax : MonoBehaviour
{
    private float length, startpos;
    public GameObject camera;
    public float parallaxEffect;

    // Start is called before the first frame update
    void Start()
    {
        startpos = transform.position.x;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    // Update is called once per frame
    void Update()
    {
        // float temp = (camera.transform.position.x * (1 - parallaxEffect));
        float dist = (camera.transform.position.x * parallaxEffect);

        transform.position = new Vector3(startpos + dist, transform.position.y, transform.position.z);

        // if (temp > startpos + length) startpos += length;
        // else if (temp < startpos - length) startpos -= length;
    }
}
