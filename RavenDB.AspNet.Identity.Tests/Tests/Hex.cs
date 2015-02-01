using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RavenDB.AspNet.Identity.Common;
using Xunit;
using Xunit.Extensions;

// ReSharper disable once CheckNamespace
namespace RavenDB.AspNet.Identity.Tests
{
    public class Hex
    {
        [Fact]
        public void Test_Hex_Roundtrip()
        {
            byte[] randomBytes = new byte[4096];
            Random rand = new Random();
            rand.NextBytes(randomBytes);

            string hex = Helper.ToHex(randomBytes);
            Assert.Equal(hex.Length, 4096*2);

            byte[] roundtrip = Helper.FromHex(hex);

            Assert.Equal(roundtrip, randomBytes);
        }

        [Fact]
        public void Hex_case_doesnt_matter()
        {
            byte[] b1 = Helper.FromHex("0123456789ABCDEFabcdef0123456789");
            byte[] b2 = Helper.FromHex("0123456789abcdefABCDEF0123456789");

            Assert.Equal(b1, b2);
        }
    }
}
