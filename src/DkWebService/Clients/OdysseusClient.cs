using DkWebService.EndPoint.Clients.Interfaces;

namespace DkWebService.EndPoint.Clients;

public class OdysseusClient : BaseClient, IOdysseusClient
{
    public OdysseusClient(IConfiguration configuration, ILogger<OdysseusClient> logger)
        : base(configuration, logger)
    {
    }

    public async Task<string[]> GetSomethingFromTheOutsideWorld(int userId)
    {
        try
        {
            return await ExecuteWithPolicyAsync(
                async context =>
                {
                    if (userId == -1)
                    {
                        throw new ArgumentException($"Invalid userId.", nameof(userId));
                    }

                    var response = await DoSomeBusinessThingAsync();
                    return response;
                },
                $"{nameof(OdysseusClient)}.{nameof(GetSomethingFromTheOutsideWorld)}");
        }
        catch (Exception e)
        {
            Logger.LogError(
                e,
                "[{Method}] - Failed to get strings for User {UserId} from Odysseus. Some AppSettings value: {SomeAppSettingValue}.",
                $"{nameof(OdysseusClient)}.{nameof(GetSomethingFromTheOutsideWorld)}",
                userId,
                Configuration.GetValue<string>("SomeAppSettingValue"));

            throw;
        }
    }

    private async Task<string[]> DoSomeBusinessThingAsync()
    {
        var str = new string[] { "Business", " ", "is", " ", "done", "!" };

        await Task.Run(() =>
        {
            Console.WriteLine(string.Join("", str));
        });

        return str;
    }
}
