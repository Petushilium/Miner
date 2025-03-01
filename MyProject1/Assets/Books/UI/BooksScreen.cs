using UnityEngine;
using UnityEngine.UI;

namespace Books.UI 
{
    [ExecuteInEditMode]
    public class BooksScreen : MonoBehaviour
    {
        [SerializeField] private int _scrollPadding = 10;
        [SerializeField] private Transform _booksScrollTransform;

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
            verticalGroup.padding.top = _scrollPadding;
            verticalGroup.padding.right = _scrollPadding;
            verticalGroup.padding.bottom = _scrollPadding;
            verticalGroup.padding.left = _scrollPadding;
            verticalGroup.spacing = _scrollPadding * 2;

            var verticalLayoutElement = _booksScrollTransform.GetComponent<LayoutElement>();
            verticalLayoutElement.minHeight = mainRectTransform.rect.width - _scrollPadding * 2;
            var horizontalGroup = verticalLayoutElement.GetComponentInChildren<HorizontalLayoutGroup>(true);
            horizontalGroup.spacing = _scrollPadding * 2;
            for (var j = 0; j < horizontalGroup.transform.childCount; j++)
            {
                if (!horizontalGroup.transform.GetChild(j).TryGetComponent<LayoutElement>(out var horizontalLayoutElement)) continue;
                horizontalLayoutElement.minWidth = mainRectTransform.rect.width - _scrollPadding * 2;
            }
        }
    }
}

