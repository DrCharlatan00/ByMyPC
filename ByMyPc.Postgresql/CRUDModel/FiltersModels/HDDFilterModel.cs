using ByMyPc.Postgresql.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ByMyPc.Postgresql.CRUDModel.FiltersModels
{
    public class HDDFilterModel
    {
        public HDDFilterModel()
        {
            
        }
        public HDDFilterModel(string? name, int? gbSize, HddConnector? connector)
        {
            Name = name;
            GbSize = gbSize;
            Connector = connector;
        }

        public string? Name { get; set; } = null;
        public int? GbSize { get; set; } = null;
        public HddConnector? Connector { get; set; } = null;
    }
}
