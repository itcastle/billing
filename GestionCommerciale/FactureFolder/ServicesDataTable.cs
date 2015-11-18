using System.ComponentModel;
using System.Data;

namespace GestionCommerciale.FactureFolder 
{
    
    
    public partial class ServicesDataTable 
    {
        public partial class DataTable1DataTable 
        { 
            public void FillOrdonnanceTable(DataTable servicesDataTable)
            {
                if (servicesDataTable == null || servicesDataTable.Rows.Count <= 0) return;
                foreach (DataRow entry in servicesDataTable.Rows)
                {
                    var row = NewRow(); 
                    row[0] = entry["Ordre"];
                    row[1] = entry["Name"];
                    row[2] = entry["Price HT"];
                    row[3] = entry["Qte"];
                    row[4] = entry["TVA"];
                    row[5] = entry["Total TTC"]; 
                    Rows.Add(row);
                }
            }
        }

        private void InitializeComponent()
        {
            ((ISupportInitialize)(this)).BeginInit();
            ((ISupportInitialize)(this)).EndInit();

        }
    }
}

