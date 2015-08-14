using System;
using System.Data;

namespace GestionCommerciale.Views.Prints
{


    public partial class InvoiceDataSet
    {
        partial class DataTable1DataTable
        {

            public void FillTable(DataTable dataTable)
            {
                try
                {
                    if (dataTable != null && dataTable.Rows.Count > 0)
                    {
                        foreach (DataRow entry in dataTable.Rows)
                        {
                            DataRow row = NewRow();

                            row[0] = entry["Ordre"];
                            row[1] = entry["Designation"];
                            row[2] = entry["Intrepretation"];
                            Rows.Add(row);
                        }
                    }
                }
                catch (Exception e)
                {


                }
            }
        }
    }
}
