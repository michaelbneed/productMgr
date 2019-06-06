using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Polly;
using Polly.Retry;
using Polly.Timeout;
using Polly.Wrap;

namespace PM.Common.Extensions
{
	public static class Resilience
	{
		public static AsyncPolicyWrap ExecutePolicy()
		{
			int maxRetryAttempts = 3;
			var pauseBetweenFailures = TimeSpan.FromSeconds(3);

			var timeoutSeconds = TimeSpan.FromSeconds(15);
			int allowedExceptionsBeforeBreak = 4;
			
			// Retry 
			var retryPolicy = Policy
				.Handle<Exception>()
				.Or<HttpRequestException>()
				.Or<SqlException>()
				.WaitAndRetryAsync(
					maxRetryAttempts,
					i => pauseBetweenFailures);

			// TimeOut 
			var timeOutPolicy = Policy
				.TimeoutAsync(timeoutSeconds, TimeoutStrategy.Optimistic);

			// Break
			var circuitBreaker = Policy
				.Handle<Exception>()
				.Or<HttpRequestException>()
				.Or<SqlException>()
				.CircuitBreakerAsync(allowedExceptionsBeforeBreak, pauseBetweenFailures);

			//Combine the policies
			var policyWrap = Policy.WrapAsync(retryPolicy, circuitBreaker, timeOutPolicy);
			return policyWrap;
		}
	}
}
