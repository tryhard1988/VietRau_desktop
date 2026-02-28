using RauViet.classes;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

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
                        if (fertilizationWorkName_NS.CompareTo(Utils.RemoveVietnameseSigns("Gieo xạ").ToLower()) == 0 || fertilizationWorkName_NS.CompareTo(Utils.RemoveVietnameseSigns("Gieo hạt").ToLower()) ==0)
                            fertilizationWorkName_NS = Utils.RemoveVietnameseSigns("Gieo sạ").ToLower().Trim();
                        else if (fertilizationWorkName_NS.StartsWith("thuc "))
                        {
                            fertilizationWorkName_NS = "bon " + fertilizationWorkName_NS;
                        }

                        var fertilizationWorkRow = workTypeTask.Result.AsEnumerable().FirstOrDefault(r => Utils.RemoveVietnameseSigns(r.Field<string>("WorkTypeName")).ToLower().Trim() == fertilizationWorkName_NS);
                        if (fertilizationWorkRow != null)
                        {
                            item["Column3"] = fertilizationWorkRow["WorkTypeID"].ToString();
                        }

                        string workTypeName_NS = Utils.RemoveVietnameseSigns(item["Column4"].ToString()).ToLower().Trim();
                        if (workTypeName_NS.CompareTo(Utils.RemoveVietnameseSigns("Gieo xạ").ToLower()) == 0 || workTypeName_NS.CompareTo(Utils.RemoveVietnameseSigns("Gieo hạt").ToLower()) == 0)
                            workTypeName_NS = Utils.RemoveVietnameseSigns("Gieo sạ").ToLower().Trim();

                        var workTypeRow = workTypeTask.Result.AsEnumerable().FirstOrDefault(r => Utils.RemoveVietnameseSigns(r.Field<string>("WorkTypeName")).ToLower().Trim() == workTypeName_NS);
                        if (workTypeRow != null)
                        {
                            item["Column4"] = workTypeRow["WorkTypeID"].ToString();
                        }

                        string materialName_NS = Utils.RemoveVietnameseSigns(item["Column7"].ToString()).ToLower().Trim();
                        if(materialName_NS.CompareTo(Utils.RemoveVietnameseSigns("so dua")) == 0 || materialName_NS.CompareTo(Utils.RemoveVietnameseSigns("Xơ dừa đã xử lý").ToLower()) == 0)
                            materialName_NS = Utils.RemoveVietnameseSigns("Xơ dừa thô").ToLower().Trim();
                        else if (materialName_NS.CompareTo(Utils.RemoveVietnameseSigns("YaraMila Winner").ToLower()) == 0)
                            materialName_NS = Utils.RemoveVietnameseSigns("YaraMila Winner (15-9-20)").ToLower().Trim();
                        else if (materialName_NS.CompareTo(Utils.RemoveVietnameseSigns("CYTOGREEN N36").ToLower()) == 0)
                            materialName_NS = Utils.RemoveVietnameseSigns("Phân bón Cytogreen N36").ToLower().Trim();
                        else if (materialName_NS.CompareTo(Utils.RemoveVietnameseSigns("Hạt giống Khổ Qua F1 TN 166").ToLower()) == 0)
                            materialName_NS = Utils.RemoveVietnameseSigns("Hạt giống khổ qua F1 SV 749").ToLower().Trim();
                        else if (materialName_NS.CompareTo(Utils.RemoveVietnameseSigns("Vôi mịn loại 1 CaO").ToLower()) == 0)
                            materialName_NS = Utils.RemoveVietnameseSigns("Vôi CaCO3").ToLower().Trim();
                        else if (materialName_NS.CompareTo(Utils.RemoveVietnameseSigns("DAP Phú Mỹ").ToLower()) == 0)
                            materialName_NS = Utils.RemoveVietnameseSigns("DAP 18-46 Vinacam").ToLower().Trim();
                        else if (materialName_NS.CompareTo(Utils.RemoveVietnameseSigns("Tricho endim").ToLower()) == 0)
                            materialName_NS = Utils.RemoveVietnameseSigns("Bio Pro TRICHO FG").ToLower().Trim();
                        else if (materialName_NS.CompareTo(Utils.RemoveVietnameseSigns("Phân bón hữu cơ DAWU 2").ToLower()) == 0)
                            materialName_NS = Utils.RemoveVietnameseSigns("Phân bón hữu cơ Mizufert 3,5-2-2 55 OM").ToLower().Trim();
                        else if (materialName_NS.CompareTo(Utils.RemoveVietnameseSigns("Fetrilon-combi").ToLower()) == 0)
                            materialName_NS = Utils.RemoveVietnameseSigns("Fetrilon combi").ToLower().Trim();
                        else if (materialName_NS.CompareTo(Utils.RemoveVietnameseSigns("Polyram").ToLower()) == 0)
                            materialName_NS = Utils.RemoveVietnameseSigns("Polyram 80WG").ToLower().Trim();
                        else if (materialName_NS.CompareTo(Utils.RemoveVietnameseSigns("Orande 280 SC").ToLower()) == 0)
                            materialName_NS = Utils.RemoveVietnameseSigns("Orande 280SC").ToLower().Trim();
                        else if (materialName_NS.CompareTo(Utils.RemoveVietnameseSigns("DAP 19-49-0 (SEU 55)").ToLower()) == 0)
                            materialName_NS = Utils.RemoveVietnameseSigns("DAP 18-46 Vinacam").ToLower().Trim();
                        else if (materialName_NS.CompareTo(Utils.RemoveVietnameseSigns("Phân bò ủ hoai").ToLower()) == 0)
                            materialName_NS = Utils.RemoveVietnameseSigns("Phân bò đã xử lý").ToLower().Trim();
                        else if (materialName_NS.CompareTo(Utils.RemoveVietnameseSigns("Magnesium sulphate (Green mag)").ToLower()) == 0)
                            materialName_NS = Utils.RemoveVietnameseSigns("BitterMag (MgSO4)").ToLower().Trim();
                        else if (materialName_NS.CompareTo(Utils.RemoveVietnameseSigns("Haifa MKP").ToLower()) == 0)
                            materialName_NS = Utils.RemoveVietnameseSigns("Nova Peak 0-52-34").ToLower().Trim();
                        else if (materialName_NS.CompareTo(Utils.RemoveVietnameseSigns("Humic acid powder").ToLower()) == 0)
                            materialName_NS = Utils.RemoveVietnameseSigns("Super Humic").ToLower().Trim();
                        else if (materialName_NS.CompareTo(Utils.RemoveVietnameseSigns("Phân gà vi sinh Nhật").ToLower()) == 0)
                            materialName_NS = Utils.RemoveVietnameseSigns("Phân gà hữu cơ Nhật AKI").ToLower().Trim();
                        else if (materialName_NS.CompareTo(Utils.RemoveVietnameseSigns("Phân trùn quế cao cấp SFARM pb01").ToLower()) == 0)
                            materialName_NS = Utils.RemoveVietnameseSigns("Phân trùn quế SFARM PB01").ToLower().Trim();
                        else if (materialName_NS.CompareTo(Utils.RemoveVietnameseSigns("NPK Phú Mỹ 20-7-7+TE").ToLower()) == 0)
                            materialName_NS = Utils.RemoveVietnameseSigns("NPK Phú Mỹ 16-16-8+13S+TE").ToLower().Trim();

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
