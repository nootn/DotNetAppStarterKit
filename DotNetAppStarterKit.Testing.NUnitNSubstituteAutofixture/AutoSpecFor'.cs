// /*
// Copyright (c) 2013 Andrew Newton (http://www.nootn.com.au)
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// */

using Ploeh.AutoFixture;

namespace DotNetAppStarterKit.Testing.NUnitNSubstituteAutofixture
{
    /// <summary>
    /// Automatic specification for allows you to define tests where the "Given" and "Wnen" is run, then multiple "Then" methods can be called individually to perform individual asserts.  Autofixture is used to set up the mock dependencies.
    /// </summary>
    /// <typeparam name="T">The subject under test</typeparam>
    /// <remarks>
    ///     Code thanks to gertjvr, originally from:
    ///     https://github.com/gertjvr/StringCalculatorKata/tree/master/src/StringCalculator.SpecFor.NUnit.UnitTests
    /// </remarks>
    public abstract class AutoSpecFor<T> : SpecFor<T>
    {
        protected IFixture Fixture;

        protected AutoSpecFor()
            : this(new Fixture())
        {
        }

        protected AutoSpecFor(IFixture fixture)
        {
            Fixture = fixture;
        }
    }
}