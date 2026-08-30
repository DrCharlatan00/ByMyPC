using System;
using System.Collections.Generic;
using System.Text;

namespace ByMyPc.Postgresql.CRUDModel.SmallModels
{
    public class MotherboardSmallDbModel
    {
        public MotherboardSmallDbModel(string name, string socket, bool isLive)
        {
            Name = name;
            Socket = socket;
            IsLive = isLive;
        }

        public string Name { get; set; } = "N/A";
        public string Socket { get; set; } = "N?A";
        public bool IsLive { get; set; } = false;

    }
}
