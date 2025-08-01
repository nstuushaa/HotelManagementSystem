//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace HotelManagementSystem
{
    using System;
    using System.Collections.Generic;
    
    public partial class Rooms
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Rooms()
        {
            this.Reservations = new HashSet<Reservations>();
            this.RoomEquipment = new HashSet<RoomEquipment>();
        }
    
        public int ID { get; set; }
        public Nullable<int> RoomNumber { get; set; }
        public Nullable<int> TypeID { get; set; }
        public Nullable<decimal> Price { get; set; }
        public Nullable<bool> Availability { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Reservations> Reservations { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RoomEquipment> RoomEquipment { get; set; }
        public virtual RoomTypes RoomTypes { get; set; }
    }
}
