using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextureCheck : MonoBehaviour
{
    Vector3 mouseTexCoord;

    [SerializeField]
    GameObject congratsPanel;
    [SerializeField]
    GameObject tutorialCanvas;
    [SerializeField]
    ParticleSystem particle;

    [SerializeField]
    Texture brushTex;
    public TMP_Text percentText;
    float percentage;
    [SerializeField]
    GameObject wallToPaint;

    Texture mainTexture;
    [SerializeField]
    float brushTexRadius = 3f;

    List<TexturePoint> texturePoints = new List<TexturePoint>();

    float paintedPixels = 0;

    bool gotTexturePoints = false;

    public bool isPainting = false;

    bool win = false;

    public class TexturePoint
    {
        public Vector3 coordinate;
        public bool isPainted = false;
    }

    private void Start()
    {
        percentText.text = "% 0";
        tutorialCanvas.SetActive(true);
    }

    private void Update()
    {           

        if (Input.GetMouseButton(0) && isPainting)
        {
            ScreenToTextureCoordinates();

            for (int i = 0; i < texturePoints.Count; i++)
            {
                if (CheckPixels(texturePoints[i].coordinate) <= brushTexRadius && !texturePoints[i].isPainted)
                {
                    texturePoints[i].isPainted = true;
                    paintedPixels++;
                }
            }
            percentage = Mathf.RoundToInt((paintedPixels / texturePoints.Count) * 100);
            percentText.text = "%" + percentage.ToString();
        }

        if(percentage >= 100 && !win)
        {
            StartCoroutine(Win());
            win = false;
                      
        }
    }

    private IEnumerator Win()
    {
        particle.Play();
        yield return new WaitForSeconds(1.5f);
        congratsPanel.SetActive(true);
        Time.timeScale = 0;
    }

    private float CheckPixels(Vector3 pixelCoord)
    {
        float distance = Vector3.Distance(mouseTexCoord, pixelCoord);
        return distance;
    }


    private void ScreenToTextureCoordinates()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform.CompareTag("wall"))
            {
                wallToPaint = hit.transform.gameObject;
                mainTexture = wallToPaint.GetComponent<MeshRenderer>().material.mainTexture;
                if (!gotTexturePoints)
                {
                    TexturePoints();
                    gotTexturePoints = true;
                }
                var pixelUV = hit.textureCoord;

                mouseTexCoord = hit.point;/*new Vector3(pixelUV.x * mainTexture.width, 0f, pixelUV.x * mainTexture.height);*/
            }
            else return;
        }
    }

    private void TexturePoints()
    {
        //for (int i = 0; i < mainTexture.width; i++)
        //{
        //    for (int j = 0; j < mainTexture.height; j++)
        //    {
        //        TexturePoint texPoint = new TexturePoint();
        //        texPoint.coordinate = new Vector3(i, 0, j);
        //        texturePoints.Add(texPoint);
        //    }
        //}
        float wallWidthRatio = wallToPaint.GetComponent<MeshCollider>().bounds.size.x / mainTexture.width;
        float wallHeightRatio = wallToPaint.GetComponent<MeshCollider>().bounds.size.y / mainTexture.height;

        for (float i = wallWidthRatio*(-mainTexture.width/2); i < wallWidthRatio * mainTexture.width/2; i+=wallWidthRatio)
        {
            for (float j = wallHeightRatio * (-mainTexture.height / 2); j < wallHeightRatio * mainTexture.height / 2; j += wallHeightRatio)
            {
                TexturePoint texPoint = new TexturePoint();
                texPoint.coordinate = wallToPaint.transform.position + new Vector3(i, j, 0);
                texturePoints.Add(texPoint);
            }
        }
    }

    //private void OnDrawGizmos()
    //{
    //    foreach (var item in texturePoints)
    //    {
    //        Gizmos.color = Color.green;
    //        Gizmos.DrawSphere(item.coordinate, 0.01f);
    //    }
    //}
}
