using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutrialHand : MonoBehaviour
{
    [SerializeField]
    float speed = 2.5f;
    [SerializeField]
    float xScale = 300f;
    [SerializeField]
    float yScale = 300f;

    Vector3 startPos;

    private void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        transform.position = startPos - new Vector3(-7f, transform.GetComponent<RectTransform>().rect.height / 2 + 10, 0) + (Vector3.right * Mathf.Sin(Time.timeSinceLevelLoad / 2 * speed) * xScale * transform.parent.GetComponent<Canvas>().scaleFactor - Vector3.up * Mathf.Sin(Time.timeSinceLevelLoad * speed) * yScale * transform.parent.GetComponent<Canvas>().scaleFactor);

        if (Input.GetMouseButtonDown(0))
        {
            gameObject.SetActive(false);
        }
    }

    private void OnDisable()
    {
        transform.position = startPos;
    }
}
