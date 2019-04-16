﻿using System.Collections.Generic;
using System.Runtime.Serialization;
using Elasticsearch.Net;

namespace Nest
{
	public interface IPutPrivilegesResponse : IResponse
	{
		IReadOnlyDictionary<string, IDictionary<string, PutPrivilegesStatus>> Applications { get; }
	}

	[JsonFormatter(typeof(DictionaryResponseFormatter<PutPrivilegesResponse, string, IDictionary<string, PutPrivilegesStatus>>))]
	public class PutPrivilegesResponse : DictionaryResponseBase<string, IDictionary<string, PutPrivilegesStatus>>, IPutPrivilegesResponse
	{
		[IgnoreDataMember]
		public IReadOnlyDictionary<string, IDictionary<string, PutPrivilegesStatus>> Applications => Self.BackingDictionary;
	}

	public class PutPrivilegesStatus
	{
		/// <summary>
		/// Whether the privilege has been created or updated.
		/// When an existing privilege is updated, created is set to false.
		/// </summary>
		[DataMember(Name = "created")]
		public bool Created { get; internal set; }
	}
}
