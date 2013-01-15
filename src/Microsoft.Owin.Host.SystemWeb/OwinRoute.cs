// <copyright file="OwinRoute.cs" company="Katana contributors">
//   Copyright 2011-2012 Katana contributors
// </copyright>
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using System.Web;
using System.Web.Routing;

namespace Microsoft.Owin.Host.SystemWeb
{
    internal sealed class OwinRoute : RouteBase
    {
        private readonly string _pathBase;
        private readonly Func<OwinAppContext> _appAccessor;

        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "By design")]
        internal OwinRoute(string pathBase, Func<OwinAppContext> appAccessor)
        {
            _pathBase = Utils.NormalizePath(HttpRuntime.AppDomainAppVirtualPath) + Utils.NormalizePath(pathBase);
            _appAccessor = appAccessor;
        }

        public override RouteData GetRouteData(HttpContextBase httpContext)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException("httpContext");
            }

            string requestPath = httpContext.Request.CurrentExecutionFilePath + httpContext.Request.PathInfo;

            var requestPathLength = requestPath.Length;
            var pathBaseLength = _pathBase.Length;
            if (requestPathLength < pathBaseLength)
            {
                return null;
            }
            if (requestPathLength > pathBaseLength && requestPath[pathBaseLength] != '/')
            {
                return null;
            }
            if (!requestPath.StartsWith(_pathBase, StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            return new RouteData(this, new OwinRouteHandler(_pathBase, requestPath.Substring(pathBaseLength), _appAccessor));
        }

        public override VirtualPathData GetVirtualPath(RequestContext requestContext, RouteValueDictionary values)
        {
            return null;
        }
    }
}
