using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace EdgeOS.API
{
    /// <summary>A class that contains methods suitable as a <see cref="RemoteCertificateValidationCallback"/> delegate that pins self-signed certificates to a trusted stored certificate.</summary>
    class ServerCertificateValidationCallback
    {
        /// <summary>Used for TLS Certificate pinning by PinPublicKey.</summary>
        public static string DataDirectory;

        /// <summary>Disable automatic generated default constructor.</summary>
        private ServerCertificateValidationCallback() { }

        /// <summary>A method suitable as a <see cref="RemoteCertificateValidationCallback"/> delegate that pins self-signed certificates to a trusted stored certificate.</summary>
        /// <param name="sender">An object that contains state information for this validation.</param>
        /// <param name="certificate">The certificate used to authenticate the remote party.</param>
        /// <param name="chain">The chain of certificate authorities associated with the remote certificate.</param>
        /// <param name="sslPolicyErrors">One or more errors associated with the remote certificate.</param>
        /// <returns>A <see cref="Boolean"/> value that determines whether the specified certificate is accepted for authentication.</returns>
        /// <remarks>See also <see href="https://msdn.microsoft.com/en-us/library/system.net.security.remotecertificatevalidationcallback(v=vs.110).aspx"/></remarks>
        public static bool PinPublicKey(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            // Having no valid certificate provided by the connection is automatically invalid/incorrect behaviour.
            if (certificate == null) { return false; }

            // To ensure a valid (but compromised) server is not being impersonated we need to know what server we were making the request to.
            HttpWebRequest originalRequest = sender as HttpWebRequest;
            if (sender == null) { return false; }

            // If this file exists we will use this to validate the server instead of the usual rules.
            string expectedCertificatePath = DataDirectory + originalRequest.Address.Host.Replace("-", "").Replace(".", "") + ".crt";

            // Check to see if we are manually accepting a server certificate.
            if (File.Exists(expectedCertificatePath))
            {
                // If this is so far valid then we validate. If this chain failed to validate, is the reason this failed to match due to the unknown root and optionally hostname (other errors are fatal)?
                if (sslPolicyErrors == SslPolicyErrors.None || ((sslPolicyErrors &~ SslPolicyErrors.RemoteCertificateNameMismatch) == SslPolicyErrors.RemoteCertificateChainErrors && chain.ChainStatus.Length == 1 && chain.ChainStatus[0].Status == X509ChainStatusFlags.UntrustedRoot))
                {
                    // We fail a name mismatch if it is not the standard EdgeRouter name.
                    if ((sslPolicyErrors &= SslPolicyErrors.RemoteCertificateNameMismatch) == SslPolicyErrors.RemoteCertificateNameMismatch && !certificate.Subject.Equals("L=New York, S=New York, O=Ubiquiti Inc., CN=UbiquitiRouterUI, C=US")) { return false; }

                    // Obtain the certificate from the file.
                    using (X509Certificate2 storedCertificate = new X509Certificate2(expectedCertificatePath))
                    {
                        // Do the 2 public keys match?
                        return certificate.GetPublicKeyString().Equals(storedCertificate.GetPublicKeyString());
                    }
                }

                // A hardcoded certificate exists but this does not match or another error occurred (we also do not perform normal validation in this case in case we have been MITM'd by a compromised but trusted CA).
                return false;
            }

            // Should otherwise do standard validation (https://stackoverflow.com/questions/28679120/how-to-call-default-servercertificatevalidationcallback-inside-customized-valida).
            return sslPolicyErrors == SslPolicyErrors.None;
        }
    }
}