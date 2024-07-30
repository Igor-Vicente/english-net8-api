using English.Net8.Api.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace English.Net8.Api.Extensions
{
    public static class UrlHelperExtensions
    {
        public static string EmailConfirmationLink(this IUrlHelper urlHelper, string userId, string code, string scheme)
        {
            return urlHelper.Action(
                action: nameof(AccountController.ConfirmEmail),
                controller: "Account",
                values: new { userId, code },
                protocol: scheme);
        }

        public static string ResetPasswordCallbackLink(this IUrlHelper urlHelper, string userId, string code, string scheme)
        {
            return urlHelper.Action(
                action: nameof(AccountController.ResetPassword),
                controller: "Account",
                values: new { userId, code },
                protocol: scheme);
        }

        public static string ResetPasswordLink(this IUrlHelper urlHelper, string appClientUrl, string userId, string code)
        {
            var encodedUserId = WebUtility.UrlEncode(userId);
            var encodedCode = WebUtility.UrlEncode(code);
            return $"{appClientUrl}/account/reset-password?userId={encodedUserId}&code={encodedCode}";
        }
    }
}
