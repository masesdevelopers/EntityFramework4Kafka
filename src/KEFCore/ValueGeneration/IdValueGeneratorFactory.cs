// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using MASES.EntityFrameworkCore.KNet.ValueGeneration.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace MASES.EntityFrameworkCore.KNet.ValueGeneration
{
    /// <summary>
    ///     A factory that creates value generators for the 'id' property that combines the primary key values.
    /// </summary>
    /// <remarks>
    ///     See <see href="https://aka.ms/efcore-docs-value-generation">EF Core value generation</see>, and
    ///     <see href="https://github.com/masesgroup/KEFCore">The EF Core Kafka database provider</see> for more information.
    /// </remarks>
    public class IdValueGeneratorFactory : ValueGeneratorFactory
    {
        /// <inheritdoc />
        public override ValueGenerator Create(IProperty property, IEntityType entityType)
            => new IdValueGenerator();
    }
}
