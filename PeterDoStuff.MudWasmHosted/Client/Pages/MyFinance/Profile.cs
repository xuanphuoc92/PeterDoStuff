using System.Security.Cryptography;
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

        public override string Name => InputName;

        public override decimal Value => (InputBlock?.Value ?? 0) * InputFactor;

        public override string Currency => InputBlock?.Currency ?? "";
    }

    public class DivideBlock : Block
    {
        public string InputName { get; set; }
        public decimal InputFactor { get; set; } = 1;
        public Block InputBlock { get; set; }

        public override string Name => InputName;

        public override decimal Value => (InputBlock?.Value ?? 0) / InputFactor;

        public override string Currency => InputBlock?.Currency ?? "";
    }

    public class MinusBlock : Block
    {
        public string InputName { get; set; }
        public Block InputBlock { get; set; }
        public List<Block> InputMinusBlocks { get; set; } = [];

        public override string Name => InputName;

        public override decimal Value => (InputBlock?.Value ?? 0) - InputMinusBlocks.Sum(b => b.Value);

        public override string Currency => InputBlock?.Currency ?? "";
    }

    public class TaxBlock : Block
    {
        public string InputName { get; set; }
        public TaxProfile InputTaxProfile { get; set; }
        public Block InputBlock { get; set; }

        public override string Name => InputName;

        public override decimal Value
        {
            get
            {
                if (InputTaxProfile == null || InputBlock == null)
                    return 0;

                decimal tax = 0;
                decimal income = InputBlock.Value;
                var levels = InputTaxProfile.Levels;

                for (int i = 0; i < levels.Count; i++)
                {
                    if (income < levels[i].From)
                        break;

                    decimal taxableAmount;
                    if (i == levels.Count -1)
                    {
                        taxableAmount = income - levels[i].From;
                    }
                    else
                    {
                        taxableAmount = Math.Min(income, levels[i + 1].From)
                            - levels[i].From;
                    }

                    tax += taxableAmount * (levels[i].RatePercentage / 100);
                }

                return tax;
            }
        }

        public override string Currency => InputBlock?.Currency ?? "";
    }

    public class TaxProfile
    {
        public string Name { get; set; }
        public List<TaxLevel> Levels { get; set; } = [];

        public static TaxProfile Singapore()
        {
            return new()
            {
                Name = "Singapore Tax",
                Levels = [
                    new() { From = 0, RatePercentage = 0 },
                    new() { From = 20000, RatePercentage = 2 },
                    new() { From = 30000, RatePercentage = 3.5M },
                    new() { From = 40000, RatePercentage = 7 },
                    new() { From = 80000, RatePercentage = 11.5M },
                    new() { From = 120000, RatePercentage = 15 },
                    new() { From = 160000, RatePercentage = 18 },
                ]
            };
        }
    }

    public class TaxLevel
    {
        public decimal From { get; set; }
        public decimal RatePercentage { get; set; }
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
            var annualTax = new TaxBlock()
            {
                InputName = "Annual Tax",
                InputBlock = annualIncome,
                InputTaxProfile = TaxProfile.Singapore(),
            };
            var monthlyTax = new DivideBlock()
            {
                InputName = "Monthly Tax",
                InputBlock = annualTax,
                InputFactor = 12,
            };
            var monthlyLivingExpense = new InputBlock()
            {
                InputName = "Monthly Living Expense",
                InputCurrency = "SGD",
                InputValue = 1800,
            };
            var monthlySaving = new MinusBlock()
            {
                InputName = "Monthly Saving",
                InputBlock = monthlyIncome,
                InputMinusBlocks = [monthlyTax, monthlyLivingExpense]
            };
            var yearlySaving = new MultiplyBlock()
            {
                InputName = "Yearly Saving",
                InputBlock = monthlySaving,
                InputFactor = 12,
            };

            profile.Blocks = [
                monthlyIncome, 
                annualIncome, 
                annualTax, 
                monthlyTax, 
                monthlyLivingExpense, 
                monthlySaving,
                yearlySaving,
            ];

            return profile;
        }
    }
}
