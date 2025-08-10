using System.ComponentModel.DataAnnotations;
using CalculationModel.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CalculationModel.Models;


namespace CalculationModel.Pages
{
    public class QuestionnaireModel : PageModel
    {
        private const int LoggingPrecision = 3;
        private const double SpecificWeightOfSteel = 7.85 / 1000000000;

        private readonly AllowanceService _allowanceService;

        [BindProperty]
        [Required(ErrorMessage = "Поле обязательно")]
        [Range(100, 5000, ErrorMessage = "Диаметр должен быть от 100 до 5000 мм")]
        public double OuterDiameter { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Поле обязательно")]
        [Range(50, 4950, ErrorMessage = "Диаметр должен быть от 50 до 4950 мм")]
        public double InnerDiameter { get; set; } = 540;

        [BindProperty]
        [Required(ErrorMessage = "Поле обязательно")]
        [Range(10, 2000, ErrorMessage = "Высота должна быть от 10 до 2000 мм")]
        public double Height { get; set; } = 215;

        [BindProperty]
        [Required(ErrorMessage = "Поле обязательно")]
        [Range(1, 10, ErrorMessage = "Количество деталей от 1 до 10")]
        public int PartsCount { get; set; } = 2;

        [BindProperty]
        [Required(ErrorMessage = "Поле обязательно")]
        public double CutLength { get; set; } = 20;

        [BindProperty]
        [Required(ErrorMessage = "Поле обязательно")]
        public double TestAllowance { get; set; } = 60;

        [BindProperty]
        public double HeatTreatmentAllowance { get; set; } = 0;

        public QuestionnaireModel(AllowanceService allowanceService)
        {
            _allowanceService = allowanceService;
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            return RedirectToPage("ResultsPage", Calculate());
        }

        private CalculationResult Calculate()
        {
            (double h1, double h2) = CalculateH1H2();

            var (heightBlank, heightBlankNominal, heightBlankMax) = CalculateHeight(h1);
            var (heightBlankWithTest, heightBlankWithTestNominal, heightBlankWithTestMax) = CalculateHeight(h2);

            double outerDiameterDetail = OuterDiameter + HeatTreatmentAllowance;

            var (outerDiameterBlank, outerDiameterBlankNominal, outerDiameterBlankMax) = CalculateOuterDiameter(heightBlankNominal, outerDiameterDetail);
            var (outerDiameterBlankWithTest, outerDiameterBlankWithTestNominal, outerDiameterBlankWithTestMax) = CalculateOuterDiameter(heightBlankWithTestNominal, outerDiameterDetail);

            double innerDiameterDetail = InnerDiameter - HeatTreatmentAllowance;

            var (innerDiameterBlank, innerDiameterBlankNominal, innerDiameterBlankMax) = CalculateInnerDiameter(heightBlankNominal, innerDiameterDetail);
            var (innerDiameterBlankWithTest, innerDiameterBlankWithTestNominal, innerDiameterBlankWithTestMax) = CalculateInnerDiameter(heightBlankWithTestNominal, innerDiameterDetail);

            var (vBlankNominal, vBlankMax) = 
                CalculateVolumes(outerDiameterBlankNominal, innerDiameterBlankNominal, heightBlankNominal,outerDiameterBlankMax, innerDiameterBlankMax, heightBlankMax);

            var (vBlankWithTestNominal, vBlankWithTestMax) = 
                CalculateVolumes(outerDiameterBlankWithTestNominal, innerDiameterBlankWithTestNominal, heightBlankWithTestNominal, outerDiameterBlankWithTestMax, innerDiameterBlankWithTestMax, heightBlankWithTestMax);


            var (massBlankNominal, massBlankMax) = CalculateMasses(vBlankNominal, vBlankMax);
            var (massBlankWithTestNominal, massBlankWithTestMax) = CalculateMasses(vBlankWithTestNominal, vBlankWithTestMax);

            return new CalculationResult
            {
                H1 = h1,
                H2 = h2,
                Height = this.Height,
                HeightBlank = heightBlank,
                HeightBlankWithTest = heightBlankWithTest,
                OuterDiameterDetail = outerDiameterDetail,
                OuterDiameterBlank = outerDiameterBlank,
                OuterDiameterBlankWithTest = outerDiameterBlankWithTest,
                InnerDiameterDetail = innerDiameterDetail,
                InnerDiameterBlank = innerDiameterBlank,
                InnerDiameterBlankWithTest = innerDiameterBlankWithTest,
                ForgingBlankMassNominal = massBlankNominal,
                ForgingBlankMassMax = massBlankMax,
                ForgingBlankWithTestMassNominal = massBlankWithTestNominal,
                ForgingBlankWithTestMassMax = massBlankWithTestMax
            };
        }

        private (double  h1, double h2) CalculateH1H2() {
            double h1 = Height * PartsCount + (CutLength * (PartsCount - 1)) + HeatTreatmentAllowance;
            double h2 = Height * PartsCount + (CutLength * (PartsCount - 1)) + TestAllowance + HeatTreatmentAllowance;
            return (h1, h2);
        }

        private (string formatted, double nominal, double max) CalculateHeight(double height)
        {
            var (allowance, delta) = _allowanceService.FindAllowanceAndDelta((int)OuterDiameter, (int)height);
            double nominal = height + allowance;
            double max = nominal + delta;
            return ($"{nominal} ± {delta}", nominal, max);
        }

        private (string formatted, double nominal, double max) CalculateOuterDiameter(double heightNominal, double outerDiameterDetail)
        {
            var (allowance, delta) = _allowanceService.FindAllowanceAndDelta((int)OuterDiameter, (int)heightNominal);
            double nominal = outerDiameterDetail + allowance;
            double max = nominal + delta;
            return ($"{nominal} ± {delta}", nominal, max);
        }

        private (string formatted, double nominal, double max) CalculateInnerDiameter(double heightNominal, double innerDiameterDetail)
        {
            var (allowance, delta) = _allowanceService.FindAllowanceAndDelta((int)OuterDiameter, (int)heightNominal);
            double nominal = innerDiameterDetail - allowance;
            double max = nominal - 3 * delta;
            return ($"{nominal} ± {3 * delta}", nominal, max);
        }

        private (double nominal, double max) CalculateVolumes(
            double outerNominal, double innerNominal, double heightNominal,
            double outerMax, double innerMax, double heightMax)
        {
            double vNominal = CalculateCylinderVolume(outerNominal, heightNominal) - CalculateCylinderVolume(innerNominal, heightNominal);
            double vMax = CalculateCylinderVolume(outerMax, heightMax) - CalculateCylinderVolume(innerMax, heightMax);
            return (vNominal, vMax);
        }

        private double CalculateCylinderVolume(double diameter, double height)
        {
            return Math.PI * diameter * diameter * height / 4;
        }

        private (double nominal, double max) CalculateMasses(double volumeNominal, double volumeMax)
        {
            return (
                Math.Round(SpecificWeightOfSteel * volumeNominal, LoggingPrecision),
                Math.Round(SpecificWeightOfSteel * volumeMax, LoggingPrecision)
            );
        }

    }
}
