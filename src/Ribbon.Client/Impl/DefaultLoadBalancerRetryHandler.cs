﻿using Ribbon.Client.Util;
using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Ribbon.Client.Impl
{
    public class DefaultLoadBalancerRetryHandler : IRetryHandler
    {
        private readonly List<Type> _retriable = new List<Type>
        {
            typeof(TimeoutException),typeof(SocketException)
        };

        private readonly List<Type> _circuitRelated = new List<Type>
        {
            typeof(TimeoutException),typeof(SocketException)
        };

        protected uint RetrySameServer { get; set; }

        protected uint RetryNextServer { get; set; }

        protected bool RetryEnabled { get; set; }

        /// <inheritdoc/>
        public DefaultLoadBalancerRetryHandler(uint retrySameServer, uint retryNextServer, bool retryEnabled)
        {
            RetrySameServer = Math.Max(retrySameServer, 0);
            RetryNextServer = Math.Max(retryNextServer, 0);
            RetryEnabled = retryEnabled;
        }

        public DefaultLoadBalancerRetryHandler(RetryHandlerConfig options)
            : this(options.MaxAutoRetries, options.MaxAutoRetriesNextServer, options.OkToRetryOnAllOperations)
        {
        }

        #region Implementation of IRetryHandler

        /// <inheritdoc/>
        public virtual bool IsRetriableException(Exception exception, bool sameServer)
        {
            return RetryEnabled && (!sameServer || Utils.IsPresentAsException(exception, GetRetriableExceptions()));
        }

        /// <inheritdoc/>
        public virtual bool IsCircuitTrippingException(Exception exception)
        {
            return Utils.IsPresentAsException(exception, GetCircuitRelatedExceptions());
        }

        /// <inheritdoc/>
        public uint GetMaxRetriesOnSameServer()
        {
            return RetrySameServer;
        }

        /// <inheritdoc/>
        public uint GetMaxRetriesOnNextServer()
        {
            return RetryNextServer;
        }

        #endregion Implementation of IRetryHandler

        protected virtual IReadOnlyCollection<Type> GetRetriableExceptions()
        {
            return _retriable;
        }

        protected virtual IReadOnlyCollection<Type> GetCircuitRelatedExceptions()
        {
            return _circuitRelated;
        }
    }
}