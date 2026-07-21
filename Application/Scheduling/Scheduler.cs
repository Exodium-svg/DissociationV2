using Application.Db;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

namespace Application.Scheduling;

public interface IScheduleWork : INotifyCompletion
{
    public void Execute() => throw new NotImplementedException();
    //public void OnCompleted(Action continuation) => throw new NotImplementedException();
}

public class BaseScheduleWork<T> : IScheduleWork
{
    private Action continuation;
    private T? result;
    public bool IsCompleted { get; private set; } = false;

    public void OnCompleted(Action continuation)
    {
        this.continuation = continuation;

        IsCompleted = true;
    }

    public T GetResult()
    {
        if (null == result)
            throw new Exception($"Was result requested before task completion?");

        return result;
    }
}

public class Scheduler
{
    BlockingCollection<IScheduleWork> stagingQueue = new();
    ConcurrentQueue<IScheduleWork> commitQueue = new();
    Thread stagingWorker;


    List<SchedulerWorker> workers;

    CancellationTokenSource cts = new();

    DatabaseContext context = new DatabaseContext();
    public Scheduler(int threads)
    {
        stagingWorker = new(WorkerLoop);
        stagingWorker.Name = $"Staging worker";
        // It doesn't need to run often, it's a background type task. But it must end when our main thread does.
        stagingWorker.Priority = ThreadPriority.BelowNormal;


        workers = new(threads);

        for (int i = 0; i < threads; i++)
            workers.Add(new SchedulerWorker(commitQueue, cts.Token, i, threads));
    }

    public void StageWork(IScheduleWork work) => stagingQueue.Add(work);

    void WorkerLoop()
    {
        foreach (var stagedWork in stagingQueue.GetConsumingEnumerable())
        {
            {
                // Incase our cpu was mostly empty we wait for a milisecond still.
                Thread.Sleep(1);

                if (cts.IsCancellationRequested)
                    break; // we must clean up

                // we must achieve multiple things here.
                // 1. A mechanism to order tasks, as in certain priority tasks must be handled first so they go first in the commit queue.
                // 2. A wait to commit certain long duration tasks or cronjobs to the DB so we must check the DB for any expired tasks that must be fired off.


                // we must yield for other threads.
                Thread.Yield();
            }
        }
    }
}

public class SchedulerWorker
{
    ConcurrentQueue<IScheduleWork> workQueue;
    Queue<IScheduleWork> stolenWork = new();
    CancellationToken token;

    Thread workerThread;

    int totalThreads;

    int WorkToSteal => workQueue.Count / totalThreads;

    public SchedulerWorker(ConcurrentQueue<IScheduleWork> queue, CancellationToken token, int index, int totalThreads)
    {
        workQueue = queue;
        this.token = token;
        this.totalThreads = totalThreads;

        workerThread = new(WorkerLoop);

        workerThread.Name = $"Scheduler Worker #{index}";
        workerThread.Priority = ThreadPriority.Normal;

        workerThread.Start();
    }

    private void WorkerLoop()
    {
        while (false == token.IsCancellationRequested)
        {
            Thread.Sleep(1);

            while (workQueue.TryDequeue(out IScheduleWork? stagedWork) && stolenWork.Count > WorkToSteal)
                stolenWork.Enqueue(stagedWork);

            if (workQueue.IsEmpty)
            {
                Thread.Yield();
                continue;
            }

            if (false == workQueue.TryDequeue(out IScheduleWork? work))
                continue;


            work.Execute();

        }
    }
}


