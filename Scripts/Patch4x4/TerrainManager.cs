using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class TerrainManager : MonoBehaviour
{
    bool[][] occupied;
    GameObject[] terrain_blocks;
    GameObject Floor4x4;

    void Start()
    {
        occupied = new bool[4][];
        for (int i = 0; i < 4; i++)
        {
            occupied[i] = new bool[4];
        }
        terrain_blocks = new GameObject[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            terrain_blocks[i] = transform.GetChild(i).gameObject;
        }

        Floor4x4 = GameObject.Find("Floor4x4");

        genTerrain();
    }

    public void genTerrain()
    {
        // pick place
        int pick_x = Random.Range(0, 4);
        int pick_z = Random.Range(0, 4);

        // occupy 1~2 in 4 direction
        int surround1 = Random.Range(0, 4);
        int surround2 = Random.Range(0, 4);

        occupied[pick_x][pick_z] = true;
        if (pick_x > 0)
            occupied[pick_x - 1][pick_z] = (surround1 == 0 || surround2 == 0);
        if (pick_z > 0)
            occupied[pick_x][pick_z - 1] = (surround1 == 1 || surround2 == 1);
        if (pick_x < 3)
            occupied[pick_x + 1][pick_z] = (surround1 == 2 || surround2 == 2);
        if (pick_z < 3)
            occupied[pick_x][pick_z + 1] = (surround1 == 3 || surround2 == 3);

        int _count = 0;
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                if (occupied[i][j])
                {
                    terrain_blocks[_count].transform.localPosition = new Vector3(i, 0, j);
                    _count++;
                }
            }
        }
    }
}
