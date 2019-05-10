using System;
using System.Collections.Generic;
using System.Text;

namespace GameTextParsing.ParseTreeProcessingHelpers
{
    class IdMaker<T>
    {
        private Dictionary<T, int> Dict { get; set; }

        private int IdCounter { get; set; }

        public IdMaker()
        {
            Dict = new Dictionary<T, int>();
            
            IdCounter = 0;
        }

        public int GetId(T element)
        {
            bool contains = Dict.TryGetValue(element, out int id);

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
