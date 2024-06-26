namespace DkWebService.EndPoint.Clients.Interfaces
{
    public interface IAgamemnonClient
    {
        Task<int> GetCalculationsByAgamemnonApiAsync(int eventId);
    }
}
