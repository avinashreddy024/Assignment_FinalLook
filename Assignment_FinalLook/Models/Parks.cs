using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Assignment_FinalLook.Models
{
    public class Parks
    {


        public class Park
        {
            [Key]
            int Id { get; set; }
            string fullName { get; set; }

            string parkCode { get; set; }
            string url { get; set; }
            string stateCode { get; set; } //State code goes here

            string phonenumber { get; set; }

            string address { get; set; }

            List<Events> events { get; set; }


        }

        public class StateCodeToState
        {
            [Key]
            public string stateCode { get; set; }

            public string stateName { get; set; }
        }

        public class Events
        {
            [Key] 
            public int Id { get; set; }
            public Park park { get; set; }

            public string parkName { get; set; }

        }

        public class Activity
        {
            [Key]
            int Id { get; set; }

            string activityName { get; set; }

            public Park park { get; set; }
        }


    }
}
