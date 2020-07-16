using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureGenerator : MonoBehaviour
{
    public static float DeepWater = 0.2f;
    public static float ShallowWater = 0.4f;
    public static float Sand = 0.5f;
    public static float Grass = 0.7f;
    public static float Forest = 0.8f;
    public static float Rock = 0.9f;
    public static float Snow = 1;

    public static Color DeepColor = new Color(0, 0, 0.5f, 1);
    public static Color ShallowColor = new Color(25 / 255f, 25 / 255f, 150 / 255f, 1);
    public static Color SandColor = new Color(240 / 255f, 240 / 255f, 64 / 255f, 1);
    public static Color GrassColor = new Color(50 / 255f, 220 / 255f, 20 / 255f, 1);
    public static Color ForestColor = new Color(16 / 255f, 160 / 255f, 0, 1);
    public static Color RockColor = new Color(0.5f, 0.5f, 0.5f, 1);
    public static Color SnowColor = new Color(1, 1, 1, 1);

    public static Texture2D GetTexture(int width, int height, Tile[,] tiles)
    {
        var texture = new Texture2D(width, height);
        var pixels = new Color[width * height];

        for (var x = 0; x < width; x++)
        {
            for (var y = 0; y < height; y++)
            {
                float value = tiles[x, y].HeightValue;

                if (value <= DeepWater) pixels[x + y * width] = DeepColor;

                else if (value > DeepWater && value <= ShallowWater) pixels[x + y * width] = ShallowColor;
                else if (value > ShallowWater && value <= Sand) pixels[x + y * width] = SandColor;
                else if (value > Sand && value <= Grass) pixels[x + y * width] = GrassColor;
                else if (value > Grass && value <= Forest) pixels[x + y * width] = ForestColor;
                else if (value > Forest && value <= Rock) pixels[x + y * width] = RockColor;
                else pixels[x + y * width] = SnowColor;
            }
        }

        texture.SetPixels(pixels);
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.Apply();
        return texture;
    }

}