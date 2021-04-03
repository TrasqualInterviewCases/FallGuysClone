using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WallPaint : MonoBehaviour
{
    Color[] colors;

    [HideInInspector]
    Es.InkPainter.Sample.MousePainter brush;

    Color color;

    [SerializeField]
    TMP_Text text;

    Texture2D tex2D;
    Texture tex;

    int pixelHeight;
    int pixelWidth;

    Rect texRect;

    Vector3 refColor;

    Dictionary<int, Color> colorDict = new Dictionary<int, Color>();
    List<Color> correctColors = new List<Color>();

    private void Start()
    {
        tex = GetComponent<MeshRenderer>().material.mainTexture;
        RenderTexture.active = tex as RenderTexture;
        pixelHeight = tex.height;
        pixelWidth = tex.width;
        texRect = new Rect(0, 0, pixelWidth, pixelHeight);
        tex2D = new Texture2D(pixelWidth, pixelHeight, TextureFormat.RGBA32, false);
        color = Camera.main.GetComponent<Es.InkPainter.Sample.MousePainter>().brush.brushColor;
    }

    private void Update()
    {
        tex = GetComponent<MeshRenderer>().material.mainTexture;
        RenderTexture.active = tex as RenderTexture;
        tex2D.ReadPixels(texRect, 0, 0);
        tex2D.Apply();
        colors = tex2D.GetPixels();

        for (int i = 0; i < colors.Length; i++)
        {
            //if(!colorDict.ContainsKey(i))
            colorDict.Add(i, colors[i]);

            if(colorDict[i] == color)
            {
                if (!correctColors.Contains(colorDict[i]))
                correctColors.Add(colorDict[i]);
            }
        }

        text.text = ((correctColors.Count / colors.Length) * 100).ToString();
    }
}
