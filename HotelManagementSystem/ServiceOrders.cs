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
    
    public partial class ServiceOrders
    {
        public int ID { get; set; }
        public Nullable<int> GuestID { get; set; }
        public Nullable<int> ServiceID { get; set; }
        public Nullable<System.DateTime> OrderDate { get; set; }
        public Nullable<int> StatusID { get; set; }
    
        public virtual Guests Guests { get; set; }
        public virtual Service Service { get; set; }
        public virtual Statuses Statuses { get; set; }
    }
}
