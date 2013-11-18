// /*
// Copyright (c) 2013 Andrew Newton (http://www.nootn.com.au)
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// */

using NUnit.Framework;

namespace DotNetAppStarterKit.Testing.NUnitNSubstituteAutofixture
{
    /// <summary>
    /// Specification for allows you to define tests where the "Given" and "Wnen" is run, then multiple "Then" methods can be called individually to perform individual asserts
    /// </summary>
    /// <typeparam name="T">The subject under test</typeparam>
    /// <remarks>
    ///     Code thanks to gertjvr, originally from:
    ///     https://github.com/gertjvr/StringCalculatorKata/tree/master/src/StringCalculator.SpecFor.NUnit.UnitTests
    /// </remarks>
    [TestFixture]
    public abstract class SpecFor<T>
    {
        protected T Subject;

        protected abstract T Given();

        protected abstract void When();

        [TestFixtureSetUp]
        public void SetUp()
        {
            Subject = Given();
            When();
        }
    }
}