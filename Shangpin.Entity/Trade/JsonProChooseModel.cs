using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shangpin.Entity.Trade
{
    public class JsonProChooseModel
    {
        public string dataname { get; set; }
        public string sizename { get; set; }
        public IList<datalist> datalist { get; set; }
    }
    public class datalist
    {
        public string id { get; set; }
        public string type { get; set; }
        public string realquantity { get; set; }
        public string quantity { get; set; }
        public string value { get; set; }
        public string name { get; set; }
        public bool selected { get; set; }
        public string img { get; set; }
        public List<sizelist> sizelist { get; set; }
    }
    public class sizelist
    {
        public string size { get; set; }
        public string value { get; set; }
        public string desc { get; set; }
        public bool stock { get; set; }
        public bool selected { get; set; }
    }
}
