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
    
    public partial class Survey
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Survey()
        {
            this.Questions = new HashSet<Question>();
            this.SurveyResponses = new HashSet<SurveyResponse>();
        }
    
        public int SurveyID { get; set; }
        public Nullable<int> UserID { get; set; }
        public string SurveyTitle { get; set; }
        public System.DateTime EndDate { get; set; }
        public string SurveyUrl { get; set; }
        public Nullable<int> TimeLimit { get; set; }
        public Nullable<int> NoOfView { get; set; }
        public Nullable<int> NoOfResponses { get; set; }
        public Nullable<bool> Status { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Question> Questions { get; set; }
        public virtual User User { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SurveyResponse> SurveyResponses { get; set; }
    }
}