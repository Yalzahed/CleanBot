
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class script1 : MonoBehaviour
{


 
    // Start is called before the first frame update

    public int TextureWidth;
    public int TextureHeight;
    public int Hitcount = 0;

    public Color foregroundColor = new Color(0, 1, 0, 0);
    public Color backgroundColor = Color.yellow;
    public Texture2D texture;
    float alpha = 0;

    float[][] texMap;


    void Start()
    {
        this.gameObject.tag = "CleaningObject";
        TextureWidth = 256;
        TextureHeight = 256;
        texMap = new float[TextureHeight][];
        createTexture();
        fill(backgroundColor);

        for (int i = 0; i < TextureWidth; i++)
        {
            texMap[i] = new float[TextureWidth];
        }
    }


    void createTexture()
    {
        texture = new Texture2D(TextureWidth, TextureHeight);
        texture.filterMode = FilterMode.Point;
        GetComponent<MeshRenderer>().material.SetTexture("_MainTex", texture);
    }

    //void whileMousePressed()
    //{
    //    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    //    RaycastHit hitInfo;
    //    if (Physics.Raycast(ray, out hitInfo))
    //    {
    //        var pixelCoords = uv2PixelCoords(hitInfo.textureCoord);
    //        Stroke(pixelCoords);
    //    }
    //}

    public void onHIT(RaycastHit hit)
    {
        var pixelCoord = uv2PixelCoords(hit.textureCoord);
        addRay(pixelCoord);
     
        Hitcount++;

    }

    public void addRay(Vector2Int pixelCoords)
    {
        try
        {
            if (texMap[pixelCoords.y][pixelCoords.x] < 100)
            {
                texMap[pixelCoords.y][pixelCoords.x]++;
                // print(texMap[pixelCoords.y][pixelCoords.x]);
                Stroke(pixelCoords);

            }
        }
        catch (System.IndexOutOfRangeException e)
        {
            print("index out of range");
        }

    }

    public void Stroke(Vector2Int pixelCoords)
    {
        try
        {
            
            
                texture.SetPixel(pixelCoords.x, pixelCoords.y,Color.Lerp(backgroundColor,foregroundColor,texMap[pixelCoords.y][pixelCoords.x]/10));
                // texture.Apply();

            

            // else if (texMap[pixelCoords.y][pixelCoords.x] < 30)
            // {
            //     texture.SetPixel(pixelCoords.x, pixelCoords.y, new Color(1f, 0.435f, 0f, 1f));
            //     // texture.Apply();

            // }

            // else if (texMap[pixelCoords.y][pixelCoords.x] < 45)
            // {
            //     texture.SetPixel(pixelCoords.x, pixelCoords.y, new Color(1f, 0.078f, 0f, 1f));
            //     // texture.Apply();

            // }

            // else if (texMap[pixelCoords.y][pixelCoords.x] < 60)
            // {
            //     texture.SetPixel(pixelCoords.x, pixelCoords.y, new Color(1f, 0.137f, 0.137f, 1));
            //     // texture.Apply();

            // }

            // else if (texMap[pixelCoords.y][pixelCoords.x] < 75)
            // {
            //     texture.SetPixel(pixelCoords.x, pixelCoords.y, new Color(0.168f, 1f, 0f, 1f));
            //     // texture.Apply();

            // }

            // else if (texMap[pixelCoords.y][pixelCoords.x] < 90)
            // {
            //     texture.SetPixel(pixelCoords.x, pixelCoords.y, new Color(0.4f, 1f, 0f, 1f));
            //     // texture.Apply();

            // }

            // else if (texMap[pixelCoords.y][pixelCoords.x] > 100)
            // {
            //     texture.SetPixel(pixelCoords.x, pixelCoords.y, new Color(0f, 1f, 0f, 1f));
            //     // texture.Apply();

            // }
        }


        catch (System.IndexOutOfRangeException e) {
            print("index was out of range");


        }
    }


    public Vector2Int uv2PixelCoords(Vector2 uv)
    {
        int x = Mathf.FloorToInt(uv.x * TextureWidth);
        int y = Mathf.FloorToInt(uv.y * TextureHeight);
        return new Vector2Int(x, y);
    }

    void fill(Color color)
    {
        Color32[] pixels = texture.GetPixels32();
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = color;
        }
        texture.SetPixels32(pixels);
        texture.Apply();
    }

    // Update is called once per frame

}
