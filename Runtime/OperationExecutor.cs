using System;
using System.Threading;
using System.Threading.Tasks;

namespace ReadyPlayerMe.Core
{
    public class OperationExecutor<T> where T : Context
    {

        private readonly IOperation<T>[] operations;
        private readonly int operationsCount;

        private CancellationTokenSource ctxSource;
        private int currentIndex;

        private float currentProgress;
        public int Timeout;

        public OperationExecutor(IOperation<T>[] operations)
        {
            this.operations = operations;
            operationsCount = operations.Length;
        }

        public bool IsCancelled => ctxSource.IsCancellationRequested;
        public event Action<float, string> ProgressChanged;

        public async Task<T> Execute(T context)
        {
            ctxSource = new CancellationTokenSource();
            foreach (IOperation<T> operation in operations)
            {
                operation.ProgressChanged += OnProgressChanged;
                operation.Timeout = Timeout;
                try
                {
                    context = await operation.Execute(context, ctxSource.Token);
                    currentIndex++;
                }
                catch (CustomException exception)
                {
                    if (ctxSource.IsCancellationRequested)
                    {
                        ctxSource.Dispose();
                        throw new CustomException(FailureType.OperationCancelled, exception.Message);
                    }

                    throw new CustomException(exception.FailureType, exception.Message);
                }
                operation.ProgressChanged -= OnProgressChanged;
            }

            return context;
        }

        public void Cancel()
        {
            if (!ctxSource.IsCancellationRequested)
            {
                ctxSource?.Cancel();
            }
        }

        private void OnProgressChanged(float progress)
        {
            currentProgress = 1f / operationsCount * (currentIndex + progress);
            ProgressChanged?.Invoke(currentProgress, operations[currentIndex].GetType().Name);
        }
    }
}
