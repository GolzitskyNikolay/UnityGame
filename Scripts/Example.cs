using UnityEngine.UI;
using UnityEngine;


public class Example : MonoBehaviour
{
    // Width and height of the texture in pixels.
    public int worldWidth;
    public int worldHeight;

    // The origin of the sampled area in the plane.
    private float xOrg;
    private float yOrg;

    // The number of cycles of the basic noise pattern that are repeated
    // over the width and height of the texture.
    public float scale;
    public float partition;

    private Texture2D noiseTex;
    private Color[] pix;
    private Renderer rend;

    public void Start()
    {
        CreateIsland();
    }

    public void CreateIsland()
    {
        xOrg = Random.Range(0, 100);
        yOrg = Random.Range(0, 100);
                
        GameObject gameWorld = GameObject.FindGameObjectWithTag("GameWorld");

        GameObject island = GameObject.CreatePrimitive(PrimitiveType.Quad);
        Object.DestroyImmediate(island.GetComponent<MeshCollider>());
        island.name = "Island";

        Mesh mesh = island.GetComponent<MeshFilter>().mesh;
        //очищаем меш, удаляем предыдущие вершины
        mesh.Clear();

        Vector3[] vert = { new Vector3(0, 0, 0), new Vector3(worldWidth, 0, 0),
                            new Vector3(0, worldHeight, 0), new Vector3(worldWidth, worldHeight, 0) };
        int[] tri = { 0, 2, 1, 2, 3, 1 };
        Vector2[] uv = { new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 1), new Vector2(1, 1) };

        mesh.vertices = vert;
        mesh.triangles = tri;
        mesh.uv = uv;


        rend = island.GetComponent<Renderer>();

        // Set up the texture and a Color array to hold pixels during processing.
        noiseTex = new Texture2D(worldWidth, worldHeight, TextureFormat.RGBA32, true);

        pix = new Color[noiseTex.width * noiseTex.height];
        rend.material.mainTexture = noiseTex;
        rend.material.shader = Shader.Find("UI/Default");

        CalcNoise();
    }

    void CalcNoise()
    {
        // For each pixel in the texture...
        float y = 0.0F;
        float xHalf = worldWidth / 2 - 1;
        float yHalf = worldHeight / 2 - 1;

        while (y < noiseTex.height)
        {
            float x = 0.0F;
            while (x < noiseTex.width)
            {

                float xCoord = xOrg + x / noiseTex.width * scale;
                float yCoord = yOrg + y / noiseTex.height * scale;

                float sample = Mathf.PerlinNoise(xCoord, yCoord);

                float xPossibility;
                float yPossibility;

               

                if (sample <  partition)
                {
                    pix[(int)y * noiseTex.width + (int)x] = Color.blue;
                }
                else
                {
                    pix[(int)y * noiseTex.width + (int)x] = Color.white;
                }

                //Debug.Log("x = " + x + "; y = " + y +"; xPos = " + xPossibility + "; yPos = " + yPossibility + "; sample = " + sample);

                x++;
            }
            y++;
        }

        // Copy the pixel data to the texture and load it into the GPU.
        noiseTex.SetPixels(pix);
        noiseTex.Apply();
    }
}