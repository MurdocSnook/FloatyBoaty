using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils {
    public class Option<T>
    {
        public T content = default(T);
        public float weight = 1;
    }

    [System.Serializable]
    public class SpawnOption : Option<GameObject> {
    }

	public static T Choose<T>(Option<T>[] spawnOptions) {
        float weightSum = 0;
        foreach (Option<T> t in spawnOptions)
        {
            weightSum += t.weight;
        }
        float choice = UnityEngine.Random.Range(0f, weightSum);
        for (int i = 0; i < spawnOptions.Length; i++)
        {
            choice -= spawnOptions[i].weight;
            if (choice <= 0)
            {
                return spawnOptions[i].content;
            }
        }
        return spawnOptions[spawnOptions.Length - 1].content;
	}
}
