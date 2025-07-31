using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagementSystem
{
    //Класс для приветсвия пользователя в определнное время суток
    public class GreetingClass
    {
        public string GreetingTime() 
        { 
            DateTime now = DateTime.Now;
            int hour = now.Hour;
            if (hour > 6 && hour < 12)
                return "Добрый вечер, ";
            if (hour > 12 && hour < 18)
                return "Добрый день, ";
            if (hour > 18 && hour < 21)
                return "Добрый вечер, ";
            else
                return "Доброй ночи, ";
        }
    }
}
