﻿using System;
using System.Collections.Generic;
using System.Linq;
using Elastic.Xunit.XunitPlumbing;
using FluentAssertions;
using Nest;
using Tests.Core.Extensions;
using Tests.Core.ManagedElasticsearch.Clusters;
using Tests.Domain;
using Tests.Framework.Integration;
using static Nest.Infer;

namespace Tests.Aggregations.Bucket.Filter
{
	/**
	 * Defines a single bucket of all the documents in the current document set context that match a specified filter.
	 * Often this will be used to narrow down the current aggregation context to a specific set of documents.
	 *
	 * Be sure to read the Elasticsearch documentation on {ref_current}/search-aggregations-bucket-filter-aggregation.html[Filter Aggregation]
	*/
	public class FilterAggregationUsageTests : AggregationUsageTestBase
	{
		public static string FirstNameToFind = Project.First.LeadDeveloper.FirstName.ToLowerInvariant();

		public FilterAggregationUsageTests(ReadOnlyCluster i, EndpointUsage usage) : base(i, usage) { }

		protected override object AggregationJson => new
		{
			bethels_projects = new
			{
				filter = new
				{
					term = new Dictionary<string, object>
					{
						{ "leadDeveloper.firstName", new { value = FirstNameToFind } }
					}
				},
				aggs = new
				{
					project_tags = new { terms = new { field = "curatedTags.name.keyword" } }
				}
			}
		};

		protected override Func<AggregationContainerDescriptor<Project>, IAggregationContainer> FluentAggs => a => a
			.Filter("bethels_projects", date => date
				.Filter(q => q.Term(p => p.LeadDeveloper.FirstName, FirstNameToFind))
				.Aggregations(childAggs => childAggs
					.Terms("project_tags", avg => avg.Field(p => p.CuratedTags.First().Name.Suffix("keyword")))
				)
			);

		protected override AggregationDictionary InitializerAggs =>
			new FilterAggregation("bethels_projects")
			{
				Filter = new TermQuery { Field = Field<Project>(p => p.LeadDeveloper.FirstName), Value = FirstNameToFind },
				Aggregations =
					new TermsAggregation("project_tags") { Field = Field<Project>(p => p.CuratedTags.First().Name.Suffix("keyword")) }
			};

		protected override void ExpectResponse(ISearchResponse<Project> response)
		{
			/** ==== Handling Responses
			* The `AggregateDictionary found on `.Aggregations` on `ISearchResponse<T>` has several helper methods
			* so we can fetch our aggregation results easily in the correct type.
			 * <<handling-aggregate-response, Be sure to read more about these helper methods>>
			*/
			response.ShouldBeValid();

			var filterAgg = response.Aggregations.Filter("bethels_projects");
			filterAgg.Should().NotBeNull();
			filterAgg.DocCount.Should().BeGreaterThan(0);
			var tags = filterAgg.Terms("project_tags");
			tags.Should().NotBeNull();
			tags.Buckets.Should().NotBeEmpty();
		}
	}

	/**[float]
	* === Empty Filter
	* When the collection of filters is empty or all are conditionless, NEST will serialize them
	* to an empty object.
	*/
	[SkipVersion("6.0.0-alpha1",
		"https://github.com/elastic/elasticsearch/issues/17518 && 6.0 https://github.com/elastic/elasticsearch/pull/17542#issuecomment-300796197")]
	public class EmptyFilterAggregationUsageTests : AggregationUsageTestBase
	{
		public EmptyFilterAggregationUsageTests(ReadOnlyCluster i, EndpointUsage usage) : base(i, usage) { }

		protected override object AggregationJson => new
		{
			empty_filter = new
			{
				filter = new { }
			}
		};

		protected override bool ExpectIsValid => false;
		protected override int ExpectStatusCode => 400;

		protected override Func<AggregationContainerDescriptor<Project>, IAggregationContainer> FluentAggs => a => a
			.Filter("empty_filter", date => date
				.Filter(f => f
					.Bool(b => b
						.Filter(new QueryContainer[0])
					)
				)
			);

		protected override AggregationDictionary InitializerAggs =>
			new FilterAggregation("empty_filter")
			{
				Filter = new BoolQuery
				{
					Filter = new List<QueryContainer>()
				}
			};

		protected override void ExpectResponse(ISearchResponse<Project> response) => response.ShouldNotBeValid();
	}

	//reproduce of https://github.com/elastic/elasticsearch-net/issues/1931
	// hide
	public class InlineScriptFilterAggregationUsageTests : AggregationUsageTestBase
	{
		private readonly string _aggName = "script_filter";
		private readonly string _ctxNumberofCommits = "doc['numberOfCommits'].value > 0";

		public InlineScriptFilterAggregationUsageTests(ReadOnlyCluster i, EndpointUsage usage) : base(i, usage) { }

		protected override object AggregationJson => new
		{
			script_filter = new
			{
				filter = new
				{
					script = new
					{
						script = new
						{
							source = _ctxNumberofCommits,
						}
					}
				}
			}
		};

		protected override Func<AggregationContainerDescriptor<Project>, IAggregationContainer> FluentAggs => a => a
			.Filter(_aggName, date => date
				.Filter(f => f
					.Script(b => b
						.Script(ss => ss
							.Source(_ctxNumberofCommits)
						)
					)
				)
			);

		protected override AggregationDictionary InitializerAggs =>
			new FilterAggregation(_aggName)
			{
				Filter = new ScriptQuery
				{
					Script = new InlineScript(_ctxNumberofCommits)
				}
			};

		protected override void ExpectResponse(ISearchResponse<Project> response)
		{
			response.ShouldBeValid();
			response.Aggregations.Filter(_aggName).DocCount.Should().BeGreaterThan(0);
		}
	}
}
