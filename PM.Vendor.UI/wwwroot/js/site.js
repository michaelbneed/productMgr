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