using Shared.Disposable;
using Shared.Reactive;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Books.UI 
{
    public sealed class BooksScreen
    {
        public class Entity : BaseDisposable
        {
            private class Logic : BaseDisposable
            {
                public struct Ctx
                {
                    public IReadOnlyReactiveCommand<float> OnUpdate;
                    public Data Data;
                }

                private readonly Ctx _ctx;

                private int _screenWidth;
                private int _screenHeight;

                public Logic(Ctx ctx)
                {
                    _ctx = ctx;

                    _ctx.OnUpdate.Subscribe(deltaTime => 
                    {
                        if (_screenWidth != Screen.width || _screenHeight != Screen.height)
                        {
                            _screenWidth = Screen.width;
                            _screenHeight = Screen.height;

                            UpdateScreen();
                        }
                    }).AddTo(this);

                    UpdateScreen();
                }

                public void UpdateScreen()
                {
                    var verticalGroup = _ctx.Data.RootTransform.GetComponentInChildren<VerticalLayoutGroup>(true);
                    verticalGroup.padding.top = _ctx.Data.ScrollPadding;
                    verticalGroup.padding.right = _ctx.Data.ScrollPadding;
                    verticalGroup.padding.bottom = _ctx.Data.ScrollPadding;
                    verticalGroup.padding.left = _ctx.Data.ScrollPadding;
                    verticalGroup.spacing = _ctx.Data.ScrollPadding;

                    _ctx.Data.BookScrollLayout.minHeight = _ctx.Data.RootTransform.rect.width - _ctx.Data.ScrollPadding * 2;
                    var horizontalGroup = _ctx.Data.BookScrollLayout.GetComponentInChildren<HorizontalLayoutGroup>(true);
                    horizontalGroup.spacing = _ctx.Data.ScrollPadding * 2;
                    for (var j = 0; j < horizontalGroup.transform.childCount; j++)
                    {
                        if (!horizontalGroup.transform.GetChild(j).TryGetComponent<LayoutElement>(out var horizontalLayoutElement)) continue;
                        horizontalLayoutElement.minWidth = _ctx.Data.RootTransform.rect.width - _ctx.Data.ScrollPadding * 2;
                    }

                    _ctx.Data.BookLayout.padding.top = _ctx.Data.ScrollPadding;
                    _ctx.Data.BookLayout.padding.right = _ctx.Data.ScrollPadding;
                    _ctx.Data.BookLayout.padding.bottom = _ctx.Data.ScrollPadding;
                    _ctx.Data.BookLayout.padding.left = _ctx.Data.ScrollPadding;
                    _ctx.Data.BookLayout.spacing = Vector2.one * _ctx.Data.ScrollPadding;

                    _ctx.Data.BookLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;

                    var bookSize = _ctx.Data.RootTransform.rect.width / _ctx.Data.BookLayout.constraintCount;
                    bookSize -= (_ctx.Data.ScrollPadding * (_ctx.Data.BookLayout.constraintCount + 1)) / _ctx.Data.BookLayout.constraintCount;
                    _ctx.Data.BookLayout.cellSize = Vector2.one * bookSize;
                }

                public void AddBook(Texture mainImage, string title, List<string> genres, string description)
                {
                    _ctx.Data.BookScrollUnit.gameObject.SetActive(false);
                    _ctx.Data.BookUnit.gameObject.SetActive(false);

                    var bookScrollUnit = UnityEngine.Object.Instantiate(_ctx.Data.BookScrollUnit, _ctx.Data.BookScrollUnit.transform.parent);
                    bookScrollUnit.gameObject.SetActive(true);
                    bookScrollUnit.SetData(mainImage, title, genres, description);

                    var bookUnit = UnityEngine.Object.Instantiate(_ctx.Data.BookUnit, _ctx.Data.BookUnit.transform.parent);
                    bookUnit.gameObject.SetActive(true);
                    bookUnit.SetData(mainImage, title, genres, description);

                    UpdateScreen();
                }
            }

            public struct Ctx
            {
                public IReadOnlyReactiveCommand<float> OnUpdate;
                public Data Data;
            }

            private readonly Ctx _ctx;

            private Logic _logic;

            public Entity(Ctx ctx)
            {
                _ctx = ctx;

                _logic = new Logic(new Logic.Ctx
                {
                    OnUpdate = _ctx.OnUpdate,
                    Data = _ctx.Data,
                }).AddTo(this);
            }

            public void UpdateScreen() => _logic.UpdateScreen();
            public void AddBook(Texture mainImage, string title, List<string> genres, string description) => _logic.AddBook(mainImage, title, genres, description);
        }

        [Serializable]
        public struct Data
        {
            [SerializeField] private RectTransform _rootTransform;

            [SerializeField] private int _scrollPadding;
            [SerializeField] private LayoutElement _booksScrollLayout;
            [SerializeField] private GridLayoutGroup _booksLayout;

            [SerializeField] private BookUnit _bookScrollUnit;
            [SerializeField] private BookUnit _bookUnit;

            public readonly RectTransform RootTransform => _rootTransform;

            public readonly int ScrollPadding => _scrollPadding;
            public readonly LayoutElement BookScrollLayout => _booksScrollLayout;
            public readonly GridLayoutGroup BookLayout => _booksLayout;

            public readonly BookUnit BookScrollUnit => _bookScrollUnit;
            public readonly BookUnit BookUnit => _bookUnit;
        }
    }
}

