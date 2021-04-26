using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Assignment_FinalLook.Models
{
        //The Following are the Database tabels that will be created.
        public class Park
        {
            [Key]
            public string parkId { get; set; }
            public string parkName { get; set; }

            public string Address { get; set; }
            public States State { get; set; } //State code goes here
            public string PhoneNuber { get; set; }

            public int EntryFee { get; set; }
            public string website { get; set; }

            public List<Events> events { get; set; }
            public List<Activities> activities { get; set; }

        }

        public class States
        {
            [Key]
            public string stateCode { get; set; }

            public string stateName { get; set; }
        }

        public class Events
        {
            [Key] 
            public int EventId { get; set; }
            public Park parkId { get; set; }

            public string EventName { get; set; }
            public int EntryFee { get; set; }

        }

        
        public class Activities
        {
            [Key]
            public int ActivityId { get; set; }

            public string ActivityName { get; set; }

            public Park parkId { get; set; }
            public int EntryFee { get; set; }
        }


        public class Users
         {   

        [Key]
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public string ParkName { get; set; }
        public string ActivityName { get; set; }
        public string state { get; set; }
        public string city { get; set; }
        }


    
}
