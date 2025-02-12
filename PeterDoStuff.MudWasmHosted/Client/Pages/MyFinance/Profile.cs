using System.Text.Json.Serialization;

namespace PeterDoStuff.MudWasmHosted.Client.Pages.MyFinance
{
    public class Profile
    {
        public string Name { get; set; }
        public List<Block> Blocks { get; set; } = [];
    }

    [JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
    [JsonDerivedType(typeof(InputBlock), typeDiscriminator: nameof(InputBlock))]
    [JsonDerivedType(typeof(MultiplyBlock), typeDiscriminator: nameof(MultiplyBlock))]
    public abstract class Block
    {
        public abstract string Name { get; }
        public abstract decimal Value { get; }
        public abstract string Currency { get; }
    }

    public class InputBlock : Block
    {
        public string InputName { get; set; }
        public decimal InputValue { get; set; }
        public string InputCurrency { get; set; }

        public override string Name => InputName;

        public override decimal Value => InputValue;

        public override string Currency => InputCurrency;
    }

    public class MultiplyBlock : Block
    {
        public decimal Factor { get; set; }
        public Block Input { get; set; }

        public override string Name => $"x{Factor}";

        public override decimal Value => (Input?.Value ?? 0) * Factor;

        public override string Currency => Input?.Currency ?? "";
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
