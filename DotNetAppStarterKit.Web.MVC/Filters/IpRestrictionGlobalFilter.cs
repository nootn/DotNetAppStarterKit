// /*
// Copyright (c) 2013 Andrew Newton (http://www.nootn.com.au)
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// */

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using DotNetAppStarterKit.Core.Caching;
using DotNetAppStarterKit.Core.Logging;
using DotNetAppStarterKit.Web.Mvc.Helpers;

namespace DotNetAppStarterKit.Web.Mvc.Filters
{
    public class IpRestrictionGlobalFilter : IActionFilter
    {
        public static Func<string[]> RestrictToIpAddresses =
            () =>
            string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["IpRestriction-IpAddressesCommaSeparated"])
                ? null
                : ConfigurationManager.AppSettings["IpRestriction-IpAddressesCommaSeparated"].Split(',');

        public static Func<string[]> RestrictToDnsAlias =
            () =>
            string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["IpRestriction-DnsAliasCommaSeparated"])
                ? null
                : ConfigurationManager.AppSettings["IpRestriction-DnsAliasCommaSeparated"].Split(',');

        public static Func<int> SuccessfulCacheTimeoutMinutes = () => 10;

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
        }

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var cacher = DependencyResolver.Current.GetService<ICacheProvider<SuccessfulIpRestrictedClient>>();
            var logger = DependencyResolver.Current.GetService<ILog<IpRestrictionGlobalFilter>>();

            var restrictToIpAddresses = RestrictToIpAddresses();
            var restrictToDnsAlias = RestrictToDnsAlias();

            if ((restrictToIpAddresses != null && restrictToIpAddresses.Any()) || (restrictToDnsAlias != null && restrictToDnsAlias.Any()))
            {
                var clientIp = IpHelper.GetClientIpAddress(filterContext.HttpContext.Request);

                //Try and cache it so it's only done once per person if successful (don't cache unsuccessful)
                if (cacher != null && cacher.GetCachedItem(clientIp) != null)
                {
                    //they are allowed in because they were before
                    return;
                }

                var foundInRestricted = false;

                if (restrictToIpAddresses != null && restrictToIpAddresses.Any())
                {
                    foundInRestricted = restrictToIpAddresses.Contains(clientIp);
                }

                var ipDnsMappings = new List<Tuple<string, string>>();

                if (!foundInRestricted)
                {
                    var counter = 0;
                    while (!foundInRestricted && counter < restrictToDnsAlias.Length)
                    {
                        var currHost = restrictToDnsAlias[counter];

                        var hostEntry = Dns.GetHostEntry(currHost);
                        if (hostEntry != null)
                        {
                            var ipOfDns = hostEntry.AddressList[0].ToString();
                            ipDnsMappings.Add(new Tuple<string, string>(currHost, ipOfDns));
                            if (string.Equals(clientIp, ipOfDns, StringComparison.InvariantCultureIgnoreCase))
                            {
                                foundInRestricted = true;
                            }
                        }

                        counter++;
                    }
                }

                if (!foundInRestricted)
                {
                    if (logger != null)
                    {
                        logger.Warning("IP {0} was not allowed in", clientIp);
                    }
                    filterContext.Result =
                        new HttpUnauthorizedResult(
                            "This resource has network restrictions in place, you are not authorized");
                }
                else
                {
                    if (cacher != null)
                    {
                        cacher.AddCachedItem(new SuccessfulIpRestrictedClient(), clientIp,
                                             SuccessfulCacheTimeoutMinutes());
                    }

                    if (logger != null)
                    {
                        logger.Debug("IP {0} was allowed in", clientIp);
                    }
                }
            }
        }
    }

    internal class SuccessfulIpRestrictedClient
    {
    }
}