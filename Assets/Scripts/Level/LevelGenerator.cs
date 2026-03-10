using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public GameObject[] roomPrefabs; // Tablica z gotowymi fragmentami pokoi
    public int gridWidth = 4;
    public int gridHeight = 4;
    public float roomSize = 10f; // Rozmiar jednego pokoju w jednostkach Unity

    void Start()
    {
        GenerateLevel();
    }

    void GenerateLevel()
    {
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                // Oblicz pozycjê dla pokoju
                Vector2 pos = new Vector2(x * roomSize, -y * roomSize);

                // Losuj pokój z tablicy
                int randomIndex = Random.Range(0, roomPrefabs.Length);

                // Stwórz pokój w wyznaczonym miejscu
                Instantiate(roomPrefabs[randomIndex], pos, Quaternion.identity);
            }
        }
    }
}