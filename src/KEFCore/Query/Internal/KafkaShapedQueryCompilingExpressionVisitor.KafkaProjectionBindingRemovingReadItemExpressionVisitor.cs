// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

namespace MASES.EntityFrameworkCore.KNet.Query.Internal;

public partial class KafkaShapedQueryCompilingExpressionVisitor
{
    private sealed class KafkaProjectionBindingRemovingReadItemExpressionVisitor : KafkaProjectionBindingRemovingExpressionVisitorBase
    {
        private readonly ReadItemExpression _readItemExpression;

        public KafkaProjectionBindingRemovingReadItemExpressionVisitor(
            ReadItemExpression readItemExpression,
            ParameterExpression jObjectParameter,
            bool trackQueryResults)
            : base(jObjectParameter, trackQueryResults)
        {
            _readItemExpression = readItemExpression;
        }

        protected override ProjectionExpression GetProjection(ProjectionBindingExpression _)
            => _readItemExpression.ProjectionExpression;
    }
}
