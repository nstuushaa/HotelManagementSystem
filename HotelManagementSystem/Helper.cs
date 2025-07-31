using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagementSystem
{
    /// Вспомогательный класс для работы с контекстом базы данных.
    public class Helper
    {
        public HotelManagement _context;

        public HotelManagement GetContext()
        { 
            if (_context == null)
                _context = new HotelManagement();
            return _context;
        }
    }
}
