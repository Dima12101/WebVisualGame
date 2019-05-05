using System;
using System.Collections.Generic;
using System.Text;

namespace GameTextParsing.ParseTreeProcessingHelpers
{
    class IdDictionary<T>
    {
        private Dictionary<T, int> Dict { get; set; }

        private int IdCounter { get; set; }

        public IdDictionary()
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

        public bool Contains(T element)
        {
            return Dict.ContainsKey(element);
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
