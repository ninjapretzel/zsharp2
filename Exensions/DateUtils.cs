using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public static class DateUtils {

	public static long UnixTimestamp(this DateTime date) {
		DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
		TimeSpan diff = DateTime.Now.Subtract(epoch);
		return (long)diff.TotalMilliseconds;
	}

	public static float TimeUntilNow(this DateTime date) {
		return (float)DateTime.Now.Subtract(date).TotalSeconds;
	}
	
	//Round a date 'down' to its day
	public static DateTime RoundToDay(this DateTime date) {
		return new DateTime(date.Year, date.Month, date.Day);
	}
	
	//Round a date 'down' to its hour
	public static DateTime RoundToHour(this DateTime date) {
		return new DateTime(date.Year, date.Month, date.Day, date.Hour, 0, 0);
	}
	
	//Round a date 'down' to its minute
	public static DateTime RoundToMinute(this DateTime date) {
		return new DateTime(date.Year, date.Month, date.Day, date.Hour, date.Minute, 0);
	}
	
}
