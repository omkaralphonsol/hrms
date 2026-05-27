<%@ Page Title="Approval Configuration" Language="C#" MasterPageFile="~/View/Layout/Site1.Master" AutoEventWireup="true" %>
<%@ Import Namespace="System" %>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="System.Configuration" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.Linq" %>
<%@ Import Namespace="MySql.Data.MySqlClient" %>

<script runat="server">
    private string ConnStr
    {
        get
        {
            return ConfigurationManager.ConnectionStrings["MysqlConnection"] != null
                ? ConfigurationManager.ConnectionStrings["MysqlConnection"].ConnectionString
                : string.Empty;
        }
    }

    [Serializable]
    protected class ApprovalOptionItem
    {
        public int Id { get; set; }
        public string Text { get; set; }
    }

    [Serializable]
    protected class ConfigRowItem
    {
        public int DayId { get; set; }
        public int TypeId { get; set; }
        public string TypeName { get; set; }
        public int ApprovalOptionId { get; set; }
        public int ApprovalDays { get; set; }
        public bool IsSaved { get; set; }
    }

    private List<ApprovalOptionItem> ApprovalOptions
    {
        get { return (List<ApprovalOptionItem>)(ViewState["ApprovalOptions"] ?? new List<ApprovalOptionItem>()); }
        set { ViewState["ApprovalOptions"] = value; }
    }

    private List<ConfigRowItem> ConfigRows
    {
        get { return (List<ConfigRowItem>)(ViewState["ConfigRows"] ?? new List<ConfigRowItem>()); }
        set { ViewState["ConfigRows"] = value; }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["userId"] == null)
        {
            Response.Redirect("~/view/authentication/login.aspx", false);
            return;
        }

        if (!IsPostBack)
        {
            ApprovalOptions = GetApprovalOptionList();
            ConfigRows = BuildInlineRows();
            BindGrid();
        }
    }

    private int GetUserId()
    {
        int userId = 0;
        int.TryParse(Convert.ToString(Session["userId"]), out userId);
        return userId;
    }

    private int GetCompanyId()
    {
        int companyId = 0;
        int.TryParse(Convert.ToString(Session["company_id"]), out companyId);
        return companyId;
    }

    private MySqlCommand BuildCmd(MySqlConnection con, string type, int dayId, int typeId, int approvalDays, int approvalOptionId)
    {
        MySqlCommand cmd = new MySqlCommand("Sp_ManageApprovalConfiguration", con);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@p_type", type);
        cmd.Parameters.AddWithValue("@p_day_id", dayId);
        cmd.Parameters.AddWithValue("@p_company_id", GetCompanyId());
        cmd.Parameters.AddWithValue("@p_type_id", typeId);
        cmd.Parameters.AddWithValue("@p_approval_days", approvalDays);
        cmd.Parameters.AddWithValue("@p_approval_option_id", approvalOptionId);
        cmd.Parameters.AddWithValue("@p_inserted_by", GetUserId());
        return cmd;
    }

    private List<ApprovalOptionItem> GetApprovalOptionList()
    {
        var options = new List<ApprovalOptionItem>();
        using (MySqlConnection con = new MySqlConnection(ConnStr))
        {
            con.Open();
            using (MySqlCommand cmd = BuildCmd(con, "BindApprovalOption", 0, 0, 0, 0))
            using (MySqlDataReader dr = cmd.ExecuteReader())
            {
                while (dr.Read())
                {
                    options.Add(new ApprovalOptionItem
                    {
                        Id = dr["Id"] != DBNull.Value ? Convert.ToInt32(dr["Id"]) : 0,
                        Text = dr["Text"] != DBNull.Value ? Convert.ToString(dr["Text"]) : string.Empty
                    });
                }
            }
        }
        return options;
    }

    private List<ConfigRowItem> BuildInlineRows()
    {
        var types = new List<ConfigRowItem>();
        var existing = new List<ConfigRowItem>();

        using (MySqlConnection con = new MySqlConnection(ConnStr))
        {
            con.Open();

            using (MySqlCommand cmd = BuildCmd(con, "BindType", 0, 0, 0, 0))
            using (MySqlDataReader dr = cmd.ExecuteReader())
            {
                while (dr.Read())
                {
                    string name = dr["Text"] != DBNull.Value ? Convert.ToString(dr["Text"]) : string.Empty;
                    if (!string.Equals(name, "Leave", StringComparison.OrdinalIgnoreCase) &&
                        !string.Equals(name, "Resignation", StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    types.Add(new ConfigRowItem
                    {
                        DayId = 0,
                        TypeId = dr["Id"] != DBNull.Value ? Convert.ToInt32(dr["Id"]) : 0,
                        TypeName = name,
                        ApprovalOptionId = 0,
                        ApprovalDays = 0,
                        IsSaved = false
                    });
                }
            }

            using (MySqlCommand cmd = BuildCmd(con, "Get", 0, 0, 0, 0))
            using (MySqlDataReader dr = cmd.ExecuteReader())
            {
                while (dr.Read())
                {
                    existing.Add(new ConfigRowItem
                    {
                        DayId = dr["day_id"] != DBNull.Value ? Convert.ToInt32(dr["day_id"]) : 0,
                        TypeId = dr["type_id"] != DBNull.Value ? Convert.ToInt32(dr["type_id"]) : 0,
                        TypeName = dr["type_name"] != DBNull.Value ? Convert.ToString(dr["type_name"]) : string.Empty,
                        ApprovalOptionId = dr["approval_option_id"] != DBNull.Value ? Convert.ToInt32(dr["approval_option_id"]) : 0,
                        ApprovalDays = dr["approval_days"] != DBNull.Value ? Convert.ToInt32(dr["approval_days"]) : 0,
                        IsSaved = true
                    });
                }
            }
        }

        var merged = new List<ConfigRowItem>();
        foreach (var t in types.OrderBy(x => x.TypeName))
        {
            var ex = existing.FirstOrDefault(x => x.TypeId == t.TypeId);
            if (ex != null)
            {
                merged.Add(ex);
            }
            else
            {
                merged.Add(t);
            }
        }
        return merged;
    }

    private void BindGrid()
    {
        gvInlineConfig.DataSource = ConfigRows;
        gvInlineConfig.DataBind();
    }

    protected void gvInlineConfig_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType != DataControlRowType.DataRow)
        {
            return;
        }

        var rowData = (ConfigRowItem)e.Row.DataItem;

        var ddlType = (DropDownList)e.Row.FindControl("ddlType");
        var ddlOption = (DropDownList)e.Row.FindControl("ddlApprovalOption");
        var txtDays = (TextBox)e.Row.FindControl("txtApprovalDays");
        var btnAction = (Button)e.Row.FindControl("btnAction");
        var hfDayId = (HiddenField)e.Row.FindControl("hfDayId");
        var hfTypeId = (HiddenField)e.Row.FindControl("hfTypeId");

        ddlType.Items.Clear();
        ddlType.Items.Add(new ListItem(rowData.TypeName, rowData.TypeId.ToString()));
        ddlType.Enabled = false;

        ddlOption.Items.Clear();
        ddlOption.Items.Add(new ListItem("-- Please Select --", ""));
        foreach (var opt in ApprovalOptions)
        {
            ddlOption.Items.Add(new ListItem(opt.Text, opt.Id.ToString()));
        }
        if (rowData.ApprovalOptionId > 0)
        {
            var item = ddlOption.Items.FindByValue(rowData.ApprovalOptionId.ToString());
            if (item != null) ddlOption.SelectedValue = rowData.ApprovalOptionId.ToString();
        }

        txtDays.Text = rowData.ApprovalDays.ToString();
        hfDayId.Value = rowData.DayId.ToString();
        hfTypeId.Value = rowData.TypeId.ToString();

        // Keep one consistent button label in UI; backend still decides Insert vs Update by day_id.
        btnAction.Text = "Save";
        btnAction.CssClass = "btn btn-sm btn-primary cfg-btn";

        ApplyRowLogic(ddlOption, txtDays, btnAction);
    }

    protected void ddlApprovalOption_SelectedIndexChanged(object sender, EventArgs e)
    {
        DropDownList ddlOption = (DropDownList)sender;
        GridViewRow row = (GridViewRow)ddlOption.NamingContainer;
        var txtDays = (TextBox)row.FindControl("txtApprovalDays");
        var btnAction = (Button)row.FindControl("btnAction");
        ApplyRowLogic(ddlOption, txtDays, btnAction);
    }

    private void ApplyRowLogic(DropDownList ddlOption, TextBox txtDays, Button btnAction)
    {
        string text = ddlOption.SelectedItem != null ? ddlOption.SelectedItem.Text : string.Empty;
        bool isNo = string.Equals(text, "No", StringComparison.OrdinalIgnoreCase);
        bool hasValue = !string.IsNullOrWhiteSpace(ddlOption.SelectedValue);

        if (isNo)
        {
            txtDays.Text = "0";
            txtDays.Enabled = false;
            btnAction.Enabled = true; // allow saving/updating "No" state
            return;
        }

        txtDays.Enabled = hasValue;
        btnAction.Enabled = hasValue;
    }

    protected void btnAction_Click(object sender, EventArgs e)
    {
        lblMsg.Text = string.Empty;
        lblMsg.CssClass = string.Empty;

        if (GetCompanyId() <= 0)
        {
            lblMsg.Text = "Company context not found in session.";
            lblMsg.CssClass = "text-danger";
            return;
        }

        Button btn = (Button)sender;
        GridViewRow row = (GridViewRow)btn.NamingContainer;
        var ddlOption = (DropDownList)row.FindControl("ddlApprovalOption");
        var txtDays = (TextBox)row.FindControl("txtApprovalDays");
        var hfDayId = (HiddenField)row.FindControl("hfDayId");
        var hfTypeId = (HiddenField)row.FindControl("hfTypeId");

        int dayId = 0, typeId = 0, approvalOptionId = 0, approvalDays = 0;
        int.TryParse(hfDayId.Value, out dayId);
        int.TryParse(hfTypeId.Value, out typeId);
        int.TryParse(ddlOption.SelectedValue, out approvalOptionId);
        int.TryParse(txtDays.Text, out approvalDays);

        if (typeId <= 0 || approvalOptionId <= 0)
        {
            lblMsg.Text = "Please select approval option.";
            lblMsg.CssClass = "text-danger";
            return;
        }

        string selectedText = ddlOption.SelectedItem != null ? ddlOption.SelectedItem.Text : string.Empty;
        if (string.Equals(selectedText, "No", StringComparison.OrdinalIgnoreCase))
        {
            approvalDays = 0;
        }

        using (MySqlConnection con = new MySqlConnection(ConnStr))
        {
            con.Open();
            using (MySqlCommand cmd = BuildCmd(con, dayId > 0 ? "Update" : "Insert", dayId, typeId, approvalDays, approvalOptionId))
            using (MySqlDataReader dr = cmd.ExecuteReader())
            {
                if (dr.Read())
                {
                    string status = Convert.ToString(dr["Status"]);
                    string remarks = Convert.ToString(dr["Remarks"]);
                    lblMsg.Text = remarks;
                    lblMsg.CssClass = string.Equals(status, "Success", StringComparison.OrdinalIgnoreCase) ? "text-success" : "text-danger";
                }
            }
        }

        ApprovalOptions = GetApprovalOptionList();
        ConfigRows = BuildInlineRows();
        BindGrid();
    }
</script>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .cfg-wrap { max-width: 1100px; margin: 0 auto; }
        .cfg-title { margin: 0; font-size: 28px; font-weight: 700; color: #1e293b; }
        .cfg-sub { margin-top: 6px; color: #64748b; font-size: 14px; }
        .cfg-info { margin-top: 12px; border: 1px solid #bfdbfe; background: #eff6ff; color: #1e3a8a; border-radius: 10px; padding: 10px 12px; font-size: 13px; }
        .cfg-card { margin-top: 12px; background: #fff; border: 1px solid #e2e8f0; border-radius: 12px; box-shadow: 0 6px 18px rgba(15,23,42,0.06); padding: 14px; }
        .cfg-table th { background: #f8fafc; font-size: 12px; color: #334155; font-weight: 700; }
        .cfg-table td { vertical-align: middle; }
        .cfg-input { min-height: 36px; border: 1px solid #cbd5e1; border-radius: 8px; font-size: 13px; }
        .cfg-btn { border-radius: 8px; min-width: 80px; }
        .cfg-note { margin-top: 10px; font-size: 12px; color: #64748b; }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="cfg-wrap">
        <h2 class="cfg-title">Approval Configuration</h2>
        <div class="cfg-sub">Configure Leave and Resignation workflow approval settings.</div>
        <div class="cfg-info">Approval workflow will trigger after configured days.</div>

        <div class="cfg-card">
            <div class="table-responsive">
                <asp:GridView ID="gvInlineConfig" runat="server" CssClass="table table-bordered align-middle cfg-table"
                    AutoGenerateColumns="false"
                    OnRowDataBound="gvInlineConfig_RowDataBound">
                    <Columns>
                        <asp:TemplateField HeaderText="Type">
                            <ItemTemplate>
                                <asp:HiddenField ID="hfDayId" runat="server" />
                                <asp:HiddenField ID="hfTypeId" runat="server" />
                                <asp:DropDownList ID="ddlType" runat="server" CssClass="form-control cfg-input" />
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Approval Option">
                            <ItemTemplate>
                                <asp:DropDownList ID="ddlApprovalOption" runat="server" CssClass="form-control cfg-input"
                                    AutoPostBack="true" OnSelectedIndexChanged="ddlApprovalOption_SelectedIndexChanged" />
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Approval Days">
                            <ItemTemplate>
                                <asp:TextBox ID="txtApprovalDays" runat="server" CssClass="form-control cfg-input" TextMode="Number" />
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Action">
                            <ItemTemplate>
                                <asp:Button ID="btnAction" runat="server" Text="Save" OnClick="btnAction_Click" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </div>

            <div class="d-flex justify-content-between align-items-center">
                <asp:Label ID="lblMsg" runat="server" />
            </div>
            <div class="cfg-note">When Approval Option is 'No', Approval Days will become 0 and editing will be disabled.</div>
        </div>
    </div>
</asp:Content>
