using System.Collections.Concurrent;

namespace org.goodspace.org.Utils.SnowflakeID
{
    /// <summary>
    /// Provides a thread-safe mechanism for generating snowflake IDs.
    /// </summary>
    public static class SnowflakeProvider
    {
        /// <summary>
        /// Thread-safe dictionary containing all instances of the <see cref="Snowflake"/> objects 
        /// for this provider.
        /// </summary>
        static readonly ConcurrentDictionary<(long, long), Snowflake> instances = new();

        /// <summary>
        /// Retrieves the <see cref="Snowflake"/> for the specified worker ID and data center ID, 
        /// creating a new instance if one does not already exist.
        /// </summary>
        /// <param name="workerId">Worker ID</param>
        /// <param name="dataCenterId">Data center ID</param>
        /// <returns><see cref="Snowflake"/></returns>
        static Snowflake GetInstance(long workerId, long dataCenterId)
        {
            while (true)
            {
                if (instances.TryGetValue((workerId, dataCenterId), out var existingGenerator))
                    return existingGenerator;

                var generator = new Snowflake(workerId, dataCenterId);

                if (instances.TryAdd((workerId, dataCenterId), generator))
                    return generator;
            }
        }

        /// <summary>
        /// Generates the next snowflake ID for the specified worker ID and data center ID.
        /// </summary>
        /// <param name="workerId">Worker ID</param>
        /// <param name="dataCenterId">Data center ID</param>
        /// <returns><see cref="long"/></returns>
        public static long Next(long workerId, long dataCenterId)
        {
            var generator = GetInstance(workerId, dataCenterId);
            return generator.Next();
        }
    }
}
