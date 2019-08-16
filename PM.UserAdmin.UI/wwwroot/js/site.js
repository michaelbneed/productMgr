function swapElements(id1, id2, idSelect) {
    var mySelectedState = document.getElementById(idSelect).checked;
    var btnOp1 = document.getElementById(id1);
    var btnOp2 = document.getElementById(id2);
    
    if (mySelectedState === true) {
        btnOp1.style.display = "block";
        btnOp2.style.display = "none";
    }
    if (mySelectedState === false) {
        btnOp2.style.display = "block";
        btnOp1.style.display = "none";
    }
}

function getCost() {
    var dividend = document.getElementById("quantityEntered");
    var divider = document.getElementById("costEntered");
    var product = divider.value / dividend.value;
    var roundedProduct = Math.round(100 * product) / 100;
    document.getElementById("costAltPackage").innerHTML = "Unit Cost " + roundedProduct;
}