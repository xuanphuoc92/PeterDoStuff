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
        public string InputName { get; set; }
        public decimal InputFactor { get; set; }
        public Block InputBlock { get; set; }

        public override string Name => $"{InputName} (x{InputFactor})";

        public override decimal Value => (InputBlock?.Value ?? 0) * InputFactor;

        public override string Currency => InputBlock?.Currency ?? "";
    }

    public class Controller
    {
        public static Profile SampleProfile()
        {
            var profile = new Profile();
            profile.Name = "John Doe";
            
            var monthlyIncome = new InputBlock()
            {
                InputName = "Monthly Income",
                InputCurrency = "SGD",
                InputValue = 8000,
            };
            var annualIncome = new MultiplyBlock()
            {
                InputName = "Annual Income",
                InputFactor = 12,
                InputBlock = monthlyIncome,
            };

            profile.Blocks = [monthlyIncome, annualIncome];

            return profile;
        }
    }
}
