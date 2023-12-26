using Unity.Entities;
using UnityEngine;

namespace YAPCG.UI
{
    public class MainUIText : MonoBehaviour
    {
        private class MainUITextBaker : Baker<MainUIText>
        {
            public override void Bake(MainUIText authoring)
            {
            }
        }
    }
}