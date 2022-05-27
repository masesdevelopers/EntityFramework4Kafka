// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using MASES.EntityFrameworkCore.KNet.Storage.Internal;

namespace MASES.EntityFrameworkCore.KNet.Query.Internal;

/// <summary>
///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
///     the same compatibility standards as public APIs. It may be changed or removed without notice in
///     any release. You should only use it directly in your code with extreme caution and knowing that
///     doing so can result in application failures when updating to a new Entity Framework Core release.
/// </summary>
public class KafkaQueryContext : QueryContext
{
    private readonly IDictionary<IEntityType, IEnumerable<ValueBuffer>> _valueBuffersCache
        = new Dictionary<IEntityType, IEnumerable<ValueBuffer>>();

    public virtual IEnumerable<ValueBuffer> GetValueBuffers(IEntityType entityType)
    {
        if (!_valueBuffersCache.TryGetValue(entityType, out var valueBuffers))
        {
            valueBuffers = Cluster.GetData(entityType);

            _valueBuffersCache[entityType] = valueBuffers;
        }

        return valueBuffers;
    }

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public KafkaQueryContext(
        QueryContextDependencies dependencies,
        IKafkaCluster kafkaCluster)
        : base(dependencies)
    {
        KafkaCluster = kafkaCluster;
    }

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public virtual IKafkaCluster KafkaCluster { get; }
}
