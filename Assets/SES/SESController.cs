using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using UnityEngine;
using Amazon;
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using MimeKit;
using Amazon.Runtime.Internal;

public class SESController : MonoBehaviour, ISESController
{
    // Private members
    private IAmazonSimpleEmailService m_client;

    // Hardcode your target (valid!) email address here for ease of reference,
    // in production I'd strongly recommend serialization + retrieval at runtime
    private static string m_target = "";

    // These MUST be stored in a separate config file in any production build(s)
    internal const string m_access = "yourAccessKeyHere";
    internal const string m_secret = "yourSecretKeyHere";

    // The AWS region where your services are hosted (default us-east-1)
    public static RegionEndpoint Region = RegionEndpoint.USEast1;

    // Default constructor (use in mono context -> initialise the client at runtime)
    public SESController() { }

    // Constructor taking IAmazonSimpleEmailService instance in its signature (IoC)
    public SESController(IAmazonSimpleEmailService client) => m_client = client;

    // Private MimeKit methods
    private static BodyBuilder GetMessageBody()
    {
        var body = new BodyBuilder()
        {
            HtmlBody = @"<p>Amazon SES Body</p>",
            TextBody = "Congratulations, you've successfully sent yourself an email!\n",
        };

        body.Attachments.Add(Application.streamingAssetsPath + "/texture.png");

        return body;
    }

    private static MimeMessage GetMessage()
    {
        var message = new MimeMessage();

        message.From.Add(new MailboxAddress("AWS SES Client", "user@domain.com"));
        message.To.Add(new MailboxAddress(string.Empty, m_target));

        message.Subject = "Amazon SES Test";

        message.Body = GetMessageBody()
            .ToMessageBody();

        return message;
    }

    private static MemoryStream GetMessageStream()
    {
        var stream = new MemoryStream();

        GetMessage().WriteTo(stream);

        return stream;
    }

    public async Task<bool> SendRawEmailAsync(IAmazonWebServiceRequest request)
    {
        using (m_client)
            try
            {
                request = new SendRawEmailRequest
                {
                     RawMessage = new RawMessage(GetMessageStream())
                };

                var response = await m_client.SendRawEmailAsync((SendRawEmailRequest)request);

                if (response.HttpStatusCode == HttpStatusCode.OK)
                {
                    Debug.Log("Response OK - check email inbox!");
                    return true;
                }
            }
            catch (Exception exception)
            {
                throw exception;
            }

        return false;
    }

    // MonoBehaviour implementation
    void Awake() => m_client = new AmazonSimpleEmailServiceClient(m_access, m_secret, Region);

    // Fire-and-forget (un-comment this once you've assigned your IAM credentials + target email) 
    // void Start() => _ = SendRawEmailAsync();
}
