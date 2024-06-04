using NUnit.Framework;
using System.Security.Cryptography;
using UserService.Controllers;

namespace TestProject;

[TestFixture]
public class RSATests
{
    [Test]
    public void TestDecryptMethod()
    {
        string path = "C:\\Users\\Mad's Residence\\Desktop\\FinalExaminationServerApplication\\FinalExaminationServerApplication\\UserService\\rsa\\private_key.pem";
        RSA plaintext = RSATools.GetPrivateKey(path);

        Assert.NotNull(plaintext);
    }

}