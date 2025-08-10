using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CalculationModel.Models;

namespace CalculationModel.Pages
{
    public class ResultsPageModel : PageModel
    {
        // ���������� ��������

        public CalculationResult Result { get; set; }

        public void OnGet(CalculationResult result)
        {
            Result = result;
        }
    }
}
