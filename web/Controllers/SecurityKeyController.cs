﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using IdentityModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;
using web.Models;

namespace web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SecurityKeyController : ControllerBase
    {
        private const string RelyingPartyId = "localhost";
        private readonly ITempDataProvider tempData;

        public SecurityKeyController(ITempDataProvider tempData)
        {
            this.tempData = tempData;            
        }

        [HttpPost]
        [Route("RegisterCallback")]
        public IActionResult RegisterCallback([FromBody] CredentialsModel model)
        {
            // 1. If the allowCredentials option was given when this authentication ceremony was initiated, verify that credential.id identifies one of the public key credentials that were listed in allowCredentials.
            var data = tempData.LoadTempData(HttpContext);
            if ((string) data["keyId"] != model.RawId) throw new Exception("Incorrect key used");

            // 2. If credential.response.userHandle is present, verify that the user identified by this value is the owner of the public key credential identified by credential.id.
            // 3. Using credential’s id attribute (or the corresponding rawId, if base64url encoding is inappropriate for your use case), look up the corresponding credential public key.
            //var user = Users.First(x => x.CredentialId == model.RawId);
            Gobie74.AspNetCore.Identity.AzureTable.User user = new Gobie74.AspNetCore.Identity.AzureTable.User();


            if (!string.IsNullOrEmpty(model.Response.UserHandle) && model.Response.UserHandle != user.UserName) throw new Exception("Incorrect user handle returned");

            // 4. Let cData, aData and sig denote the value of credential’s response's clientDataJSON, authenticatorData, and signature respectively.
            var cData = model.Response.ClientDataJson;
            var aData = model.Response.AuthenticatorData;
            var sig = model.Response.Signature;

            // 5. Let JSONtext be the result of running UTF-8 decode on the value of cData.
            var jsonText = Encoding.UTF8.GetString(Base64Url.Decode(cData));

            // 6. Let C, the client data claimed as used for the signature, be the result of running an implementation-specific JSON parser on JSONtext.
            var c = JsonConvert.DeserializeObject<ClientData>(jsonText);

            // 7. Verify that the value of C.type is the string webauthn.get.
            if (c.Type != "webauthn.get") throw new Exception("Incorrect client data type");

            // 8. Verify that the value of C.challenge matches the challenge that was sent to the authenticator in the PublicKeyCredentialRequestOptions passed to the get() call.
            var challenge = (string)data["challenge"];
            if (Base64Url.Decode(c.Challenge) == Convert.FromBase64String(challenge)) throw new Exception("Incorrect challenge");

            // 9. Verify that the value of C.origin matches the Relying Party's origin.
            if (c.Origin != "http://localhost:5000") throw new Exception("Incorrect origin");

            // 10. Verify that the value of C.tokenBinding.status matches the state of Token Binding for the TLS connection over which the attestation was obtained.
            // If Token Binding was used on that TLS connection, also verify that C.tokenBinding.id matches the base64url encoding of the Token Binding ID for the connection.
            // TODO: Token binding once out of draft

            // 11. Verify that the rpIdHash in aData is the SHA-256 hash of the RP ID expected by the Relying Party.
            var hasher = new SHA256Managed();

            var span = Base64Url.Decode(aData).AsSpan();
            // RP ID Hash (32 bytes)
            var rpIdHash = span.Slice(0, 32); span = span.Slice(32);

            // Flags (1 byte)
            var flagsBuf = span.Slice(0, 1).ToArray();
            var flags = new BitArray(flagsBuf); span = span.Slice(1);
            var userPresent = flags[0]; // (UP)
            // Bit 1 reserved for future use (RFU1)
            var userVerified = flags[2]; // (UV)
            // Bits 3-5 reserved for future use (RFU2)
            var attestedCredentialData = flags[6]; // (AT) "Indicates whether the authenticator added attested credential data"
            var extensionDataIncluded = flags[7]; // (ED)

            // Signature counter (4 bytes, big-endian unint32)
            var counterBuf = span.Slice(0, 4); span = span.Slice(4);
            var counter = BitConverter.ToUInt32(counterBuf); // https://www.w3.org/TR/webauthn/#signature-counter

            var computedRpIdHash = hasher.ComputeHash(Encoding.UTF8.GetBytes(RelyingPartyId));
            if (!rpIdHash.SequenceEqual(computedRpIdHash)) throw new Exception("Incorrect RP ID");

            // 12. If user verification is required for this assertion, verify that the User Verified bit of the flags in aData is set.
            // TODO: Handle user verificaton required

            // 13. If user verification is not required for this assertion, verify that the User Present bit of the flags in aData is set.
            if (userPresent == false) throw new Exception("User not present");

            //14. Verify that the values of the client extension outputs in clientExtensionResults
            // TODO: Handle extension results

            // 15. Let hash be the result of computing a hash over the cData using SHA-256.
            var hash = hasher.ComputeHash(Base64Url.Decode(cData)); // 32 bytes

            // 16. Using the credential public key looked up in step 3, verify that sig is a valid signature over the binary concatenation of aData and hash.
            var a = Base64Url.Decode(model.Response.AuthenticatorData);
            var sigBase = new byte[a.Length + hash.Length];
            a.CopyTo(sigBase, 0);
            hash.CopyTo(sigBase, a.Length);

            var ecDsa = ECDsa.Create(new ECParameters
            {
                Curve = ECCurve.NamedCurves.nistP256,
                Q = new ECPoint
                {
                    //X = Base64Url.Decode(user.PublicKey.X),
                    //Y = Base64Url.Decode(user.PublicKey.Y)
                }
            });
            
            var isValid = ecDsa.VerifyData(sigBase, DeserializeSignature(sig), HashAlgorithmName.SHA256);

            if (isValid)
            {
                // 17. the signature counter value adata.signCount is nonzero or the value stored in conjunction with credential’s id attribute is nonzero
                //if (user.Counter < counter)
                //{
                //    user.Counter = Convert.ToInt32(counter);
                //    HttpContext.SignInAsync("cookie",
                //        new ClaimsPrincipal(new ClaimsIdentity(new List<Claim> {new Claim("name", user.UserName)}, "cookie")));

                //    var returnUrl = (string) data["returnUrl"];
                //    if (returnUrl[0] == '/') return Redirect(returnUrl);
                //    return RedirectToAction("Index", "Home");
                //}

                throw new Exception("Possible cloned authenticator");
            }

            throw new Exception("Invalid Signature");
        }

        private byte[] DeserializeSignature(string signature)
        {
            // Thanks to: https://crypto.stackexchange.com/questions/1795/how-can-i-convert-a-der-ecdsa-signature-to-asn-1
            var s = Base64Url.Decode(signature);

            using (var ms = new MemoryStream(s))
            {
                var header = ms.ReadByte();
                var b1 = ms.ReadByte();

                var markerR = ms.ReadByte();
                var b2 = ms.ReadByte();
                var vr = new byte[b2];
                ms.Read(vr, 0, vr.Length);
                vr = RemoveAnyNegativeFlag(vr);

                var markerS = ms.ReadByte();
                var b3 = ms.ReadByte();
                var vs = new byte[b3];
                ms.Read(vs, 0, vs.Length);
                vs = RemoveAnyNegativeFlag(vs);

                var parsedSignature = new byte[vr.Length + vs.Length];
                vr.CopyTo(parsedSignature, 0);
                vs.CopyTo(parsedSignature, vr.Length);

                return parsedSignature;
            }
        }

        private byte[] RemoveAnyNegativeFlag(byte[] input)
        {
            if (input[0] != 0) return input;

            var output = new byte[input.Length - 1];
            Array.Copy(input, 1, output, 0, output.Length);
            return output;
        }
    }
}