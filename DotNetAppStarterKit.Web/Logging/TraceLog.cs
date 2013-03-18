// /*
// Copyright (c) 2013 Andrew Newton (http://www.nootn.com)
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// */

using System;
using System.Web;
using DotNetAppStarterKit.Core.Logging;

namespace DotNetAppStarterKit.Web.Logging
{
    public class TraceLog<T> : ILog<T>
    {
        protected readonly HttpContextBase Context;

        public TraceLog(HttpContextBase context)
        {
            Context = context;
        }

        public virtual void Debug(Func<string> message)
        {
            Context.Trace.Write("Debug", PrependTypeName(message()));
        }

        public virtual void Debug(string message, params object[] formatArgs)
        {
            Context.Trace.Write("Debug", string.Format(PrependTypeName(message), formatArgs));
        }

        public virtual void Error(Func<string> message)
        {
            Context.Trace.Warn("Error", PrependTypeName(message()));
        }

        public virtual void Error(string message, Exception exception)
        {
            Context.Trace.Warn("Error", message, exception);
        }

        public virtual void Error(string message, params object[] formatArgs)
        {
            Context.Trace.Warn("Error", string.Format(PrependTypeName(message), formatArgs));
        }

        public virtual void Information(Func<string> message)
        {
            Context.Trace.Write("Information", PrependTypeName(message()));
        }

        public virtual void Information(string message, params object[] formatArgs)
        {
            Context.Trace.Write("Information", string.Format(PrependTypeName(message), formatArgs));
        }

        public virtual void Warning(Func<string> message)
        {
            Context.Trace.Warn("Warning", PrependTypeName(message()));
        }

        public virtual void Warning(string message, params object[] formatArgs)
        {
            Context.Trace.Warn("Warning", string.Format(PrependTypeName(message), formatArgs));
        }

        protected string PrependTypeName(string message)
        {
            return string.Concat(typeof (T).FullName, ": ", message);
        }
    }
}