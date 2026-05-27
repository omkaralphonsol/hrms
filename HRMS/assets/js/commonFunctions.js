function toggleFilterDrawer() {
    var filterDrawer = document.getElementById("filterDrawer");
    filterDrawer.classList.toggle("show");
    return false;
}

function closeFilterDrawer() {
    var filterDrawer = document.getElementById("filterDrawer");
    filterDrawer.classList.remove("show");
    return false;
}
function handleDropdownChange() {
    return false;
}
function updateDrawerState(isOpen) {
    var hdnDrawerState = document.getElementById('<%= hdnDrawerState.ClientID %>');
    hdnDrawerState.value = isOpen ? 'open' : 'closed';
}

