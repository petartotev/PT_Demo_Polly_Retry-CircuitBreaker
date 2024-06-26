namespace DkWebService.EndPoint.Clients.Interfaces;

public interface IOdysseusClient
{
    Task<string[]> GetSomethingFromTheOutsideWorldAsync(int userId);
}
