using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MiniMap : MonoBehaviour
{
    public GameObject earth;

    private float scale;
    private Texture2D texture;
    private Transform player;
    private float diff;
    private Vector2 lastPos;
    private bool posIsChanged = false;
    private GameObject map;

    // Start is called before the first frame update
    void Start()
    {
        lastPos = new Vector2(0, 0);
        scale = earth.transform.localScale.x;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        map = GameObject.FindGameObjectWithTag("MiniMap");
    }

    // Update is called once per frame
    void Update()
    {
        if (texture == null && map.GetComponent<RawImage>().texture != null)
        {
            map.transform.rotation.Set(0f, 0f, 180, 0);
            texture = (Texture2D) map.GetComponent<RawImage>().texture;
            diff = texture.width / scale;

            lastPos.x =
            System.Convert.ToInt32(
                player.position.x * diff);
            lastPos.y =
                System.Convert.ToInt32(
                    player.position.y * diff);

            texture.SetPixel(0, 0, Color.red);
        }
        else
        {
            int plX = System.Convert.ToInt32(player.position.x * diff);
            int plY = System.Convert.ToInt32(player.position.y * diff);
            //Debug.Log( plX + "   " + plY);

            texture.SetPixel(plX, plY, Color.red);
        }



        // 



    }
}
