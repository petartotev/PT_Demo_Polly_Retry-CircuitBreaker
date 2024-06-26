namespace DkWebService.EndPoint.Clients.Interfaces
{
    public interface IPerseusClient
    {
        Task<bool> CheckIfDepositIsFraudAsync(int depositId);
    }
}
