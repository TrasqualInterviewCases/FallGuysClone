using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager gmInstance;

    public event Action<bool> OnRaceStart;


    [Header("UI Elements")]
    [SerializeField]
    GameObject startButton;
    [SerializeField]
    Canvas countdownCanvas;
    [SerializeField]
    TMP_Text countdownText;
    [SerializeField]
    TMP_Text timerText;


    [Header ("Prefabs")]
    public List<Transform> spawnPoints = new List<Transform>();
    [SerializeField]
    GameObject opponentPrefab;
    [SerializeField]
    GameObject playerPrefab;


    private void Awake()
    {
        gmInstance = this;
    }

    private void Start()
    {
        SetupCharacters();
    }

    private IEnumerator CountDownTimer()
    {
        Time.timeScale = 0;
        countdownCanvas.gameObject.SetActive(true);
        int i = 3;
        while (i > 0)
        {
            countdownText.transform.localScale = Vector3.zero;
            Color a = countdownText.color;
            a.a = 0f;
            countdownText.color = a;
            countdownText.text = i.ToString();
            while(countdownText.transform.localScale.x < 1)
            {
                countdownText.transform.localScale += Vector3.one * Time.unscaledDeltaTime*2;
                a.a += Time.unscaledDeltaTime*2;
                countdownText.color = a;
                yield return null;
            }
            i--;
            yield return new WaitForSecondsRealtime(.8f);
        }

        countdownCanvas.gameObject.SetActive(false);
        Time.timeScale = 1f;
        timerText.gameObject.SetActive(true);
        OnRaceStart?.Invoke(true);
    }

    public void OnStartButtonClicked()
    {
        startButton.SetActive(false);
        StartCoroutine(CountDownTimer());
    }

    private void SetupCharacters()
    {
        for (int i = 0; i < spawnPoints.Count; i++)
        {
            if(i != 4)
            {
                Instantiate(opponentPrefab, spawnPoints[i].position, Quaternion.identity);
            }
            else if(i == 4)
            {
                Instantiate(playerPrefab, spawnPoints[i].position, Quaternion.identity);
            }
        }
    }
}
