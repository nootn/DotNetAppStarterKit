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
using System.Runtime.CompilerServices;

namespace DotNetAppStarterKit.Core.Logging
{
    /// <summary>
    ///     Write messages to the diagnostic log.
    /// </summary>
    /// <typeparam name="T">
    ///     The class consuming the log.
    ///     this parameter is used to customise loggers on a per-client basis
    ///     without the overhead of dynamically determining the client type for
    ///     every log.
    /// </typeparam>
    public interface ILog<T>
    {
        /// <summary>
        ///     Write a message for debugging purposes, describing internal program
        ///     events and actions not usually shown in production logs.
        /// </summary>
        /// <param name="message">The message to write, including format specifications.</param>
        /// <param name="formatArgs">Arguments to match the format parameters in the message, if any.</param>
        void Debug(string message, params object[] formatArgs);

        /// <summary>
        ///     Write a message for debugging purposes, describing internal program
        ///     events and actions not usually shown in production logs.
        /// </summary>
        /// <param name="message">
        ///     A function that calculates the message to write. The
        ///     cost of invoking this function is only incurred if the message is above the current
        ///     logging threshold.
        /// </param>
        void Debug(Func<string> message);

        /// <summary>
        ///     Write a message describing internal program events and actions, at a level suitable for
        ///     display in production logs.
        /// </summary>
        /// <param name="message">The message to write, including format specifications.</param>
        /// <param name="formatArgs">Arguments to match the format parameters in the message, if any.</param>
        void Information(string message, params object[] formatArgs);

        /// <summary>
        ///     Write a message describing internal program events and actions, at a level suitable for
        ///     display in production logs.
        /// </summary>
        /// <param name="message">
        ///     A function that calculates the message to write. The
        ///     cost of invoking this function is only incurred if the message is above the current
        ///     logging threshold.
        /// </param>
        void Information(Func<string> message);

        /// <summary>
        ///     Write a message describing internal program events and actions that may in some
        ///     circumstances indicate a problem with the application, its configuration or dependencies.
        /// </summary>
        /// <param name="message">The message to write, including format specifications.</param>
        /// <param name="formatArgs">Arguments to match the format parameters in the message, if any.</param>
        void Warning(string message, params object[] formatArgs);

        /// <summary>
        ///     Write a message describing internal program events and actions that may in some
        ///     circumstances indicate a problem with the application, its configuration or dependencies.
        /// </summary>
        /// <param name="message">
        ///     A function that calculates the message to write. The
        ///     cost of invoking this function is only incurred if the message is above the current
        ///     logging threshold.
        /// </param>
        void Warning(Func<string> message);

        /// <summary>
        ///     Write a message describing internal program events and actions that indicate a
        ///     problem with the application, its configuration or dependencies.
        /// </summary>
        /// <param name="message">The message to write, including format specifications.</param>
        /// <param name="formatArgs">Arguments to match the format parameters in the message, if any.</param>
        void Error(string message, params object[] formatArgs);

        /// <summary>
        ///     Write a message describing internal program events and actions that indicate a
        ///     problem with the application, its configuration or dependencies.
        /// </summary>
        /// <param name="message">
        ///     A function that calculates the message to write. The
        ///     cost of invoking this function is only incurred if the message is above the current
        ///     logging threshold.
        /// </param>
        void Error(Func<string> message);

        /// <summary>
        ///     Write a message describing internal program events and actions that indicate a
        ///     problem with the application, its configuration or dependencies.
        /// </summary>
        /// <param name="message">The message to write.</param>
        /// <param name="exception">An exception indicating the problem.</param>
        void Error(string message, Exception exception);
    }
}