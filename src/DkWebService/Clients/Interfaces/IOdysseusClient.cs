namespace DkWebService.EndPoint.Clients.Interfaces;

public interface IOdysseusClient
{
    Task<string[]> GetSomethingFromTheOutsideWorld(int userId);
}
