using UnityEngine;
using UnityEngine.UI;

namespace Miner.UI 
{
    [ExecuteInEditMode]
    public class FactoryScreen : MonoBehaviour
    {
        [SerializeField] private int _padding = 10;
        [SerializeField] private ScrollRect _verticalScrollRect;

        private int _screenWidth;
        private int _screenHeight;

        private void Start()
        {
            UpdateScreen();
        }

        private void Update()
        {
            if (_screenWidth != Screen.width || _screenHeight != Screen.height) 
            {
                _screenWidth = Screen.width;
                _screenHeight = Screen.height;

                UpdateScreen();
            }
        }

        private void UpdateScreen()
        {
            var mainRectTransform = transform as RectTransform;

            var verticalGroup = GetComponentInChildren<VerticalLayoutGroup>(true);
            verticalGroup.padding.top = _padding;
            verticalGroup.padding.right = _padding;
            verticalGroup.padding.bottom = _padding;
            verticalGroup.padding.left = _padding;
            verticalGroup.spacing = _padding * 2;

            for (var i = 0; i < verticalGroup.transform.childCount; i++)
            {
                if (!verticalGroup.transform.GetChild(i).TryGetComponent<LayoutElement>(out var verticalLayoutElement)) continue;
                verticalLayoutElement.minHeight = mainRectTransform.rect.width - _padding * 2;
                var horizontalGroup = verticalLayoutElement.GetComponentInChildren<HorizontalLayoutGroup>(true);
                horizontalGroup.spacing = _padding * 2;
                for (var j = 0; j < horizontalGroup.transform.childCount; j++) 
                {
                    if (!horizontalGroup.transform.GetChild(j).TryGetComponent<LayoutElement>(out var horizontalLayoutElement)) continue;
                    horizontalLayoutElement.minWidth = mainRectTransform.rect.width - _padding * 2;
                }
            }
        }
    }
}

