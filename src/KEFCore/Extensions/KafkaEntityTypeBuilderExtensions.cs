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

using MASES.EntityFrameworkCore.KNet.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace MASES.EntityFrameworkCore.KNet;

/// <summary>
///     Extension methods for <see cref="EntityTypeBuilder" /> for the Kafka provider.
/// </summary>
/// <remarks>
///     See <see href="https://aka.ms/efcore-docs-modeling">Modeling entity types and relationships</see>, and
///     <see href="https://github.com/masesgroup/KEFCore">The EF Core Kafka database provider</see> for more information and examples.
/// </remarks>
public static class KafkaEntityTypeBuilderExtensions
{
    /// <summary>
    ///     Configures the container that the entity type maps to when targeting Azure Cosmos.
    /// </summary>
    /// <remarks>
    ///     See <see href="https://aka.ms/efcore-docs-modeling">Modeling entity types and relationships</see>, and
    ///     <see href="https://aka.ms/efcore-docs-cosmos">Accessing Azure Cosmos DB with EF Core</see> for more information.
    /// </remarks>
    /// <param name="entityTypeBuilder">The builder for the entity type being configured.</param>
    /// <param name="name">The name of the container.</param>
    /// <returns>The same builder instance so that multiple calls can be chained.</returns>
    public static EntityTypeBuilder ToContainer(
        this EntityTypeBuilder entityTypeBuilder,
        string? name)
    {
        Check.NotNull(entityTypeBuilder, nameof(entityTypeBuilder));
        Check.NullButNotEmpty(name, nameof(name));

        entityTypeBuilder.Metadata.SetContainer(name);

        return entityTypeBuilder;
    }

    /// <summary>
    ///     Configures the container that the entity type maps to when targeting Azure Cosmos.
    /// </summary>
    /// <remarks>
    ///     See <see href="https://aka.ms/efcore-docs-modeling">Modeling entity types and relationships</see>, and
    ///     <see href="https://aka.ms/efcore-docs-cosmos">Accessing Azure Cosmos DB with EF Core</see> for more information.
    /// </remarks>
    /// <typeparam name="TEntity">The entity type being configured.</typeparam>
    /// <param name="entityTypeBuilder">The builder for the entity type being configured.</param>
    /// <param name="name">The name of the container.</param>
    /// <returns>The same builder instance so that multiple calls can be chained.</returns>
    public static EntityTypeBuilder<TEntity> ToContainer<TEntity>(
        this EntityTypeBuilder<TEntity> entityTypeBuilder,
        string? name)
        where TEntity : class
        => (EntityTypeBuilder<TEntity>)ToContainer((EntityTypeBuilder)entityTypeBuilder, name);

    /// <summary>
    ///     Configures the container that the entity type maps to when targeting Azure Cosmos.
    /// </summary>
    /// <remarks>
    ///     See <see href="https://aka.ms/efcore-docs-modeling">Modeling entity types and relationships</see>, and
    ///     <see href="https://aka.ms/efcore-docs-cosmos">Accessing Azure Cosmos DB with EF Core</see> for more information.
    /// </remarks>
    /// <param name="entityTypeBuilder">The builder for the entity type being configured.</param>
    /// <param name="name">The name of the container.</param>
    /// <param name="fromDataAnnotation">Indicates whether the configuration was specified using a data annotation.</param>
    /// <returns>
    ///     The same builder instance if the configuration was applied,
    ///     <see langword="null" /> otherwise.
    /// </returns>
    public static IConventionEntityTypeBuilder? ToContainer(
        this IConventionEntityTypeBuilder entityTypeBuilder,
        string? name,
        bool fromDataAnnotation = false)
    {
        if (!entityTypeBuilder.CanSetContainer(name, fromDataAnnotation))
        {
            return null;
        }

        entityTypeBuilder.Metadata.SetContainer(name, fromDataAnnotation);

        return entityTypeBuilder;
    }

    /// <summary>
    ///     Returns a value indicating whether the container that the entity type maps to can be set
    ///     from the current configuration source
    /// </summary>
    /// <remarks>
    ///     See <see href="https://aka.ms/efcore-docs-modeling">Modeling entity types and relationships</see>, and
    ///     <see href="https://aka.ms/efcore-docs-cosmos">Accessing Azure Cosmos DB with EF Core</see> for more information.
    /// </remarks>
    /// <param name="entityTypeBuilder">The builder for the entity type being configured.</param>
    /// <param name="name">The name of the container.</param>
    /// <param name="fromDataAnnotation">Indicates whether the configuration was specified using a data annotation.</param>
    /// <returns><see langword="true" /> if the configuration can be applied.</returns>
    public static bool CanSetContainer(
        this IConventionEntityTypeBuilder entityTypeBuilder,
        string? name,
        bool fromDataAnnotation = false)
    {
        Check.NotNull(entityTypeBuilder, nameof(entityTypeBuilder));
        Check.NullButNotEmpty(name, nameof(name));

        return entityTypeBuilder.CanSetAnnotation(KafkaAnnotationNames.ContainerName, name, fromDataAnnotation);
    }

    /// <summary>
    ///     Configures the property name that the entity is mapped to when stored as an embedded document.
    /// </summary>
    /// <remarks>
    ///     See <see href="https://aka.ms/efcore-docs-modeling">Modeling entity types and relationships</see>, and
    ///     <see href="https://aka.ms/efcore-docs-cosmos">Accessing Azure Cosmos DB with EF Core</see> for more information.
    /// </remarks>
    /// <param name="entityTypeBuilder">The builder for the entity type being configured.</param>
    /// <param name="name">The name of the parent property.</param>
    /// <returns>The same builder instance so that multiple calls can be chained.</returns>
    public static OwnedNavigationBuilder ToJsonProperty(
        this OwnedNavigationBuilder entityTypeBuilder,
        string? name)
    {
        entityTypeBuilder.OwnedEntityType.SetContainingPropertyName(name);

        return entityTypeBuilder;
    }

    /// <summary>
    ///     Configures the property name that the entity is mapped to when stored as an embedded document.
    /// </summary>
    /// <remarks>
    ///     See <see href="https://aka.ms/efcore-docs-modeling">Modeling entity types and relationships</see>, and
    ///     <see href="https://aka.ms/efcore-docs-cosmos">Accessing Azure Cosmos DB with EF Core</see> for more information.
    /// </remarks>
    /// <param name="entityTypeBuilder">The builder for the entity type being configured.</param>
    /// <param name="name">The name of the parent property.</param>
    /// <returns>The same builder instance so that multiple calls can be chained.</returns>
    public static OwnedNavigationBuilder<TOwnerEntity, TDependentEntity> ToJsonProperty<TOwnerEntity, TDependentEntity>(
        this OwnedNavigationBuilder<TOwnerEntity, TDependentEntity> entityTypeBuilder,
        string? name)
        where TOwnerEntity : class
        where TDependentEntity : class
    {
        entityTypeBuilder.OwnedEntityType.SetContainingPropertyName(name);

        return entityTypeBuilder;
    }

    /// <summary>
    ///     Configures the property name that the entity is mapped to when stored as an embedded document.
    /// </summary>
    /// <remarks>
    ///     See <see href="https://aka.ms/efcore-docs-modeling">Modeling entity types and relationships</see>, and
    ///     <see href="https://aka.ms/efcore-docs-cosmos">Accessing Azure Cosmos DB with EF Core</see> for more information.
    /// </remarks>
    /// <param name="entityTypeBuilder">The builder for the entity type being configured.</param>
    /// <param name="name">The name of the parent property.</param>
    /// <param name="fromDataAnnotation">Indicates whether the configuration was specified using a data annotation.</param>
    /// <returns>
    ///     The same builder instance if the configuration was applied,
    ///     <see langword="null" /> otherwise.
    /// </returns>
    public static IConventionEntityTypeBuilder? ToJsonProperty(
        this IConventionEntityTypeBuilder entityTypeBuilder,
        string? name,
        bool fromDataAnnotation = false)
    {
        if (!entityTypeBuilder.CanSetJsonProperty(name, fromDataAnnotation))
        {
            return null;
        }

        entityTypeBuilder.Metadata.SetContainingPropertyName(name, fromDataAnnotation);

        return entityTypeBuilder;
    }

    /// <summary>
    ///     Returns a value indicating whether the parent property name to which the entity type is mapped to can be set
    ///     from the current configuration source
    /// </summary>
    /// <remarks>
    ///     See <see href="https://aka.ms/efcore-docs-modeling">Modeling entity types and relationships</see>, and
    ///     <see href="https://aka.ms/efcore-docs-cosmos">Accessing Azure Cosmos DB with EF Core</see> for more information.
    /// </remarks>
    /// <param name="entityTypeBuilder">The builder for the entity type being configured.</param>
    /// <param name="name">The name of the parent property.</param>
    /// <param name="fromDataAnnotation">Indicates whether the configuration was specified using a data annotation.</param>
    /// <returns><see langword="true" /> if the configuration can be applied.</returns>
    public static bool CanSetJsonProperty(
        this IConventionEntityTypeBuilder entityTypeBuilder,
        string? name,
        bool fromDataAnnotation = false)
    {
        Check.NotNull(entityTypeBuilder, nameof(entityTypeBuilder));
        Check.NullButNotEmpty(name, nameof(name));

        return entityTypeBuilder.CanSetAnnotation(KafkaAnnotationNames.PropertyName, name, fromDataAnnotation);
    }

    /// <summary>
    ///     Configures the property that is used to store the partition key.
    /// </summary>
    /// <remarks>
    ///     See <see href="https://aka.ms/efcore-docs-modeling">Modeling entity types and relationships</see>, and
    ///     <see href="https://aka.ms/efcore-docs-cosmos">Accessing Azure Cosmos DB with EF Core</see> for more information.
    /// </remarks>
    /// <param name="entityTypeBuilder">The builder for the entity type being configured.</param>
    /// <param name="name">The name of the partition key property.</param>
    /// <returns>The same builder instance so that multiple calls can be chained.</returns>
    public static EntityTypeBuilder HasPartitionKey(
        this EntityTypeBuilder entityTypeBuilder,
        string? name)
    {
        entityTypeBuilder.Metadata.SetPartitionKeyPropertyName(name);

        return entityTypeBuilder;
    }

    /// <summary>
    ///     Configures the property that is used to store the partition key.
    /// </summary>
    /// <remarks>
    ///     See <see href="https://aka.ms/efcore-docs-modeling">Modeling entity types and relationships</see>, and
    ///     <see href="https://aka.ms/efcore-docs-cosmos">Accessing Azure Cosmos DB with EF Core</see> for more information.
    /// </remarks>
    /// <param name="entityTypeBuilder">The builder for the entity type being configured.</param>
    /// <param name="name">The name of the partition key property.</param>
    /// <returns>The same builder instance so that multiple calls can be chained.</returns>
    public static EntityTypeBuilder<TEntity> HasPartitionKey<TEntity>(
        this EntityTypeBuilder<TEntity> entityTypeBuilder,
        string? name)
        where TEntity : class
    {
        entityTypeBuilder.Metadata.SetPartitionKeyPropertyName(name);

        return entityTypeBuilder;
    }

    /// <summary>
    ///     Configures the property that is used to store the partition key.
    /// </summary>
    /// <remarks>
    ///     See <see href="https://aka.ms/efcore-docs-modeling">Modeling entity types and relationships</see>, and
    ///     <see href="https://aka.ms/efcore-docs-cosmos">Accessing Azure Cosmos DB with EF Core</see> for more information.
    /// </remarks>
    /// <param name="entityTypeBuilder">The builder for the entity type being configured.</param>
    /// <param name="propertyExpression">The  partition key property.</param>
    /// <returns>The same builder instance so that multiple calls can be chained.</returns>
    public static EntityTypeBuilder<TEntity> HasPartitionKey<TEntity, TProperty>(
        this EntityTypeBuilder<TEntity> entityTypeBuilder,
        Expression<Func<TEntity, TProperty>> propertyExpression)
        where TEntity : class
    {
        Check.NotNull(propertyExpression, nameof(propertyExpression));

        entityTypeBuilder.Metadata.SetPartitionKeyPropertyName(propertyExpression.GetMemberAccess().GetSimpleMemberName());

        return entityTypeBuilder;
    }

    /// <summary>
    ///     Configures the property that is used to store the partition key.
    /// </summary>
    /// <remarks>
    ///     See <see href="https://aka.ms/efcore-docs-modeling">Modeling entity types and relationships</see>, and
    ///     <see href="https://aka.ms/efcore-docs-cosmos">Accessing Azure Cosmos DB with EF Core</see> for more information.
    /// </remarks>
    /// <param name="entityTypeBuilder">The builder for the entity type being configured.</param>
    /// <param name="name">The name of the partition key property.</param>
    /// <param name="fromDataAnnotation">Indicates whether the configuration was specified using a data annotation.</param>
    /// <returns>
    ///     The same builder instance if the configuration was applied,
    ///     <see langword="null" /> otherwise.
    /// </returns>
    public static IConventionEntityTypeBuilder? HasPartitionKey(
        this IConventionEntityTypeBuilder entityTypeBuilder,
        string? name,
        bool fromDataAnnotation = false)
    {
        if (!entityTypeBuilder.CanSetPartitionKey(name, fromDataAnnotation))
        {
            return null;
        }

        entityTypeBuilder.Metadata.SetPartitionKeyPropertyName(name, fromDataAnnotation);

        return entityTypeBuilder;
    }

    /// <summary>
    ///     Returns a value indicating whether the property that is used to store the partition key can be set
    ///     from the current configuration source
    /// </summary>
    /// <remarks>
    ///     See <see href="https://aka.ms/efcore-docs-modeling">Modeling entity types and relationships</see>, and
    ///     <see href="https://aka.ms/efcore-docs-cosmos">Accessing Azure Cosmos DB with EF Core</see> for more information.
    /// </remarks>
    /// <param name="entityTypeBuilder">The builder for the entity type being configured.</param>
    /// <param name="name">The name of the partition key property.</param>
    /// <param name="fromDataAnnotation">Indicates whether the configuration was specified using a data annotation.</param>
    /// <returns><see langword="true" /> if the configuration can be applied.</returns>
    public static bool CanSetPartitionKey(
        this IConventionEntityTypeBuilder entityTypeBuilder,
        string? name,
        bool fromDataAnnotation = false)
    {
        Check.NotNull(entityTypeBuilder, nameof(entityTypeBuilder));
        Check.NullButNotEmpty(name, nameof(name));

        return entityTypeBuilder.CanSetAnnotation(KafkaAnnotationNames.PartitionKeyName, name, fromDataAnnotation);
    }

    /// <summary>
    ///     Configures this entity to use CosmosDb etag concurrency checks.
    /// </summary>
    /// <remarks>
    ///     See <see href="https://aka.ms/efcore-docs-modeling">Modeling entity types and relationships</see>, and
    ///     <see href="https://aka.ms/efcore-docs-cosmos">Accessing Azure Cosmos DB with EF Core</see> for more information.
    /// </remarks>
    /// <param name="entityTypeBuilder">The builder for the entity type being configured.</param>
    /// <returns>The same builder instance so that multiple calls can be chained.</returns>
    public static EntityTypeBuilder UseETagConcurrency(this EntityTypeBuilder entityTypeBuilder)
    {
        Check.NotNull(entityTypeBuilder, nameof(entityTypeBuilder));

        entityTypeBuilder.Property<string>("_etag")
            .ValueGeneratedOnAddOrUpdate()
            .IsConcurrencyToken();
        return entityTypeBuilder;
    }

    /// <summary>
    ///     Configures this entity to use CosmosDb etag concurrency checks.
    /// </summary>
    /// <remarks>
    ///     See <see href="https://aka.ms/efcore-docs-modeling">Modeling entity types and relationships</see>, and
    ///     <see href="https://aka.ms/efcore-docs-cosmos">Accessing Azure Cosmos DB with EF Core</see> for more information.
    /// </remarks>
    /// <param name="entityTypeBuilder">The builder for the entity type being configured.</param>
    /// <returns>The same builder instance so that multiple calls can be chained.</returns>
    public static EntityTypeBuilder<TEntity> UseETagConcurrency<TEntity>(this EntityTypeBuilder<TEntity> entityTypeBuilder)
        where TEntity : class
    {
        Check.NotNull(entityTypeBuilder, nameof(entityTypeBuilder));
        UseETagConcurrency((EntityTypeBuilder)entityTypeBuilder);
        return entityTypeBuilder;
    }

    /// <summary>
    ///     Configures the time to live for analytical store in seconds at container scope.
    /// </summary>
    /// <remarks>
    ///     See <see href="https://aka.ms/efcore-docs-modeling">Modeling entity types and relationships</see>, and
    ///     <see href="https://aka.ms/efcore-docs-cosmos">Accessing Azure Cosmos DB with EF Core</see> for more information.
    /// </remarks>
    /// <param name="entityTypeBuilder">The builder for the entity type being configured.</param>
    /// <param name="seconds">The time to live.</param>
    /// <returns>The same builder instance so that multiple calls can be chained.</returns>
    public static EntityTypeBuilder HasAnalyticalStoreTimeToLive(
        this EntityTypeBuilder entityTypeBuilder,
        int? seconds)
    {
        entityTypeBuilder.Metadata.SetAnalyticalStoreTimeToLive(seconds);

        return entityTypeBuilder;
    }

    /// <summary>
    ///     Configures the time to live for analytical store in seconds at container scope.
    /// </summary>
    /// <remarks>
    ///     See <see href="https://aka.ms/efcore-docs-modeling">Modeling entity types and relationships</see>, and
    ///     <see href="https://aka.ms/efcore-docs-cosmos">Accessing Azure Cosmos DB with EF Core</see> for more information.
    /// </remarks>
    /// <param name="entityTypeBuilder">The builder for the entity type being configured.</param>
    /// <param name="seconds">The time to live.</param>
    /// <returns>The same builder instance so that multiple calls can be chained.</returns>
    public static EntityTypeBuilder<TEntity> HasAnalyticalStoreTimeToLive<TEntity>(
        this EntityTypeBuilder<TEntity> entityTypeBuilder,
        int? seconds)
        where TEntity : class
    {
        entityTypeBuilder.Metadata.SetAnalyticalStoreTimeToLive(seconds);

        return entityTypeBuilder;
    }

    /// <summary>
    ///     Configures the time to live for analytical store in seconds at container scope.
    /// </summary>
    /// <remarks>
    ///     See <see href="https://aka.ms/efcore-docs-modeling">Modeling entity types and relationships</see>, and
    ///     <see href="https://aka.ms/efcore-docs-cosmos">Accessing Azure Cosmos DB with EF Core</see> for more information.
    /// </remarks>
    /// <param name="entityTypeBuilder">The builder for the entity type being configured.</param>
    /// <param name="seconds">The time to live.</param>
    /// <param name="fromDataAnnotation">Indicates whether the configuration was specified using a data annotation.</param>
    /// <returns>
    ///     The same builder instance if the configuration was applied,
    ///     <see langword="null" /> otherwise.
    /// </returns>
    public static IConventionEntityTypeBuilder? HasAnalyticalStoreTimeToLive(
        this IConventionEntityTypeBuilder entityTypeBuilder,
        int? seconds,
        bool fromDataAnnotation = false)
    {
        if (!entityTypeBuilder.CanSetAnalyticalStoreTimeToLive(seconds, fromDataAnnotation))
        {
            return null;
        }

        entityTypeBuilder.Metadata.SetAnalyticalStoreTimeToLive(seconds, fromDataAnnotation);

        return entityTypeBuilder;
    }

    /// <summary>
    ///     Returns a value indicating whether the time to live for analytical store can be set
    ///     from the current configuration source
    /// </summary>
    /// <remarks>
    ///     See <see href="https://aka.ms/efcore-docs-modeling">Modeling entity types and relationships</see>, and
    ///     <see href="https://aka.ms/efcore-docs-cosmos">Accessing Azure Cosmos DB with EF Core</see> for more information.
    /// </remarks>
    /// <param name="entityTypeBuilder">The builder for the entity type being configured.</param>
    /// <param name="seconds">The time to live.</param>
    /// <param name="fromDataAnnotation">Indicates whether the configuration was specified using a data annotation.</param>
    /// <returns><see langword="true" /> if the configuration can be applied.</returns>
    public static bool CanSetAnalyticalStoreTimeToLive(
        this IConventionEntityTypeBuilder entityTypeBuilder,
        int? seconds,
        bool fromDataAnnotation = false)
    {
        Check.NotNull(entityTypeBuilder, nameof(entityTypeBuilder));

        return entityTypeBuilder.CanSetAnnotation(KafkaAnnotationNames.AnalyticalStoreTimeToLive, seconds, fromDataAnnotation);
    }

    /// <summary>
    ///     Configures the default time to live in seconds at container scope.
    /// </summary>
    /// <remarks>
    ///     See <see href="https://aka.ms/efcore-docs-modeling">Modeling entity types and relationships</see>, and
    ///     <see href="https://aka.ms/efcore-docs-cosmos">Accessing Azure Cosmos DB with EF Core</see> for more information.
    /// </remarks>
    /// <param name="entityTypeBuilder">The builder for the entity type being configured.</param>
    /// <param name="seconds">The time to live.</param>
    /// <returns>The same builder instance so that multiple calls can be chained.</returns>
    public static EntityTypeBuilder HasDefaultTimeToLive(
        this EntityTypeBuilder entityTypeBuilder,
        int? seconds)
    {
        entityTypeBuilder.Metadata.SetDefaultTimeToLive(seconds);

        return entityTypeBuilder;
    }

    /// <summary>
    ///     Configures the default time to live in seconds at container scope.
    /// </summary>
    /// <remarks>
    ///     See <see href="https://aka.ms/efcore-docs-modeling">Modeling entity types and relationships</see>, and
    ///     <see href="https://aka.ms/efcore-docs-cosmos">Accessing Azure Cosmos DB with EF Core</see> for more information.
    /// </remarks>
    /// <param name="entityTypeBuilder">The builder for the entity type being configured.</param>
    /// <param name="seconds">The time to live.</param>
    /// <returns>The same builder instance so that multiple calls can be chained.</returns>
    public static EntityTypeBuilder<TEntity> HasDefaultTimeToLive<TEntity>(
        this EntityTypeBuilder<TEntity> entityTypeBuilder,
        int? seconds)
        where TEntity : class
    {
        entityTypeBuilder.Metadata.SetDefaultTimeToLive(seconds);

        return entityTypeBuilder;
    }

    /// <summary>
    ///     Configures the default time to live in seconds at container scope.
    /// </summary>
    /// <remarks>
    ///     See <see href="https://aka.ms/efcore-docs-modeling">Modeling entity types and relationships</see>, and
    ///     <see href="https://aka.ms/efcore-docs-cosmos">Accessing Azure Cosmos DB with EF Core</see> for more information.
    /// </remarks>
    /// <param name="entityTypeBuilder">The builder for the entity type being configured.</param>
    /// <param name="seconds">The time to live.</param>
    /// <param name="fromDataAnnotation">Indicates whether the configuration was specified using a data annotation.</param>
    /// <returns>
    ///     The same builder instance if the configuration was applied,
    ///     <see langword="null" /> otherwise.
    /// </returns>
    public static IConventionEntityTypeBuilder? HasDefaultTimeToLive(
        this IConventionEntityTypeBuilder entityTypeBuilder,
        int? seconds,
        bool fromDataAnnotation = false)
    {
        if (!entityTypeBuilder.CanSetDefaultTimeToLive(seconds, fromDataAnnotation))
        {
            return null;
        }

        entityTypeBuilder.Metadata.SetDefaultTimeToLive(seconds, fromDataAnnotation);

        return entityTypeBuilder;
    }

    /// <summary>
    ///     Returns a value indicating whether the default time to live can be set
    ///     from the current configuration source
    /// </summary>
    /// <remarks>
    ///     See <see href="https://aka.ms/efcore-docs-modeling">Modeling entity types and relationships</see>, and
    ///     <see href="https://aka.ms/efcore-docs-cosmos">Accessing Azure Cosmos DB with EF Core</see> for more information.
    /// </remarks>
    /// <param name="entityTypeBuilder">The builder for the entity type being configured.</param>
    /// <param name="seconds">The time to live.</param>
    /// <param name="fromDataAnnotation">Indicates whether the configuration was specified using a data annotation.</param>
    /// <returns><see langword="true" /> if the configuration can be applied.</returns>
    public static bool CanSetDefaultTimeToLive(
        this IConventionEntityTypeBuilder entityTypeBuilder,
        int? seconds,
        bool fromDataAnnotation = false)
    {
        Check.NotNull(entityTypeBuilder, nameof(entityTypeBuilder));

        return entityTypeBuilder.CanSetAnnotation(KafkaAnnotationNames.DefaultTimeToLive, seconds, fromDataAnnotation);
    }







    /// <summary>
    ///     Configures a query used to provide data for an entity type.
    /// </summary>
    /// <remarks>
    ///     See <see href="https://aka.ms/efcore-docs-modeling">Modeling entity types and relationships</see>, and
    ///     <see href="https://github.com/masesgroup/KEFCore">The EF Core Kafka database provider</see> for more information and examples.
    /// </remarks>
    /// <param name="entityTypeBuilder">The builder for the entity type being configured.</param>
    /// <param name="query">The query that will provide the underlying data for the entity type.</param>
    /// <returns>The same builder instance so that multiple calls can be chained.</returns>
    public static EntityTypeBuilder ToKafkaQuery(
        this EntityTypeBuilder entityTypeBuilder,
        LambdaExpression? query)
    {
        Check.NotNull(query, nameof(query));

        entityTypeBuilder.Metadata.SetKafkaQuery(query);

        return entityTypeBuilder;
    }

    /// <summary>
    ///     Configures a query used to provide data for an entity type.
    /// </summary>
    /// <remarks>
    ///     See <see href="https://aka.ms/efcore-docs-modeling">Modeling entity types and relationships</see>, and
    ///     <see href="https://github.com/masesgroup/KEFCore">The EF Core Kafka database provider</see> for more information and examples.
    /// </remarks>
    /// <param name="entityTypeBuilder">The builder for the entity type being configured.</param>
    /// <param name="query">The query that will provide the underlying data for the entity type.</param>
    /// <returns>The same builder instance so that multiple calls can be chained.</returns>
    public static EntityTypeBuilder<TEntity> ToKafkaQuery<TEntity>(
        this EntityTypeBuilder<TEntity> entityTypeBuilder,
        Expression<Func<IQueryable<TEntity>>> query)
        where TEntity : class
    {
        Check.NotNull(query, nameof(query));

        entityTypeBuilder.Metadata.SetKafkaQuery(query);

        return entityTypeBuilder;
    }

    /// <summary>
    ///     Configures a query used to provide data for an entity type.
    /// </summary>
    /// <remarks>
    ///     See <see href="https://aka.ms/efcore-docs-modeling">Modeling entity types and relationships</see>, and
    ///     <see href="https://github.com/masesgroup/KEFCore">The EF Core Kafka database provider</see> for more information and examples.
    /// </remarks>
    /// <param name="entityTypeBuilder">The builder for the entity type being configured.</param>
    /// <param name="query">The query that will provide the underlying data for the entity type.</param>
    /// <param name="fromDataAnnotation">Indicates whether the configuration was specified using a data annotation.</param>
    /// <returns>
    ///     The same builder instance if the query was set, <see langword="null" /> otherwise.
    /// </returns>
    public static IConventionEntityTypeBuilder? ToKafkaQuery(
        this IConventionEntityTypeBuilder entityTypeBuilder,
        LambdaExpression? query,
        bool fromDataAnnotation = false)
    {
        if (CanSetKafkaQuery(entityTypeBuilder, query, fromDataAnnotation))
        {
            entityTypeBuilder.Metadata.SetKafkaQuery(query, fromDataAnnotation);

            return entityTypeBuilder;
        }

        return null;
    }

    /// <summary>
    ///     Returns a value indicating whether the given Kafka query can be set from the current configuration source.
    /// </summary>
    /// <remarks>
    ///     See <see href="https://aka.ms/efcore-docs-modeling">Modeling entity types and relationships</see>, and
    ///     <see href="https://github.com/masesgroup/KEFCore">The EF Core Kafka database provider</see> for more information and examples.
    /// </remarks>
    /// <param name="entityTypeBuilder">The builder for the entity type being configured.</param>
    /// <param name="query">The query that will provide the underlying data for the keyless entity type.</param>
    /// <param name="fromDataAnnotation">Indicates whether the configuration was specified using a data annotation.</param>
    /// <returns><see langword="true" /> if the given Kafka query can be set.</returns>
    public static bool CanSetKafkaQuery(
        this IConventionEntityTypeBuilder entityTypeBuilder,
        LambdaExpression? query,
        bool fromDataAnnotation = false)
#pragma warning disable EF1001 // Internal EF Core API usage.
#pragma warning disable CS0612 // Il tipo o il membro è obsoleto
        => entityTypeBuilder.CanSetAnnotation(CoreAnnotationNames.DefiningQuery, query, fromDataAnnotation);
#pragma warning restore CS0612 // Il tipo o il membro è obsoleto
#pragma warning restore EF1001 // Internal EF Core API usage.
}
