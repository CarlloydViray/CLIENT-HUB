using BPOI_HUB.model.apps.tools;
using BPOI_HUB.model.database;
using BPOI_HUB.model.user_management;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Npgsql;
using System.Data;

namespace BPOI_HUB.pages.apps.tools
{
    public class Client_surveyModel : PageModel
    {


        public User user;

        public POSTGRE_DB pg;

        public IActionResult OnGet()
        {
            if (HttpContext.Session.Keys.Any())
            {

                user = new()
                {
                    UserName = HttpContext.Session.GetString("username")
                };


                UserManager um = new();

                um.GetUserData(ref user);


                return Page();
            }
            else
            {

                user = new User();
            }
            return null;
        }

        public IActionResult OnPostSubmit(Client_survey_model result)
        {
            string connectionString = "Host=localhost;Username=postgres;Password=3029247;Database=hub_web";

            using NpgsqlConnection connection = new NpgsqlConnection(connectionString);
            connection.Open();

            using NpgsqlCommand cmd = new NpgsqlCommand("INSERT INTO survey(\"survey_clientName\", \"survey_ProactiveServiceRating\", \"survey_ProactiveServiceComment\", \"survey_SeamlessExperienceRating\", \"survey_SeamlessExperienceComment\", \"survey_DigitalFluencyRating\", \"survey_DigitalFluencyComment\", \"survey_SatisfactionRating\", \"survey_SatisfactionComment\", \"survey_SuggestionsComment\", \"survey_Date\") " +
                    "VALUES (@UserName, @ProactiveServiceRating, @ProactiveServiceComment, @SeamlessExperienceRating, @SeamlessExperienceComment, @DigitalFluencyRating, @DigitalFluencyComment, @SatisfactionRating, @SatisfactionComment, @SuggestionsComment,@Date)", connection);

            cmd.Parameters.AddWithValue("UserName", "DUMMY");
            cmd.Parameters.AddWithValue("ProactiveServiceRating", (object)result.ProactiveServiceRating ?? 0);
            cmd.Parameters.AddWithValue("ProactiveServiceComment", (object)result.ProactiveServiceComment ?? DBNull.Value);
            cmd.Parameters.AddWithValue("SeamlessExperienceRating", (object)result.SeamlessExperienceRating ?? 0);
            cmd.Parameters.AddWithValue("SeamlessExperienceComment", (object)result.SeamlessExperienceComment ?? DBNull.Value);
            cmd.Parameters.AddWithValue("DigitalFluencyRating", (object)result.DigitalFluencyRating ?? 0);
            cmd.Parameters.AddWithValue("DigitalFluencyComment", (object)result.DigitalFluencyComment ?? DBNull.Value);
            cmd.Parameters.AddWithValue("SatisfactionRating", (object)result.SatisfactionRating ?? 0);
            cmd.Parameters.AddWithValue("SatisfactionComment", (object)result.SatisfactionComment ?? DBNull.Value);
            cmd.Parameters.AddWithValue("SuggestionsComment", (object)result.SuggestionsComment ?? DBNull.Value);
            cmd.Parameters.AddWithValue("Date", DateTime.Now);

            cmd.ExecuteNonQuery();

            TempData["success"] = "Survey Submitted";
            return Page();
        }
    }
}
