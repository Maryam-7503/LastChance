using OtpNet;

namespace WebApplication1.Services
{
    public class TwoFactorService
    {
        public string GenerateSecret()
        {
            var key = KeyGeneration.GenerateRandomKey(20);
            return Base32Encoding.ToString(key);
        }

        public string GenerateQrCodeUrl(string email, string secret)
        {
            return $"otpauth://totp/WebApplication1:{email}?secret={secret}&issuer=WebApplication1";
        }

        public bool VerifyCode(string secret, string code)
        {
            var bytes = Base32Encoding.ToBytes(secret);
            var totp = new Totp(bytes);
            return totp.VerifyTotp(code, out _, new VerificationWindow(2, 2));
        }
    }
}