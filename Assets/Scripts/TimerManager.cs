using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerManager : MonoBehaviour
{
    private Text timeDisplay;
    private float playedTime;

    private void Awake()
    {
        timeDisplay = GetComponent<Text>();
    }

    // Start is called before the first frame update
    void Start()
    {
        playedTime = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        playedTime += Time.deltaTime;
        int roundedTime = Mathf.RoundToInt(playedTime);
        timeDisplay.text = string.Format("{0:D2}:{1:D2}:{2:D2}", 
                (roundedTime / 3600) % 24, 
                (roundedTime / 60) % 60, 
                (roundedTime) % 60);
    }
}
