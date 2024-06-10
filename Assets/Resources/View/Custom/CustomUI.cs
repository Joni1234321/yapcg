using UnityEngine.UIElements;

namespace YAPCG.Resources.View.Custom
{
    public abstract class CustomUI : VisualElement
    {
        protected readonly VisualElement Body;
        protected CustomUI(string templateResourcePath)
        {
            Body = UnityEngine.Resources.Load<VisualTreeAsset>(templateResourcePath).CloneTree().hierarchy[0];
            Add(Body);
        }
        
        protected T Q<T>(string name = null, string className = null) where T : VisualElement => Body.Q<T>(name, className);
        protected UQueryBuilder<T> Query<T>(string name = null, string className = null) where T : VisualElement => Body.Query<T>(name, className);

    }
}