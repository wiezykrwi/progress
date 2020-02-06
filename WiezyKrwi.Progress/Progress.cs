using System;
using System.Diagnostics;
using System.Threading;

namespace WiezyKrwi.Progress
{
    public class Progress : IDisposable
    {
        private readonly int _totalCount;
        private readonly Timer _timer;
        private readonly Stopwatch _stopwatch;

        private int _count;
        private int _skipCount;

        public Progress(int totalCount, int timerMillis = 1000)
        {
            _totalCount = totalCount;

            _timer = new Timer(WriteProgress, null, timerMillis, timerMillis);
            _stopwatch = Stopwatch.StartNew();

            _count = 0;
        }

        private void WriteProgress(object state)
        {
            string targetTime;
            
            if (_count <= 100)
            {
                targetTime = "";
            }
            else
            {
                var calculatedTotalTime = new TimeSpan(_stopwatch.ElapsedTicks / _count * (_totalCount - _skipCount));
                if (calculatedTotalTime.Days > 0)
                {
                    targetTime = $"/ {calculatedTotalTime:d\\.hh\\:mm\\:ss}";
                }
                else if (calculatedTotalTime.Hours > 0)
                {
                    targetTime = $"/ {calculatedTotalTime:hh\\:mm\\:ss}";
                }
                else
                {
                    targetTime = $"/ {calculatedTotalTime:mm\\:ss}";
                }
            }

            string elapsedTime;

            if (_stopwatch.Elapsed.Days > 0)
            {
                elapsedTime = _stopwatch.Elapsed.ToString(@"d\.hh\:mm\:ss");
            }
            else if (_stopwatch.Elapsed.Hours > 0)
            {
                elapsedTime = _stopwatch.Elapsed.ToString(@"hh\:mm\:ss");
            }
            else
            {
                elapsedTime = _stopwatch.Elapsed.ToString(@"mm\:ss");
            }

            var percentage = (_count + _skipCount) * 100f / _totalCount;
            
            var test = $@"[] {percentage:0.0}% {_count:#,0} / {_totalCount:#,0} {elapsedTime}{targetTime} ";
            if (_skipCount != 0)
            {
                test = $@"[] {percentage:0.0}% {_count:#,0} / {(_count + _skipCount):#,0} / {_totalCount:#,0} {elapsedTime}{targetTime} ";
            }

            var totalProgressBarWidth = Console.WindowWidth - test.Length;

            var completedPercentageCount = (int) (totalProgressBarWidth / 100f * percentage);

            Console.CursorLeft = 0;

            var completedPercentageString = new string('#', completedPercentageCount);
            var incompletePercentageString = new string('-', totalProgressBarWidth - completedPercentageCount);

            if (_skipCount == 0)
            {
                Console.Write($@"[{completedPercentageString}{incompletePercentageString}] {percentage:0.0}% {_count:#,0} / {_totalCount:#,0} {elapsedTime}{targetTime}");
            }
            else
            {
                Console.Write($@"[{completedPercentageString}{incompletePercentageString}] {percentage:0.0}% {_count:#,0} / {(_count + _skipCount):#,0} / {_totalCount:#,0} {elapsedTime}{targetTime}");
            }
        }

        public void Tick()
        {
            _count++;
        }

        public void Skip()
        {
            _skipCount++;
        }

        public void Dispose()
        {
            _timer.Dispose();
            _stopwatch.Stop();
            WriteProgress(null);
        }
    }
}