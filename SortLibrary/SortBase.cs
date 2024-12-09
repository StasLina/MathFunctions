
using System.Diagnostics;
using MathData;

namespace SortLibrary
{

    public abstract class SortBase
    {
        public RecordSortResults Results { get; set; }
        Stopwatch _stopwatch;
        public int InstructionCount { get; protected set; }

        // ����� ��� ����������, ����� ���������� � ����������
        //public abstract void Sort(List<double> data);
        public abstract void Sort(List<double> data, bool order);


        List<double> _data;

        public List<double> Data { get => _data; set => _data = value; }
        public void TimingSort(List<double> data, bool order)
        {
            _stopwatch = new Stopwatch();
            _data = data;
            // ��������� ������
            _stopwatch.Start();
            Sort(data, order);
            _stopwatch.Stop();
            long elapsedMilliseconds = _stopwatch.ElapsedMilliseconds;

            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                Results.Time = _stopwatch.ElapsedMilliseconds;
                Results.Results = _data;
                Results.Iteration = InstructionCount;

            }
            );
        }

        public event Func<List<double>, Task<bool>>? EventAtion;

        // ����� ��� ���������� �������� ����������
        protected async Task IncrementInstructionCount()
        {
            InstructionCount++;

            if (Results != null)
            {
                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                    {
                        Results.Time = _stopwatch.ElapsedMilliseconds;
                        Results.Iteration = InstructionCount;
                    }
                );
                //Results.Time = _stopwatch.ElapsedMilliseconds;


                if (Results.cts.Token.IsCancellationRequested)
                {
                    Results.cts.Token.ThrowIfCancellationRequested();
                }
                else
                {
                    // ���������  �� �����

                    while (Results.isPaused)
                    {
                        Monitor.Wait(Results._lock); // ���, ���� ������������ �� ����� �����
                    }
                }
            }



            if (EventAtion != null)
            {
                // ������� ���������� ����������� ������������
                bool result = await EventAtion.Invoke(_data); // ���� ���������� ����������� ������������
                if (result)
                {
                    // ���� ������� ���������, ����������� ����������
                    throw new OperationCanceledException("�������� ��������.");
                }
            }
        }


        void Pause()
        {
            lock (Results._lock)
            {
                Results.isPaused = true;
            }
        }

        void Resume()
        {
            lock (Results._lock)
            {
                Results.isPaused = false;
                Monitor.PulseAll(Results._lock); // ���������� ��� ��������� ������
            }
        }

        public abstract Task SortAsync(List<double> data, bool ascending, CancellationToken cancellationToken);
    }


}
