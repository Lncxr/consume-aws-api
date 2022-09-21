using System;
using System.Net;
using Amazon.Runtime.Internal;
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using NUnit.Framework;
using NSubstitute;
using System.Threading.Tasks;
using UnityEngine;

public class SESControllerTests
{
    // Declare the system-under-test
    private ISESController m_sut;

    // Client that we'll use to communicate with AWS resources
    private IAmazonSimpleEmailService m_client;

    // Instance of an AWS services request we'll pass to our client
    private IAmazonWebServiceRequest m_request;

    // Here we initialise test data that we can re-use, this is useful as a failsafe in that if
    // any of our test data fails to initialise the rest of the test cases will fail!
    [OneTimeSetUp]
    public void SetUp()
    {
        // Set up mocked dependency
        m_client = Substitute.For<IAmazonSimpleEmailService>();

        // Inject mocked dependency via SUT's constructor signature
        m_sut = new SESController(m_client);

        m_request = Substitute.For<SendRawEmailRequest>();

        m_client.ClearReceivedCalls();
    }

    [Test]
    public async void Should_CatchExceptionWithExpectedMessage_When_ClientThrowsException()
    {
        // Given...
        m_client.SendRawEmailAsync(Arg.Any<SendRawEmailRequest>())
            .Returns(x => Task.FromException<SendRawEmailResponse>(new AmazonSimpleEmailServiceException("SES exception")));

        // When...
        try
        {
            await m_sut.SendRawEmailAsync(m_request);

            Assert.Fail("Did not catch any exceptions");
        }
        // Then...
        catch (Exception exception)
        {
            Debug.Log($"Exception message : {exception.Message}");
            Assert.IsInstanceOf(typeof(AmazonSimpleEmailServiceException), exception);
            Assert.That(exception.Message, Is.EqualTo("SES exception"));
            Assert.IsNotEmpty(m_client.ReceivedCalls());
        }
    }

    [Test]
    public async void Should_CatchExceptionWithExpectedMessage_When_ClientReceivesBadRequest()
    {
        // Given...
        m_request = new SendRawEmailRequest { RawMessage = null };

        m_client.SendRawEmailAsync(Arg.Is<SendRawEmailRequest>(x => x.RawMessage == null))
            .Returns(x => Task.FromException<SendRawEmailResponse>(new AmazonSimpleEmailServiceException("Invalid 'SendRawEmailRequest' supplied")));

        // When...
        try
        {
            await m_sut.SendRawEmailAsync(m_request);

            Assert.Fail("Did not catch bad request exception");
        }
        // Then...
        catch (Exception exception)
        {
            Debug.Log($"Exception message (bad request) : {exception.Message}");
            Assert.IsInstanceOf(typeof(AmazonSimpleEmailServiceException), exception);
            Assert.That(exception.Message, Is.EqualTo("Invalid 'SendRawEmailRequest' supplied"));
            Assert.IsNotEmpty(m_client.ReceivedCalls());
        }
    }

    [Test]
    public async void Should_ReturnFalse_When_ReceivingBadHttpStatusCode()
    {
        // Given...
        m_client.SendRawEmailAsync(Arg.Any<SendRawEmailRequest>()).Returns(new SendRawEmailResponse
        {
            HttpStatusCode = HttpStatusCode.BadRequest
        });

        // When...

        // Then...
        Assert.IsFalse(await m_sut.SendRawEmailAsync(m_request));
    }

    [Test]
    public async void Should_ReturnTrue_When_ReceivingOkHttpStatusCode()
    {
        // Given...
        m_client.SendRawEmailAsync(Arg.Any<SendRawEmailRequest>()).Returns(new SendRawEmailResponse
        {
            HttpStatusCode = HttpStatusCode.OK
        });

        // When...

        // Then...
        Assert.IsTrue(await m_sut.SendRawEmailAsync(m_request));
    }
}

