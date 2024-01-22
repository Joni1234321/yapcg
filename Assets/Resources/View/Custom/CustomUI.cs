using UnityEngine.UIElements;

namespace YAPCG.Resources.View.Custom
{
    public abstract class CustomUI : VisualElement
    {
        private readonly VisualElement _body;
        protected CustomUI(string templateResourcePath)
        {
            _body = UnityEngine.Resources.Load<VisualTreeAsset>(templateResourcePath).CloneTree();
            Add(_body);
        }

        protected T Q<T>(string name = null, string className = null) where T : VisualElement => _body.Q<T>(name, className);
        protected UQueryBuilder<T> Query<T>(string name = null, string className = null) where T : VisualElement => _body.Query<T>(name, className);

    }
}