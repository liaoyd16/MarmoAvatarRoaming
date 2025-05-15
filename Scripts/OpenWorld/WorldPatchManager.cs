using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class WorldPatchManager : MonoBehaviour
{
    // own a map first
    MapManager m_map;
    [SerializeField] float patch_width = 4;
    [SerializeField] int padding = 1;
    [SerializeField] float re_origin_thres = 100;

    // secondly, make a torus
    Torus<GameObject> active_patch;
    [SerializeField] Vector3 active_patch_pos;

    [SerializeField] GameObject agent;
    // wherever agent goes, render around agent
    // if agent is too far away from origin, reset origin
    public Vector3 offset_in_map { get;  private set; }

    // object pool
    ObjectPool<GameObject> patch4x4_pool;

    void Start()
    {
        m_map = new MapManager(patch_width);
        offset_in_map = Vector3.zero;

        // initialize pool
        patch4x4_pool = new ObjectPool<GameObject>(createFunc, actionOnRelease: actionOnRelease);

        // populate active_patch
        active_patch = new Torus<GameObject>(padding);
        populateAll();
    }

    void Update() {
        active_patch_pos = active_patch.get(0,0).transform.position;
        if (agent.transform.position.magnitude > re_origin_thres)
            reOrigin();
    }
    
    void LateUpdate()
    {
        // need to load new patches?
        Vector3 agent_leave = (agent.transform.position
             - active_patch.get(0, 0).transform.position) / m_map.patch_wid;

        if (agent_leave.x > 1f) //move right
        {
            patchRight();
        }
        else if (agent_leave.x < 0f)
        {
            patchLeft();
        }
        if (agent_leave.z > 1f)
        {
            patchForward();
        }
        else if (agent_leave.z < 0f)
        {
            patchBack();
        }
    }

    GameObject createFunc()
    {
        GameObject p = Instantiate(Resources.Load<GameObject>("Prefabs/Patch4x4"), transform.position + Vector3.down * 100, Quaternion.identity, this.transform);
        return p;
    }

    void actionOnRelease(GameObject p)
    {
        p.SetActive(false);
        p.transform.SetLocalPositionAndRotation(
            transform.position + Vector3.down * 100, Quaternion.identity);
    }

    void reOrigin()
    {
        offset_in_map += Vector3.right * agent.transform.position.x +
                         Vector3.forward * agent.transform.position.z; // no up-down allowed

        // minus agent transform
        for (int i = -active_patch.padding; i < active_patch.padding; i++)
        {
            for (int j = -active_patch.padding; j < active_patch.padding; j++)
            {
                active_patch.get(i, j).transform.position
                    -= agent.transform.position;
            }
        }

        agent.transform.position = Vector3.zero;
    }

    void fromMapPopulateAt(int ixmap, int izmap, int ix, int iz)
    {
        // access map[ixmap][izmap]
        m_map.getPatch(ixmap, izmap);

        // get a patch here
        GameObject patch_floor = patch4x4_pool.Get();

        patch_floor.transform.localPosition =
            Vector3.right * ixmap * m_map.patch_wid +
            Vector3.forward * izmap * m_map.patch_wid -
            offset_in_map;

        // if map says certain object are on the floor
        Debug.LogFormat("at patch {0},{1} there are {2} trees",
                ixmap, izmap, m_map.patch_info.idx); // totodo: attach a child object to patch

        // now, add patch_floor to active patch
        // but first do release job?
        if (active_patch.get(ix, iz))
            patch4x4_pool.Release(active_patch.get(ix, iz));

        // attach to active_patch[pad-1][j]
        patch_floor.SetActive(true);
        active_patch.set(ix, iz, patch_floor);
    }

    void populateAll()
    {
        int ixmap, izmap;
        m_map.patchIndexOf(agent.transform.position.x, agent.transform.position.z, out ixmap, out izmap);

        for (int ix = -active_patch.padding; ix < active_patch.padding; ix ++) {
            for (int iz = -active_patch.padding; iz < active_patch.padding; iz++)
            {
                fromMapPopulateAt(ixmap+ix, izmap+iz, ix, iz);
            }
        }
    }

    void patchRight()
    {
        // get patch info of rightmost colomn
        GameObject rightmost_patch = active_patch.get(active_patch.padding - 1, 0);
        Vector3 rightmost_mappos = rightmost_patch.transform.position + offset_in_map;
        int ixmap, izmap;
        m_map.patchIndexOf(rightmost_mappos.x, rightmost_mappos.z, out ixmap, out izmap);

        // patches roll right
        active_patch.rollRight();

        // instantiate the new rightmost column
        for (int j = -active_patch.padding; j < active_patch.padding; j++)
        {
            fromMapPopulateAt(ixmap+1, izmap+j, active_patch.padding-1, j);
        }
    }

    void patchLeft()
    {
        // get patch info of leftmost colomn
        GameObject leftmost_patch = active_patch.get(-active_patch.padding, 0);
        Vector3 leftmost_mappos = leftmost_patch.transform.position + offset_in_map;
        int ixmap, izmap;
        m_map.patchIndexOf(leftmost_mappos.x, leftmost_mappos.z, out ixmap, out izmap);

        // patches roll left
        active_patch.rollLeft();

        // instantiate the new leftmost column
        for (int j = -active_patch.padding; j < active_patch.padding; j++)
        {
            fromMapPopulateAt(ixmap - 1, izmap + j, -active_patch.padding, j);
        }
    }

    void patchForward()
    {
        // get patch info of forwardmost colomn
        GameObject fwdmost_patch = active_patch.get(0, active_patch.padding-1);
        Vector3 fwdmost_mappos = fwdmost_patch.transform.position + offset_in_map;
        int ixmap, izmap;
        m_map.patchIndexOf(fwdmost_mappos.x, fwdmost_mappos.z, out ixmap, out izmap);

        // patches roll forward
        active_patch.rollForward();

        // instantiate the new fwdmost column
        for (int i = -active_patch.padding; i < active_patch.padding; i++)
        {
            fromMapPopulateAt(ixmap + i, izmap + 1, i, active_patch.padding-1);
        }
    }

    void patchBack()
    {
        // get patch info of backmost colomn
        GameObject backmost_patch = active_patch.get(0, -active_patch.padding);
        Vector3 backmost_mappos = backmost_patch.transform.position + offset_in_map;
        int ixmap, izmap;
        m_map.patchIndexOf(backmost_mappos.x, backmost_mappos.z, out ixmap, out izmap);

        // patches roll back
        active_patch.rollBack();

        // instantiate the new backmost column
        for (int i = -active_patch.padding; i < active_patch.padding; i++)
        {
            fromMapPopulateAt(ixmap + i, izmap - 1, i, -active_patch.padding);
        }
    }
}