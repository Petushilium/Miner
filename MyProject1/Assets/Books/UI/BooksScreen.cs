using UnityEngine;
using UnityEngine.UI;

namespace Books.UI 
{
    [ExecuteInEditMode]
    public class BooksScreen : MonoBehaviour
    {
        [SerializeField] private int _scrollPadding = 10;
        [SerializeField] private LayoutElement _booksScrollLayout;
        [SerializeField] private GridLayoutGroup _booksLayout;

        [SerializeField] private BookUnit _bookScrollUnit;
        [SerializeField] private BookUnit _bookUnit;

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
            verticalGroup.spacing = _scrollPadding;

            _booksScrollLayout.minHeight = mainRectTransform.rect.width - _scrollPadding * 2;
            var horizontalGroup = _booksScrollLayout.GetComponentInChildren<HorizontalLayoutGroup>(true);
            horizontalGroup.spacing = _scrollPadding * 2;
            for (var j = 0; j < horizontalGroup.transform.childCount; j++)
            {
                if (!horizontalGroup.transform.GetChild(j).TryGetComponent<LayoutElement>(out var horizontalLayoutElement)) continue;
                horizontalLayoutElement.minWidth = mainRectTransform.rect.width - _scrollPadding * 2;
            }

            _booksLayout.padding.top = _scrollPadding;
            _booksLayout.padding.right = _scrollPadding;
            _booksLayout.padding.bottom = _scrollPadding;
            _booksLayout.padding.left = _scrollPadding;
            _booksLayout.spacing = Vector2.one * _scrollPadding;

            _booksLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;

            var bookSize = mainRectTransform.rect.width / _booksLayout.constraintCount;
            bookSize -= (_scrollPadding * (_booksLayout.constraintCount + 1)) / _booksLayout.constraintCount;
            _booksLayout.cellSize = Vector2.one * bookSize;
        }

        public void AddBook(Texture mainImage, string title, string[] genres, string description) 
        {
            _bookScrollUnit.gameObject.SetActive(false);
            _bookUnit.gameObject.SetActive(false);

            var bookScrollUnit = Instantiate(_bookScrollUnit, _bookScrollUnit.transform.parent);
            bookScrollUnit.gameObject.SetActive(true);
            bookScrollUnit.SetData(mainImage, title, genres, description);

            var bookUnit = Instantiate(_bookUnit, _bookUnit.transform.parent);
            bookUnit.gameObject.SetActive(true);
            bookUnit.SetData(mainImage, title, genres, description);

            UpdateScreen();
        }
    }
}

