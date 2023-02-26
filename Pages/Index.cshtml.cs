using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.ML;
using static EmailSpamModel_ConsoleApp1.EmailSpamModel;

namespace EmailSpamWebApp.Pages
{
    public class IndexModel : PageModel
    {
        private readonly PredictionEnginePool<ModelInput, ModelOutput> _predictionEnginePool;

        public IndexModel(PredictionEnginePool<ModelInput, ModelOutput> predictionEnginePool)
        {
            _predictionEnginePool = predictionEnginePool;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            string emailSubject = Request.Form["emailSubject"].ToString();
            string emailBody = Request.Form["emailBody"].ToString();

            if (
                (emailSubject == null || emailSubject.Length == 0) &&
                (emailBody == null || emailBody.Length == 0)
                ) return Page();

            string emailContents = emailSubject + " " + emailBody; 
            emailContents = emailContents.Replace("\n", "").Replace("\r", "");

            var input = new ModelInput { Body = emailContents };

            var prediction = _predictionEnginePool.Predict(input);

            var predictionResult = Convert.ToBoolean(prediction.PredictedLabel) ? "SPAM" : "NOT SPAM";
            var predictionConfidence = (prediction.Score.Max()*100).ToString("N2");

            TempData["predictionResult"] = predictionResult;
            TempData["predictionConfidence"] = predictionConfidence;

            return Page();
        }
    }
}