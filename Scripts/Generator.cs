using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class Generator : MonoBehaviour
{

    // Настраиваемые переменные для Unity Inspector
    [SerializeField]
    int Width = 256;
    [SerializeField]
    int Height = 256;
    [SerializeField]
    int TerrainOctaves = 6;
    [SerializeField]
    double TerrainFrequency = 1.25;

    private ArrayList islands = new ArrayList();

    // Модуль генератора шума
    ImplicitFractal HeightMap;

    // Данные карты высот
    MapData HeightData;

    // Конечные объекты
    Tile[,] Tiles;

    // Вывод нашей текстуры (компонент unity)
    MeshRenderer HeightMapRenderer;

    Texture2D texture2D;

    GameObject earth;


    void Start()
    {
        // Получаем меш, в который будут рендериться выходные данные
        HeightMapRenderer = transform.GetComponent<MeshRenderer>();

        // Инициализируем генератор
        Initialize();

        // Создаем карту высот
        GetData(HeightMap, ref HeightData);

        // Создаем конечные объекты на основании наших данных
        LoadTiles();

        // Рендерим текстурное представление нашей карты
        texture2D = TextureGenerator.GetTexture(Width, Height, Tiles);

        GetIslands();

        HeightMapRenderer.materials[0].mainTexture = texture2D;
        GameObject.FindGameObjectWithTag("MiniMap").GetComponent<RawImage>().texture = texture2D;

        earth = GameObject.FindGameObjectWithTag("Earth");
    }

    private void Initialize()
    {
        // Инициализируем генератор карты высот
        HeightMap = new ImplicitFractal(FractalType.Multi,
                                       BasisType.Simplex,
                                       InterpolationType.Quintic,
                                       TerrainOctaves,
                                       TerrainFrequency,
                                       UnityEngine.Random.Range(0, int.MaxValue));
    }

    // Извлекаем данные из модуля шума
    private void GetData(ImplicitModuleBase module, ref MapData mapData)
    {
        mapData = new MapData(Width, Height);

        // циклично проходим по каждой точке x,y - получаем значение высоты
        for (var x = 0; x < Width; x++)
        {
            for (var y = 0; y < Height; y++)
            {
                //Сэмплируем шум с небольшими интервалами
                float x1 = x / (float)Width;
                float y1 = y / (float)Height;

                float value = (float)HeightMap.Get(x1, y1);

                //отслеживаем максимальные и минимальные найденные значения
                if (value > mapData.Max) mapData.Max = value;
                if (value < mapData.Min) mapData.Min = value;

                mapData.Data[x, y] = value;
            }
        }
    }

    // Создаем массив тайлов из наших данных
    private void LoadTiles()
    {
        Tiles = new Tile[Width, Height];

        for (var y = 0; y < Height; y++)
        {
            for (var x = 0; x < Width; x++)
            {
                Tile t = new Tile();
                t.X = x;
                t.Y = y;

                float value = HeightData.Data[x, y];

                //нормализуем наше значение от 0 до 1
                value = (value - HeightData.Min) / (HeightData.Max - HeightData.Min);

                t.HeightValue = value;

                Tiles[x, y] = t;
            }
        }
    }

    private void GetIslands()
    {
        Vector2 firstPoint = new Vector2(0, 0);


        bool firstPointIsFounded = false;

        HashSet<Vector2> islandsBorders = new HashSet<Vector2>();

        for (var j = 0; j < Height; j++)
        {
            for (var l = 0; l < Width; l++)
            {
                if (Tiles[l, j].HeightValue > 0.4f && Tiles[l, j].HeightValue <= 0.5 && !islandsBorders.Contains(new Vector2(l, j))
                    && (GetWaterOrEarthAround(l, j, true, null) != null))
                {
                    firstPoint = new Vector2(l, j);

                    texture2D.SetPixel((int)firstPoint.x, (int)firstPoint.y, TextureGenerator.ForestColor);
                    texture2D.Apply();

                    Vector2[] sortedIslandBorders = new Vector2[Width * Height];
                    sortedIslandBorders[0] = firstPoint;
                    islandsBorders.Add(firstPoint);

                    //Debug.Log("start point is (" + firstPoint.x + ", " + firstPoint.y + ")");
                    bool islandIsFounded = false;
                    int i = 1;

                    while (!islandIsFounded)
                    {
                        int x = (int)sortedIslandBorders[i - 1].x; //last point with earth
                        int y = (int)sortedIslandBorders[i - 1].y;

                        List<Vector2> borderPoints = GetNextBorderEarth(x, y, islandsBorders); //getting next point with earth

                        if (borderPoints.Count != 0)
                        {
                            int maxCountOfWaterAround = GetCountOfWaterAround((int)borderPoints[0].x, (int)borderPoints[0].y);
                            int elementIndex = 0;


                            //choose cell with max count of water around
                            for (int e = 2; e <= borderPoints.Count; e++)
                            {
                                int currentCountOfWaterAround = GetCountOfWaterAround((int)borderPoints[e - 1].x, (int)borderPoints[e - 1].y);
                                if (currentCountOfWaterAround > maxCountOfWaterAround && !islandsBorders.Contains(borderPoints[e - 1]))
                                {
                                    maxCountOfWaterAround = currentCountOfWaterAround;
                                    elementIndex = e - 1;
                                }
                            }

                            if (!islandsBorders.Contains(borderPoints[elementIndex]))
                            {
                                //Debug.Log("point before = (" + x + ", " + y + "); point now = (" + newEarth);
                                sortedIslandBorders[i] = borderPoints[elementIndex];
                                islandsBorders.Add(sortedIslandBorders[i]);
                                texture2D.SetPixel((int)sortedIslandBorders[i].x, (int)sortedIslandBorders[i].y, TextureGenerator.SnowColor);
                                texture2D.Apply();
                                i++;
                            }
                        }
                        else
                        {
                            islandIsFounded = true;
                        }
                    }


                    EdgeCollider2D collider = gameObject.AddComponent<EdgeCollider2D>();
                    Vector2[] colliderPoints = new Vector2[i + 1];
                    for (int e = 1; e <= i; e++)
                    {
                        colliderPoints[e - 1] = sortedIslandBorders[e - 1]/100;
                    }
                    colliderPoints[i] = colliderPoints[0]; //замыкаем коллайдер
                    collider.points = colliderPoints;

                    firstPointIsFounded = true;
                    break;
                }

                if (firstPointIsFounded) break;
            }
        }
    }

    //return all border points with earth near with water
    private List<Vector2> GetNextBorderEarth(int x, int y, HashSet<Vector2> islandBorders)
    {
        HashSet<Vector2> checkedEarth = new HashSet<Vector2>(islandBorders);
        Vector2? nextEarth = null;
        int checkedPoints = 0;
        int maxEarthAround = 8;

        List<Vector2> earthWithWaterAround = new List<Vector2>();

        while (checkedPoints < maxEarthAround)
        {
            //Debug.Log("Searching any earth around;  (" + x + ", " + y + ")");
            nextEarth = GetWaterOrEarthAround(x, y, false, checkedEarth);
            checkedPoints++;

            if (nextEarth != null)
            {
                Vector2 earth = (Vector2)nextEarth;
                checkedEarth.Add(earth);

                Vector2? waterOrNull = GetWaterOrEarthAround((int)earth.x, (int)earth.y, true, null);
                if (waterOrNull != null)
                {
                    Vector2 water = (Vector2)waterOrNull;
                    //Debug.Log("point (" + earth.x + ", " + earth.y + ") is earth and has water in point (" + water.x + ", " + water.y + ")");
                    earthWithWaterAround.Add(earth);
                }
                else if (IsBoardOfWorld((int)earth.x, (int)earth.y))
                {
                    //Debug.Log("point (" + earth.x + ", " + earth.y + ") is earth and the world border");
                    earthWithWaterAround.Add(earth);
                }
            }
        }

        return earthWithWaterAround;
    }

    private bool IsBoardOfWorld(int x, int y)
    {
        if (x + 1 == Width || y + 1 == Height || x == 0 || y == 0) return true;
        else return false;
    }

    private int GetCountOfWaterAround(int x, int y)
    {
        int countOfWater = 0;
        float minValue = TextureGenerator.DeepWater;
        float maxValue = TextureGenerator.ShallowWater;

        if (y + 1 < Height && Tiles[x, y + 1].HeightValue > minValue && Tiles[x, y + 1].HeightValue <= maxValue) countOfWater++;

        if (y - 1 >= 0 && Tiles[x, y - 1].HeightValue > minValue && Tiles[x, y - 1].HeightValue <= maxValue) countOfWater++;

        if (x + 1 < Width && Tiles[x + 1, y].HeightValue > minValue && Tiles[x + 1, y].HeightValue <= maxValue) countOfWater++;

        if (x - 1 >= 0 && Tiles[x - 1, y].HeightValue > minValue && Tiles[x - 1, y].HeightValue <= maxValue) countOfWater++;

        if (x + 1 < Width && y - 1 >= 0 && Tiles[x + 1, y - 1].HeightValue > minValue && Tiles[x + 1, y - 1].HeightValue <= maxValue) countOfWater++;

        if (x - 1 >= 0 && y - 1 >= 0 && Tiles[x - 1, y - 1].HeightValue > minValue && Tiles[x - 1, y - 1].HeightValue <= maxValue) countOfWater++;

        if (x - 1 >= 0 && y + 1 < Height && Tiles[x - 1, y + 1].HeightValue > minValue && Tiles[x - 1, y + 1].HeightValue <= maxValue) countOfWater++;

        if (x + 1 < Width && y + 1 < Height && Tiles[x + 1, y + 1].HeightValue > minValue && Tiles[x + 1, y + 1].HeightValue <= maxValue) countOfWater++;

        return countOfWater;
    }

    private Vector2? GetWaterOrEarthAround(int x, int y, bool getWater, HashSet<Vector2> earthWithoutWater)
    {
        float minValue;
        float maxValue;

        if (getWater)
        {
            //Debug.Log("trying to get water...");
            minValue = TextureGenerator.DeepWater;
            maxValue = TextureGenerator.ShallowWater;
        }
        else
        {
            minValue = TextureGenerator.ShallowWater; ;
            maxValue = TextureGenerator.Snow;
        }

        if (y + 1 < Height && Tiles[x, y + 1].HeightValue > minValue && Tiles[x, y + 1].HeightValue <= maxValue &&
           (getWater || (!getWater && !earthWithoutWater.Contains(new Vector2(x, y + 1)))))
        {
            //Debug.Log("getWater = " + getWater + "; (x, y + 1) -> value = " + Tiles[x, y + 1].HeightValue + ",  " +
            //"(!getWater && !earthWithoutWater.Contains(new Vector2(x, y + 1)) = " + (!getWater && !earthWithoutWater.Contains(new Vector2(x, y + 1))));
            return new Vector2(x, y + 1);
        }

        else if (y - 1 >= 0 && Tiles[x, y - 1].HeightValue > minValue && Tiles[x, y - 1].HeightValue <= maxValue &&
           (getWater || (!getWater && !earthWithoutWater.Contains(new Vector2(x, y - 1)))))
        {
            // Debug.Log("getWater = " + getWater + "; (x, y - 1) -> value = " + Tiles[x, y - 1].HeightValue + ",  " +
            //"(!getWater && !earthWithoutWater.Contains(new Vector2(x , y - 1)) = " + (!getWater && !earthWithoutWater.Contains(new Vector2(x, y - 1))));
            return new Vector2(x, y - 1);
        }

        else if (x + 1 < Width && Tiles[x + 1, y].HeightValue > minValue && Tiles[x + 1, y].HeightValue <= maxValue &&
            (getWater || (!getWater && !earthWithoutWater.Contains(new Vector2(x + 1, y)))))
        {
            // Debug.Log("getWater = " + getWater + "; (x + 1, y) -> value = " + Tiles[x + 1, y].HeightValue + ",  " +
            //"(!getWater && !earthWithoutWater.Contains(new Vector2(x + 1, y)) = " + (!getWater && !earthWithoutWater.Contains(new Vector2(x + 1, y))));
            return new Vector2(x + 1, y);
        }


        else if (x - 1 >= 0 && Tiles[x - 1, y].HeightValue > minValue && Tiles[x - 1, y].HeightValue <= maxValue &&
            (getWater || (!getWater && !earthWithoutWater.Contains(new Vector2(x - 1, y)))))
        {
            //Debug.Log("getWater = " + getWater + "; (x - 1, y) -> value = " + Tiles[x - 1, y].HeightValue + ",  " +
            //"(!getWater && !earthWithoutWater.Contains(new Vector2(x - 1 , y)) = " + (!getWater && !earthWithoutWater.Contains(new Vector2(x - 1, y))));
            return new Vector2(x - 1, y);
        }




        else if (x + 1 < Width && y - 1 >= 0 && Tiles[x + 1, y - 1].HeightValue > minValue && Tiles[x + 1, y - 1].HeightValue <= maxValue &&
            (getWater || (!getWater && !earthWithoutWater.Contains(new Vector2(x + 1, y - 1)))))
        {
            //Debug.Log("getWater = " + getWater + "; (x + 1, y - 1) -> value = " + Tiles[x + 1, y - 1].HeightValue + ",  " +
            // "(!getWater && !earthWithoutWater.Contains(new Vector2(x + 1, y - 1)) = " + (!getWater && !earthWithoutWater.Contains(new Vector2(x + 1, y - 1))));
            return new Vector2(x + 1, y - 1);
        }

        else if (x - 1 >= 0 && y - 1 >= 0 && Tiles[x - 1, y - 1].HeightValue > minValue && Tiles[x - 1, y - 1].HeightValue <= maxValue &&
            (getWater || (!getWater && !earthWithoutWater.Contains(new Vector2(x - 1, y - 1)))))
        {
            // Debug.Log("getWater = " + getWater + "; (x - 1, y - 1) -> value = " + Tiles[x - 1, y - 1].HeightValue + ",  " +
            //"(!getWater && !earthWithoutWater.Contains(new Vector2(x - 1 , y - 1)) = " + (!getWater && !earthWithoutWater.Contains(new Vector2(x - 1, y - 1))));
            return new Vector2(x - 1, y - 1);
        }

        else if (x - 1 >= 0 && y + 1 < Height && Tiles[x - 1, y + 1].HeightValue > minValue && Tiles[x - 1, y + 1].HeightValue <= maxValue &&
            (getWater || (!getWater && !earthWithoutWater.Contains(new Vector2(x - 1, y + 1)))))
        {
            //Debug.Log("getWater = " + getWater + "; (x - 1, y + 1) -> value = " + Tiles[x - 1, y + 1].HeightValue + ",  " +
            //"(!getWater && !earthWithoutWater.Contains(new Vector2(x - 1 , y + 1)) = " + (!getWater && !earthWithoutWater.Contains(new Vector2(x - 1, y + 1))));
            return new Vector2(x - 1, y + 1);
        }

        else if (x + 1 < Width && y + 1 < Height && Tiles[x + 1, y + 1].HeightValue > minValue && Tiles[x + 1, y + 1].HeightValue <= maxValue &&
            (getWater || (!getWater && !earthWithoutWater.Contains(new Vector2(x + 1, y + 1)))))
        {
            //Debug.Log("getWater = " + getWater + "; (x + 1, y + 1) -> value = " + Tiles[x + 1, y + 1].HeightValue + ",  " +
            //"(!getWater && !earthWithoutWater.Contains(new Vector2(x + 1, y + 1)) = " + (!getWater && !earthWithoutWater.Contains(new Vector2(x + 1, y + 1))));
            return new Vector2(x + 1, y + 1);
        }

        else return null;
    }
}
