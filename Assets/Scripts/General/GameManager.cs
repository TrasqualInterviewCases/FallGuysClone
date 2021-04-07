using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
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
    [SerializeField]
    TMP_Text standingsText;

    [Header ("Prefabs")]
    public List<Transform> spawnPoints = new List<Transform>();
    [SerializeField]
    GameObject opponentPrefab;
    [SerializeField]
    GameObject playerPrefab;

    List<GameObject> opponents = new List<GameObject>();
    List<CharacterBase> standingsList = new List<CharacterBase>();


    private void Awake()
    {
        gmInstance = this;
        SetupCharacters();
        Standings();
    }

    private void Start()
    {
        AudioManager.instance.PlayAudio("music");
    }

    private void Update()
    {
        SortStandings();
    }

    private IEnumerator CountDownTimer()
    {
        Time.timeScale = 0;
        countdownCanvas.gameObject.SetActive(true);
        int i = 3;
        AudioManager.instance.PlayAudio("countdown");
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
        AudioManager.instance.StopAudio("countdown");
        countdownCanvas.gameObject.SetActive(false);
        Time.timeScale = 1f;
        timerText.gameObject.SetActive(true);
        standingsText.gameObject.SetActive(true);
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
                GameObject opponent = Instantiate(opponentPrefab, spawnPoints[i].position, Quaternion.identity);
                opponents.Add(opponent);
            }
            else if(i == 4)
            {
                Instantiate(playerPrefab, spawnPoints[i].position, Quaternion.identity);                
            }
        }
    }

    public void FinishRace()
    {
        foreach (GameObject opponent in opponents)
        {
            opponent.SetActive(false);
        }
        standingsText.gameObject.SetActive(false);
        AudioManager.instance.StopAudio("music");
        AudioManager.instance.PlayAudio("music2");
    }

    public void OnPlayAgainClicked()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }

    public void OnQuitClicked()
    {
        Application.Quit();
    }

    private void Standings()
    {
        CharacterBase[] characters = FindObjectsOfType<CharacterBase>();

        for (int i = 0; i < characters.Length; i++)
        {
            standingsList.Add(characters[i]);
        }
    }

    private void SortStandings()
    {
        standingsList.Sort(CompareStandings);
        standingsList.Reverse();
        for (int i = 0; i < standingsList.Count; i++)
        {
            if (standingsList[i].GetComponent<PlayerCharacter>() != null)
            {
                standingsText.text = (i+1) + "/" + (standingsList.Count);
            }
        }
    }

    private int CompareStandings(CharacterBase z1, CharacterBase z2)
    {
        return z1.transform.position.z.CompareTo(z2.transform.position.z);
    }
}
