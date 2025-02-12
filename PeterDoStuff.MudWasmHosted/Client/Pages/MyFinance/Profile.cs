using System.Text.Json.Serialization;

namespace PeterDoStuff.MudWasmHosted.Client.Pages.MyFinance
{
    public class Profile
    {
        public string Name { get; set; }
        public List<Block> Blocks { get; set; } = [];
    }

    [JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
    [JsonDerivedType(typeof(UsdBlock), typeDiscriminator: nameof(UsdBlock))]
    [JsonDerivedType(typeof(VndBlock), typeDiscriminator: nameof(VndBlock))]
    public abstract class Block
    {
        public abstract string Name { get; set; }
        public abstract decimal Value { get; set; }
        public abstract string Currency { get; set; }
    }

    public class UsdBlock : Block
    {
        public override string Name { get; set; }
        public override decimal Value { get; set; }
        public override string Currency { get; set; } = "USD";
    }

    public class VndBlock : Block
    {
        public override string Name { get; set; }
        public override decimal Value { get; set; }
        public override string Currency { get; set; } = "VND";
    }

    public class Controller
    {
        public static Profile SampleProfile()
            => new()
            {
                Name = "John Doe",
            };
    }
}
