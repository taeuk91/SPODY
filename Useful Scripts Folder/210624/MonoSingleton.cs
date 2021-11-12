using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Util 
{
    public class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
    {

        #region Singleton Codes..

        protected static T instance = null;
        public static T Instance
        {
            get
            {
                return instance;
            }
        }

        protected virtual void Awake()
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType(typeof(T)) as T;
            }
        }

        #endregion
    }
}



