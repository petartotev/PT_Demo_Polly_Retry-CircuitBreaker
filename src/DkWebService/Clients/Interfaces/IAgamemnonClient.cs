namespace DkWebService.EndPoint.Clients.Interfaces
{
    public interface IAgamemnonClient
    {
        Task<int> GetCalculationsByAgamemnonApi(int eventId);
    }
}
