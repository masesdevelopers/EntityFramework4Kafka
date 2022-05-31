// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using MASES.EntityFrameworkCore.KNet.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Utilities;

// ReSharper disable once CheckNamespace
namespace MASES.EntityFrameworkCore.KNet
{
    /// <summary>
    ///     Cosmos-specific extension methods for <see cref="ModelBuilder" />.
    /// </summary>
    /// <remarks>
    ///     See <see href="https://aka.ms/efcore-docs-modeling">Modeling entity types and relationships</see>, and
    ///     <see href="https://github.com/masesgroup/KEFCore">The EF Core Kafka database provider</see> for more information.
    /// </remarks>
    public static class KafkaModelBuilderExtensions
    {
        /// <summary>
        ///     Configures the default container name that will be used if no name
        ///     is explicitly configured for an entity type.
        /// </summary>
        /// <remarks>
        ///     See <see href="https://aka.ms/efcore-docs-modeling">Modeling entity types and relationships</see>, and
        ///     <see href="https://github.com/masesgroup/KEFCore">The EF Core Kafka database provider</see> for more information.
        /// </remarks>
        /// <param name="modelBuilder">The model builder.</param>
        /// <param name="name">The default container name.</param>
        /// <returns>The same builder instance so that multiple calls can be chained.</returns>
        public static ModelBuilder HasDefaultContainer(
            this ModelBuilder modelBuilder,
            string? name)
        {
            Check.NotNull(modelBuilder, nameof(modelBuilder));
            Check.NullButNotEmpty(name, nameof(name));

            modelBuilder.Model.SetDefaultContainer(name);

            return modelBuilder;
        }

        /// <summary>
        ///     Configures the default container name that will be used if no name
        ///     is explicitly configured for an entity type.
        /// </summary>
        /// <remarks>
        ///     See <see href="https://aka.ms/efcore-docs-modeling">Modeling entity types and relationships</see>, and
        ///     <see href="https://github.com/masesgroup/KEFCore">The EF Core Kafka database provider</see> for more information.
        /// </remarks>
        /// <param name="modelBuilder">The model builder.</param>
        /// <param name="name">The default container name.</param>
        /// <param name="fromDataAnnotation">Indicates whether the configuration was specified using a data annotation.</param>
        /// <returns>
        ///     The same builder instance if the configuration was applied,
        ///     <see langword="null" /> otherwise.
        /// </returns>
        public static IConventionModelBuilder? HasDefaultContainer(
            this IConventionModelBuilder modelBuilder,
            string? name,
            bool fromDataAnnotation = false)
        {
            if (!modelBuilder.CanSetDefaultContainer(name, fromDataAnnotation))
            {
                return null;
            }

            modelBuilder.Metadata.SetDefaultContainer(name, fromDataAnnotation);

            return modelBuilder;
        }

        /// <summary>
        ///     Returns a value indicating whether the given container name can be set as default.
        /// </summary>
        /// <remarks>
        ///     See <see href="https://aka.ms/efcore-docs-modeling">Modeling entity types and relationships</see>, and
        ///     <see href="https://github.com/masesgroup/KEFCore">The EF Core Kafka database provider</see> for more information.
        /// </remarks>
        /// <param name="modelBuilder">The model builder.</param>
        /// <param name="name">The default container name.</param>
        /// <param name="fromDataAnnotation">Indicates whether the configuration was specified using a data annotation.</param>
        /// <returns><see langword="true" /> if the given container name can be set as default.</returns>
        public static bool CanSetDefaultContainer(
            this IConventionModelBuilder modelBuilder,
            string? name,
            bool fromDataAnnotation = false)
        {
            Check.NullButNotEmpty(name, nameof(name));

            return modelBuilder.CanSetAnnotation(KafkaAnnotationNames.ContainerName, name, fromDataAnnotation);
        }
    }
}
