//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace API_IMC.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class tblUsers
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tblUsers()
        {
            this.tblHistoricalEvaluations = new HashSet<tblHistoricalEvaluations>();
        }
    
        public int intPkIdUser { get; set; }
        public string strIdentification { get; set; }
        public string strUserName { get; set; }
        public string strPassword { get; set; }
        public string strName { get; set; }
        public string strLastName { get; set; }
        public int intFkIdRol { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblHistoricalEvaluations> tblHistoricalEvaluations { get; set; }
        public virtual tblRoles tblRoles { get; set; }
    }
}
