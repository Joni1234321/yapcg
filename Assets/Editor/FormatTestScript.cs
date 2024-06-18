using System.Globalization;
using UnityEngine;

namespace YAPCG
{
    [ExecuteAlways]
    public class FormatTeest : MonoBehaviour
    {
        public string Format;

        public float[] Values;

        public string[] Outs;

        void Update()
        {
            Outs = new string[Values.Length];
            for (int i = 0; i < Values.Length; i++)
                Outs[i] = Values[i].ToString(Format, CultureInfo.CurrentCulture);
        }
    }
}
