using System.Collections.Generic;
using System;
using System.Threading;

namespace Molinos.DataAgro.Entities.Helpers
{
    public static class Retry
    {
        /// <summary>
        /// Makes a shallow copy of an entity object. This works much like a MemberwiseClone
        /// but directly instantiates a new object and copies only properties that work with
        /// EF and don't have the NotMappedAttribute.
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="source">The source entity.</param>

        public static void Do(
            Action action,
            TimeSpan retryInterval,
            int maxAttemptCount = 3)
        {
            Do<object>(() =>
            {
                action();
                return null;
            }, retryInterval, maxAttemptCount);
        }

        public static T Do<T>(
            Func<T> action,
            TimeSpan retryInterval,
            int maxAttemptCount = 3)
        {
            var exceptions = new List<Exception>();



            for (int attempted = 0; attempted < maxAttemptCount; attempted++)
            {
                try
                {
                    if (attempted > 0)
                    {
                        Thread.Sleep(retryInterval);
                    }
                    return action();
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                }
            }
            throw new AggregateException(exceptions);
        }
    }
}