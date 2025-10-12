using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vodo.Models
{
    public enum JobType
    {
        Excavation = 1,        // Земляные работы / раскопка
        PipelineRepair = 2,    // Ремонт трубопровода
        ValveReplacement = 3,  // Замена запорной арматуры (задвижек, клапанов и т. п.)
        Emergency = 4,         // Аварийные работы
        Inspection = 5,        // Осмотр / обследование объекта
        Other = 99             // Прочие работы
    }
}
