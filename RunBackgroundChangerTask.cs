using System;
using System.Threading;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.IO;

namespace BackgroundChanger
{
    class RunBackgroundChangerTask
    {
        public const int MIN_TIME = 1800 * 1000; // 30 minutes in seconds converted to milliseconds
        public const int MAX_TIME = 172800 * 1000; // 2 days in seconds converted into milliseconds

        public static void Run(int nextInterval)
        {
            CancellationTokenSource source = new CancellationTokenSource();

            var t = Task.Run(async delegate
            {
                await Task.Delay(nextInterval, source.Token);

                GetImageUrl();

                Random rnd = new Random();
                int nextBackgroundChange = rnd.Next(MIN_TIME, MAX_TIME);
                Run(nextBackgroundChange);
            });

            try
            {
                t.Wait();
            }
            catch (AggregateException ae)
            {
                source.Cancel();
            }

            source.Dispose();
        }

        private static string ToApplicationPath(string fileName)
        {
            var exePath = Path.GetDirectoryName(System.Reflection
                                .Assembly.GetExecutingAssembly().CodeBase);

            Regex appPathMatcher = new Regex(@"(?<!fil)[A-Za-z]:\\+[\S\s]*?(?=\\+bin)");
            var appRoot = appPathMatcher.Match(exePath).Value;
            return Path.Combine(appRoot, fileName);
        }

        private static string[] GetImageUrl()
        {
            string[] imageURLs = Directory.GetFiles(ToApplicationPath("images"));

            Random rand = new Random();
            int index = rand.Next(imageURLs.Length);

            SetBackgroundImage.Set(imageURLs[index]);

            return imageURLs;
        }
    }
}
