using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Extentions
{
    public static class ExtentionsMethods
    {
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> collection)
        {
            return collection == null || collection.Count() == 0;
        }

        public static bool IsAllFalse(this bool[,] collection)
        {
            if(collection == null)
				return false;

            foreach (var item in collection)
            {
                if(item) return false;
            }

            return true;
        }

        public static void StopWaitAndDo(this MonoBehaviour mono)
        {
            if(mono) 
            {
                mono.StopAllCoroutines();
                Debug.LogFormat("<color=olive><size=15><b>{0}</b></size></color>", "will breake coroutine");
            } 
        }

        public static void WaitAndDo(this MonoBehaviour mono ,float time, Action action)
        {
            if(mono)
				mono.StartCoroutine(CoroutineWaitAndDo(time,action));   
        }

        private static IEnumerator CoroutineWaitAndDo(float time, Action action)
        {
            yield return
				new WaitForSecondsRealtime(time);
            if(action != null)
				action();
        }
    }
}

