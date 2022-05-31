// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.Text;
using MASES.EntityFrameworkCore.KNet.Internal;
using MASES.EntityFrameworkCore.KNet.Storage.Internal;
using Newtonsoft.Json.Linq;

#nullable disable

namespace MASES.EntityFrameworkCore.KNet.Query.Internal
{
    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public partial class KafkaShapedQueryCompilingExpressionVisitor
    {
        private sealed class QueryingEnumerable<T> : IAsyncEnumerable<T>, IEnumerable<T>, IQueryingEnumerable
        {
            private readonly QueryContext _queryContext;
            private readonly IEnumerable<ValueBuffer> _innerEnumerable;
            private readonly Func<QueryContext, ValueBuffer, T> _shaper;
            private readonly Type _contextType;
            private readonly IDiagnosticsLogger<DbLoggerCategory.Query> _queryLogger;
            private readonly bool _standAloneStateManager;
            private readonly bool _threadSafetyChecksEnabled;

            public QueryingEnumerable(
                QueryContext queryContext,
                IEnumerable<ValueBuffer> innerEnumerable,
                Func<QueryContext, ValueBuffer, T> shaper,
                Type contextType,
                bool standAloneStateManager,
                bool threadSafetyChecksEnabled)
            {
                _queryContext = queryContext;
                _innerEnumerable = innerEnumerable;
                _shaper = shaper;
                _contextType = contextType;
                _queryLogger = queryContext.QueryLogger;
                _standAloneStateManager = standAloneStateManager;
                _threadSafetyChecksEnabled = threadSafetyChecksEnabled;
            }

            public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
                => new Enumerator(this, cancellationToken);

            public IEnumerator<T> GetEnumerator()
                => new Enumerator(this);

            IEnumerator IEnumerable.GetEnumerator()
                => GetEnumerator();

            public string ToQueryString()
                => KafkaStrings.NoQueryStrings;

            private sealed class Enumerator : IEnumerator<T>, IAsyncEnumerator<T>
            {
                private readonly QueryContext _queryContext;
                private readonly IEnumerable<ValueBuffer> _innerEnumerable;
                private readonly Func<QueryContext, ValueBuffer, T> _shaper;
                private readonly Type _contextType;
                private readonly IDiagnosticsLogger<DbLoggerCategory.Query> _queryLogger;
                private readonly bool _standAloneStateManager;
                private readonly CancellationToken _cancellationToken;
                private readonly IConcurrencyDetector? _concurrencyDetector;

                private IEnumerator<ValueBuffer>? _enumerator;

                public Enumerator(QueryingEnumerable<T> queryingEnumerable, CancellationToken cancellationToken = default)
                {
                    _queryContext = queryingEnumerable._queryContext;
                    _innerEnumerable = queryingEnumerable._innerEnumerable;
                    _shaper = queryingEnumerable._shaper;
                    _contextType = queryingEnumerable._contextType;
                    _queryLogger = queryingEnumerable._queryLogger;
                    _standAloneStateManager = queryingEnumerable._standAloneStateManager;
                    _cancellationToken = cancellationToken;
                    Current = default!;

                    _concurrencyDetector = queryingEnumerable._threadSafetyChecksEnabled
                        ? _queryContext.ConcurrencyDetector
                        : null;
                }

                public T Current { get; private set; }

                object IEnumerator.Current
                    => Current!;

                public bool MoveNext()
                {
                    try
                    {
                        _concurrencyDetector?.EnterCriticalSection();

                        try
                        {
                            return MoveNextHelper();
                        }
                        finally
                        {
                            _concurrencyDetector?.ExitCriticalSection();
                        }
                    }
                    catch (Exception exception)
                    {
                        _queryLogger.QueryIterationFailed(_contextType, exception);

                        throw;
                    }
                }

                public ValueTask<bool> MoveNextAsync()
                {
                    try
                    {
                        _concurrencyDetector?.EnterCriticalSection();

                        try
                        {
                            _cancellationToken.ThrowIfCancellationRequested();

                            return new ValueTask<bool>(MoveNextHelper());
                        }
                        finally
                        {
                            _concurrencyDetector?.ExitCriticalSection();
                        }
                    }
                    catch (Exception exception)
                    {
                        _queryLogger.QueryIterationFailed(_contextType, exception);

                        throw;
                    }
                }

                private bool MoveNextHelper()
                {
                    if (_enumerator == null)
                    {
                        EntityFrameworkEventSource.Log.QueryExecuting();

                        _enumerator = _innerEnumerable.GetEnumerator();
                        _queryContext.InitializeStateManager(_standAloneStateManager);
                    }

                    var hasNext = _enumerator.MoveNext();

                    Current = hasNext
                        ? _shaper(_queryContext, _enumerator.Current)
                        : default!;

                    return hasNext;
                }

                public void Dispose()
                {
                    _enumerator?.Dispose();
                    _enumerator = null;
                }

                public ValueTask DisposeAsync()
                {
                    var enumerator = _enumerator;
                    _enumerator = null;

                    return enumerator.DisposeAsyncIfAvailable();
                }

                public void Reset()
                    => throw new NotSupportedException(CoreStrings.EnumerableResetNotSupported);
            }
        }

        /*
        private sealed class QueryingEnumerable<T> : IEnumerable<T>, IAsyncEnumerable<T>, IQueryingEnumerable
        {
            private readonly KafkaQueryContext _kafkaQueryContext;
            private readonly ISqlExpressionFactory _sqlExpressionFactory;
            private readonly SelectExpression _selectExpression;
            private readonly Func<KafkaQueryContext, JObject, T> _shaper;
            private readonly IQuerySqlGeneratorFactory _querySqlGeneratorFactory;
            private readonly Type _contextType;
            private readonly string _partitionKey;
            private readonly IDiagnosticsLogger<DbLoggerCategory.Query> _queryLogger;
            private readonly bool _standAloneStateManager;
            private readonly bool _threadSafetyChecksEnabled;

            public QueryingEnumerable(
                KafkaQueryContext kafkaQueryContext,
                ISqlExpressionFactory sqlExpressionFactory,
                IQuerySqlGeneratorFactory querySqlGeneratorFactory,
                SelectExpression selectExpression,
                Func<KafkaQueryContext, JObject, T> shaper,
                Type contextType,
                string partitionKeyFromExtension,
                bool standAloneStateManager,
                bool threadSafetyChecksEnabled)
            {
                _kafkaQueryContext = kafkaQueryContext;
                _sqlExpressionFactory = sqlExpressionFactory;
                _querySqlGeneratorFactory = querySqlGeneratorFactory;
                _selectExpression = selectExpression;
                _shaper = shaper;
                _contextType = contextType;
                _queryLogger = kafkaQueryContext.QueryLogger;
                _standAloneStateManager = standAloneStateManager;
                _threadSafetyChecksEnabled = threadSafetyChecksEnabled;

                var partitionKey = selectExpression.GetPartitionKey(kafkaQueryContext.ParameterValues);
                if (partitionKey != null && partitionKeyFromExtension != null && partitionKeyFromExtension != partitionKey)
                {
                    throw new InvalidOperationException(KafkaStrings.PartitionKeyMismatch(partitionKeyFromExtension, partitionKey));
                }

                _partitionKey = partitionKey ?? partitionKeyFromExtension;
            }

            public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
                => new AsyncEnumerator(this, cancellationToken);

            public IEnumerator<T> GetEnumerator()
                => new Enumerator(this);

            IEnumerator IEnumerable.GetEnumerator()
                => GetEnumerator();

            private KafkaSqlQuery GenerateQuery()
                => _querySqlGeneratorFactory.Create().GetSqlQuery(
                    (SelectExpression)new InExpressionValuesExpandingExpressionVisitor(
                            _sqlExpressionFactory,
                            _kafkaQueryContext.ParameterValues)
                        .Visit(_selectExpression),
                    _kafkaQueryContext.ParameterValues);

            public string ToQueryString()
            {
                var sqlQuery = GenerateQuery();
                if (sqlQuery.Parameters.Count == 0)
                {
                    return sqlQuery.Query;
                }

                var builder = new StringBuilder();
                foreach (var parameter in sqlQuery.Parameters)
                {
                    builder
                        .Append("-- ")
                        .Append(parameter.Name)
                        .Append("='")
                        .Append(parameter.Value)
                        .AppendLine("'");
                }

                return builder.Append(sqlQuery.Query).ToString();
            }

            private sealed class Enumerator : IEnumerator<T>
            {
                private readonly QueryingEnumerable<T> _queryingEnumerable;
                private readonly KafkaQueryContext _kafkaQueryContext;
                private readonly SelectExpression _selectExpression;
                private readonly Func<KafkaQueryContext, JObject, T> _shaper;
                private readonly Type _contextType;
                private readonly string _partitionKey;
                private readonly IDiagnosticsLogger<DbLoggerCategory.Query> _queryLogger;
                private readonly bool _standAloneStateManager;
                private readonly IConcurrencyDetector _concurrencyDetector;

                private IEnumerator<JObject> _enumerator;

                public Enumerator(QueryingEnumerable<T> queryingEnumerable)
                {
                    _queryingEnumerable = queryingEnumerable;
                    _kafkaQueryContext = queryingEnumerable._kafkaQueryContext;
                    _shaper = queryingEnumerable._shaper;
                    _selectExpression = queryingEnumerable._selectExpression;
                    _contextType = queryingEnumerable._contextType;
                    _partitionKey = queryingEnumerable._partitionKey;
                    _queryLogger = queryingEnumerable._queryLogger;
                    _standAloneStateManager = queryingEnumerable._standAloneStateManager;

                    _concurrencyDetector = queryingEnumerable._threadSafetyChecksEnabled
                        ? _kafkaQueryContext.ConcurrencyDetector
                        : null;
                }

                public T Current { get; private set; }

                object IEnumerator.Current
                    => Current;

                public bool MoveNext()
                {
                    try
                    {
                        _concurrencyDetector?.EnterCriticalSection();

                        try
                        {
                            if (_enumerator == null)
                            {
                                var sqlQuery = _queryingEnumerable.GenerateQuery();

                                EntityFrameworkEventSource.Log.QueryExecuting();

                                _enumerator = _kafkaQueryContext.KafkaCluster
                                    .ExecuteSqlQuery(
                                        _selectExpression.Container,
                                        _partitionKey,
                                        sqlQuery)
                                    .GetEnumerator();
                                _kafkaQueryContext.InitializeStateManager(_standAloneStateManager);
                            }

                            var hasNext = _enumerator.MoveNext();

                            Current
                                = hasNext
                                    ? _shaper(_kafkaQueryContext, _enumerator.Current)
                                    : default;

                            return hasNext;
                        }
                        finally
                        {
                            _concurrencyDetector?.ExitCriticalSection();
                        }
                    }
                    catch (Exception exception)
                    {
                        _queryLogger.QueryIterationFailed(_contextType, exception);

                        throw;
                    }
                }

                public void Dispose()
                {
                    _enumerator?.Dispose();
                    _enumerator = null;
                }

                public void Reset()
                    => throw new NotSupportedException(CoreStrings.EnumerableResetNotSupported);
            }

            private sealed class AsyncEnumerator : IAsyncEnumerator<T>
            {
                private readonly QueryingEnumerable<T> _queryingEnumerable;
                private readonly KafkaQueryContext _cosmosQueryContext;
                private readonly SelectExpression _selectExpression;
                private readonly Func<KafkaQueryContext, JObject, T> _shaper;
                private readonly Type _contextType;
                private readonly string _partitionKey;
                private readonly IDiagnosticsLogger<DbLoggerCategory.Query> _queryLogger;
                private readonly bool _standAloneStateManager;
                private readonly CancellationToken _cancellationToken;
                private readonly IConcurrencyDetector _concurrencyDetector;

                private IAsyncEnumerator<JObject> _enumerator;

                public AsyncEnumerator(QueryingEnumerable<T> queryingEnumerable, CancellationToken cancellationToken)
                {
                    _queryingEnumerable = queryingEnumerable;
                    _cosmosQueryContext = queryingEnumerable._kafkaQueryContext;
                    _shaper = queryingEnumerable._shaper;
                    _selectExpression = queryingEnumerable._selectExpression;
                    _contextType = queryingEnumerable._contextType;
                    _partitionKey = queryingEnumerable._partitionKey;
                    _queryLogger = queryingEnumerable._queryLogger;
                    _standAloneStateManager = queryingEnumerable._standAloneStateManager;
                    _cancellationToken = cancellationToken;

                    _concurrencyDetector = queryingEnumerable._threadSafetyChecksEnabled
                        ? _cosmosQueryContext.ConcurrencyDetector
                        : null;
                }

                public T Current { get; private set; }

                public async ValueTask<bool> MoveNextAsync()
                {
                    try
                    {
                        _concurrencyDetector?.EnterCriticalSection();

                        try
                        {
                            if (_enumerator == null)
                            {
                                var sqlQuery = _queryingEnumerable.GenerateQuery();

                                EntityFrameworkEventSource.Log.QueryExecuting();

                                _enumerator = _cosmosQueryContext.KafkaCluster
                                    .ExecuteSqlQueryAsync(
                                        _selectExpression.Container,
                                        _partitionKey,
                                        sqlQuery)
                                    .GetAsyncEnumerator(_cancellationToken);
                                _cosmosQueryContext.InitializeStateManager(_standAloneStateManager);
                            }

                            var hasNext = await _enumerator.MoveNextAsync().ConfigureAwait(false);

                            Current
                                = hasNext
                                    ? _shaper(_cosmosQueryContext, _enumerator.Current)
                                    : default;

                            return hasNext;
                        }
                        finally
                        {
                            _concurrencyDetector?.ExitCriticalSection();
                        }
                    }
                    catch (Exception exception)
                    {
                        _queryLogger.QueryIterationFailed(_contextType, exception);

                        throw;
                    }
                }

                public ValueTask DisposeAsync()
                {
                    var enumerator = _enumerator;
                    if (enumerator != null)
                    {
                        _enumerator = null;
                        return enumerator.DisposeAsync();
                    }

                    return default;
                }
            }
        }
        */
    }
}