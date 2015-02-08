using System;
using Blun.AspNet.Identity.RavenDB.Common;
using NUnit.Framework;

// ReSharper disable once CheckNamespace
namespace Blun.AspNet.Identity.RavenDB.Tests
{
    [TestFixture]
    public class Hex
    {
        [Test]
        public void Test_Hex_Roundtrip()
        {
            byte[] randomBytes = new byte[4096];
            Random rand = new Random();
            rand.NextBytes(randomBytes);

            string hex = Helper.ToHex(randomBytes);
            Assert.AreEqual(hex.Length, 4096 * 2);

            byte[] roundtrip = Helper.FromHex(hex);

            Assert.AreEqual(roundtrip, randomBytes);
        }

        [Test]
        public void Hex_case_doesnt_matter()
        {
            byte[] b1 = Helper.FromHex("0123456789ABCDEFabcdef0123456789");
            byte[] b2 = Helper.FromHex("0123456789abcdefABCDEF0123456789");

            Assert.AreEqual(b1, b2);
        }
    }
}
