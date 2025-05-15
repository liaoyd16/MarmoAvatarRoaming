
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.VisualScripting;
using UnityEngine;

public class PatchInfoBuffer
{
    static int MAX_OBJ_COUNT = 512;
    public string[] gameobj_list { get; private set; }
    public int idx { get; private set; }
    public PatchInfoBuffer()
    {
        gameobj_list = new string[MAX_OBJ_COUNT];
        idx = 0;
    }

    public void doFill(string objname)
    {
        try
        {
            gameobj_list[idx] = objname;
            idx++;
        }
        catch
        {
            UnityEngine.Debug.Log("index out of range");
        }
        
    }

    public void doEmpty()
    {
        idx = 0;
    }
}

public class MapManager
{
    // how big is a patch
    public float patch_wid { get;  private set; }

    // a buffer to write in: 
    // what objects are to be instantiated here?
    public PatchInfoBuffer patch_info;

    public MapManager(float wid)
    {
        patch_wid = wid;
        patch_info = new PatchInfoBuffer();
    }

    public void getPatch(int iz, int ix)
    {
        patch_info.doEmpty();
        patch_info.doFill("tree"); // debug: simple case
    }

    public void patchIndexOf(float z, float x, out int iz, out int ix)
    {
        iz = Mathf.FloorToInt(z / patch_wid);
        ix = Mathf.FloorToInt(x / patch_wid);
    }
}