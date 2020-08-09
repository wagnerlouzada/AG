using MediatR;
using MediatR.Pipeline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Diagnostics;
using Serilog;

namespace Base.Controllers
{
    public class RequestLoggerBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly ILogger _logger;

        public RequestLoggerBehavior(ILogger logger)
        {
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            _logger.Debug("Debug Mediator (Handling) {Request_Type} - {@Request_Content}", typeof(TRequest).Name, request);

            var response = await next();

            _logger.Debug("Debug Mediator (Handling) {Response_Type} - {@Response_Content}", typeof(TResponse).Name, response);

            return response;
        }

        public Task Process(TRequest request)
        {
            _logger.Debug("Debug Mediator...");

            return Task.CompletedTask;
        }

    }
}