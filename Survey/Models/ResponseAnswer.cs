//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Survey.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class ResponseAnswer
    {
        public int ResponseAnswerID { get; set; }
        public Nullable<int> SurveyResponseID { get; set; }
        public Nullable<int> QuestionID { get; set; }
        public Nullable<int> QuestionOptionID { get; set; }
        public string FreeTextAnswer { get; set; }
    
        public virtual Question Question { get; set; }
        public virtual QuestionOption QuestionOption { get; set; }
        public virtual SurveyResponse SurveyResponse { get; set; }
    }
}
