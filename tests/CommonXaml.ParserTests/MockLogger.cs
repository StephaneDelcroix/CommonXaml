// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace CommonXaml.ParserTests;

class MockLogger : ILogger
{
    public MockLogger(LogLevel logLevel)
    {
        this.LogLevel = logLevel;
    }

    public class LogEvent
    {
        public LogLevel LogLevel { get; set; }
        public EventId EventId { get; set; }
        public Exception? Exception { get; set; }
        public string? Message { get; set; }
    }

    public List<LogEvent> Events = new();
    readonly LogLevel LogLevel;

    public IDisposable BeginScope<TState>(TState state) => new Scope<TState>();

    public bool IsEnabled(LogLevel logLevel) => logLevel>=LogLevel;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        if (logLevel < LogLevel)
            return;

        Events.Add(new LogEvent {
            LogLevel = logLevel,
            EventId = eventId,
            Exception = exception,
            Message = formatter(state, exception),
        }) ;
    }

    class Scope<TState> : IDisposable
    {
        public void Dispose()
        {
        }
    }
}