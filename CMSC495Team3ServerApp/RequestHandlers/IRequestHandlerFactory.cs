using System.Collections;

namespace CMSC495Team3ServerApp.RequestHandlers
{
    public interface IRequestHandlerFactory
    {
        IRequestHandler Get(string urlSegment);
    }
}