# waiting_queue

C# waiting_queue based on generic `Queue<T>` and events

- use `CanEnqueue` to detect whether `enqueue` will flush old item
- use `CanDequeue` to detect whether `dequeue` will throw `InvalidOperationException` if empty
- use `EnqueueWaitAvailable` blocks `enqueue` until not full
- use `DequeueWaitAvailable` blocks `dequeue` until not empty
