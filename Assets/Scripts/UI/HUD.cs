using System;
using UnityEngine;

namespace YAPCG.UI
{
    public class HUD : MonoBehaviour
    {
        
        

        public static HUD Instance { get; private set; }
        private void Awake()
        {
            if (Instance != null)
                Debug.LogError("Second instance of HUD created");

            Instance = this;
        }
        
        
    }
}