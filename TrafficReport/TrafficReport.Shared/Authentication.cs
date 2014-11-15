using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Credentials;

namespace TrafficReport
{
    class Authentication
    {
        /// <summary>
        /// Authenticate the user
        /// </summary>
        /// <remarks>
        /// On Phone this method does not always return and instead the app may get reactivated via App.OnActivated.
        /// This method cannot be called in OnNavigatedTo() or during the Loaded event (fails with 
        /// 0x800706BE 'Remote Procedure Call Failed').
        /// </remarks>
        /// <returns>True if authentication succeeded</returns>
        internal static async Task<bool> AuthenticateAsync()
        {
            PasswordVault vault = new PasswordVault();

            PasswordCredential credential = null;
            
            try
            {
                credential = vault.FindAllByResource("MicrosoftAccount").FirstOrDefault();
            }
            catch (Exception)
            {
                // FindAllByResource throws when no credential was registered
            }

            if (credential != null)
            {
                // Create a user from the stored credentials.
                var user = new MobileServiceUser(credential.UserName);
                credential.RetrievePassword();
                user.MobileServiceAuthenticationToken = credential.Password;
                App.MobileServices.CurrentUser = user;

                // Test if the cached credential has expired.
                try
                {
                    await App.MobileServices.InvokeApiAsync("ping", HttpMethod.Get, null);
                    Debug.WriteLine("Signed in as {0} (reusing credentials)", user.UserId);
                    return true;
                }
                catch (MobileServiceInvalidOperationException ex)
                {
                    if (ex.Response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        // Remove the credential with the expired token.
                        vault.Remove(credential);
                        credential = null;
                    }
                }
            }

            try
            {
                // On Phone this call does not always return and instead the app may 
                // get reactivated (via App.OnActivated). The call cannot happen during OnNavigatedTo() or 
                // the Loaded event (fails with 0x800706BE 'Remote Procedure Call Failed').
                MobileServiceUser user = await App.MobileServices.LoginAsync(
                    MobileServiceAuthenticationProvider.MicrosoftAccount,
                    /*useSingleSignOn*/true
                    );
                Debug.WriteLine("Signed in as {0} (using new credentials)", user.UserId);

                // Create and store the user credentials.
                credential = new PasswordCredential(
                    "MicrosoftAccount",
                    user.UserId,
                    user.MobileServiceAuthenticationToken
                    );
                vault.Add(credential);

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Sign-in failed ({0})", ex.Message);
            }

            return false;
        }
    }
}
