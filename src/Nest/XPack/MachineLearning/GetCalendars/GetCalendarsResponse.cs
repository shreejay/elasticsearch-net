﻿using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
	/// <summary>
	/// Retrieves configuration information for calendars.
	/// </summary>
	public interface IGetCalendarsResponse : IResponse
	{
		/// <summary>
		/// The count of calendars.
		/// </summary>
		[DataMember(Name = "count")]
		long Count { get; }

		/// <summary>
		/// An array of calendar resources.
		/// </summary>
		[DataMember(Name = "calendars")]
		IReadOnlyCollection<Calendar> Calendars { get; }
	}

	public class Calendar
	{
		[DataMember(Name = "calendar_id")]
		public string CalendarId { get; set; }

		[DataMember(Name = "job_ids")]
		public IReadOnlyCollection<string> JobIds { get; set; } = EmptyReadOnly<string>.Collection;

		[DataMember(Name = "description")]
		public string Description { get; set; }
	}

	public class GetCalendarsResponse : ResponseBase, IGetCalendarsResponse
	{
		/// <inheritdoc cref="IGetCalendarsResponse.Count" />
		public long Count { get; internal set; }

		/// <inheritdoc cref="IGetCalendarsResponse.Calendars" />
		public IReadOnlyCollection<Calendar> Calendars { get; internal set; } = EmptyReadOnly<Calendar>.Collection;
	}
}
