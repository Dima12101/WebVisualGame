using System;
using System.Collections.Generic;
using System.Text;

namespace GameTextParsing
{
    class IdentifierDictionary<T>
    {
        private Dictionary<T, int> Dict { get; set; }

        private int IdCounter { get; set; }

        public IdentifierDictionary()
        {
            Dict = new Dictionary<T, int>();

            IdCounter = 0;
        }

        public int Add(T element, out bool contains)
        {
            contains = Dict.TryGetValue(element, out int id);

            if (!contains)
            {
                id = ++IdCounter;

                Dict.Add(element, id);
            }

            return id;
        }

        public int UniqueID()
        {
            return ++IdCounter;
        }

        public bool TryGetId(T element, out int id)
        {
            return Dict.TryGetValue(element, out id);
        }
    }

}
