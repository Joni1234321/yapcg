using UnityEngine;
using UnityEngine.UIElements;

namespace YAPCG
{
    public class SimulationScript : MonoBehaviour
    {
        public UIDocument UIDocument;
        public MultiColumnListView Table;         
        void OnEnable()
        {
            Table = UIDocument.rootVisualElement.Q<MultiColumnListView>("playground-simulation-table");
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
