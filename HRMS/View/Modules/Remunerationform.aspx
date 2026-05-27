<%@ Page Title="" Language="C#" MasterPageFile="~/View/Layout/Site1.Master" AutoEventWireup="true" CodeBehind="Remunerationform.aspx.cs" Inherits="HRMS.View.Modules.Remunerationform" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="https://unpkg.com/boxicons@2.1.4/css/boxicons.min.css" rel="stylesheet" />
    <link href="https://cdn.jsdelivr.net/npm/flatpickr/dist/flatpickr.min.css" rel="stylesheet" />
    <link href="https://cdn.jsdelivr.net/npm/select2@4.1.0-rc.0/dist/css/select2.min.css" rel="stylesheet" />
    <script src="https://cdn.jsdelivr.net/npm/flatpickr"></script>
    <script src="https://cdn.jsdelivr.net/npm/select2@4.1.0-rc.0/dist/js/select2.min.js"></script>
    <style>
        .payroll-wrap { max-width: 1320px; margin: 0 auto; color: #1f2937; }
        .payroll-header { display: flex; justify-content: space-between; align-items: flex-start; gap: 12px; margin-bottom: 14px; }
        .payroll-title { margin: 0; font-size: 1.75rem; font-weight: 700; color: #111827; }
        .payroll-subtitle { margin-top: 4px; color: #6b7280; font-size: .95rem; }
        .sec-card { background: #fff; border: 1px solid #e5e7eb; border-radius: 12px; box-shadow: 0 4px 14px rgba(17,24,39,.05); padding: 16px; margin-bottom: 14px; }
        .sec-head { margin: 0 0 10px 0; font-size: 1.05rem; font-weight: 700; color: #1f2937; }
        .payroll-wrap label { font-size: 12px; font-weight: 600; color: #374151; margin-bottom: 6px; }
        .payroll-wrap .form-control, .payroll-wrap select, .payroll-wrap input[type='text'] { border: 1px solid #d1d5db; border-radius: 8px; min-height: 38px; font-size: 13px; box-shadow: none; }
        .split-grid { display: grid; grid-template-columns: 1fr 1fr; gap: 14px; }
        .split-card { border: 1px solid #e5e7eb; border-radius: 10px; background: #fff; padding: 10px; }
        .split-card.earn { border-top: 2px solid #16a34a; }
        .split-card.deduct { border-top: 2px solid #dc2626; }
        .split-header { display: flex; align-items: center; justify-content: space-between; margin-bottom: 8px; }
        .split-title { margin: 0; font-size: .95rem; font-weight: 700; }
        .split-card.earn .split-title { color: #166534; }
        .split-card.deduct .split-title { color: #991b1b; }
        .pay-table { width: 100%; border-collapse: collapse; border: 1px solid #e5e7eb; border-radius: 8px; overflow: hidden; }
        .pay-table th, .pay-table td { padding: 8px 10px; border-bottom: 1px solid #edf0f3; vertical-align: middle; font-size: 13px; }
        .pay-table th { background: #f9fafb; color: #374151; font-size: 12px; font-weight: 700; }
        .pay-table tr:last-child td { border-bottom: none; }
        .trash-btn { width: 24px; height: 24px; border-radius: 6px; border: 1px solid #fecaca; color: #dc2626; background: #fff; display: inline-flex; align-items: center; justify-content: center; font-size: 14px; }
        .accordion-btn { width: 100%; border: 1px solid #e5e7eb; border-radius: 8px; background: #fff; padding: 10px 12px; text-align: left; font-size: .95rem; font-weight: 600; color: #1f2937; display: flex; align-items: center; justify-content: space-between; }
        .appraisal-body { max-height: 0; overflow: hidden; transition: max-height .3s ease, opacity .3s ease; opacity: 0; }
        .appraisal-body.open { max-height: 420px; opacity: 1; margin-top: 10px; }
        .summary-row { display: grid; grid-template-columns: repeat(4, 1fr); border: 1px solid #e5e7eb; border-radius: 8px; overflow: hidden; background: #fff; }
        .summary-item { padding: 10px 12px; border-right: 1px solid #eceff3; }
        .summary-item:last-child { border-right: none; }
        .summary-label { display: block; font-size: 11px; color: #6b7280; margin-bottom: 2px; text-transform: uppercase; letter-spacing: .03em; }
        .summary-value { font-size: 15px; font-weight: 700; color: #111827; }
        .component-modal { position: fixed; inset: 0; display: none; z-index: 1080; background: rgba(17,24,39,.45); align-items: center; justify-content: center; padding: 16px; }
        .component-card { width: min(560px,100%); background: #fff; border-radius: 10px; border: 1px solid #e5e7eb; box-shadow: 0 14px 34px rgba(17,24,39,.2); }
        .component-head { padding: 10px 12px; border-bottom: 1px solid #edf0f3; display: flex; align-items: center; justify-content: space-between; }
        .component-title { margin: 0; font-size: .95rem; font-weight: 700; color: #111827; }
        .component-body { padding: 12px; }
        .hide-legacy { display: none !important; }
        @media (max-width: 1024px) { .split-grid, .summary-row { grid-template-columns: 1fr; } .payroll-header { flex-direction: column; align-items: flex-start; } .summary-item { border-right: none; border-bottom: 1px solid #eceff3; } .summary-item:last-child { border-bottom: none; } }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:HiddenField ID="hfComponentOptions" runat="server" />
    <asp:HiddenField ID="hfDynamicComponents" runat="server" />
    <div class="payroll-wrap">
        <div class="payroll-header">
            <div>
                <h2 class="payroll-title">Remuneration Form</h2>
            </div>
        </div>

        <div class="sec-card">
            <div class="sec-head">Employee Details</div>
            <div class="row g-3">
                <div class="col-md-3"><label>Employee Name</label><asp:DropDownList ID="ddl_username" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddl_employeename_SelectedIndexChanged" /></div>
                <div class="col-md-3"><label>Employee Code</label><asp:DropDownList ID="ddl_empcode" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddl_employeeCode_SelectedIndexChanged" /></div>
                <div class="col-md-2"><label>Designation</label><asp:DropDownList ID="ddldesign" runat="server" CssClass="form-control" AutoPostBack="true" /></div>
                <div class="col-md-2"><label>Month</label><asp:DropDownList ID="ddlMonth" runat="server" CssClass="form-control" /></div>
                <div class="col-md-2"><label>Year</label><asp:DropDownList ID="ddlYear" runat="server" CssClass="form-control" /></div>
            </div>
        </div>

        <div class="sec-card">
            <div class="sec-head">Salary Components</div>
            <div class="split-grid">
                <div class="split-card earn">
                    <div class="split-header"><h4 class="split-title">Earnings</h4><button type="button" class="btn btn-outline-success btn-sm" onclick="openComponentModal('Earning')">+ Add Earning Component</button></div>
                    <table class="pay-table"><thead><tr><th>Component Name</th><th>Amount</th><th style="width:44px;"></th></tr></thead><tbody id="earningsBody">
                        <tr><td>Basic Salary</td><td><asp:TextBox ID="txtBasicSalary" runat="server" CssClass="form-control text-end" onkeyup="calculateSalary()" /></td><td></td></tr>
                        <tr><td>House Rent Allowance</td><td><asp:TextBox ID="txtHRA" runat="server" CssClass="form-control text-end" onkeyup="calculateSalary()" /></td><td></td></tr>
                        <tr><td>Special Allowance</td><td><asp:TextBox ID="txtSpecial" runat="server" CssClass="form-control text-end" onkeyup="calculateSalary()" /></td><td></td></tr>
                    </tbody></table>
                </div>
                <div class="split-card deduct">
                    <div class="split-header"><h4 class="split-title">Deductions</h4><button type="button" class="btn btn-outline-danger btn-sm" onclick="openComponentModal('Deduction')">+ Add Deduction Component</button></div>
                    <table class="pay-table"><thead><tr><th>Component Name</th><th>Amount</th><th style="width:44px;"></th></tr></thead><tbody id="deductionsBody">
                        <tr><td>Professional Tax</td><td><asp:TextBox ID="txtProfessionalTax" runat="server" CssClass="form-control text-end" onkeyup="calculateSalary()" /></td><td></td></tr>
                    </tbody></table>
                </div>
            </div>
            <asp:UpdatePanel ID="upAdditionalComponents" runat="server" UpdateMode="Conditional" CssClass="hide-legacy"><ContentTemplate><asp:Panel ID="pnlAdditionalComponents" runat="server" Visible="false"><asp:DropDownList ID="ddl_component" runat="server" CssClass="form-control" /><asp:Button ID="btnAddComponent" runat="server" Text="Add" OnClick="btnAddSalaryComponent_Click" /><asp:Repeater ID="rptComponents" runat="server"><ItemTemplate><div class="legacy-row" data-name='<%# Eval("ComponentName") %>' data-deduction='<%# Eval("IsDeduction") %>'></div></ItemTemplate></asp:Repeater></asp:Panel></ContentTemplate><Triggers><asp:AsyncPostBackTrigger ControlID="btnShowAdditional" EventName="Click" /></Triggers></asp:UpdatePanel>
            <div class="d-none"><asp:Button ID="btnShowAdditional" runat="server" Text="Hidden Add" OnClick="btnShowAdditional_Click" /></div>
        </div>

        <div class="sec-card">
            <div class="sec-head">Appraisal Details</div>
            <button type="button" class="accordion-btn" onclick="toggleAppraisal()">Appraisal Details <i class='bx bx-chevron-down' id='appChevron'></i></button>
            <div id="appraisalBody" class="appraisal-body"><asp:Panel ID="pnlAppraisal" runat="server" Visible="true"><div class="row g-3 mt-1"><div class="col-md-3"><label>Appraisal Type</label><select id="ddlAppraisalType" class="form-control" onchange="applyAppraisal();"><option value="Percentage">Percentage</option><option value="Fixed">Fixed</option></select></div><div class="col-md-3"><label>Appraisal Percentage (%)</label><asp:TextBox ID="txtAppraisalPercent" runat="server" CssClass="form-control text-end" onkeyup="applyAppraisal();" /></div><div class="col-md-3"><label>Appraisal Amount</label><asp:TextBox ID="txtIncrementAmount" runat="server" CssClass="form-control text-end" ReadOnly="true" /><asp:HiddenField ID="hfIncrementAmount" runat="server" /></div><div class="col-md-3"><label>Revised Salary</label><asp:TextBox ID="txtRevisedSalary" runat="server" CssClass="form-control text-end" ReadOnly="true" /></div></div></asp:Panel></div>
            <div class="d-none"><asp:Button ID="btnshowApprisal" runat="server" Text="Hidden App" OnClick="btnshowApprisal_Click" /><asp:TextBox runat="server" ID="txtLastAppraisalDate" CssClass="form-control" /><asp:TextBox runat="server" ID="txtCurrentAppraisalDate" CssClass="form-control" /></div>
        </div>

        <div class="sec-card">
            <div class="sec-head">Salary Summary</div>
            <div class="summary-row"><div class="summary-item"><span class="summary-label">Total Earnings</span><span class="summary-value" id="sumEarn">0.00</span></div><div class="summary-item"><span class="summary-label">Total Deductions</span><span class="summary-value" id="sumDeduct">0.00</span></div><div class="summary-item"><span class="summary-label">Net Pay</span><span class="summary-value" id="sumNet">0.00</span></div><div class="summary-item"><span class="summary-label">Revised Salary</span><span class="summary-value" id="sumRevised">0.00</span></div></div>
            <div class="d-none"><asp:TextBox ID="txtTotalEarnings" runat="server" /><asp:TextBox ID="txtTotalDeductions" runat="server" /><asp:TextBox ID="txtNetPay" runat="server" /><asp:TextBox ID="txtTotalEarningsSummary" runat="server" /><asp:TextBox ID="txtTotalDeductionsSummary" runat="server" /><asp:TextBox ID="txtNetPaySummary" runat="server" /></div>
        </div>

        <div class="d-flex justify-content-end gap-2 mb-2"><asp:Button ID="btnReset" runat="server" Text="Reset" CssClass="btn btn-outline-secondary" OnClick="btnReset_Click" /><asp:Button ID="btnSave" runat="server" Text="Save Remuneration" CssClass="btn btn-primary" OnClick="btn_saveremuneration_click" /></div>
    </div>

    <div class="component-modal" id="componentModal"><div class="component-card"><div class="component-head"><h5 class="component-title">Add Component</h5><button type="button" class="trash-btn" style="border-color:#d1d5db;color:#4b5563" onclick="closeComponentModal()">×</button></div><div class="component-body"><div class="row g-3"><div class="col-md-6" id="existingCompWrap"><label>Select Existing Component</label><asp:DropDownList ID="ddl_componentModal" runat="server" CssClass="form-control" /></div><div class="col-md-6" id="existingAmtWrap"><label>Amount</label><input type="text" id="txtComponentAmount" class="form-control text-end" value="0.00" /></div><div class="col-12"><button type="button" class="btn btn-link p-0" onclick="toggleCreateNew()">+ Create New Component</button></div><div class="col-md-6" id="newCompNameWrap" style="display:none;"><label>New Component Name</label><input type="text" id="txtNewCompName" class="form-control" placeholder="Enter component name" /></div><div class="col-md-6" id="newCompAmtWrap" style="display:none;"><label>Amount</label><input type="text" id="txtNewCompAmount" class="form-control text-end" value="0.00" /></div></div><div class="d-flex justify-content-end gap-2 mt-3"><button type="button" class="btn btn-outline-secondary" onclick="closeComponentModal()">Cancel</button><button type="button" class="btn btn-primary" id="btnAddExisting" onclick="addComponentFromModal()">Add Component</button><button type="button" class="btn btn-success" id="btnAddNew" style="display:none;" onclick="addNewComponentFromModal()">Save & Add</button></div></div></div></div>

    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11/dist/sweetalert2.all.min.js"></script>
    <script>
        function initEmployeeSearchDropdowns() {
            if (typeof $ === 'undefined' || !$.fn || !$.fn.select2) {
                return;
            }

            var $name = $('#<%= ddl_username.ClientID %>');
            var $code = $('#<%= ddl_empcode.ClientID %>');

            if ($name.length) {
                $name.select2({
                    width: '100%',
                    placeholder: 'Type name or code',
                    allowClear: true
                });
            }

            if ($code.length) {
                $code.select2({
                    width: '100%',
                    placeholder: 'Type code or name',
                    allowClear: true
                });
            }
        }

        let componentMode = 'Earning';
        function showsalarySavedMessage(status, remark) { Swal.fire({ icon: status === 'Success' ? 'success' : 'error', text: remark, timer: 4000, showConfirmButton: false }); }
        function toggleAppraisal() { const body = document.getElementById('appraisalBody'); const chev = document.getElementById('appChevron'); body.classList.toggle('open'); chev.classList.toggle('bx-chevron-up'); chev.classList.toggle('bx-chevron-down'); }
        function hydrateModalComponentsFromBackend() {
            var ddl = document.getElementById('<%= ddl_componentModal.ClientID %>');
            var hidden = document.getElementById('<%= hfComponentOptions.ClientID %>');
            if (!ddl || !hidden) return;

            var options = [];
            try { options = JSON.parse(hidden.value || "[]"); } catch (e) { options = []; }

            ddl.innerHTML = "";
            var placeholder = document.createElement("option");
            placeholder.value = "";
            placeholder.text = "-- Please Select --";
            ddl.appendChild(placeholder);

            for (var i = 0; i < options.length; i++) {
                var type = String(options[i].ComponentType || "").toUpperCase();
                var isDeductionType = (type === 'DEDUCTION' || type === 'DEDUCTIONS');
                var isEarningType = (type === 'EARNING' || type === 'EARNINGS');

                if (componentMode === 'Deduction' && !isDeductionType) continue;
                if (componentMode !== 'Deduction' && !isEarningType) continue;

                var opt = document.createElement("option");
                opt.value = String(options[i].Id || "");
                opt.text = options[i].Text || "";
                ddl.appendChild(opt);
            }
        }

        function openComponentModal(mode) {
            componentMode = mode;
            hydrateModalComponentsFromBackend();
            var ddl = document.getElementById('<%= ddl_componentModal.ClientID %>');
            console.log('ddl_componentModal options:', ddl ? ddl.options.length : 'null');
            document.getElementById('componentModal').style.display = 'flex';
            resetModalFields();
        }
        function closeComponentModal() { document.getElementById('componentModal').style.display = 'none'; resetModalFields(); }
        function toggleCreateNew() { const visible = document.getElementById('newCompNameWrap').style.display !== 'none'; document.getElementById('existingCompWrap').style.display = visible ? 'block' : 'none'; document.getElementById('existingAmtWrap').style.display = visible ? 'block' : 'none'; document.getElementById('newCompNameWrap').style.display = visible ? 'none' : 'block'; document.getElementById('newCompAmtWrap').style.display = visible ? 'none' : 'block'; document.getElementById('btnAddExisting').style.display = visible ? 'inline-block' : 'none'; document.getElementById('btnAddNew').style.display = visible ? 'none' : 'inline-block'; }
        function resetModalFields() { document.getElementById('txtComponentAmount').value = '0.00'; document.getElementById('txtNewCompName').value = ''; document.getElementById('txtNewCompAmount').value = '0.00'; document.getElementById('existingCompWrap').style.display = 'block'; document.getElementById('existingAmtWrap').style.display = 'block'; document.getElementById('newCompNameWrap').style.display = 'none'; document.getElementById('newCompAmtWrap').style.display = 'none'; document.getElementById('btnAddExisting').style.display = 'inline-block'; document.getElementById('btnAddNew').style.display = 'none'; }
        function createDynamicRow(name, amount, isDeduction) { const tbody = isDeduction ? document.getElementById('deductionsBody') : document.getElementById('earningsBody'); const tr = document.createElement('tr'); tr.className = 'dynamic-component'; tr.setAttribute('data-deduction', isDeduction ? 'True' : 'False'); tr.setAttribute('data-name', name); tr.innerHTML = '<td>' + name + '</td>' + '<td><input type="text" class="form-control text-end salary-amount" value="' + amount.toFixed(2) + '" onkeyup="calculateSalary()"></td>' + '<td class="text-center"><button type="button" class="trash-btn" onclick="removeRow(this)"><i class="bx bx-x"></i></button></td>'; tbody.appendChild(tr); }
        function hasComponent(name, isDeduction) { const tbody = isDeduction ? document.getElementById('deductionsBody') : document.getElementById('earningsBody'); return Array.from(tbody.querySelectorAll('tr.dynamic-component')).some(function (row) { return (row.getAttribute('data-name') || '').toLowerCase() === name.toLowerCase(); }); }
        function addComponentFromModal() { const ddl = document.getElementById('<%= ddl_componentModal.ClientID %>'); if (!ddl || !ddl.value) { showsalarySavedMessage('error', 'Please select existing component'); return; } const name = ddl.options[ddl.selectedIndex].text; if (hasComponent(name, componentMode === 'Deduction')) return; const amount = parseFloat(document.getElementById('txtComponentAmount').value) || 0; createDynamicRow(name, amount, componentMode === 'Deduction'); calculateSalary(); closeComponentModal(); }
        async function addNewComponentFromModal() {
            const name = document.getElementById('txtNewCompName').value.trim();
            if (!name) { showsalarySavedMessage('error', 'Please enter component name'); return; }
            if (hasComponent(name, componentMode === 'Deduction')) return;
            const amount = parseFloat(document.getElementById('txtNewCompAmount').value) || 0;
            const dbType = componentMode === 'Deduction' ? 'DEDUCTION' : 'EARNING';
            const appendLocally = function () {
                const hidden = document.getElementById('<%= hfComponentOptions.ClientID %>');
                let options = [];
                try { options = JSON.parse(hidden.value || "[]"); } catch (e) { options = []; }
                const exists = options.some(x => (String(x.Text || '')).toLowerCase() === name.toLowerCase());
                if (!exists) {
                    options.push({ Id: "", Text: name, ComponentType: dbType });
                    hidden.value = JSON.stringify(options);
                }
                createDynamicRow(name, amount, componentMode === 'Deduction');
                calculateSalary();
                closeComponentModal();
            };

            try {
                const response = await fetch('Remunerationform.aspx/SaveNewSalaryComponent', {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json; charset=utf-8' },
                    body: JSON.stringify({ componentName: name, componentType: dbType })
                });
                if (!response.ok) {
                    appendLocally();
                    showsalarySavedMessage('Success', 'Component added. It will be persisted on Save Remuneration.');
                    return;
                }
                const payload = await response.json();
                const result = payload && payload.d ? payload.d : null;
                if (!result || !result.success) {
                    appendLocally();
                    showsalarySavedMessage('Success', 'Component added. It will be persisted on Save Remuneration.');
                    return;
                }

                // Reload dropdown source in-memory so the new item appears immediately in popup list.
                const hidden = document.getElementById('<%= hfComponentOptions.ClientID %>');
                let options = [];
                try { options = JSON.parse(hidden.value || "[]"); } catch (e) { options = []; }
                const exists = options.some(x => (String(x.Text || '')).toLowerCase() === name.toLowerCase());
                if (!exists) {
                    options.push({ Id: String(result.id || ''), Text: String(result.text || name), ComponentType: dbType });
                    hidden.value = JSON.stringify(options);
                }

                createDynamicRow(name, amount, componentMode === 'Deduction');
                calculateSalary();
                closeComponentModal();
                showsalarySavedMessage('Success', result.message || 'Component created successfully');
            } catch (e) {
                appendLocally();
                showsalarySavedMessage('Success', 'Component added. It will be persisted on Save Remuneration.');
            }
        }
        function removeRow(button) { const row = button.closest('tr'); if (row) row.remove(); calculateSalary(); }
        function syncDynamicComponentsToHidden() {
            var rows = document.querySelectorAll('tr.dynamic-component');
            var items = [];
            rows.forEach(function (row) {
                var name = row.getAttribute('data-name') || '';
                var isDeduction = (row.getAttribute('data-deduction') === 'True');
                var input = row.querySelector('.salary-amount');
                var amount = parseFloat(input ? input.value : '0') || 0;
                items.push({ ComponentName: name, Amount: amount, IsDeduction: isDeduction });
            });
            var hf = document.getElementById('<%= hfDynamicComponents.ClientID %>');
            if (hf) hf.value = JSON.stringify(items);
        }

        function calculateSalary() { const basic = parseFloat(document.getElementById('<%= txtBasicSalary.ClientID %>').value) || 0; const hra = parseFloat(document.getElementById('<%= txtHRA.ClientID %>').value) || 0; const special = parseFloat(document.getElementById('<%= txtSpecial.ClientID %>').value) || 0; const professionalTax = parseFloat(document.getElementById('<%= txtProfessionalTax.ClientID %>').value) || 0; let totalEarnings = basic + hra + special; let totalDeductions = professionalTax; document.querySelectorAll('.salary-amount').forEach(function (input) { const amount = parseFloat(input.value) || 0; const tr = input.closest('tr'); const isDeduction = tr && tr.getAttribute('data-deduction') === 'True'; if (isDeduction) totalDeductions += amount; else totalEarnings += amount; }); const netPay = totalEarnings - totalDeductions; document.getElementById('<%= txtTotalEarnings.ClientID %>').value = totalEarnings.toFixed(2); document.getElementById('<%= txtTotalDeductions.ClientID %>').value = totalDeductions.toFixed(2); document.getElementById('<%= txtNetPay.ClientID %>').value = netPay.toFixed(2); document.getElementById('<%= txtTotalEarningsSummary.ClientID %>').value = totalEarnings.toFixed(2); document.getElementById('<%= txtTotalDeductionsSummary.ClientID %>').value = totalDeductions.toFixed(2); document.getElementById('<%= txtNetPaySummary.ClientID %>').value = netPay.toFixed(2); document.getElementById('sumEarn').textContent = totalEarnings.toFixed(2); document.getElementById('sumDeduct').textContent = totalDeductions.toFixed(2); document.getElementById('sumNet').textContent = netPay.toFixed(2); const revisedBox = document.getElementById('<%= txtRevisedSalary.ClientID %>'); document.getElementById('sumRevised').textContent = ((parseFloat(revisedBox ? revisedBox.value : '0') || 0)).toFixed(2); syncDynamicComponentsToHidden(); }
        function applyAppraisal() { const basicBox = document.getElementById('<%= txtBasicSalary.ClientID %>'); const percentBox = document.getElementById('<%= txtAppraisalPercent.ClientID %>'); const incrementBox = document.getElementById('<%= txtIncrementAmount.ClientID %>'); const hiddenField = document.getElementById('<%= hfIncrementAmount.ClientID %>'); const revised = document.getElementById('<%= txtRevisedSalary.ClientID %>'); const originalBasic = parseFloat(basicBox.getAttribute('data-original')) || parseFloat(basicBox.value) || 0; const type = document.getElementById('ddlAppraisalType').value; const entered = parseFloat(percentBox.value) || 0; let increment = 0; if (type === 'Fixed') { increment = entered; } else { increment = (originalBasic * entered) / 100; } incrementBox.value = increment.toFixed(2); hiddenField.value = increment.toFixed(2); basicBox.value = (originalBasic + increment).toFixed(2); if (revised) revised.value = (originalBasic + increment).toFixed(2); calculateSalary(); }
        window.onload = function () { const basicBox = document.getElementById('<%= txtBasicSalary.ClientID %>'); if (basicBox && !basicBox.getAttribute('data-original')) basicBox.setAttribute('data-original', basicBox.value || '0'); calculateSalary(); };
        document.addEventListener('DOMContentLoaded', function () { flatpickr('#<%= txtLastAppraisalDate.ClientID %>', { dateFormat: 'd-m-Y', allowInput: true }); flatpickr('#<%= txtCurrentAppraisalDate.ClientID %>', { dateFormat: 'd-m-Y', allowInput: true }); initEmployeeSearchDropdowns(); });
        if (typeof Sys !== 'undefined' && Sys.WebForms && Sys.WebForms.PageRequestManager) {
            Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
                initEmployeeSearchDropdowns();
            });
        }
    </script>
</asp:Content>
