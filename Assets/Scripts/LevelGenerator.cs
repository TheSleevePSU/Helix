using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelGenerator : MonoBehaviour
{
    public GameObject wall;
    public GameObject floor;
    public GameObject barrel;
    public GameObject[] enemies;
    public int[] enemyCounts;

    public int levelWidth;
    public int levelHeight;

    public int barrels;

    public float chunkChance;
    int chunkSize = 5;
    private List<int[,]> chunks;
    private List<GameObject> emptyTiles;

    void Start()
    {
        chunks = new List<int[,]>();
        emptyTiles = new List<GameObject>();

        InitializeChunks();
    }

    public void CreateLevel()
    {
        if (GameController.instance != null)
        {
            GameController.instance.ClearLevel();

            ClearExistingLevel();

            ReadLevelGenTable();

            CreateEmptyRoom(0, 0, levelWidth, levelHeight);

            for (int x = 1; x < levelWidth - 1; x += chunkSize)
            {
                for (int y = 1; y < levelHeight - 1; y += chunkSize)
                {
                    if (Random.value < chunkChance)
                        AddChunk(x, y);
                    else
                        AddEmptyChunk(x, y);
                }
            }
            AddPlayer();
            AddEnemies();
            AddBarrels();
        }
    }

    void ClearExistingLevel()
    {
        emptyTiles.Clear();
        SpriteRenderer[] sr = FindObjectsOfType<SpriteRenderer>();
        PlaceSaver[] ps = FindObjectsOfType<PlaceSaver>();
        foreach (SpriteRenderer s in sr)
        {
            if (s.gameObject.GetComponent<Player>() == null)
                Destroy(s.gameObject);
        }
        foreach (PlaceSaver p in ps)
        {
            Destroy(p.gameObject);
        }
    }

    void ReadLevelGenTable()
    {
        switch (GameController.instance.level)
        {
            case 1:
                enemyCounts = new int[8] { 6, 5, 4, 3, 2, 1, 0, 0 };
                break;
            case 2:
                enemyCounts = new int[8] { 5, 4, 3, 2, 1, 0, 0, 0 };
                break;
            case 3:
                enemyCounts = new int[8] { 6, 5, 4, 3, 2, 1, 0, 0 };
                break;
            case 4:
                enemyCounts = new int[8] { 7, 6, 5, 4, 3, 2, 1, 0 };
                break;
            case 5:
                enemyCounts = new int[8] { 8, 7, 6, 5, 4, 3, 2, 1 };
                break;
            default:
                break;
        }
    }

    void AddBarrels()
    {
        emptyTiles.Shuffle();
        for (int b = 0; b < barrels; b++)
        {
            if (emptyTiles.Count > 0)
            {
                Instantiate(barrel, new Vector3(emptyTiles[0].transform.position.x, emptyTiles[0].transform.position.y), Quaternion.identity);
                emptyTiles.RemoveAt(0);
            }
        }
    }

    void AddPlayer()
    {
        emptyTiles.Shuffle();
        Player.instance.transform.position = emptyTiles[0].transform.position;
        emptyTiles.RemoveAt(0);
        Player.instance.GeneratePreviewObjects();
    }

    void AddEnemies()
    {
        emptyTiles.Shuffle();
        for (int e = 0; e < enemies.Length; e++)
        {
            for (int ec = 0; ec < enemyCounts[e]; ec++)
            {
                if (emptyTiles.Count > 0)
                {
                    Instantiate(enemies[e], new Vector3(emptyTiles[0].transform.position.x, emptyTiles[0].transform.position.y), Quaternion.identity);
                    emptyTiles.RemoveAt(0);
                }
            }
        }
    }

    void CreateEmptyRoom(int lowerLeftX, int lowerLeftY, int width, int height)
    {
        for (int x = 0; x < width; x++)
        {
            Instantiate(wall, new Vector3(x, 0), Quaternion.identity);
            Instantiate(wall, new Vector3(x, width - 1), Quaternion.identity);
        }
        for (int y = 1; y < height - 1; y++)
        {
            Instantiate(wall, new Vector3(0, y), Quaternion.identity);
            Instantiate(wall, new Vector3(height - 1, y), Quaternion.identity);
        }
        //for (int x = 1; x < width - 1; x++)
        //{
        //    for (int y = 1; y < height - 1; y++)
        //    {
        //        Instantiate(floor, new Vector3(x, y), Quaternion.identity);
        //    }
        //}
    }

    void AddChunk(int x, int y)
    {
        for (int i = 0; i < chunkSize; i++)
        {
            for (int j = 0; j < chunkSize; j++)
            {
                //Collider2D[] c2d = Physics2D.OverlapPointAll(new Vector2(x + i, y + j));
                //foreach (Collider2D c in c2d)
                //{
                //    if (c.GetComponent<Tile>() != null)
                //}
                if (chunks[0][i, j] == 1)
                {
                    Instantiate(wall, new Vector3(x + i, y + j), Quaternion.identity);
                }
                else
                {
                    GameObject t = Instantiate(floor, new Vector3(x + i, y + j), Quaternion.identity) as GameObject;
                    emptyTiles.Add(t);
                }
            }
        }
        chunks.RemoveAt(0);
    }

    void AddEmptyChunk(int x, int y)
    {
        for (int i = 0; i < chunkSize; i++)
        {
            for (int j = 0; j < chunkSize; j++)
            {
                GameObject t = Instantiate(floor, new Vector3(x + i, y + j), Quaternion.identity) as GameObject;
                emptyTiles.Add(t);
            }
        }
    }

    void InitializeChunks()
    {
        chunks.Clear();

        chunks.Add(new int[5, 5] { { 0, 0, 0, 0, 0 }, { 0, 1, 0, 0, 0 }, { 0, 1, 0, 0, 0 }, { 0, 1, 1, 0, 0 }, { 0, 0, 0, 0, 0 } });
        chunks.Add(new int[5, 5] { { 0, 0, 0, 0, 0 }, { 0, 1, 1, 0, 0 }, { 0, 1, 0, 0, 0 }, { 0, 1, 0, 0, 0 }, { 0, 0, 0, 0, 0 } });
        chunks.Add(new int[5, 5] { { 0, 0, 0, 0, 0 }, { 0, 1, 0, 0, 0 }, { 0, 1, 1, 0, 0 }, { 0, 1, 0, 0, 0 }, { 0, 0, 0, 0, 0 } });
        chunks.Add(new int[5, 5] { { 0, 0, 0, 0, 0 }, { 0, 0, 1, 0, 0 }, { 0, 1, 1, 0, 0 }, { 0, 1, 0, 0, 0 }, { 0, 0, 0, 0, 0 } });
        chunks.Add(new int[5, 5] { { 0, 0, 0, 0, 0 }, { 0, 0, 1, 0, 0 }, { 0, 0, 1, 1, 0 }, { 0, 0, 0, 1, 0 }, { 0, 0, 0, 0, 0 } });
        chunks.Add(new int[5, 5] { { 0, 0, 0, 0, 0 }, { 0, 0, 0, 1, 0 }, { 0, 0, 1, 1, 0 }, { 0, 0, 1, 0, 0 }, { 0, 0, 0, 0, 0 } });
        chunks.Add(new int[5, 5] { { 0, 1, 0, 1, 0 }, { 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0 }, { 0, 1, 0, 0, 0 }, { 0, 0, 0, 0, 0 } });
        chunks.Add(new int[5, 5] { { 0, 0, 1, 0, 0 }, { 0, 1, 1, 0, 1 }, { 0, 0, 1, 0, 0 }, { 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0 } });
        chunks.Add(new int[5, 5] { { 1, 0, 0, 0, 0 }, { 0, 0, 0, 1, 0 }, { 0, 0, 1, 0, 0 }, { 0, 0, 0, 0, 0 }, { 1, 0, 0, 0, 1 } });
        //chunks.Add(new int[5, 5] { { 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0 } });

        chunks.Add(RotateByNinety(new int[5, 5] { { 0, 0, 0, 0, 0 }, { 0, 1, 0, 0, 0 }, { 0, 1, 0, 0, 0 }, { 0, 1, 1, 0, 0 }, { 0, 0, 0, 0, 0 } }));
        chunks.Add(RotateByNinety(new int[5, 5] { { 0, 0, 0, 0, 0 }, { 0, 1, 1, 0, 0 }, { 0, 1, 0, 0, 0 }, { 0, 1, 0, 0, 0 }, { 0, 0, 0, 0, 0 } }));
        chunks.Add(RotateByNinety(new int[5, 5] { { 0, 0, 0, 0, 0 }, { 0, 1, 0, 0, 0 }, { 0, 1, 1, 0, 0 }, { 0, 1, 0, 0, 0 }, { 0, 0, 0, 0, 0 } }));
        chunks.Add(RotateByNinety(new int[5, 5] { { 0, 0, 0, 0, 0 }, { 0, 0, 1, 0, 0 }, { 0, 1, 1, 0, 0 }, { 0, 1, 0, 0, 0 }, { 0, 0, 0, 0, 0 } }));
        chunks.Add(RotateByNinety(new int[5, 5] { { 0, 0, 0, 0, 0 }, { 0, 0, 1, 0, 0 }, { 0, 0, 1, 1, 0 }, { 0, 0, 0, 1, 0 }, { 0, 0, 0, 0, 0 } }));
        chunks.Add(RotateByNinety(new int[5, 5] { { 0, 0, 0, 0, 0 }, { 0, 0, 0, 1, 0 }, { 0, 0, 1, 1, 0 }, { 0, 0, 1, 0, 0 }, { 0, 0, 0, 0, 0 } }));
        chunks.Add(RotateByNinety(new int[5, 5] { { 0, 1, 0, 1, 0 }, { 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0 }, { 0, 1, 0, 0, 0 }, { 0, 0, 0, 0, 0 } }));
        chunks.Add(RotateByNinety(new int[5, 5] { { 0, 0, 1, 0, 0 }, { 0, 1, 1, 0, 1 }, { 0, 0, 1, 0, 0 }, { 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0 } }));
        chunks.Add(RotateByNinety(new int[5, 5] { { 1, 0, 0, 0, 0 }, { 0, 0, 0, 1, 0 }, { 0, 0, 1, 0, 0 }, { 0, 0, 0, 0, 0 }, { 1, 0, 0, 0, 1 } }));

        chunks.Add(RotateByNinety(RotateByNinety(new int[5, 5] { { 0, 0, 0, 0, 0 }, { 0, 1, 0, 0, 0 }, { 0, 1, 0, 0, 0 }, { 0, 1, 1, 0, 0 }, { 0, 0, 0, 0, 0 } })));
        chunks.Add(RotateByNinety(RotateByNinety(new int[5, 5] { { 0, 0, 0, 0, 0 }, { 0, 1, 1, 0, 0 }, { 0, 1, 0, 0, 0 }, { 0, 1, 0, 0, 0 }, { 0, 0, 0, 0, 0 } })));
        chunks.Add(RotateByNinety(RotateByNinety(new int[5, 5] { { 0, 0, 0, 0, 0 }, { 0, 1, 0, 0, 0 }, { 0, 1, 1, 0, 0 }, { 0, 1, 0, 0, 0 }, { 0, 0, 0, 0, 0 } })));
        chunks.Add(RotateByNinety(RotateByNinety(new int[5, 5] { { 0, 0, 0, 0, 0 }, { 0, 0, 1, 0, 0 }, { 0, 1, 1, 0, 0 }, { 0, 1, 0, 0, 0 }, { 0, 0, 0, 0, 0 } })));
        chunks.Add(RotateByNinety(RotateByNinety(new int[5, 5] { { 0, 0, 0, 0, 0 }, { 0, 0, 1, 0, 0 }, { 0, 0, 1, 1, 0 }, { 0, 0, 0, 1, 0 }, { 0, 0, 0, 0, 0 } })));
        chunks.Add(RotateByNinety(RotateByNinety(new int[5, 5] { { 0, 0, 0, 0, 0 }, { 0, 0, 0, 1, 0 }, { 0, 0, 1, 1, 0 }, { 0, 0, 1, 0, 0 }, { 0, 0, 0, 0, 0 } })));
        chunks.Add(RotateByNinety(RotateByNinety(new int[5, 5] { { 0, 1, 0, 1, 0 }, { 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0 }, { 0, 1, 0, 0, 0 }, { 0, 0, 0, 0, 0 } })));
        chunks.Add(RotateByNinety(RotateByNinety(new int[5, 5] { { 0, 0, 1, 0, 0 }, { 0, 1, 1, 0, 1 }, { 0, 0, 1, 0, 0 }, { 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0 } })));
        chunks.Add(RotateByNinety(RotateByNinety(new int[5, 5] { { 1, 0, 0, 0, 0 }, { 0, 0, 0, 1, 0 }, { 0, 0, 1, 0, 0 }, { 0, 0, 0, 0, 0 }, { 1, 0, 0, 0, 1 } })));

        chunks.Add(RotateByNinety(RotateByNinety(RotateByNinety(new int[5, 5] { { 0, 0, 0, 0, 0 }, { 0, 1, 0, 0, 0 }, { 0, 1, 0, 0, 0 }, { 0, 1, 1, 0, 0 }, { 0, 0, 0, 0, 0 } }))));
        chunks.Add(RotateByNinety(RotateByNinety(RotateByNinety(new int[5, 5] { { 0, 0, 0, 0, 0 }, { 0, 1, 1, 0, 0 }, { 0, 1, 0, 0, 0 }, { 0, 1, 0, 0, 0 }, { 0, 0, 0, 0, 0 } }))));
        chunks.Add(RotateByNinety(RotateByNinety(RotateByNinety(new int[5, 5] { { 0, 0, 0, 0, 0 }, { 0, 1, 0, 0, 0 }, { 0, 1, 1, 0, 0 }, { 0, 1, 0, 0, 0 }, { 0, 0, 0, 0, 0 } }))));
        chunks.Add(RotateByNinety(RotateByNinety(RotateByNinety(new int[5, 5] { { 0, 0, 0, 0, 0 }, { 0, 0, 1, 0, 0 }, { 0, 1, 1, 0, 0 }, { 0, 1, 0, 0, 0 }, { 0, 0, 0, 0, 0 } }))));
        chunks.Add(RotateByNinety(RotateByNinety(RotateByNinety(new int[5, 5] { { 0, 0, 0, 0, 0 }, { 0, 0, 1, 0, 0 }, { 0, 0, 1, 1, 0 }, { 0, 0, 0, 1, 0 }, { 0, 0, 0, 0, 0 } }))));
        chunks.Add(RotateByNinety(RotateByNinety(RotateByNinety(new int[5, 5] { { 0, 0, 0, 0, 0 }, { 0, 0, 0, 1, 0 }, { 0, 0, 1, 1, 0 }, { 0, 0, 1, 0, 0 }, { 0, 0, 0, 0, 0 } }))));
        chunks.Add(RotateByNinety(RotateByNinety(RotateByNinety(new int[5, 5] { { 0, 1, 0, 1, 0 }, { 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0 }, { 0, 1, 0, 0, 0 }, { 0, 0, 0, 0, 0 } }))));
        chunks.Add(RotateByNinety(RotateByNinety(RotateByNinety(new int[5, 5] { { 0, 0, 1, 0, 0 }, { 0, 1, 1, 0, 1 }, { 0, 0, 1, 0, 0 }, { 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0 } }))));
        chunks.Add(RotateByNinety(RotateByNinety(RotateByNinety(new int[5, 5] { { 1, 0, 0, 0, 0 }, { 0, 0, 0, 1, 0 }, { 0, 0, 1, 0, 0 }, { 0, 0, 0, 0, 0 }, { 1, 0, 0, 0, 1 } }))));

        chunks.Shuffle();
    }

    int[,] RotateByNinety(int[,] arr)
    {
        int i, j;
        int n = chunkSize;

        for (i = 0; i < n; i++)
        {
            for (j = i; j < n; j++)
            {
                if (i != j)
                {
                    arr[i, j] ^= arr[j, i];
                    arr[j, i] ^= arr[i, j];
                    arr[i, j] ^= arr[j, i];
                }
            }
        }
        for (i = 0; i < n / 2; i++)
        {
            for (j = 0; j < n; j++)
            {
                arr[j, i] ^= arr[j, n - 1 - i];
                arr[j, n - 1 - i] ^= arr[j, i];
                arr[j, i] ^= arr[j, n - 1 - i];
            }
        }

        return arr;
    }
}
