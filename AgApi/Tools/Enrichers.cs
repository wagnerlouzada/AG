using Serilog;
using Serilog.Configuration;
using System;
using Serilog.Core;
using Serilog.Events;
using Microsoft.AspNetCore.Http;
using System.Web;
using Serilog.Enrichers;
using AppV.Models.CQRS.Handlers;

namespace Serilog.Enrichers
{

        public class UserNameEnricher : ILogEventEnricher
        {
            LogEventProperty _cachedProperty;

            public const string UserNamePropertyName = "UserName";

            /// <summary>
            /// Enrich the log event.
            /// </summary>
            /// <param name="logEvent">The log event to enrich.</param>
            /// <param name="propertyFactory">Factory for creating new properties to add to the event.</param>
            public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
            {
                logEvent.AddPropertyIfAbsent(GetLogEventProperty(propertyFactory));
            }

            private LogEventProperty GetLogEventProperty(ILogEventPropertyFactory propertyFactory)
            {
                // Don't care about thread-safety, in the worst case the field gets overwritten and one
                // property will be GCed
                if (_cachedProperty == null)
                    _cachedProperty = CreateProperty(propertyFactory);

                return _cachedProperty;
            }

            // Qualify as uncommon-path
            private static LogEventProperty CreateProperty(ILogEventPropertyFactory propertyFactory)
            {
                String username = "";
            
                //if (HttpContext.Current.User.Identity.IsAuthenticated)
                //    username = HttpContext.Current.User.Identity.Name;

                return propertyFactory.CreateProperty(UserNamePropertyName, username);
            }

        }
    }

    public static class LoggerConfigurationExtensions
    {
        /// <summary>
        /// Enrich log events with a MachineName property containing the current <see cref="Environment.MachineName"/>.
        /// </summary>
        /// <param name="enrichmentConfiguration">Logger enrichment configuration.</param>
        /// <returns>Configuration object allowing method chaining.</returns>
        public static LoggerConfiguration WithUserName(
           this LoggerEnrichmentConfiguration enrichmentConfiguration)
        {
            if (enrichmentConfiguration == null) throw new ArgumentNullException(nameof(enrichmentConfiguration));
            return enrichmentConfiguration.With<UserNameEnricher>();
        }
    }

