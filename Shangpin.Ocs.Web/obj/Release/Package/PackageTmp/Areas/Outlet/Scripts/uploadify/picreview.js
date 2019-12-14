function mousePos(e) {
    var x, y;
    var e = e || window.event;
    return { x: e.screenX, y: e.screenY };
}
function showPic(sUrl) {
    var x, y;

    y = $(window).height() / 2;
    x = ($(window).width() / 2)-150;
    //x = mousePos(e).x - 10;
    //y = mousePos(e).y - 10;
    document.getElementById("Layer1").style.left = x;
    document.getElementById("Layer1").style.top = y;
    document.getElementById("Layer1").innerHTML = "<img src=\"" + sUrl + "\">";
    document.getElementById("Layer1").style.display = "block";
}
function hiddenPic() {
    document.getElementById("Layer1").innerHTML = "";
    document.getElementById("Layer1").style.display = "none";
}