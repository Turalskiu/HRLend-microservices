namespace Helpers.Collections
{
    public static class IEnumerableHelper
    {
        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
        {
            List<T> tempList = new List<T>(source);
            Random random = new Random();
            int n = tempList.Count;

            for (int i = 0; i < n; i++)
            {
                int randomIndex = random.Next(i, n);
                T temp = tempList[i];
                tempList[i] = tempList[randomIndex];
                tempList[randomIndex] = temp;
            }

            return tempList;
        }

    }
}
