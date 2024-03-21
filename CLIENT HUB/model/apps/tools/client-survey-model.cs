using Microsoft.AspNetCore.Mvc;

namespace BPOI_HUB.model.apps.tools
{
    public class    Client_survey_model
    {
        [BindProperty]
        public string UserName { get; set; }

        [BindProperty]
        public int ProactiveServiceRating { get; set; }

        [BindProperty]
        public string ProactiveServiceComment { get; set;}

        [BindProperty]
        public int SeamlessExperienceRating { get; set; }

        [BindProperty]
        public string SeamlessExperienceComment { get; set; }

        [BindProperty]
        public int DigitalFluencyRating { get; set; }

        [BindProperty]
        public string DigitalFluencyComment { get; set; }

        [BindProperty]
        public int SatisfactionRating { get; set; }

        [BindProperty]
        public string SatisfactionComment { get; set; }

        [BindProperty]
        public string SuggestionsComment { get; set; }


    }
}
