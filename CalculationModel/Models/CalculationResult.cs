namespace CalculationModel.Models
{
    public class CalculationResult
    {
        public double H1 { get; set; }
        public double H2 { get; set; }
        public double Height { get; set; }
        public string HeightBlank { get; set; }
        public string HeightBlankWithTest { get; set; }
        public double OuterDiameterDetail { get; set; }
        public string OuterDiameterBlank { get; set; }
        public string OuterDiameterBlankWithTest { get; set; }
        public double InnerDiameterDetail { get; set; }
        public string InnerDiameterBlank { get; set; }
        public string InnerDiameterBlankWithTest { get; set; }
        public double ForgingBlankMassNominal { get; set; }
        public double ForgingBlankMassMax { get; set; }
        public double ForgingBlankWithTestMassNominal { get; set; }
        public double ForgingBlankWithTestMassMax { get; set; }
    }
}
