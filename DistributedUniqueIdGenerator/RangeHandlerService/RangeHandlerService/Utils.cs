using System.Collections.Concurrent;

namespace RangeHandlerService
{
    public static class Utils
    {
        private static object _obj = new object();
        private static ConcurrentQueue<Tuple<int, int>> _queue = new ConcurrentQueue<Tuple<int, int>>();
        private static int _prevrangeLimit = 0;
        private static int _rangeLimitEnd = 0;
        public static void LoadRanges(int maxRangeLimit, int rangeSplit)
        {
            int start = 0, end = _prevrangeLimit;
            while (end <= maxRangeLimit)
            {
                start = end + 1;
                end += rangeSplit;
                _queue.Enqueue(Tuple.Create(start, end));
            }
        }

        public static Tuple<int, int> getCurrentRange(int upperRangeLimit, int rangeSplit)
        {
            Tuple<int, int> result;

            //Duplicate results occurred during concurrent testing, so a lock was implemented for thread safety.
            lock (_obj)
            {
                if (_queue.Count == 0)
                {
                    _prevrangeLimit = _rangeLimitEnd;
                    _rangeLimitEnd += upperRangeLimit;
                    LoadRanges(_rangeLimitEnd, rangeSplit);
                }

                bool isRemoved = _queue.TryDequeue(out result);
            }

            return result is null ? Tuple.Create(0, 0) : result;
        }

    }
}
