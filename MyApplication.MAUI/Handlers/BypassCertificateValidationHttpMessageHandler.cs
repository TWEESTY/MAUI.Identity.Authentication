namespace MyApplication.MAUI.Handlers;

public class BypassCertificateValidationHttpMessageHandler : HttpClientHandler
{
    public BypassCertificateValidationHttpMessageHandler() : base()
    {
        this.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) =>
        {
            if (cert != null && cert.Issuer.Equals("CN=localhost"))
                return true;
            return errors == System.Net.Security.SslPolicyErrors.None;
        };
    }
}

