using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

// DOCS: https://ee-erp11-test.excoeng.com/ERP11-TST/api/help/v2/index.html
namespace EpicorRestApiHelloWorld
{
    public partial class Form1 : Form
    {
        public Form1()
        {

            InitializeComponent();
            DataTable testTableFromNormalService = LoadDataFromNormalEpicorService();
            DataTable testTableFromCustomService = LoadDataFromCustomEpicorService();
            dataGridView1.DataSource = testTableFromNormalService;
            dataGridView2.DataSource = testTableFromCustomService;
        }

        private DataTable LoadDataFromNormalEpicorService()
        {
            string epicorService = "Erp.Bo.QuoteSvc/Quotes";
            int top = 20;
            string orderby = "QuoteNum";
            string fields = "QuoteNum, CustomerCustID";
            string parameterString = $"$select={fields}&$orderby={orderby} desc&$top={top}";

            string responseBody = EpicorApiFunctions.HitEpicorWithAGet(epicorService, parameterString);

            if (responseBody == null)
            {
                MessageBox.Show("No data received from Epicor.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }

            // var responseObject = JsonConvert.DeserializeObject<QuoteResponse>(responseBody);
            // Comment from Rodrigo: At this point you can deserialize the json into one of your objects if you're feeling fancy
            // On this example I just use the json to return a DataTable for clarity

            // Parse JSON into a JObject
            JObject obj = JObject.Parse(responseBody);

            // Epicor returns data inside the `value` property
            JArray rows = (JArray)obj["value"];

            // Convert JArray to DataTable
            DataTable table = JsonConvert.DeserializeObject<DataTable>(rows.ToString());
            return table;
        }

        private DataTable LoadDataFromCustomEpicorService()
        {
            // This requires a BAQ to be setup in Epicor for the query, helps with really complicated joins/filters etc, should also be a lot faster than fetching everything and messing with it in code.
            // Rodrigo, Shea or Adam can help setting this up in Epicor so your app just gets the table it needs, ask for a "BAQ" (Business Activity Query)
            
            string epicorService = "BaqSvc/RM-JobAsmblTest/Data"; // You can also do /$metadata if you want to be really fancy and fetch the table structure dynamically. 
            int top = 20;
            string orderby = "JobOper_StartDate";
            string fields = "JobOper_StartDate, JobAsmbl_JobNum, JobAsmbl_AssemblySeq, JobAsmbl_PartNum, JobOper_JobComplete, JobOper_OpComplete";
            string parameterString = $"$select={fields}&$orderby={orderby} desc&$top={top}";

            string responseBody = EpicorApiFunctions.HitEpicorWithAGet(epicorService, parameterString, useTestServer: false); // by passing false there you hit the prod server instead of the test Epicor server.

            if (responseBody == null)
            {
                MessageBox.Show("No data received from Epicor.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }

            // Parse JSON into a JObject
            JObject obj = JObject.Parse(responseBody);

            // Epicor returns data inside the `value` property
            JArray rows = (JArray)obj["value"];

            // Convert JArray to DataTable
            DataTable table = JsonConvert.DeserializeObject<DataTable>(rows.ToString());
            return table;
        }





        // OTHER STUFF
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
