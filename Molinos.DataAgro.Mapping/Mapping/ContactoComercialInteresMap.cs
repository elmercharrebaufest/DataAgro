
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Molinos.DataAgro.Entities
{
    public class ContactoComercialInteresMap : EntityTypeConfiguration<ContactoComercialInteres>
    {
        public ContactoComercialInteresMap()
        {

            // Properties
            // Table & Column Mappings
            this.ToTable("ContactoComercialInteres");
            this.Property(t => t.ContactoComercialInteresId).HasColumnName("ContactoComercialInteresId");
            this.Property(t => t.ContactoComercialId).HasColumnName("ContactoComercialId");
            this.Property(t => t.NroItem).HasColumnName("NroItem");
            this.Property(t => t.InteresId).HasColumnName("InteresId");
        }
    }
}


