/******************************************************************************
    CoAPWorks CoAP API Client
    
    MIT License

    Copyright (c) [2023] [Idaax Inc., www.coapsharp.com]

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in all
    copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    SOFTWARE.
 *****************************************************************************/

namespace Idaax.CoAPWorks.CoAPAPI.Client
{
    /// <summary>
    /// Holds information about local time of the machine (for longitude and latitude values)
    /// </summary>
    public class LocalTimeInfo
    {
        /// <summary>
        /// Whether daylight saving time is active or not
        /// </summary>
        public bool IsInDST { get; set; }
        /// <summary>
        /// The timezone (e.g. Asia/Kolkata)
        /// </summary>
        public string TimeZone { get; set; }
        /// <summary>
        /// The local date time
        /// </summary>
        public DateTime LocaDateTime { get; set; }
    }
}
