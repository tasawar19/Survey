using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Survey.Models
{
    public class AnswerViewModel
    {
        public int SurveyID { get; set; }
        public string SurveyName { get; set; }
        public List<Questions> Questions { get; set; }
    }

    public class Questions
    {
        public int? QuestionIndex { get; set; }
        public string QuestionText { get; set; }

        public List<QuestionOptions> QuestionOptions { get; set; }

    }

    public class QuestionOptions
    {
        public int QuestionOptionID { get; set; }
        public string OptionText { get; set; }
        public int AnswerCount { get; set; }
        public int MAnsCount { get; set; }
        public int FAnsCount { get; set; }
    }
}

