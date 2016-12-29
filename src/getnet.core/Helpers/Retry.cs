using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;

namespace getnet
{
    public static class Retry
    {
        public static T Do<T>(
            Func<T> action,
            TimeSpan retryInterval,
            int retryCount = 3
            )
        {
            return Do<T>(action, retryInterval, null, retryCount);
        }

        public static void Do(
            Action action,
            TimeSpan retryInterval,
            int retryCount = 3)
        {
            Do<object>(() =>
            {
                action();
                return null;
            }, retryInterval, null, retryCount);
        }

        public static T Do<T>(
            Func<T> action,
            TimeSpan retryInterval,
            Type breakOnExceptionType,
            int retryCount = 3)
        {
            var exceptions = new List<Exception>();

            for (int retry = 0; retry < retryCount; retry++)
            {
                try
                {
                    if (retry > 0)
                        Thread.Sleep(retryInterval);
                    return action();
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                    if (breakOnExceptionType != null && ex.GetType() == breakOnExceptionType)
                        break;
                }
            }

            throw new AggregateException(exceptions);
        }
    }
}
