// ***********************************************************************
// Copyright (c) 2014 NUnit Project
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// ***********************************************************************

using System.Reflection;

//
// Common Information for the NUnit assemblies
//
[assembly: AssemblyCompany("NUnit Software")]
[assembly: AssemblyProduct("NUnit 3.0")]
[assembly: AssemblyCopyright("Copyright (C) 2016 NUnit Project")]
[assembly: AssemblyTrademark("NUnit is a trademark of NUnit Software")]

#if PORTABLE
[assembly: AssemblyMetadata("PCL", "True")]
#endif

#if DEBUG
#if NETSTANDARD1_3
[assembly: AssemblyConfiguration("Net Standard 1.3 Debug")]
#elif PORTABLE
[assembly: AssemblyConfiguration("Portable Debug")]
#else
[assembly: AssemblyConfiguration("Debug")]
#endif
#else
#if NETSTANDARD1_3
[assembly: AssemblyConfiguration("Net Standard 1.3")]
#elif PORTABLE
[assembly: AssemblyConfiguration("Portable")]
#else
[assembly: AssemblyConfiguration("")]
#endif
#endif

[assembly: AssemblyVersion("3.3.0.0")]
[assembly: AssemblyFileVersion("3.3.0.0")]