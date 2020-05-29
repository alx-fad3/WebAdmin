using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Opener2.Models
{
    public class Agreement
    {
        public string CreatedOn { get; set; }
        public string Npf { get; set; }
        public string Snils { get; set; }
        public string ContactName { get; set; }
        public string Gender { get; set; }
        public string BirthDate { get; set; }
        public string Status { get; set; }
        public string Comment { get; set; }
        public string AgentUsername { get; set; }
        public string Agent { get; set; }
        public string Signer { get; set; }
        public string BusinessUnit { get; set; }
        public string SaleChannel { get; set; }


        public List<string> ToListForView()
        {
            return new List<string>()
            {
                "Статус: " + Status, "СНИЛС: " + Snils, "Дата: " + CreatedOn, "Фонд: " + Npf, "Клиент: " + ContactName, "Агент: " + Agent, "Агент: " + AgentUsername, "Подписант: " + Signer, "Канал продаж: " + SaleChannel, "ОП: " + BusinessUnit
            };
        }
    }

    public class AgreementForCharts
    {
        public string CreatedOn { get; set; }
        public string BusinessUnit { get; set; }
    }

    
}