using Amazon.Lambda.Core;
using System.Text;
using Newtonsoft.Json;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace EsepWebhook;

public class Function
{

    /// <summary>
    /// A function that interprets input from the Github Webhook and pulls out the issue.
    /// </summary>
    /// <param name="input"></param>
    /// <param name="context"></param>
    /// <returns>
    /// { issue: { html_url: “link to the issue created” }}
    /// </returns>
    public string FunctionHandler(object input, ILambdaContext context)
    {
        dynamic json = JsonConvert.DeserializeObject<dynamic>(input.ToString());

        string payload = $"{{'text':'Issue Created: {json.issue.html_url}'}}";

        var client = new HttpClient();
        var webRequest = new HttpRequestMessage(HttpMethod.Post, Environment.GetEnvironmentVariable("SLACK_URL"))
        {
            Content = new StringContent(payload, Encoding.UTF8, "application/json")
        };

        var response = client.Send(webRequest);
        using var reader = new StreamReader(response.Content.ReadAsStream());

        return reader.ReadToEnd();
    }
}
