using Lean.Pool;
using UnityEngine;

namespace ThisIsBlast.Managers
{
    public static class PoolManager<T> where T : Component
    {
        public static T SpawnObject(T poolObject, Transform parent=null)
        {
            var obj =LeanPool.Spawn(poolObject);
            if (parent != null)
            {
                obj.transform.SetParent(parent);
            }

            return obj;
        }

        public static void DespawnObject(T poolObject)
        {
            LeanPool.Despawn(poolObject);
        }
    }
    
    public static class PoolManager
    {
        public static void DespawnAllObjects()
        {
            LeanPool.DespawnAll();
        }

        public static GameObject Spawn(GameObject particlePrefab, Vector3 pos, Quaternion rot, Transform parent = null)
        {
            var obj =LeanPool.Spawn(particlePrefab,pos,rot,parent);
            return obj;
        }

        public static void Despawn(GameObject poolObject,float duration)
        {
            LeanPool.Despawn(poolObject,duration);
        }
    }
}
