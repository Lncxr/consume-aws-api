# Consume AWS API in Unity
Repository demonstrating the consumption of a third-party API (AWS SES) in Unity using IoC and behaviour-driven development.

## Testing

### Mocking Dependencies with NSubstitute

Within the project you'll find a subfolder called 'SESControllerTests', it's particularly important when working with a third-party API (in this case, SES via the AWS SDK) that we mock and test individual units of functionality so we can build a client wrapper that is robust and easy to maintain. This is generally good practice in Unity as your test assembly will require references to any other modules your client implementation depends on, giving you a clear picture of how tightly coupled your modules are.

In this project we make full use of NSubstitute to mock the 'AmazonSimpleEmailServiceClient' we use to interact with AWS cloud services (so we don't send any real POST requests - which we pay for!). By substituting the dependency via the relevant interface 'IAmazonSimpleEmailService' we can verify our client code's interaction with the dependency before making any real calls to AWS resources.