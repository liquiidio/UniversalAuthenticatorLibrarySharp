using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;


// based on https://github.com/EOSIO/ual-plainjs-renderer/blob/master/src/UALJs.ts
public class UnityUAL : MonoBehaviour
{
    public IChain[] Chains;
    public string AppName;
    public List<Authenticator> Authenticators;

    /**
     * @param chains          A list of chains the dapp supports.
     *
     * @param appName         The name of the app using the authenticators
     *
     * @param authenticators  A list of authenticator apps that the dapp supports.
     */
    public UnityUAL(IChain[] chains, string appName, List<Authenticator> authenticators)
    {
        Chains = chains;
        AppName = appName;
        Authenticators = authenticators;
    }

    public bool IsAutologin { get; private set; }
    public Authenticator ActiveAuthenticator { get; private set; }

    /**
     * Returns an object with a list of initialized Authenticators that returned true for shouldRender()
     * as well as an authenticator that supports autoLogin
     */
    public AuthenticatorResponse GetAuthenticators()
    {
        return new AuthenticatorResponse()
        {
            AvailableAuthenticators = Authenticators.Where(a => a.ShouldRender()).ToArray(),
            AutoLoginAuthenticator = Authenticators.FirstOrDefault(a => a.ShouldRender() && a.ShouldAutoLogin())
        };
    }

    /**
     * Initializes UAL: If a renderConfig was provided and no autologin authenticator
     * is returned it will render the Auth Button and relevant DOM elements.
     *
     */
    public void Init()
    {
        var authenticators = GetAuthenticators();

        // perform this check first, if we're autologging in we don't render a dom
        if (authenticators.AutoLoginAuthenticator != null)
        {
            IsAutologin = true;
            LoginUser(authenticators.AutoLoginAuthenticator);
            ActiveAuthenticator = authenticators.AutoLoginAuthenticator;
        }
        else
        {
            // check for existing session and resume if possible
            AttemptSessionLogin(authenticators.AvailableAuthenticators);

            // TODO @Evans @David, here we show the UI with the different Buttons somehow
            // we need to decide if we go with 2 different UnityUALs (Canvas and UiToolki)
            // or if we use some form of configuration
            // I think we should also allow for a little bit of styling
            // like horizontal vs. vertical alignment (or even auto, for mobile?!)

            foreach(var authenticator in authenticators.AvailableAuthenticators)
            {
                // Has Icon, Style, TextColor etc.
                var buttonStyle = authenticator.GetStyle();

                // called when the specific Button is pressed
                Action onClick = () => LoginUser(authenticator);
            }
        }
    }

    private void AttemptSessionLogin(Authenticator[] availableAuthenticators)
    {
        var sessionExpiration = PlayerPrefs.GetString("UALJs.SESSION_EXPIRATION_KEY");
        if (sessionExpiration != null)
        {
            // clear session if it has expired and continue
            if (DateTime.TryParse(sessionExpiration, out var expiration) && expiration <= DateTime.Now)
            {
                //localStorage.clear()
                PlayerPrefs.DeleteKey(""); // TODO
            }
            else
            {
                var authenticatorName = PlayerPrefs.GetString("UALJs.SESSION_AUTHENTICATOR_KEY");
                var sessionAuthenticator = Authenticators.FirstOrDefault(a => a.GetType().Name == authenticatorName);
                var accountName = PlayerPrefs.GetString("UALJs.SESSION_ACCOUNT_NAME_KEY");
                LoginUser(sessionAuthenticator, accountName);
            }
        }
    }

    private async void LoginUser(Authenticator authenticator, string accountName = null)
    {
        User[] users;

        // set the active authenticator so we can use it in logout
        ActiveAuthenticator = authenticator;

        var invalidateSeconds = ActiveAuthenticator.ShouldInvalidateAfter();
        var invalidateAt = DateTime.Now;
        invalidateAt.AddSeconds(invalidateSeconds);

        PlayerPrefs.SetString("UALJs.SESSION_EXPIRATION_KEY", invalidateAt.ToString());
        PlayerPrefs.SetString("UALJs.SESSION_AUTHENTICATOR_KEY", authenticator.GetType().Name);

        try
        {
            if (!string.IsNullOrEmpty(accountName))
            {
                users = await authenticator.Login(accountName);

                PlayerPrefs.SetString("UALJs.SESSION_ACCOUNT_NAME_KEY", accountName);
            }
            else
            {
                users = await authenticator.Login();
            }

            // send our users back, this should be done different, within the Authenticator,
            // likely in Update() to allow asynchronity of events
            UserCallbackHandler(users);

        }
        catch (Exception e)
        {
            Debug.LogError(e);
            //this.clearStorageKeys()
            throw e;
        }

        // reset our modal state if we're not autologged in (no dom is rendered for autologin)
        if (!this.IsAutologin)
        {
            //this.dom!.reset()
        }
    }

    // TODO, this should be done different
    private void UserCallbackHandler(User[] users)
    {
        throw new NotImplementedException();
    }
}
