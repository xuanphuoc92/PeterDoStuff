using PeterDoStuff.Extensions;

namespace PeterDoStuff.MudWasmHosted.Client.Extensions
{
    public static class BasicExtensions
    {
        public static void MoveUp<T>(this List<T> list, T item)
        {
            int currentIndex = list.IndexOf(item);
            if (currentIndex <= 0)
                return;

            list.RemoveAt(currentIndex);
            list.Insert(currentIndex - 1, item);
        }

        public static void MoveDown<T>(this List<T> list, T item)
        {
            int currentIndex = list.IndexOf(item);
            if (currentIndex == list.Count - 1)
                return;

            list.RemoveAt(currentIndex);
            list.Insert(currentIndex + 1, item);
        }

        public static string[] ToParagraphs(this string input)
            => input?.Split("\n").Select(p => p.Trim()).Where(p => p.IsNullOrEmpty() == false).ToArray() ?? [];
    }
}
