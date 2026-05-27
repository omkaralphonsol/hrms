document.addEventListener("DOMContentLoaded", function () {
    handleDropdownChange();
});

function handleDropdownChange() {
    console.log("handleDropdownChange function called");

    var dropdown = document.getElementById("ddlsearchp");
    var txtSearchRow = document.getElementById("txtSearchRow");
    var txtSearch = document.getElementById("txtSearch");

    console.log('Dropdown:', dropdown);
    console.log('txtSearchRow:', txtSearchRow);
    console.log('txtSearch:', txtSearch);

    if (txtSearch) {
        if (dropdown && dropdown.selectedIndex !== 0) {            
            var selectedOption = dropdown.options[dropdown.selectedIndex].text;

           
            if (isDynamicDropdown(selectedOption)) {
                console.log("Selected option requires a dynamic dropdown.");               
                generateDynamicDropdown(selectedOption);
                txtSearchRow.style.display = "none";
            } else {                
                txtSearchRow.style.display = "block";
                console.log("Dropdown selection changed. Textbox and label are now visible.");
                txtSearch.placeholder = "Enter " + selectedOption;
            }
        } else {            
            txtSearchRow.style.display = "none";
            console.log("Dropdown selection changed. Textbox and label are now hidden.");
            txtSearch.placeholder = "Enter..";
        }
    } else {
        console.error("Error: txtSearch element not found.");
    }
}
function isDynamicDropdown(option) {
    console.log("isDynamicDropdown function called");
    var isDynamic = option === "State" || option === "City" || option === "Gender";

    console.log("Is dynamic dropdown for option '" + option + "': " + isDynamic);

    return isDynamic;
}






function clearButtonClick() {
    var dropdown = document.getElementById('ddlsearchp');
    var txtSearchRow = document.getElementById('txtSearchRow');

    if (dropdown && txtSearchRow) {       
        dropdown.selectedIndex = 0;  
        txtSearch.value = "";
        txtSearchRow.style.display = "none";
    } else {
        console.error("Error: Dropdown or txtSearchRow element not found.");
    }
    return false;
}
