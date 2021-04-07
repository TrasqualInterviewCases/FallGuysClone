using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TimerText : MonoBehaviour
{
    private float timer = 0f;

    TMP_Text timerText;

    private void Start()
    {
        timerText = GetComponent<TMP_Text>();
    }

    private void Update()
    {
        timer += Time.deltaTime;
        DisplayTimerText();
    }

    private void DisplayTimerText()
    {
        float min = Mathf.FloorToInt(timer / 60);
        float sec = Mathf.FloorToInt(timer % 60);

        timerText.text = string.Format("{0:00}:{1:00}", min, sec);
    }
}
