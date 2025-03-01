using System;
using UnityEngine;

namespace Books.ScriptableObjects
{
    [CreateAssetMenu(fileName = "Books", menuName = "Scriptable Objects/Books")]
    public class Books : ScriptableObject
    {
        [Serializable]
        public struct Book 
        {
            [SerializeField] private string _title;
            [SerializeField] private Texture _mainImage;
            [SerializeField] private string[] _genres;
            [TextArea(5, 15)]
            [SerializeField] private string _description;

            public readonly string Title => _title;
            public readonly Texture MainImage => _mainImage;
            public readonly string[] Genres => _genres;
            public readonly string Description => _description;
        }

        [SerializeField] private Book[] _books;
        public Book[] BooksData => _books;
    }
}

