
namespace org.goodspace.org.Utils.SnowflakeID
{
    /// <summary>
    /// Generates snowflake IDs for a specific worker ID and data center ID.
    /// </summary>
    /// <remarks>This class should always be a singleton for any given worker ID and data center ID.
    /// Otherwise, duplicate IDs may be generated.</remarks>
    public class Snowflake
    {
        /// <summary>
        /// Epoch time for the snowflake ID generator.
        /// </summary>
        /// <remarks>Thursday, November 4, 2010 1:42:54 AM UTC</remarks>
        public const long Epoch = 1288834974000L;

        // Worker ID bits
        const int WorkerIdBits = 5;

        // Data center ID bits
        const int DataCenterIdBits = 3;

        // Sequence number bits
        const int SequenceBits = 8;

        const long MaxWorkerId = -1L ^ (-1L << WorkerIdBits);
        const long MaxDataCenterId = -1L ^ (-1L << DataCenterIdBits);
        const long SequenceMask = -1L ^ (-1L << SequenceBits);
        const int WorkerIdShift = SequenceBits;
        const int DataCenterIdShift = SequenceBits + WorkerIdBits;

        /// <summary>
        /// 
        /// </summary>
        const int TimestampLeftShift = SequenceBits + WorkerIdBits + DataCenterIdBits;

        long _sequence = 0L;
        long _lastTimestamp = -1L;

        /// <summary>
        /// Worker ID
        /// </summary>
        public long WorkerId { get; protected set; }

        /// <summary>
        /// Data center ID
        /// </summary>
        public long DataCenterId { get; protected set; }

        /// <summary>
        /// Sequence number
        /// </summary>
        public long Sequence
        {
            get { return _sequence; }
            internal set { _sequence = value; }
        }

        /// <summary>
        /// Creates a new instance of the <see cref="Snowflake"/> class.
        /// </summary>
        /// <param name="workerId">Worker ID</param>
        /// <param name="dataCenterId">Data center ID</param>
        /// <param name="sequence">Sequence number</param>
        /// <exception cref="ArgumentException"></exception>
        public Snowflake(long workerId, long dataCenterId, long sequence = 0L)
        {
            if (workerId > MaxWorkerId || workerId < 0)
                throw new ArgumentException($"Worker ID must be greater than or equal to 0 and less than or equal to {MaxWorkerId}", nameof(workerId));

            if (dataCenterId > MaxDataCenterId || dataCenterId < 0)
                throw new ArgumentException($"Data center ID must be greater than or equal to 0 and less than or equal to {MaxDataCenterId}", nameof(dataCenterId));

            WorkerId = workerId;
            DataCenterId = dataCenterId;
            _sequence = sequence;
        }

        /// <summary>
        /// Synchronization object
        /// </summary>
        readonly object _lock = new();

        /// <summary>
        /// Generates the next snowflake ID.
        /// </summary>
        /// <returns><see cref="long"/></returns>
        /// <exception cref="Exception"></exception>
        public long Next()
        {
            lock (_lock)
            {
                var timestamp = TimeGen();

                if (timestamp < _lastTimestamp)
                    throw new Exception($"Timestamp error");

                if (_lastTimestamp == timestamp)
                {
                    _sequence = (_sequence + 1) & SequenceMask;

                    if (_sequence == 0)
                        timestamp = TilNextMillis(_lastTimestamp);
                }
                else
                {
                    _sequence = 0;
                }

                _lastTimestamp = timestamp;
                return ((timestamp - Epoch) << TimestampLeftShift) | (DataCenterId << DataCenterIdShift) | (WorkerId << WorkerIdShift) | _sequence;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lastTimestamp"></param>
        /// <returns></returns>
        static long TilNextMillis(long lastTimestamp)
        {
            var timestamp = TimeGen();

            while (timestamp <= lastTimestamp)
                timestamp = TimeGen();

            return timestamp;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        static long TimeGen()
        {
            return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }
    }
}
