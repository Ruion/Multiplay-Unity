using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class PreparePool : MonoBehaviour
{
    public List<GameObject> Prefabs;

    private void Start()
    {
        DefaultPool pool = PhotonNetwork.PrefabPool as DefaultPool;
        if (pool != null && this.Prefabs != null)
        {
            foreach (GameObject prefab in this.Prefabs)
            {
                if (!pool.ResourceCache.ContainsKey(prefab.name))
                    pool.ResourceCache.Add(prefab.name, prefab);
            }
        }
    }
}