namespace AppServer3
{
    public class SequenceGenerator
    {
        private int _currentId; //This holds the next Id that could be utilized by the upcoming request
        private Tuple<int, int>? _currentRange; //This holds the current range for generating sequence IDs per HTTP request
        private object _obj = new object();

        public async Task<IResult> getSequenceId(string url)
        {
            int result = -1;

            //Obtain a range at the process start or when the current range is depleted.
            if (_currentId == 0 || _currentId == _currentRange?.Item2)
                await PullRangeFromRangeHandler(url);

            lock (_obj)
            {
                result = _currentId++;
            }
            return result == -1 ? TypedResults.NotFound(result) : TypedResults.Ok(result);
        }

        private async Task PullRangeFromRangeHandler(string url)
        {
            var client = new HttpClient();
            var response = await client.GetAsync(url);
            string res = await response.Content.ReadAsStringAsync();
            _currentRange = DeserializeStringToTuple(res);
            _currentId = _currentRange.Item1;
        }

        private Tuple<int, int> DeserializeStringToTuple(string apiResult)
        {
            string[] strings = apiResult.Split(",");
            int rangeStart = Convert.ToInt32(strings[0].Split(':')[1]);
            int rangeEnd = Convert.ToInt32(strings[1].Split(":")[1].Replace("}", string.Empty));
            return Tuple.Create(rangeStart, rangeEnd);
        }
    }
}
