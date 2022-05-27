// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

/*
*  Copyright 2022 MASES s.r.l.
*
*  Licensed under the Apache License, Version 2.0 (the "License");
*  you may not use this file except in compliance with the License.
*  You may obtain a copy of the License at
*
*  http://www.apache.org/licenses/LICENSE-2.0
*
*  Unless required by applicable law or agreed to in writing, software
*  distributed under the License is distributed on an "AS IS" BASIS,
*  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
*  See the License for the specific language governing permissions and
*  limitations under the License.
*
*  Refer to LICENSE for more information.
*/

using MASES.EntityFrameworkCore.KNet.Infrastructure.Internal;
using MASES.EntityFrameworkCore.KNet.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace MASES.EntityFrameworkCore.KNet;

/// <summary>
///     Extension methods for <see cref="IReadOnlyEntityType" /> for the Kafka provider.
/// </summary>
/// <remarks>
///     See <see href="https://aka.ms/efcore-docs-modeling">Modeling entity types and relationships</see>, and
///     <see href="https://github.com/masesgroup/KEFCore">The EF Core Kafka database provider</see> for more information and examples.
/// </remarks>
public static class KafkaEntityTypeExtensions
{
    /// <summary>
    ///     Returns the name of the container to which the entity type is mapped.
    /// </summary>
    /// <param name="entityType">The entity type to get the container name for.</param>
    /// <returns>The name of the container to which the entity type is mapped.</returns>
    public static string? GetContainer(this IReadOnlyEntityType entityType)
        => entityType.BaseType != null
            ? entityType.GetRootType().GetContainer()
            : ((string?)entityType[KafkaAnnotationNames.ContainerName]
                ?? GetDefaultContainer(entityType));

    private static string? GetDefaultContainer(IReadOnlyEntityType entityType)
        => entityType.FindOwnership() != null
            ? null
            : (entityType.Model.GetDefaultContainer()
                ?? entityType.ShortName());

    /// <summary>
    ///     Sets the name of the container to which the entity type is mapped.
    /// </summary>
    /// <param name="entityType">The entity type to set the container name for.</param>
    /// <param name="name">The name to set.</param>
    public static void SetContainer(this IMutableEntityType entityType, string? name)
        => entityType.SetOrRemoveAnnotation(
            KafkaAnnotationNames.ContainerName,
            Check.NullButNotEmpty(name, nameof(name)));

    /// <summary>
    ///     Sets the name of the container to which the entity type is mapped.
    /// </summary>
    /// <param name="entityType">The entity type to set the container name for.</param>
    /// <param name="name">The name to set.</param>
    /// <param name="fromDataAnnotation">Indicates whether the configuration was specified using a data annotation.</param>
    public static void SetContainer(
        this IConventionEntityType entityType,
        string? name,
        bool fromDataAnnotation = false)
        => entityType.SetOrRemoveAnnotation(
            KafkaAnnotationNames.ContainerName,
            Check.NullButNotEmpty(name, nameof(name)),
            fromDataAnnotation);

    /// <summary>
    ///     Gets the <see cref="ConfigurationSource" /> for the container to which the entity type is mapped.
    /// </summary>
    /// <param name="entityType">The entity type to find configuration source for.</param>
    /// <returns>The <see cref="ConfigurationSource" /> for the container to which the entity type is mapped.</returns>
    public static ConfigurationSource? GetContainerConfigurationSource(this IConventionEntityType entityType)
        => entityType.FindAnnotation(KafkaAnnotationNames.ContainerName)
            ?.GetConfigurationSource();

    /// <summary>
    ///     Returns the name of the parent property to which the entity type is mapped.
    /// </summary>
    /// <param name="entityType">The entity type to get the containing property name for.</param>
    /// <returns>The name of the parent property to which the entity type is mapped.</returns>
    public static string? GetContainingPropertyName(this IReadOnlyEntityType entityType)
        => entityType[KafkaAnnotationNames.PropertyName] as string
            ?? GetDefaultContainingPropertyName(entityType);

    private static string? GetDefaultContainingPropertyName(IReadOnlyEntityType entityType)
        => entityType.FindOwnership() is IReadOnlyForeignKey ownership
            ? ownership.PrincipalToDependent!.Name
            : null;

    /// <summary>
    ///     Sets the name of the parent property to which the entity type is mapped.
    /// </summary>
    /// <param name="entityType">The entity type to set the containing property name for.</param>
    /// <param name="name">The name to set.</param>
    public static void SetContainingPropertyName(this IMutableEntityType entityType, string? name)
        => entityType.SetOrRemoveAnnotation(
            KafkaAnnotationNames.PropertyName,
            Check.NullButNotEmpty(name, nameof(name)));

    /// <summary>
    ///     Sets the name of the parent property to which the entity type is mapped.
    /// </summary>
    /// <param name="entityType">The entity type to set the containing property name for.</param>
    /// <param name="name">The name to set.</param>
    /// <param name="fromDataAnnotation">Indicates whether the configuration was specified using a data annotation.</param>
    public static void SetContainingPropertyName(
        this IConventionEntityType entityType,
        string? name,
        bool fromDataAnnotation = false)
        => entityType.SetOrRemoveAnnotation(
            KafkaAnnotationNames.PropertyName,
            Check.NullButNotEmpty(name, nameof(name)),
            fromDataAnnotation);

    /// <summary>
    ///     Gets the <see cref="ConfigurationSource" /> for the parent property to which the entity type is mapped.
    /// </summary>
    /// <param name="entityType">The entity type to find configuration source for.</param>
    /// <returns>The <see cref="ConfigurationSource" /> for the parent property to which the entity type is mapped.</returns>
    public static ConfigurationSource? GetContainingPropertyNameConfigurationSource(this IConventionEntityType entityType)
        => entityType.FindAnnotation(KafkaAnnotationNames.PropertyName)
            ?.GetConfigurationSource();

    /// <summary>
    ///     Returns the name of the property that is used to store the partition key.
    /// </summary>
    /// <param name="entityType">The entity type to get the partition key property name for.</param>
    /// <returns>The name of the partition key property.</returns>
    public static string? GetPartitionKeyPropertyName(this IReadOnlyEntityType entityType)
        => entityType[KafkaAnnotationNames.PartitionKeyName] as string;

    /// <summary>
    ///     Sets the name of the property that is used to store the partition key key.
    /// </summary>
    /// <param name="entityType">The entity type to set the partition key property name for.</param>
    /// <param name="name">The name to set.</param>
    public static void SetPartitionKeyPropertyName(this IMutableEntityType entityType, string? name)
        => entityType.SetOrRemoveAnnotation(
            KafkaAnnotationNames.PartitionKeyName,
            Check.NullButNotEmpty(name, nameof(name)));

    /// <summary>
    ///     Sets the name of the property that is used to store the partition key.
    /// </summary>
    /// <param name="entityType">The entity type to set the partition key property name for.</param>
    /// <param name="name">The name to set.</param>
    /// <param name="fromDataAnnotation">Indicates whether the configuration was specified using a data annotation.</param>
    public static void SetPartitionKeyPropertyName(
        this IConventionEntityType entityType,
        string? name,
        bool fromDataAnnotation = false)
        => entityType.SetOrRemoveAnnotation(
            KafkaAnnotationNames.PartitionKeyName,
            Check.NullButNotEmpty(name, nameof(name)),
            fromDataAnnotation);

    /// <summary>
    ///     Gets the <see cref="ConfigurationSource" /> for the property that is used to store the partition key.
    /// </summary>
    /// <param name="entityType">The entity type to find configuration source for.</param>
    /// <returns>The <see cref="ConfigurationSource" /> for the partition key property.</returns>
    public static ConfigurationSource? GetPartitionKeyPropertyNameConfigurationSource(this IConventionEntityType entityType)
        => entityType.FindAnnotation(KafkaAnnotationNames.PartitionKeyName)
            ?.GetConfigurationSource();

    /// <summary>
    ///     Returns the property that is used to store the partition key.
    /// </summary>
    /// <param name="entityType">The entity type to get the partition key property for.</param>
    /// <returns>The name of the partition key property.</returns>
    public static IReadOnlyProperty? GetPartitionKeyProperty(this IReadOnlyEntityType entityType)
    {
        var partitionKeyPropertyName = entityType.GetPartitionKeyPropertyName();
        return partitionKeyPropertyName == null
            ? null
            : entityType.FindProperty(partitionKeyPropertyName);
    }

    /// <summary>
    ///     Returns the property that is used to store the partition key.
    /// </summary>
    /// <param name="entityType">The entity type to get the partition key property for.</param>
    /// <returns>The name of the partition key property.</returns>
    public static IMutableProperty? GetPartitionKeyProperty(this IMutableEntityType entityType)
    {
        var partitionKeyPropertyName = entityType.GetPartitionKeyPropertyName();
        return partitionKeyPropertyName == null
            ? null
            : entityType.FindProperty(partitionKeyPropertyName);
    }

    /// <summary>
    ///     Returns the property that is used to store the partition key.
    /// </summary>
    /// <param name="entityType">The entity type to get the partition key property for.</param>
    /// <returns>The name of the partition key property.</returns>
    public static IConventionProperty? GetPartitionKeyProperty(this IConventionEntityType entityType)
    {
        var partitionKeyPropertyName = entityType.GetPartitionKeyPropertyName();
        return partitionKeyPropertyName == null
            ? null
            : entityType.FindProperty(partitionKeyPropertyName);
    }

    /// <summary>
    ///     Returns the property that is used to store the partition key.
    /// </summary>
    /// <param name="entityType">The entity type to get the partition key property for.</param>
    /// <returns>The name of the partition key property.</returns>
    public static IProperty? GetPartitionKeyProperty(this IEntityType entityType)
    {
        var partitionKeyPropertyName = entityType.GetPartitionKeyPropertyName();
        return partitionKeyPropertyName == null
            ? null
            : entityType.FindProperty(partitionKeyPropertyName);
    }

    /// <summary>
    ///     Returns the name of the property that is used to store the ETag.
    /// </summary>
    /// <param name="entityType">The entity type to get the etag property name for.</param>
    /// <returns>The name of the etag property.</returns>
    public static string? GetETagPropertyName(this IReadOnlyEntityType entityType)
        => entityType[KafkaAnnotationNames.ETagName] as string;

    /// <summary>
    ///     Sets the name of the property that is used to store the ETag key.
    /// </summary>
    /// <param name="entityType">The entity type to set the etag property name for.</param>
    /// <param name="name">The name to set.</param>
    public static void SetETagPropertyName(this IMutableEntityType entityType, string? name)
        => entityType.SetOrRemoveAnnotation(
            KafkaAnnotationNames.ETagName,
            Check.NullButNotEmpty(name, nameof(name)));

    /// <summary>
    ///     Sets the name of the property that is used to store the ETag.
    /// </summary>
    /// <param name="entityType">The entity type to set the ETag property name for.</param>
    /// <param name="name">The name to set.</param>
    /// <param name="fromDataAnnotation">Indicates whether the configuration was specified using a data annotation.</param>
    public static void SetETagPropertyName(
        this IConventionEntityType entityType,
        string? name,
        bool fromDataAnnotation = false)
        => entityType.SetOrRemoveAnnotation(
            KafkaAnnotationNames.ETagName,
            Check.NullButNotEmpty(name, nameof(name)),
            fromDataAnnotation);

    /// <summary>
    ///     Gets the <see cref="ConfigurationSource" /> for the property that is used to store the etag.
    /// </summary>
    /// <param name="entityType">The entity type to find configuration source for.</param>
    /// <returns>The <see cref="ConfigurationSource" /> for the etag property.</returns>
    public static ConfigurationSource? GetETagPropertyNameConfigurationSource(this IConventionEntityType entityType)
        => entityType.FindAnnotation(KafkaAnnotationNames.ETagName)
            ?.GetConfigurationSource();

    /// <summary>
    ///     Gets the property on this entity that is mapped to cosmos ETag, if it exists.
    /// </summary>
    /// <param name="entityType">The entity type to get the ETag property for.</param>
    /// <returns>The property mapped to ETag, or <see langword="null" /> if no property is mapped to ETag.</returns>
    public static IReadOnlyProperty? GetETagProperty(this IReadOnlyEntityType entityType)
    {
        Check.NotNull(entityType, nameof(entityType));
        var etagPropertyName = entityType.GetETagPropertyName();
        return !string.IsNullOrEmpty(etagPropertyName) ? entityType.FindProperty(etagPropertyName) : null;
    }

    /// <summary>
    ///     Gets the property on this entity that is mapped to cosmos ETag, if it exists.
    /// </summary>
    /// <param name="entityType">The entity type to get the ETag property for.</param>
    /// <returns>The property mapped to etag, or <see langword="null" /> if no property is mapped to ETag.</returns>
    public static IProperty? GetETagProperty(this IEntityType entityType)
        => (IProperty?)((IReadOnlyEntityType)entityType).GetETagProperty();

    /// <summary>
    ///     Returns the time to live for analytical store in seconds at container scope.
    /// </summary>
    /// <param name="entityType">The entity type.</param>
    /// <returns>The time to live.</returns>
    public static int? GetAnalyticalStoreTimeToLive(this IReadOnlyEntityType entityType)
        => entityType.BaseType != null
            ? entityType.GetRootType().GetAnalyticalStoreTimeToLive()
            : (int?)entityType[KafkaAnnotationNames.AnalyticalStoreTimeToLive];

    /// <summary>
    ///     Sets the time to live for analytical store in seconds at container scope.
    /// </summary>
    /// <param name="entityType">The entity type.</param>
    /// <param name="seconds">The time to live to set.</param>
    public static void SetAnalyticalStoreTimeToLive(this IMutableEntityType entityType, int? seconds)
        => entityType.SetOrRemoveAnnotation(
            KafkaAnnotationNames.AnalyticalStoreTimeToLive,
            seconds);

    /// <summary>
    ///     Sets the time to live for analytical store in seconds at container scope.
    /// </summary>
    /// <param name="entityType">The entity type.</param>
    /// <param name="seconds">The time to live to set.</param>
    /// <param name="fromDataAnnotation">Indicates whether the configuration was specified using a data annotation.</param>
    public static void SetAnalyticalStoreTimeToLive(
        this IConventionEntityType entityType,
        int? seconds,
        bool fromDataAnnotation = false)
        => entityType.SetOrRemoveAnnotation(
            KafkaAnnotationNames.AnalyticalStoreTimeToLive,
            seconds,
            fromDataAnnotation);

    /// <summary>
    ///     Gets the <see cref="ConfigurationSource" /> for the time to live for analytical store in seconds at container scope.
    /// </summary>
    /// <param name="entityType">The entity typer.</param>
    /// <returns>The <see cref="ConfigurationSource" /> for the time to live for analytical store.</returns>
    public static ConfigurationSource? GetAnalyticalStoreTimeToLiveConfigurationSource(this IConventionEntityType entityType)
        => entityType.FindAnnotation(KafkaAnnotationNames.AnalyticalStoreTimeToLive)
            ?.GetConfigurationSource();

    /// <summary>
    ///     Returns the default time to live in seconds at container scope.
    /// </summary>
    /// <param name="entityType">The entity type.</param>
    /// <returns>The time to live.</returns>
    public static int? GetDefaultTimeToLive(this IReadOnlyEntityType entityType)
        => entityType.BaseType != null
            ? entityType.GetRootType().GetDefaultTimeToLive()
            : (int?)entityType[KafkaAnnotationNames.DefaultTimeToLive];

    /// <summary>
    ///     Sets the default time to live in seconds at container scope.
    /// </summary>
    /// <param name="entityType">The entity type.</param>
    /// <param name="seconds">The time to live to set.</param>
    public static void SetDefaultTimeToLive(this IMutableEntityType entityType, int? seconds)
        => entityType.SetOrRemoveAnnotation(
            KafkaAnnotationNames.DefaultTimeToLive,
            seconds);

    /// <summary>
    ///     Sets the default time to live in seconds at container scope.
    /// </summary>
    /// <param name="entityType">The entity type.</param>
    /// <param name="seconds">The time to live to set.</param>
    /// <param name="fromDataAnnotation">Indicates whether the configuration was specified using a data annotation.</param>
    public static void SetDefaultTimeToLive(
        this IConventionEntityType entityType,
        int? seconds,
        bool fromDataAnnotation = false)
        => entityType.SetOrRemoveAnnotation(
            KafkaAnnotationNames.DefaultTimeToLive,
            seconds,
            fromDataAnnotation);

    /// <summary>
    ///     Gets the <see cref="ConfigurationSource" /> for the default time to live in seconds at container scope.
    /// </summary>
    /// <param name="entityType">The entity type to find configuration source for.</param>
    /// <returns>The <see cref="ConfigurationSource" /> for the default time to live.</returns>
    public static ConfigurationSource? GetDefaultTimeToLiveConfigurationSource(this IConventionEntityType entityType)
        => entityType.FindAnnotation(KafkaAnnotationNames.DefaultTimeToLive)
            ?.GetConfigurationSource();





    /// <summary>
    ///     Gets the LINQ query used as the default source for queries of this type.
    /// </summary>
    /// <param name="entityType">The entity type to get the Kafka query for.</param>
    /// <returns>The LINQ query used as the default source.</returns>
    public static LambdaExpression? GetKafkaQuery(this IReadOnlyEntityType entityType)
#pragma warning disable EF1001 // Internal EF Core API usage.
#pragma warning disable CS0612 // Il tipo o il membro è obsoleto
        => (LambdaExpression?)entityType[CoreAnnotationNames.DefiningQuery];
#pragma warning restore CS0612 // Il tipo o il membro è obsoleto
#pragma warning restore EF1001 // Internal EF Core API usage.

    /// <summary>
    ///     Sets the LINQ query used as the default source for queries of this type.
    /// </summary>
    /// <param name="entityType">The entity type.</param>
    /// <param name="kafkaQuery">The LINQ query used as the default source.</param>
    public static void SetKafkaQuery(
        this IMutableEntityType entityType,
        LambdaExpression? kafkaQuery)
        => entityType
#pragma warning disable EF1001 // Internal EF Core API usage.
#pragma warning disable CS0612 // Il tipo o il membro è obsoleto
            .SetOrRemoveAnnotation(CoreAnnotationNames.DefiningQuery, kafkaQuery);
#pragma warning restore CS0612 // Il tipo o il membro è obsoleto
#pragma warning restore EF1001 // Internal EF Core API usage.

    /// <summary>
    ///     Sets the LINQ query used as the default source for queries of this type.
    /// </summary>
    /// <param name="entityType">The entity type.</param>
    /// <param name="kafkaQuery">The LINQ query used as the default source.</param>
    /// <param name="fromDataAnnotation">Indicates whether the configuration was specified using a data annotation.</param>
    /// <returns>The configured entity type.</returns>
    public static LambdaExpression? SetKafkaQuery(
        this IConventionEntityType entityType,
        LambdaExpression? kafkaQuery,
        bool fromDataAnnotation = false)
        => (LambdaExpression?)entityType
#pragma warning disable EF1001 // Internal EF Core API usage.
#pragma warning disable CS0612 // Il tipo o il membro è obsoleto
            .SetOrRemoveAnnotation(CoreAnnotationNames.DefiningQuery, kafkaQuery, fromDataAnnotation)
#pragma warning restore CS0612 // Il tipo o il membro è obsoleto
#pragma warning restore EF1001 // Internal EF Core API usage.
            ?.Value;

    /// <summary>
    ///     Returns the configuration source for <see cref="GetKafkaQuery" />.
    /// </summary>
    /// <param name="entityType">The entity type.</param>
    /// <returns>The configuration source for <see cref="GetKafkaQuery" />.</returns>
    public static ConfigurationSource? GetDefiningQueryConfigurationSource(this IConventionEntityType entityType)
#pragma warning disable EF1001 // Internal EF Core API usage.
#pragma warning disable CS0612 // Il tipo o il membro è obsoleto
        => entityType.FindAnnotation(CoreAnnotationNames.DefiningQuery)?.GetConfigurationSource();
#pragma warning restore CS0612 // Il tipo o il membro è obsoleto
#pragma warning restore EF1001 // Internal EF Core API usage.

    public static string TopicName(this IEntityType entityType, KafkaOptionsExtension options)
    {
        return $"{options.DatabaseName}.{entityType.Name}";
    }

    public static string StorageIdForTable(this IEntityType entityType, KafkaOptionsExtension options)
    {
        return $"Table_{entityType.TopicName(options)}";
    }

    public static string ApplicationIdForTable(this IEntityType entityType, KafkaOptionsExtension options)
    {
        return $"{options.ApplicationId}_{entityType.Name}";
    }

    public static short ReplicationFactor(this IEntityType entityType, KafkaOptionsExtension options)
    {
        var replicationFactor = options.DefaultReplicationFactor;
        return replicationFactor;
    }

    public static int NumPartitions(this IEntityType entityType, KafkaOptionsExtension options)
    {
        var numPartitions = options.DefaultNumPartitions;
        return numPartitions;
    }
}
