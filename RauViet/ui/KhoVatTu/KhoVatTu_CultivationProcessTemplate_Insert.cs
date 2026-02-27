using RauViet.classes;
using System;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media.Media3D;
using Color = System.Drawing.Color;

namespace RauViet.ui
{
    public partial class KhoVatTu_CultivationProcessTemplate_Insert : Form
    {
        System.Data.DataTable mExcelData;

        public KhoVatTu_CultivationProcessTemplate_Insert()
        {
            InitializeComponent();
        }

        private async void LuuThayDoiBtn_Click(object sender, EventArgs e)
        {
            foreach (DataRow item in mExcelData.Rows)
            {
                int SKU = Convert.ToInt32(item["Column1"]);
                int CultivationTypeID = Convert.ToInt32(item["Column2"]);
                int FertilizationWorkTypeID = Convert.ToInt32(item["Column3"]);
                int WorkTypeID = Convert.ToInt32(item["Column4"]);
                string BaseDateType = Convert.ToString(item["Column5"]);
                int DaysAfter = Convert.ToInt32(item["Column6"]);
                int? MaterialID = null;

                if(!string.IsNullOrEmpty(item["Column7"].ToString().Trim()))
                    MaterialID = Convert.ToInt32(item["Column7"]);

                decimal MaterialQuantity = Convert.ToDecimal(item["Column8"]);
                decimal WaterAmount = Convert.ToDecimal(item["Column9"]);

                int newId = await SQLManager_KhoVatTu.Instance.insertCultivationProcessTemplateAsync(SKU, CultivationTypeID, BaseDateType, DaysAfter, FertilizationWorkTypeID, WorkTypeID, MaterialID, MaterialQuantity, WaterAmount);
                if(newId <= 0)
                {
                    MessageBox.Show("Error");
                    return;
                }
            }
            MessageBox.Show("Success");
        }

        private async void open_btn_Click(object sender, EventArgs e)
        {
            using (System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog())
            {
                ofd.Title = "Chọn file Excel";
                ofd.Filter = "Excel Files|*.xlsx;*.xls|All Files|*.*";
                ofd.Multiselect = false; // chỉ cho chọn 1 file

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    var workTypeTask = SQLStore_KhoVatTu.Instance.GetWorkTypeAsync();
                    var CultivationTypeTask = SQLStore_KhoVatTu.Instance.GetCultivationTypeAsync();
                    var materialTask = SQLStore_KhoVatTu.Instance.getMaterialAsync();

                    await Task.WhenAll(workTypeTask, CultivationTypeTask, materialTask);

                    string filePath = ofd.FileName;
                    mExcelData = Utils.LoadExcel_NoHeader(filePath);
                    dataGV.DataSource = mExcelData;

                    foreach(DataRow item in mExcelData.Rows)
                    {
                        string cultivationName_NS = Utils.RemoveVietnameseSigns(item["Column2"].ToString()).ToLower().Trim();
                        var cultivationRow = CultivationTypeTask.Result.AsEnumerable().FirstOrDefault(r =>Utils.RemoveVietnameseSigns(r.Field<string>("CultivationTypeName")).ToLower().Trim() == cultivationName_NS);
                        if (cultivationRow != null)
                        {
                            item["Column2"] = cultivationRow["CultivationTypeID"].ToString();
                        }

                        string fertilizationWorkName_NS = Utils.RemoveVietnameseSigns(item["Column3"].ToString()).ToLower().Trim();
                        var fertilizationWorkRow = workTypeTask.Result.AsEnumerable().FirstOrDefault(r => Utils.RemoveVietnameseSigns(r.Field<string>("WorkTypeName")).ToLower().Trim() == fertilizationWorkName_NS);
                        if (fertilizationWorkRow != null)
                        {
                            item["Column3"] = fertilizationWorkRow["WorkTypeID"].ToString();
                        }

                        string workTypeName_NS = Utils.RemoveVietnameseSigns(item["Column4"].ToString()).ToLower().Trim();
                        var workTypeRow = workTypeTask.Result.AsEnumerable().FirstOrDefault(r => Utils.RemoveVietnameseSigns(r.Field<string>("WorkTypeName")).ToLower().Trim() == workTypeName_NS);
                        if (workTypeRow != null)
                        {
                            item["Column4"] = workTypeRow["WorkTypeID"].ToString();
                        }

                        string materialName_NS = Utils.RemoveVietnameseSigns(item["Column7"].ToString()).ToLower().Trim();
                        var materialRow = materialTask.Result.AsEnumerable().FirstOrDefault(r => Utils.RemoveVietnameseSigns(r.Field<string>("MaterialName")).ToLower().Trim() == materialName_NS);
                        if (materialRow != null)
                        {
                            item["Column7"] = materialRow["MaterialID"].ToString();
                        }
                    }
                }
            }
        }

    }
}
