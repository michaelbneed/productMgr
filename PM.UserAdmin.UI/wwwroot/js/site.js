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

function getTotalCost() {
    var multiplier = document.getElementById("quantityEntered");
    var value = document.getElementById("costEntered");
    var product = multiplier.value * value.value;
    var roundedProduct = Math.round(100 * product) / 100;
    document.getElementById("totalCostAltPackage").innerHTML = "Total Cost " + roundedProduct;

}