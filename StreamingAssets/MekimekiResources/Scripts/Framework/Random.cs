    using System.Collections.Generic;

    public static class Random
    {
        private static System.Random _random = new System.Random();

        public static int Next(int includeMin, int excludeMax)
        {
            return _random.Next(includeMin, excludeMax);
        }

        public static T Get<T>(List<T> list)
        {
            return list[Next(0, list.Count)];
        }
    }
