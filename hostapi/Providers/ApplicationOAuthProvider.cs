using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using hostapi.Models;
using hostapi.Classes.Configuracion;

namespace hostapi.Providers
{
    public class ApplicationOAuthProvider : OAuthAuthorizationServerProvider
    {
        private int minutes = 10;

        private readonly string _publicClientId;

        public ApplicationOAuthProvider(string publicClientId)
        {
            if (publicClientId == null)
            {
                throw new ArgumentNullException("publicClientId");
            }

            _publicClientId = publicClientId;
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            var userManager = context.OwinContext.GetUserManager<ApplicationUserManager>();
            //ApplicationUser user = await userManager.FindAsync(context.UserName, context.Password);
            ApplicationUser user = await userManager.FindByEmailAsync(context.UserName);
            
            if (user != null)
            {
                DateTime today = DateTime.Now;
                if (user.LockoutEnabled && user.LockoutEndDateUtc != null && user.LockoutEndDateUtc.Value > today)
                {
                    if (userManager.GetAccessFailedCount(user.Id) == 0)
                    {
                        context.SetError("user_lockedout_by_admin", "User is lockedout.");
                    }
                    else
                    {
                        context.SetError("user_lockedout", "User is lockedout.");
                    }
                    return;
                }
                else if (userManager.CheckPassword(user, context.Password))
                {
                    if (user.PhoneNumberConfirmed == true)
                    {
                        Authentication model = new Authentication();
                        var error = await model.LoadRights(user.Id);
                        model = null;
                        if (error != null)
                        {
                            context.SetError("user_rigths", "Error generando derechos \n" + error.message);
                            return;
                        }

                    }

                    if (userManager.GetAccessFailedCount(user.Id) > 0)
                    {
                        userManager.ResetAccessFailedCount(user.Id);
                    }

                    if (userManager.SupportsUserLockout && user.LockoutEnabled)
                    {
                        user.LockoutEnabled = false;
                        await userManager.UpdateAsync(user);
                    }
                    ClaimsIdentity oAuthIdentity = await user.GenerateUserIdentityAsync(userManager,
                       OAuthDefaults.AuthenticationType);
                    ClaimsIdentity cookiesIdentity = await user.GenerateUserIdentityAsync(userManager,
                        CookieAuthenticationDefaults.AuthenticationType);

                    AuthenticationProperties properties = CreateProperties(user.UserName);
                    AuthenticationTicket ticket = new AuthenticationTicket(oAuthIdentity, properties);
                    context.Validated(ticket);
                    context.Request.Context.Authentication.SignIn(cookiesIdentity);
                    return;
                }
                else if (userManager.SupportsUserLockout)
                {
                    if (user.LockoutEnabled && user.LockoutEndDateUtc.Value <= today)
                    {
                        userManager.ResetAccessFailedCount(user.Id);
                        user.LockoutEnabled = false;
                        await userManager.UpdateAsync(user);
                    }

                    await userManager.AccessFailedAsync(user.Id);
                    if (userManager.GetAccessFailedCount(user.Id) == 3)
                    {
                        user.LockoutEndDateUtc = DateTime.Now.AddMinutes(minutes);
                        user.LockoutEnabled = true;
                        await userManager.UpdateAsync(user);
                    }

                } 
                
                
            }
            context.SetError("invalid_grant", "The user name or password is incorrect.");
            return;

        }

        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
            {
                context.AdditionalResponseParameters.Add(property.Key, property.Value);
            }

            return Task.FromResult<object>(null);
        }

        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            // Resource owner password credentials does not provide a client ID.
            if (context.ClientId == null)
            {
                context.Validated();
            }

            return Task.FromResult<object>(null);
        }

        public override Task ValidateClientRedirectUri(OAuthValidateClientRedirectUriContext context)
        {
            if (context.ClientId == _publicClientId)
            {
                Uri expectedRootUri = new Uri(context.Request.Uri, "/");

                if (expectedRootUri.AbsoluteUri == context.RedirectUri)
                {
                    context.Validated();
                }
            }

            return Task.FromResult<object>(null);
        }

        public static AuthenticationProperties CreateProperties(string userName)
        {
            IDictionary<string, string> data = new Dictionary<string, string>
            {
                { "userName", userName }
            };
            return new AuthenticationProperties(data);
        }
    }
}