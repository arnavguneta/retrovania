using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 15f;
    private float travelTime;

    private AudioSource audioSrc;
    private Rigidbody2D myBody;

    public AudioClip onHit;
    public AudioClip onShoot;

    void Awake()
    {
        myBody = GetComponent<Rigidbody2D>();
        audioSrc = GetComponent<AudioSource>();
    }
    // Start is called before the first frame update
    void Start()
    {
        audioSrc.PlayOneShot(onShoot, 0.8f);
        myBody.velocity = transform.right * speed;
        travelTime = 0.0f;
    }

    void Update() {
        Destroy(gameObject,1); // destroy after 1 seconds
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
            audioSrc.PlayOneShot(onHit, 0.8f);
    }
}
