namespace Quick.SL651.Utils
{
    public class TaskUtils
    {
        public static async Task<Task> TaskWait(Task task, int timeout)
        {
            var timeoutTask = Task.Delay(timeout);
            var retTask = await Task.WhenAny(task, timeoutTask);
            if (retTask == timeoutTask)
                throw new TimeoutException();
            return retTask;
        }

        /// <summary>
        /// 任务等待
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="task"></param>
        /// <param name="timeoutValue"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static async Task<Task<T>> TaskWait<T>(Task<T> task, int timeout)
        {
            var timeoutTask = Task.Delay(timeout).ContinueWith(t => default(T));
            var retTask = await Task.WhenAny(task, timeoutTask);
            if (retTask == timeoutTask)
                throw new TimeoutException();
            return retTask;
        }
    }
}
