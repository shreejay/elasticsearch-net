﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Elasticsearch.Net;

namespace Nest
{
	public partial interface IElasticClient
	{
		/// <summary>
		/// Gets follower stats. Returns shard-level stats about the following tasks associated with each shard for the specified indices.
		/// </summary>
		IFollowIndexStatsResponse FollowIndexStats(Indices indices, Func<FollowIndexStatsDescriptor, IFollowIndexStatsRequest> selector = null);

		/// <inheritdoc cref="FollowIndexStats(Indices, System.Func{Nest.FollowIndexStatsDescriptor,Nest.IFollowIndexStatsRequest})" />
		IFollowIndexStatsResponse FollowIndexStats(IFollowIndexStatsRequest request);

		/// <inheritdoc cref="FollowIndexStats(Indices, System.Func{Nest.FollowIndexStatsDescriptor,Nest.IFollowIndexStatsRequest})" />
		Task<IFollowIndexStatsResponse> FollowIndexStatsAsync(Indices indices, Func<FollowIndexStatsDescriptor, IFollowIndexStatsRequest> selector = null,
			CancellationToken ct = default
		);

		/// <inheritdoc cref="FollowIndexStats(Indices, System.Func{Nest.FollowIndexStatsDescriptor,Nest.IFollowIndexStatsRequest})" />
		Task<IFollowIndexStatsResponse> FollowIndexStatsAsync(IFollowIndexStatsRequest request, CancellationToken ct = default);
	}

	public partial class ElasticClient
	{
		/// <inheritdoc cref="FollowIndexStats(Indices, System.Func{Nest.FollowIndexStatsDescriptor,Nest.IFollowIndexStatsRequest})" />
		public IFollowIndexStatsResponse FollowIndexStats(Indices indices, Func<FollowIndexStatsDescriptor, IFollowIndexStatsRequest> selector = null) =>
			FollowIndexStats(selector.InvokeOrDefault(new FollowIndexStatsDescriptor(indices)));

		/// <inheritdoc cref="FollowIndexStats(Indices, System.Func{Nest.FollowIndexStatsDescriptor,Nest.IFollowIndexStatsRequest})" />
		public IFollowIndexStatsResponse FollowIndexStats(IFollowIndexStatsRequest request) =>
			DoRequest<IFollowIndexStatsRequest, FollowIndexStatsResponse>(request, request.RequestParameters);

		/// <inheritdoc cref="FollowIndexStats(Indices, System.Func{Nest.FollowIndexStatsDescriptor,Nest.IFollowIndexStatsRequest})" />
		public Task<IFollowIndexStatsResponse> FollowIndexStatsAsync(
			Indices indices,
			Func<FollowIndexStatsDescriptor, IFollowIndexStatsRequest> selector = null,
			CancellationToken ct = default
		) => FollowIndexStatsAsync(selector.InvokeOrDefault(new FollowIndexStatsDescriptor(indices)), ct);

		/// <inheritdoc cref="FollowIndexStats(Indices, System.Func{Nest.FollowIndexStatsDescriptor,Nest.IFollowIndexStatsRequest})" />
		public Task<IFollowIndexStatsResponse> FollowIndexStatsAsync(IFollowIndexStatsRequest request, CancellationToken ct = default) =>
			DoRequestAsync<IFollowIndexStatsRequest, IFollowIndexStatsResponse, FollowIndexStatsResponse>(request, request.RequestParameters, ct);
	}
}
