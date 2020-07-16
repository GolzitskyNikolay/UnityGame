using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Earth : MonoBehaviour
{
    public int countOfPoints = 4;
    public int minDistance = 5;
    public int maxDistance = 15;
    public float mistake = 1;
    public Material material;

    Vector2[] vert2;

    // Start is called before the first frame update
    void Start()
    {
        GameObject gameWorld = GameObject.FindGameObjectWithTag("GameWorld");

        GameObject earth = GameObject.CreatePrimitive(PrimitiveType.Quad);
        Object.DestroyImmediate(earth.GetComponent<MeshCollider>());
        earth.name = "Earth";

        MeshRenderer meshRenderer = earth.GetComponent<MeshRenderer>();
        meshRenderer.material = material;

        Mesh mesh = earth.GetComponent<MeshFilter>().mesh;

        //очищаем меш, удаляем предыдущие вершины
        mesh.Clear();

        AddPoints(countOfPoints, GameObject.FindGameObjectWithTag("Player").transform.position,
            minDistance, maxDistance, mesh, mistake);

        earth.AddComponent<EdgeCollider2D>();
        earth.GetComponent<EdgeCollider2D>().points = vert2;

    }

    private void AddPoints(int countOfPoints, Vector3 centralPoint, int minDistance,
        int maxDistance, Mesh mesh, float pointDistance)
    {
        if (countOfPoints >= 3)
        {

            //Debug.Log("centralPoint = (" + centralPoint.x + ", " + centralPoint.y + ")");

            // вершины
            Vector3[] vert = new Vector3[countOfPoints + 1];

            //для коллайдера нам не нужна центральная точка, но нужно последним элментом добавить первую,
            //поэтому размерность такая же
            vert2 = new Vector2[countOfPoints + 1]; 

            //добавляем центральную точку, т.к. она есть во всех тругольниках
            vert[0] = centralPoint;

            //треугольники. Строятся от центральной точки объекта к каждой сгенерированной точке
            int[] tri = new int[countOfPoints * 3];

            //поворот на этот угол (переводим в радианы, так как коси и синусы по умолчанию считают в них)
            float alpha = 360 / countOfPoints * Mathf.Deg2Rad;

            //расстояние от центральной точки до первой
            float lastPointDistance = Random.Range(minDistance, maxDistance);

            for (int i = 1; i <= countOfPoints; i++)
            {
                float distance = Random.Range(lastPointDistance - Random.Range(0, mistake), lastPointDistance + Random.Range(0, mistake));

                Vector2 point = new Vector2(-distance, 0);

                float cos = Mathf.Cos(alpha * (i - 1));
                float sin = Mathf.Sin(alpha * (i - 1));

                //Новые координаты точки при повороте 
                float newX = (point.x - centralPoint.x) * cos - (point.y - centralPoint.y) * sin + centralPoint.x;

                float newY = (point.x - centralPoint.x) * sin + (point.y - centralPoint.y) * cos + centralPoint.y;

                vert[i] = new Vector3(newX, newY, 0);

                vert2[i - 1] = new Vector3(newX, newY);

                //Debug.Log(i + ") rotation = " + alpha * (i - 1) + ", random = " + distance + ", x = " + (int)newX + ", y = " + (int)newY);

                //записываем стороны треугольников по часовой стрелке
                tri[3 * i - 3] = 0;
                tri[3 * i - 2] = i + 1;
                tri[3 * i - 1] = i;
            }

            //замыкаем коллайдер
            vert2[countOfPoints] = vert2[0];

            //соединяем сторону последнего треугольника с первой точкой
            tri[3 * countOfPoints - 2] = 1;

            for (int i = 1; i <= countOfPoints * 3; i++)
            {
                //Debug.Log("tri[" + (i - 1) + "] = " + tri[i-1]);
            }

            mesh.vertices = vert;
            mesh.triangles = tri;
        }
    }
}
