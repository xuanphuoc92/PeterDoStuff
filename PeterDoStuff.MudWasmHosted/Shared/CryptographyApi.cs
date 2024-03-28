using System.Dynamic;

namespace PeterDoStuff.MudWasmHosted.Shared
{
    public interface CryptographyApi
    {
        public record Argon2idBody(byte[] Input, byte[] Salt);
        public Task<byte[]> HashArgon2idQuick(Argon2idBody body);
    }
}
