## SnowflakeID

A thread-safe mechanism for generating snowflake IDs.

### Introduction

From the [Snowflake ID Wikipedia page](https://en.wikipedia.org/wiki/Snowflake_ID):

> Snowflake IDs, or snowflakes, are a form of unique identifier used in distributed computing. The format was created by Twitter (now X) and is used for the IDs of tweets.
> It is popularly believed that every snowflake has a unique structure, so they took the name "snowflake ID".

This package provides a mechanism for generating snowflake IDs containing a timestamp, data center ID, worker ID, and sequence number. The sequence number is incremented if another caller concurrently generates a snowflake ID 
for the same data center ID and worker ID.

Each combination of data center ID and worker ID should be a singleton to prevent duplicate IDs from being generated.

### Example Usage

The `SnowflakeProvider` class provides the `Next` method that generates a snowflake ID for the specified data center ID and worker ID. The provider class should be a singleton 
across all applications that use it.

To generate a new snowflake ID for data center `1` and worker ID `1`:

```
var snowflakeId = SnowflakeProvider.Next(1, 1);
```

If the snowflake ID functionality cannot be centralized into a service with a singleton instance of the provider class, the `Snowflake` class can be used independently in multiple places as long as each combination of 
data center ID and worker ID are singletons across your domain context.

To use the `Snowflake` class for a specific data center ID and worker ID (in this case data center ID `2` and worker ID `4`):

```
var snowflakeGenerator = new Snowflake(2, 4);
var snowflakeId = snowflakeGenerator.Next();
```
